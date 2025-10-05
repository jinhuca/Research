using DataAccessLayer;
using DevExpress.Mvvm.Native;
using FileSerializer;
using MahApps.Metro.Controls;
using PDFReportsGenerator;
using Shared;
using SmartAblationSystem.Converters;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Communication;
using UniversalLoginManager;
using static Communication.CanBusMessageDefinition;
using static LogSystem.LogService;
using Action = System.Action;
using BindableBase = Prism.Mvvm.BindableBase;

namespace SmartAblationSystem.ViewModels
{
  public class TreatmentRecordsViewModel : BindableBase, IDisposable, IDataExportable
  {
    #region Fields

    private readonly PDFConversion _PDFConversion = new PDFConversion();
    private readonly PDFCaseReport _PDFCaseReport = new PDFCaseReport();
    private readonly JsonManager _JsonFileManager = new JsonManager();
    private readonly AblationDataExtractor _AblationDataExtractor = new AblationDataExtractor(null, null);
    private readonly USBDriveConnectionManager.USBDriveConnectionManager _usbDriveConnectionManager;
    private LoginManager.AccessControlType _accessControlType = LoginManager.AccessControlType.USER;
    private readonly DataAccess _dataAccess;

    #endregion Fields

    #region Constants

    private const string Dash = "-";
    private const string DoubleDash = "--";
    private const string DoctorTitle = "Dr. ";
    private const string Whitespace = " ";
    private const string ExportFolder = "PatientRecord";
    private const string Underscore = "_";

    #endregion Constants

    #region ProcedureRecords Collection Properties

    private List<ProcedureRecords> _allProcedures;

    private ObservableCollection<ProcedureRecords> _filteredProceduresList = new ObservableCollection<ProcedureRecords>();
    public ObservableCollection<ProcedureRecords> FilteredProcedureRecordsList
    {
      get => _filteredProceduresList;
      set => SetProperty(ref _filteredProceduresList, value);
    }

    private ObservableCollection<ProcedureRecords> _selectedProceduresList = new ObservableCollection<ProcedureRecords>();
    public ObservableCollection<ProcedureRecords> SelectedProceduresList
    {
      get => _selectedProceduresList;
      set => SetProperty(ref _selectedProceduresList, value);
    }

    private IDisposable _ProcedureRecordsListSubscription;

    #endregion ProcedureRecords Collection Properties

    public event EventHandler PlaybackModeEvent;
    public event EventHandler USBExportProgressEvent;
    private UserType _userType = UserType.Unknown;

    #region Sort Procedure Command

