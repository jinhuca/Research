using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Communication;
using Prism.Commands;
using Shared;
using static LogSystem.LogService;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the Report View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class ReportViewModel : BindableBase, IAblationSiteAware
  {
    public ICommand DiagnosisCommand { get; private set; }
    public ICommand OutcomeCommand { get; private set; }

    public ICommand ProcedureLogCommand { get; private set; }

    private string currentPhysicianName = "--";

    private AblationSiteEnum ablationSite = AblationSiteEnum.OTHER;

    private bool displayAblationSiteWarning;

    private ObservableCollection<AblationReport> ablationList;

    private bool isUsingCryterionBallon = false;
    private Enumeration.WeightUnit weightUnit;

    private int skinToSkinDuration = 0;
    List<Enumeration.CatheterType> catheterTypeList;
    Enumeration.CatheterType catheterType = 0;

    private int currentTreatmentNumber = 0;

    private MaliciousDataChangeModel maliciousDataChangeModel = MaliciousDataChangeModel.Instance;

    private readonly DataAccess _dataAccess; 

    /// <summary>
    /// This constructor initializes the Reports View Model's properties and data access
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ReportViewModel()
    {
      this.DiagnosisCommand = new DelegateCommand<object>(this.OnDiagnosisCommand, this.CanDiagnosisCommand);
      this.OutcomeCommand = new DelegateCommand<object>(this.OnOutcomeCommand, this.CanOutcomeCommand);
      this.ProcedureLogCommand = new DelegateCommand<object>(this.OnProcedureLogCommand, this.CanProcedureLogCommand);

      AblationList = new ObservableCollection<AblationReport>();

      CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;

      _dataAccess = CommonViewModel.Current.Data.DataAccess;
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The parameter's name that has changed.</param>
    private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      CommonViewModel commonviewmodel = sender as CommonViewModel;

      switch (e.PropertyName)
      {
        case "CurrentPatient":
          RaisePropertyChanged("CurrentPatient");
          break;

        case "AblationSummary":
          RaisePropertyChanged("AblationSummary");
          break;

        case "CurrentProcedure":
          RaisePropertyChanged("CurrentProcedure");
          break;

        case "AblationList":
          RaisePropertyChanged("AblationList");
          break;
      }
    }

    private bool _IsPatientInfoVisibilityMutable;

    public bool IsPatientInfoVisibilityMutable
    {
      get => _IsPatientInfoVisibilityMutable;
      set => SetProperty(ref _IsPatientInfoVisibilityMutable, value);
    }

    public bool IsPatientInfoVisible
    {
      get
      {
        if (CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser)
        {
          return false;
        }

        if (!IsPatientInfoVisibilityMutable)
        {
          return true;
        }

        return NotificationModel.Instance?.CurrentPhysician?.preference != null 
               && NotificationModel.Instance.CurrentPhysician.preference.IsShowPatientInfo;
      }

      set
      {
        if(!IsPatientInfoVisibilityMutable)
        {
          return;
        }

        try
        {
          if(NotificationModel.Instance?.CurrentPhysician?.preference != null)
          {
            NotificationModel.Instance.CurrentPhysician.preference.IsShowPatientInfo = value;
            NotificationModel.Instance.SaveNotification();
          }
        }
        catch(Exception ex)
        {
          LogException(ex);
        }
        finally
        {
          RaisePropertyChanged();
          RaisePropertyChanged(nameof(NotificationModel.CurrentPhysician.preference.IsShowPatientInfo));
        }
      }
    }

    /// <summary>
    /// This read-only property returns if the current user has Cryterion type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDoctor
    {
      get
      {
        return CommonViewModel.Current.IsDoctor;
      }
    }

    /// <summary>
    /// This read-only property returns if the current physician full name.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CurrentPhysician
    {
      get
      {
        if (IsDoctor)
        {
          Physician Pphysician = CommonViewModel.Current.Data.DataAccess.GetphysicianByID(CommonViewModel.Current.CurrentUser.Id);
          currentPhysicianName = "Dr. " + Pphysician.FirstName + " " + Pphysician.LastName;
        }
        else if (CommonViewModel.Current.IsAdminUser)
        {
          Physician Pphysician = CommonViewModel.Current.CurrentPatient.Physician;
          currentPhysicianName = "Dr. " + Pphysician.FirstName + " " + Pphysician.LastName;
        }
        else
          currentPhysicianName = "--";

        return currentPhysicianName;
      }
    }



    /// <summary>
    /// This property gets/sets display warning value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DisplayAblationSiteWarning
    {
      get
      {
        return this.displayAblationSiteWarning;
      }
      set
      {
        this.displayAblationSiteWarning = value;
        RaisePropertyChanged("DisplayAblationSiteWarning");
      }
    }
    /// <summary>
    /// This property gets or sets the ablation site
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AblationSiteEnum AblationSite
    {
      get
      {
        return this.ablationSite;
      }
      set
      {
        if (value != AblationSiteEnum.UNKNOWN)
        {
          if (MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated)
          {
            if (this.ablationSite != value)
            {

              //Update the procedure's diagnosis in the database
              ProcedureLog newProcedureLog = new ProcedureLog();

              newProcedureLog.Description = "Treatment " + CurrentTreatmentNumber.ToString() + " Ablation Site Change";
              newProcedureLog.LogDate = DateTime.Now;

              newProcedureLog.PreviousInformation = AblationList[CurrentTreatmentNumber - 1].AblationSite.GetDescription();

              newProcedureLog.CommittedInformation = value.ToString();
              newProcedureLog.ProcedureId = CommonViewModel.Current.CurrentProcedure.Id;

              CommonViewModel.Current.Data.DataAccess.AddProcedureLog(newProcedureLog);

              this.ablationSite = value;
              RaisePropertyChanged("AblationSite");

              CommonViewModel.Current.CurrentProcedure.IsDataEdited = true;
              CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
              this.RaisePropertyChanged(nameof(CurrentProcedure));

              ChangeAblationSite(value);
            }
          }
          this.ablationSite = value;
          RaisePropertyChanged("AblationSite");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the current patient value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DataAccessLayer.Patient CurrentPatient
    {
      get { return CommonViewModel.Current.CurrentPatient; }
      set
      {
        RaisePropertyChanged("CurrentPatient");
      }
    }

    /// <summary>
    /// This property gets/sets the current procedure value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Procedure CurrentProcedure
    {
      get
      {
        if (CommonViewModel.Current?.CurrentProcedure != null)
        {
          IsUsingCryterionBallon = true;

        }
        else
        {

          IsUsingCryterionBallon = false;
        }

        return CommonViewModel.Current.CurrentProcedure;

      }
      set
      {
        RaisePropertyChanged("CurrentProcedure");
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Outcome command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanOutcomeCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the operations to get the Ablation Outcome text captured and saved in the database when
    /// the Outcome command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    private void OnOutcomeCommand(object obj)
    {
      string previousOutCome = CommonViewModel.Current.CurrentProcedure.OutCome;

      var treatmentNotesPopup = new TextEntryPopupNew(this, CommonViewModel.TextEntryType.Outcome);
      treatmentNotesPopup.ShowDialog();

      try
      {

        //Update the procedure's outcome in the database
        CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
        RaisePropertyChanged("CurrentProcedure");

        if (MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated)
        {
          string newOutcome = CommonViewModel.Current.CurrentProcedure.OutCome;

          if (!string.Equals(previousOutCome, newOutcome))
          {

            //Update the procedure's diagnosis in the database
            ProcedureLog newProcedureLog = new ProcedureLog();

            newProcedureLog.Description = "Outcome Change";
            newProcedureLog.LogDate = DateTime.Now;
            newProcedureLog.PreviousInformation = previousOutCome;
            newProcedureLog.CommittedInformation = newOutcome;
            newProcedureLog.ProcedureId = CommonViewModel.Current.CurrentProcedure.Id;

            CommonViewModel.Current.Data.DataAccess.AddProcedureLog(newProcedureLog);

            ChangeAblationOrOutcome(newOutcome);

            CommonViewModel.Current.CurrentProcedure.IsDataEdited = true;
            CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
            this.RaisePropertyChanged(nameof(CurrentProcedure));
          }

        }

      }
      catch (Exception ex)
      {
        LogException(ex);
        Tuple<long, string, string, string> genericMessage55 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID55, (int)Enumeration.ErrorTypes.GUI);
        Tuple<long, string, string, string> genericMessage56 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID56, (int)Enumeration.ErrorTypes.GUI);

        //MessagePopup messagePopup = new MessagePopup(genericMessage55.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, genericMessage55.Item2);
        MessagePopup messagePopup = new MessagePopup(genericMessage55.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "");
        messagePopup.ShowDialog();
      }
    }

    /// <summary>
    /// Function/Command that handles the operations to get the Ablation Diagnosis text captured and saved in the database when
    /// the Diagnosis command is invoked.
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    private void OnDiagnosisCommand(object obj)
    {
      string previousDiagnosis = CommonViewModel.Current.CurrentProcedure.Diagnosis;
      var treatmentNotesPopup = new TextEntryPopupNew(this, CommonViewModel.TextEntryType.Diagnosis);
      treatmentNotesPopup.ShowDialog();

      try
      {
        CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
        RaisePropertyChanged("CurrentProcedure");

        if (MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated)
        {
          string newDiagnosis = CommonViewModel.Current.CurrentProcedure.Diagnosis;

          if (!string.Equals(previousDiagnosis, newDiagnosis))
          {

            //Update the procedure's diagnosis in the database
            ProcedureLog newProcedureLog = new ProcedureLog();

            newProcedureLog.Description = "Diagnosis Change";
            newProcedureLog.LogDate = DateTime.Now;
            newProcedureLog.PreviousInformation = previousDiagnosis;
            newProcedureLog.CommittedInformation = newDiagnosis;
            newProcedureLog.ProcedureId = CommonViewModel.Current.CurrentProcedure.Id;

            CommonViewModel.Current.Data.DataAccess.AddProcedureLog(newProcedureLog);

            ChangeAblationDiagnosis(newDiagnosis);

            CommonViewModel.Current.CurrentProcedure.IsDataEdited = true;
            CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
            this.RaisePropertyChanged(nameof(CurrentProcedure));
          }
        }


      }
      catch (Exception ex)
      {
        LogException(ex);
        Tuple<long, string, string, string> genericMessage57 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID57, (int)Enumeration.ErrorTypes.GUI);
        Tuple<long, string, string, string> genericMessage58 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID58, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup messagePopup = new MessagePopup(genericMessage57.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, ""); //genericMessage58.Item2
        messagePopup.ShowDialog();
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Diagnosis command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanDiagnosisCommand(object obj)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the procedure log
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">object</param>

    private void OnProcedureLogCommand(object obj)
    {
      DataLogPopup dataLogPopup = new DataLogPopup(this);
      dataLogPopup.ShowDialog();
    }


    /// <summary>
    /// Function that returns if the system can invoke the Procedure Log Command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanProcedureLogCommand(object obj)
    {
      return true;
    }
    /// <summary>
    /// Gets/sets procedure log
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ObservableCollection<ProcedureLog> ProcedureLogs
    {
      get
      {
        return CommonViewModel.Current.Data.DataAccess.GetAllProcedureLogsAccordingToProcedureID(CurrentProcedure.Id);
      }

      set
      {
        RaisePropertyChanged("ProcedureLogs");
      }
    }

    /// <summary>
    /// This read-only property returns the Ablation Summary object
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Models.AblationSummary AblationSummary
    {
      get { return CommonViewModel.Current.AblationSummary; }
    }

    /// <summary>
    /// This read-only property returns the Procedure Date
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string ProcedureDate
    {
      get
      {
        return CommonViewModel.Current?.CurrentProcedure?.ProcedureStartDateTime.ToString("MMMM dd, yyyy");
      }
    }

    /// <summary>
    /// Gets hospital name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private string _hospitalName = string.Empty;
    public string HospitalName
    {
      get
      {
        if (string.IsNullOrEmpty(_hospitalName))
        {
          _hospitalName = _dataAccess.GetHospitalName();
        }

        return _hospitalName;
      }
    }

    /// <summary>
    /// This property gets/sets the Ablation List (Collection of AblationReport object)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ObservableCollection<AblationReport> AblationList
    {
      get
      {
        ablationList.Clear();
        int ablationNum = 0;

        foreach (var ablationDataDetail in CommonViewModel.Current.AllAblationDataList.Where(a => a.Any()))
        {

          int duration = ablationDataDetail.Count(a => a.SystemState == (int)Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION ||
                                                       a.SystemState == (int)Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);

          int minDMSValue = ablationDataDetail[ablationDataDetail.Count - 1].MinimumDiaphragmMovementValue;

          /*****************************************************************************/

          int lastTimeToThaw = 0;
          if (ablationDataDetail[ablationDataDetail.Count - 1].TC1Reading >= ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature)
          {
            lastTimeToThaw = ablationDataDetail[ablationDataDetail.Count - 1].TimeToThaw;
          }

          int totalThawingTime_ = ablationDataDetail.Count(x =>
            x.SystemState == (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);

          var ablationSiteValue = ablationDataDetail[ablationDataDetail.Count - 1].AblationSite;

          ablationList.Add(new 
            AblationReport(ablationDataDetail[ablationDataDetail.Count - 1].AblationID.ToString(),
            Enum.IsDefined(typeof(AblationSiteEnum), ablationSiteValue) ? (AblationSiteEnum)ablationSiteValue : AblationSiteEnum.UNKNOWN,
            duration,
            ablationDataDetail[ablationDataDetail.Count - 1].TemperatureRate,
            ablationDataDetail[ablationDataDetail.Count - 1].MaxTemperatureRate,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeToTargetTemperature,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeToVeinIsolation,
            ablationDataDetail[ablationDataDetail.Count - 1].RequiredTargetTemperature,
            // ablationDataDetail[ablationDataDetail.Count - 1].TimeToThaw,
            lastTimeToThaw,
            ablationDataDetail[ablationDataDetail.Count - 1].ThawTimerToTemperature,
            ablationDataDetail[ablationDataDetail.Count - 1].CatheterId,
            ablationDataDetail[ablationDataDetail.Count - 1].CatheterLot,
            _dataAccess.GetAblationNote(ablationDataDetail[ablationDataDetail.Count - 1].AblationID, ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId),
            ablationDataDetail[ablationDataDetail.Count - 1].ProcedureId,
            minDMSValue,
            ablationDataDetail[ablationDataDetail.Count - 1].MinimumEsophagusTemperatureValue,
            ablationDataDetail[ablationDataDetail.Count - 1].Error,
            ablationDataDetail[0].TimeStamp,
            ablationDataDetail[ablationDataDetail.Count - 1].IsUsedForEngineering,
            ablationDataDetail[ablationDataDetail.Count - 1].BalloonSize,
            totalThawingTime_,
            ablationDataDetail[ablationDataDetail.Count - 1].TimeSinceVeinIsolation,
            ablationDataDetail[ablationDataDetail.Count -1].TemperatureAtIsolation
          ));

          ablationNum++;
        }

        if (CommonViewModel.Current.CurrentAblation == null)
        {
          InBodyTime = CommonViewModel.Current.CurrentProcedure.SkinToSkinDuration / 60;
        }

        return ablationList;
      }
      set
      {
        ablationList = value;
        RaisePropertyChanged(nameof(AblationList));
      }
    }

    /// <summary>
    /// Gets/sets data access
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DataAccess DataAccess => _dataAccess;

    /// <summary>
    /// Gets/sets a value indicating whether is using cryterion balloon
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingCryterionBallon { get => isUsingCryterionBallon; set => isUsingCryterionBallon = value; }

    /// <summary>
    /// Gets or sets a value for weight unit
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Enumeration.WeightUnit WeightUnit
    {
      get
      {
        return weightUnit;
      }
      set
      {
        weightUnit = value;
        RaisePropertyChanged("WeightUnit");
      }
    }
    /// <summary>
    /// Gets or sets a value for skin to skin duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int SkinToSkinDuration
    {
      get
      {
        return skinToSkinDuration;
      }
      set
      {
        skinToSkinDuration = value;
        RaisePropertyChanged("SkinToSkinDuration");
      }
    }

    private int _inBodyTime;
    public int InBodyTime
    {
      get => _inBodyTime;
      set => SetProperty(ref _inBodyTime, value);
    }

    /// <summary>
    /// Gets or sets a value for catheter type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Enumeration.CatheterType CatheterType
    {                        
      get
      {
        return catheterType;
      }

      set
      {
        catheterType = value;
        RaisePropertyChanged("CatheterType");
      }
    }
    /// <summary>
    /// Gets or sets a value for malicious data change model 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal MaliciousDataChangeModel MaliciousDataChangeModel
    {
      get
      {
        return maliciousDataChangeModel;
      }

      set
      {
        maliciousDataChangeModel = value;
      }
    }
    /// <summary>
    /// Gets or sets a value for current treatment number
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CurrentTreatmentNumber
    {
      get
      {
        return currentTreatmentNumber;
      }

      set
      {
        currentTreatmentNumber = value;
      }
    }


    /// <summary>
    /// This function is for reload procedure data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// Fix Bug #559:summary report screen did not update when update the Vein Isolation was changed from playback mode
    public void ReloadProcedureData()
    {
      if (CommonViewModel.Current.CurrentProcedure != null)
      {
        RaisePropertyChanged("CurrentProcedure");
        RaisePropertyChanged("CurrentPhysician");
        MaliciousDataChangeModel.AblationReportChanges.Clear();
        MaliciousDataChangeModel.IsDataEdited = false;
        GetCatheterTypeList();
        CurrentTreatmentNumber = 0;
      }

      if (CommonViewModel.Current.CurrentPatient != null)
        RaisePropertyChanged("CurrentPatient");

      if (CommonViewModel.Current.AllAblationDataList != null)
        RaisePropertyChanged("AblationList");


      RaisePropertyChanged("IsDoctor");
      RaisePropertyChanged("WeightUnit");

    }
    /// <summary>
    /// This function is for change ablation site
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ChangeAblationSite(AblationSiteEnum _ablationSite)
    {
      MaliciousDataChangeModel.IsDataEdited = true;

      MaliciousDataChangeModel.PopulateNewReoprtchange(new AblationReportChanges(CommonViewModel.Current.CurrentProcedure.Id, CurrentTreatmentNumber, _ablationSite,
      AblationList[CurrentTreatmentNumber - 1].Notes, CommonViewModel.Current.CurrentProcedure.Diagnosis, CommonViewModel.Current.CurrentProcedure.OutCome));
    }

    /// <summary>
    /// This function is for change ablation diagnosis
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ChangeAblationDiagnosis(string diagnosis)
    {
      // all treatment have one note and one diagnosis so the CurrentTreatmentNumber can considired 1 

      MaliciousDataChangeModel.IsDataEdited = true;

      MaliciousDataChangeModel.PopulateNewReoprtchange(new AblationReportChanges(CommonViewModel.Current.CurrentProcedure.Id, 1, AblationSite,
      AblationList[0].Notes, diagnosis, CommonViewModel.Current.CurrentProcedure.OutCome));
    }
    /// <summary>
    /// This function is for change ablation outcome
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ChangeAblationOrOutcome(string outcome)
    {

      // all treatment have one note and one diagnosis so the CurrentTreatmentNumber can considired 1 

      MaliciousDataChangeModel.IsDataEdited = true;

      MaliciousDataChangeModel.PopulateNewReoprtchange(new AblationReportChanges(CommonViewModel.Current.CurrentProcedure.Id, 1, AblationSite,
      AblationList[0].Notes, CommonViewModel.Current.CurrentProcedure.Diagnosis, outcome));
    }

    /// <summary>
    /// This function is for change ablation note
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ChangeAblationNote(string previousInformation, string committedInformation)
    {
      MaliciousDataChangeModel.IsDataEdited = true;

      ProcedureLog newProcedureLog = new ProcedureLog();

      newProcedureLog.Description = "Treatment " + CurrentTreatmentNumber.ToString() + " Note Change";
      newProcedureLog.LogDate = DateTime.Now;
      newProcedureLog.PreviousInformation = previousInformation;
      newProcedureLog.CommittedInformation = committedInformation;
      newProcedureLog.ProcedureId = CommonViewModel.Current.CurrentProcedure.Id;

      CommonViewModel.Current.Data.DataAccess.AddProcedureLog(newProcedureLog);

      CommonViewModel.Current.CurrentProcedure.IsDataEdited = true;
      CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
      this.RaisePropertyChanged(nameof(CurrentProcedure));

      MaliciousDataChangeModel.PopulateNewReoprtchange(new AblationReportChanges(CommonViewModel.Current.CurrentProcedure.Id, CurrentTreatmentNumber, AblationSite,
      committedInformation, CommonViewModel.Current.CurrentProcedure.Diagnosis, CommonViewModel.Current.CurrentProcedure.OutCome));
    }
    /// <summary>
    /// This function is for reset ablation site
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetAblationSite(AblationSiteEnum _ablationSite)
    {
      this.ablationSite = _ablationSite;
    }

    /// <summary>
    /// This read-only property returns if the current user has the Cryterion type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// Fix Bug #560: Logged in as BSC,  the Summary Report screen shows BMI.
    public bool IsCryterionUser
    {
      get
      {
        return CommonViewModel.Current.IsCryterionUser;
      }
    }

    /// <summary>
    /// This function get/set catheter type list value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<Enumeration.CatheterType> CatheterTypeList
    {
      get
      {
        return catheterTypeList;
      }

      set
      {
        catheterTypeList = value;
        RaisePropertyChanged("CatheterTypeList");
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
      for (int i = 0; i < ablationList.Count; i++)
      {

        if (ablationList[i].IsUsedForEngineering == true || ablationList[i].CatheterId == 0)
          CatheterTypeID = 0;
        else
          CatheterTypeID = _dataAccess.GetCatheterTypeById(ablationList[i].CatheterId);

        if (!IsInAready(TypeList, CatheterTypeID))
          TypeList.Add((Enumeration.CatheterType)CatheterTypeID);

      }

      if (TypeList?.Count != 0)
        CatheterTypeList = TypeList;


    }

    /// <summary>
    /// This function verify it the catheter type already in the list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool IsInAready(List<Enumeration.CatheterType> list, int ID)
    {
      bool returnvalue = false;
      foreach (Enumeration.CatheterType item in list)
      {
        if (item == (Enumeration.CatheterType)ID)
        {
          returnvalue = true;
          break;
        }
      }
      return returnvalue;
    }

    public void UpdateAblationSiteChanged(AblationSiteEnum newAblationSite)
    {
      CommonViewModel.Current.UpdateAblationSite(CurrentTreatmentNumber, newAblationSite);
      CommonViewModel.Current.GenerateAblationSummary();  
    }

  }
}