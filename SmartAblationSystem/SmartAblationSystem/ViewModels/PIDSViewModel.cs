using Communication;
using Console;
using DataAccessLayer;
using FileSerializer;
using MicroLibrary;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the PIDs View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PIDSViewModel : BindableBase
    {
        public ICommand WriteToMicroControllerCommand { get; private set; }
        public ICommand TargetInjectionFlowCommand { get; private set; }
        public ICommand TargetInjectionPressureCommand { get; private set; }
        public ICommand TargetBalloonPressureCommand { get; private set; }
        public ICommand WriteCMCUPIDToDbCommand { get; private set; }
        public ICommand WritePMCUPIDToDbCommand { get; private set; }
        public ICommand IncrementSystemStateCommand { get; private set; }
        public ICommand IncreaseTimeCommand { get; private set; }
        public ICommand DecreaseTimeCommand { get; private set; }

        public ICommand IncreaseCentralPIDvalueCommand { get; private set; }

        public ICommand DecreaseCentralPIDvalueCommand { get; private set; }

        public ICommand IncreasePatientPIDvalueCommand { get; private set; }

        public ICommand DecreasePatientPIDvalueCommand { get; private set; }
        public ICommand FastButtonCommand { get; }
        public ICommand SlowButtonCommand { get; }


        // These code will be moved  to the right place
        private const int PatientPIDMessageElementId = 55;

        private const int BallonSizeConfigurationMessageElementId = 57;

        private const int CentralMicroControllerPIDMessageElementId = 16;
        private const int CentralMicroControllerTargetInjectionFlow = 15;
        private const int PatientMicroControllerTargetBalloonPressure = 52;

        private const double PidIncrementValue = 0.5;

        private const int maxWritingTime = 3;

        public event EventHandler<EventArgs> PT2PT3FM1IbPEvent;

        private DispatcherTimer timer = new DispatcherTimer();
        //private MicroTimer loggingTimer = new MicroTimer();
        private MicroTimer ablationTimer = new MicroTimer();

        private string selectedState = string.Empty;

        private bool isCatheterConnected = false;
        private bool isCatheterTubeConnected = false;

        private bool enableOrDisablePIDManualMode = false;
        private bool enableOrDisablePressureFlowMode = false;

        private bool isManualModeEnabled = false;
        private bool isAutomaticModeEnabled = true;

        private bool isPressureModeEnabled = true;
        private bool isFlowModeEnabled = false;

        private bool isPressureFlowActivated = false;

        private bool isPIDModeActivated = false;

        private bool canSaveEngineeringReportFiles = false;

        private int numberOfRetry = 3;
        private int cryoTherapyTime;

        private int refreshCycle = 0;
        private bool cryoTherapyTimeVisible;

        private bool saveInProgress = false;
        private List<EngineeringDataFile> engineeringDataFileList;

        private DataAccess dataAccess;

        public ICommand ConnectCommand { get; private set; }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ICommand PIDModeCommand { get; private set; }

        public ICommand PressureFlowCommand { get; private set; }

        public ICommand FaultResetCommand { get; private set; }

        public ICommand ReadFromMicroControllerCommand { get; private set; }
        public ICommand SaveToUSBCommand { get; private set; }

        public ICommand EnableDASBallonCommand { get; private set; }

        private CommonViewModel localCommonViewModel = CommonViewModel.Current;

        private List<DriveInfo> usbDriveList;
        private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;

        private FileSystemWatcher engineeringFolderWatcher;

        private MessageStateId PreviousSystemState = MessageStateId.CAN_ID_STATE_UNKNOWN;

        private bool catheterIsConnecting = false;

        private bool isTuningPid = false;

        private double lastFlowReadingValue = 0;

        /// <summary>
        /// This constructor initializes the PIDs View Model's properties, commands and data access
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PIDSViewModel()
        {
            localCommonViewModel.PropertyChanged += Current_PropertyChanged;
            this.WriteToMicroControllerCommand = new DelegateCommand<object>(this.OnWriteToMicroController, this.CanWriteToMicroController);
            this.TargetInjectionFlowCommand = new DelegateCommand<object>(this.OnTargetInjectionFlowCommand, this.CanTargetInjectionFlowCommand);
            this.TargetBalloonPressureCommand = new DelegateCommand<object>(this.OnTargetBalloonPressureCommand, this.CanTargetBalloonPressureCommand);
            this.ConnectCommand = new DelegateCommand<object>(this.OnConnectCommand, this.CanConnectCommand);
            this.StartCommand = new DelegateCommand<object>(this.OnStartCommand, this.CanStartCommand);
            this.StopCommand = new DelegateCommand<object>(this.OnStopCommand, this.CanStopCommand);
            this.PIDModeCommand = new DelegateCommand<object>(this.OnPIDModeCommand, this.CanPIDModeCommand);
            this.PressureFlowCommand = new DelegateCommand<object>(this.OnPressureFlowCommand, this.CanPressureFlowCommand);
            this.SaveToUSBCommand = new DelegateCommand<object>(this.OnSaveToUSBCommand, this.CanSaveToUSBCommand);
            this.IncreaseTimeCommand = new DelegateCommand<object>(this.OnIncreaseTimeCommand, this.CanIncreaseTimeCommand);
            this.DecreaseTimeCommand = new DelegateCommand<object>(this.OnDecreaseTimeCommand, this.CanDecreaseTimeCommand);

            this.ReadFromMicroControllerCommand = new DelegateCommand<object>(this.OnReadFromMicroControllerCommand, this.CanReadFromMicroControllerCommand);

            this.FaultResetCommand = new DelegateCommand<object>(this.OnFaultResetCommand, this.CanFaultResetCommand);

            this.WriteCMCUPIDToDbCommand = new DelegateCommand<object>(this.OnWriteCMCUPIDToDbCommand, this.CanWriteCMCUPIDToDbCommand);

            this.WritePMCUPIDToDbCommand = new DelegateCommand<object>(this.OnWritePMCUPIDToDbCommand, this.CanWritePMCUPIDToDbCommand);

            this.IncrementSystemStateCommand = new DelegateCommand<object>(this.OnIncrementSystemStateCommand, this.CanIncrementSystemStateCommand);

            this.IncreaseCentralPIDvalueCommand =  new DelegateCommand<object>(this.OnIncreaseCentralPIDvalueCommand, this.CanIncreaseCentralPIDvalueCommand);
            this.DecreaseCentralPIDvalueCommand = new DelegateCommand<object>(this.OnDecreaseCentralPIDvalueCommand, this.CanDecreaseCentralPIDvalueCommand);

            this.IncreasePatientPIDvalueCommand = new DelegateCommand<object>(this.OnIncreasePatientPIDvalueCommand, this.CanIncreasePatientPIDvalueCommand);
            this.DecreasePatientPIDvalueCommand = new DelegateCommand<object>(this.OnDecreasePatientPIDvalueCommand, this.CanDecreasePatientPIDvalueCommand);

            this.EnableDASBallonCommand = new DelegateCommand<object>(this.OnEnableDASBallonCommand, this.CanEnableDASBallonCommand);

            FastButtonCommand = new DelegateCommand(OnFastButtonCommand, () => true);
            SlowButtonCommand = new DelegateCommand(OnSlowButtonCommand, () => true);

            this.dataAccess = CommonViewModel.Current.Data.DataAccess;  

            EngineeringReportingModel.initialize();

            //Get notified of each USB drive connection event
            usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);
            if (usbDriveConnectionManager != null)
            {
                try
                {
                    USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID41, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID42, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, titleMessage.Item2);
                    messagePopup.ShowDialog();
                }
            }

            CanSaveEngineeringReportFiles = USBDriveConnected && IsEngineeringFilesFolderNotEmpty();

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            //loggingTimer.Interval = 5000000; // we are using 5000ms inteval
            //loggingTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(loggingTimer_tick);
            //loggingTimer.Stop();

            ablationTimer.Interval = 1000000;
            ablationTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(ablationTimer_tick);
            ablationTimer.Stop();

            //Watch for add/edit/delete files in the Engineering Report folder.
            //This is required in order to enable/disable the Save to USB button.
            engineeringFolderWatcher = new FileSystemWatcher();
            //Creates the engineering report folder, if already exists, this call does nothing
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EngineeringData.engineeringReportFolder));

            engineeringFolderWatcher.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EngineeringData.engineeringReportFolder);
            engineeringFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            engineeringFolderWatcher.Filter = "*.json";
            engineeringFolderWatcher.Changed += new FileSystemEventHandler(OnEngineeringFolderChanged);
            engineeringFolderWatcher.Deleted += new FileSystemEventHandler(OnEngineeringFolderChanged);
            engineeringFolderWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// This property gets/sets the Cryotherapy time value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CryoTherapyTime
        {
            get
            {
                return cryoTherapyTime;
            }

            set
            {
                cryoTherapyTime = value;
                RaisePropertyChanged("CryoTherapyTime");
            }
        }

        /// <summary>
        /// This property gets/sets the Cryotherapy time visible value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CryoTherapyTimeVisible
        {
            get
            {
                return cryoTherapyTimeVisible;
            }

            set
            {
                cryoTherapyTimeVisible = value;
                RaisePropertyChanged("CryoTherapyTimeVisible");
            }
        }

        /// <summary>
        /// This property gets/sets the Required Ablation Time value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RequiredAblationTime
        {
            get
            {
                return CommonViewModel.Current.RequiredAblationTime;
            }

            set
            {
                CommonViewModel.Current.RequiredAblationTime = value;
                RaisePropertyChanged("RequiredAblationTime");
            }
        }

        /// <summary>
        /// This property gets/sets the Deflate After Thaw boolean flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DeflateAfterThaw
        {
            get
            {
                return CommonViewModel.Current.DeflateAfterThaw;
            }

            set
            {
                CommonViewModel.Current.DeflateAfterThaw = value;
                RaisePropertyChanged("DeflateAfterThaw");
            }
        }

        /// <summary>
        /// This property gets/sets Save in Progress boolean flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool SaveInProgress
        {
            get
            {
                return saveInProgress;
            }
            set
            {
                saveInProgress = value;
                RaisePropertyChanged("SaveInProgress");
            }
        }

        /// <summary>
        /// This property gets/sets the Engineering Data File List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<EngineeringDataFile> EngineeringDataFileList
        {
            get
            {
                return engineeringDataFileList;
            }
            set
            {
                engineeringDataFileList = value;
                RaisePropertyChanged("EngineeringDataFileList");
            }
        }

        /// <summary>
        /// Function that is invoked when the Engineering Folder path content's has changed
        /// It then verifies if the folder is empty or not and sets "save enginnering report files"
        /// boolean flag.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="source">The source object that trigerred the event.</param>
        /// <param name="e">The file system event argument when the event was trigerred.</param>
        private void OnEngineeringFolderChanged(object source, FileSystemEventArgs e)
        {
            CanSaveEngineeringReportFiles = USBDriveConnected && IsEngineeringFilesFolderNotEmpty();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Save to USB command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanSaveToUSBCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that saves the procedure to USB drive when the Save To USB
        /// command is invoked.  It allows the user to select which file shall be saved
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The command parameter (not used in this function).</param>
        private async void OnSaveToUSBCommand(object sender)
        {
            bool operationCompleted = false;

            if (USBDriveList != null && USBDriveList.Count > 0 &&
                IsEngineeringFilesFolderNotEmpty())
            {
                try
                {
                    //Generate the file list in the Engineering Data Folder
                    EngineeringDataFileList = GetEngineeringDataFilesList();

                    //Display file selector to allow the engineers to select which file they want to save
                    EngineeringDataSelector engineeringDataSelector = new EngineeringDataSelector(this);

                    if ((bool)engineeringDataSelector.ShowDialog())
                    {
                        SaveInProgress = true;
                        operationCompleted = await Task.Run(() => SaveToUSB());
                    }
                }
                catch (IOException ex)
                {
                    //Specified folder is a file
                    //Unknown network name
                    //Invalid path
                    ex.ToString();
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID43, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // TODO
                    ex.ToString();

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID45, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (ArgumentException ex)
                {
                    // TODO
                    ex.ToString();

                    //Path is null
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID46, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (NotSupportedException ex)
                {
                    // TODO
                    ex.ToString();

                    //The path contains : sign that is invalid for the Drive (ex: C:/)

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID47, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (ObjectDisposedException ex)
                {
                    // TODO
                    ex.ToString();

                    //The target file/directory does not exist anymore (ex: file deleted, drive removed)

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID48, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();


                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID49, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                finally
                {
                    SaveInProgress = false;
                }
            }

            if (operationCompleted)
            {
                Tuple<long, string, string, string> genericMessage51 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID51, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> genericMessage52 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID52, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup dialogPopup = new MessagePopup(genericMessage51.Item2, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, messageTitle: genericMessage52.Item2);
                dialogPopup.ShowDialog();
            }
        }

        /// <summary>
        /// Function/Command that handles the Required Ablation Time incrementation when the
        /// Increase Time command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnIncreaseTimeCommand(object arg)
        {
            RequiredAblationTime += 30;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Increase Time command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanIncreaseTimeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Required Ablation Time decrementation when the
        /// Decrease Time command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnDecreaseTimeCommand(object arg)
        {
            RequiredAblationTime -= 30;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Decrease Time command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanDecreaseTimeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// This read-only property generates and return the list of json files in the engineering report folder
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>The list of enginnering data files in the engineering report path.</returns>
        private List<EngineeringDataFile> GetEngineeringDataFilesList()
        {
            List<EngineeringDataFile> engineeringDataFilesList = new List<EngineeringDataFile>();
            EngineeringDataFile engineeringDataFile = null;

            DirectoryInfo engineeringReportDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EngineeringData.engineeringReportFolder));

            foreach (var file in engineeringReportDirectory.GetFiles("*.json"))
            {
                engineeringDataFile = new EngineeringDataFile();
                engineeringDataFile.Filename = file.FullName;
                engineeringDataFile.Selected = false;
                engineeringDataFilesList.Add(engineeringDataFile);
            }

            return engineeringDataFilesList;
        }

        /// <summary>
        /// This function handles the Engineering files saving on a USB drive.  It allows file selection, conversion to JSON/CSV
        /// and saving on a USB Drive
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Boolean if the operation was successfull.</returns>
        private bool SaveToUSB()
        {
            EngineeringData engineeringData = null;
            string saveToUSBPath = "";
            bool operationCompleted = false;

            if (USBDriveList != null && USBDriveList.Count > 0 &&
                IsEngineeringFilesFolderNotEmpty())
            {
                try
                {
                    //for all selected files in the folder, generate a CSV file and write it to the USB Drive
                    CSVManager csvManager = new CSVManager();
                    JsonManager jsonManager = new JsonManager();
                    foreach (var file in EngineeringDataFileList)
                    {
                        if (file.Selected)
                        {
                            engineeringData = jsonManager.DeserializeEngineeringData(file.Filename);

                            //Convert into CSV then save to USB
                            if (engineeringData != null)
                            {
                                saveToUSBPath = USBDriveList[0].Name + EngineeringData.engineeringReportFolder + Path.GetFileNameWithoutExtension(file.Filename);
                                csvManager.GenerateAndWriteToFile(engineeringData, saveToUSBPath);
                            }
                        }
                    }
                    operationCompleted = true;
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                    throw;
                }
            }

            return operationCompleted;
        }

        /// <summary>
        /// This function invokes the PT2/PT3/FM1/Ibp event when PT2/PT3/FM1/IBP change has been trigerred from
        /// an external source
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The parameter's name that has changed.</param>
        protected virtual void OnPT2PT3FM1IbPChanged(object sender, EcgEventArgs e)
        {
            PT2PT3FM1IbPEvent?.Invoke(sender, e);
        }

        /// <summary>
        /// This function is invoked by the timer at each tick and triggers PT2/PT3/FM1/IBP event change
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            RefreshCycle++;

            OnPT2PT3FM1IbPChanged(CommonViewModel.Current, null);
        }

        /// <summary>
        /// This function is invoked by the loggingTimer.  It stops the timer and adds current data to the database
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event (not used in this function).</param>
        /// <param name="e">The event arguments.</param>
        //private void loggingTimer_tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //if (SystemState != MessageStateId.CAN_ID_STATE_IDLE &&
        //        //   SystemState != MessageStateId.CAN_ID_STATE_READY &&
        //        //   SystemState != MessageStateId.CAN_ID_STATE_INFLATION &&
        //        //    SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
        //        //{
        //        //    CorrectTheFlow(FM1Reading);
        //        //}

        //        //else
        //        //{
        //        //    loggingTimer.Stop();
        //        //}


        //        //this.dataAccess.AddCMCUPIDLoga(FM1Reading, PT2Reading, TargetInjectionFlow, TargetInjectionPressure, TargetInjectionFlow - FM1Reading
        //        //    , TargetInjectionPressure - PT2Reading, PatientPGain, PatientIGain, PatientDGain, PatientPIDOffset);

        //        //this.dataAccess.AddPMCUPIDLoga(PT3Reading, CP1Reading, CP2Reading, PIDDutyCycle, PatientPIDDutyCycle, TargetBalloonPressure,
        //        //    TargetBalloonPressure - CP1Reading, PGain, IGain, DGain, PIDOffset);
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO
        //        ex.ToString();
        //    }
        //}

        /// <summary>
        /// This function is invoked by the ablationTimer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event (not used in this function).</param>
        /// <param name="e">The event arguments.</param>
        private void ablationTimer_tick(object sender, EventArgs e)
        {
            try
            {
                CryoTherapyTime++;

                if (this.CryoTherapyTime == RequiredAblationTime && 
                    CommonViewModel.Current.SystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
                {
                    //Stop the ablation procedure, but keep the ablation timer running.
                    StopAblationProcedure();
                }
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// This property gets/sets PGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PGain
        {
            get
            {
                return localCommonViewModel.PGain;
            }
            set
            {
                try
                {
                    localCommonViewModel.PGain = value;
                    RaisePropertyChanged("PGain");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets IGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double IGain
        {
            get
            {
                return localCommonViewModel.IGain;
            }

            set
            {
                try
                {
                    localCommonViewModel.IGain = value;
                    RaisePropertyChanged("IGain");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets DGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DGain
        {
            get
            {
                return localCommonViewModel.DGain;
            }

            set
            {
                try
                {
                    localCommonViewModel.DGain = value;
                    RaisePropertyChanged("DGain");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets PID Offset value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PIDOffset
        {
            get
            {
                return localCommonViewModel.PIDOffset;
            }
            set
            {
                try
                {
                    localCommonViewModel.PIDOffset = value;
                    RaisePropertyChanged("PIDOffset");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets Patient PGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPGain
        {
            get
            {
                return localCommonViewModel.PatientPGain;
            }
            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PatientPGain = value;
                        RaisePropertyChanged("PatientPGain");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets Patient IGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientIGain
        {
            get
            {
                return localCommonViewModel.PatientIGain;
            }

            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PatientIGain = value;
                        RaisePropertyChanged("PatientIGain");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Patient DGain value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientDGain
        {
            get
            {
                return localCommonViewModel.PatientDGain;
            }

            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PatientDGain = value;
                        RaisePropertyChanged("PatientDGain");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Patient PID Offset value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPIDOffset
        {
            get
            {
                return localCommonViewModel.PatientPIDOffset;
            }

            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PatientPIDOffset = value;
                        RaisePropertyChanged("PatientPIDOffset");
                    }

                }
                catch { }
            }
        }


        #region DAS Balloon
        /// <summary>
        /// This property gets/sets ramp up time by step value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampUpTimeByStep
        {
            get
            {
                return localCommonViewModel.RampUpTimeByStep;
            }

            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.RampUpTimeByStep = value;
                        RaisePropertyChanged("RampUpTimeByStep");
                    }
                }
                catch { }

            }
        }
        /// <summary>
        /// This property gets/sets the pressure ramp up value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampUpValue
        {
            get
            {
                return localCommonViewModel.PressureRampUpValue;
            }
            set
            {

                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PressureRampUpValue = value;
                        RaisePropertyChanged("PressureRampUpValue");
                    }
                }
                catch { }

            }
        }
        /// <summary>
        /// This property gets/sets ramp down time by step value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampDownTimeByStep
        {
            get
            {
                return localCommonViewModel.RampDownTimeByStep;
            }
            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.RampDownTimeByStep = value;
                        RaisePropertyChanged("RampDownTimeByStep");
                    }
                }
                catch { }

            }
        }

        /// <summary>
        /// This property gets/sets the pressure ramp down value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampDownValue
        {
            get
            {
                return localCommonViewModel.PressureRampDownValue;
            }
            set
            {
                try
                {
                    if (value >= 0)
                    {
                        localCommonViewModel.PressureRampDownValue = value;
                        RaisePropertyChanged("PressureRampDownValue");
                    }
                }
                catch { }

            }
        }
        /// <summary>
        /// This property gets/sets pressure set point value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureSetPoint
        {
            get
            {
                return CommonViewModel.Current.ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentPressureSetpoint;
            }

            set
            {
                RaisePropertyChanged("PressureSetPoint");
            }
        }
        /// <summary>
        /// This property gets/sets DAS balloon enabled value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DASBalloonEnabled
        {
            get
            {
                return CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled;
            }
            set
            {
                RaisePropertyChanged("DASBalloonEnabled");
                RaisePropertyChanged("PressureSetPoint");
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether is system using DAS balloon or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemUsingDASBalloon
        {
            get
            {

#if DEBUG
                return true;
#else
                return (CommonViewModel.Current.IsSystemUsingDASBalloon && CommonViewModel.Current.IsCatheterCableConnected);
#endif
            }
        }

        #endregion


        /// <summary>
        /// This read-only property returns the system's States List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<string> StatesList
        {
            get
            {
                List<string> convertedState = new List<string>();
                Communication.CanBusMessageDefinition.MessageStateId[] states = (Communication.CanBusMessageDefinition.MessageStateId[])Enum.GetValues(typeof(Communication.CanBusMessageDefinition.MessageStateId));

                foreach (Communication.CanBusMessageDefinition.MessageStateId element in states)
                {
                    convertedState.Add(element.ToString().Replace("CAN_ID_STATE_", string.Empty));
                }

                return convertedState;
            }
        }

        /// <summary>
        /// This property gets/sets the Target Injection Flow value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionFlow
        {
            get
            {
                localCommonViewModel.Console.InjectionFlowValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = localCommonViewModel.TargetInjectionFlow;
                return localCommonViewModel.TargetInjectionFlow;
            }

            set
            {
                try
                {
                    localCommonViewModel.TargetInjectionFlow = value;
                    localCommonViewModel.Console.InjectionFlowValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = value;
                    RaisePropertyChanged("TargetInjectionFlow");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Target Balloon Pressure value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetBalloonPressure
        {
            get
            {
                localCommonViewModel.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = localCommonViewModel.TargetBalloonPressure;
                return localCommonViewModel.TargetBalloonPressure;
            }

            set
            {
                try
                {
                    localCommonViewModel.TargetBalloonPressure = value;
                    localCommonViewModel.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = value;
                    RaisePropertyChanged("TargetBalloonPressure");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Target Injection Pressure value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionPressure
        {
            get
            {
                localCommonViewModel.Console.InjectionPressureValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = localCommonViewModel.TargetInjectionPressure;
                return localCommonViewModel.TargetInjectionPressure;
            }

            set
            {
                try
                {
                    localCommonViewModel.TargetInjectionPressure = value;
                    localCommonViewModel.Console.InjectionPressureValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = value;
                    RaisePropertyChanged("TargetInjectionPressure");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the system's state value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId SystemState
        {
            get
            {
                CryoTherapyTimeVisible = false;

                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
                {
                    IsCatheterConnectedAndInIReadyState = true;
                }
                else
                {
                    IsCatheterConnectedAndInIReadyState = false;
                }

                //Only display the ablation timer when in Ablation
                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION ||
                    CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    CryoTherapyTimeVisible = true;
                }
                else
                {
                    CryoTherapyTimeVisible = false;
                }

                //Start the Ablation Timer when the system state falls in Ablation
                if (PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION &&
                    CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    //Start the Ablation Timer
                    CryoTherapyTime = 0;
                    ablationTimer.Start();
                }
                else if (
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION ||
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE ||
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
                {
                    //When not in ablation, stop the ablation timer and reset the cryotherapy time.
                    if (ablationTimer.Enabled)
                    {
                        ablationTimer.Stop();
                    }

                    CryoTherapyTime = 0;                    
                }
                else if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_TRANSITION && PreviousSystemState != MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    //loggingTimer.Start();
                }

                PreviousSystemState = localCommonViewModel.SystemState;
                return localCommonViewModel.SystemState;
            }

            set
            {
                localCommonViewModel.SystemState = value;
                RaisePropertyChanged("SystemState");
            }
        }

        /// <summary>
        /// This function manages the Procedure Stop (console and flags update)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void StopAblationProcedure()
        {
            CommonViewModel.Current.IsAblationProcedureStarted = false;
            CommonViewModel.Current.IsAblationProcedureEnded = true;
            CommonViewModel.Current.Console.Stop();
        }

        /// <summary>
        /// This function activates the catheter if the conditions are verified
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ActivateCatheterIfConditionsApply()
        {
            RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
            RaisePropertyChanged("IsCatheterCableConnected");
            RaisePropertyChanged("IsCatheterTubeConnected");
            RaisePropertyChanged("IsSystemUsingDASBalloon");

            if (localCommonViewModel.IsCMCUReady && localCommonViewModel.IsPMCUReady)
                CatheterIsConnecting = false;

            if (localCommonViewModel.IsCatheterCableConnected && localCommonViewModel.IsCatheterTubeConnected)
            {
                IsCatheterConnected = true;
                // Here we activate the ouputs
                //localCommonViewModel.Console.IinjectionEnable();
                //localCommonViewModel.Console.VacuumEnable();
            }
        }

        /// <summary>
        /// This property gets/sets Catheter connected flag.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnected
        {
            get
            {
                return localCommonViewModel.IsCatheterConnected;
            }

            set
            {
                SetProperty(ref this.isCatheterConnected, value);
            }
        }

        /// <summary>
        /// This property gets/sets Catheter cable connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterCableConnected
        {
            get
            {
                bool _IsCatheterCableConnected = CommonViewModel.Current.IsCatheterCableConnected;

                if (_IsCatheterCableConnected && (!CommonViewModel.Current.IsCMCUReady || !CommonViewModel.Current.IsPMCUReady))
                    CatheterIsConnecting = true;
                else
                    CatheterIsConnecting = false;

                return _IsCatheterCableConnected;
            }

            set
            {
                // SetProperty(ref this.isCatheterCableConnected, value);
                // SetProperty(ref this.catheterCableConnectedStatusImage,  "");
                RaisePropertyChanged("IsCatheterConnected");  //Not sure if needed any more
                RaisePropertyChanged("IsCatheterCableConnected");
            }
        }

        /// <summary>
        /// This property gets/sets Catheter tube connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterTubeConnected
        {
            get
            {
                return localCommonViewModel.IsCatheterTubeConnected;
            }
            set
            {
                SetProperty(ref this.isCatheterTubeConnected, value);
                RaisePropertyChanged("IsCatheterTubeConnected");
            }
        }

        /// <summary>
        /// This property gets/sets Catheter electrically connected and in idle state flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterElectricallyConnectedAndInIdleState
        {
            get
            {
                bool isCatheterElectricallyConnectedAndInIdleState = (IsCatheterCableConnected &&
                        localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE && CommonViewModel.Current.IsCatheterValid
                        && CommonViewModel.Current.IsCMCUReady && CommonViewModel.Current.IsPMCUReady);

                if (!IsCatheterCableConnected || isCatheterElectricallyConnectedAndInIdleState)
                {
                    //CatheterIsConnecting = false;
                }

                else
                {

                }

                return isCatheterElectricallyConnectedAndInIdleState;
            }
            set
            {
                RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is catheter connecting or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CatheterIsConnecting
        {
            get
            {
                return catheterIsConnecting;
            }

            set
            {
                if (value != catheterIsConnecting)
                {
                    catheterIsConnecting = value;
                    RaisePropertyChanged("CatheterIsConnecting");
                }
            }
        }

        /// <summary>
        /// This property gets/sets Catheter connected and in ready state flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnectedAndInIReadyState
        {
            get
            {
                return (localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY && CommonViewModel.Current.IsCatheterValid);
            }
            set
            {
                RaisePropertyChanged("IsCatheterConnectedAndInIReadyState");
            }
        }

        /// <summary>
        /// This property gets/sets the system's selected state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string SelectedState
        {
            get
            {
                return selectedState;
            }

            set
            {
                switch (StatesList.IndexOf(value))
                {
                    case 0:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;

                        break;

                    case 1:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;

                        break;

                    case 2:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;

                        break;

                    case 3:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;

                        break;

                    case 4:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;

                        break;

                    case 5:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;

                        break;

                    case 6:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;

                        break;

                    case 7:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION;

                        break;
                }
                SetProperty(ref this.selectedState, value);
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Write to Microcontroller command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanWriteToMicroController(object arg)
        {
            // To do
            return true;
        }

        /// <summary>
        /// Function/Command that handles writing to the patient/central microcontroller when the
        /// Write to MicroController command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter.</param>
        private void OnWriteToMicroController(object arg)
        {
            int state = 0;

            state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

            if (state == 0 || state == 7)
                return;

            if (arg.ToString() == "PatientMicroController")
            {
                for (int i = 0; i < maxWritingTime; i++)
                {
                    localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = PatientPGain;
                    localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = PatientIGain;
                    localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = PatientDGain;
                    localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = PatientPIDOffset;

                    localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].RampUpTimeByStep = RampUpTimeByStep;
                    localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureRampUpValue = PressureRampUpValue;
                    localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].RampDownTimeByStep = RampDownTimeByStep;
                    localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureRampDownValue = PressureRampDownValue;


                    localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, PatientPIDMessageElementId);
                    System.Threading.Thread.Sleep(20);
                    localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, BallonSizeConfigurationMessageElementId);
                }
            }
            //To do
            else if (arg.ToString() == "CentralMicroController")
            {
                localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = PGain;
                localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = IGain;
                localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = DGain;
                localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = PIDOffset;

                localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, CentralMicroControllerPIDMessageElementId);
            }
        }

        /// <summary>
        /// This property gets/sets the USB Drive list connected to the system
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<DriveInfo> USBDriveList
        {
            get
            {
                return usbDriveList;
            }
            set
            {
                usbDriveList = value;
                RaisePropertyChanged("USBDriveConnected");
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Save Engineering Report Files command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Boolean CanSaveEngineeringReportFiles
        {
            get
            {
                return canSaveEngineeringReportFiles;
            }
            set
            {
                canSaveEngineeringReportFiles = value;
                RaisePropertyChanged("CanSaveEngineeringReportFiles");
            }
        }

        /// <summary>
        /// This read-only function returns if the engineering files folder is empty
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Boolean if the enginnering files folder is empty.</returns>
        public Boolean IsEngineeringFilesFolderNotEmpty()
        {
            int fileCount = 0;

            try
            {
                fileCount = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EngineeringData.engineeringReportFolder),
                                               "*.json",
                                               SearchOption.TopDirectoryOnly).Length;
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();

                //no matter what wrong happened, keep the filecount to zero.
                fileCount = 0;
            }

            return fileCount > 0;
        }

        /// <summary>
        /// This read-only property returns if a USB Drive is connected to the system
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Boolean USBDriveConnected
        {
            get
            {
                return USBDriveList != null && USBDriveList.Count != 0;
            }
        }

        /// <summary>
        /// This function handles the sender's (USB Drive Connection to the system) PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The object that triggerred the event (not used in this function).</param>
        /// <param name="e">The event parameters (not used in this function).</param>
        private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
                CanSaveEngineeringReportFiles = USBDriveConnected && IsEngineeringFilesFolderNotEmpty();
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();

                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID41, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID42, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, titleMessage.Item2);
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Target Injection Flow command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanTargetInjectionFlowCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the writing value to Can Bus the Target Injection Flow
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnTargetInjectionFlowCommand(object arg)
        {
            int state = 0;

            state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

            if (state == 0 || state == 7)
                return;

            localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = TargetInjectionFlow;
            localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = TargetInjectionPressure;
            localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, CentralMicroControllerTargetInjectionFlow);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Target Balloon Pressure command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanTargetBalloonPressureCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles writing to the Can Bus when the Target Balloon Pressure
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnTargetBalloonPressureCommand(object arg)
        {
            int state = 0;

            state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

            if (state == 0 || state == 7)
                return;

            localCommonViewModel.Console.Balloon.TargetBalloonPressure = TargetBalloonPressure;
            localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, PatientMicroControllerTargetBalloonPressure);
        }

        /// <summary>
        /// Function/Command that handles connection/disconnection when the Connect
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnConnectCommand(object arg)
        {
            if (localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                localCommonViewModel.Console.Disconnect();
                //IsCatheterConnected = false;
            }
            else if (IsCatheterCableConnected)
            {
                localCommonViewModel.Console.Connect();
                //IsCatheterConnected = true;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Connect command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanConnectCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the console start when the Start command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnStartCommand(object arg)
        {
            localCommonViewModel.Console.Start();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanStartCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the console stop when the Stop command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnStopCommand(object arg)
        {
            localCommonViewModel.Console.Stop();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Stop command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanStopCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles PIDs mode when the PIDs Mode command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnPIDModeCommand(object arg)
        {
            if (IsPIDModeActivated)
            {
                localCommonViewModel.Console.EnableOrDisablePIDManualMode = true;
                EnableOrDisablePIDManualMode = true;

                IsManualModeEnabled = true;
                IsAutomaticModeEnabled = false;
            }
            else
            {
                localCommonViewModel.Console.EnableOrDisablePIDManualMode = false;
                EnableOrDisablePIDManualMode = false;

                IsManualModeEnabled = false;
                IsAutomaticModeEnabled = true;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the PID Mode command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanPIDModeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Pressure Flow mode when the Pressure Flow command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnPressureFlowCommand(object arg)
        {
            //Flow
            if (IsPressureFlowActivated)
            {
                localCommonViewModel.Console.EnableOrDisablePressureFlowMode = true;
                EnableOrDisablePressureFlowMode = true;

                IsFlowModeEnabled = true;
                IsPressureModeEnabled = false;
            }

            //Pressure
            else
            {
                localCommonViewModel.Console.EnableOrDisablePressureFlowMode = false;
                EnableOrDisablePressureFlowMode = false;

                IsFlowModeEnabled = false;
                IsPressureModeEnabled = true;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Pressure Flow command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanPressureFlowCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the reading from Central or Patient MicroController when the Read from MicroController
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (central or patient micro controller).</param>
        private void OnReadFromMicroControllerCommand(object arg)
        {

            #region Reading Code

            if ((IsCatheterCableConnected && localCommonViewModel.IsPMCUReady && localCommonViewModel.IsCMCUReady) || !IsCatheterCableConnected)
            {

                int state = 0;

                state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

                if (state == 0 || state == 7)
                    return;

                if (arg.ToString() == "CentralMicroController")
                {
                    for (int i = 0; i < numberOfRetry; i++)
                    {
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, CentralMicroControllerTargetInjectionFlow);
                        System.Threading.Thread.Sleep(20);
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, CentralMicroControllerPIDMessageElementId);
                        System.Threading.Thread.Sleep(20);
                    }
                }
                //To do
                else if (arg.ToString() == "PatientMicroController")
                {
                    for (int i = 0; i < numberOfRetry; i++)
                    {
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, PatientMicroControllerTargetBalloonPressure);
                        System.Threading.Thread.Sleep(20);
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, PatientPIDMessageElementId);
                        System.Threading.Thread.Sleep(20);
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, BallonSizeConfigurationMessageElementId);
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }

            #endregion Reading Code
        }

        /// <summary>
        /// Function that returns if the system can Read from MicroController command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReadFromMicroControllerCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the fault reset when the Fault Reset
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnFaultResetCommand(object arg)
        {
            localCommonViewModel.Console.FailResetEnable();
            System.Threading.Thread.Sleep(10);
            localCommonViewModel.Console.FailResetDisable();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Fault Reset command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanFaultResetCommand(object arg)
        {
            return true;
        }

        #region Write to PIDs DB

        /// <summary>
        /// Function/Command that handles the writing to CMCU PID when the Write to CMCU PID to DB
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnWriteCMCUPIDToDbCommand(object arg)
        {
            int state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

            if (state == 0 || state == 7)
                return;

            foreach (var catheterType in dataAccess.GetCatheterTypes())
            {
              dataAccess.UpdateCMCUPIDValues(state, PGain, IGain, DGain, PIDOffset, catheterType.ID);
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Write CMCU PID to DB command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanWriteCMCUPIDToDbCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the writing to PMCU PID when the Write to PMCU PID to DB
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnWritePMCUPIDToDbCommand(object arg)
        {
          int state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

          if (state == 0 || state == 7)
            return;

          foreach (var catheterType in dataAccess.GetCatheterTypes())
          {
            dataAccess.UpdatePMCUPIDValues(state, PatientPGain, PatientIGain, PatientDGain, PatientPIDOffset, catheterType.ID);
          }

          dataAccess.UpdateBalloonParametersValues(state, RampUpTimeByStep, PressureRampUpValue, RampDownTimeByStep, PressureRampDownValue);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Write PMCU PID to DB command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanWritePMCUPIDToDbCommand(object arg)
        {
            return true;
        }

        #endregion Write to PIDs DB

        /// <summary>
        /// This property gets/sets the PT2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2Reading
        {
            get
            {
                return localCommonViewModel.PT2Reading;
            }

            set
            {
                localCommonViewModel.PT2Reading = value;
                RaisePropertyChanged("PT2Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the PT3 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3Reading
        {
            get
            {
                return localCommonViewModel.PT3Reading;
            }

            set
            {
                localCommonViewModel.PT3Reading = value;
                RaisePropertyChanged("PT3Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the FM1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1Reading
        {
            get
            {

                if (RefreshCycle >= 10)
                {
                    RefreshCycle = 0;
                    LastFlowReadingValue = localCommonViewModel.FM1Reading;
                    return LastFlowReadingValue;
                }
                else
                {
                    return LastFlowReadingValue;
                }
            }

            set
            {
                localCommonViewModel.FM1Reading = value;
                RaisePropertyChanged("FM1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the PID Duty Cycle value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PIDDutyCycle
        {
            get
            {
                return localCommonViewModel.PIDDutyCycle;
            }

            set
            {
                localCommonViewModel.PIDDutyCycle = value;
                RaisePropertyChanged("PIDDutyCycle");
            }
        }

        /// <summary>
        /// This property gets/sets the Patient PID Duty Cycle value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPIDDutyCycle
        {
            get
            {
                return localCommonViewModel.PatientPIDDutyCycle;
            }

            set
            {
                localCommonViewModel.PatientPIDDutyCycle = value;
                RaisePropertyChanged("PatientPIDDutyCycle");
            }
        }

        /// <summary>
        /// This property gets/sets the CP1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP1Reading
        {
            get
            {
                return localCommonViewModel.CP1Reading;
            }

            set
            {
                localCommonViewModel.CP1Reading = value;
                RaisePropertyChanged("CP1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the CP2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP2Reading
        {
            get
            {
                return localCommonViewModel.CP2Reading;
            }

            set
            {
                localCommonViewModel.CP2Reading = value;
                RaisePropertyChanged("CP2Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the Enable or Disable PID Manual Mode value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnableOrDisablePIDManualMode
        {
            get
            {
                return enableOrDisablePIDManualMode;
            }

            set
            {
                SetProperty(ref this.enableOrDisablePIDManualMode, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Enable or Disable Pressure Flow mode value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnableOrDisablePressureFlowMode
        {
            get
            {
                return enableOrDisablePressureFlowMode;
            }

            set
            {
                SetProperty(ref this.enableOrDisablePressureFlowMode, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Manuel Mode Enabled value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsManualModeEnabled
        {
            get
            {
                return isManualModeEnabled;
            }

            set
            {
                SetProperty(ref this.isManualModeEnabled, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Automatic Mode Enabled value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAutomaticModeEnabled
        {
            get
            {
                return isAutomaticModeEnabled;
            }

            set
            {
                SetProperty(ref this.isAutomaticModeEnabled, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Pressure Mode Enabled value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPressureModeEnabled
        {
            get
            {
                return isPressureModeEnabled;
            }

            set
            {
                SetProperty(ref this.isPressureModeEnabled, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Flow Mode Enabled value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFlowModeEnabled
        {
            get
            {
                return isFlowModeEnabled;
            }

            set
            {
                SetProperty(ref this.isFlowModeEnabled, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Pressure Flow Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPressureFlowActivated
        {
            get
            {
                return isPressureFlowActivated;
            }

            set
            {
                SetProperty(ref this.isPressureFlowActivated, value);
            }
        }

        /// <summary>
        /// This property gets/sets the PID Mode Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPIDModeActivated
        {
            get
            {
                return isPIDModeActivated;
            }

            set
            {
                SetProperty(ref this.isPIDModeActivated, value);
            }
        }

        /// <summary>
        /// This property gets/sets the TC1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC1Reading
        {
            get
            {
                return localCommonViewModel.TC1Reading;
            }

            set
            {
                localCommonViewModel.TC1Reading = value;
                RaisePropertyChanged("TC1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets the Logging Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsLoggingActivated
        {
            get
            {
                return EngineeringReportingModel.IsLoggingActivated;
            }

            set
            {
                EngineeringReportingModel.IsLoggingActivated = value;
                RaisePropertyChanged("IsLoggingActivated");
            }
        }

        /// <summary>
        /// Gets or sets a value for engineering catheter signature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int EngineeringCatheterSignature
        {
            get
            {
                return CommonViewModel.Current.EngineeringCatheterSignature;
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether is tuning Pid or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsTuningPid
        {
            get
            {
                return isTuningPid;
            }
            set
            {
                isTuningPid = value;
                if (value)
                {
                    RaisePropertyChanged("RequiredAblationTime");
                }
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Increment System State command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanIncrementSystemStateCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System's state incrementation when the Increment System State
        /// command is invoked.  This function shall only be called when building in Simulator Mode
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnIncrementSystemStateCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
        }

        /// <summary>
        /// Returns if the system can invoke the increase central PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanIncreaseCentralPIDvalueCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles increase central PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnIncreaseCentralPIDvalueCommand(object arg)
        {
            string parameter = arg.ToString();

            if (parameter == "PGain")
            {
                PGain += PidIncrementValue;
            }

            else if (parameter == "IGain")
            {
                IGain += PidIncrementValue;
            }

            else if (parameter == "DGain")
            {
                DGain += PidIncrementValue;
            }

            else if (parameter == "OffsetGain")
            {
                PIDOffset += PidIncrementValue;
            }

            OnWriteToMicroController("CentralMicroController");
        }

        /// <summary>
        /// Returns if the system can invoke the decrease central PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanDecreaseCentralPIDvalueCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles decrease central PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnDecreaseCentralPIDvalueCommand(object arg)
        {
            string parameter = arg.ToString();

            if (parameter == "PGain")
            {
                PGain -= PidIncrementValue;
            }

            else if (parameter == "IGain")
            {
                IGain -= PidIncrementValue;
            }

            else if (parameter == "DGain")
            {
                DGain -= PidIncrementValue;
            }

            else if (parameter == "OffsetGain")
            {
                PIDOffset -= PidIncrementValue;
            }

            OnWriteToMicroController("CentralMicroController");
        }
        /// <summary>
        /// Returns if the system can invoke the increase patient PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanIncreasePatientPIDvalueCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles decrease patient PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnIncreasePatientPIDvalueCommand(object arg)
        {
            string parameter = arg.ToString();

            if (parameter == "PGain")
            {
                PatientPGain += PidIncrementValue;
            }

            else if(parameter == "IGain")
            {
                PatientIGain += PidIncrementValue;
            }

            else if (parameter == "DGain")
            {
                PatientDGain += PidIncrementValue;
            }

            else if (parameter == "OffsetGain")
            {
                PatientPIDOffset += PidIncrementValue;
            }

            OnWriteToMicroController("PatientMicroController");
        }
        
        /// <summary>
        /// Returns if the system can invoke the decrease patient PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanDecreasePatientPIDvalueCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Returns if the system can invoke the decrease patient PID value command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnDecreasePatientPIDvalueCommand(object arg)
        {
            string parameter = arg.ToString();

            if (parameter == "PGain")
            {
                PatientPGain -= PidIncrementValue;
            }

            else if (parameter == "IGain")
            {
                PatientIGain -= PidIncrementValue;
            }

            else if (parameter == "DGain")
            {
                PatientDGain -= PidIncrementValue;
            }

            else if (parameter == "OffsetGain")
            {
                PatientPIDOffset -= PidIncrementValue;
            }

            OnWriteToMicroController("PatientMicroController");
        }


        /// <summary>
        /// Function that returns if the system can invoke the enable DAS ballon command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanEnableDASBallonCommand(object arg)
        {
            return true;
            //return CommonViewModel.Current.ChangeBalloonTypeFSM.CatheterType == Enumeration.CatheterType.ID_28_mm;
        }
        /// <summary>
        /// Function/Command that handles the DAS ballon enabled
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="param">The command's parameter (not used in this function).</param>
        private void OnEnableDASBallonCommand(object obj)
        {


            CommonViewModel localCommonViewModel = CommonViewModel.Current;

            localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled = !localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled;
            DASBalloonEnabled = localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled;

        }
        /// <summary>
        /// Function/Command that handles stop ablation timer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StopAblationTimer()
        {
            ablationTimer.Stop();
        }

        /// <summary>
        /// Gets or sets the Lock the foot switch boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool LockTheFootSwitch
        {
            get
            {
                return CommonViewModel.Current.LockTheFootSwitch;
            }

            set
            {
                CommonViewModel.Current.LockTheFootSwitch = value;
                RaisePropertyChanged("LockTheFootSwitch");
            }
        }
        /// <summary>
        /// Gets or sets the refresh cycle value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RefreshCycle { get => refreshCycle; set => refreshCycle = value; }
        /// <summary>
        /// Gets or sets the last flow reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LastFlowReadingValue { get => lastFlowReadingValue; set => lastFlowReadingValue = value; }


        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The property changed arguments.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CommonViewModel commonviewmodel = sender as CommonViewModel;

            if (IsTuningPid)
            {
                switch (e.PropertyName)
                {
                    case "TC1Reading":
                        RaisePropertyChanged("TC1Reading");
                        break;

                    case "CP2Reading":
                        RaisePropertyChanged("CP2Reading");
                        break;

                    case "CP1Reading":
                        RaisePropertyChanged("CP1Reading");
                        break;

                    case "TIPReading":
                        RaisePropertyChanged("TIPReading");
                        break;

                    case "PS1Reading":
                        RaisePropertyChanged("PS1Reading");
                        break;

                    case "PS2Reading":
                        RaisePropertyChanged("PS2Reading");
                        break;

                    case "FM1Reading":
                        RaisePropertyChanged("FM1Reading");
                        break;

                    case "PIDDutyCycle":
                        RaisePropertyChanged("PIDDutyCycle");
                        break;

                    case "PatientPIDDutyCycle":
                        RaisePropertyChanged("PatientPIDDutyCycle");
                        break;

                    case "LC1Reading":
                        RaisePropertyChanged("LC1Reading");
                        break;

                    case "PT2Reading":
                        RaisePropertyChanged("PT2Reading");
                        break;

                    case "PT3Reading":
                        RaisePropertyChanged("PT3Reading");
                        break;

                    case "IBPReading":
                        RaisePropertyChanged("IBPReading");
                        break;

                    case "DeflateAfterThaw":
                        RaisePropertyChanged("DeflateAfterThaw");
                        break;

                    case "RequiredAblationTime":
                        RaisePropertyChanged("RequiredAblationTime");
                        break;

                    #region Patient register

                    case "PatientMicroControllerFirmwareVersion":
                        RaisePropertyChanged("PatientMicroControllerFirmwareVersion");
                        break;

                    case "PMCUSystemStatusErrorCode":
                        RaisePropertyChanged("PMCUSystemStatusErrorCode");
                        break;

                    case "CatheterFirmwareVersion":
                        RaisePropertyChanged("CatheterFirmwareVersion");
                        break;

                    case "CatheterID":
                        RaisePropertyChanged("CatheterID");
                        RaisePropertyChanged("IsSystemUsingDASBalloon");
                        break;

                    case "CatheterSerialNumber":
                        RaisePropertyChanged("CatheterSerialNumber");
                        break;

                    case "CatheterExpirationDate":
                        RaisePropertyChanged("CatheterExpirationDate");
                        break;

                    case "CatheterLastUseDate":
                        RaisePropertyChanged("CatheterLastUseDate");
                        break;

                    case "NumberOfInjections":
                        RaisePropertyChanged("NumberOfInjections");
                        break;

                    case "TargetBalloonPressure":
                        RaisePropertyChanged("TargetBalloonPressure");
                        break;

                    case "ThresholdForCP2High":
                        RaisePropertyChanged("ThresholdForCP2High");
                        break;

                    case "ThresholdForCP1High":
                        RaisePropertyChanged("ThresholdForCP1High");
                        break;

                    case "ThresholdForCTC1High":
                        RaisePropertyChanged("ThresholdForCTC1High");
                        break;

                    case "ThresholdForCTC2High":
                        RaisePropertyChanged("ThresholdForCTC2High");
                        break;

                    case "PatientPGain":
                        RaisePropertyChanged("PatientPGain");
                        break;

                    case "PatientIGain":
                        RaisePropertyChanged("PatientIGain");
                        break;

                    case "PatientDGain":
                        RaisePropertyChanged("PatientDGain");
                        break;

                    case "PatientPIDOffset":
                        RaisePropertyChanged("PatientPIDOffset");
                        break;
                    #region DAS Timing

                    case "RampUpTimeByStep":
                        RaisePropertyChanged("RampUpTimeByStep");
                        break;

                    case "PressureRampUpValue":
                        RaisePropertyChanged("PressureRampUpValue");
                        break;

                    case "RampDownTimeByStep":
                        RaisePropertyChanged("RampDownTimeByStep");
                        break;

                    case "PressureRampDownValue":
                        RaisePropertyChanged("PressureRampDownValue");
                        break;

                    #endregion

                    #endregion Patient register

                    #region Central Micro Controller: Register Values

                    case "CentralMicroControllerFirmwareVersion":
                        RaisePropertyChanged("CentralMicroControllerFirmwareVersion");
                        break;

                    case "CPLDErrorRegister":
                        RaisePropertyChanged("CPLDErrorRegister");
                        break;

                    case "CPLDValveRegister":
                        RaisePropertyChanged("CPLDValveRegister");
                        break;

                    case "CPLDSystemRegister":
                        RaisePropertyChanged("CPLDSystemRegister");
                        break;

                    case "SystemState":
                        RaisePropertyChanged("SystemState");
                        break;

                    case "AblationTime":
                        RaisePropertyChanged("AblationTime");
                        break;

                    case "ContinuousThawing":
                        RaisePropertyChanged("ContinuousThawing");
                        break;

                    case "TargetInjectionFlow":
                        RaisePropertyChanged("TargetInjectionFlow");
                        break;

                    case "TargetInjectionPressure":
                        RaisePropertyChanged("TargetInjectionPressure");
                        break;

                    case "PGain":
                        RaisePropertyChanged("PGain");
                        break;

                    case "IGain":
                        RaisePropertyChanged("IGain");
                        break;

                    case "DGain":
                        RaisePropertyChanged("DGain");
                        break;

                    case "PIDOffset":
                        RaisePropertyChanged("PIDOffset");
                        break;

                    case "ThresholdForPT1High":
                        RaisePropertyChanged("ThresholdForPT1High");
                        break;

                    case "ThresholdForPT1Fail":
                        RaisePropertyChanged("ThresholdForPT1Fail");
                        break;

                    case "ThresholdForPT1Low":
                        RaisePropertyChanged("ThresholdForPT1Low");
                        break;

                    case "PT1LowRange":
                        RaisePropertyChanged("PT1LowRange");
                        break;

                    case "PT1HighRange":
                        RaisePropertyChanged("PT1HighRange");
                        break;

                    case "ThresholdPT2High":
                        RaisePropertyChanged("ThresholdPT2High");
                        break;

                    case "PT2LowRange":
                        RaisePropertyChanged("PT2LowRange");
                        break;

                    case "PT2HighRange":
                        RaisePropertyChanged("PT2HighRange");
                        break;

                    case "ThresholdPT3High":
                        RaisePropertyChanged("ThresholdPT3High");
                        break;

                    case "PT3LowRange":
                        RaisePropertyChanged("PT3LowRange");
                        break;

                    case "PT3HighRange":
                        RaisePropertyChanged("PT3HighRange");
                        break;

                    case "ThresholdPT4high":
                        RaisePropertyChanged("ThresholdPT4high");
                        break;

                    case "PT4LowRange":
                        RaisePropertyChanged("PT4LowRange");
                        break;

                    case "PT4HighRange":
                        RaisePropertyChanged("PT4HighRange");
                        break;

                    case "ThresholdTS1High":
                        RaisePropertyChanged("ThresholdTS1High");
                        break;

                    case "TS1LowRange":
                        RaisePropertyChanged("TS1LowRange");
                        break;

                    case "TS1HighRange":
                        RaisePropertyChanged("TS1HighRange");
                        break;

                    case "ThresholdFM1Low":
                        RaisePropertyChanged("ThresholdFM1Low");
                        break;

                    case "ThresholdFM1High":
                        RaisePropertyChanged("ThresholdFM1High");
                        break;

                    case "FM1LowRange":
                        RaisePropertyChanged("FM1LowRange");
                        break;

                    case "FM1HighRange":
                        RaisePropertyChanged("FM1HighRange");
                        break;

                    case "ThresholdPS1High":
                        RaisePropertyChanged("ThresholdPS1High");
                        break;

                    case "PS1LowRange":
                        RaisePropertyChanged("PS1LowRange");
                        break;

                    case "PS1HighRange":
                        RaisePropertyChanged("PS1HighRange");
                        break;

                    case "ThresholdPS2High":
                        RaisePropertyChanged("ThresholdPS2High");
                        break;

                    case "PS2LowRange":
                        RaisePropertyChanged("PS2LowRange");
                        break;

                    case "PS2HighRange":
                        RaisePropertyChanged("PS2HighRange");
                        break;

                    case "ThresholdLC1Warning":
                        RaisePropertyChanged("ThresholdLC1Warning");
                        break;

                    case "ThresholdLC1Fail":
                        RaisePropertyChanged("ThresholdLC1Fail");
                        break;

                    case "LC1LowRange":
                        RaisePropertyChanged("LC1LowRange");
                        break;

                    case "LC1HighRange":
                        RaisePropertyChanged("LC1HighRange");
                        break;

                    case "CMCUSystemStatusError":
                        RaisePropertyChanged("CMCUSystemStatusError");
                        break;

                    #endregion Central Micro Controller: Register Values

                    #region Errors

                    case "IsPMCUExceptionType1":
                        RaisePropertyChanged("IsPMCUExceptionType1");
                        break;

                    case "IsPMCUExceptionType2":
                        RaisePropertyChanged("IsPMCUExceptionType2");
                        break;

                    case "IsPMCUExceptionType3":
                        RaisePropertyChanged("IsPMCUExceptionType3");
                        break;

                    case "IsPMCUExceptionType4":
                        RaisePropertyChanged("IsPMCUExceptionType4");
                        break;

                    case "IsPMCUExceptionType5":
                        RaisePropertyChanged("IsPMCUExceptionType5");
                        break;

                    case "IsPMCUCPLDWatchDogTimerError":
                        RaisePropertyChanged("IsPMCUCPLDWatchDogTimerError");
                        break;

                    case "IsInnerBalloonPressureTooHigh":
                        RaisePropertyChanged("IsInnerBalloonPressureTooHigh");
                        break;

                    case "IsInnerBalloonPressureTooLow":
                        RaisePropertyChanged("IsInnerBalloonPressureTooLow");
                        break;

                    case "IsInnerBalloonPressureReadingOutOfRange":
                        RaisePropertyChanged("IsInnerBalloonPressureReadingOutOfRange");
                        break;

                    case "IsOuterBalloonPressureTooHigh":
                        RaisePropertyChanged("IsOuterBalloonPressureTooHigh");
                        break;

                    case "IsOuterBalloonPressureTooLow":
                        RaisePropertyChanged("IsOuterBalloonPressureTooLow");
                        break;

                    case "IsOuterBalloonPressureReadingOutOrRange":
                        RaisePropertyChanged("IsOuterBalloonPressureReadingOutOrRange");
                        break;

                    case "IsBalloonTipPressureTooHigh":
                        RaisePropertyChanged("IsBalloonTipPressureTooHigh");
                        break;

                    case "IsBalloonTipPressureTooLow":
                        RaisePropertyChanged("IsBalloonTipPressureTooLow");
                        break;

                    case "IsBalloonTipPressurePeadingOutOfRange":
                        RaisePropertyChanged("IsBalloonTipPressurePeadingOutOfRange");
                        break;

                    case "IsThawingTemperatureTooHigh":
                        RaisePropertyChanged("IsThawingTemperatureTooHigh");
                        break;

                    case "IsThawingTemperatureTooLow":
                        RaisePropertyChanged("IsThawingTemperatureTooLow");
                        break;

                    case "IsCatheterCableConnected":
                        RaisePropertyChanged("IsCatheterCableConnected");
                        RaisePropertyChanged("IsSystemUsingDASBalloon");
                        break;

                    case "IsCatheterTubeConnected":
                        ActivateCatheterIfConditionsApply();
                        break;

                    case "IsCMCUExceptionType1":
                        RaisePropertyChanged("IsCMCUExceptionType1");
                        break;

                    case "IsCMCUExceptionType2":
                        RaisePropertyChanged("IsCMCUExceptionType2");
                        break;

                    case "IsCMCUExceptionType3":
                        RaisePropertyChanged("IsCMCUExceptionType3");
                        break;

                    case "IsCMCUExceptionType4":
                        RaisePropertyChanged("IsCMCUExceptionType4");
                        break;

                    case "IsCMCUExceptionType5":
                        RaisePropertyChanged("IsCMCUExceptionType5");
                        break;

                    case "IsCMCUCPLDWatchDogTimerError":
                        RaisePropertyChanged("IsCMCUCPLDWatchDogTimerError");
                        break;

                    case "IsCMCUTwoMultiplexReadingDoesNotMatch":
                        RaisePropertyChanged("IsCMCUTwoMultiplexReadingDoesNotMatch");
                        break;

                    case "IsCMCUFlowTooHigh":
                        RaisePropertyChanged("IsCMCUFlowTooHigh");
                        break;

                    case "IsCMCUFlowTooLow":
                        RaisePropertyChanged("IsCMCUFlowTooLow");
                        break;

                    case "IsCMCUFlowReadingOutOfRange":
                        RaisePropertyChanged("IsCMCUFlowReadingOutOfRange");
                        break;

                    case "IsCMCULoadCellWeightWarning":
                        RaisePropertyChanged("IsCMCULoadCellWeightWarning");
                        break;

                    case "IsCMCULoadCellWeightFail":
                        RaisePropertyChanged("IsCMCULoadCellWeightFail");
                        break;

                    case "IsCMCULoadCellReadingOutOfRange":
                        RaisePropertyChanged("IsCMCULoadCellReadingOutOfRange");
                        break;

                    case "IsCMCUPressureInTankIsHighFanToBeOn":
                        RaisePropertyChanged("IsCMCUPressureInTankIsHighFanToBeOn");
                        break;

                    case "IsCMCUPressurePT1InTankIsLow":
                        RaisePropertyChanged("IsCMCUPressurePT1InTankIsLow");
                        break;

                    case "IsCMCUPressurePT1InTankIsTooHigh":
                        RaisePropertyChanged("IsCMCUPressurePT1InTankIsTooHigh");
                        break;

                    case "IsCMCUPressurePT1InTankReadingOutOfRange":
                        RaisePropertyChanged("IsCMCUPressurePT1InTankReadingOutOfRange");
                        break;

                    case "IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh":
                        RaisePropertyChanged("IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh");
                        break;

                    case "IsCMCUPT2ReadingOutOfRange":
                        RaisePropertyChanged("IsCMCUPT2ReadingOutOfRange");
                        break;

                    case "IsCMCUReturnPressurePT3TooHigh":
                        RaisePropertyChanged("IsCMCUReturnPressurePT3TooHigh");
                        break;

                    case "IsCMCUReturnPressurePT3OutOfRange":
                        RaisePropertyChanged("IsCMCUReturnPressurePT3OutOfRange");
                        break;

                    case "IsCMCUVacuumPressurePT4TooHigh":
                        RaisePropertyChanged("IsCMCUVacuumPressurePT4TooHigh");
                        break;

                    case "IsCMCUVacuumPressurePT4OutOfRange":
                        RaisePropertyChanged("IsCMCUVacuumPressurePT4OutOfRange");
                        break;

                    case "IsCMCUSubCoolerTemperatureIsHigh":
                        RaisePropertyChanged("IsCMCUSubCoolerTemperatureIsHigh");
                        break;

                    case "IsCMCUSubCoolerTemperatureOutOfRange":
                        RaisePropertyChanged("IsCMCUSubCoolerTemperatureOutOfRange");
                        break;

                    case "IsCMCUInjectionVentPressureIsHigh":
                        RaisePropertyChanged("IsCMCUInjectionVentPressureIsHigh");
                        break;

                    case "IsCMCUInjectionVertPressureOutOfRange":
                        RaisePropertyChanged("IsCMCUInjectionVertPressureOutOfRange");
                        break;

                    case "IsCMCUScavengingPressureIsHigh":
                        RaisePropertyChanged("IsCMCUScavengingPressureIsHigh");
                        break;

                    case "IsCMCUScavengingPressureOutOfRange":
                        RaisePropertyChanged("IsCMCUScavengingPressureOutOfRange");
                        break;

                    #endregion Errors

                    #region catheter ready

                    case "IsCMCUReady":
                    case "IsPMCUReady":
                        ActivateCatheterIfConditionsApply();
                        break;

                    case "IsCatheterValid":
                        ActivateCatheterIfConditionsApply();
                        break;

                        #endregion
                }
            }
        }

        public Enumeration.InflationSpeedMode InflationSpeedMode => CommonViewModel.Current.Console.EnableFastInflationMode
                                                                      ? Enumeration.InflationSpeedMode.Fast
                                                                      : Enumeration.InflationSpeedMode.Slow;

        public void RefreshInflationSpeedMode()
        {
          RaisePropertyChanged(nameof(InflationSpeedMode));
        }

        private void OnFastButtonCommand()
        {
          CommonViewModel.Current.Console.EnableFastInflationMode = true;
          RefreshInflationSpeedMode();
        }

        private void OnSlowButtonCommand()
        {
          CommonViewModel.Current.Console.EnableFastInflationMode = false;
          RefreshInflationSpeedMode();
        }
    }
}