    private bool _sortIsAscendingColumn;
    public ICommand SortProcedureColumnCommand { get; }
    private void OnSortProcedureColumn(string columnName)
    {
      _sortIsAscendingColumn = !_sortIsAscendingColumn;
      switch(columnName)
      {
        case nameof(ProcedureDate):
          FilteredProcedureRecordsList = _sortIsAscendingColumn
              ? new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderBy(procedureRecords_ => procedureRecords_.ProcedureDate))
              : new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderByDescending(procedureRecords_ => procedureRecords_.ProcedureDate));
          break;
        case nameof(PatientFirstName):
          FilteredProcedureRecordsList = _sortIsAscendingColumn
              ? new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderBy(procedureRecords_ => procedureRecords_.PatientFirstName))
              : new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderByDescending(procedureRecords_ => procedureRecords_.PatientFirstName));
          break;
        case nameof(PatientLastName):
          FilteredProcedureRecordsList = _sortIsAscendingColumn
              ? new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderBy(procedureRecords_ => procedureRecords_.PatientLastName))
              : new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderByDescending(procedureRecords_ => procedureRecords_.PatientLastName));
          break;
        case nameof(ProcedureID):
          FilteredProcedureRecordsList = _sortIsAscendingColumn
              ? new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderBy(procedureRecords_ => procedureRecords_.Procedure.Id))
              : new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderByDescending(procedureRecords_ => procedureRecords_.Procedure.Id));
          break;
        case nameof(Physician):
          FilteredProcedureRecordsList = _sortIsAscendingColumn
              ? new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderBy(procedureRecords_ => procedureRecords_.Patient.Physician.Name))
              : new ObservableCollection<ProcedureRecords>(FilteredProcedureRecordsList.OrderByDescending(procedureRecords_ => procedureRecords_.Patient.Physician.Name));
          break;
      }
      this.SortVisualStateManager.SortColumn(columnName, _sortIsAscendingColumn);
    }

    public string ProcedureID => "ProcedureID";
    public string ProcedureDate => "ProcedureDate";
    public string PatientFirstName => "PatientFirstName";
    public string PatientLastName => "PatientLastName";
    public string Physician => "Physician";

    #endregion Sort Procedure Command

    #region Select All Procedures Command

    public ICommand SelectAllProceduresCommand { get; }

    private bool CanSelectAllProcedure(bool? arg) => true;

    public void OnSelectAllProcedures(bool? arg)
    {
      switch(arg)
      {
        case false:
          UnselectAll_();
          break;
        case true:
          SelectAll();
          break;
        case null:
          IndeterminateSelect();
          break;
      }

      void UnselectAll_()
      {
        foreach(var procedureRecords_ in FilteredProcedureRecordsList)
        {
          procedureRecords_.Selected = false;
        }
        AllSelected = false;
      }

      void SelectAll()
      {
        foreach(var procedureRecords_ in FilteredProcedureRecordsList)
        {
          procedureRecords_.Selected = true;
        }

        if(_allProcedures.All(pr_ => pr_.Selected))
        {
          AllSelected = true;
        }
        else if(FilteredProcedureRecordsList.All(p => p.Selected) && _allProcedures.Any(p => !p.Selected))
        {
          AllSelected = null;
        }
        else
        {
          AllSelected = false;
        }
      }

      void IndeterminateSelect()
      {
        foreach(var procedureRecords_ in FilteredProcedureRecordsList)
        {
          procedureRecords_.Selected = false;
        }
        AllSelected = false;
      }
    }

    private bool? _allSelected = false;
    public bool? AllSelected
    {
      get => _allSelected;
      set => SetProperty(ref _allSelected, value);
    }

    #endregion Select All Procedures Command

    #region Commands

    public ICommand AblationNumberForwardCommand { get; }
    public ICommand AblationNumberBackwardCommand { get; }
    public ICommand SaveToUSBCommand { get; }
    public ICommand PrintPDFCommand { get; }
    public ICommand ProcedureLogCommand { get; }
    public ICommand ClearFilterCommand { get; }
    private ICommand navigateToViewCommand;
    public ICommand NavigateToViewCommand
    {
      get => navigateToViewCommand;
      set => SetProperty(ref navigateToViewCommand, value);
    }
    #endregion Commands

    private const float _balloon31mmThreshold = 7.3f;

    public event EventHandler TipOrBalloonPressureSelectionChangedEvent;

    private FileAction fileAction;

    private CancellationTokenSource _cancellationTokenSource;
    public SortVisualStateManager SortVisualStateManager { get; } = new SortVisualStateManager();
    public TreatmentRecordsViewModel()
    {
      AblationList = new ObservableCollection<AblationReport>();

      ClearFilterCommand = new Prism.Commands.DelegateCommand<object>(OnClearFilterCommand, obj => true);
      SortProcedureColumnCommand = new Prism.Commands.DelegateCommand<string>(OnSortProcedureColumn, obj => true);
      AblationNumberForwardCommand = new Prism.Commands.DelegateCommand<object>(OnAblationNumberForward, CanAblationNumberForward);
      AblationNumberBackwardCommand = new Prism.Commands.DelegateCommand<object>(OnAblationNumberBackward, CanAblationNumberBackward);
      SaveToUSBCommand = new Prism.Commands.DelegateCommand<object>(OnSaveToUSBCommand, CanSaveToUSBCommand) 
        .ObservesProperty(() => AnyProcedureSelected);
      PrintPDFCommand = new Prism.Commands.DelegateCommand<object>(OnPrintPDFCommand, CanPrintPDFCommand)
          .ObservesProperty(() => AnyProcedureSelected);
      ProcedureLogCommand = new Prism.Commands.DelegateCommand<object>(OnProcedureLogCommand, CanProcedureLogCommand);
      SelectAllProceduresCommand = new Prism.Commands.DelegateCommand<bool?>(OnSelectAllProcedures, obj => true);

      _dataAccess = CommonViewModel.Current.Data.DataAccess;
      CommonViewModel.Current.PropertyChanged += CommonViewModel_PropertyChanged;

      _usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);
      USBDriveList = _usbDriveConnectionManager.GetUSBDriveList();
      IsExportingCurrentProcedure = false;
      PropertyChanged += TreatmentRecordsViewModel_PropertyChanged;
      GetUserType();
    }

    public async void InitializeProcedureListsAsync()
    {
      IsLoadingProcedureList = true;
      _allProcedures = await Task.Run(GetAllProcedures);
      FilteredProcedureRecordsList = new ObservableCollection<ProcedureRecords>(_allProcedures.OrderBy(pr_ => pr_.Procedure.ProcedureStartDateTime, ListSortDirection.Descending));

      _sortIsAscendingColumn = true;
      _ProcedureRecordsListSubscription = _allProcedures.ToObservable()
          .Select(x => Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                  handler => x.PropertyChanged += handler,
                  handler => x.PropertyChanged -= handler))
          .Merge()
          .Subscribe(ProcedureRecordsSelectionChanged);
      IsLoadingProcedureList = false;
      NavigatedProcedureRecords = FilteredProcedureRecordsList?.FirstOrDefault();
    }

    #region Event Handlers

    private void TreatmentRecordsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch(e.PropertyName)
      {
        case nameof(SelectedFromDate):
        case nameof(SelectedToDate):
        case nameof(LastNameSearch):
        case nameof(FirstNameSearch):
        case nameof(ProcedureIDSearch):
        case nameof(PatientIDSearch):
        case nameof(PhysicianSearch):
          FilterProcedureRecords();
          break;
        case nameof(NavigatedProcedureRecords):
          if (NavigatedProcedureRecords != null)
          {
            ExtractProcedureRecordsData(NavigatedProcedureRecords);
          }
          IsSummaryReportVisible = NavigatedProcedureRecords != null;
          break;
      }
    }

    private void CommonViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch(e.PropertyName)
      {
        case nameof(CommonViewModel.TC1Reading):
          RaisePropertyChanged(nameof(TC1Reading));
          RaisePropertyChanged(nameof(VeinIsolationDuration));
          break;
        case nameof(CommonViewModel.PT2Reading):
          RaisePropertyChanged(nameof(PT2Reading));
          break;
        case nameof(CommonViewModel.CP1Reading):
          RaisePropertyChanged(nameof(CP1Reading));
          break;
        case nameof(CommonViewModel.FM1Reading):
          RaisePropertyChanged(nameof(FM1Reading));
          break;
        case nameof(CommonViewModel.LC1Reading):
          RaisePropertyChanged(nameof(LC1Reading));
          break;
        case nameof(CommonViewModel.CurrentPatient):
          RaisePropertyChanged(nameof(CurrentPatient));
          break;
        case nameof(CommonViewModel.AblationSite):
          RaisePropertyChanged(nameof(AblationSite));
          break;
        case nameof(CommonViewModel.EcgChannel3And4Reading):
          RaisePropertyChanged(nameof(EcgChannel3And4Reading));
          break;
        case nameof(CommonViewModel.EcgChannel5And6Reading):
          RaisePropertyChanged(nameof(EcgChannel5And6Reading));
          RaisePropertyChanged(nameof(EsophagusTemperatureThresholdReached));
          break;
        case nameof(CommonViewModel.EcgChannel7And8Reading):
          RaisePropertyChanged(nameof(EcgChannel7And8Reading));
          break;
        case nameof(CommonViewModel.BloodDetecorImValue):
          RaisePropertyChanged(nameof(BloodDetecorImValue));
          break;
      }
    }

    private void ProcedureRecordsSelectionChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
    {
      var procedureRecord_ = (ProcedureRecords)eventPattern.Sender;
      if(procedureRecord_.Selected)
      {
        SelectedProceduresList?.Add(procedureRecord_);
      }
      else
      {
        SelectedProceduresList?.Remove(procedureRecord_);
      }

      if(_allProcedures.All(procedureRecords_ => procedureRecords_.Selected))
      {
        AllSelected = true;
      }
      else if(_allProcedures.All(procedureRecords_ => procedureRecords_.Selected == false))
      {
        AllSelected = false;
      }
      else
      {
        AllSelected = null;
      }

      RaisePropertyChanged(nameof(AnyProcedureSelected));
    }

    #endregion Event Handlers

    #region Loading Propertyies

    private bool _isLoadingProcedureList;
    public bool IsLoadingProcedureList
    {
      get => _isLoadingProcedureList;
      set => SetProperty(ref _isLoadingProcedureList, value);
    }

    private bool _dataLoading = true;
    public bool DataLoading
    {
      get => _dataLoading;
      set
      {
        if(value != _dataLoading)
        {
          _dataLoading = value;
          RaisePropertyChanged();
          RaisePropertyChanged(nameof(CanSelectProcedure));
        }
      }
    }

    private bool _isProcedureLoading;
    public bool IsProcedureLoading
    {
      get => _isProcedureLoading;
      set
      {
        if(value != _isProcedureLoading && !DataLoading)
        {
          SetProperty(ref _isProcedureLoading, value);
          RaisePropertyChanged(nameof(CanSelectProcedure));
        }
      }
    }

    #endregion Loading Propertyies

    #region Search Filter

    private void OnClearFilterCommand(object arg)
    {
      SelectedFromDate = null;
      SelectedToDate = null;
      FirstNameSearch = null;
      LastNameSearch = null;
      ProcedureIDSearch = null;
      PatientIDSearch = null;
      PhysicianSearch = null;
      FilterProcedureRecords();
    }

    public bool IsEnabledProcedureFilter => !CommonViewModel.Current.IsUser;
    public bool IsEnabledPatientFilter => CommonViewModel.Current.IsDoctor || CommonViewModel.Current.IsAdminUser;
    public bool IsEnabledPhysicianFilter => CommonViewModel.Current.IsAdminUser;

    private bool _isProceduresNotNull;
    public bool IsProceduresNotNull
    {
      get => _isProceduresNotNull;
      set => SetProperty(ref _isProceduresNotNull, value);
    }

    private CalendarDateRange _futureDisabledDates;
    public CalendarDateRange FutureDisabledDates
    {
      get => _futureDisabledDates;
      set => SetProperty(ref _futureDisabledDates, value);
    }

    private CalendarDateRange _pastDisabledDates;
    public CalendarDateRange PastDisabledDates
    {
      get => _pastDisabledDates;
      set => SetProperty(ref _pastDisabledDates, value);
    }

    private DateTime? _selectedFromDate;
    public DateTime? SelectedFromDate
    {
      get => _selectedFromDate;
      set
      {
        SetProperty(ref _selectedFromDate, value);
        if(SelectedFromDate != null)
        {
          PastDisabledDates = value != null ? new CalendarDateRange(DateTime.MinValue, SelectedFromDate.Value) : null;
        }
      }
    }

    private DateTime? _selectedToDate;
    public DateTime? SelectedToDate
    {
      get => _selectedToDate;
      set
      {
        SetProperty(ref _selectedToDate, value);
        if(SelectedToDate != null)
        {
          FutureDisabledDates = value != null ? new CalendarDateRange(SelectedToDate.Value, DateTime.MaxValue) : null;
        }
      }
    }

    private string _firstNameSearch;
    public string FirstNameSearch
    {
      get => _firstNameSearch;
      set => SetProperty(ref _firstNameSearch, value);
    }

    private string _lastNameSearch;
    public string LastNameSearch
    {
      get => _lastNameSearch;
      set => SetProperty(ref _lastNameSearch, value);
    }

    private string _procedureIdSearch;
    public string ProcedureIDSearch
    {
      get => _procedureIdSearch;
      set => SetProperty(ref _procedureIdSearch, value);
    }

    private string _patientIdSearch;
    public string PatientIDSearch
    {
      get => _patientIdSearch;
      set => SetProperty(ref _patientIdSearch, value);
    }

    private string _physicianSearch;
    public string PhysicianSearch
    {
      get => _physicianSearch;
      set => SetProperty(ref _physicianSearch, value);
    }

    private async void FilterProcedureRecords()
    {
      await Task.Run(() =>
      {
        if(_allProcedures == null) return;
        var filteredProcedureRecordsEnumerable_ = _allProcedures
          .AsParallel()
          .Where(procedureRecords => (SelectedToDate == null || procedureRecords.ProcedureDate <= SelectedToDate.Value.AddDays(1)) &&
                                     (SelectedFromDate == null || procedureRecords.ProcedureDate >= SelectedFromDate) &&
                                     FilterParameterCheck(procedureRecords.PatientFirstName, FirstNameSearch) &&
                                     FilterParameterCheck(procedureRecords.PatientLastName, LastNameSearch) &&
                                     FilterParameterCheck(procedureRecords.Procedure.Id.ToString(), ProcedureIDSearch) &&
                                     FilterParameterCheck(procedureRecords.Patient.HospitalPatientId, PatientIDSearch) &&
                                     FilterParameterCheck(procedureRecords.Patient.Physician.FirstName + Whitespace + procedureRecords.Patient.Physician.LastName, PhysicianSearch))
          .OrderBy(procedureRecords => procedureRecords.Procedure.ProcedureStartDateTime, ListSortDirection.Descending);

        var together_ = (filteredProcedureRecordsEnumerable_ ?? Enumerable.Empty<ProcedureRecords>()).Concat(SelectedProceduresList ?? Enumerable.Empty<ProcedureRecords>()).Distinct();
        FilteredProcedureRecordsList = new ObservableCollection<ProcedureRecords>(together_);
      });

      _sortIsAscendingColumn = true;
    }

    private bool FilterParameterCheck(string strParameter, string searchParameter)
    {
      if(searchParameter == null)
      {
        return true;
      }
      return searchParameter.Length <= strParameter.Length && strParameter.Substring(0, searchParameter.Length).ToLower() == searchParameter.ToLower();
    }

    #endregion Search Filter

    #region Navigated Procedures

    private ProcedureRecords _navigatedProcedureRecords;
    public ProcedureRecords NavigatedProcedureRecords
    {
      get => _navigatedProcedureRecords;
      set => SetProperty(ref _navigatedProcedureRecords, value);
    }

    private bool _isSummaryReportVisible;

    public bool IsSummaryReportVisible
    {
      get => _isSummaryReportVisible;
      set => SetProperty(ref _isSummaryReportVisible, value);
    }

    #endregion Navigated Procedures

    #region User Properties

    public bool IsCryterionUser => CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser;

    public bool IsBSCADMINUser => CommonViewModel.Current.IsBSCADMINUser;

    public bool IsDoctor => CommonViewModel.Current.IsDoctor;

    public bool IsAdminUser => CommonViewModel.Current.IsAdminUser;

    public bool IsDoctorGroup => IsDoctor || IsAdminUser;

    public bool EngineeringDataAccess => CommonViewModel.Current.IsCryterionUser
                                                                             || CommonViewModel.Current.IsBSCADMINUser
                                                                             || CommonViewModel.Current.IsAdminUser;

    private bool _isExportingCurrentProcedure;

    public bool IsExportingCurrentProcedure
    {
      get => _isExportingCurrentProcedure;
      set => SetProperty(ref _isExportingCurrentProcedure, value);
    }

    private string currentPhysicianName = DoubleDash;
    public string CurrentPhysician
    {
      get
      {
        if(CommonViewModel.Current.IsDoctor)
        {
          var physician_ = _dataAccess.GetphysicianByID(CommonViewModel.Current.CurrentUser.Id);
          currentPhysicianName = DoctorTitle + physician_.FirstName + Whitespace + physician_.LastName;
        }
        else
        {
          currentPhysicianName = DoubleDash;
        }
        return currentPhysicianName;
      }
    }

    #endregion User Properties

    #region SaveToUSB Properties

    private bool _isPasswordValid;
    public bool IsPasswordValid
    {
      get => _isPasswordValid;
      set
      {
        SetProperty(ref _isPasswordValid, value);
        RaisePropertyChanged(nameof(IsPasswordConfirmed));
      }
    }

    private bool _isPasswordConfirmed;
    public bool IsPasswordConfirmed
    {
      get => _isPasswordConfirmed;
      set
      {
        SetProperty(ref _isPasswordConfirmed, value);
        RaisePropertyChanged(nameof(IsOkEnabled));
      }
    }

    private bool _patientInfoAnonymized;
    public bool IsPatientInfoAnonymized
    {
      get => _patientInfoAnonymized;
      set => SetProperty(ref _patientInfoAnonymized, value);
    }

    public bool PatientInfoAnonymousVisible => (SaveToCSVSelected || SaveToPDFSelected) && (IsDoctor || IsAdminUser);

    public bool IsOkEnabled => IsPasswordValid
                                                         && IsPasswordConfirmed
                                                         && (SaveToCSVSelected || SaveToPDFSelected || SaveToJSONSelected || SaveLogSelected || SaveToReportSelected)
                                                         || SaveToJSONSelected && !SaveToCSVSelected && !SaveToPDFSelected && (_userType == UserType.Bsc || _userType == UserType.BostonBsc)
                                                         || (!SaveToCSVSelected && !SaveToPDFSelected && !SaveToJSONSelected && SaveLogSelected);

    private bool _saveInProgress;
    public bool SaveInProgress
    {
      get => _saveInProgress;
      set
      {
        SetProperty(ref _saveInProgress, value);
        RaisePropertyChanged(nameof(CanSelectProcedure));
      }
    }

    private bool _saveToCSVSelected;
    public bool SaveToCSVSelected
    {
      get => _saveToCSVSelected;
      set
      {
        SetProperty(ref _saveToCSVSelected, value);
        RaisePropertyChanged(nameof(IsPasswordVisible));
        RaisePropertyChanged(nameof(FilePassword));
        RaisePropertyChanged(nameof(ConfirmPassword));
        RaisePropertyChanged(nameof(IsOkEnabled));
        RaisePropertyChanged(nameof(PatientInfoAnonymousVisible));
        if(!value && !SaveToPDFSelected)
          IsPatientInfoAnonymized = false;
      }
    }

    private bool _saveToJSONSelected;
    public bool SaveToJSONSelected
    {
      get => _saveToJSONSelected;
      set
      {
        SetProperty(ref _saveToJSONSelected, value);
        RaisePropertyChanged(nameof(IsPasswordVisible));
        RaisePropertyChanged(nameof(FilePassword));
        RaisePropertyChanged(nameof(ConfirmPassword));
        RaisePropertyChanged(nameof(IsOkEnabled));
        if(!value)
        {
          DeletionSelected = false;
        }
      }
    }

    private bool _saveToPDFSelected;
    public bool SaveToPDFSelected
    {
      get => _saveToPDFSelected;
      set
      {
        SetProperty(ref _saveToPDFSelected, value);
        RaisePropertyChanged(nameof(IsPasswordVisible));
        RaisePropertyChanged(nameof(FilePassword));
        RaisePropertyChanged(nameof(ConfirmPassword));
        RaisePropertyChanged(nameof(IsOkEnabled));
        RaisePropertyChanged(nameof(PatientInfoAnonymousVisible));
        if(!value && !SaveToCSVSelected)
          IsPatientInfoAnonymized = false;
      }
    }

    private bool saveToReportSelected;
    public bool SaveToReportSelected
    {
      get => saveToReportSelected;
      set
      {
        SetProperty(ref saveToReportSelected, value);
        RaisePropertyChanged(nameof(IsPasswordVisible));
        RaisePropertyChanged(nameof(IsOkEnabled));
        RaisePropertyChanged(nameof(FilePassword));
        RaisePropertyChanged(nameof(ConfirmPassword));
      }
    }

    private bool _saveLogSelected;
    public bool SaveLogSelected
    {
      get => _saveLogSelected;
      set
      {
        SetProperty(ref _saveLogSelected, value);
        RaisePropertyChanged(nameof(IsPasswordVisible));
        RaisePropertyChanged(nameof(IsOkEnabled));
        RaisePropertyChanged(nameof(FilePassword));
        RaisePropertyChanged(nameof(ConfirmPassword));
      }
    }

    #endregion SaveToUSB Properties

    #region Deletion

    private bool _deletionSelected;

    public bool DeletionSelected
    {
      get => _deletionSelected;
      set
      {
        SetProperty(ref _deletionSelected, value);
        RaisePropertyChanged(nameof(FilteredProcedureRecordsList));
      }
    }

    #endregion Deletion

    #region USB Connection Properties

    private List<DriveInfo> _usbDriveList;
    public List<DriveInfo> USBDriveList
    {
      get => _usbDriveList;
      set => SetProperty(ref _usbDriveList, value);
    }

    private string _fileToExport = string.Empty;
    public string FileToExport
    {
      get => _fileToExport;
      set => SetProperty(ref _fileToExport, value);
    }

    public bool USBDriveConnected => USBDriveList != null && USBDriveList.Count != 0;

    public bool AnyProcedureSelected => _allProcedures?.Any(procedureRecords_ => procedureRecords_.Selected) == true;

    private bool isPrinterAvailable = true;
    public bool IsPrinterAvailable 
    { 
      get => isPrinterAvailable; 
      set => SetProperty(ref isPrinterAvailable, value);
    }

    #endregion USB Connection Properties

    #region Password

    public bool IsPasswordVisible => SaveToJSONSelected && _userType != UserType.Bsc && _userType != UserType.BostonBsc
                                                                     || SaveToCSVSelected
                                                                     || SaveToPDFSelected
                                                                     || SaveToReportSelected;

    private string _filePassword = string.Empty;
    public string FilePassword
    {
      get => _filePassword;
      set
      {
        ValidatePassword(value);
        IsPasswordValid = GetErrors(nameof(FilePassword)) == null;
        ValidateConfirmPassword(ConfirmPassword);
        SetProperty(ref _filePassword, value);
      }
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
      get => _confirmPassword;
      set
      {
        ValidateConfirmPassword(value);
        IsPasswordConfirmed = GetErrors(nameof(ConfirmPassword)) == null;
        SetProperty(ref _confirmPassword, value);
      }
    }

    #endregion Password

    public bool CanSelectProcedure => !IsProcedureLoading && !DataLoading && !SaveInProgress;

    public string ProcedureStartTime { get; set; } = string.Empty;

    public string ProcedureEndTime { get; set; } = string.Empty;

    #region Save procedure properties

    private int _progressBarValue;
    public int ProgressBarValue
    {
      get => _progressBarValue;
      set
      {
        SetProperty(ref _progressBarValue, value);
        USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
      }
    }

    private bool _isExportingFiles;
    public bool IsExportingFiles
    {
      get => _isExportingFiles;
      set
      {
        SetProperty(ref _isExportingFiles, value);
        USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
      }
    }

    private bool _isCanceled;
    public bool IsCanceled
    {
      get => _isCanceled;
      set
      {
        SetProperty(ref _isCanceled, value);
        USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
      }
    }

    #endregion Save procedure properties

    private List<ProcedureRecords> GetAllProcedures()
    {
      var procedureRecordsList_ = new List<ProcedureRecords>();
      IsProceduresNotNull = false;
      try
      {
        if(IsDoctor)
        {
          var userId_ = CommonViewModel.Current.CurrentUser.Id;
          procedureRecordsList_ = CreateProcedureRecordsListByUserId(userId_);
        }
        else if(IsCryterionUser || IsBSCADMINUser || IsAdminUser)
        {
          var allUsers_ = _dataAccess.GetAllUsers();
          foreach (var user_ in allUsers_)
          {
            procedureRecordsList_.AddRange(CreateProcedureRecordsListByUserId(user_.Id));
          }

          if (IsCryterionUser || IsBSCADMINUser)
          {
            foreach (var procedureRecord_ in procedureRecordsList_)
            {
              procedureRecord_.Procedure.Patient.FirstName = procedureRecord_.Procedure.Patient.LastName = Dash;
              procedureRecord_.Procedure.Patient.Physician.Name = Dash;
            }
          }
        }
      }
      catch(Exception ex_)
      {
        LogException(ex_);
        return null;
      }
      IsProceduresNotNull = !(procedureRecordsList_ is null);
      return procedureRecordsList_;
    }

    private List<ProcedureRecords> CreateProcedureRecordsListByUserId(int userId)
    {
      var procedureListByPhysician_ = _dataAccess.GetProceduresByPhysician(userId);
      var procedureList_ = FilterProcedures(procedureListByPhysician_);
      var physician_ = _dataAccess.GetphysicianByID(userId);
      var allPatients_ = _dataAccess.GetAllPatient();
      allPatients_?.ForEach(patient => patient.Physician = physician_);
      var procedureRecordsList_ = new List<ProcedureRecords>();
      foreach (var procedure_ in procedureList_)
      {
        var procRecord_ = new ProcedureRecords { Procedure = procedure_ };
        procRecord_.Procedure.Patient = allPatients_?.Find(patient => patient.ID == procedure_.PatientID);
        procedureRecordsList_.Add(procRecord_);
      }
      return procedureRecordsList_;
    }

    private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
    {
      USBDriveList = _usbDriveConnectionManager.GetUSBDriveList();
      RaisePropertyChanged(nameof(USBDriveConnected));
    }

    /// <summary>
    /// This function filter out the treatment records which does not have json files.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private List<Procedure> FilterProcedures(List<Procedure> proceduresList)
    {
      var list_ = new List<Procedure>();
      foreach(var procedure_ in proceduresList)
      {
        var ablationFilter = new List<Ablation>();
        if(procedure_.Ablations.Count > 0)
        {
          foreach(var ablation in procedure_.Ablations)
          {
            if(ablation?.DataFile?.Length > 10 && File.Exists(ablation.DataFile))
            {
              ablationFilter.Add(ablation);
            }
          }
          if(ablationFilter.Count > 0)
          {
            procedure_.Ablations = null;
            procedure_.Ablations = ablationFilter;
            list_.Add(procedure_);
          }
        }
      }
      return list_;
    }

    public DataAccessLayer.Patient CurrentPatient => CommonViewModel.Current?.CurrentPatient;

    private bool _isUsingCryterionBallon;
    public bool IsUsingCryterionBallon
    {
      get => _isUsingCryterionBallon;
      set => SetProperty(ref _isUsingCryterionBallon, value);
    }

    public int PreviuosPhysicianId { get; set; } = -1;

    /// <summary>
    /// This property gets/sets CP1ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets TC1ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets FM1ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double FM1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets PT2ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets LC1ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets MaxEcgChannel1And2ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets MaxEcgChannel3And4ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets EcgChannel1And2ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets EcgChannel3And4ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets EcgChannel5And6ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel5And6ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets EcgChannel7And8ReadingPlayback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel7And8ReadingPlayback { get; set; }

    private bool _isUsedForEngineering = false;
    public bool IsUsedForEngineering
    {
      get => _isUsedForEngineering;
      set => SetProperty(ref _isUsedForEngineering, value);
    }

    private Enumeration.CatheterType _catheterType = 0;
    public Enumeration.CatheterType CatheterType
    {
      get => _catheterType;
      set => SetProperty(ref _catheterType, value);
    }

    private string _ablationSiteText = "";
    public string AblationSiteText
    {
      get => _ablationSiteText;
      set => SetProperty(ref _ablationSiteText, value);
    }

    /// <summary>
    /// This property gets/sets the TC1Reading sensor value when connected, otherwise
    /// it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TC1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.TC1Reading : TC1ReadingPlayback;

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.TC1Reading = value;
        else
          TC1ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the CP1Reading sensor value when connected, otherwise
    /// it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.CP1Reading : CP1ReadingPlayback;

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.CP1Reading = value;
        else
          CP1ReadingPlayback = value;

        RaisePropertyChanged();
        RaisePropertyChanged(nameof(TipOrBalloonPressureReading));
      }
    }

    /// <summary>
    /// This property gets/sets the PT2Reading sensor value when connected, otherwise
    /// it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT2Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PT2Reading : PT2ReadingPlayback;

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT2Reading = value;
        else
          PT2ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the FM1Reading sensor value when connected, otherwise
    /// it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double FM1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.FM1Reading : FM1ReadingPlayback;

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.FM1Reading = value;
        else
          FM1ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the LC1Reading sensor value when connected, otherwise
    /// it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.LC1Reading : LC1ReadingPlayback;

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.LC1Reading = value;
        else
          LC1ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the MaxEcgChannel1And2Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel1And2Reading
    {
      get
      {
        double localMaxEcgChannel1And2Reading = CommonViewModel.Current.MaxEcgChannel1And2Reading;
        CommonViewModel.Current.MaxEcgChannel1And2Reading = 0;

        return SensorReadingMananger.AreSensorsConnected ? localMaxEcgChannel1And2Reading : MaxEcgChannel1And2ReadingPlayback;
      }

      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.MaxEcgChannel1And2Reading = value;
        else
          MaxEcgChannel1And2ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the EcgChannel1And2Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel1And2Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.EcgChannel1And2Reading : EcgChannel1And2ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel1And2Reading = value;
        else
          EcgChannel1And2ReadingPlayback = value;

        RaisePropertyChanged();
        RaisePropertyChanged(nameof(TipOrBalloonPressureReading));
      }
    }

    /// <summary>
    /// This property gets/sets the EcgChannel3And4Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel3And4Reading
    {
      get
      {
        if(SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.EcgChannel3And4Reading;
        else
          return EcgChannel3And4ReadingPlayback;
      }
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel3And4Reading = value;
        else
          EcgChannel3And4ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the MaxEcgChannel3And4Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel3And4Reading
    {
      get
      {
        double localMaxEcgChannel3And4Reading = CommonViewModel.Current.MaxEcgChannel3And4Reading;
        CommonViewModel.Current.MaxEcgChannel3And4Reading = 0;
        return SensorReadingMananger.AreSensorsConnected ? localMaxEcgChannel3And4Reading : MaxEcgChannel3And4ReadingPlayback;
      }
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.MaxEcgChannel3And4Reading = value;
        else
          MaxEcgChannel3And4ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the EcgChannel5And6Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel5And6Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.EcgChannel5And6Reading : EcgChannel5And6ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel5And6Reading = value;
        else
          EcgChannel5And6ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the EcgChannel7And8Reading sensor value when connected,
    /// otherwise it gets/sets the Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel7And8Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.EcgChannel7And8Reading : EcgChannel7And8ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel7And8Reading = value;
        else
          EcgChannel7And8ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets the Patient PID Duty Cycle value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PWMBAL
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PatientPIDDutyCycle : PatientPIDDutyCyclePlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PatientPIDDutyCycle = value;
        else
          PatientPIDDutyCyclePlayback = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// Gets or sets the Patient PID Duty Cycle value in playback mode
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PatientPIDDutyCyclePlayback { get; set; }

    /// <summary>
    /// Gets or sets the PID Duty cycle value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PWMINJ
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PIDDutyCycle : PIDDutyCyclePlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PIDDutyCycle = value;
        else
          PIDDutyCyclePlayback = value;
        RaisePropertyChanged();
      }
    }

    public double PIDDutyCyclePlayback { get; set; }

    public double PT1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PT1Reading : PT1ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT1Reading = value;
        else
          PT1ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    public double PT1ReadingPlayback { get; set; }

    public int BloodDetecorImValue
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.BloodDetecorImValue : BloodDetecorImValuePlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.BloodDetecorImValue = value;
        else
          BloodDetecorImValuePlayback = value;

        RaisePropertyChanged();
      }
    }

    public int BloodDetecorImValuePlayback { get; set; }

    public double PT3Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PT3Reading : PT3ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT3Reading = value;
        else
          PT3ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    public double PT3ReadingPlayback { get; set; }

    public double PT4Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PT4Reading : PT4ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT4Reading = value;
        else
          PT4ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    public double PT4ReadingPlayback { get; set; }

    public double PT5Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.PT5Reading : PT5ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT5Reading = value;
        else
          PT5ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }

    public double PT5ReadingPlayback { get; set; }

    public double TS1Reading
    {
      get => SensorReadingMananger.AreSensorsConnected ? CommonViewModel.Current.TS1Reading : TS1ReadingPlayback;
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.TS1Reading = value;
        else
          TS1ReadingPlayback = value;
        RaisePropertyChanged();
      }
    }

    public double TS1ReadingPlayback { get; set; }

    private double _temperatureRate;
    public double TemperatureRate
    {
      get => _temperatureRate;
      set => SetProperty(ref _temperatureRate, value);
    }

    private double _maxTemperatureRate;
    public double MaxTemperatureRate
    {
      get => _maxTemperatureRate;
      set => SetProperty(ref _maxTemperatureRate, value);
    }

    private int _requiredTargetTemperature;
    public int RequiredTargetTemperature
    {
      get => _requiredTargetTemperature;
      set => SetProperty(ref _requiredTargetTemperature, value);
    }

    private int _timeToTargetTemperature;
    public int TimeToTargetTemperature
    {
      get => _timeToTargetTemperature;
      set => SetProperty(ref _timeToTargetTemperature, value);
    }

    private int _veinIsolationDuration;
    public int VeinIsolationDuration
    {
      get => _veinIsolationDuration;
      set => SetProperty(ref _veinIsolationDuration, value);
    }

    private int _temperatureAtTTI;
    public int TemperatureAtTTI
    {
      get => _temperatureAtTTI;
      set => SetProperty(ref _temperatureAtTTI, value);
    }

    private int _timeSinceTTI;
    public int TimeSinceTTI
    {
      get => _timeSinceTTI;
      set => SetProperty(ref _timeSinceTTI, value);
    }

    private int _timeToThawTemperature;
    public int TimeToThawTemperature
    {
      get => _timeToThawTemperature;
      set => SetProperty(ref _timeToThawTemperature, value);
    }

    private bool _esophagusTemperatureThresholdReached;
    public bool EsophagusTemperatureThresholdReached
    {
      get => _esophagusTemperatureThresholdReached;
      set => SetProperty(ref _esophagusTemperatureThresholdReached, value);
    }

    private bool diaphragmAmplitudeThresholdReached;
    public bool DiaphragmAmplitudeThresholdReached
    {
      get => diaphragmAmplitudeThresholdReached;
      set => SetProperty(ref diaphragmAmplitudeThresholdReached, value);
    }

    private bool _ignoreMinimumDiaphragmMovement;
    public bool IgnoreMinimumDiaphragmMovement
    {
      get => _ignoreMinimumDiaphragmMovement;
      set => SetProperty(ref _ignoreMinimumDiaphragmMovement, value);
    }

    public AblationSiteEnum AblationSite
    {
      get => CommonViewModel.Current.AblationSite;
      set
      {
        CommonViewModel.Current.AblationSite = value;
        RaisePropertyChanged();
      }
    }

    public List<AblationDataDetails> SingleAblationDatasList { get; set; } = new List<AblationDataDetails>();

    private int cryoTherapyTime;
    public int CryoTherapyTime
    {
      get => cryoTherapyTime;
      set => SetProperty(ref cryoTherapyTime, value);
    }

    private int _totalCryoTherapyTime;
    public int TotalCryoTherapyTime
    {
      get => _totalCryoTherapyTime;
      set => SetProperty(ref _totalCryoTherapyTime, value);
    }

    #region thawing state
    private readonly bool isPlayBack = false;
    public bool IsPlayBack => isPlayBack;

    private int requiredAblationTime;
    public int RequiredAblationTime
    {
      get => requiredAblationTime;
      set => SetProperty(ref requiredAblationTime, value);
    }

    private int actualAblationTime;
    public int ActualAblationTime
    {
      get => actualAblationTime;
      set => SetProperty(ref actualAblationTime, value);
    }

    private readonly int thawingElapsedTime = 0;
    public int ThawingElapsedTime => thawingElapsedTime;
    #endregion thawing state


    private bool _isVisible = true;
    public bool IsVisible
    {
      get => _isVisible;
      set => SetProperty(ref _isVisible, value);
    }

    private bool _patientIDVisible = !(CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser);
    public bool PatientIDVisible
    {
      get => _patientIDVisible;
      set => SetProperty(ref _patientIDVisible, value);
    }

    private int _ablationNumber;
    public int AblationNumber
    {
      get => _ablationNumber;
      set => SetProperty(ref _ablationNumber, value);
    }

    private string treatmentNotes = string.Empty;
    public string TreatmentNotes
    {
      get => treatmentNotes;
      set => SetProperty(ref treatmentNotes, value);
    }

    private int _lowAblationTemperatureAlarm = -55;
    public int LowAblationTemperatureAlarm
    {
      get => _lowAblationTemperatureAlarm;
      set => SetProperty(ref _lowAblationTemperatureAlarm, value);
    }

    private int _highAblationTemperatureAlarm = 21;
    public int HighAblationTemperatureAlarm
    {
      get => _highAblationTemperatureAlarm;
      set => SetProperty(ref _highAblationTemperatureAlarm, value);
    }

    private int _thawTimerToTemperature = 20;
    public int ThawTimerToTemperature
    {
      get => _thawTimerToTemperature;
      set => SetProperty(ref _thawTimerToTemperature, value);
    }

    private int _esophagusTemperature = 35;
    public int EsophagusTemperature
    {
      get => _esophagusTemperature;
      set => SetProperty(ref _esophagusTemperature, value);
    }

    private int _diaphragmAmplitude = 80;
    public int DiaphragmAmplitude
    {
      get => _diaphragmAmplitude;
      set => SetProperty(ref _diaphragmAmplitude, value);
    }

    private int _treatmentNumber;
    public int TreatmentNumber
    {
      get => _treatmentNumber;
      set => SetProperty(ref _treatmentNumber, value);
    }

    private int _totalTreatmentNumber;
    public int TotalTreatmentNumber
    {
      get => _totalTreatmentNumber;
      set => SetProperty(ref _totalTreatmentNumber, value);
    }

    private bool _isSnowFlakeVisible;
    public bool IsSnowFlakeVisible
    {
      get => _isSnowFlakeVisible;
      set => SetProperty(ref _isSnowFlakeVisible, value);
    }

    private bool _isLastAblationDataLoaded;
    public bool IsLastAblationDataLoaded
    {
      get => _isLastAblationDataLoaded;
      set => SetProperty(ref _isLastAblationDataLoaded, value);
    }

    private MessageStateId _systemState;
    public MessageStateId SystemState
    {
      get => _systemState;
      set => SetProperty(ref _systemState, value);
    }

    private bool _isDiaphragmMovementDetected;
    public bool IsDiaphragmMovementDetected
    {
      get => _isDiaphragmMovementDetected;
      set => SetProperty(ref _isDiaphragmMovementDetected, value);
    }

    private bool _isTargetTemperatureReached;
    public bool IsTargetTemperatureReached
    {
      get => _isTargetTemperatureReached;
      set => SetProperty(ref _isTargetTemperatureReached, value);
    }

    private bool _isThawTemperatureReached;
    public bool IsThawTemperatureReached
    {
      get => _isThawTemperatureReached;
      set => SetProperty(ref _isThawTemperatureReached, value);
    }

    private List<Enumeration.CatheterType> _catheterTypeList;
    public List<Enumeration.CatheterType> CatheterTypeList
    {
      get => _catheterTypeList;
      set => SetProperty(ref _catheterTypeList, value);
    }

    private bool _dasBalloonEnabled;
    public bool DASBalloonEnabled
    {
      get => _dasBalloonEnabled;
      set => SetProperty(ref _dasBalloonEnabled, value);
    }

    public ObservableCollection<ProcedureLog> ProcedureLogs
    {
      get => _dataAccess.GetAllProcedureLogsAccordingToProcedureID(NavigatedProcedureRecords.Procedure.Id);
      set => RaisePropertyChanged();
    }

    /// <summary>
    /// Function/Command that handles the treatment number and display elements when the Ablation Forward
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="OnAblationNumberForward">The command parameter (not used in this function).</param>
    private void OnAblationNumberForward(object OnAblationNumberForward)
    {
      TreatmentNumber++;
      TreatmentNumber = TreatmentNumber >= TotalTreatmentNumber ? TotalTreatmentNumber : TreatmentNumber;

      if(TreatmentNumber <= TotalTreatmentNumber)
      {
        ResetDisplayElements();
        LoadPlaybackMode(TreatmentNumber);
      }
    }

    public void SetToTreatment(int treatment)
    {
      if(treatment <= TotalTreatmentNumber)
      {
        LoadPlaybackMode(treatment);
      }
    }

    private bool CanAblationNumberForward(object arg) => true;

    private void OnAblationNumberBackward(object OnAblationNumberBackward)
    {
      TreatmentNumber--;

      TreatmentNumber = TreatmentNumber <= 0 ? 1 : TreatmentNumber;

      if(TreatmentNumber <= TotalTreatmentNumber)
      {
        ResetDisplayElements();
        LoadPlaybackMode(_treatmentNumber);
      }
    }

    public void ExtractProcedureRecordsData(ProcedureRecords selectedProcedure)
    {
      if(selectedProcedure?.Procedure == null)
      {
        return;
      }
      IsProcedureLoading = true;
      DataLoading = true;

      try
      {
        if(_AblationDataExtractor.AblationReportAccordingToProcedure.ContainsKey(selectedProcedure) && !selectedProcedure.Selected)
        {
          _AblationDataExtractor.AblationReportAccordingToProcedure.Remove(selectedProcedure);
        }
        ResetProcedureData();
        ResetDisplayElements();
        PlaybackModeEvent?.Invoke(null, null); //handle chart clearing
        CommonViewModel.Current?.AllAblationDataList?.Clear();
        CommonViewModel.Current.CurrentProcedure = selectedProcedure.Procedure;
        CommonViewModel.Current.CurrentPatient = selectedProcedure.Patient;

        List<Ablation> allAblationByProcedureId = CommonViewModel.Current?.Data?.DataAccess?.GetAllAblationByProcedureId(selectedProcedure.Procedure.Id);
        if(allAblationByProcedureId != null && allAblationByProcedureId.Count > 0)
        {
          CommonViewModel.Current.GenerateAblationSummary();
          TotalTreatmentNumber = allAblationByProcedureId.Count;

          foreach(Ablation ablation in allAblationByProcedureId)
          {
            if(!string.IsNullOrWhiteSpace(ablation.DataFile) && File.Exists(ablation.DataFile))
            {
              LoadAblationDataFromFile(ablation.DataFile);
            }
          }
          if(TotalTreatmentNumber > 0 && TreatmentNumber == 0 && AblationNumberForwardCommand.CanExecute(null))
          {
            AblationNumberForwardCommand.Execute(null);
          }
        }
        List<AblationReport> _ablationList = new List<AblationReport>(AblationList);
        if(!_AblationDataExtractor.AblationReportAccordingToProcedure.ContainsKey(selectedProcedure))
        {
          _AblationDataExtractor.AblationReportAccordingToProcedure.Add(selectedProcedure, _ablationList);
        }
        if(allAblationByProcedureId?.Count > 0)
        {
          GetCatheterTypeList();
        }
      }
      catch(Exception ex)
      {
        LogException(ex);
        Application.Current.BeginInvoke(() =>
        {
          var genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID112, (int)Enumeration.ErrorTypes.GUI);
          var messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok);
          messagePopup.ShowDialog();
        });
      }
      finally
      {
        DataLoading = false;
        IsProcedureLoading = false;
      }
    }

    private async Task DeleteProcedureDataFileAsync(List<ProcedureRecords> selectedProcedures)
    {
      var lst = new List<string>();
      foreach(var p in selectedProcedures)
      {
        lst.AddRange(p.Procedure.Ablations.Select(a => a.DataFile));
      }

      await Task.Run(() =>
      {
        foreach(var f_ in lst)
        {
          try
          {
            File.Delete(f_);
          }
          catch(Exception e)
          {
            LogException(e);
            Application.Current.BeginInvoke(() =>
                  {
                    var errorPopup_ = new MessagePopup("Error in deleting data file.", MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
                    errorPopup_.ShowDialog();
                  });
          }
        }
      });
    }

    private async Task LogDeleteActionsAsync(List<ProcedureRecords> selectedProcedures)
    {
      await Task.Run(() =>
      {
        try
        {
          foreach(var pr_ in selectedProcedures)
          {
            var message_ = "ID: " + pr_.Procedure.Id + ", " + "Date: " + pr_.ProcedureDate;
            CommonViewModel.Current.LogUserAction(Enumeration.Actions.DeleteProcedure, message_);
          }
        }
        catch(Exception e)
        {
          LogException(e);
        }
      });
    }

    private async Task ArchiveProceduresOnDBAsync(List<ProcedureRecords> selectedProcedures)
    {
      var result = selectedProcedures?.Select(p_ => p_.Procedure).ToList();
      await Task.Run(() =>
      {
        try
        {
          _dataAccess.ArchiveProcedures(result);
        }
        catch(Exception e)
        {
          LogException(e);
        }
      });
    }

    public void CheckProcedureToSave(ProcedureRecords checkedProcedureRecord)
    {
      if(checkedProcedureRecord == null)
      {
        return;
      }

      if(_AblationDataExtractor.AblationReportAccordingToProcedure.ContainsKey(checkedProcedureRecord)
              && !checkedProcedureRecord.Selected)
      {
        _AblationDataExtractor.AblationReportAccordingToProcedure.Remove(checkedProcedureRecord);
      }

      if(checkedProcedureRecord.Procedure != null)
      {
        List<Ablation> ablationList_ = _dataAccess.GetAllAblationByProcedureId(checkedProcedureRecord.Procedure.Id);
        if(ablationList_?.Count > 0)
        {
          List<AblationReport> ablationReportList_ = new List<AblationReport>(AblationList);
          if(!_AblationDataExtractor.AblationReportAccordingToProcedure.ContainsKey(checkedProcedureRecord))
          {
            _AblationDataExtractor.AblationReportAccordingToProcedure.Add(checkedProcedureRecord, ablationReportList_);
          }
          GetCatheterTypeList();
        }
      }
    }

    private bool CanSaveToUSBCommand(object arg) => AnyProcedureSelected;

    private bool CanPrintPDFCommand(object arg) => AnyProcedureSelected;

    private string _exceptionMessage = string.Empty;
    public string ExceptionMessage
    {
      get => _exceptionMessage;
      set => SetProperty(ref _exceptionMessage, value);
    }

    private string _usbPath = string.Empty;
    public string USBPath
    {
      get => _usbPath;
      set => SetProperty(ref _usbPath, value);
    }

    private async void OnPrintPDFCommand(object selectedProcedures)
    {
      if(SelectedProceduresList?.Count == 0)
        return;
      
      IsPrinterAvailable = false;
      using(var service_ = new DataExportService(_userType, (SelectedProceduresList ?? throw new InvalidOperationException()).ToList()))
      {
        await service_.PrintPdfReport();
      }
      IsPrinterAvailable = true;
    }

    private async void OnSaveToUSBCommand(object selectedProcedure)
    {
      if(!USBDriveConnected || !AnyProcedureSelected)
      {
        return;
      }
      USBPath = USBDriveList[0]?.Name + ExportFolder + Path.DirectorySeparatorChar;
      if(string.IsNullOrEmpty(USBPath))
      {
        throw new ArgumentNullException(nameof(USBPath));
      }

      var saveProcedureDialog_ = new SaveProcedureToUSB(this);
      var dialogResult_ = saveProcedureDialog_?.ShowDialog();
      if(dialogResult_.HasValue && dialogResult_.Value)
      {
        _cancellationTokenSource = new CancellationTokenSource();
        var exportDialog_ = new FileExportCancellationPopup(_cancellationTokenSource, this);
        var procedureSaved_ = false;
        ProgressBarValue = 0;
        IsExportingFiles = true;
        IsCanceled = false;
        var exportTask_ = Task.Run(() =>
        {
          try
          {
            SaveInProgress = true;
            procedureSaved_ = ExportSelectedFilesToUsb();
          }
          catch(Exception ex)
          {
            ExceptionMessage = ex.Message;
            LogException(ex);
            _cancellationTokenSource.Cancel();
          }
          finally
          {
            SaveInProgress = false;
          }
        });
        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
        {
          exportDialog_.ShowDialog();
        }));
        await exportTask_;
        if(procedureSaved_)
        {
          IsExportingFiles = false;
          IsCanceled = false;
        }
        else
        {
          IsExportingFiles = false;
          IsCanceled = true;
        }
      }
    }

    private void GetUserType()
    {
      if(CommonViewModel.Current.IsCryterionUser)
      {
        _accessControlType = LoginManager.AccessControlType.CRYTERION;
        _userType = UserType.Bsc;
      }
      else if(CommonViewModel.Current.IsBSCADMINUser)
      {
        _accessControlType = LoginManager.AccessControlType.BSCADMIN;
        _userType = UserType.BostonBsc;
      }
      else if(CommonViewModel.Current.IsDoctor)
      {
        _accessControlType = LoginManager.AccessControlType.DOCTOR;
        _userType = UserType.Doctor;
      }
      else if(CommonViewModel.Current.IsAdminUser)
      {
        _accessControlType = LoginManager.AccessControlType.ADMIN;
        _userType = UserType.Admin;
      }
    }

    public async Task OnDeleteDataFiles(bool? delete)
    {
      await DeleteFilteredProcedureListAsync();
      await DeleteAllProcedureListAsync();
      await DeleteProcedureDataFileAsync(SelectedProceduresList.ToList());
      await ArchiveProceduresOnDBAsync(SelectedProceduresList.ToList());
      await LogDeleteActionsAsync(SelectedProceduresList.ToList());

      AllSelected = false;
      DeletionSelected = false;
      if (SelectedProceduresList?.Any(procedure => CurrentPatient != null && procedure.Patient.ID == CurrentPatient.ID) == true)
      {
        CommonViewModel.Current.CurrentPatient = null;
        RaisePropertyChanged(nameof(CurrentPatient));
      }
      SelectedProceduresList?.Clear();
      RaisePropertyChanged(nameof(AnyProcedureSelected));
      RaisePropertyChanged(nameof(NavigatedProcedureRecords));
    }

    private async Task DeleteFilteredProcedureListAsync()
    {
      await Application.Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        foreach(var procedure_ in SelectedProceduresList)
        {
          FilteredProcedureRecordsList.Remove(procedure_);
        }
      }));
    }

    private async Task DeleteAllProcedureListAsync()
    {
      await Task.Run(() =>
      {
        foreach(var p_ in SelectedProceduresList)
        {
          _allProcedures.Remove(p_);
        }
      });
    }

    private bool ExportSelectedFilesToUsb()
    {
      var destinationPath_ = USBPath + _userType + Underscore;
      var exportSelectedFilesToUsb_ = false;

      if(SaveLogSelected)
      {
        var dir_ = new DirectoryInfo(USBPath);
        ZipLogsToUsb(dir_, _cancellationTokenSource.Token);
      }

      foreach(var procedureRecord_ in SelectedProceduresList)
      {
        if(_cancellationTokenSource.Token.IsCancellationRequested)
        {
          IsExportingFiles = false;
          IsCanceled = true;
          return false;
        }

        FileToExport = destinationPath_ + procedureRecord_.Procedure.Description;
        var allAblationDataDetails_ = ExtractAblationDetails(procedureRecord_);
        CheckProcedureToSave(procedureRecord_);

        if(allAblationDataDetails_?.Count > 0)
        {
          exportSelectedFilesToUsb_ = SaveSelectedProcedureToUsb(allAblationDataDetails_, FileToExport, procedureRecord_);
        }

        if(exportSelectedFilesToUsb_)
        {
          ProgressBarValue += 1;
        }
      }

      if(SaveToReportSelected)
      {
        SaveCaseReportToUSB();
      }

      return exportSelectedFilesToUsb_;
    }

    private List<List<AblationDataDetails>> ExtractAblationDetails(ProcedureRecords procRec)
    {
      var allAblationDataDetails_ = new List<List<AblationDataDetails>>();
      var jsonManager_ = new JsonManager();

      try
      {
        foreach(var ablation_ in procRec.Procedure.Ablations)
        {
          if(!string.IsNullOrEmpty(ablation_?.DataFile) && File.Exists(ablation_.DataFile))
          {
            var ablationData_ = LoadAblationFromFile(jsonManager_, ablation_.DataFile);
            if(ablationData_ != null)
              allAblationDataDetails_.Add(ablationData_);
          }
        }
        return allAblationDataDetails_;
      }
      catch(Exception ex_)
      {
        LogException(ex_);
        return null;
      }
    }

    private bool SaveCaseReportToUSB()
    {
      var result_ = true;

      string procedureStartDate = "";
      string procedureEndDate = "";
      string caseReportName = "CaseReport";
      List<ProcedureRecords> procedureRecordsListWhereFrom = new List<ProcedureRecords>();
      if(ProcedureStartTime == "0") procedureStartDate = "1900-01-01";
      else
      {
        caseReportName += ProcedureStartTime;
        procedureStartDate = ProcedureStartTime + "-01-01";
      }

      if(ProcedureEndTime == "0") procedureEndDate = "2900-12-31";
      else
      {
        if(ProcedureStartTime != ProcedureEndTime) caseReportName += "-" + ProcedureEndTime;
        procedureEndDate = ProcedureEndTime + "-12-31";
      }

      procedureRecordsListWhereFrom = FilteredProcedureRecordsList.Where(p =>
          p.ProcedureDate >= DateTime.Parse(procedureStartDate) &&
          p.ProcedureDate <= DateTime.Parse(procedureEndDate)).ToList();

      Application.Current.BeginInvoke(() =>
      {
        CaseSummaryReport caseSummaryReport = new CaseSummaryReport(procedureRecordsListWhereFrom);
        caseSummaryReport.Visibility = Visibility.Collapsed;
        caseSummaryReport.Show();
      });

      try
      {
        _PDFCaseReport.GeneratePDFCaseReport(procedureRecordsListWhereFrom, caseReportName, HospitalName);

        string sourceFilePath = "";

        string mysavePath = USBDriveList[0].Name + "PatientRecord\\" + caseReportName + ".pdf";
        sourceFilePath = getCaseFilePath(mysavePath, caseReportName) + ".pdf";
        if(File.Exists(mysavePath))
        {
          File.Delete(mysavePath);
        }

        File.Copy(sourceFilePath, mysavePath);
        _PDFConversion.Protect(sourceFilePath, mysavePath, FilePassword);
        File.Delete(sourceFilePath);
      }
      catch(Exception e)
      {
        LogException(e);
        result_ = false;
      }
      return result_;
    }

    /// <summary>
    /// Function that returns ablation summary info
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private AblationSummary GetTheAblationSummary(List<List<AblationDataDetails>> ListAblationDetails)
    {
      int duration = 0;

      //Clears all existing data
      AblationSummary AblationSummary = new AblationSummary();
      int ablationSite = (int)AblationSiteEnum.OTHER;

      if(AblationSummary != null && ListAblationDetails?.Count > 0)
      {
        // Only compute the duration in Ablation (not thawing)
        // Generate/compute the Ablation duration (depending of the site) for each Ablations in the procedure.
        foreach(List<AblationDataDetails> listAblationDetails in ListAblationDetails)
        {
          if(listAblationDetails != null)
          {
            //Compute the Ablation duration (stop the increment when in Thawing)
            foreach(AblationDataDetails ablationDetails in listAblationDetails)
            {
              //Keep duration for ablation only
              if(ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_THAWING &&
                      ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_IDLE &&
                      ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_READY &&
                      ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION &&
                      ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_EXCEPTION &&
                      ablationDetails.SystemState != (int)MessageStateId.CAN_ID_STATE_UNKNOWN)
              {
                duration = ablationDetails.ID;
              }

              ablationSite = ablationDetails.AblationSite;
            }
            switch(ablationSite)
            {
              case (int)AblationSiteEnum.RSPV:
                AblationSummary.TotalRSPV++;
                AblationSummary.TotalRSPVDuration += duration;
                break;

              case (int)AblationSiteEnum.RIPV:
                AblationSummary.TotalRIPV++;
                AblationSummary.TotalRIPVDuration += duration;
                break;

              case (int)AblationSiteEnum.LSPV:
                AblationSummary.TotalLSPV++;
                AblationSummary.TotalLSPVDuration += duration;
                break;

              case (int)AblationSiteEnum.LIPV:
                AblationSummary.TotalLIPV++;
                AblationSummary.TotalLIPVDuration += duration;
                break;

              case (int)AblationSiteEnum.LCPV:
                AblationSummary.TotalLCPV++;
                AblationSummary.TotalLCPVDuration += duration;
                break;

              case (int)AblationSiteEnum.RMPV:
                AblationSummary.TotalRMPV++;
                AblationSummary.TotalRMPVDuration += duration;
                break;

              case (int)AblationSiteEnum.OTHER:
                AblationSummary.TotalOther++;
                AblationSummary.TotalOtherDuration += duration;
                break;
            }
          }
        }
      }
      return AblationSummary;
    }

    private bool SaveSelectedProcedureToUsb(
        List<List<AblationDataDetails>> allAblationDataList,
        string saveToUSBPath,
        ProcedureRecords procedureRecord)
    {
      if(allAblationDataList == null)
      {
        throw new ArgumentNullException(nameof(allAblationDataList));
      }

      if(saveToUSBPath == null)
      {
        throw new ArgumentNullException(nameof(saveToUSBPath));
      }

      if(procedureRecord == null)
      {
        throw new ArgumentNullException(nameof(procedureRecord));
      }

      if(USBPath == null)
      {
        throw new ArgumentNullException(nameof(USBPath));
      }

      if(!Directory.Exists(USBPath))
      {
        Directory.CreateDirectory(USBPath);
      }

      var selectedProcedureSaved_ = false;
      var dest_ = new DirectoryInfo(USBPath);

      using(var dataExportService_ = new DataExportService(_userType, procedureRecord, dest_, IsPatientInfoAnonymized, FilePassword))
      {
        if(SaveToJSONSelected)
        {
          try
          {
            selectedProcedureSaved_ = File.Exists(dataExportService_.ExportJsonFile()?.FullName);
          }
          catch(Exception e)
          {
            LogException(e);
            selectedProcedureSaved_ = false;
          }
        }

        if(SaveToCSVSelected)
        {
          try
          {
            selectedProcedureSaved_ = File.Exists(dataExportService_.ExportExcelFile()?.FullName);
          }
          catch(Exception e)
          {
            LogException(e);
            selectedProcedureSaved_ = false;
          }
        }
        else
        {
          selectedProcedureSaved_ = true;
        }

        if(SaveToPDFSelected)
        {
          try
          {
            selectedProcedureSaved_ = File.Exists(dataExportService_.ExportPdfFile().FullName);
          }
          catch(Exception e)
          {
            LogException(e);
            selectedProcedureSaved_ = false;
          }
        }
      }

      return selectedProcedureSaved_;
    }

    private string getCaseFilePath(string path, string caseFileName)
    {
      var basePath_ = GetBasePath() + "PDFFiles\\";
      var temppath_ = basePath_ + caseFileName;
      return temppath_;
    }

    /// <summary>
    /// Procedure that reset the procedure data (lists and counters) and notifies the listeners to update the view
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetProcedureData()
    {
      CommonViewModel.Current?.AllAblationDataList?.Clear();
      SingleAblationDatasList.Clear();
      CommonViewModel.Current?.AblationSummary.ClearAblationSummary();
      RaisePropertyChanged(nameof(AblationSummary));

      TotalTreatmentNumber = 0;
      TreatmentNumber = 0;

      CommonViewModel.Current.CurrentPatient = null;
      RaisePropertyChanged(nameof(CurrentPatient));

      CommonViewModel.Current.CurrentAblation = null;
    }

    public void ResetTreatmentInfo()
    {
      ResetDisplayElements();
      LoadPlaybackMode(TreatmentNumber);
    }

    private void ResetDisplayElements()
    {
      CP1Reading = 0;
      EcgChannel3And4Reading = 0;
      EcgChannel5And6Reading = 0;
      EcgChannel7And8Reading = 0;
      TC1Reading = 0;
      CryoTherapyTime = 0;
      TemperatureRate = 0;
      MaxTemperatureRate = 0;
      RequiredTargetTemperature = 0;
      TimeToTargetTemperature = 0;
      VeinIsolationDuration = 0;
      ThawTimerToTemperature = 0;
      TimeToThawTemperature = 0;
    }

    private void LoadPlaybackMode(int treatmentNumber)
    {
      if(CommonViewModel.Current.AllAblationDataList.Count != 0)
      {
        if(CommonViewModel.Current.AllAblationDataList.Count >= treatmentNumber)
        {
          SingleAblationDatasList = CommonViewModel.Current.AllAblationDataList[treatmentNumber - 1];

          if(SingleAblationDatasList != null && SingleAblationDatasList.Count > 0)
          {
            CommonViewModel.Current.AblationSite = (AblationSiteEnum)SingleAblationDatasList[0].AblationSite;
            DASBalloonEnabled = SingleAblationDatasList[0].PressureSetPoint > _balloon31mmThreshold;
          }
        }
      }
      PlaybackModeEvent?.Invoke(null, null);

      var isSingleAblationDataListValid = SingleAblationDatasList != null;
      var lastIndex = isSingleAblationDataListValid ? SingleAblationDatasList.Count - 1 : -1;

      var ablationSite = isSingleAblationDataListValid && lastIndex >= 0 && Enum.IsDefined(typeof(AblationSiteEnum), SingleAblationDatasList[lastIndex].AblationSite)
          ? (AblationSiteEnum)SingleAblationDatasList[lastIndex].AblationSite
          : AblationSiteEnum.UNKNOWN;

      CatheterType = isSingleAblationDataListValid && lastIndex >= 0 ? (Enumeration.CatheterType)(SingleAblationDatasList[lastIndex].CatheterId & 0x03) : Enumeration.CatheterType.ID_UNKNOWN_mm;
      AblationSiteText = ablationSite.GetDescription();
      IsUsedForEngineering = isSingleAblationDataListValid && lastIndex >= 0 && SingleAblationDatasList[lastIndex].IsUsedForEngineering;
      RequiredAblationTime = isSingleAblationDataListValid && lastIndex >= 0 ? SingleAblationDatasList[lastIndex].RequiredAblationTime : 0;
      ActualAblationTime = isSingleAblationDataListValid
        ? SingleAblationDatasList.Count(item =>
          item.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION || item.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION)
        : 0;
    }

    private bool CanAblationNumberBackward(object arg) => true;

    /// <summary>
    /// Function that retrieve an ablation (data and ECG Data) from a JSON file and deserialize it in a
    /// JSON file
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="filename">The ablation file name on the disk.</param>
    private void LoadAblationDataFromFile(string filename)
    {
      List<AblationDataDetails> ablationDatasList_;

      try
      {
        ablationDatasList_ = LoadAblationFromFile(_JsonFileManager, filename);

        if(ablationDatasList_ == null)
          TotalTreatmentNumber--;
        else
        {
          ablationDatasList_ = ablationDatasList_
            .DistinctBy(ab => ab.ID)
            .ToList();

          if(!ProcedureLogModel.CanReloadProcudreInformation)
          {
            if(CommonViewModel.Current.AllAblationDataList == null)
            {
              CommonViewModel.Current.AllAblationDataList = new List<List<AblationDataDetails>>();
            }
            CommonViewModel.Current.AllAblationDataList.Add(ablationDatasList_);
          }
        }
      }
      catch(FileNotFoundException e)
      {
        LogException(e);
        throw new Exception("The treatment file could not be found!", e);
      }
      catch(Exception exception_)
      {
        LogException(exception_);
        throw new Exception("An error occurred while loading the treatment file in memory!", exception_);
      }
    }


    /// <summary>
    /// Function that returns the value of Base path
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string GetBasePath()
    {
      var thePath_ = string.Empty;
      var path_ = AppDomain.CurrentDomain.BaseDirectory;
      var extractedStrings_ = Regex.Split(path_, "bin");  //split it in bin
      thePath_ = extractedStrings_[0];
      return thePath_;
    }

    private ObservableCollection<AblationReport> ablationList;
    /// <summary>
    /// Gets/sets ablation list value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    public ObservableCollection<AblationReport> AblationList
    {
      get
      {
        ablationList.Clear();
        foreach(var ablationDataDetail in CommonViewModel.Current.AllAblationDataList.Where(a => a.Any()))
        {
          int duration = ablationDataDetail.Count(a => a.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION
                                                                                                   || a.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION);

          int MinDMSValue = ablationDataDetail[ablationDataDetail.Count - 1].MinimumDiaphragmMovementValue;

          int lastTimeToThaw = 0;
          if(ablationDataDetail[ablationDataDetail.Count - 1].TC1Reading >= ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature)
          {
            lastTimeToThaw = ablationDataDetail[ablationDataDetail.Count - 1].TimeToThaw;
          }

          int totalThawingTime_ = ablationDataDetail.Count(x =>
            x.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);

          var ablationSiteValue = ablationDataDetail[ablationDataDetail.Count - 1].AblationSite;
          ablationList.Add(new AblationReport(ablationDataDetail[ablationDataDetail.Count - 1].AblationID.ToString(),
                                              Enum.IsDefined(typeof(AblationSiteEnum), ablationSiteValue) ? (AblationSiteEnum)ablationSiteValue : AblationSiteEnum.UNKNOWN,
                                              duration,
                                              ablationDataDetail[ablationDataDetail.Count - 1].TemperatureRate,
                                              ablationDataDetail[ablationDataDetail.Count - 1].MaxTemperatureRate,
                                              ablationDataDetail[ablationDataDetail.Count - 1].TimeToTargetTemperature,
                                              ablationDataDetail[ablationDataDetail.Count - 1].TimeToVeinIsolation,
                                              ablationDataDetail[ablationDataDetail.Count - 1].RequiredTargetTemperature,
                                              lastTimeToThaw,
                                              ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature,
                                              ablationDataDetail[ablationDataDetail.Count - 1].CatheterId,
                                              ablationDataDetail[ablationDataDetail.Count - 1].CatheterLot,
                                              _dataAccess.GetAblationNote(ablationDataDetail[ablationDataDetail.Count - 1].AblationID, ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId),
                                              ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId,
                                              MinDMSValue,
                                              ablationDataDetail[ablationDataDetail.Count - 1].MinimumEsophagusTemperatureValue,
                                              ablationDataDetail[ablationDataDetail.Count - 1].Error,
                                              ablationDataDetail[0].TimeStamp,
                                              ablationDataDetail[ablationDataDetail.Count - 1].IsUsedForEngineering,
                                              ablationDataDetail[ablationDataDetail.Count - 1].BalloonSize,
                                              totalThawingTime_,
                                              ablationDataDetail[ablationDataDetail.Count - 1].TimeSinceVeinIsolation,
                                              ablationDataDetail[ablationDataDetail.Count - 1].TemperatureAtIsolation
                              ));
        }
        return ablationList;
      }
      set => SetProperty(ref ablationList, value);
    }

    private List<AblationReport> GetAblationReportListByProcedureRecord(List<List<AblationDataDetails>> lst)
    {
      List<AblationReport> result_ = new List<AblationReport>();
      foreach(var ablationDataDetail in lst)
      {
        var duration = ablationDataDetail.Count(
            a => a.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION || a.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION);

        var MinDMSValue = ablationDataDetail[ablationDataDetail.Count - 1].MinimumDiaphragmMovementValue;

        var lastTimeToThaw = 0;
        if(ablationDataDetail[ablationDataDetail.Count - 1].TC1Reading >= ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature)
        {
          lastTimeToThaw = ablationDataDetail[ablationDataDetail.Count - 1].TimeToThaw;
        }

        int totalThawingTime_ = ablationDataDetail.Count(x =>
          x.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);

        var ablationSiteValue = ablationDataDetail[ablationDataDetail.Count - 1].AblationSite;
        var report_ = new AblationReport(
            ablationDataDetail[ablationDataDetail.Count - 1].AblationID.ToString(),
            Enum.IsDefined(typeof(AblationSiteEnum), ablationSiteValue) ? (AblationSiteEnum)ablationSiteValue : AblationSiteEnum.UNKNOWN,
            duration,
            ablationDataDetail[ablationDataDetail.Count - 1].TemperatureRate,
            ablationDataDetail[ablationDataDetail.Count - 1].MaxTemperatureRate,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeToTargetTemperature,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeToVeinIsolation,
            ablationDataDetail[ablationDataDetail.Count - 1].RequiredTargetTemperature,
            lastTimeToThaw,
            ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature,
            ablationDataDetail[ablationDataDetail.Count - 1].CatheterId,
            ablationDataDetail[ablationDataDetail.Count - 1].CatheterLot,
            _dataAccess.GetAblationNote(ablationDataDetail[ablationDataDetail.Count - 1].AblationID, ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId),
            ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId,
            MinDMSValue,
            ablationDataDetail[ablationDataDetail.Count - 1].MinimumEsophagusTemperatureValue,
            ablationDataDetail[ablationDataDetail.Count - 1].Error,
            ablationDataDetail[0].TimeStamp,
            ablationDataDetail[ablationDataDetail.Count - 1].IsUsedForEngineering,
            ablationDataDetail[ablationDataDetail.Count - 1].BalloonSize,
            totalThawingTime_,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeSinceVeinIsolation,
            ablationDataDetail[ablationDataDetail.Count - 1].TemperatureAtIsolation
        );
        result_.Add(report_);
      }

      return result_;
    }

    /// <summary>
    /// Function/Command that handles the Procedure log command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnProcedureLogCommand(object obj)
    {
      DataLogPopup dataLogPopup = new DataLogPopup(this);
      dataLogPopup.ShowDialog();
    }

    private bool CanProcedureLogCommand(object obj) => true;

    /// <summary>
    /// Gets Hospital name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string HospitalName => _dataAccess.GetHospitalName() ?? "";

    /// <summary>
    /// Gets/sets value for CP2 Reading Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP2ReadingPlayback { get; set; }

    /// <summary>
    /// Gets/sets value for CP2 reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP2Reading
    {
      get
      {
        if(SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.CP2Reading;
        else
          return CP2ReadingPlayback; //123.123456789123456789123456789;
      }
      set
      {
        if(SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.CP2Reading = value;
        else
          CP2ReadingPlayback = value;

        RaisePropertyChanged();
      }
    }
    /// <summary>
    /// Gets/sets value for tip or balloon pressure reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TipOrBalloonPressureReading => TipPressureSelected ? EcgChannel1And2Reading : CP1Reading;

    /// <summary>
    /// Gets/sets a value to indicating whether is tip pressure selected
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool TipPressureSelected
    {
      get => TipBalloonPressureSelection.TipPressureSelected;
      set
      {
        if(TipBalloonPressureSelection.TipPressureSelected != value)
        {
          TipBalloonPressureSelection.TipPressureSelected = value;
          RaisePropertyChanged();
          TipOrBalloonPressureSelectionChangedEvent?.Invoke(null, null);
        }
      }
    }

    private Enumeration.WeightUnit weightUnit;
    public Enumeration.WeightUnit WeightUnit
    {
      get => weightUnit;
      set => SetProperty(ref weightUnit, value);
    }

    private string SecondsToMinutesIntConvert(int sec)
    {
      return (sec / 60).ToString();
    }

    /// <summary>
    /// Convert a procedure to string
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">procedure object to convert</param>
    /// <param name="parameter">procedure parameter</param>
    /// <returns></returns>
    private string ProcedureConverter(object value, string parameter)
    {
      ProcedureToStringConverter procedurevalue = new ProcedureToStringConverter();
      return procedurevalue.Convert(value, null, parameter, null).ToString();
    }

    /// <summary>
    /// Gets/sets a value for data access layer
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public object DataAccessLayer { get; internal set; }

    internal MaliciousDataChangeModel MaliciousDataChangeModelInstance { get; set; } = MaliciousDataChangeModel.Instance;

    /// <summary>
    /// get latest treatment note
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private ProcedureRecords GetLatestTreatmentNote(ProcedureRecords procRec)
    {
      List<AblationReportChanges> _ablationReportChanges = MaliciousDataChangeModelInstance.AblationReportChanges;
      foreach(Ablation ablation in procRec.Procedure.Ablations)
      {
        ablation.TreatmentNote = _dataAccess.GetAblationNote(ablation.AblationNumber, procRec.Procedure.Id);
      }
      return procRec;
    }

    public bool ActionLogExported { get; set; }
    public bool ErrorLogExported { get; set; }
    public bool SmartFreezeLogExported { get; set; }
    public bool WinEventLogExported { get; set; }

    private string _logMessage = string.Empty;
    public string LogMessage
    {
      get => _logMessage;
      set => SetProperty(ref _logMessage, value);
    }

    public int ProcedureRecordsCount => SelectedProceduresList.Count;

    public int LogFileCount => 4;

    private int _logProgressBarValue;
    public int LogProgressBarValue
    {
      get => _logProgressBarValue;
      set
      {
        SetProperty(ref _logProgressBarValue, value);
        USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
      }
    }

    private void ZipLogsToUsb(DirectoryInfo directoryInfo, CancellationToken cancellationToken)
    {
      try
      {
        var csn_ = _dataAccess?.GetConsoleSerialNumber() ?? string.Empty;
        var lst_ = SelectedProceduresList.ToList();
        using(var service_ = new DataExportService(UserType.Bsc, directoryInfo, lst_, csn_))
        {
          var result_ = service_.ExportLogFile(this, cancellationToken);
          if(!File.Exists(result_?.FullName) && cancellationToken.IsCancellationRequested)
          {
            LogMessage = "Export log files cancelled.";
          }
        }
      }
      catch(Exception e)
      {
        LogException(e);
      }
    }

    /// <summary>
    /// This function generate catheter type list 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void GetCatheterTypeList()
    {
      int CatheterTypeID = 0;
      List<Enumeration.CatheterType> TypeList = new List<Enumeration.CatheterType>();
      for(int i = 0; i < ablationList.Count; i++)
      {
        if(ablationList[i].IsUsedForEngineering == true || ablationList[i].CatheterId == 0)
          CatheterTypeID = 0;
        else
          CatheterTypeID = _dataAccess.GetCatheterTypeById(ablationList[i].CatheterId);

        if(!IsInAready(TypeList, CatheterTypeID))
          TypeList.Add((Enumeration.CatheterType)CatheterTypeID);
      }
      CatheterTypeList = TypeList;
    }

    /// <summary>
    /// This function verify it the catheter type already in the list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool IsInAready(List<Enumeration.CatheterType> list, int ID)
    {
      bool returnvalue = false;
      foreach(Enumeration.CatheterType item in list)
      {
        if(item == (Enumeration.CatheterType)ID)
        {
          returnvalue = true;
          break;
        }
      }
      return returnvalue;
    }


    /// <summary>
    /// Get procedure logs list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="PId">log ID</param>
    /// <returns>all loged procedure</returns>
    private List<string> GetProcedureLogsList(int PId)
    {
      ObservableCollection<ProcedureLog> PLog = CommonViewModel.Current.Data.DataAccess.GetAllProcedureLogsAccordingToProcedureID(PId);
      int procdureLogCount = PLog.Count;
      List<string> procdureLogstring = new List<string>();
      for(int i = 0; i < procdureLogCount; i++)
      {
        procdureLogstring.Add(PLog[i].LogDate.ToString() + " : " + PLog[i].Description + "  from " + PLog[i].PreviousInformation + " to " + PLog[i].CommittedInformation);
      }
      return procdureLogstring;
    }

    /// <summary>
    /// Get a string that contains translated header for AblationDetails Tab
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>string of translated header</returns>
    private List<String> GetAblationDetailsCSVHeaders()
    {
      List<string> ablationdetailslist = new List<string>();

      if(_userType == UserType.Doctor || _userType == UserType.Admin)
      //if(fileType.ToLower() == DoctorFileType || fileType.ToLower() == AdminFileType)
      {
        ablationdetailslist.Add(FieldToTextConverterobject("TimeStampUID", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("TimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
        ablationdetailslist.Add(FieldToTextConverterobject("AblationIDUID", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("SystemStateUID", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("TemperatureRateUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + "/" + FieldToTextConverterobject("SecLabel", null, null) + ")");
        ablationdetailslist.Add("Balloon Temperature" + " (TC1)" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
        ablationdetailslist.Add(FieldToTextConverterobject("DMSValueUID", null, null) + " (G)");
        ablationdetailslist.Add(FieldToTextConverterobject("DMSValueUID", null, null) + " (%)");
        ablationdetailslist.Add(FieldToTextConverterobject("Displayed Eso. Temp", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 1", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 2", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 3", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 4", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 5", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 6", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 7", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 8", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 9", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 10", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 11", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 12", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 13", null, null));
      }
      else if(_userType == UserType.BostonBsc || _userType == UserType.Bsc)
      {
        ablationdetailslist.Add("Timestamp");
        ablationdetailslist.Add("Time");
        ablationdetailslist.Add("ID");
        ablationdetailslist.Add("State");
        ablationdetailslist.Add("TR");
        ablationdetailslist.Add("TC1"); // 
        ablationdetailslist.Add("TC1CJ"); // PMCU CJ READING
        ablationdetailslist.Add("PT1"); // 
        ablationdetailslist.Add("PT2");
        ablationdetailslist.Add("PT3");
        ablationdetailslist.Add("PT4");
        ablationdetailslist.Add("PT5");
        ablationdetailslist.Add("PS1");
        ablationdetailslist.Add("FM1");
        ablationdetailslist.Add("TS1");
        ablationdetailslist.Add("TN2O");
        ablationdetailslist.Add("LC1");
        ablationdetailslist.Add("IBP");
        ablationdetailslist.Add("OBP");
        ablationdetailslist.Add("TS1CJ"); // CMCU CJ READING
        ablationdetailslist.Add("IPWM"); // PWMINJ
        ablationdetailslist.Add("BPWM");
        ablationdetailslist.Add("DMS (G)");
        ablationdetailslist.Add("DMS %");
        ablationdetailslist.Add("BDI");
        ablationdetailslist.Add("ESO TEMP");
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 1", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 2", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 3", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 4", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 5", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 6", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 7", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 8", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 9", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 10", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 11", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 12", null, null));
        ablationdetailslist.Add(FieldToTextConverterobject("Eso. Ch 13", null, null));
      }

      return ablationdetailslist;
    }

    /// <summary>
    /// Gets a string list that contains translated headers for Doctor treatments in GeneralInfo tab.  
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>a string list</returns>
    private List<String> GetTreatmentCSVHeader()
    {
      List<string> doctortreatmentList = new List<string>();

      doctortreatmentList.Add(FieldToTextConverterobject("AblationSiteLabel", null, null));
      doctortreatmentList.Add("Balloon Size");
      doctortreatmentList.Add("Minimum Temperature" + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("CoolingTimerSetpointUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("CoolingTimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("AblationDurationSetpointUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("timetoveinisolationUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("ThawTimeUID", null, null) + " (" + FieldToTextConverterobject("SecLabel", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("ThawTimerSetpointUID", null, null) + " (" + FieldToTextConverterobject("celsiusTransUID", null, null) + ")");
      doctortreatmentList.Add(FieldToTextConverterobject("CatheterIDUID", null, null));
      doctortreatmentList.Add(FieldToTextConverterobject("CatheterLotNumUID", null, null));
      doctortreatmentList.Add(FieldToTextConverterobject("CatheterSerialUID", null, null));
      doctortreatmentList.Add(FieldToTextConverterobject("MinimumDMSValueUID", null, null) + " (%)");
      doctortreatmentList.Add("Minimum Esophagus Temperature");

      return doctortreatmentList;
    }


    /// <summary>
    /// Get translated text by passing GUIUID value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <returns>a translated string</returns>
    private string FieldToTextConverterobject(object value, System.Type targetType, object parameter)
    {
      FieldToTextConverter fieldtotextValue = new FieldToTextConverter();
      return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
    }

    /// <summary>
    /// Set procedure value based on parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string ProcedureToStringConverterobject(object value, System.Type targetType, object parameter)
    {
      string returnresult = " ";
      ProcedureToStringConverter fieldtotextValue = new ProcedureToStringConverter();
      returnresult = fieldtotextValue.Convert(value, targetType, parameter, null).ToString();

      if(returnresult == "00")
        returnresult = "--";

      return returnresult;
    }

    /// <summary>
    /// Set toise unit value based on parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string ToiseUnitToTextConverterobject(object value, System.Type targetType, object parameter)
    {
      ToiseUnitToTextConverter fieldtotextValue = new ToiseUnitToTextConverter();
      return fieldtotextValue.Convert(value, targetType, parameter, null).ToString();
    }

    #region IDisposable Implementation

    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
      if(!disposedValue)
      {
        if(disposing)
        {
          // (1) dispose managed state (managed objects)

          // (2) unsubscribe events
          CommonViewModel.Current.PropertyChanged -= CommonViewModel_PropertyChanged;
          PropertyChanged -= TreatmentRecordsViewModel_PropertyChanged;
          _ProcedureRecordsListSubscription.Dispose();
        }

        // (3) free unmanaged resources (unmanaged objects) and override finalizer
        _usbDriveConnectionManager.Dispose();
        _dataAccess.Dispose();

        // (4) set large fields to null
        _allProcedures = null;
        disposedValue = true;
      }
    }

    // override finalizer to free unmanaged resources
    ~TreatmentRecordsViewModel()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: false);
    }

    private List<AblationDataDetails> LoadAblationFromFile(JsonManager fs, string filename)
    {
      try
      {
        var ablationData = fs.DeserializeAblationData<AblationFileDataStruct>(filename);

        return ablationData != null
            ? ablationData.ConvertToAblationDataDetails()
            : fs.DeserializeAblationData<List<AblationDataDetails>>(filename);
      }
      catch(Exception e)
      {
        return null;
      }
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation
    
    public void RefreshPropertyChanged()
    {
      this.RaisePropertyChanged(nameof(NavigatedProcedureRecords));
    }

    #region INotifyDataErrorInfo Interface

    public IEnumerable GetErrors(string propertyName)
      => _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;

    public bool HasErrors => _errorsByPropertyName.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    #endregion

    #region INotifyDataErrorInfo Implementation

    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
    private readonly Regex _passwordValidationRegex = new Regex("^(?=.*[0-9]).{8,16}$", RegexOptions.Compiled);
    private void AddError(string propertyName, string error)
    {
      if(!_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName[propertyName] = new List<string>();
      }
      if(!_errorsByPropertyName[propertyName].Contains(error))
      {
        _errorsByPropertyName[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
      }
    }

    public void ClearErrors(string propertyName)
    {
      if(_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName.Remove(propertyName);
        RaiseErrorsChanged(propertyName);
      }
    }

    private void RaiseErrorsChanged(string propertyName)
      => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    private void ValidatePassword(string pw)
    {
      ClearErrors(nameof(FilePassword));
      if(string.IsNullOrEmpty(pw))
      {
        AddError(nameof(FilePassword), UIConstants.PasswordEmptyErrorMessage);
        return;
      }
      if(!_passwordValidationRegex.IsMatch(pw))
      {
        AddError(nameof(FilePassword), UIConstants.PasswordInvalidMessage);
        IsPasswordValid = false;
      }
      else
      {
        IsPasswordValid = true;
      }
    }

    private void ValidateConfirmPassword(string cpw)
    {
      ClearErrors(nameof(ConfirmPassword));
      if(cpw != FilePassword || !_passwordValidationRegex.IsMatch(FilePassword))
      {
        AddError(nameof(ConfirmPassword), UIConstants.PasswordNotMatchMessage);
        IsPasswordConfirmed = false;
      }
      else
      {
        IsPasswordConfirmed = true;
      }
    }

    #endregion
  }
}