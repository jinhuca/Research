using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the Patient View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class PatientViewModel : BindableBase
  {
    private static DateTime _defaultBirthDate = new DateTime(1960, 6, 15); 
    private static DateTime _defaultBirthDateInDB = new DateTime(1800, 1, 1);
    private DateTime? birthDate = null;


    private bool isDateSelected = false;

    private bool isPatientFoundInDatabase = false;
    private bool isPhysicianSelectionEnabled = true;
    private bool isTherePatient = false;

    private GenderType gender = GenderType.None;

    private ObservableCollection<string> physiciansName = new ObservableCollection<string>();
    private ObservableCollection<Physician> Physicians = new ObservableCollection<Physician>();
    private string selectedPhysician = string.Empty;
    private string physicianFullName = string.Empty;
    private string hospitalPatientId = string.Empty;
    private string firstName = string.Empty;
    private string lastName = string.Empty;
    private string height = string.Empty;
    private string weight = string.Empty;
    private Enumeration.WeightUnit weightUnit;

    private bool isPatientInfoValid = false;

    private const double MIN_WEIGHT = 0;
    private const double MAX_WEIGHT = 657;
    private const double MIN_HEIGHT = 0;
    private const double MAX_HEIGHT = 299;

    private const double maximumProcedureDuration = 12;

    public ICommand NextCommand { get; private set; }
    public ICommand SearchPatientCommand { get; private set; }

    public ICommand BirthDateCommand { get; private set; }

    public ICommand LoadPreviousPatientCommand { get; private set; }
    public ICommand BackToHomeCommand { get; private set; }

    /// <summary>
    /// This constructor initializes the Patient View Model properties and commands
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public PatientViewModel()
    {
      this.NextCommand = new DelegateCommand<object>(this.OnNextCommand).ObservesCanExecute(() => IsPatientInfoValid);
      this.SearchPatientCommand = new DelegateCommand<object>(this.OnSearchPatient, this.CanSearchPatient);
      this.LoadPreviousPatientCommand = new DelegateCommand<object>(this.OnLoadPreviousPatientCommand, this.CanLoadPreviousPatientCommand);
      BackToHomeCommand = new DelegateCommand(ExecuteBackToHomeCommand, () => true);
      CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
    }

    /// <summary>
    /// Function that returns if the system can invoke the Birth Date command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanBirthDateCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function that returns if the system can invoke the load previous patient command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanLoadPreviousPatientCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Birth Date logic when the BirthDate  command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command's parameter (not used in this function).</param>
    private void OnLoadPreviousPatientCommand(object obj)
    {
      //AppTrace.Log("Started Command to load previous patient.", LogLevel.Debug);
      // CommonViewModel.Current.CurrentProcedure = ProcedureLogModel.PreviousProcedure;
      this.HospitalPatientId = ProcedureLogModel.PreviousLogedPatient.HospitalPatientId;
      OnSearchPatient(null);
      IsPhysicianSelectionEnabled = false;
      //CommonViewModel.Current.CurrentProcedure = ProcedureLogModel.PreviousProcedure;
      //CommonViewModel.Current.AreSensorsInPlayBackMode = true;
      ProcedureLogModel.CanReloadProcudreInformation = true;
      //AppTrace.Log("Completed command to load previous patient.", LogLevel.Debug);
    }

    /*********************************/

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "PhysicianList":
          RaisePropertyChanged("PhysiciansList");
          RaisePropertyChanged("PhysiciansComList");
          break;
      }
    }

    /// <summary>
    /// This property gets/sets Birthdate value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DateTime? Birthdate
    {
      get
      {
        return birthDate;
      }
      set
      {
        birthDate = value;
        RaisePropertyChanged("Birthdate");
        IsDateSelected = birthDate.HasValue;

        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets the Is Physician Selection Enabled flag
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsPhysicianSelectionEnabled
    {
      get
      {
        return isPhysicianSelectionEnabled;
      }

      set
      {
        isPhysicianSelectionEnabled = value;
        RaisePropertyChanged("IsPhysicianSelectionEnabled");
      }
    }

    /// <summary>
    /// This property gets/sets Gender value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public GenderType Gender
    {
      get
      {
        if (CommonViewModel.Current.CurrentPatient != null)
        {
          return Enum.IsDefined(typeof(GenderType), CommonViewModel.Current.CurrentPatient.Gender)
                   ? (GenderType)CommonViewModel.Current.CurrentPatient.Gender
                   : GenderType.None;
        }

        return this.gender;
      }
      set
      {
        SetProperty(ref this.gender, value);

        if (CommonViewModel.Current.CurrentPatient != null)
          CommonViewModel.Current.CurrentPatient.Gender = (short)value;

        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Search Patient command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanSearchPatient(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Patient search in the database when the Patient Search
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="OnSearchPatient">The command's parameter (not used in this function).</param>
    private void OnSearchPatient(object OnSearchPatient)
    {
      //AppTrace.Log("Started to search patient.", LogLevel.Debug);

      DataAccessLayer.Patient currentPatient = CommonViewModel.Current.Data.DataAccess.GetPatientByHospitalID(this.hospitalPatientId);

      //AppTrace.Log($"Get hospital patient Id: {hospitalPatientId}.");

      string InputhospitalPatientId = this.hospitalPatientId;
      IsDateSelected = false;

      if (currentPatient != null && currentPatient.FirstName != null)
      {
        isPatientFoundInDatabase = true;
        IsPhysicianSelectionEnabled = true; // false;
        CommonViewModel.Current.CurrentPatient = currentPatient;

        this.FirstName = currentPatient.FirstName;
        this.LastName = currentPatient.LastName;

        this.Gender = Enum.IsDefined(typeof(GenderType), currentPatient.Gender) ? (GenderType)currentPatient.Gender : GenderType.None;

        if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
        {
          this.Weight = Scale.ConvertKgToLb((double)currentPatient.Weight).ToString("00");

          this.Height = Toise.ConvertCmToInch((double)currentPatient.Height).ToString("00");
        }
        else
        {
          this.Weight = currentPatient.Weight?.ToString("00");

          this.Height = currentPatient.Height?.ToString("00");
        }

        Gender = Enum.IsDefined(typeof(GenderType), currentPatient.Gender)
                   ? (GenderType)currentPatient.Gender
                   : GenderType.None; 

        this.Birthdate = currentPatient.DateOfBirth.Year == 1800 
                            ? (DateTime?)null 
                            : currentPatient.DateOfBirth;

        //Verify if the Patient's default Physician EXISTS in the database and is ACTIVE. 
        //if (currentPatient.Physician != null && PhysiciansList.Contains(currentPatient.Physician.Name))
        if (currentPatient.Physician != null)
        {
          List<Physician> t = PhysiciansComList.Where(p => p.Name == currentPatient.Physician.Name).ToList();

          if (t.Count > 0) //Select the Patient's default Physician in the list.
            this.SelectedPhysician = currentPatient.Physician.Name;
        }
        else
        {
          //The default Patient's Physician is not ACTIVE, do not select any Physician in the list
          //and set the Patient's Physician to NULL -> it must be selected in the list.
          this.SelectedPhysician = string.Empty;
          currentPatient.Physician = null;
        }
      }
      else
      {
        IsPhysicianSelectionEnabled = true;
        isPatientFoundInDatabase = false;
        CommonViewModel.Current.CurrentPatient = null;

        //Should clear all fields and set them to thier default values
        ResetPatientInfo();
        if (OnSearchPatient.ToString() == "PatientSearch")
        {
          HospitalPatientId = InputhospitalPatientId;
        }
      }
    }

    /// <summary>
    /// Function/Command that handles Physician/Patient validation and Ablation Procedure creation
    /// when the Next command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="OnNextCommand">The command's parameter (not used in this function).</param>
    private void OnNextCommand(object OnNextCommand)
    {
      CommonViewModel.Current.ResetCanTwoStopWatch();

      Physician physician = null;
      double convertedWeight = 0;
      double convertedHeight = 0;
      
      if (string.IsNullOrEmpty(Weight)) Weight = "0"; 
      if (string.IsNullOrEmpty(Height)) Height = "0";

      try
      {
        physician = CommonViewModel.Current.Data.DataAccess.GetPhysician(this.SelectedPhysician);
      }
      catch (Exception exception)
      {
        exception.ToString();
        Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID28, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup dialogPopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.ErrorMessage, Views.MessagePopup.ButtonType.Ok);
        dialogPopup.ShowDialog();
        return;
      }

      if (CommonViewModel.Current.CurrentPatient == null)//Patient not found, add the Patient in the database
      {
        if (physician == null)
        {
          //This should never happen its just a safe guard

          Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID30, (int)Enumeration.ErrorTypes.GUI);
          Tuple<long, string, string, string> physicianMissingMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID31, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup dialogPopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.ErrorMessage, Views.MessagePopup.ButtonType.Ok, physicianMissingMessage.Item2);
          dialogPopup.ShowDialog();
          return;
        }

        try
        {
          if (CommonViewModel.Current.Data.DataAccess.GetPatientByHospitalID(this.HospitalPatientId) != null)
          {
            Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID34, (int)Enumeration.ErrorTypes.GUI);
            Tuple<long, string, string, string> patientAlreadyExistsMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID35, (int)Enumeration.ErrorTypes.GUI);

            MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, patientAlreadyExistsMessage.Item2);
            messagePopup.ShowDialog();
            return;
          }

          //Convert Weight
          try
          {
            Double.TryParse(this.Weight, out convertedWeight);
          }
          catch (Exception ex)
          {
            ex.ToString();
          }

          //Convert Height
          try
          {
            Double.TryParse(this.Height, out convertedHeight);
          }
          catch (Exception ex)
          {
            ex.ToString();
          }

          CommonViewModel.Current.Data.DataAccess.AddPatient(this.Birthdate ?? _defaultBirthDateInDB,
            this.FirstName.Trim(), this.LastName.Trim(), physician.ID, DateTime.Now,
            this.HospitalPatientId, convertedHeight, convertedWeight, (short)this.Gender);

          NotificationModel.Instance.CurrentPhysician = physician;
        }
        catch (Exception ex)
        {
          ex.ToString();
          Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID36, (int)Enumeration.ErrorTypes.GUI);
          Tuple<long, string, string, string> patientInsertionErrorMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID37, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, patientInsertionErrorMessage.Item2);
          messagePopup.ShowDialog();
        }

        DataAccessLayer.Patient currentPatient = CommonViewModel.Current.Data.DataAccess.GetPatientByHospitalID(this.HospitalPatientId);
        if (currentPatient != null && currentPatient.FirstName != "")
        {
          CommonViewModel.Current.CurrentPatient = currentPatient;

          if (ProcedureLogModel.PreviousLogedPatient?.ID != currentPatient?.ID)
          {
            ProcedureLogModel.LastTreatmnetDate = ProcedureLogModel.ReferenceDate;
            ProcedureLogModel.CanReloadProcudreInformation = false;
          }

          ProcedureLogModel.PreviousLogedPatient = currentPatient;

          if (ProcedureLogModel.LastTreatmnetDate == ProcedureLogModel.ReferenceDate)
            ProcedureLogModel.LastTreatmnetDate = DateTime.Now;
        }
      }

      //The Patient already exists, assign the Physician regardless if it's his default one.
      else
      {
        //Assign the selected physician to the Patient (even if it has not changed).
        if (physician != null)
        {
          CommonViewModel.Current.CurrentPatient.Physician = physician;

          DataAccessLayer.Patient currentPatient = CommonViewModel.Current.CurrentPatient;


          if (ProcedureLogModel.PreviousLogedPatient?.ID != currentPatient?.ID)
          {
            ProcedureLogModel.LastTreatmnetDate = ProcedureLogModel.ReferenceDate;
            ProcedureLogModel.CanReloadProcudreInformation = false;
          }


          //Set The Previous patient
          ProcedureLogModel.PreviousLogedPatient = currentPatient;

          if (ProcedureLogModel.LastTreatmnetDate == ProcedureLogModel.ReferenceDate)
            ProcedureLogModel.LastTreatmnetDate = DateTime.Now;

        }
        else
        {
          Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID38, (int)Enumeration.ErrorTypes.GUI);
          Tuple<long, string, string, string> physicianNotFoundMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID29, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, physicianNotFoundMessage.Item2);
          messagePopup.ShowDialog();
          return;
        }
      }

      try
      {
        if (!ProcedureLogModel.CanReloadProcudreInformation)
        {
          CommonViewModel.Current.OcclusionPressureTareValue = 0;
        }
        //Save the new procedure data
        Procedure newProcedure = null;
        string procedureDescription = string.Empty;


        //Date Legal/Supported file name format in windows and the Patient ID. This is for the log file name
        procedureDescription = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + CommonViewModel.Current?.CurrentPatient?.ID.ToString();


        //Update the Patient (its Physician may have changed)


        DataAccessLayer.Patient patient = CommonViewModel.Current?.Data?.DataAccess?.GetPatientByID(CommonViewModel.Current.CurrentPatient.ID);
        patient.Physician = physician;
        patient.PhysicianID = physician.ID;

        patient.DateOfBirth = this.Birthdate.HasValue ? Birthdate.Value : _defaultBirthDateInDB;
        patient.FirstName = this.FirstName.Trim();
        patient.LastName = this.LastName.Trim();
        patient.Gender = (short)this.Gender;
        patient.TreatmentDateTime = DateTime.Now;


        //Get the weight
        if (convertedWeight == 0)
        {
          try
          {
            double.TryParse(this.Weight, out convertedWeight);
          }
          catch (Exception ex)
          {
            ex.ToString();
          }
        }


        //Get the height
        if (convertedHeight == 0)
        {
          try
          {
            double.TryParse(this.Height, out convertedHeight);
          }
          catch (Exception ex)
          {
            ex.ToString();
          }
        }

        if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
        {
          convertedWeight = Scale.ConvertLbToKg(convertedWeight);

          convertedHeight = Toise.ConvertInchToCm(convertedHeight);
        }

        patient.Weight = convertedWeight;
        patient.Height = convertedHeight;

        DataAccessLayer.Patient patientCopy = new DataAccessLayer.Patient(patient);

        CommonViewModel.Current.CurrentPatient = CommonViewModel.Current?.Data?.DataAccess?.UpdatePatient(patient);

        patient.FirstName = patientCopy.FirstName;
        patient.LastName = patientCopy.LastName;
        patient.Gender = patientCopy.Gender;
        patient.Height = patientCopy.Height;
        patient.Weight = patientCopy.Weight;
        patient.DateOfBirth = patientCopy.DateOfBirth;

        if (!ProcedureLogModel.CanReloadProcudreInformation)
        {
          newProcedure = CommonViewModel.Current?.Data?.DataAccess?.AddProcedure(procedureDescription, DateTime.Now, CommonViewModel.Current.CurrentPatient.Physician.ID, CommonViewModel.Current.CurrentPatient.ID);
        }
        else
        {
          newProcedure = ProcedureLogModel.PreviousProcedure;
        }


        CommonViewModel.Current.LogUserAction(Enumeration.Actions.CreateProcedure);

        if (newProcedure == null)
        {
          throw new Exception();
        }
        else
        {
          CommonViewModel.Current.CurrentProcedure = newProcedure;
          NotificationModel.Instance.Physician = physician;
          NotificationModel.Instance.CurrentPhysician = physician;
          CommonViewModel.Current.CanStartTherapy = true;
          CommonViewModel.Current.DMSDetectionThreshold = Math.Max(physician.preference.DMSDetectionThreshold, Constants.MaxDMSDetectionThreshold);

          ProcedureLogModel.PreviousProcedure = newProcedure;
        }
      }
      catch (Exception ex)
      {
        // TODO
        ex.ToString();

        Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID39, (int)Enumeration.ErrorTypes.GUI);
        Tuple<long, string, string, string> procedureCreationErrorMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID40, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, procedureCreationErrorMessage.Item2);
        messagePopup.ShowDialog();
      }

    }

    /// <summary>
    /// This read-only property retrieve (from the database) and returns the Pysician List
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ObservableCollection<string> PhysiciansList
    {
      get
      {
        physiciansName.Clear();
        List<Physician> Physicians = CommonViewModel.Current?.Data?.DataAccess?.GetAllActivePhysicians();

        foreach (Physician physician in Physicians)
        {
          physiciansName.Add(physician.Name);
        }

        return physiciansName;
      }
    }

    public ObservableCollection<Physician> PhysiciansComList
    {
      get
      {
        Physicians.Clear();
        // ObservableCollection<Physician> PhysicianCollection = new ObservableCollection<Physician>() ;
        List<Physician> Physicianss = CommonViewModel.Current?.Data?.DataAccess?.GetAllActivePhysicians();
        foreach (var p in Physicianss)
        {
          p.FirstName = "Dr. " + p.FirstName + " " + p.LastName;
          Physicians.Add(p);

        }
        return Physicians;
      }
    }

    /// <summary>
    /// This property gets/sets Selected Physician
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string SelectedPhysician
    {
      get
      {
        return selectedPhysician;
      }
      set
      {
        SetProperty(ref this.selectedPhysician, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets Hospital Patient Id value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string HospitalPatientId
    {
      get
      {
        return hospitalPatientId;
      }

      set
      {
        if (CommonViewModel.Current.CurrentPatient != null && 
            value != CommonViewModel.Current.CurrentPatient.HospitalPatientId)
        {
          //A patient has already been search (and found), but a different hospital patient ID has been entered
          ResetPatientInfo();
        }

        SetProperty(ref this.hospitalPatientId, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets Patient's First Name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string FirstName
    {
      get
      {

        return firstName;
      }

      set
      {
        SetProperty(ref this.firstName, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets Patient's Last Name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string LastName
    {
      get
      {

        return lastName;
      }

      set
      {
        SetProperty(ref this.lastName, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets Patient's Height
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string Height
    {
      get
      {
        return height;
      }
      set
      {
        SetProperty(ref this.height, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets Patient's Weight
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string Weight
    {
      get
      {
        return weight;
      }
      set
      {
        SetProperty(ref this.weight, value);
        ValidatePatientInformation();
      }
    }

    /// <summary>
    /// This property gets/sets the Patient Info Valid flag
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsPatientInfoValid
    {
      get
      {
        return isPatientInfoValid;
      }

      set
      {
        SetProperty(ref this.isPatientInfoValid, value);
      }
    }
    /// <summary>
    /// This property gets/sets the Patient Weight Unit value
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
    /// Gets or sets a value indicating whether is date selected or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDateSelected
    {
      get
      {
        return isDateSelected;
      }

      set
      {
        isDateSelected = value;
        RaisePropertyChanged("IsDateSelected");
      }
    }

    public DateTime DefaultBirthDay => _defaultBirthDate;

    public string HospitalName => CommonViewModel.Current.Data.DataAccess.GetHospitalName();

    /// <summary>
    /// Gets or sets a value indicating whether is there patient or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTherePatient
    {
      get
      {
        return isTherePatient;
      }
      set
      {
        isTherePatient = value;
        RaisePropertyChanged("IsTherePatient");
      }
    }

    /// <summary>
    /// Function that validates the Patient information
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ValidatePatientInformation()
    {
      double convertedWeight = 0;
      double convertedHeight = 0;

      try
      {
        double.TryParse(Weight, out convertedWeight);
      }
      catch (Exception ex)
      {
        ex.ToString();
      }

      try
      {
        double.TryParse(Height, out convertedHeight);
      }
      catch (Exception ex)
      {
        ex.ToString();
      }

      if (SelectedPhysician == string.Empty || HospitalPatientId == string.Empty || FirstName == string.Empty || LastName == string.Empty)
        IsPatientInfoValid = false;
      else
      {
        FileAction fileAction = new FileAction();
        //InputRegularExpression
        //Regex objAlphaNumericPattern = new Regex("^[a-zA-Z0-9 _.,-]*$");
        if (fileAction.InputRegularExpression(FirstName) && fileAction.InputRegularExpression(LastName) && fileAction.InputRegularExpression(HospitalPatientId))
          IsPatientInfoValid = true;
        else
          IsPatientInfoValid = false;
      }
    }

    /// <summary>
    /// Function that resets the patient informations values
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetPatientInfo()
    {
      if (CommonViewModel.Current.CurrentPatient != null)
        CommonViewModel.Current.CurrentPatient = null;

      isPatientFoundInDatabase = false;
      IsPhysicianSelectionEnabled = true;
      Birthdate = (DateTime?)null;  //DateTime.MinValue;
      IsDateSelected = false;
      Gender = GenderType.None;

      SelectedPhysician = string.Empty;
      HospitalPatientId = string.Empty;
      FirstName = string.Empty;
      LastName = string.Empty;
      Height = string.Empty;
      Weight = string.Empty;
      RaisePropertyChanged("WeightUnit");

      if (ProcedureLogModel.PreviousLogedPatient != null)
      {
        double hours = (DateTime.Now - ProcedureLogModel.LastTreatmnetDate).TotalHours;

        if (hours >= maximumProcedureDuration)
          IsTherePatient = false;
        else
          IsTherePatient = true;

        if (ProcedureLogModel.IsUserAccessRecord)
        {
          IsTherePatient = false;
          ProcedureLogModel.ResetInformation();
        }
      }
    }

    private void ExecuteBackToHomeCommand()
    {
      this.ResetPatientInfo();
      CommonViewModel.Current.OnViewchanged(new ViewsEventArgs() { ViewName = "Home" });
    }
  }
}