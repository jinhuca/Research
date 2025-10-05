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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UniversalLoginManager;
using static Communication.CanBusMessageDefinition;
using Communication;
using RijndaelCryptography;
using BootLoader;
using RS232Communication;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq;
using Shared;
using static LogSystem.LogService;
using System.Reactive.Subjects;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Common View Model.  It is used by View Models to manage properties and events that
    /// are common among each other.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CommonViewModel : BindableBase
    {
        private volatile bool _canManageRTRCatheterMessage = true;
        
        private static CommonViewModel current;

        private static UserControl homeView;

        public event EventHandler<ViewsEventArgs> ViewChanged;

        /// <summary>
        /// Text entry type enumeration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum TextEntryType
        {
            TreatmentNotes = 0,
            Diagnosis = 1,
            Outcome = 2,
            ReportTreatmentNotes = 3
        }

        #region changing Tank
        private bool accessedChangeTankFromCryotherapy = false;
        private bool isUserAllowedToChangeTank = false;
        //  private bool isTreatmentSuccess = false;

        #endregion

        private int ablationTime;
        private int requiredAblationTime = 240;
        private int cryoTherapyTime = 0;
        private NotificationModel notificationModel = NotificationModel.Instance;

        private InflateDeflateBalloonModel inflateDeflateBalloonModel;
        private AncestralPasswordEncrypter ancestralPasswordEncrypter = AncestralPasswordEncrypter.Instance;

        private int catheterExpirationDay;
        private int catheterExpirationMonth;
        private int catheterExpirationYear;

        private int catheterLastUseHour = 0;
        private int catheterLastUseDay = 0;
        private int catheterLastUseMonth = 0;
        private int catheterLastUseYear = 0;

        private int sentCatheterLastUseHour = 0;
        private int sentCatheterLastUseDay = 0;
        private int sentCatheterLastUseMonth = 0;
        private int sentCatheterLastUseYear = 0;

        private string screenName = "Home";
        private string maintenanceScreenName = "Electrical Signal";
        private bool isMaintenanceModeScreenSelected = false;

        //I am doing That to avoid the case of the micro controller do not send the last expiration date
        private DateTime catheterExpirationDate = new DateTime(1900, 1, 1);

        // we do not to some one try to crash the code and  use catheter...
        private DateTime inavalidCatheterExpirationDate = new DateTime(1800, 1, 1);

        private int catheterID;

        // I am doing That to avoid the case of the micro controller do not send the last used date
        private DateTime catheterLastUseDate = new DateTime(1900, 1, 1);

        private bool isCatheterLastUseDateUpdated = false;
        private bool isCatheterExpirationDateUpdated = false;

        private int catheterFirmwareVersion = 0;
        private int catheterSerialNumber;
        private int catheterLot;

        private const int CentralMicroControllerFirmwareVersionId = 8;
        private const int PatientMicroControllerFirmwareVersionId = 48;
        private const int CatheterFirmwareVersionId = 56;
        private const int RepeaterFirmwareAndICBFirmwareId = 11;
        private const int RemoteFirmwareId = 24;

        // Central Micro Controller: Register Values
        private int centralMicroControllerFirmwareVersion = 0;
        private int centralMicroControllerBootLoaderFirmwareVersion = 0;

        private int cpldFirmwareVersion = 0;
        private int cpldBootLoaderFirmwareVersion = -1;

        // Connection Box Register values
        private int repeaterFirmware = 0;
        private int repeaterBootLoaderFirmware = 0;

        private int iCBFirmware = 0;
        private int iCBBootLoaderFirmwareVersion = 0;

        private int remoteControlFirmware = 0;
        private int remoteControlBootLoaderFirmwareVersion = 0;

        private int databaseVersion;

        private string guiVersion = string.Empty;

        private Machine console;
        private Data data;

        private CatheterValidator catheterValidator;
        private ObservableCollection<Models.ActionLogRecord> actionLog;

        private OuterBalloonPressureThreshold outerBalloonPressureThreshold;

        private double continuousThawing;
        private double cP1Reading;

        private double cP2Reading;

        private double tIPReading;

        private double patientPIDDutyCycle;

        private int cPLDErrorRegister;

        private int cPLDSystemRegister;

        private int cPLDValveRegister;

        private double dGain;

        private Int64 pMCUSystemStatusErrorCode;

        private double fM1HighRange;

        private double fM1LowRange;

        // Flow meter
        private double fM1Reading;

        private double iGain;

        private double lC1HighRange;

        private double lC1LowRange;

        // Load cell
        private double lC1Reading;

        private int numberOfInjections;

        private double patientDGain;

        private double patientIGain;

        //Blood Detector
        private int bloodDetecorImValue;
        private int bloodDetectionType = -1;


        // Patient Micro Controller: Register Values
        private int patientMicroControllerFirmwareVersion = 0;
        private int patientMicroControllerBootLoaderFirmwareVersion = 0;

        private double patientPGain;

        private double patientPIDOffset;

        private double pGain;

        private double pIDOffset;

        private double rampUpTimeByStep;

        private double pressureRampUpValue;

        private double rampDownTimeByStep;

        private double pressureRampDownValue;

        private double pS1HighRange;

        private double pS1LowRange;

        // Pressure switch
        private double pS1Reading;

        private double pS2HighRange;
        private double pS2LowRange;
        private double pS2Reading;

        private double pT1HighRange;

        private double pT1LowRange;

        // Pressure Transducer
        private double pT1Reading;

        private double pT2HighRange;
        private double pT2LowRange;
        private double pT2Reading;

        private double pT3HighRange;
        private double pT3LowRange;
        private double pT3Reading;

        private double pT4HighRange;
        private double pT4LowRange;
        private double pT4Reading;
        private double pT5Reading;
        private double pIDDutyCycle;



        private MessageStateId systemState;
        private MessageStateId simulatedSystemState = MessageStateId.CAN_ID_STATE_IDLE;

        private MessageStateId previousSystemState = MessageStateId.CAN_ID_STATE_UNKNOWN;

        private Int64 cMCUSystemStatusError;

        private double targetBalloonPressure;

        private double targetInjectionFlow;

        private double baseTargetInjectionFlow;

        private double targetInjectionPressure;

        // thermocouple  and temperature sensor
        private double tC1Reading;

        private double pMCUCJReading;

        private double tC2Reading;
        private double thresholdFM1High;
        private double thresholdFM1Low;
        private double thresholdForCP1High;
        private double thresholdForInnerBallonPressureLow; // the naming the database is CP1PressureHighRangeLimit(we are using that in byte 4 and 5 for id 53 ) 
        private double thresholdForOuterBallonPressure; //The naming wase thresholdForCP2High we have to armonise naming according to the last document of yong
        private double thresholdForCTC1High;
        private double thresholdForCTC2High;
        private short lowerBloodThreshold;
        private short upperBloodThreshold;
        private double thresholdForPT1Fail;
        private double thresholdForPT1High;
        private double thresholdForPT1Low;
        private double thresholdLC1Fail;
        private double thresholdLC1Warning;
        private double thresholdPS1High;
        private double thresholdPS2High;
        private double thresholdPT2High;
        private double thresholdPT3High;
        private double thresholdPT4high;
        private double thresholdTS1High;
        private double tS1HighRange;
        private double tS1LowRange;
        private double tS1Reading;
        private double cMCUCJReading;
        private double tN2OReading;
        private double thawingTemperatureSetPoint = 20;

        //Ecg

        private double ecgChannel1And2Reading;
        private double rawEcgChannel1And2Reading;

        private double ecgChannel3And4Reading;
        private double ecgChannel5And6Reading = 1000;

        private double maxEcgChannel1And2Reading = 0;
        private double maxEcgChannel3And4Reading = 0;
        private double maxEcgChannel5And6Reading = 0;

        private double ecgChannel7And8Reading;

        private double[] _highResolutionDmsReading;

        //ETS
        private double etsSesnor1 = 1000;
        private double etsSesnor2 = 1000;
        private double etsSesnor3 = 1000;
        private double etsSesnor4 = 1000;
        private double etsSesnor5 = 1000;
        private double etsSesnor6 = 1000;
        private double etsSesnor7 = 1000;
        private double etsSesnor8 = 1000;
        private double etsSesnor9 = 1000;
        private double etsSesnor10 = 1000;
        private double etsSesnor11 = 1000;
        private double etsSesnor12 = 1000;
        private double etsSesnor13 = 1000;
        private double tIP = 1000;
        private double minimumTemperature = 1000;

        List<int> listOfSesnorsState = new List<int>();

        private double eTSMinimumTemperature = 1000;


        private DMSLogic DMSLogic;

        private int requiredAblationTimeAccordingToState = 240;

        private double ecgChannel9And10Reading;
        private double channelTipReading;
        private double channelAccelerometerReading;
        private double dASLowFlow = 7800;

        private bool canstartDiaphragmMovementMonitoring = false;
        private bool canChangeTank = false;
        private Procedure currentProcedure;
        private Ablation currentAblation;
        private DataAccessLayer.Patient currentPatient;

        private bool isAblationProcedureEnded = true;
        private bool isAblationProcedureStarted = false;
        private bool canStartTherapy = false;
        private bool canEndProcedure = false;

        private bool isSystemInDataError = false;

        // lock the ecg data
        private readonly object _myVar_Lock = new object();

        public readonly object _myRegister_Lock = new object();

        public readonly object _bloodPressure_Lock = new object();

        private readonly object _errorIdMessageAndSolutionList_Lock = new object();

        private List<double> ecgChannel1And2Readings = new List<double>();
        private List<double> ecgChannel3And4Readings = new List<double>();
        private List<double> ecgChannel5And6Readings = new List<double>();
        private List<double> ecgChannel7And8Readings = new List<double>();

        private List<double> ecgChannel9And10Readings = new List<double>();
        private List<double> channelTipReadings = new List<double>();
        private List<double> channelAccelerometerReadings = new List<double>();

        // here we want to send specific data for ids 51,52,53,54,55
        private List<int> CatheterInfoIds = new List<int> { 51, 52, 53, 54, 55 };

        // Used for flow and pressure
        private const int CentralMicroControllerTargetInjectionFlow = 15;

        private List<List<AblationDataDetails>> allAblationDataList = new List<List<AblationDataDetails>>();

        #region PMCU status and error

        private bool isPMCUExceptionType1 = false;
        private bool isPMCUExceptionType2 = false;
        private bool isPMCUExceptionType3 = false;
        private bool ispMCUExceptionType4 = false;
        private bool isPMCUExceptionType5 = false;
        private bool isPMCUCPLDWatchDogTimerError = false;
        private bool isInnerBalloonPressureTooHigh = false;
        private bool isInnerBalloonPressureTooLow = false;
        private bool isInnerBalloonPressureReadingOutOfRange = false;
        private bool isOuterBalloonPressureTooHigh = false;
        private bool isOuterBalloonPressureTooLow = false;
        private bool isOuterBalloonPressureReadingOutOrRange = false;
        private bool isBalloonTipPressureTooHigh = false;
        private bool isBalloonTipPressureTooLow = false;
        private bool isBalloonTipPressurePeadingOutOfRange = false;
        private bool isThawingTemperatureTooHigh = false;
        private bool isThawingTemperatureTooLow = false;
        private bool isBalloonTemperatureTooHigh = false;
        private bool isBloodDetectedInCatheter = false;
        private bool isBloodDetectorwireOpen = false;
        private bool isPMCUReady = false;
        private bool isPMCUSelfTestFail = false;

        // Catheter bools
        private bool isCatheterCableConnected = false;

        private bool isCatheterTubeConnected = false;

        private ICanBusCommunication catheterCommunicationData;
        private uint catheterEventId;

        #region Vein Isolation
        private bool isVeinIsolated = false;

        #endregion

        #endregion PMCU status and error

        #region CMCU status Error

        private bool isCMCUExceptionType1 = false;
        private bool isCMCUExceptionType2 = false;
        private bool isCMCUExceptionType3 = false;
        private bool isCMCUExceptionType4 = false;
        private bool isCMCUExceptionType5 = false;
        private bool isCMCUCPLDWatchDogTimerError = false;
        private bool isCMCUTwoMultiplexReadingDoesNotMatch = false;
        private bool isCMCUFlowTooHigh = false;
        private bool isCMCUFlowTooLow = false;
        private bool isCMCUFlowReadingOutOfRange = false;
        private bool isCMCULoadCellWeightWarning = false;
        private bool isCMCULoadCellWeightFail = false;
        private bool isCMCULoadCellReadingOutOfRange = false;
        private bool isCMCUPressureInTankIsHighFanToBeOn = false;
        private bool isCMCUPressurePT1InTankIsLow = false;
        private bool isCMCUPressurePT1InTankIsTooHigh = false;
        private bool isCMCUPressurePT1InTankReadingOutOfRange = false;
        private bool isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh = false;
        private bool isCMCUPT2ReadingOutOfRange = false;
        private bool isCMCUReturnPressurePT3TooHigh = false;
        private bool isCMCUReturnPressurePT3OutOfRange = false;
        private bool isCMCUVacuumPressurePT4TooHigh = false;
        private bool isCMCUVacuumPressurePT4OutOfRange = false;
        private bool isCMCUSubCoolerTemperatureIsHigh = false;
        private bool isCMCUSubCoolerTemperatureOutOfRange = false;
        private bool isCMCUInjectionVentPressureIsHigh = false;
        private bool isCMCUInjectionVertPressureOutOfRange = false;
        private bool isCMCUScavengingPressureIsHigh = false;
        private bool isFootSwitchLocked = false;
        private bool isCMCUSelfTestFail = false;
        private bool isCMCUReady = false;

        private Helpers.Enumeration.TankWeight gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

        private bool isSytemInWarning = false;

        #endregion CMCU status Error

        // Violating MVVM. but ING requirements want that
        private bool isTheUserInProcedureScreen = false;

        private bool isCatheterValid = false;

        private MicroTimer catheterConnectedTimer = new MicroTimer();

        private readonly System.Timers.Timer canOneTimer = new System.Timers.Timer();
        private readonly System.Timers.Timer canTwoTimer = new System.Timers.Timer();

        private readonly System.Timers.Timer remoteControlTimer = new System.Timers.Timer();
        private readonly System.Timers.Timer usageTimer = new System.Timers.Timer();

        private readonly System.Timers.Timer skinToSkinAblationTimer = new System.Timers.Timer();

#if Simulator
        private readonly System.Timers.Timer ackTimer = new System.Timers.Timer();
#endif

        private List<int> patientMicroControllerRegisterIDSDynamicTable = new List<int>();
        private List<int> centralMicroControllerRegisterIDSDynamicTable = new List<int>();
        private Dictionary<int, bool> patientMicroControllerackRegistersTable = new Dictionary<int, bool>();
        private Dictionary<int, bool> centralMicroControllerAckRegistersTable = new Dictionary<int, bool>();
        private bool isReadingFromMicroControllerForRegisterValidation = false;
        private List<Tuple<double, double>> listOfValues = new List<Tuple<double, double>>();

        private bool isWindowLoaded = false;

        private AblationSummary ablationSummary;
        private List<Physician> physicianList;

        #region Acks

        #region CMUC

        private bool ackForRegisters15 = false;
        private bool aAckForRegisters16 = false;
        private bool ackForRegisters17 = false;
        private bool ackForRegisters18 = false;
        private bool acKForRegisters19 = false;
        private bool acKForRegisters20 = false;
        private bool acKForRegisters21 = false;
        private bool acKForRegisters22 = false;
        private bool acKForRegisters23 = false;
        private bool acKForRegisters24 = false;
        private bool acKForRegisters25 = false;
        private bool acKForRegisters26 = false;
        private bool acKForRegisters27 = false;
        private bool acKForRegisters28 = false;
        private bool acKForRegisters29 = false;
        private bool acKForRegisters30 = false;
        private bool acKForRegisters31 = false;
        private bool acKForRegisters32 = false;
        private bool acKForRegisters33 = false;
        private bool acKForRegisters34 = false;

        #endregion CMUC

        #region PMUC

        private bool acKForRegisters52 = false;
        private bool acKForRegisters53 = false;
        private bool acKForRegisters54 = false;
        private bool acKForRegisters55 = false;

        #endregion PMUC

        #endregion Acks

        #region Errors

        private string pmcuPreviousError = string.Empty;
        private string cmcuPreviousError = string.Empty;
        private string cmcuPreviousWarning = string.Empty;

        private string genericError = string.Empty;

        List<Tuple<long, string, string, string>> errorIdMessageAndSolutionList = new List<Tuple<long, string, string, string>>();


        Tuple<long, string, string, string> cmcuTupleError = new Tuple<long, string, string, string>(0, string.Empty, string.Empty, string.Empty);

        Tuple<long, string, string, string> pmcuTupleError = new Tuple<long, string, string, string>(0, string.Empty, string.Empty, string.Empty);

        #endregion Errors

        private bool isCatheterInError = false;
        private bool isCanTwoInError = false;
        private bool isCanOneInError = false;
        private bool isCanOneWasInError = false;
        private bool isCanOneReseted = false;
        private bool isWarningVisible = false;

        private bool isCanTwoReseted = false;
        private bool isCanTwoWasInError = false;
        private bool isICBConnected = false;


        #region CAN1 and CAN2 communicationn lost

        private Stopwatch canOneStopWatchCommunicationLost = new Stopwatch();

        private Stopwatch canTwoStopWatchCommunicationLost = new Stopwatch();

        private Stopwatch catheterStopWatchDisconnection = new Stopwatch();

        private Stopwatch resetCMCUErrorStopWatchDisconnection = new Stopwatch();

        private Stopwatch resetPMCUErrorStopWatchDisconnection = new Stopwatch();

        private Stopwatch remoteControlStopWatchDisconnection = new Stopwatch();

        private Stopwatch iCBStopWatchDisconnection = new Stopwatch();

        private long catheterMaximumTimeDisconnection = 0;

        private long errorResetingMaximumTime = 3000;

        private long iCBMaximumTimeOut = 2000;

        private long ecg1An2RefreshTime = 1000;
        private long ecg3An4RefreshTime = 1000;

        private bool isDiaphragmMovementDetected = true;
        private double dmsDetectionThreshold = 0.003;
        private bool ignoreMinimumDiaphragmMovementValue = false;
        private byte diaphragmMovementCompter = 0;
        private const byte diaphragmMovementCompterOneSecondeValue = 50;
        private const byte diaphragmMovementCompterOneSecondeValuePeakToPeak = 25;
        private List<double> diaphragmMovementTable = new List<double>();
        private List<double> diaphragmMovementTablePeakToPeak = new List<double>();
        private double maximumAveragePacingLevel = 0;
        private const double PacingLevelMaxvalue = 1;

        private long canOneMaximumTimeOut = 3000;
        private long canTwoMaximumTimeOut = 3000;
        private long remoteContolMaximumTimeOut = 3000;
        private uint remoteControlTimingToFactor = 25;
        private uint remoteControlTimingToFactorIncrement = 0;

        #endregion CAN1 and CAN2 communicationn lost

        private Stopwatch Ecgs1And2StopWatch = new Stopwatch();
        private Stopwatch Ecgs3And4StopWatch = new Stopwatch();

        private int tipPressureDiaphragmMovementEsophagusTemperatureTime = 0;

        private bool areSensorsInPlayBackMode = false;
        private bool isPlayBackModeDeactivted = false;
        private bool isUsingAutoPlayback = false;


        private ChangeBalloonTypeFSM changeBalloonTypeFSM;
        private bool isSystemUsingDASBalloon = false;

        private RemoteControlFSM remoteControlFSM;

        private DiaphragmConditioning diaphragmConditioning;

        private LoginManager loginManager;
        private WarningMessagesManager.WarningMessagesManager warningMessagesManager;
        private string hospitalName = "Unknown";

        private bool isSystemRested = false;
        private bool isVacuumDisconnected = false;
        private bool isCPLDLatching = false;

        private bool stopListeningCanOneCommunication = false;
        private bool stopListeningCanTwoCommunication = false;

        private long minutesOfUse = 0;

        private uint requiredVolume = 0;

        private DataAccessLayer.Tank currentTank;

        private bool catheterIsConnecting = false;

        public event EventHandler<AblationTimerEventArgs> AblationTimerChangedEvent;

        private Enumeration Enumeration = new Enumeration();

        private bool canUpadteRequiredAblationTime = true;


        #region Solenoid Valves States

        private bool isSolenoidValve1ON = false;
        private bool isSolenoidValve2ON = false;
        private bool isSolenoidValve3ON = false;
        private bool isSolenoidValve4ON = false;
        private bool isSolenoidValve5ON = false;
        private bool isSolenoidValve6ON = false;
        private bool isSolenoidValve7ON = false;
        private bool isSolenoidValve8ON = false;
        private bool isSolenoidValve9ON = false;


        #endregion

        private bool allowFirmwareReading = false;
        private bool isUsedForEngineering = false;
        private bool isFirmwareConsumedDataCorrectly = false;
        private bool isBootLoaderUpdatingFirmware = false;

        private int numberOfRetry = 8;

        private int skinToSkinDuration = 0;

        private bool isAllowedToSetPlayBack = false;

        private bool gUIIsRunning = true;

        private bool isBloodPressureSensorConnected = false;

        private bool isMultiEtsSesnorConnected = false;

        private bool isUsingLowFlow = false;

        private bool isLowFlowActivated = false;

        private bool isUsingDaylightSavingTime = false;

        private bool isUserManualOpned = false;

        private bool isRemoteControlInError = false;

        private double[] currentBloodPressureValue;   

        private double occlusionPressureTareValue = 0;
        private int occlusionPressureGraphAxisYMaximum = 40;
        private int occlusionPressureGraphAxisYMinimum = 10;
        private int occlusionPressureGraphSweepSpeed = 6;

        ASCIIToByteConverter aSCIIToByteConverter;

        byte[] bootLoaderData;

        uint packetNumber = 0;
        private double upgradeStatus = 0;
        private double moduleKey = 0;

        Tuple<long, string, string, string> LoadCellWeightWarningSolution;
        Tuple<long, string, string, string> PressureInTankIsHighFanToBeOnSolution;
        Tuple<long, string, string, string> PressurePT1InTankIsLowSolution;
        Tuple<long, string, string, string> PressurePT1InTankIsHighSolution;
        Tuple<long, string, string, string> SubCoolerTemperatureIsHighSolution;

        private int treatmentNumber;
        #region Warrning From the DB


        #endregion


        private ISerialPortManager _spManager;

        private LSPROEnumeartion lSPROEnumeartion;

        private bool isLsproInitialized = false;

        private string portName = string.Empty;

        readonly static long iCBTimeOutAtInitialization = Properties.Settings.Default.ICBTimeOutAtInitialization; //5

        readonly static long canTwoMaximumTimeOutReference = Properties.Settings.Default.canTwoMaximumTimeOutReference; //3

        private int repeaterFirmwareDBVersion = 0;

        private int iCBFirmwareDBVersion = 0;

        private int remoteControlFirmwareDBVersion = 0;

        private int patientMicroControllerFirmwareVersionDBVersion = 0;

        private int patientMicroControllerBootLoaderFirmwareVersionDBVersion = 0;

        private int repeaterBootLoaderFirmwareDBVersion = 0;

        private int cpldFirmwareVersionDBVersion = 0;

        private int centralMicroControllerFirmwareVersionDBVersion = 0;

        private int centralMicroControllerBootLoaderFirmwareVersionDBVersion = 0;

        private int catheterFirmwareVersionDBVersion = 0;

        public double tC1LSProReading = 0;
        //  private int consoleVersionID = 0;

        private bool wasAblationTimeManuallyChanged = false;

        private int temporaryManualAblationTime = 240;

        private bool isFixedTimerSelected = true;
        private bool iSTTIFixedTimerSelected = false;
        private bool iSTTIDurationTimerSelected = false;
        private bool iSTTISelected = false;
        List<Tuple<long, string, string, string>> RemoteControlIssueMessageList = new List<Tuple<long, string, string, string>>();

        /// <summary>
        /// This constructor initializes the Common View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CommonViewModel(Machine machine_, ISerialPortManager serialPortManager_)
        {
            catheterMaximumTimeDisconnection = Convert.ToInt64(ConfigurationManager.AppSettings["CMTD"]);
            // We create the DB first
            this.data = new Data();

            // Warrning 
            LoadCellWeightWarningSolution = data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.LoadCellWeightWarning, (int)Enumeration.ErrorTypes.CMCU);
            PressureInTankIsHighFanToBeOnSolution = data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PressureInTankIsHighFanToBeOn, (int)Enumeration.ErrorTypes.CMCU);
            PressurePT1InTankIsLowSolution = data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PressurePT1InTankIsLow, (int)Enumeration.ErrorTypes.CMCU);
            PressurePT1InTankIsHighSolution = data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PressurePT1InTankIsTooHigh, (int)Enumeration.ErrorTypes.CMCU);
            SubCoolerTemperatureIsHighSolution = data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.SubCoolerTemperatureIsHigh, (int)Enumeration.ErrorTypes.CMCU);

            console = machine_;
            current = this;

            this.catheterValidator = new CatheterValidator(this.data);

            catheterConnectedTimer.Interval = 500000; // we are using 500ms inteval
            catheterConnectedTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(catheterConnectedTimer_tick);

            InitializeRegisterIDSDynamicTables();
            InitializeAckRegistersTable();

            ablationSummary = new AblationSummary();
            physicianList = new List<Physician>();
            actionLog = new ObservableCollection<Models.ActionLogRecord>();
#if Simulator
            ackTimer.Interval = 2000;
            ackTimer.Elapsed += ackTimerTimer_tick;
#endif
            //Set usage  time
            MinutesOfUse = this.data.DataAccess.GetConsoleUtilisationDuration();

            UsageTimer.Interval = 60000;
            UsageTimer.Elapsed += UsageTimer_tick;
            UsageTimer.Start();

            Ecgs1And2StopWatch.Start();
            Ecgs3And4StopWatch.Start();

            loginManager = new LoginManager(this.data.DataAccess);  //get users list
            loginManager.PropertyChanged += CurrentLogin_PropertyChanged;

            warningMessagesManager = new WarningMessagesManager.WarningMessagesManager();
            if (warningMessagesManager != null && warningMessagesManager.WarningMessagesList != null)
            {
                warningMessagesManager.WarningMessagesList.CollectionChanged += WarningMessagesList_CollectionChanged;
            }

            DataAccessLayer.Tank _tank = this.Data?.DataAccess?.GetCurrentTank();

            if (_tank != null)
            {
                CurrentTank = _tank;
                TankBuilder tankBuilder = new TankBuilder(CurrentTank, data);

                Console.Tank.MetalWeight = tankBuilder.MetalWeight;

                this.console.LoadCellOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].LoadCellThresholdFail = (double)(this.Data?.DataAccess?.GetLoadCellThresholdFail() + tankBuilder.MetalWeight);
            }
            // If MetalWeight is not created before the CommonViewModel object on bootup, force an initial value to LC1 to create it.
            LC1Reading = 30;

            Languages.InitializeErrorTranslation();

            DatabaseVersion = (int)this.data?.DataAccess?.GetDatabaseVersion();

            Console.PurgeTheConsole = data.DataAccess.IsConsoleUsingPurgeFunctionality();

            Console.DeactivateFeatuers = data.DataAccess.IsConsoleUsingCatheterDeflateSwitchFunctionality();

            BalloonRampDown.IsBalloonRampDownActivated = data.DataAccess.IsConsoleUsingBalloonRampDownFunctionality();
            ConnectionBox = new ConnectionBox() {DiaphragmeMinimumValue = Constants.MaxDMSDetectionThreshold};

#if !DEBUG

      if (!data.DataAccess.IsConsoleReleased())
          {
				LogInfo("Console is not released. Shutdown application.");
            System.Windows.Application.Current.Shutdown();
            //Do not allow the user to relaese the console until the validation is done
          }
#endif
            InflateDeflateBalloonModel = new InflateDeflateBalloonModel(this.data, this.console);

            ChangeBalloonTypeFSM = new ChangeBalloonTypeFSM(InflateDeflateBalloonModel);
            ChangeBalloonTypeFSM.PropertyChanged += ChangeBalloonTypeFSM_PropertyChanged;

            DiaphragmConditioning = new DiaphragmConditioning(0);
            DiaphragmConditioning.PropertyChanged += DiaphragmConditioning_PropertyChanged;


            this.DMSLogic = new DMSLogic();
      
            if (data.DataAccess.GetCurrentWeightUnit() == (int)Enumeration.WeightUnit.Lbs)
            {
                Scale.CurrentWeightUnit = Enumeration.WeightUnit.Lbs;
                Toise.CurrentToiseUnit = Enumeration.LengthUnit.Inches;
            }
            else
            {
                Scale.CurrentWeightUnit = Enumeration.WeightUnit.Kg;
                Toise.CurrentToiseUnit = Enumeration.LengthUnit.Centimeters;
            }

            ASCIIToByteConverter = new ASCIIToByteConverter();

            BootLoaderData = new byte[8];

            SkinToSkinAblationTimer.Interval = 1000;
            SkinToSkinAblationTimer.Elapsed += SkinToSkinAblation_Tick;

            Tuple<Int64, Int64> HardDriveLimits = data.DataAccess.GetHardDriveLimits();

            DrivesInformation.WarningThreshold = HardDriveLimits.Item1;
            DrivesInformation.FailureThreshold = HardDriveLimits.Item2;

            DrivesInformation.GetTotalFreeSpace();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo assemblyVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            GuiVersion = assemblyVersion.FileVersion;


            if (data.DataAccess.IsConsoleUsingDeflateAfterThawFunctionality())
            {
                Console.DeflateAfterThaw = true;
                Console.EnableDefalteAfterThaw = true;

            }
            else
            {
                Console.DeflateAfterThaw = false;
                Console.EnableDefalteAfterThaw = false;
            }

      Console.IsUsingBloodPressureSensor = data.DataAccess.IsUsingBloodPressureSensor();
      
            RemoteControlFSM = new RemoteControlFSM();

            try
            {
              PortName = data.DataAccess.GetLSPROComPort();

              SpManager = serialPortManager_;
              SpManager.NewSerialDataRecieved +=
                new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
              // Update and Start listening LSPROCRC32   
              SpManager.SetPortName(PortName); 
              SpManager.InitializeLSPROCRC32AndStart();
            }
            catch (Exception ex)
            {
	            LogException(ex);
            }

            lSPROEnumeartion = new LSPROEnumeartion();

            IsLsproInitialized = SpManager.IsLsproInitialized;

            AblationSite = AblationSiteEnum.OTHER;

            IsUsingLowFlow = data.DataAccess.IsSystemUsingLowFlow();
            IsUsingDaylightSavingTime = data.DataAccess.IsUsingDaylightSavingTime();
            //  IsUsingAutoPl

            outerBalloonPressureThreshold = new OuterBalloonPressureThreshold();
            
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
            {
              this.SubscribeTransducersEvents();

              //Subscribe  to the registers
              this.console.registerEvent += new EventHandler<RegisterValuesEventArgs>(RegisterChanged);
              //Subscribe to Connection box register
              this.console.canTwoRegisterEvent += new EventHandler<RegisterValuesEventArgs>(CanTwoRegisterChanged);
            });
        }

        private void DiaphragmConditioning_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (SystemState == MessageStateId.CAN_ID_STATE_TRANSITION || SystemState == MessageStateId.CAN_ID_STATE_ABLATION || SystemState == MessageStateId.CAN_ID_STATE_THAWING)
            {
                Task.Delay(1000).ContinueWith(t => Console.IsConsoleInAblationState = true);
                Task.Delay(1000).ContinueWith(t => DiaphragmConditioning.IsDiaphragmReseting = false);

            }

        }

        /// <summary>
        /// Occurs when the skin to skin ablation Timer Tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinToSkinAblation_Tick(object sender, EventArgs e)
        {
            // for Test we use IsCatheterCableConnected = true;
            ProcedureLogModel.TrackSkinToSkinDuration(CatheterTemperature, SystemState, IsCatheterCableConnected, true);
        }

        /// <summary>
        /// Gets or sets the Accessed Change Tank from Cryotherapy screen value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool AccessedChangeTankFromCryotherapy
        {
            get
            {
                return accessedChangeTankFromCryotherapy;
            }
            set
            {
                accessedChangeTankFromCryotherapy = value;
                RaisePropertyChanged("AccessedChangeTankFromCryotherapy");
            }
        }

        /// <summary>
        /// Gets or sets the Screen Name value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ScreenName
        {
            get
            {
                return screenName;
            }
            set
            {
                screenName = value;
                RaisePropertyChanged("ScreenName");
            }
        }

        /// <summary>
        /// Gets or sets the Database version value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DatabaseVersion
        {
            get
            {
                return databaseVersion;
            }
            set
            {
                databaseVersion = value;
                RaisePropertyChanged("DatabaseVersion");
            }
        }

        /// <summary>
        /// Gets or sets The Graphical user interface version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string GuiVersion
        {
            get
            {
                return guiVersion;
            }
            set
            {
                guiVersion = value;
                RaisePropertyChanged("GuiVersion");
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
                return requiredAblationTime;
            }

            set
            {
                requiredAblationTime = value;

                if (requiredAblationTime <= 0)
                    requiredAblationTime = 30;
                else if (requiredAblationTime > 240)
                    requiredAblationTime = 240;

                RequiredAblationTimeAccordingToState = requiredAblationTime;
                RaisePropertyChanged("RequiredAblationTime");
                //   RaisePropertyChanged("RequiredAblationTimeAccordingToState");
                AblationTimerChangedEvent?.Invoke(null, new AblationTimerEventArgs(requiredAblationTime));


            }
        }

        /// <summary>
        /// Gets/sets required ablation time according to state value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RequiredAblationTimeAccordingToState
        {
            get
            {
                return requiredAblationTimeAccordingToState;
            }
            set
            {
                // if (!IsPlayBackModeDeactivted) //???
                // {
                requiredAblationTimeAccordingToState = value;
                RaisePropertyChanged("RequiredAblationTimeAccordingToState");
                //  }
            }
        }

        /// <summary>
        /// Gets or sets the Maintenance Screen Name value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string MaintenanceScreenName
        {
            get
            {
                return maintenanceScreenName;
            }
            set
            {
                maintenanceScreenName = value;
                RaisePropertyChanged("MaintenanceScreenName");
            }
        }

        /// <summary>
        /// Gets or sets Is Maintenance Mode Screen Selected value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsMaintenanceModeScreenSelected
        {
            get
            {
                return isMaintenanceModeScreenSelected;
            }
            set
            {
                isMaintenanceModeScreenSelected = value;
                RaisePropertyChanged("IsMaintenanceModeScreenSelected");
            }
        }

        /// <summary>
        /// Gets if the heart beat is activated
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool HeartbeatActivated
        {
            get
            {
                return Current.Console.HeartbeatActivated;
            }
        }


        /// <summary>
        /// Gets or sets the Deflate After Thaw boolean flag value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DeflateAfterThaw
        {
            get
            {
                return Current.Console.DeflateAfterThaw;
            }
            set
            {
                Current.Console.DeflateAfterThaw = value;
                RaisePropertyChanged("DeflateAfterThaw");
            }
        }

        /// <summary>
        /// Gets or sets the Enable Slow Inflation Mode value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnableFastInflationMode
        {
            get
            {
                return Current.Console.EnableFastInflationMode;
            }

            set
            {
                Current.Console.EnableFastInflationMode = value;
                RaisePropertyChanged(nameof(EnableFastInflationMode));
            }
        }
        /// <summary>
        /// Gets or sets the Lock the foot switch boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool LockTheFootSwitch
        {
            get
            {
                return Current.Console.LockTheFootSwitch;
            }

            set
            {
                Current.Console.LockTheFootSwitch = value;
                RaisePropertyChanged("LockTheFootSwitch");
            }

        }
        /// <summary>
        /// Gets or sets the activate diaphragm and esophagus audio alerts boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool ActivateDiaphragmAndEsophagusAudioAlerts
        {
            get
            {
                return Current.Console.ActivateDiaphragmAndEsophagusAudioAlerts;
            }

            set
            {
                Current.Console.ActivateDiaphragmAndEsophagusAudioAlerts = value;
                RaisePropertyChanged("ActivateDiaphragmAndEsophagusAudioAlerts");
            }


        }

        /// <summary>
        /// Gets or sets the purge the console boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool PurgeTheConsole
        {
            get
            {
                return Current.Console.PurgeTheConsole;
            }

            set
            {
                Current.Console.PurgeTheConsole = value;
                RaisePropertyChanged("PurgeTheConsole");
            }

        }


        /// <summary>
        /// Gets or sets the deactive features boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DeactivateFeatuers
        {
            get
            {
                return Current.Console.DeactivateFeatuers;
            }

            set
            {
                Current.Console.DeactivateFeatuers = value;
                RaisePropertyChanged("DeactivateFeatuers");
            }

        }



        /// <summary>
        /// Gets or sets the enhanced audio features boolean value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnabaleEnhancedAudio
        {
            get
            {
                return Current.Console.EnabaleEnhancedAudio;
            }

            set
            {
                Current.Console.DeactivateFeatuers = value;
                RaisePropertyChanged("EnabaleEnhancedAudio");
            }

        }


        /// <summary>
        /// Gets or sets the Required Volume value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public uint RequiredVolume
        {
            get
            {
                return requiredVolume;
            }
            set
            {
                if (!IsCanOneInError)
                {

                    if (value > 100 || value < 0)
                        return;
                    requiredVolume = value;
                    CommonViewModel.Current.Console.SetAudioLevel(requiredVolume);
                    RaisePropertyChanged("RequiredVolume");
                }


            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the sytem is using low flow 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsingLowFlow
        {
            get
            {
                return isUsingLowFlow;
            }
            set
            {
                isUsingLowFlow = value;
                RaisePropertyChanged("IsUsingLowFlow");

            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the low flow is activated 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsLowFlowActivated
        {
            get
            {
                return isLowFlowActivated;
            }
            set
            {
                isLowFlowActivated = value;
                RaisePropertyChanged("IsLowFlowActivated");

            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the daylight saving is used 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsingDaylightSavingTime
        {
            get
            {
                return isUsingDaylightSavingTime;
            }
            set
            {
                isUsingDaylightSavingTime = value;
                RaisePropertyChanged("IsUsingDaylightSavingTime");

            }
        }
        /// <summary>
        /// Gets or sets current blood pressure value 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double[] CurrentBloodPressureValue
        {
          get => currentBloodPressureValue;
          set => SetProperty(ref currentBloodPressureValue, value); 
        }

        /// <summary>
        /// Starts Can Bus One stopwatch communication monitoring
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StartCanOneStopWatchCommunicationMonitoring()
        {
            if (canOneTimer != null)
            {
                canOneTimer.Interval = 3000;
                canOneTimer.Elapsed -= canOneTimer_tick;
                canOneTimer.Elapsed += canOneTimer_tick;
                canOneTimer.Start();
            }

            if (CanOneStopWatchCommunicationLost != null)
            {
                CanOneStopWatchCommunicationLost.Restart();
            }
            RequiredVolume = 100;
        }

        /// <summary>
        /// Starts CAN Bus Two stopwatch communication monitoring
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StartCanTwoStopWatchCommunicationMonitoring()
        {
            if (canTwoTimer != null)
            {
                canTwoTimer.Interval = 3000;
                canTwoTimer.Elapsed -= canTwoTimer_tick;
                canTwoTimer.Elapsed += canTwoTimer_tick;
                canTwoTimer.Start();
            }

            CanTwoStopWatchCommunicationLost?.Restart();
            ICBStopWatchDisconnection?.Restart();
        }


        /// <summary>
        /// Stop CAN BUS two stopWatch communication Monitoring
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StopCanTwoStopWatchCommunicationMonitoring()
        {
            if (canTwoTimer != null)
            {
                canTwoTimer.Stop();
            }

            CanTwoStopWatchCommunicationLost?.Stop();
            CanTwoStopWatchCommunicationLost?.Reset();

            ICBStopWatchDisconnection?.Stop();
            ICBStopWatchDisconnection?.Reset();
        }

        /// <summary>
        /// Start Remote control stopWatch communication monitoring
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void StartRemoteControlStopWatchCommunicationMonitoring()
        {
            if (remoteControlTimer != null)
            {
                remoteControlTimer.Interval = 3000;
                remoteControlTimer.Elapsed += remoteControlTimer_tick;
                remoteControlTimer.Start();
            }

            if (RemoteControlStopWatchDisconnection != null)
            {
                RemoteControlStopWatchDisconnection.Start();
            }
        }

        /// <summary>
        /// Gets the Action Log observable collection
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<UserAction> ActionLog
        {
            get
            {
                return data.DataAccess.GetAllUserActions();
            }
        }

        /// <summary>
        /// Gets or sets the Physician List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<Physician> PhysicianList
        {
            get
            {
                return physicianList;
            }
            set
            {
                this.physicianList = value;
                RaisePropertyChanged("PhysicianList");
            }
        }

        /// <summary>
        /// Logs a User Action in the database
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="actionID">The action ID to log in the database.</param>
        public void LogUserAction(Enumeration.Actions actionID, string msg = "")
        {
            if (LoginManager.CurrentUser != null && data?.DataAccess != null)
            {
                data.DataAccess.LogUserAction(LoginManager.CurrentUser, (int)actionID, msg);
            }
        }

        /// <summary>
        /// Occurs when the Warning Messages List's collection changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Warning Message List Collection.</param>
        /// <param name="e">A System.Collections.Specialized.NotifyCollectionChangedEventArgs that contains the event data.</param>
        private void WarningMessagesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IsWarningVisible");
            RaisePropertyChanged("WarningMessagesManager");
        }

        /// <summary>
        /// Occurs when the Current Login property changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Current Login object.</param>
        /// <param name="e">A System.ComponentModel.PropertyChangedEventArgs that contains the event data.</param>
        private void CurrentLogin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UserCostTrackingType":
                case "UserAccessControlType":
                case "UserAuthenticationType":
                    RaisePropertyChanged("CurrentUser");
                    break;

                case "CurrentLogin":
                    RaisePropertyChanged("CurrentUser");
                    break;
            }
        }


        /// <summary>
        /// Occurs when the Balloon Type property changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Current Login object.</param>
        /// <param name="e">A System.ComponentModel.PropertyChangedEventArgs that contains the event data.</param>
        private void ChangeBalloonTypeFSM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case "InflateDeflateBalloonModel":

                    break;

                case "DASBalloonEnabled":
                    {
                        IsFirmwareConsumedDataCorrectly = false;
                        Task<bool> task = SendDasPressureSetpointAndACK();
                    }

                    break;
            }
        }
        /// <summary>
        ///Send DAS Pressure set point information
        ///. Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <Id>SF-SDS-0138</Id>
        private async Task<bool> SendDasPressureSetpointAndACK()
        {
            List<int> AckTracking = new List<int>();
            AcknowledgeVerificationUsingAbsorbentElement acknowledgeVerificationUsingAbsorbentElement = new AcknowledgeVerificationUsingAbsorbentElement();


            await Task.Run(() =>
            {
                foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
                {

                    if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        //TargetBalloonPressure = 0;
                        int state = 0;
                        state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                        double targetInjectionFlow = ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionFlow;
                        double targetInjectionPressure = ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionPressure;

                        this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionFlow = targetInjectionFlow;
                        this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionPressure = targetInjectionPressure;

                        this.console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetBalloonPressure = ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentPressureSetpoint;

                        Console.WriteFromMicroController((MessageStateId)state, CatheterInfoIds[1]);
                        System.Threading.Thread.Sleep(10);

                        Console.WriteFromMicroController((MessageStateId)state, CentralMicroControllerTargetInjectionFlow);
                        System.Threading.Thread.Sleep(10);
                    }
                }
            });

            return true;
        }


        /// <summary>
        /// Send low flow value
        ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal async void SendLowFlowValue()
        {
            await Task.Run(() =>
            {
                //       
                int state = 0;
                state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), MessageStateId.CAN_ID_STATE_ABLATION);

                double _targetInjectionLowFlow = 0;
                double _targetInjectionFlow = 0;

                _targetInjectionLowFlow = this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionLowFlow;
                _targetInjectionFlow = ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow;
                DASLowFlow = this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].DASLowFlow;

                if (!IsLowFlowActivated && !ChangeBalloonTypeFSM.DASBalloonEnabled)
                {
                    //send 7800
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow = baseTargetInjectionFlow;

                }

                else if ((IsLowFlowActivated && ChangeBalloonTypeFSM.DASBalloonEnabled))
                {
                    //send define the value
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow = DASLowFlow;

                }
                else if (!IsLowFlowActivated && ChangeBalloonTypeFSM.DASBalloonEnabled)
                {
                    //DAS value
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow = _targetInjectionFlow;

                }

                else if (IsLowFlowActivated && !ChangeBalloonTypeFSM.DASBalloonEnabled)
                {

                    // send 6800
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow = _targetInjectionLowFlow;

                }

                Console.WriteFromMicroController((MessageStateId)state, CentralMicroControllerTargetInjectionFlow);
                System.Threading.Thread.Sleep(10);
            });

        }


        /// <summary>
        /// Subscribe to Transducers events
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void SubscribeTransducersEvents()
        {
            if (this.console != null)
            {
                this.console.pressureTransducerEvent += new EventHandler<PressureTransducerEventArgs>(PressureChanged);
                this.console.thermocoupleEvent += new EventHandler<ThermocoupleEventArgs>(TemperatureChanged);
                this.console.pressureSwitchEvent += new EventHandler<PressureSwitchEventArgs>(PressureSwitchChanged);
                this.console.flowMeterEvent += new EventHandler<FlowMeterEventArgs>(FlowChanged);
                this.console.loadCellEvent += new EventHandler<LoadCellEventArgs>(LoadChanged);
                this.console.bloodDetectorEvent += new EventHandler<BloodDetectorEventArgs>(BloodDetectorEvent);
                this.console.ecgEventArgs += new EventHandler<EcgEventArgs>(EcgChannel1And2Channel3And4Channel5And6Channel7And8Changed);
                this.console.remoteControlMembraneSwitchStateEventArgs += new EventHandler<RemoteControlMembraneSwitchStateEventArgs>(RemoteControlMembraneChanged);
                this.console.bloodPressureSensorStateEventArgs += new EventHandler<BloodPressureSensorEventArgs>(BloodPressureSensorConnectionChanged);
                this.console.probeEventArgs += new EventHandler<ProbeEventArgs>(ProbeSesnorChanged);
            }
        }

        /// <summary>
        /// Unsubscribe to Transducers events
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void UnsubscribeTransducersEvents()
        {
            if (this.console != null)
            {
                this.console.pressureTransducerEvent -= new EventHandler<PressureTransducerEventArgs>(PressureChanged);
                this.console.thermocoupleEvent -= new EventHandler<ThermocoupleEventArgs>(TemperatureChanged);
                this.console.pressureSwitchEvent -= new EventHandler<PressureSwitchEventArgs>(PressureSwitchChanged);
                this.console.flowMeterEvent -= new EventHandler<FlowMeterEventArgs>(FlowChanged);
                this.console.loadCellEvent -= new EventHandler<LoadCellEventArgs>(LoadChanged);
                this.console.bloodDetectorEvent -= new EventHandler<BloodDetectorEventArgs>(BloodDetectorEvent);
                this.console.ecgEventArgs -= new EventHandler<EcgEventArgs>(EcgChannel1And2Channel3And4Channel5And6Channel7And8Changed);
            }
        }

        /// <summary>
        /// Returns the ECG time by applying a conversion of the recieved time.  The ECG timer
        /// runs faster than the temperature timer so a conversion is needed to make them match when in
        /// Playback mode
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="time">The ablation procedure time.</param>
        /// <param name="ablationECGDataList">The ablation ECG data list.</param>
        /// <returns>Integer representing the synchronized ECG Time.</returns>
        public int computeECGTimeSynchronized(double time, List<AblationECGData> ablationECGDataList, int correcetionFactor, bool isUsingPixcelCorrectionFactor = false)
        {
            //time = time + 1;

            //ECG is executed at 40ms (25 x per second), make it match with the Temperature that is executed
            //each 1000ms (1x per second).
            if (isUsingPixcelCorrectionFactor)
            {
                return (int)(correcetionFactor * time);
            }
            return (int)(25 * time);
        }

        /// <summary>
        /// Occurs when the Catheter Connected Timer Tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Catheter Connecter Timer.</param>
        /// <param name="e">Represents the base class for classes that
        /// contain event data, and provides a value to use for events that do not include event data.</param>
        private void catheterConnectedTimer_tick(object sender, EventArgs e)
        {
            //if (catheterCommunicationData != null && catheterCommunicationData.CanBusOneEventArgs != null)
            //{
            //    SendRequestedData(catheterCommunicationData, catheterCommunicationData.CanBusOneEventArgs.Id, catheterEventId, true, true);
            //}
        }

#if Simulator
        /// <summary>
        /// Occurs when the Ack Timer tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Ack Timer object.</param>
        /// <param name="e">Represents the base class for classes that contain timer event data.</param>
        private void ackTimerTimer_tick(object sender, EventArgs e)
        {
            lock (_myRegister_Lock)
            {
                List<int> localpatientMicroControllerRegisterIDSDynamicTable = new List<int>();
                localpatientMicroControllerRegisterIDSDynamicTable = PatientMicroControllerRegisterIDSDynamicTable;
                List<int> localcentralMicroControllerRegisterIDSDynamicTable = new List<int>();
                localcentralMicroControllerRegisterIDSDynamicTable = CentralMicroControllerRegisterIDSDynamicTable;

                if (PatientMicroControllerAckRegistersTable != null && !PatientMicroControllerAckRegistersTable.ContainsValue(false) &&
                    CentralMicroControllerAckRegistersTable != null && !CentralMicroControllerAckRegistersTable.ContainsValue(false))
                {
                    InitializeRegisterIDSDynamicTables();
                    switch (SimulatedSystemState)
                    {
                        case MessageStateId.CAN_ID_STATE_IDLE:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_READY;
                            break;

                        case MessageStateId.CAN_ID_STATE_READY:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_INFLATION;
                            break;

                        case MessageStateId.CAN_ID_STATE_INFLATION:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_TRANSITION;
                            break;

                        case MessageStateId.CAN_ID_STATE_TRANSITION:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_ABLATION;
                            break;

                        case MessageStateId.CAN_ID_STATE_ABLATION:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_THAWING;
                            break;

                        case MessageStateId.CAN_ID_STATE_THAWING:
                            SimulatedSystemState = MessageStateId.CAN_ID_STATE_IDLE;
                            if (ackTimer != null)
                            {
                                ackTimer.Stop();
                            }
                            return;
                    }
                }

                foreach (var item in PatientMicroControllerAckRegistersTable)  // 52 55
                {
                    if (item.Value == false && Console != null)
                    {
                        Console.ReadFromMicroController(SimulatedSystemState, item.Key);
                        System.Threading.Thread.Sleep(20);
                    }
                }

                foreach (var item in CentralMicroControllerAckRegistersTable)  //  15   34
                {
                    if (item.Value == false && Console != null)
                    {
                        Console.ReadFromMicroController(SimulatedSystemState, item.Key);  //id
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }
        }
#endif 

        /// <summary>
        /// Occurs when the Can One Timer Tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Can One Timer.</param>
        /// <param name="e">Represents the base class for classes that contain can one timer event data.</param>
        private void canOneTimer_tick(object sender, EventArgs e)
        {
            if ((!IsCanOneInError && !StopListeningCanOneCommunication && HeartbeatActivated) && !Console.GUIInMaintenanceMode)
            {
                if (CanOneStopWatchCommunicationLost != null &&
                    CanOneStopWatchCommunicationLost.ElapsedMilliseconds > canOneMaximumTimeOut)
                {
                    this.Console.InjectionDisable();
                    IsCanOneInError = true;
                    ErrorIdMessageAndSolutionList.Add(Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID82, (int)Enumeration.ErrorTypes.GUI));
                    GenericError = $"Error 2 - {(int)Enumeration.GUIMessages.ID82:X8} " + Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID82, (int)Enumeration.ErrorTypes.GUI).Item2;

                    List<Tuple<long, string, string, string>> copyOfErrorIdMessageAndSolutionList = new List<Tuple<long, string, string, string>>(ErrorIdMessageAndSolutionList);
                    Task.Delay(4000).ContinueWith(t => SavErrors((int)Enumeration.ErrorTypes.GUI, copyOfErrorIdMessageAndSolutionList, PreviousSystemState));
                    LogInfo("CAN1 Communication error occurs");
                    DisplayErrorMessage("CAN1 Communication", string.Empty);
                    //DisplayRemoteControlWarningMessage("CAN1 Communication", string.Empty);
                }
            }
            //else if (WarningMessageManager.SearchMessage("CAN1 Communication"))
            //{
            //    this.Console.InjectionDisable();
            //}
        }


        /// <summary>
        /// Occurs when the Can Two Timer Tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Can Two Timer.</param>
        /// <param name="e">Represents the base class for classes that contain can one timer event data.</param>
        private void canTwoTimer_tick(object sender, EventArgs e)
        {
            if (ICBStopWatchDisconnection.ElapsedMilliseconds > ICBMaximumTimeOut)
            {
#if (Simulator == false)
                DisconnectTheICB();
#endif

            }

            if ((!IsCanTwoInError && !StopListeningCanTwoCommunication && IsCatheterCableConnected && CanstartDiaphragmMovementMonitoring && HeartbeatActivated) && !Console.GUIInMaintenanceMode)
            {
                if (CanTwoStopWatchCommunicationLost != null &&
                    CanTwoStopWatchCommunicationLost.ElapsedMilliseconds > canTwoMaximumTimeOut && Console.AskForVitalParameters)
                {
                    this.Console.InjectionDisable();
                    IsCanTwoInError = true;
                    GenericError = $"Error 2 - {(int)Enumeration.GUIMessages.ID83:X8} " + Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID83, (int)Enumeration.ErrorTypes.GUI).Item2;
                    ErrorIdMessageAndSolutionList.Add(Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID83, (int)Enumeration.ErrorTypes.GUI));

                    List<Tuple<long, string, string, string>> copyOfErrorIdMessageAndSolutionList = new List<Tuple<long, string, string, string>>(ErrorIdMessageAndSolutionList);
                    Task.Delay(4000).ContinueWith(t => SavErrors((int)Enumeration.ErrorTypes.GUI, copyOfErrorIdMessageAndSolutionList, PreviousSystemState));
                    LogInfo("CAN2 Communication error occurs");

                    DisplayErrorMessage("CAN2 Communication", string.Empty);
                }
            }
            else if (WarningMessageManager.SearchMessage("CAN2 Communication"))
            {
                this.Console.InjectionDisable();
            }
        }


        /// <summary>
        /// Occurs when the remote controler Timer Tick event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">Remote control.</param>
        /// <param name="e">Represents the base class for classes that contain can one timer event data.</param>
        private void remoteControlTimer_tick(object sender, EventArgs e)
        {
            if ((!IsCanTwoInError && !StopListeningCanTwoCommunication && IsCatheterCableConnected && CanstartDiaphragmMovementMonitoring && HeartbeatActivated) && !Console.GUIInMaintenanceMode)
            {
                if (RemoteControlStopWatchDisconnection != null &&
                    RemoteControlStopWatchDisconnection.ElapsedMilliseconds > remoteContolMaximumTimeOut)
                {
                    DisplayErrorMessage("Remeote Control HeartBeat Time Out", string.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the Usage Timer  event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Usage Timer.</param>
        /// <param name="e">Represents the base class for classes that contain can two timer event data.</param>
        private void UsageTimer_tick(object sender, EventArgs e)
        {
            MinutesOfUse++;
            this.data.DataAccess.ChangeConsoleUtilisationDuration();
        }

        private void RestartCanTwoStopWatchCommunicationLost(bool resetTimeoutValue = true ) 
        {
          CanTwoStopWatchCommunicationLost?.Restart();
          ICBStopWatchDisconnection?.Restart();

          if (IsCanTwoInError)
          {
            IsCanTwoInError = false;
          }

          if (resetTimeoutValue)
          {
            canTwoMaximumTimeOut = canTwoMaximumTimeOutReference;
          }
        } 

        /// <summary>
        /// Occurs when the Console's ecgEventArgs is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The Console ECG Event Args.</param>
        /// <param name="e">Represents the base class for classes that contain channel 1...10 event data</param>
        private void EcgChannel1And2Channel3And4Channel5And6Channel7And8Changed(object sender, EcgEventArgs e)
        {
            RestartCanTwoStopWatchCommunicationLost();

            var communicationData = sender as ICanBusCommunication;

            byte[] data = communicationData.CanBusTwoEventArgs.Data;

            switch (e.ID)
            {
                case 8:
#if (Simulator == False)
                    EcgChannel1And2Reading = CanBusMessageConverter.ConverteECGDecimalData(data, 0, 100.0);
#endif
                    //ESO Temp

                    double temporayEsoValue = CanBusMessageConverter.ConverteECGDecimalData(data, 4);
                    EtsSesnor13 = temporayEsoValue;

                    if (temporayEsoValue == -100)
                    {
                        temporayEsoValue = 100;
                    }
                    EcgChannel5And6Reading = temporayEsoValue;

                    // THE diaphragm graph 
                    EcgChannel3And4Reading = CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0);

                    if (IsMultiEtsSesnorConnected && !AreSensorsInPlayBackMode)
                        AnalyseEsophagusTemperature();


                    //The diaphragm %
                    EcgChannel7And8Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 6);


                    break;

                case 9:
                    EcgChannel9And10Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
                    ChannelTipReading = CanBusMessageConverter.ConverteDecimalData(data, 2);
                    ChannelAccelerometerReading = CanBusMessageConverter.ConverteDecimalData(data, 4);

                    EcgChannel9And10Readings.Add(EcgChannel9And10Reading);
                    ChannelTipReadings.Add(ChannelTipReading);
                    ChannelAccelerometerReadings.Add(ChannelAccelerometerReading);
                    break;

                case 32: // High-resolution DMS message
                  var dmsData = new double[4];
                  dmsData[0] = CanBusMessageConverter.ConverteECGDecimalData(data, 0, 100.0);
                  dmsData[1] = CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0);
                  dmsData[2] = CanBusMessageConverter.ConverteECGDecimalData(data, 4, 100.0);
                  dmsData[3] = CanBusMessageConverter.ConverteECGDecimalData(data, 6, 100.0);
                  HighResolutionDmsReading = dmsData;

                  break;
            }
        }

        /// <summary>
        /// Occurs when remote control membrane changes
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <param name="sender">Remote control.</param>
        /// <param name="e">Membrane state event</param>
        /// <Id> SF-SDS-0139</Id>
        private void RemoteControlMembraneChanged(object sender, RemoteControlMembraneSwitchStateEventArgs e)
        {
            RestartCanTwoStopWatchCommunicationLost();

            var communicationData = sender as ICanBusCommunication;

            byte[] data = communicationData.CanBusTwoEventArgs.Data;

            if (!SensorReadingMananger.AllowRemoteControl || (IsDoctor && IsUsedForEngineering))
                return;

            switch (e.ID)
            {
                case 26:

                    RemoteControlFSM.MembraneSwitchStateLogic(data, e.ID);

                    if (RemoteControlFSM.CurrrentSwitchState == SwitchState.SwitchStateDeactivated)
                    {
                        IsRemoteControlInError = false;
                    }
                    else
                    {
                        if (!IsRemoteControlInError)
                        {
                            if (remoteControlFSM.CurrrentSwitchState == SwitchState.StopButton)
                            {
                                Console.Stop();

                                if (SystemState == MessageStateId.CAN_ID_STATE_READY)
                                {
                                    Console.Disconnect();
                                    IsVacuumDisconnected = true;
                                }
                            }
                            //else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.StartButton &&
                            //         IsCatheterCableConnected && IsCMCUReady && IsPMCUReady)
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.StartButton)
                            {
                                if (IsCatheterCableConnected && IsCMCUReady && IsPMCUReady)
                                {
                                    if (SystemState == MessageStateId.CAN_ID_STATE_IDLE)
                                    {
                                        Console.Connect();
                                        IsVacuumDisconnected = false;
                                    }
                                    else if (SystemState != MessageStateId.CAN_ID_STATE_INFLATION || _allowStartAblation) 
                                    {
                                        Console.Start();
                                    }

                                    if (systemState == MessageStateId.CAN_ID_STATE_ABLATION ||
                                        systemState == MessageStateId.CAN_ID_STATE_TRANSITION)
                                    {
                                        IsVeinIsolated = true;
                                    }
                                }
                            }
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.AblationTimerIncrement)
                            {
                                if (SystemState != MessageStateId.CAN_ID_STATE_THAWING && !AreSensorsInPlayBackMode)
                                {
                                    RequiredAblationTime += 30;
                                    TemporaryManualAblationTime = RequiredAblationTime;
                                    WasAblationTimeManuallyChanged = true;

                                    ISTTISelected = false;
                                    ISTTIDurationTimerSelected = false;
                                    ISTTIFixedTimerSelected = false;
                                    IsFixedTimerSelected = true;
                                }

                            }
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.AblationTimerDecrement)
                            {
                                if (SystemState != MessageStateId.CAN_ID_STATE_THAWING && (RequiredAblationTime - 30 > CryoTherapyTime) && !AreSensorsInPlayBackMode)
                                {
                                    RequiredAblationTime -= 30;
                                    TemporaryManualAblationTime = RequiredAblationTime;
                                    WasAblationTimeManuallyChanged = true;

                                    ISTTISelected = false;
                                    ISTTIDurationTimerSelected = false;
                                    ISTTIFixedTimerSelected = false;
                                    IsFixedTimerSelected = true;
                                }
                            }
                            //Ablation site
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.AblationSiteLeft)
                            {
                                AblationSiteCarousselModel.MoveAblationSiteToTheLeft();
                                if (AreSensorsInPlayBackMode)
                                {
                                    UpdateAblationSite(TreatmentNumber, AblationSiteCarousselModel.CurrentAblationSite);
                                    GenerateAblationSummary();
                                }
                                AblationSite = AblationSiteCarousselModel.CurrentAblationSite;
                            }
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.AblationSiteRight)
                            {
                                AblationSiteCarousselModel.MoveAblationSiteToTheRight();
                                if (AreSensorsInPlayBackMode)
                                {
                                    UpdateAblationSite(TreatmentNumber, AblationSiteCarousselModel.CurrentAblationSite);
                                    GenerateAblationSummary();
                                }
                                AblationSite = AblationSiteCarousselModel.CurrentAblationSite;
                            }
                            //else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.BalloonDiameterIncrease &&
                            //         (SystemState == MessageStateId.CAN_ID_STATE_INFLATION ||
                            //          SystemState == MessageStateId.CAN_ID_STATE_THAWING))
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.BalloonDiameterIncrease)
                            {
                                if (SystemState == MessageStateId.CAN_ID_STATE_INFLATION || SystemState == MessageStateId.CAN_ID_STATE_THAWING)
                                {
                                    if (SystemState == MessageStateId.CAN_ID_STATE_THAWING && TC1Reading < 20)
                                        return;

                                    if (!ChangeBalloonTypeFSM.DASBalloonEnabled && IsSystemUsingDASBalloon)
                                    {
                                      _rcBalloonDiameterButtonPressedSubject.OnNext(true); 
                                    }
                                }
                            }
                            //else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.BalloonDiameterDecrease &&
                            //         (SystemState == MessageStateId.CAN_ID_STATE_INFLATION ||
                            //          SystemState == MessageStateId.CAN_ID_STATE_THAWING))
                            else if (RemoteControlFSM.CurrrentSwitchState == SwitchState.BalloonDiameterDecrease)
                            {
                                if (SystemState == MessageStateId.CAN_ID_STATE_INFLATION || SystemState == MessageStateId.CAN_ID_STATE_THAWING)
                                {
                                    if (SystemState == MessageStateId.CAN_ID_STATE_THAWING && TC1Reading < 20)
                                        return;

                                    if (BalloonRampDown.IsBalloonRampDownActivated && ChangeBalloonTypeFSM.DASBalloonEnabled)
                                    {
                                      _rcBalloonDiameterButtonPressedSubject.OnNext(false); 
                                    }
                                }
                            }
                            else
                            {
                                IsRemoteControlInError = true;
                                //DisplayRemoteControlWarningMessage("RemoteControlIssue", string.Empty);
                                DisplayErrorMessage("RemoteControlIssue", string.Empty);
                            }
                        }
                    }
                    break;

                case 27:

                    break;

                case 28:
                    RemoteControlStopWatchDisconnection.Restart();
                    break;
            }
        }

        /// <summary>
        /// This property gets/sets the Treatment Number value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TreatmentNumber
        {
            get
            {
                return treatmentNumber;
            }

            set
            {
                treatmentNumber = value;
                RaisePropertyChanged("TreatmentNumber");
            }
        }
        private void BloodPressureSensorConnectionChanged(object sender, BloodPressureSensorEventArgs e)
        {
            var communicationData = sender as ICanBusCommunication;

            byte[] data = communicationData.CanBusTwoEventArgs.Data;

            switch (e.ID)
            {
                case 1:

                    uint status = data[0];
                    IsBloodPressureSensorConnected = (status & (uint)SensorConnectionStatus.Pressure) == (uint)SensorConnectionStatus.Pressure;
                    IsMultiEtsSesnorConnected = (status & (uint)SensorConnectionStatus.ETSMulti) == (uint)SensorConnectionStatus.ETSMulti;
                    break;

                case 7:
                    double[] _bloodPressureValue = new double[4]{ 0, 0, 0, 0 };
                    CanBusMessageConverter.ConverteBloodPressureData(data, out _bloodPressureValue);
                    // Apply the Occlusion Pressure Tare value to the converted occlusion pressure result
                    CurrentBloodPressureValue = _bloodPressureValue
                      .Select(b => Math.Max(b + OcclusionPressureTareValue, 0)).ToArray();
                    break;
            }
        }

        private void ProbeSesnorChanged(object sender, ProbeEventArgs e)
        {
            if (!IsMultiEtsSesnorConnected)
                return;

            var communicationData = sender as ICanBusCommunication;

            byte[] data = communicationData.CanBusTwoEventArgs.Data;

            switch (e.ID)
            {

                case 5:

                    EtsSesnor1 = data[0];
                    EtsSesnor2 = data[1];
                    EtsSesnor3 = data[2];
                    EtsSesnor4 = data[3];
                    EtsSesnor5 = data[4];
                    EtsSesnor6 = data[5];
                    EtsSesnor7 = data[6];
                    EtsSesnor8 = data[7];

                    break;

                case 6:
                    EtsSesnor9 = data[0];
                    EtsSesnor10 = data[1];
                    EtsSesnor11 = data[2];
                    EtsSesnor12 = data[3];

                    break;


            }

            if (!AreSensorsInPlayBackMode)
                AnalyseEsophagusTemperature();
        }
#if Simulator

        double simulatedTemp = 34;

        public void IncreaseSesnorSimulation()
        {
            EtsSesnor1 = simulatedTemp;
            EtsSesnor2 = simulatedTemp;
            EtsSesnor3 = simulatedTemp;
            EtsSesnor4 = simulatedTemp;
            EtsSesnor5 = simulatedTemp;
            EtsSesnor6 = simulatedTemp;
            EtsSesnor7 = simulatedTemp;
            EtsSesnor8 = simulatedTemp;
            EtsSesnor9 = simulatedTemp;
            EtsSesnor10 = simulatedTemp;
            EtsSesnor11 = simulatedTemp;
            EtsSesnor12 = simulatedTemp;

            List<double> sesnors = new List<double> { Math.Round(ecgChannel5And6Reading), simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp,
                                                      simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp,
                                                      simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp};
            ListOfSesnorsState.Clear();

            ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(sesnors, out eTSMinimumTemperature);

            MinimumTemperature = eTSMinimumTemperature;

            simulatedTemp++;
        }

        public void DeacreaeSesnorSimulation()
        {
            EtsSesnor1 = simulatedTemp;
            EtsSesnor2 = simulatedTemp;
            EtsSesnor3 = simulatedTemp;
            EtsSesnor4 = simulatedTemp;
            EtsSesnor5 = simulatedTemp;
            EtsSesnor6 = simulatedTemp;
            EtsSesnor7 = simulatedTemp;
            EtsSesnor8 = simulatedTemp;
            EtsSesnor9 = simulatedTemp;
            EtsSesnor10 = simulatedTemp;
            EtsSesnor11 = simulatedTemp;
            EtsSesnor12 = simulatedTemp;
            EtsSesnor13 = simulatedTemp;
            List<double> sesnors = new List<double> { Math.Round(ecgChannel5And6Reading), simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp,
                                                      simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp,
                                                      simulatedTemp, simulatedTemp, simulatedTemp, simulatedTemp};
            ListOfSesnorsState.Clear();

            ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(sesnors, out eTSMinimumTemperature);

            MinimumTemperature = eTSMinimumTemperature;
            simulatedTemp--;
        }
#endif

        private void SendSystemStateToRemoteControl(MessageStateId _systemState)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            switch ((int)_systemState)
            {
                case (int)MessageStateId.CAN_ID_STATE_IDLE:
                    data[0] = 1;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_READY:
                    data[0] = 2;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                    data[0] = 3;

                    break;
                case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                    data[0] = 4;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                    data[0] = 5;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_THAWING:
                    data[0] = 6;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                    data[0] = 7;
                    break;
            }

            Console.SendStateToRemoteCotrol(_systemState, data);
        }

        /// <summary>
        /// Gets the current CommonViewModel
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <id>SF-SDS-0102</id>
        public static CommonViewModel Current
        {
            get
            {
                return current;
            }
        }

        /// <summary>
        /// Gets or sets Window Loaded boolean flag value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsWindowLoaded
        {
            get { return isWindowLoaded; }
            set { isWindowLoaded = value; }
        }

        /// <summary>
        /// Gets or sets CryoTherapy Time value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
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
            }
        }


        /// <summary>
        /// Gets or sets Ablation Time value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int AblationTime
        {
            get
            {
                return ablationTime;
            }

            set
            {
                SetProperty(ref this.ablationTime, value);
            }
        }

        /// <summary>
        /// Gets or sets Was Ablatino Time Manually Changed value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool WasAblationTimeManuallyChanged
        {
            get
            {
                return wasAblationTimeManuallyChanged;
            }
            set
            {
                SetProperty(ref this.wasAblationTimeManuallyChanged, value);
            }
        }

        public int TemporaryManualAblationTime
        {
            get
            {
                return temporaryManualAblationTime;
            }
            set
            {
                SetProperty(ref this.temporaryManualAblationTime, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Expiration Date value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime CatheterExpirationDate
        {
            get
            {
                return catheterExpirationDate;
            }

            set
            {
                SetProperty(ref this.catheterExpirationDate, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Id value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterID
        {
            get
            {
                return catheterID;
            }

            set
            {
                SetProperty(ref this.catheterID, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Last Use Date value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime CatheterLastUseDate
        {
            get
            {
                return catheterLastUseDate;
            }

            set
            {
                SetProperty(ref this.catheterLastUseDate, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Firmware Version value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterFirmwareVersion
        {
            get
            {
                return catheterFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.catheterFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Serial Number value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterSerialNumber
        {
            get
            {
                return catheterSerialNumber;
            }
            set
            {
                SetProperty(ref this.catheterSerialNumber, value);
            }
        }

        /// <summary>
        /// Gets or sets the Central Micro Controller Firmware Version value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerFirmwareVersion
        {
            get
            {
                return centralMicroControllerFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.centralMicroControllerFirmwareVersion, value);

            }
        }

        /// <summary>
        /// Gets or sets the Continuous Thawing value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ContinuousThawing
        {
            get
            {
                return continuousThawing;
            }
            set
            {
                SetProperty(ref this.continuousThawing, value);
            }
        }

        /// <summary>
        /// Gets or sets the CP1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP1Reading
        {
            get
            {
                return cP1Reading;
            }

            set
            {
                cP1Reading = value;
                if (SensorReadingMananger.AreSensorsConnected)
                    RaisePropertyChanged(nameof(CP1Reading));
            }
        }

        /// <summary>
        /// Gets or sets the CP2 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP2Reading
        {
            get
            {
                return cP2Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.cP2Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the CPLD Error Register value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CPLDErrorRegister
        {
            get
            {
                return cPLDErrorRegister;
            }
            set
            {
                SetProperty(ref this.cPLDErrorRegister, value);
            }
        }

        /// <summary>
        /// Gets or sets the CPLD System Register value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CPLDSystemRegister
        {
            get
            {
                return cPLDSystemRegister;
            }
            set
            {
                SetProperty(ref this.cPLDSystemRegister, value);
            }
        }

        /// <summary>
        /// Gets or sets the CPLD Valve Register value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CPLDValveRegister
        {
            get
            {
                return cPLDValveRegister;
            }
            set
            {
                SetProperty(ref this.cPLDValveRegister, value);
            }
        }

        /// <summary>
        /// Gets or sets the D Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DGain
        {
            get
            {
                return dGain;
            }

            set
            {
                SetProperty(ref this.dGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the PMCU System Status Error Code value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Int64 PMCUSystemStatusErrorCode
        {
            get
            {
                return pMCUSystemStatusErrorCode;
            }

            set
            {
              SetProperty(ref this.pMCUSystemStatusErrorCode, value);
            }
        }

        /// <summary>
        /// Gets or sets the FM1 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1HighRange
        {
            get
            {
                return fM1HighRange;
            }

            set
            {
                SetProperty(ref this.fM1HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the FM1 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1LowRange
        {
            get
            {
                return fM1LowRange;
            }

            set
            {
                SetProperty(ref this.fM1LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the FM1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1Reading
        {
            get
            {
                return fM1Reading;
            }

            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.fM1Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the Home View User Control value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl HomeView
        {
            get
            {
                return homeView;
            }
            set
            {
                homeView = value;
            }
        }

        /// <summary>
        /// Gets or sets the I Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double IGain
        {
            get
            {
                return iGain;
            }

            set
            {
                SetProperty(ref this.iGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the LC1 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LC1HighRange
        {
            get
            {
                return lC1HighRange;
            }

            set
            {
                SetProperty(ref this.lC1HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the LC1 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LC1LowRange
        {
            get
            {
                return lC1LowRange;
            }

            set
            {
                SetProperty(ref this.lC1LowRange, value);
            }
        }

        private double lC1ReadingWithMetalPreviousValue = 1000;

        /// <summary>
        /// Gets or sets the LC1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LC1Reading
        {
            get
            {
                return lC1Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                {

                    if (lC1ReadingWithMetalPreviousValue != value)
                    {

                        //if (Console?.Tank != null)
                        //{
                        if (Console.Tank.MetalWeight == 0)
                        {
                            DataAccessLayer.Tank _tank = this.Data?.DataAccess?.GetCurrentTank();

                            if (_tank != null)
                            {
                                CurrentTank = _tank;
                                TankBuilder tankBuilder = new TankBuilder(CurrentTank, data);
                                Console.Tank.MetalWeight = tankBuilder.MetalWeight;
                            }
                        }

                        lC1Reading = value - this.Console.Tank.MetalWeight;
                        if (lC1Reading < 0)
                            lC1Reading = 0;
                        //}
                        /*else
                        {
                            lC1Reading = 30;
                        }*/

                        lC1ReadingWithMetalPreviousValue = value;

                        RaisePropertyChanged("LC1Reading");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Number of Injections value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int NumberOfInjections
        {
            get
            {
                return numberOfInjections;
            }

            set
            {
                SetProperty(ref this.numberOfInjections, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient D Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientDGain
        {
            get
            {
                return patientDGain;
            }

            set
            {
                SetProperty(ref this.patientDGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient I Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientIGain
        {
            get
            {
                return patientIGain;
            }

            set
            {
                SetProperty(ref this.patientIGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient Micro Controller Firmware Version value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerFirmwareVersion
        {
            get
            {
                return patientMicroControllerFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.patientMicroControllerFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient P Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPGain
        {
            get
            {
                return patientPGain;
            }

            set
            {
                SetProperty(ref this.patientPGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient PID Offset value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPIDOffset
        {
            get
            {
                return patientPIDOffset;
            }

            set
            {
                SetProperty(ref this.patientPIDOffset, value);
            }
        }

        /// <summary>
        /// Gets or sets the P Gain value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PGain
        {
            get
            {
                return pGain;
            }

            set
            {
                SetProperty(ref this.pGain, value);
            }
        }

        /// <summary>
        /// Gets or sets the PID Offset value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PIDOffset
        {
            get
            {
                return pIDOffset;
            }

            set
            {
                SetProperty(ref this.pIDOffset, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS1 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS1HighRange
        {
            get
            {
                return pS1HighRange;
            }

            set
            {
                SetProperty(ref this.pS1HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS1 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS1LowRange
        {
            get
            {
                return pS1LowRange;
            }

            set
            {
                SetProperty(ref this.pS1LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS1Reading
        {
            get
            {
                return pS1Reading;
            }

            set
            {
                SetProperty(ref this.pS1Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS2 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS2HighRange
        {
            get
            {
                return pS2HighRange;
            }

            set
            {
                SetProperty(ref this.pS2HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS2 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS2LowRange
        {
            get
            {
                return pS2LowRange;
            }

            set
            {
                SetProperty(ref this.pS2LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PS2 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS2Reading
        {
            get
            {
                return pS2Reading;
            }

            set
            {
                SetProperty(ref this.pS2Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT1 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT1HighRange
        {
            get
            {
                return pT1HighRange;
            }

            set
            {
                SetProperty(ref this.pT1HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT1 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT1LowRange
        {
            get
            {
                return pT1LowRange;
            }

            set
            {
                SetProperty(ref this.pT1LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT1Reading
        {
            get
            {
                return pT1Reading;
            }

            set
            {
                SetProperty(ref this.pT1Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT2 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2HighRange
        {
            get
            {
                return pT2HighRange;
            }

            set
            {
                SetProperty(ref this.pT2HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT2 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2LowRange
        {
            get
            {
                return pT2LowRange;
            }

            set
            {
                SetProperty(ref this.pT2LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT2 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2Reading
        {
            get
            {
                return pT2Reading;
            }

            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.pT2Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT3 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3HighRange
        {
            get
            {
                return pT3HighRange;
            }

            set
            {
                SetProperty(ref this.pT3HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT3 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3LowRange
        {
            get
            {
                return pT3LowRange;
            }

            set
            {
                SetProperty(ref this.pT3LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT3 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3Reading
        {
            get
            {
                return pT3Reading;
            }

            set
            {
                SetProperty(ref this.pT3Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT4 High Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT4HighRange
        {
            get
            {
                return pT4HighRange;
            }

            set
            {
                SetProperty(ref this.pT4HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT4 Low Range value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT4LowRange
        {
            get
            {
                return pT4LowRange;
            }

            set
            {
                SetProperty(ref this.pT4LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the PT4 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT4Reading
        {
            get
            {
                return pT4Reading;
            }

            set
            {
                SetProperty(ref this.pT4Reading, value);
            }
        }



        /// <summary>
        /// Gets or sets the System State value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId SystemState
        {
            get
            {
                return systemState;
            }

            set
            {
                SetProperty(ref this.systemState, value);
#if Simulator
                Console.IsConsoleInAblationState = (value == MessageStateId.CAN_ID_STATE_TRANSITION ||
                                    value == MessageStateId.CAN_ID_STATE_ABLATION) ? true : false;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the Simulated System State value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId SimulatedSystemState
        {
            get
            {
                return simulatedSystemState;
            }

            set
            {
                SetProperty(ref this.simulatedSystemState, value);
#if Simulator
                Console.IsConsoleInAblationState = (value == MessageStateId.CAN_ID_STATE_TRANSITION ||
                                    value == MessageStateId.CAN_ID_STATE_ABLATION) ? true : false;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the CMCU System Status Error value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Int64 CMCUSystemStatusError
        {
            get
            {
                return cMCUSystemStatusError;
            }
            set
            {
              SetProperty(ref this.cMCUSystemStatusError, value);
            }
        }

        /// <summary>
        /// Gets or sets the Target Balloon Pressure value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetBalloonPressure
        {
            get
            {
                return targetBalloonPressure;
            }
            set
            {
                SetProperty(ref this.targetBalloonPressure, value);
            }
        }

        /// <summary>
        /// Gets or sets the Target Injection Flow value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionFlow
        {
            get
            {
                return targetInjectionFlow;
            }
            set
            {
                SetProperty(ref this.targetInjectionFlow, value);
            }
        }

        /// <summary>
        /// Gets or sets the Target Injection Pressure value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionPressure
        {
            get
            {
                return targetInjectionPressure;
            }
            set
            {
                SetProperty(ref this.targetInjectionPressure, value);
            }
        }

        /// <summary>
        /// Gets or sets the TC1 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC1Reading
        {
            get
            {
                return tC1Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                {
                    tC1Reading = value;
                    RaisePropertyChanged("TC1Reading");
                }
                CatheterTemperature = value;
                // Also updates the value to send to the LSPro
                tC1LSProReading = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CatheterTemperature
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TC2 Reading value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC2Reading
        {
            get
            {
                return tC2Reading;
            }
            set
            {
                SetProperty(ref this.tC2Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold FM1 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdFM1High
        {
            get
            {
                return thresholdFM1High;
            }
            set
            {
                SetProperty(ref this.thresholdFM1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold FM1 Low value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdFM1Low
        {
            get
            {
                return thresholdFM1Low;
            }
            set
            {
                SetProperty(ref this.thresholdFM1Low, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For CP1 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForCP1High
        {
            get
            {
                return thresholdForCP1High;
            }
            set
            {
                SetProperty(ref this.thresholdForCP1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For Outer Balloon Pressure value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForOuterBallonPressure
        {
            get
            {
                return thresholdForOuterBallonPressure;
            }
            set
            {
                SetProperty(ref this.thresholdForOuterBallonPressure, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For CTC 1 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForCTC1High
        {
            get
            {
                return thresholdForCTC1High;
            }

            set
            {
                SetProperty(ref this.thresholdForCTC1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For CTC2 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForCTC2High
        {
            get
            {
                return thresholdForCTC2High;
            }

            set
            {
                SetProperty(ref this.thresholdForCTC2High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold for PT1 Fail value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForPT1Fail
        {
            get
            {
                return thresholdForPT1Fail;
            }
            set
            {
                SetProperty(ref this.thresholdForPT1Fail, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For PT1 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForPT1High
        {
            get
            {
                return thresholdForPT1High;
            }

            set
            {
                SetProperty(ref this.thresholdForPT1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold for PT1 Low value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForPT1Low
        {
            get
            {
                return thresholdForPT1Low;
            }

            set
            {
                SetProperty(ref this.thresholdForPT1Low, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold LC1 Fail value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdLC1Fail
        {
            get
            {
                return thresholdLC1Fail;
            }

            set
            {
                SetProperty(ref this.thresholdLC1Fail, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold LC1 Warning value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdLC1Warning
        {
            get
            {
                return thresholdLC1Warning;
            }

            set
            {
                SetProperty(ref this.thresholdLC1Warning, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold PS1 High value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdPS1High
        {
            get
            {
                return thresholdPS1High;
            }

            set
            {
                SetProperty(ref this.thresholdPS1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold PS2 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdPS2High
        {
            get
            {
                return thresholdPS2High;
            }

            set
            {
                SetProperty(ref this.thresholdPS2High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold PT2 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdPT2High
        {
            get
            {
                return thresholdPT2High;
            }

            set
            {
                SetProperty(ref this.thresholdPT2High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold PT3 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdPT3High
        {
            get
            {
                return thresholdPT3High;
            }
            set
            {
                SetProperty(ref this.thresholdPT3High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold PT4 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdPT4high
        {
            get
            {
                return thresholdPT4high;
            }
            set
            {
                SetProperty(ref this.thresholdPT4high, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold TS1 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdTS1High
        {
            get
            {
                return thresholdTS1High;
            }

            set
            {
                SetProperty(ref this.thresholdTS1High, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold TS1 High Range value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TS1HighRange
        {
            get
            {
                return tS1HighRange;
            }

            set
            {
                SetProperty(ref this.tS1HighRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the Threshold TS1 Low Range value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TS1LowRange
        {
            get
            {
                return tS1LowRange;
            }

            set
            {
                SetProperty(ref this.tS1LowRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the TS1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TS1Reading
        {
            get
            {
                return tS1Reading;
            }

            set
            {
                SetProperty(ref this.tS1Reading, value);
            }
        }

        private AblationSiteEnum _ablationSite = AblationSiteEnum.OTHER;
        /// <summary>
        /// Gets or sets the Ablation Site value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSiteEnum AblationSite
        {
            get => this._ablationSite;
            set
            {
                if (this.AblationSummary != null)
                {
                    this.AblationSummary.CurrentAblationSite = value;
                    AblationSiteCarousselModel.CurrentAblationSite = value;
                }

                SetProperty(ref this._ablationSite, value); 
            }
        }

        /// <summary>
        /// Gets the Console (Machine object)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Machine Console
        {
            get
            {
                return console;
            }
        }

        /// <summary>
        /// Gets the Data (Data object)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal Data Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Gets or sets the Current Patient value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DataAccessLayer.Patient CurrentPatient
        {
            get
            {
                return currentPatient;
            }
            set
            {
                SetProperty(ref this.currentPatient, value);
            }
        }

        /// <summary>
        /// Gets or sets the Current Ablation value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Ablation CurrentAblation
        {
            get
            {
                return currentAblation;
            }
            set
            {
                SetProperty(ref this.currentAblation, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Ablation is Started or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAblationProcedureStarted
        {
            get
            {
                return isAblationProcedureStarted;
            }
            set
            {
                SetProperty(ref this.isAblationProcedureStarted, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Ablation has Ended or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAblationProcedureEnded
        {
            get
            {
                return isAblationProcedureEnded;
            }
            set
            {
                SetProperty(ref this.isAblationProcedureEnded, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Therapy can Start or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanStartTherapy
        {
            get
            {
                return canStartTherapy;
            }
            set
            {
                canStartTherapy = value;
                RaisePropertyChanged("CanStartTherapy");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Procedure can End or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanEndProcedure
        {
            get
            {
                return canEndProcedure;
            }
            set
            {
                canEndProcedure = value;
                RaisePropertyChanged("canEndProcedure");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Catheter is valid or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterValid
        {
            get
            {
                return isCatheterValid;
            }
            set
            {
                isCatheterValid = value;
                RaisePropertyChanged("IsCatheterValid");
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 1 and 2 Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> EcgChannel1And2Readings
        {
            get
            {
                { lock (_myVar_Lock) return ecgChannel1And2Readings; }
            }

            set
            {
                { lock (_myVar_Lock) ecgChannel1And2Readings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 3 and 4 Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> EcgChannel3And4Readings
        {
            get
            {
                { lock (_myVar_Lock) return ecgChannel3And4Readings; }
            }

            set
            {
                { lock (_myVar_Lock) ecgChannel3And4Readings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 5 and 6 Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> EcgChannel5And6Readings
        {
            get
            {
                { lock (_myVar_Lock) return ecgChannel5And6Readings; }
            }

            set
            {
                { lock (_myVar_Lock) ecgChannel5And6Readings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 7 and 8 Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> EcgChannel7And8Readings
        {
            get
            {
                { lock (_myVar_Lock) return ecgChannel7And8Readings; }
            }

            set
            {
                { lock (_myVar_Lock) ecgChannel7And8Readings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 9 and 10 Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> EcgChannel9And10Readings
        {
            get
            {
                { lock (_myVar_Lock) return ecgChannel9And10Readings; }
            }

            set
            {
                { lock (_myVar_Lock) ecgChannel9And10Readings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the Channel Tip Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> ChannelTipReadings
        {
            get
            {
                { lock (_myVar_Lock) return channelTipReadings; }
            }

            set
            {
                { lock (_myVar_Lock) channelTipReadings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the Channel Accelerometer Readings List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<double> ChannelAccelerometerReadings
        {
            get
            {
                { lock (_myVar_Lock) return channelAccelerometerReadings; }
            }

            set
            {
                { lock (_myVar_Lock) channelAccelerometerReadings = value; }
            }
        }

        /// <summary>
        /// Gets or sets the List of Ablation Data Details List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<List<AblationDataDetails>> AllAblationDataList
        {
            get
            {
                return allAblationDataList;
            }
            set
            {
                allAblationDataList = value;
                RaisePropertyChanged("AllAblationDataList");
            }
        }
        /// <summary>
        /// Updates ablation site
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void UpdateAblationSite(int treatmentNumber, AblationSiteEnum ablationSite)
        {
            if (AllAblationDataList?.Count >= treatmentNumber)
            {
                // //AppTrace.Log($"Start to update Ablation Site in Current System State {Current.SystemState} and Previous System State {Current.PreviousSystemState}.",
                //      LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel), nameof(UpdateAblationSite));
                if (treatmentNumber == AllAblationDataList?.Count)
                  AblationSite = ablationSite;//sets the Ablation summary -> current ablation site

                //Update all ablation data details with the new ablation site
                foreach (AblationDataDetails ablationData in AllAblationDataList[treatmentNumber - 1])
                {
                    if (ablationData != null)
                    {
                        ablationData.AblationSite = (int)ablationSite;
                    }
                }

                //Save the ablation file (overwrite)
                UpdateAblationData(AllAblationDataList[treatmentNumber - 1], treatmentNumber);

                // //AppTrace.Log($"Finish Updating Ablation Site in Current System State {Current.SystemState} and Previous System State {Current.PreviousSystemState}.",
                //     LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel), nameof(UpdateAblationSite));
            }
        }
        /// <summary>
        /// Updates ablation Data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void UpdateAblationData(List<AblationDataDetails> ablationDetails, int treatmentNumber)
        {
            JsonManager jsonFileManager = new JsonManager();
            int ablationCounter = 1;
            string filePath = string.Empty;

            List<Ablation> ablationList = CommonViewModel.Current?.Data?.DataAccess?.GetAllAblationByProcedureId(CurrentProcedure.Id);

            //Find the ablation file path
            foreach (Ablation ablation in ablationList)
            {
                if (ablationCounter == treatmentNumber)
                {
                    filePath = ablation.DataFile;
                    break;
                }

                ablationCounter++;
            }

            var ablationFileStruct = AblationFileDataStruct.ConvertAblationDataDetailsToFileStruct(ablationDetails);
            jsonFileManager.SerializeAndUpdateExistingFile(ablationFileStruct, filePath);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Diaphragm Movement Monitoring can Start or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanstartDiaphragmMovementMonitoring
        {
            get
            {
                return canstartDiaphragmMovementMonitoring;
            }

            set
            {
                canstartDiaphragmMovementMonitoring = value;
                RaisePropertyChanged("CanstartDiaphragmMovementMonitoring");
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 1 and 2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel1And2Reading
        {
            get
            {
                return ecgChannel1And2Reading;
            }

            set
            {
                RawEcgChannel1And2Reading = value;
                double calculatedEcgChannel1And2Reading = 0;
                if (value + OcclusionPressureTareValue >= 0)
                    calculatedEcgChannel1And2Reading = value + OcclusionPressureTareValue;
                else
                    calculatedEcgChannel1And2Reading = 0;
                ecgChannel1And2Reading = calculatedEcgChannel1And2Reading;
                RaisePropertyChanged("EcgChannel1And2Reading");
            }

        }

        /// <summary>
        /// Gets or sets the Raw ECG Channel 1 and 2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RawEcgChannel1And2Reading
        {
            get
            {
                return rawEcgChannel1And2Reading;
            }

            set
            {
                rawEcgChannel1And2Reading = value;
                RaisePropertyChanged("RawEcgChannel1And2Reading");
            }

        }

        /// <summary>
        /// Gets or sets the ECG Channel 3 and 4 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel3And4Reading
        {
            get
            {
                return ecgChannel3And4Reading;
            }

            set
            {
                ecgChannel3And4Reading = value;
                MaxEcgChannel3And4Reading = Math.Max(value, MaxEcgChannel3And4Reading);
                RaisePropertyChanged(nameof(EcgChannel3And4Reading));
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 5 and 6 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel5And6Reading
        {
            get
            {
                if (IsMultiEtsSesnorConnected)
                    return MinimumTemperature;
                return ecgChannel5And6Reading;
            }
            set
            {
                ecgChannel5And6Reading = value;
                RaisePropertyChanged("EcgChannel5And6Reading");

#if Simulator
                if (IsMultiEtsSesnorConnected && !AreSensorsInPlayBackMode)
                    AnalyseEsophagusTemperature();
#endif

            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 7 and 8 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel7And8Reading
        {
            get
            {
                return ecgChannel7And8Reading;
            }
            set
            {

                ecgChannel7And8Reading = value;
                RaisePropertyChanged("EcgChannel7And8Reading");

                if (!IgnoreMinimumDiaphragmMovementValue)
                    IsDiaphragmMovementDetected = this.DMSLogic?.GetDMSState(ecgChannel7And8Reading, systemState) ?? false;
                else
                    IsDiaphragmMovementDetected = true;


            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 9 and 10 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgChannel9And10Reading
        {
            get
            {
                return ecgChannel9And10Reading;
            }
            set
            {
                ecgChannel9And10Reading = value;
            }
        }

        /// <summary>
        /// Gets or sets the ECG Channel 9 and 10 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double[] HighResolutionDmsReading
        {
          get => _highResolutionDmsReading;
          set => SetProperty(ref _highResolutionDmsReading, value);
        }

        /// <summary>
        /// Gets or sets the Channel Tip value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ChannelTipReading
        {
            get
            {
                return channelTipReading;
            }

            set
            {
                channelTipReading = value;
            }
        }

        /// <summary>
        /// Gets or sets the Channel Accelerometer Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ChannelAccelerometerReading
        {
            get
            {
                return channelAccelerometerReading;
            }

            set
            {
                channelAccelerometerReading = value;
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Expiration Day value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationDay
        {
            get
            {
                return catheterExpirationDay;
            }

            set
            {
                SetProperty(ref this.catheterExpirationDay, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Expiration Month value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationMonth
        {
            get
            {
                return catheterExpirationMonth;
            }

            set
            {
                SetProperty(ref this.catheterExpirationMonth, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Expiration Year value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationYear
        {
            get
            {
                return catheterExpirationYear;
            }

            set
            {
                SetProperty(ref this.catheterExpirationYear, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Last Use Day value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseDay
        {
            get
            {
                return catheterLastUseDay;
            }

            set
            {
                SetProperty(ref this.catheterLastUseDay, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Last Use Month value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseMonth
        {
            get
            {
                return catheterLastUseMonth;
            }

            set
            {
                SetProperty(ref this.catheterLastUseMonth, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Last Use Year value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseYear
        {
            get
            {
                return catheterLastUseYear;
            }

            set
            {
                SetProperty(ref this.catheterLastUseYear, value);
            }
        }


        private bool IsCatheterCableConnectedLastvalue = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Catheter Cable is Connected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterCableConnected
        {
            get
            {

                return isCatheterCableConnected;
            }
            set
            {

                if (isCatheterCableConnected != value)
                {
                    CanTwoStopWatchCommunicationLost.Reset();

                    if (value)
                        canTwoMaximumTimeOut = iCBTimeOutAtInitialization;
                    else
                        canTwoMaximumTimeOut = canTwoMaximumTimeOutReference;


                    isCatheterCableConnected = value;
                    SensorReadingMananger.IsCatheterCableConnected = value;
                    RaisePropertyChanged("IsCatheterCableConnected");

                    if (!isCatheterCableConnected)
                    {
                        Console.Disconnect();
                        IsVacuumDisconnected = true;
                        ResetCatheterInformation();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Catheter Tube is Connected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterTubeConnected
        {
            get
            {
                return isCatheterTubeConnected;
            }

            set
            {
                SetProperty(ref this.isCatheterTubeConnected, value);
                RaisePropertyChanged("IsCatheterTubeConnected");
                RaisePropertyChanged("IsCatheterConnected");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the User is in the Procedure Screen or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsTheUserInProcedureScreen
        {
            get
            {
                return isTheUserInProcedureScreen;
            }

            set
            {
                SetProperty(ref this.isTheUserInProcedureScreen, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU Exception is Type 1 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUExceptionType1
        {
            get
            {
                return isPMCUExceptionType1;
            }

            set
            {
                isPMCUExceptionType1 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU Exception is Type 2 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUExceptionType2
        {
            get
            {
                return isPMCUExceptionType2;
            }

            set
            {
                isPMCUExceptionType2 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU Exception is Type 3 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUExceptionType3
        {
            get
            {
                return isPMCUExceptionType3;
            }

            set
            {
                isPMCUExceptionType3 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU Exception is Type 4 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUExceptionType4
        {
            get
            {
                return ispMCUExceptionType4;
            }

            set
            {
                ispMCUExceptionType4 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU Exception is Type 5 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUExceptionType5
        {
            get
            {
                return isPMCUExceptionType5;
            }

            set
            {
                isPMCUExceptionType5 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU CPLD Watchdog timer is in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUCPLDWatchDogTimerError
        {
            get
            {
                return isPMCUCPLDWatchDogTimerError;
            }

            set
            {
                isPMCUCPLDWatchDogTimerError = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Inner Balloon Pressure is Too High or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsInnerBalloonPressureTooHigh
        {
            get
            {
                return isInnerBalloonPressureTooHigh;
            }

            set
            {
                isInnerBalloonPressureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Inner Balloon Pressure is Too Low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsInnerBalloonPressureTooLow
        {
            get
            {
                return isInnerBalloonPressureTooLow;
            }
            set
            {
                isInnerBalloonPressureTooLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Inner Balloon Pressure Reading is Out of Range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsInnerBalloonPressureReadingOutOfRange
        {
            get
            {
                return isInnerBalloonPressureReadingOutOfRange;
            }
            set
            {
                isInnerBalloonPressureReadingOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Outer Balloon Pressure is Too High or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsOuterBalloonPressureTooHigh
        {
            get
            {
                return isOuterBalloonPressureTooHigh;
            }
            set
            {
                isOuterBalloonPressureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Outer Balloon Pressure is too low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsOuterBalloonPressureTooLow
        {
            get
            {
                return isOuterBalloonPressureTooLow;
            }
            set
            {
                isOuterBalloonPressureTooLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Outer Balloon Pressure Reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsOuterBalloonPressureReadingOutOrRange
        {
            get
            {
                return isOuterBalloonPressureReadingOutOrRange;
            }
            set
            {
                isOuterBalloonPressureReadingOutOrRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Balloon Tip Pressure is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBalloonTipPressureTooHigh
        {
            get
            {
                return isBalloonTipPressureTooHigh;
            }
            set
            {
                isBalloonTipPressureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Balloon Tip Pressure is too low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBalloonTipPressureTooLow
        {
            get
            {
                return isBalloonTipPressureTooLow;
            }
            set
            {
                isBalloonTipPressureTooLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Balloon Tip Pressure Reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBalloonTipPressurePeadingOutOfRange
        {
            get
            {
                return isBalloonTipPressurePeadingOutOfRange;
            }
            set
            {
                isBalloonTipPressurePeadingOutOfRange = value;
            }
        }

        private volatile bool _isBalloonTemperatureLowWarning;
        public bool IsBalloonTemperatureLowWarning => _isBalloonTemperatureLowWarning; 

        /// <summary>
        /// Gets or sets a value indicating whether the Thawing Temperature is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsThawingTemperatureTooHigh
        {
            get
            {
                return isThawingTemperatureTooHigh;
            }
            set
            {
                isThawingTemperatureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Thawing Temperature is too low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsThawingTemperatureTooLow
        {
            get
            {
                return isThawingTemperatureTooLow;
            }
            set
            {
                isThawingTemperatureTooLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Exception is Type 1 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUExceptionType1
        {
            get
            {
                return isCMCUExceptionType1;
            }
            set
            {
                isCMCUExceptionType1 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Exception is Type 2 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUExceptionType2
        {
            get
            {
                return isCMCUExceptionType2;
            }
            set
            {
                isCMCUExceptionType2 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Exception is Type 3 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUExceptionType3
        {
            get
            {
                return isCMCUExceptionType3;
            }
            set
            {
                isCMCUExceptionType3 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Exception is Type 4 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUExceptionType4
        {
            get
            {
                return isCMCUExceptionType4;
            }
            set
            {
                isCMCUExceptionType4 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Exception is Type 5 or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUExceptionType5
        {
            get
            {
                return isCMCUExceptionType5;
            }
            set
            {
                isCMCUExceptionType5 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU CPLD Watchdog timer is in Error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUCPLDWatchDogTimerError
        {
            get
            {
                return isCMCUCPLDWatchDogTimerError;
            }
            set
            {
                isCMCUCPLDWatchDogTimerError = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Two Multiplex Reading matches or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUTwoMultiplexReadingDoesNotMatch
        {
            get
            {
                return isCMCUTwoMultiplexReadingDoesNotMatch;
            }
            set
            {
                isCMCUTwoMultiplexReadingDoesNotMatch = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Flow is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUFlowTooHigh
        {
            get
            {
                return isCMCUFlowTooHigh;
            }
            set
            {
                isCMCUFlowTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Flow is too low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUFlowTooLow
        {
            get
            {
                return isCMCUFlowTooLow;
            }
            set
            {
                isCMCUFlowTooLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU flow reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUFlowReadingOutOfRange
        {
            get
            {
                return isCMCUFlowReadingOutOfRange;
            }
            set
            {
                isCMCUFlowReadingOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Load cell reached the warning weight or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCULoadCellWeightWarning
        {
            get
            {
                return isCMCULoadCellWeightWarning;
            }
            set
            {
                isCMCULoadCellWeightWarning = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Load cell weight failed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCULoadCellWeightFail
        {
            get
            {
                return isCMCULoadCellWeightFail;
            }
            set
            {
                isCMCULoadCellWeightFail = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Load cell reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCULoadCellReadingOutOfRange
        {
            get
            {
                return isCMCULoadCellReadingOutOfRange;
            }
            set
            {
                isCMCULoadCellReadingOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Pressure in tank is hight (Fan to be on) or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPressureInTankIsHighFanToBeOn
        {
            get
            {
                return isCMCUPressureInTankIsHighFanToBeOn;
            }
            set
            {
                isCMCUPressureInTankIsHighFanToBeOn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU pressure PT1 in Tank is low or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPressurePT1InTankIsLow
        {
            get
            {
                return isCMCUPressurePT1InTankIsLow;
            }
            set
            {
                isCMCUPressurePT1InTankIsLow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Pressure PT1 in tank is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPressurePT1InTankIsTooHigh
        {
            get
            {
                return isCMCUPressurePT1InTankIsTooHigh;
            }
            set
            {
                isCMCUPressurePT1InTankIsTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU pressure PT1 in tank reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPressurePT1InTankReadingOutOfRange
        {
            get
            {
                return isCMCUPressurePT1InTankReadingOutOfRange;
            }
            set
            {
                isCMCUPressurePT1InTankReadingOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Pressure PT2 (after Catheter
        /// but before return line) is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh
        {
            get
            {
                return isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh;
            }
            set
            {
                isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU PT2 Reading is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUPT2ReadingOutOfRange
        {
            get
            {
                return isCMCUPT2ReadingOutOfRange;
            }
            set
            {
                isCMCUPT2ReadingOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU return pressure PT3 is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUReturnPressurePT3TooHigh
        {
            get
            {
                return isCMCUReturnPressurePT3TooHigh;
            }
            set
            {
                isCMCUReturnPressurePT3TooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU return pressure PT3 is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUReturnPressurePT3OutOfRange
        {
            get
            {
                return isCMCUReturnPressurePT3OutOfRange;
            }
            set
            {
                isCMCUReturnPressurePT3OutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Vacuum pressure PT4 is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUVacuumPressurePT4TooHigh
        {
            get
            {
                return isCMCUVacuumPressurePT4TooHigh;
            }
            set
            {
                isCMCUVacuumPressurePT4TooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Vacuum pressure PT4 is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUVacuumPressurePT4OutOfRange
        {
            get
            {
                return isCMCUVacuumPressurePT4OutOfRange;
            }
            set
            {
                isCMCUVacuumPressurePT4OutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU Sub cooler temperature is high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUSubCoolerTemperatureIsHigh
        {
            get
            {
                return isCMCUSubCoolerTemperatureIsHigh;
            }
            set
            {
                isCMCUSubCoolerTemperatureIsHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU sub cooler temperature is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUSubCoolerTemperatureOutOfRange
        {
            get
            {
                return isCMCUSubCoolerTemperatureOutOfRange;
            }
            set
            {
                isCMCUSubCoolerTemperatureOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU injection vent pressure is high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUInjectionVentPressureIsHigh
        {
            get
            {
                return isCMCUInjectionVentPressureIsHigh;
            }
            set
            {
                isCMCUInjectionVentPressureIsHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU injection vent pressure is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUInjectionVertPressureOutOfRange
        {
            get
            {
                return isCMCUInjectionVertPressureOutOfRange;
            }
            set
            {
                isCMCUInjectionVertPressureOutOfRange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU scavenging pressure is high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUScavengingPressureIsHigh
        {
            get
            {
                return isCMCUScavengingPressureIsHigh;
            }
            set
            {
                isCMCUScavengingPressureIsHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU scavenging pressure is out of range or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFootSwitchLocked
        {
            get
            {
                return isFootSwitchLocked;
            }
            set
            {

                if (value != isFootSwitchLocked)
                {
                    isFootSwitchLocked = value;
                    RaisePropertyChanged("IsFootSwitchLocked");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Gas State value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Enumeration.TankWeight GasState
        {
            get
            {
                return gasState;
            }
            set
            {
                if (value != gasState)
                {
                    gasState = value;
                    RaisePropertyChanged("GasState");
                }
            }
        }

        /// <summary>
        /// Gets or sets the PT5 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT5Reading
        {
            get
            {
                return pT5Reading;
            }
            set
            {
                SetProperty(ref this.pT5Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the CMCU J Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CMCUCJReading
        {
            get
            {
                return cMCUCJReading;
            }
            set
            {
                SetProperty(ref this.cMCUCJReading, value);
            }
        }

        /// <summary>
        /// Gets or sets the PMCU CJ Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PMCUCJReading
        {
            get
            {
                return pMCUCJReading;
            }
            set
            {
                SetProperty(ref this.pMCUCJReading, value);
            }
        }

        /// <summary>
        /// Returns whether the Catheter is connected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnected
        {
            get
            {
                return (IsCatheterCableConnected && IsCatheterTubeConnected);
            }
        }

        /// <summary>
        /// Gets or sets the TIP Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TIPReading
        {
            get
            {
                return tIPReading;
            }
            set
            {
                SetProperty(ref this.tIPReading, value);
            }
        }

        /// <summary>
        /// Gets or sets the TN2O reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TN2OReading
        {
            get
            {
                return tN2OReading;
            }
            set
            {
                SetProperty(ref this.tN2OReading, value);
            }
        }

        /// <summary>
        /// Gets or sets the List of Patient Micro controller register IDS Dynamic table's integer values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<int> PatientMicroControllerRegisterIDSDynamicTable
        {
            get
            {
                { lock (_myRegister_Lock) return patientMicroControllerRegisterIDSDynamicTable; }
            }
            set
            {
                { lock (_myRegister_Lock) patientMicroControllerRegisterIDSDynamicTable = value; }
            }
        }

        /// <summary>
        /// Gets or sets the List of Central Micro controller register IDS Dynamic table's integer values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<int> CentralMicroControllerRegisterIDSDynamicTable
        {
            get
            {
                { lock (_myRegister_Lock) return centralMicroControllerRegisterIDSDynamicTable; }
            }
            set
            {
                { lock (_myRegister_Lock) centralMicroControllerRegisterIDSDynamicTable = value; }
            }
        }

        /// <summary>
        /// Gets or sets the Dictionary of Patient Micro controller Ack Register Table values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Dictionary<int, bool> PatientMicroControllerAckRegistersTable
        {
            get
            {
                { lock (_myRegister_Lock) return patientMicroControllerackRegistersTable; }
            }
            set
            {
                { lock (_myRegister_Lock) patientMicroControllerackRegistersTable = value; }
            }
        }

        /// <summary>
        /// Gets or sets the Dictionary of Central Micro controller Ack Register Table values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Dictionary<int, bool> CentralMicroControllerAckRegistersTable
        {
            get
            {
                { lock (_myRegister_Lock) return centralMicroControllerAckRegistersTable; }
            }
            set
            {
                { lock (_myRegister_Lock) centralMicroControllerAckRegistersTable = value; }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Reading from micro controller for register validation or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsReadingFromMicroControllerForRegisterValidation
        {
            get
            {
                return isReadingFromMicroControllerForRegisterValidation;
            }
            set
            {
                isReadingFromMicroControllerForRegisterValidation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CMCU is ready or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUReady
        {
            get
            {
                return isCMCUReady;
            }
            set
            {
                if (value != isCMCUReady)
                {
                    isCMCUReady = value;
                    RaisePropertyChanged("IsCMCUReady");
                    StartAckProcess(value & IsPMCUReady);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PMCU is ready or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUReady
        {
            get
            {
                return isPMCUReady;
            }
            set
            {
                if (value != isPMCUReady)
                {
                    isPMCUReady = value;
                    RaisePropertyChanged("IsPMCUReady");
                    StartAckProcess(value & IsCMCUReady);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the catheter is in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterInError
        {
            get
            {
                return isCatheterInError;
            }
            set
            {
                isCatheterInError = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter lot value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLot
        {
            get
            {
                return catheterLot;
            }
            set
            {
                SetProperty(ref this.catheterLot, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warning is visible or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsWarningVisible
        {
            get
            {
                if (CommonViewModel.current.warningMessagesManager != null &&
                    CommonViewModel.current.warningMessagesManager.WarningMessagesList != null)
                {
                    return CommonViewModel.current.warningMessagesManager.WarningMessagesList.Count > 0;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetProperty(ref this.isWarningVisible, value);
            }
        }

        /// <summary>
        /// Gets or sets a the Warning Message Manager value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public WarningMessagesManager.WarningMessagesManager WarningMessageManager
        {
            get
            {
                return this.warningMessagesManager;
            }
            set
            {
                this.warningMessagesManager = value;
                RaisePropertyChanged("WarningMessageManager");
            }
        }

        /// <summary>
        /// Gets or sets the Can One Stopwatch communication lost value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Stopwatch CanOneStopWatchCommunicationLost
        {
            get
            {
                return canOneStopWatchCommunicationLost;
            }
            set
            {
                canOneStopWatchCommunicationLost = value;
            }
        }

        /// <summary>
        /// Gets or sets the Can Two Stopwatch communication lost value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Stopwatch CanTwoStopWatchCommunicationLost
        {
            get
            {
                return canTwoStopWatchCommunicationLost;
            }
            set
            {
                canTwoStopWatchCommunicationLost = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Can Tow is in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCanTwoInError
        {
            get
            {
                return isCanTwoInError;
            }
            set
            {
                isCanTwoInError = value;
            }
        }

        /// <summary>
        /// Gets or sets the Current Procedure value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Procedure CurrentProcedure
        {
            get
            {
                return currentProcedure;
            }
            set
            {
                SetProperty(ref this.currentProcedure, value);
            }
        }

        /// <summary>
        /// Gets or sets the Max ECG Channel 1 and 2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaxEcgChannel1And2Reading
        {
            get
            {
                return maxEcgChannel1And2Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.maxEcgChannel1And2Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the Max ECG Channel 3 and 4 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaxEcgChannel3And4Reading
        {
            get
            {
                return maxEcgChannel3And4Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.maxEcgChannel3And4Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets the Max ECG Channel 5 and 6 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaxEcgChannel5And6Reading
        {
            get
            {
                return maxEcgChannel5And6Reading;
            }
            set
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    SetProperty(ref this.maxEcgChannel5And6Reading, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Catheter last use date is updated or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterLastUseDateUpdated
        {
            get
            {
                return isCatheterLastUseDateUpdated;
            }
            set
            {
                isCatheterLastUseDateUpdated = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the Catheter expiration date was updated or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterExpirationDateUpdated
        {
            get
            {
                return isCatheterExpirationDateUpdated;
            }
            set
            {
                isCatheterExpirationDateUpdated = value;
            }
        }


        /// <summary>
        /// Gets or sets the Catheter Last use hour value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseHour
        {
            get
            {
                return catheterLastUseHour;
            }
            set
            {
                catheterLastUseHour = value;
            }
        }

        /// <summary>
        /// Gets or sets the PID Duty cycle value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PIDDutyCycle
        {
            get
            {
                return pIDDutyCycle;
            }
            set
            {
                SetProperty(ref this.pIDDutyCycle, value);
            }
        }

        /// <summary>
        /// Gets or sets the Patient PID Duty Cycle value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PatientPIDDutyCycle
        {
            get
            {
                return patientPIDDutyCycle;
            }
            set
            {
                SetProperty(ref this.patientPIDDutyCycle, value);
            }
        }

        /// <summary>
        /// Gets or sets the Catheter Stop Watch Disconnection (Stopwatch) value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Stopwatch CatheterStopWatchDisconnection
        {
            get
            {
                return catheterStopWatchDisconnection;
            }

            set
            {
                catheterStopWatchDisconnection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Tip Pressure Diaphragm Movement Esophagus Temperature Time integer value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TipPressureDiaphragmMovementEsophagusTemperatureTime
        {
            get
            {
                return tipPressureDiaphragmMovementEsophagusTemperatureTime;
            }
            set
            {
                tipPressureDiaphragmMovementEsophagusTemperatureTime = value;
                RaisePropertyChanged("TipPressureDiaphragmMovementEsophagusTemperatureTime");
            }
        }

        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets or sets the Ablation Summary value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSummary AblationSummary
        {
            get
            {
                return ablationSummary;
            }
            set
            {
                try
                {
                    _rwLock.EnterWriteLock();
                    SetProperty(ref ablationSummary, value);
                    RaisePropertyChanged("AblationSymmary");
                }
                catch (Exception ex)
                {
	                LogException(ex);
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Sensors are in playback mode or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool AreSensorsInPlayBackMode
        {
            get
            {
                return areSensorsInPlayBackMode;
            }
            set
            {
                try
                {
                    areSensorsInPlayBackMode = value;
                    RaisePropertyChanged("AreSensorsInPlayBackMode");
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Balloon Temperature is too high or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBalloonTemperatureTooHigh
        {
            get
            {
                return isBalloonTemperatureTooHigh;
            }
            set
            {
                isBalloonTemperatureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets the Login Manager value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public LoginManager LoginManager
        {
            get
            {
                return loginManager;
            }
            set
            {
                loginManager = value;
                RaisePropertyChanged("Login");
            }
        }

        /// <summary>
        /// Returns the Current User value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return LoginManager.CurrentUser;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Current User is Cryterion type or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCryterionUser
        {
            get
            {
                if (CurrentUser != null && CurrentUser.Types != null && CurrentUser.Types.Count > 0)
                {
                    foreach (DataAccessLayer.Type userType in CurrentUser.Types)
                    {
                        if (userType.Id == (int)LoginManager.AccessControlType.CRYTERION)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Current User is BSC Admin user
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBSCADMINUser
        {
            get
            {
                if (CurrentUser != null && CurrentUser.Types != null && CurrentUser.Types.Count > 0)
                {
                    foreach (DataAccessLayer.Type userType in CurrentUser.Types)
                    {
                        if (userType.Id == (int)LoginManager.AccessControlType.BSCADMIN)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Current User is Admin type or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAdminUser
        {
            get
            {
                if (CurrentUser != null && CurrentUser.Types != null && CurrentUser.Types.Count > 0)
                {
                    foreach (DataAccessLayer.Type userType in CurrentUser.Types)
                    {
                        if (userType.Id == (int)LoginManager.AccessControlType.ADMIN)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the User is User type or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUser
        {
            get
            {
                if (CurrentUser != null && CurrentUser.Types != null && CurrentUser.Types.Count > 0)
                {
                    foreach (DataAccessLayer.Type userType in CurrentUser.Types)
                    {
                        if (userType.Id == (int)LoginManager.AccessControlType.USER)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the User is Doctor type or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsDoctor
        {
            get
            {
                if (CurrentUser != null && CurrentUser.Types != null && CurrentUser.Types.Count > 0)
                {
                    foreach (DataAccessLayer.Type userType in CurrentUser.Types)
                    {
                        if (userType.Id == (int)LoginManager.AccessControlType.DOCTOR)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fixed time selected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFixedTimerSelected
        {
            get
            {
                return isFixedTimerSelected;
            }
            set
            {

                isFixedTimerSelected = value;
                RaisePropertyChanged("IsFixedTimerSelected");
                if (isFixedTimerSelected)
                {
                    ISTTIFixedTimerSelected = false;
                    ISTTIDurationTimerSelected = false;
                    ISTTISelected = false;
                    CanUpadteRequiredAblationTime = true;

                    ProcedureLogModel.AblationTimersSet = new Tuple<bool, bool, bool>(true, false, false);
                }

            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether TTI fixed timer selected  or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool ISTTIFixedTimerSelected
        {
            get
            {
                return iSTTIFixedTimerSelected;
            }
            set
            {
                iSTTIFixedTimerSelected = value;
                RaisePropertyChanged("ISTTIFixedTimerSelected");
                if (iSTTIFixedTimerSelected)
                {
                    IsFixedTimerSelected = false;
                    ISTTIDurationTimerSelected = false;
                    ISTTISelected = true;
                    CanUpadteRequiredAblationTime = false;

                    ProcedureLogModel.AblationTimersSet = new Tuple<bool, bool, bool>(false, true, false);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether TTI duration timer selected  or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool ISTTIDurationTimerSelected
        {
            get
            {
                return iSTTIDurationTimerSelected;
            }
            set
            {
                iSTTIDurationTimerSelected = value;
                RaisePropertyChanged("ISTTIDurationTimerSelected");
                if (iSTTIDurationTimerSelected)
                {
                    IsFixedTimerSelected = false;
                    ISTTIFixedTimerSelected = false;
                    ISTTISelected = true;
                    CanUpadteRequiredAblationTime = false;

                    ProcedureLogModel.AblationTimersSet = new Tuple<bool, bool, bool>(false, false, true);
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether TTI selected  or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool ISTTISelected
        {
            get
            {
                if (SensorReadingMananger.AreSensorsConnected)
                    return iSTTISelected;
                else
                    return ISTTISelectedPlayback;
            }
            set
            {

                if (SensorReadingMananger.AreSensorsConnected)
                    iSTTISelected = value;
                else
                    ISTTISelectedPlayback = value;

                RaisePropertyChanged("ISTTISelected");
            }

        }

        public bool ISTTISelectedPlayback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Reset CMCU Error Stopwatch disconnection (Stopwatch) value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Stopwatch ResetCMCUErrorStopWatchDisconnection
        {
            get
            {
                return resetCMCUErrorStopWatchDisconnection;
            }
            set
            {
                resetCMCUErrorStopWatchDisconnection = value;
            }
        }

        /// <summary>
        /// Gets or sets the Reset PMCU Error Stopwatch disconnection (Stopwatch) value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Stopwatch ResetPMCUErrorStopWatchDisconnection
        {
            get
            {
                return resetPMCUErrorStopWatchDisconnection;
            }
            set
            {
                resetPMCUErrorStopWatchDisconnection = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the System is reseted or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemRested
        {
            get
            {
                return isSystemRested;
            }
            set
            {
                isSystemRested = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Vacuum is disconnecteds or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsVacuumDisconnected
        {
            get
            {
                return isVacuumDisconnected;
            }
            set
            {
                isVacuumDisconnected = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CPLD is latching or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCPLDLatching
        {
            get
            {
                return isCPLDLatching;
            }
            set
            {
                isCPLDLatching = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Can One is in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCanOneInError
        {
            get
            {
                return isCanOneInError;
            }
            set
            {
                isCanOneInError = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Can One communication has stopped listening or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool StopListeningCanOneCommunication
        {
            get
            {
                return stopListeningCanOneCommunication;
            }
            set
            {
                stopListeningCanOneCommunication = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Tank can be changed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanChangeTank
        {
            get
            {
                return canChangeTank;
            }

            set
            {
                canChangeTank = value;
            }
        }

        /// <summary>
        /// Gets or sets the Hospital Name value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalName
        {
            get
            {
                return hospitalName;
            }
            set
            {
                hospitalName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Can Two communication has stopped listening or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool StopListeningCanTwoCommunication
        {
            get
            {
                return stopListeningCanTwoCommunication;
            }
            set
            {
                stopListeningCanTwoCommunication = value;
            }
        }

        /// <summary>
        /// Gets or sets the Usage Timer (DispatcherTimer) value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public System.Timers.Timer UsageTimer
        {
            get
            {
                return usageTimer;
            }
        }

        /// <summary>
        /// Gets or sets the Minutes of Use value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public long MinutesOfUse
        {
            get
            {
                return minutesOfUse;
            }
            set
            {
                SetProperty(ref this.minutesOfUse, value);
            }
        }

        /// <summary>
        /// Gets or sets the CPLD Firmware version value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CpldFirmwareVersion
        {
            get
            {
                return cpldFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.cpldFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets the Repeater Firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterFirmware
        {
            get
            {
                return repeaterFirmware;
            }
            set
            {
                SetProperty(ref this.repeaterFirmware, value);
            }
        }

        /// <summary>
        /// Gets or sets the ICB Firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ICBFirmware
        {
            get
            {
                return iCBFirmware;
            }
            set
            {
                SetProperty(ref this.iCBFirmware, value);
            }
        }

        /// <summary>
        /// Gets or sets the Current Tank value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DataAccessLayer.Tank CurrentTank
        {
            get
            {
                return currentTank;
            }
            set
            {
                currentTank = value;
                RaisePropertyChanged("CurrentTank");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the System is in warning or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSytemInWarning
        {
            get
            {
                return isSytemInWarning;
            }
            set
            {
                isSytemInWarning = value;
            }
        }

        /// <summary>
        /// Gets or sets Is Blood Detected In Catheter value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBloodDetectedInCatheter
        {
            get
            {
                return isBloodDetectedInCatheter;
            }

            set
            {
                isBloodDetectedInCatheter = value;
            }
        }

        /// <summary>
        /// Gets or sets the Threshold For Inner Balloon Pressure Low value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThresholdForInnerBallonPressureLow
        {
            get
            {
                return thresholdForInnerBallonPressureLow;
            }

            set
            {
                SetProperty(ref this.thresholdForInnerBallonPressureLow, value);
            }
        }

        /// <summary>
        /// Gets or sets Catheter is Connecting value
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
        /// Gets or sets Is CMCU self test fail value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUSelfTestFail
        {
            get
            {
                return isCMCUSelfTestFail;
            }

            set
            {
                isCMCUSelfTestFail = value;
            }
        }

        /// <summary>
        /// Gets or sets Is PMCU Self Test Fail value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUSelfTestFail
        {
            get
            {
                return isPMCUSelfTestFail;
            }

            set
            {
                isPMCUSelfTestFail = value;
            }
        }

        /// <summary>
        /// Gets or sets Is Diaphragm Movement Detected value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsDiaphragmMovementDetected
        {
            get
            {
                return isDiaphragmMovementDetected;
            }

            set
            {
                isDiaphragmMovementDetected = value;
                RaisePropertyChanged("IsDiaphragmMovementDetected");
            }
        }

        /// <summary>
        /// Gets or sets the Minimum Diaphragm Movement value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DMSDetectionThreshold
        {
            get
            {
                return Current.Console.DmsDetectionThreshold;
            }

            set
            {
                Current.Console.DmsDetectionThreshold = value;
                RaisePropertyChanged("DMSDetectionThreshold");
            }
        }

        /// <summary>
        /// Gets or sets the Diaphragm Movement Compter value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte DiaphragmMovementCompter
        {
            get
            {
                return diaphragmMovementCompter;
            }

            set
            {
                diaphragmMovementCompter = value;
            }
        }

        /// <summary>
        /// Gets or sets the Is Vein Isolated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsVeinIsolated
        {
            get
            {
                return isVeinIsolated;
            }

            set
            {
                if (value != isVeinIsolated)
                {
                    isVeinIsolated = value;
                    if (isVeinIsolated)
                        RaisePropertyChanged("IsVeinIsolated");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Maximum Average Pacing Level value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <id>SF-SDS-0105</id>
        public double MaximumAveragePacingLevel
        {
            get
            {
                if (DiaphragmConditioning.IsDiaphragmReseting)
                    return 0;
                return maximumAveragePacingLevel;
            }

            set
            {

                if (value > PacingLevelMaxvalue)
                {
                    maximumAveragePacingLevel = PacingLevelMaxvalue;
                }

                else
                {
                    maximumAveragePacingLevel = value;
                }
                RaisePropertyChanged("MaximumAveragePacingLevel");
            }
        }

        /// <summary>
        /// Gets or sets Is Playback Mode Deactivated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPlayBackModeDeactivted
        {
            get
            {
                return isPlayBackModeDeactivted;
            }

            set
            {
                isPlayBackModeDeactivted = value;
                RaisePropertyChanged("IsPlayBackModeDeactivted");
            }
        }



        public bool IsUsingAutoPlayback
        {
            get
            {
                return isUsingAutoPlayback;
            }

            set
            {
                isUsingAutoPlayback = value;
                RaisePropertyChanged("IsUsingAutoPlayback");
            }
        }



        /// <summary>
        /// Gets or sets Generic Error value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string GenericError
        {
            get
            {
                return genericError;
            }

            set
            {
                if (value != genericError)
                {
                    genericError = value;
                    RaisePropertyChanged("GenericError");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Previous System State value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId PreviousSystemState
        {
            get
            {
                return previousSystemState;
            }

            set
            {
                SetProperty(ref this.previousSystemState, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether can one was in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCanOneWasInError
        {
            get
            {
                return isCanOneWasInError;
            }

            set
            {
                isCanOneWasInError = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether CanTwo was in error or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCanTwoWasInError
        {
            get
            {
                return isCanTwoWasInError;
            }

            set
            {
                isCanTwoWasInError = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether can one was reseted or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCanOneReseted
        {
            get
            {
                return isCanOneReseted;
            }

            set
            {
                isCanOneReseted = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether CanTwo was reseted or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        public bool IsCanTwoReseted
        {
            get
            {
                return isCanTwoReseted;
            }

            set
            {
                isCanTwoReseted = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ignore minimum diaphragm movement or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IgnoreMinimumDiaphragmMovementValue
        {
            get
            {
                return ignoreMinimumDiaphragmMovementValue;
            }

            set
            {
                ignoreMinimumDiaphragmMovementValue = value;
                RaisePropertyChanged("IgnoreMinimumDiaphragmMovementValue");
            }
        }
        /// <summary>
        /// Gets or sets an error id message and solution list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<Tuple<long, string, string, string>> ErrorIdMessageAndSolutionList
        {
            get
            {
                { lock (_errorIdMessageAndSolutionList_Lock) return errorIdMessageAndSolutionList; }
            }

            set
            {
                { lock (_errorIdMessageAndSolutionList_Lock) errorIdMessageAndSolutionList = value; }
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 1 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve1ON
        {
            get
            {
                return isSolenoidValve1ON;
            }

            set
            {
                isSolenoidValve1ON = value;
                RaisePropertyChanged("IsSolenoidValve1ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 2 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve2ON
        {
            get
            {
                return isSolenoidValve2ON;
            }

            set
            {
                isSolenoidValve2ON = value;
                RaisePropertyChanged("IsSolenoidValve2ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 3 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve3ON
        {
            get
            {
                return isSolenoidValve3ON;
            }

            set
            {
                isSolenoidValve3ON = value;
                RaisePropertyChanged("IsSolenoidValve3ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 4 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve4ON
        {
            get
            {
                return isSolenoidValve4ON;
            }

            set
            {
                isSolenoidValve4ON = value;
                RaisePropertyChanged("IsSolenoidValve4ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 5 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve5ON
        {
            get
            {
                return isSolenoidValve5ON;
            }

            set
            {
                isSolenoidValve5ON = value;
                RaisePropertyChanged("IsSolenoidValve5ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 6 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve6ON
        {
            get
            {
                return isSolenoidValve6ON;
            }

            set
            {
                isSolenoidValve6ON = value;
                RaisePropertyChanged("IsSolenoidValve6ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve7 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve7ON
        {
            get
            {
                return isSolenoidValve7ON;
            }

            set
            {
                isSolenoidValve7ON = value;
                RaisePropertyChanged("IsSolenoidValve7ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 8 is on or not.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve8ON
        {
            get
            {
                return isSolenoidValve8ON;
            }

            set
            {
                isSolenoidValve8ON = value;
                RaisePropertyChanged("IsSolenoidValve8ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether solenoid valve 9 is on or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSolenoidValve9ON
        {
            get
            {
                return isSolenoidValve9ON;
            }

            set
            {
                isSolenoidValve9ON = value;
                RaisePropertyChanged("IsSolenoidValve9ON");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether can update requiredablation time or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanUpadteRequiredAblationTime
        {
            get => canUpadteRequiredAblationTime;
            set => canUpadteRequiredAblationTime = value;
        }
        /// <summary>
        /// Gets or sets a value indicating whether allow user change tank or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUserAllowedToChangeTank
        {
            get
            {

                return (isUserAllowedToChangeTank &&
                       (SystemState == MessageStateId.CAN_ID_STATE_IDLE ||
                        SystemState == MessageStateId.CAN_ID_STATE_READY ||
                        SystemState == MessageStateId.CAN_ID_STATE_EXCEPTION));

            }

            private set
            {
                if (value != isUserAllowedToChangeTank)
                {
                    isUserAllowedToChangeTank = value;
                    RaisePropertyChanged("IsUserAllowedToChangeTank");
                }
            }
        }
        /// <summary>
        /// Gets or sets connection box value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ConnectionBox ConnectionBox { get; set; }

        /// <summary>
        /// Gets or sets sent catheter last use hour
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SentCatheterLastUseHour
        {
            get => sentCatheterLastUseHour;
            set => sentCatheterLastUseHour = value;
        }
        /// <summary>
        /// Gets or sets sent catheter last use day
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SentCatheterLastUseDay
        {
            get => sentCatheterLastUseDay;
            set => sentCatheterLastUseDay = value;
        }
        /// <summary>
        /// Gets or sets sent catheter last use month
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SentCatheterLastUseMonth
        {
            get => sentCatheterLastUseMonth;
            set => sentCatheterLastUseMonth = value;
        }
        /// <summary>
        /// Gets or sets sent catheter last use year
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SentCatheterLastUseYear
        {
            get => sentCatheterLastUseYear;
            set => sentCatheterLastUseYear = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow firm ware reading or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool AllowFirmwareReading { get => allowFirmwareReading; set => allowFirmwareReading = value; }

        /// <summary>
        /// Gets or sets an int value for Engineering catheter signature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int EngineeringCatheterSignature
        {
            get
            {
                return this.console.ServiceDevices.EngineeringCatheterSignature;
            }

        }
        /// <summary>
        /// Gets or sets an double value for ramp up time by step
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampUpTimeByStep
        {
            get
            {
                return rampUpTimeByStep;
            }

            set
            {
                SetProperty(ref this.rampUpTimeByStep, value);
            }
        }

        /// <summary>
        /// Gets or sets an double value for pressure ramp up
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampUpValue
        {
            get
            {
                return pressureRampUpValue;
            }
            set
            {

                SetProperty(ref this.pressureRampUpValue, value);
            }
        }
        /// <summary>
        /// Gets or sets an double value for ramp down time by step
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampDownTimeByStep
        {
            get
            {
                return rampDownTimeByStep;
            }
            set
            {
                SetProperty(ref this.rampDownTimeByStep, value);
            }
        }

        /// <summary>
        /// Gets or sets an double value for pressure ramp down 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampDownValue
        {
            get
            {
                return pressureRampDownValue;
            }
            set
            {
                SetProperty(ref this.pressureRampDownValue, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for inflate deflate balloon
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public InflateDeflateBalloonModel InflateDeflateBalloonModel
        {
            get => inflateDeflateBalloonModel;

            set => inflateDeflateBalloonModel = value;
        }

        /// <summary>
        /// Gets or sets a change balloon type finite state machine
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ChangeBalloonTypeFSM ChangeBalloonTypeFSM
        {
            get
            {
                return changeBalloonTypeFSM;
            }
            set
            {

                SetProperty(ref this.changeBalloonTypeFSM, value);
            }
        }
        /// <summary>
        /// Gets or sets  a value indicating whether DAS Ballon is using or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemUsingDASBalloon
        {
            get
            {
                return isSystemUsingDASBalloon;
            }
            set
            {
                isSystemUsingDASBalloon = value;
                RaisePropertyChanged("IsSystemUsingDASBalloon");
            }
        }
        /// <summary>
        /// Gets or sets  a value indicating whether used for Engineering
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsedForEngineering
        {
            get
            {
                return isUsedForEngineering;
            }
            set
            {
                isUsedForEngineering = value;
                RaisePropertyChanged("IsUsedForEngineering");
            }
        }
        /// <summary>
        /// Gets or sets  a value indicating whether Firmware consume data is correctly
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFirmwareConsumedDataCorrectly
        {
            get
            {
                return isFirmwareConsumedDataCorrectly;
            }
            set
            {
                SetProperty(ref this.isFirmwareConsumedDataCorrectly, value);
            }
        }
        /// <summary>
        /// Gets or sets a value for skin to skin duration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SkinToSkinDuration
        {
            get => skinToSkinDuration;
            set => skinToSkinDuration = value;
        }

        /// <summary>
        /// Gets or sets  a tuple value for CMCU Error
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Tuple<long, string, string, string> CmcuTupleError
        {
            get => cmcuTupleError;
            set => cmcuTupleError = value;
        }

        /// <summary>
        /// Gets or sets  a tuple value for PMCU Error
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Tuple<long, string, string, string> PmcuTupleError
        {
            get => pmcuTupleError;
            set => pmcuTupleError = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating whether is system in error
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemInDataError
        {
            get => isSystemInDataError;
            set => isSystemInDataError = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating whether is system allowed to go to playback
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAllowedToSetPlayBack
        {
            get => isAllowedToSetPlayBack;
            set => isAllowedToSetPlayBack = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the GUI is running
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool GUIIsRunning
        {
            get => gUIIsRunning;
            set => gUIIsRunning = value;
        }


        /// <summary>
        /// Gets or sets or the ASCII To Byte converter object
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ASCIIToByteConverter ASCIIToByteConverter
        {
            get => aSCIIToByteConverter;
            set => aSCIIToByteConverter = value;
        }

        /// <summary>
        /// Gets or sets or the boot loader data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] BootLoaderData
        {
            get => bootLoaderData;
            set => bootLoaderData = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the boot loader updating firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBootLoaderUpdatingFirmware
        {
            get
            {
                return isBootLoaderUpdatingFirmware;
            }
            set
            {
                isBootLoaderUpdatingFirmware = value;
                RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
            }
        }

        /// <summary>
        /// Gets or sets  the skin to skin ablation timer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public System.Timers.Timer SkinToSkinAblationTimer
        {
            get => skinToSkinAblationTimer;
        }

        /// <summary>
        /// Gets or sets  the upgrade status
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double UpgradeStatus
        {
            get
            {
                return upgradeStatus;
            }
            set
            {
                upgradeStatus = value;
                RaisePropertyChanged("UpgradeStatus");
            }
        }

        /// <summary>
        /// Gets or sets  the module key for the update
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ModuleKey
        {
            get
            {
                return moduleKey;
            }
            set
            {
                moduleKey = value;
                RaisePropertyChanged("ModuleKey");
            }
        }

        private double _loadCellCalibrationFactor;
        public double LoadCellCalibrationFactor
        {
          get => _loadCellCalibrationFactor;
          set
          {
            this._loadCellCalibrationFactor = value;
            this.RaisePropertyChanged(nameof(LoadCellCalibrationFactor));
          }
        }

        private double _loadCellCalibrationOffset;

        public double LoadCellCalibrationOffset
        {
          get => _loadCellCalibrationOffset;
          set
          {
            this._loadCellCalibrationOffset = value;
            this.RaisePropertyChanged(nameof(LoadCellCalibrationOffset));
          }
        }

        /// <summary>
        /// Gets or sets  the central microController bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerBootLoaderFirmwareVersion
        {
            get
            {
                return centralMicroControllerBootLoaderFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.centralMicroControllerBootLoaderFirmwareVersion, value);
            }
        }


        /// <summary>
        /// Gets or sets  the CPLD bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CpldBootLoaderFirmwareVersion
        {
            get
            {
                return cpldBootLoaderFirmwareVersion;
            }
            set
            {
                SetProperty(ref this.cpldBootLoaderFirmwareVersion, value);
            }
        }


        /// <summary>
        /// Gets or sets  the repeater bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterBootLoaderFirmware
        {
            get
            {
                return repeaterBootLoaderFirmware;
            }
            set
            {
                SetProperty(ref this.repeaterBootLoaderFirmware, value);
            }
        }

        /// <summary>
        /// Gets or sets  the patient bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerBootLoaderFirmwareVersion
        {
            get
            {
                return patientMicroControllerBootLoaderFirmwareVersion;
            }
            set
            {

                SetProperty(ref this.patientMicroControllerBootLoaderFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets the ICB bootloader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ICBBootLoaderFirmwareVersion
        {
            get
            {
                return iCBBootLoaderFirmwareVersion;
            }
            set
            {

                SetProperty(ref this.iCBBootLoaderFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets  the remote control bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RemoteControlBootLoaderFirmwareVersion
        {
            get
            {
                return remoteControlBootLoaderFirmwareVersion;
            }
            set
            {
                if (RemoteControlFirmware == 4098)
                    SetProperty(ref this.remoteControlBootLoaderFirmwareVersion, 0);
                else
                    SetProperty(ref this.remoteControlBootLoaderFirmwareVersion, value);
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the blood detector wire is open
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0002</id>
        public bool IsBloodDetectorwireOpen
        {

            get
            {
                return isBloodDetectorwireOpen;
            }
            set
            {
                isBloodDetectorwireOpen = value;
            }
        }

        /// <summary>
        /// Gets or sets the blood detector impedance
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0003</id>
        public int BloodDetecorImValue
        {
            get
            {
                return bloodDetecorImValue;
            }
            set
            {
                bloodDetecorImValue = value;
                RaisePropertyChanged("BloodDetecorImValue");
            }
        }

        /// <summary>
        /// Gets or sets the blood detection type
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0004</id>
        public int BloodDetectionType
        {
            get
            {
                return bloodDetectionType;
            }
            set
            {
                SetProperty(ref this.bloodDetectionType, value);
            }
        }

        /// <summary>
        /// Gets or sets the blood detection lower threshold
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0005</id>
        public short LowerBloodThreshold
        {
            get
            {
                return lowerBloodThreshold;
            }
            set
            {
                SetProperty(ref this.lowerBloodThreshold, value);
            }
        }

        /// <summary>
        /// Gets or sets the blood detection upper threshold
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0001</id>
        public short UpperBloodThreshold
        {
            get
            {
                return upperBloodThreshold;
            }
            set
            {

                SetProperty(ref this.upperBloodThreshold, value);
            }

        }

        /// <summary>
        /// Gets or sets the thawing temperature set point.
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0007</id>
        public double ThawingTemperatureSetPoint
        {
            get
            {
                return thawingTemperatureSetPoint;
            }
            set
            {
                SetProperty(ref this.thawingTemperatureSetPoint, value);
            }
        }

        public RemoteControlFSM RemoteControlFSM
        {
            get
            {
                return remoteControlFSM;
            }
            set
            {
                SetProperty(ref this.remoteControlFSM, value);
            }
        }

        public Stopwatch RemoteControlStopWatchDisconnection
        {
            get
            {
                return remoteControlStopWatchDisconnection;
            }
            set
            {
                remoteControlStopWatchDisconnection = value;
            }
        }

        public LSPROEnumeartion LSPROEnumeartion
        {
            get => lSPROEnumeartion;
            set => lSPROEnumeartion = value;
        }
        public bool IsLsproInitialized
        {
            get => isLsproInitialized;
            set => isLsproInitialized = value;
        }
        public string PortName
        {
            get
            {
                return portName;
            }
            set
            {
                portName = value;
                RaisePropertyChanged("PortName");
            }
        }

        public ISerialPortManager SpManager
        {
            get => _spManager;
            set => _spManager = value;
        }

        public int RemoteControlFirmware
        {
            get
            {
                return remoteControlFirmware;
            }
            set
            {
                SetProperty(ref this.remoteControlFirmware, value);
            }
        }

        public bool IsBloodPressureSensorConnected
        {
            get
            {
                return isBloodPressureSensorConnected;
            }
            set
            {

                SetProperty(ref this.isBloodPressureSensorConnected, value);
            }
        }



        public bool IsMultiEtsSesnorConnected
        {
            get
            {
                //#if Simulator
                //                return  true;   //false; //
                //#endif
                return isMultiEtsSesnorConnected;
            }
            set
            {

                SetProperty(ref this.isMultiEtsSesnorConnected, value);
            }
        }

        public double OcclusionPressureTareValue
        {
            get
            {
                return occlusionPressureTareValue;
            }
            set
            {
                SetProperty(ref this.occlusionPressureTareValue, value);
            }
        }

        public int OcclusionPressureGraphAxisYMaximum
        {
            get
            {
                return occlusionPressureGraphAxisYMaximum;
            }
            set
            {
                SetProperty(ref this.occlusionPressureGraphAxisYMaximum, value);
            }
        }

        public int OcclusionPressureGraphAxisYMinimum
        {
            get
            {
                return occlusionPressureGraphAxisYMinimum;
            }
            set
            {
                SetProperty(ref this.occlusionPressureGraphAxisYMinimum, value);
            }
        }

        public int OcclusionPressureGraphSweepSpeed
        {
            get
            {
                return occlusionPressureGraphSweepSpeed;
            }
            set
            {
                SetProperty(ref this.occlusionPressureGraphSweepSpeed, value);
            }
        }

        public double DASLowFlow
        {
            get => dASLowFlow;
            set => dASLowFlow = value;
        }


        /// <summary>
        /// Gets or sets a DiaphragmConditioning object
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DiaphragmConditioning DiaphragmConditioning
        {
            get
            {
                return diaphragmConditioning;
            }
            set
            {

                SetProperty(ref this.diaphragmConditioning, value);
            }
        }



        /// <summary>
        /// Initializes the Register IDS Dynamic tables
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeRegisterIDSDynamicTables()
        {
            // according to 13/06/2017 email(yong, chadi) the validation will be only for ids (15 to 34 and  52 to 55)
            // Befor i wase reading  from 48 to 56 and 8 to 36
            foreach (var item in PatientMicroControllerAckRegistersTable)
            {
                if (PatientMicroControllerAckRegistersTable.Count > item.Key)
                    PatientMicroControllerAckRegistersTable[item.Key] = false;
            }

            foreach (var item in CentralMicroControllerAckRegistersTable)
            {
                if (CentralMicroControllerAckRegistersTable.Count > item.Key)
                    CentralMicroControllerAckRegistersTable[item.Key] = false;
            }
        }

        /// <summary>
        /// Initializes the Ack Registers table
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeAckRegistersTable()
        {
#region CMCU

            CentralMicroControllerAckRegistersTable.Add(15, ackForRegisters15);
            CentralMicroControllerAckRegistersTable.Add(16, aAckForRegisters16);
            CentralMicroControllerAckRegistersTable.Add(17, ackForRegisters17);
            CentralMicroControllerAckRegistersTable.Add(18, ackForRegisters18);
            CentralMicroControllerAckRegistersTable.Add(19, acKForRegisters19);
            CentralMicroControllerAckRegistersTable.Add(20, acKForRegisters20);
            CentralMicroControllerAckRegistersTable.Add(21, acKForRegisters21);
            CentralMicroControllerAckRegistersTable.Add(22, acKForRegisters22);
            CentralMicroControllerAckRegistersTable.Add(23, acKForRegisters23);
            CentralMicroControllerAckRegistersTable.Add(24, acKForRegisters24);
            CentralMicroControllerAckRegistersTable.Add(25, acKForRegisters25);
            CentralMicroControllerAckRegistersTable.Add(26, acKForRegisters26);
            CentralMicroControllerAckRegistersTable.Add(27, acKForRegisters27);
            CentralMicroControllerAckRegistersTable.Add(28, acKForRegisters28);
            CentralMicroControllerAckRegistersTable.Add(29, acKForRegisters29);
            CentralMicroControllerAckRegistersTable.Add(30, acKForRegisters30);
            CentralMicroControllerAckRegistersTable.Add(31, acKForRegisters31);
            CentralMicroControllerAckRegistersTable.Add(32, acKForRegisters32);
            CentralMicroControllerAckRegistersTable.Add(33, acKForRegisters33);
            CentralMicroControllerAckRegistersTable.Add(34, acKForRegisters34);

#endregion CMCU

#region PMCU

            PatientMicroControllerAckRegistersTable.Add(52, acKForRegisters52);
            PatientMicroControllerAckRegistersTable.Add(53, acKForRegisters53);
            PatientMicroControllerAckRegistersTable.Add(54, acKForRegisters54);
            PatientMicroControllerAckRegistersTable.Add(55, acKForRegisters55);

#endregion PMCU
        }


        /// <summary>
        /// Occurs when the ViewChanged event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="e">A ViewEventsArgs that contains the event data.</param>
        public void OnViewchanged(ViewsEventArgs e)
        {
            if (ViewChanged != null)
            {
                ViewChanged(null, e);
            }
        }

        /// <summary>
        /// Occurs when the Flow Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A FlowMeterEventArgs that contains the event data.</param>
        private void FlowChanged(object sender, FlowMeterEventArgs e)
        {
            ResetCanOneStopWatch();
            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;

                switch (e.FlowMeter.ID)
                {
                    case 0:

                        FM1Reading = CanBusMessageConverter.ConverteDecimalDataFM1(data, 0);
                        PT5Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        PIDDutyCycle = CanBusMessageConverter.ConverteDecimalData(data, 4);
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when the Load Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A LoadCellEventArgs that contains the event data.</param>
        private void LoadChanged(object sender, LoadCellEventArgs e)
        {
            ResetCanOneStopWatch();
            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;
                LC1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
            }
        }

        /// <summary>
        /// Occurs when the blood detection event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A BloodDetectorEventArgs that contains the event data.</param>
        private void BloodDetectorEvent(object sender, BloodDetectorEventArgs e)
        {
            ResetCanOneStopWatch();
            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;

                BloodDetectionType = (int)CanBusMessageConverter.ConverteDecimalData(data, 0);
                BloodDetecorImValue = (int)CanBusMessageConverter.ConverteDecimalData(data, 4);

            }
        }

        /// <summary>
        /// Occurs when the Pressure Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A PressureTransducerEventArgs that contains the event data.</param>
        private void PressureChanged(object sender, PressureTransducerEventArgs e)
        {
            ResetCanOneStopWatch();

            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;

                switch (e.Type)
                {
                    //
                    case PressureTransducerEventArgs.PressureType.TP:

                        PT1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PT2Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        PT3Reading = CanBusMessageConverter.ConverteDecimalData(data, 4);
                        PT4Reading = CanBusMessageConverter.ConverteDecimalData(data, 6);
                        break;

                    case PressureTransducerEventArgs.PressureType.CP:

                        CP1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        CP2Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
                        TIPReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
                        PatientPIDDutyCycle = CanBusMessageConverter.ConverteDecimalData(data, 6);
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when the Pressure Switch Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A PressureSwitchEventArgs that contains the event data.</param>
        private void PressureSwitchChanged(object sender, PressureSwitchEventArgs e)
        {
            ResetCanOneStopWatch();
            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;
                PS1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
                PS2Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
            }
        }

        /// <summary>
        /// Occurs when the Register Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A RegisterValuesEventArgs that contains the event data.</param>
        private void RegisterChanged(object sender, RegisterValuesEventArgs e)
        {
            ResetCanOneStopWatch();

            listOfValues.Clear();

            var communicationData = sender as ICanBusCommunication;
            byte[] data = null;

            if (communicationData != null && communicationData.CanBusOneEventArgs.Data != null)
            {
                data = communicationData.CanBusOneEventArgs.Data;
            }

            if (communicationData.CanBusOneEventArgs.Falgs != (int)FrameType.Remote && data != null)
            {

                // Register values Main Microcontroller
                switch (e.ID)
                {
                    case 8:

                        CentralMicroControllerFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
                        CpldFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
                        CentralMicroControllerBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 4);
                        break;

                    case 9:

                        CPLDErrorRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 10:

                        CPLDValveRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 11:

                        CPLDSystemRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 12:

                        // we wase supossed to use these message Id to get the state. but we changed the Message
                        // construction so the state is included in the message. so we are supposed to never get these Message ID
                        // SystemState = (MessageStateId)CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 13:

                        AblationTime = CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 14:

                        ContinuousThawing = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        break;

                    case 15:

                        TargetInjectionFlow = CanBusMessageConverter.ConverteDecimalDataFM1(data, 0);
                        TargetInjectionPressure = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(15);

                            if (listOfValues != null &&
                                console != null &&
                                console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine != null &&
                                console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                listOfValues.Add(Tuple.Create(console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetInjectionFlow, TargetInjectionFlow));
                                listOfValues.Add(Tuple.Create(console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetInjectionPressure, TargetInjectionPressure));
                            }

                            CentralMicroControllerAckRegistersTable[15] = true;
                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }
                        break;

                    case 16:

                        PGain = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        IGain = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        DGain = CanBusMessageConverter.ConverteDecimalData(data, 4);
                        PIDOffset = CanBusMessageConverter.ConverteDecimalData(data, 6);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(16);

                            if (listOfValues != null &&
                                console != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, IGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, DGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PIDOffset));
                            }

                            CentralMicroControllerAckRegistersTable[16] = true;

                            //RegistersComparator.CompareValues(listOfValues, 4);
                        }

                        break;

                    case 17:

                        ThresholdForPT1High = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        ThresholdForPT1Fail = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        ThresholdForPT1Low = CanBusMessageConverter.ConverteDecimalData(data, 4);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(17);
                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerOneValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdForPT1High));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].TankPressureTooHigh, ThresholdForPT1Fail));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].TankPressureLow, ThresholdForPT1Low));
                            }

                            CentralMicroControllerAckRegistersTable[17] = true;

                            //RegistersComparator.CompareValues(listOfValues, 3);
                        }
                        break;

                    case 18:

                        PT1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PT1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            // CentralMicroControllerRegisterIDSDynamicTable.Remove(18);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerOneValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT1LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT1HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[18] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 19:

                        ThresholdPT2High = CanBusMessageConverter.ConverteDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(19);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerTwoValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerTwoValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT2High));
                            }

                            CentralMicroControllerAckRegistersTable[19] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }

                        break;

                    case 20:
                        PT2LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PT2HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(20);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerTwoValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerTwoValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT2LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT2HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[20] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 21:
                        ThresholdPT3High = CanBusMessageConverter.ConverteDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(21);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerThreeValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerThreeValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT3High));
                            }

                            CentralMicroControllerAckRegistersTable[21] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }

                        break;

                    case 22:
                        PT3LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PT3HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(22);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerThreeValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerThreeValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT3LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT3HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[22] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 23:

                        ThresholdPT4high = CanBusMessageConverter.ConverteDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(23);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerFourValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerFourValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT4high));
                            }

                            CentralMicroControllerAckRegistersTable[23] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }

                        break;

                    case 24:

                        PT4LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PT4HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(24);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureTransducerFourValueAccordingToTheStateMachine != null &&
                                console.PressureTransducerFourValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT4LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT4HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[24] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 25:
                        ThresholdTS1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(25);

                            if (listOfValues != null &&
                                console != null &&
                                console.TemperatureSensorOneValueAccordingToTheStateMachine != null &&
                                console.TemperatureSensorOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureThresholdHighLimit, ThresholdTS1High));
                            }

                            CentralMicroControllerAckRegistersTable[25] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }

                        break;

                    case 26:

                        TS1LowRange = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        TS1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(26);

                            if (listOfValues != null &&
                                console != null &&
                                console.TemperatureSensorOneValueAccordingToTheStateMachine != null &&
                                console.TemperatureSensorOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureLowRangeLimit, TS1LowRange));
                                listOfValues.Add(Tuple.Create(console.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureHighRangeLimit, TS1HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[26] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 27:
                        ThresholdFM1Low = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 0);
                        ThresholdFM1High = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(27);

                            if (listOfValues != null &&
                                console != null &&
                                console.FlowMeterOneValueAccordingToTheStateMachine != null &&
                                console.FlowMeterOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterThresholLowlimit, ThresholdFM1Low));
                                listOfValues.Add(Tuple.Create(console.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterThresholHighlimit, ThresholdFM1High));
                            }

                            CentralMicroControllerAckRegistersTable[27] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 28:
                        FM1LowRange = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 0);
                        FM1HighRange = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(28);

                            if (listOfValues != null &&
                                console != null &&
                                console.FlowMeterOneValueAccordingToTheStateMachine != null &&
                                console.FlowMeterOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterLowRangeLimit, FM1LowRange));
                                listOfValues.Add(Tuple.Create(console.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterHighRangelimit, FM1HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[28] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 29:

                        ThresholdPS1High = CanBusMessageConverter.ConverteDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(29);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureSwitchOneValueAccordingToTheStateMachine != null &&
                                console.PressureSwitchOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPS1High));
                            }

                            CentralMicroControllerAckRegistersTable[29] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }
                        break;

                    case 30:
                        PS1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PS1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(30);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureSwitchOneValueAccordingToTheStateMachine != null &&
                                console.PressureSwitchOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PS1LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PS1HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[30] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 31:
                        ThresholdPS2High = CanBusMessageConverter.ConverteDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(31);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureSwitchTwoValueAccordingToTheStateMachine != null &&
                                console.PressureSwitchTwoValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPS2High));
                            }

                            CentralMicroControllerAckRegistersTable[31] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }
                        break;

                    case 32:
                        PS2LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PS2HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(32);

                            if (listOfValues != null &&
                                console != null &&
                                console.PressureSwitchTwoValueAccordingToTheStateMachine != null &&
                                console.PressureSwitchTwoValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PS2LowRange));
                                listOfValues.Add(Tuple.Create(console.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PS2HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[32] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }
                        break;

                    case 33:
                        ThresholdLC1Warning = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        ThresholdLC1Fail = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(33);

                            if (listOfValues != null &&
                                console != null &&
                                console.LoadCellOneValueAccordingToTheStateMachine != null &&
                                console.LoadCellOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellThresholdWarning, ThresholdLC1Warning));
                                listOfValues.Add(Tuple.Create(console.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellThresholdFail, ThresholdLC1Fail));
                            }

                            CentralMicroControllerAckRegistersTable[33] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }
                        break;

                    case 34:
                        LC1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        LC1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            //CentralMicroControllerRegisterIDSDynamicTable.Remove(34);

                            if (listOfValues != null &&
                                console != null &&
                                console.LoadCellOneValueAccordingToTheStateMachine != null &&
                                console.LoadCellOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellLowRangeLimit, LC1LowRange));
                                listOfValues.Add(Tuple.Create(console.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellHighRangeLimit, LC1HighRange));
                            }

                            CentralMicroControllerAckRegistersTable[34] = true;

                            //RegistersComparator.CompareValues(listOfValues, 2);
                        }

                        break;

                    case 35:
                        SystemState = (MessageStateId)(Convert.ToInt32(communicationData.CanBusOneEventArgs.Id) & (Int32)Mask.CAN_ID_STATE_MASK);

                        remoteControlTimingToFactorIncrement++;
                        //PreviousSystemState = SystemState;

                        //I have to verify if we are sending the state many time
                        if (PreviousSystemState != SystemState || (remoteControlTimingToFactorIncrement >= remoteControlTimingToFactor))
                        {
                            SendSystemStateToRemoteControl(SystemState);

                            if (SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
                            {
                                PreviousSystemState = SystemState;
                            }
                            remoteControlTimingToFactorIncrement = 0;
                        }

                        CMCUSystemStatusError = CanBusMessageConverter.ConvertStatusErrorData(data);


                        if (IsWindowLoaded && HeartbeatActivated)
                            GetCMCUStatusError(CMCUSystemStatusError);

                        if (Console != null)
                        {
                            if (!DiaphragmConditioning?.IsDiaphragmReseting??false)
                            {
                                Console.IsConsoleInAblationState = (SystemState == MessageStateId.CAN_ID_STATE_TRANSITION ||
                                                                    SystemState == MessageStateId.CAN_ID_STATE_ABLATION || 
                                                                    SystemState == MessageStateId.CAN_ID_STATE_THAWING) ? true : false;
                            }

                        }
                        break;

                    case 36:

                        GetSolenoidValvesStatus(CanBusMessageConverter.ConvertValvesStatusData(data));

                        break;

                    // Register values Patient Microcontroller

                    case 48:
                        PatientMicroControllerFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
                        PatientMicroControllerBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
                        break;

                    case 49:
                        PMCUSystemStatusErrorCode = CanBusMessageConverter.ConvertStatusErrorData(data);

                        if (IsWindowLoaded && HeartbeatActivated)
                            GetPMCUStatusError(PMCUSystemStatusErrorCode);

                        break;

                    case 5:
                      if (data != null)
                      {
                        var containerBatch = CanBusMessageConverter.ConvertDataToUInt32(data, 0); 
                        var catheterLot2 = (UInt16)CanBusMessageConverter.ConverteCatheterInfoData(data, 4);

                        CatheterContainerTag = $"{containerBatch:D8}-{catheterLot2:D3}";
                        LogInfo($"Receive extended Catheter Container tag : {CatheterContainerTag}");
                      }

                      break; 
                    case 50:

                        if (!AllowFirmwareReading)
                        {
                            if (data != null)
                            {
                                CatheterID = data[0]; //CanBusMessageConverter.ConverteInfoData(data, 0);
                                CatheterSerialNumber = data[1]; //CanBusMessageConverter.ConverteInfoData(data, 2);
                                CatheterLot = CanBusMessageConverter.ConverteCatheterInfoData(data, 2);

                                // we are using exception datetime data because there is nodate time in C
                                CatheterExpirationMonth = data[4];
                                CatheterExpirationDay = data[5];
                                CatheterExpirationYear = CanBusMessageConverter.ConverteInfoData(data, 6);


                                CatheterExpirationDate = new DateTime(CatheterExpirationYear, CatheterExpirationMonth, CatheterExpirationDay);

                                if (IsCatheterLastUseDateUpdated && SentCatheterLastUseDay != 0 && SentCatheterLastUseMonth != 0
                                    && SentCatheterLastUseYear != 0 && _canManageRTRCatheterMessage)
                                {
                                  // here we validate 
                                  if (IsWindowLoaded)
                                  {
                                    _canManageRTRCatheterMessage = false; 
                                    // Invoke ManageRTRCatheterMessage if the LastUsedDate is Updated (Start Validating Catheter and acknowledge the console)
                                    // Console has an issue that would not send RTR message if we send multiple Acknowledge messages in 50 ms
                                    ManageRTRforCatheter(data, communicationData, e.ID);
                                    Task.Delay(500).ContinueWith(_ => _canManageRTRCatheterMessage = true);
                                  }

                                }

                                IsCatheterExpirationDateUpdated = true;
                            }
                        }
                        break;

                    case 51:
                        if (!AllowFirmwareReading)
                        {
                            if (IsCatheterExpirationDateUpdated)
                            {


                                if (data != null)
                                {
                                    // we are using exception datetime data because there is nodate time in C
                                    CatheterLastUseHour = data[0];
                                    CatheterLastUseDay = data[1];
                                    CatheterLastUseMonth = data[2];
                                    CatheterLastUseYear = CanBusMessageConverter.ConverteCatheterInfoData(data, 3);

                                    SentCatheterLastUseHour = data[0];
                                    SentCatheterLastUseDay = data[1];
                                    SentCatheterLastUseMonth = data[2];
                                    SentCatheterLastUseYear = CanBusMessageConverter.ConverteCatheterInfoData(data, 3);



                                    //if (CatheterLastUseHour == 0 || CatheterLastUseDay == 0 || CatheterLastUseMonth == 0 || CatheterLastUseYear == 0)
                                    if (CatheterLastUseDay == 0 || CatheterLastUseMonth == 0 || CatheterLastUseYear == 0)   //Emily changed for SCB-318
                                    {
                                        CatheterLastUseDate = DateTime.Now;

                                        CatheterLastUseHour = CatheterLastUseDate.Hour;
                                        CatheterLastUseDay = CatheterLastUseDate.Day;
                                        CatheterLastUseMonth = CatheterLastUseDate.Month;
                                        CatheterLastUseYear = CatheterLastUseDate.Year;

                                        // DO not use minutes and secondes an ms
                                        CatheterLastUseDate = new DateTime(CatheterLastUseYear, CatheterLastUseMonth, CatheterLastUseDay, CatheterLastUseHour, 0, 0, 0);




                                        // Sest the date 

                                        if (this.Console != null && this.Console.Catheter != null)
                                        {
                                            this.Console.Catheter.CatheterLastUseHour = CatheterLastUseHour;
                                            this.Console.Catheter.CatheterLastUseDay = CatheterLastUseDay;
                                            this.Console.Catheter.CatheterLastUseMonth = CatheterLastUseMonth;
                                            this.Console.Catheter.CatheterLastUseYear = CatheterLastUseYear;
                                        }

                                        if (communicationData != null && communicationData.CanBusOneEventArgs != null)
                                        {
                                            SendRequestedData(communicationData, communicationData.CanBusOneEventArgs.Id, (uint)e.ID, true, true);


                                        }
                                    }


                                    NumberOfInjections = CanBusMessageConverter.ConverteCatheterInfoData(data, 5);
                                    IsCatheterLastUseDateUpdated = true;


                                    if (this.Console != null && this.Console.Catheter != null)
                                    {
                                        this.Console.Catheter.CatheterLastUseHour = CatheterLastUseHour;
                                        this.Console.Catheter.CatheterLastUseDay = CatheterLastUseDay;
                                        this.Console.Catheter.CatheterLastUseMonth = CatheterLastUseMonth;
                                        this.Console.Catheter.CatheterLastUseYear = CatheterLastUseYear;
                                    }
                                }
                            }
                        }
                        break;

                    case 52:
                        TargetBalloonPressure = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            PatientMicroControllerRegisterIDSDynamicTable.Remove(52);

                            if (listOfValues != null && console != null &&
                                console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine != null &&
                                console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetBalloonPressure, TargetBalloonPressure));
                            }

                            PatientMicroControllerAckRegistersTable[52] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }
                        break;

                    case 53:
                        ThresholdForCP1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        ThresholdForOuterBallonPressure = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
                        ThresholdForInnerBallonPressureLow = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            PatientMicroControllerRegisterIDSDynamicTable.Remove(53);

                            if (listOfValues != null && console != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // here i am waiting for the threshold these code have to be updated
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, IGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, DGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PIDOffset));
                            }

                            PatientMicroControllerAckRegistersTable[53] = true;

                            //RegistersComparator.CompareValues(listOfValues, 4);
                        }
                        break;

                    case 54:
                        ThresholdForCTC1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        ThresholdForCTC2High = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
                        ThawingTemperatureSetPoint = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);

                        LowerBloodThreshold = (short)CanBusMessageConverter.ConverteDecimalData(data, 4);
                        UpperBloodThreshold = (short)CanBusMessageConverter.ConverteDecimalData(data, 6);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            PatientMicroControllerRegisterIDSDynamicTable.Remove(54);

                            if (listOfValues != null && console != null &&
                                console.ThermocoupleOneValueAccordingToTheStateMachine != null &&
                                console.ThermocoupleOneValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.ThermocoupleOneValueAccordingToTheStateMachine[SystemState].ThawingTemperature, ThresholdForCTC1High));
                            }

                            PatientMicroControllerAckRegistersTable[54] = true;

                            //RegistersComparator.CompareValues(listOfValues, 1);
                        }

                        break;

                    case 55:
                        PatientPGain = CanBusMessageConverter.ConverteDecimalData(data, 0);
                        PatientIGain = CanBusMessageConverter.ConverteDecimalData(data, 2);
                        PatientDGain = CanBusMessageConverter.ConverteDecimalData(data, 4);
                        PatientPIDOffset = CanBusMessageConverter.ConverteDecimalData(data, 6);

                        if (IsReadingFromMicroControllerForRegisterValidation)
                        {
                            PatientMicroControllerRegisterIDSDynamicTable.Remove(55);

                            if (listOfValues != null && console != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine != null &&
                                console.PatientMicroControllerPIDValueAccordingToTheStateMachine.ContainsKey(SystemState))
                            {
                                // To Do
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PatientPGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, PatientIGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, PatientDGain));
                                listOfValues.Add(Tuple.Create(console.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PatientPIDOffset));
                            }

                            PatientMicroControllerAckRegistersTable[55] = true;

                            //RegistersComparator.CompareValues(listOfValues, 4);
                        }
                        break;

                    case 56:
                        CatheterFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
                        break;

                    case 57:

                        RampUpTimeByStep = CanBusMessageConverter.ConvertRampUpTimeAndRampDownTimeByStepData(data, 0);
                        PressureRampUpValue = CanBusMessageConverter.ConvertRampUpPressureAndRampDownPressureByStepData(data, 2);
                        RampDownTimeByStep = CanBusMessageConverter.ConvertRampUpTimeAndRampDownTimeByStepData(data, 4);
                        PressureRampDownValue = CanBusMessageConverter.ConvertRampUpPressureAndRampDownPressureByStepData(data, 6);
                        break;

                    case 59:
                        SendInit();

                        break;

                    //TODO
                    case 60:
                        ModuleKey = CanBusMessageConverter.ConvertModuleKeyData(data);
                        UpgradeStatus = CanBusMessageConverter.ConvertUpgradeStatusData(data);
                        break;

                    // LoadCell Calibration Readback data
                    case 62:
                      LoadCellCalibrationFactor = CanBusMessageConverter.ConvertDecimalDataWithFactor(data, 2, 10000);
                      LoadCellCalibrationOffset = CanBusMessageConverter.ConverteDecimalData(data, 4);
                    break;
                }
            }
            else
            {
                if (e != null && e.ID == 50)
                {

                    if (IsWindowLoaded)
                        ManageRTRforCatheter(data, communicationData, e.ID);
                }

                if (e != null && e.ID == 51)
                {
                }

                if (IsCatheterValid && (e != null && e.ID != 51 && e.ID != 58))
                {
                    // we are using RTR so we have to ansewr with the same ID and to build the data we will use local id
                    if (communicationData != null && communicationData.CanBusOneEventArgs != null)
                    {
                        SendRequestedData(communicationData, communicationData.CanBusOneEventArgs.Id, (uint)e.ID);
                    }
                    // catheterConnectedTimer.Stop();
                }

                if (e != null && e.ID == 58)
                {
                    if (IsWindowLoaded)
                    {

                        AnswerRTRBootData(communicationData);

                    }

                }

#region Boot Loader

#endregion
            }
        }

        /// <summary>
        /// Answer RTR Boot Data
        ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="communicationData">communication data</param>
        private void AnswerRTRBootData(ICanBusCommunication communicationData)
        {
            if (AllowFirmwareReading)
            {
                if (ASCIIToByteConverter.CanSendEndTransmission)
                {
                    this.Console.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_END, ASCIIToByteConverter.Initdata);

                }

                else
                {
                    for (int i = 0; i < 8; i++)
                    {

                        Array.Clear(BootLoaderData, 0, 8);
                        packetNumber = 0;
                        BootLoaderData = ASCIIToByteConverter.GetPacket(out packetNumber);

                        this.Console.AnswerRTRBootMessage(packetNumber, (int)communicationData.CanBusOneEventArgs.Id, BootLoaderData);
                    }
                }
            }
        }

        /// <summary>
        /// Answer RTR Boot Data FOR ICB Or Reapeter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="communicationData">communication data</param>
        private void AnswerRTRBootDataFORICBOrReapeter(ICanBusCommunication communicationData)
        {
            if (ASCIIToByteConverter.CanSendEndTransmission)
            {
                this.Console.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_END, ASCIIToByteConverter.Initdata);

            }

            else
            {
                for (int i = 0; i < 8; i++)
                {

                    Array.Clear(BootLoaderData, 0, 8);
                    packetNumber = 0;
                    BootLoaderData = ASCIIToByteConverter.GetPacket(out packetNumber);

                    this.Console.AnswerRTRBootMessageForICBOrReapeter(packetNumber, (int)communicationData.CanBusTwoEventArgs.Id, BootLoaderData);
                }
            }
        }

        /// <summary>
        /// Occurs when the Can Two Register Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A RegisterValuesEventArgs that contains the event data.</param>
        private void CanTwoRegisterChanged(object sender, RegisterValuesEventArgs e)
        {
            RestartCanTwoStopWatchCommunicationLost();
            
            var communicationData = sender as ICanBusCommunication;
            byte[] data = null;

            if (communicationData != null && communicationData.CanBusTwoEventArgs.Data != null)
            {
                data = communicationData.CanBusTwoEventArgs.Data;
            }

            switch (e.ID)
            {
                case 11:

                    RepeaterFirmware = CanBusMessageConverter.ConverteInfoData(data, 0);
                    ICBFirmware = CanBusMessageConverter.ConverteInfoData(data, 2);
                    RepeaterBootLoaderFirmware = CanBusMessageConverter.ConverteInfoData(data, 4);
                    ICBBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 6);
                    break;

                case 24:

                    RemoteControlFirmware = CanBusMessageConverter.ConverteInfoData(data, 0);
                    RemoteControlBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
                    break;

                case 58:
                    AnswerRTRBootDataFORICBOrReapeter(communicationData);
                    break;

                case 59:
                    SendInitFORICBOrReapeter();
                    break;

                case 60:
                    ModuleKey = CanBusMessageConverter.ConvertModuleKeyData(data);
                    UpgradeStatus = CanBusMessageConverter.ConvertUpgradeStatusData(data);
                    break;
            }
        }

        /// <summary>
        /// Occurs when the Temperature Changed event is raised
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The compoment for which a property has changed.</param>
        /// <param name="e">A ThermocoupleEventArgs that contains the event data.</param>
        private void TemperatureChanged(object sender, ThermocoupleEventArgs e)
        {
            ResetCanOneStopWatch();

            var communicationData = sender as ICanBusCommunication;

            if (communicationData != null && communicationData.CanBusOneEventArgs != null && e != null)
            {
                byte[] data = communicationData.CanBusOneEventArgs.Data;

                switch (e.Type)
                {
                    case ThermocoupleEventArgs.ThermocoupleType.TC:

                        TC1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        TC2Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
                        PMCUCJReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
                        break;

                    case ThermocoupleEventArgs.ThermocoupleType.TS:

                        TS1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
                        CMCUCJReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
                        TN2OReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
                        break;
                }
            }
        }

        /// <summary>
        /// Sends requested data to the Console
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="communicationData">A ICanBusCommunication.</param>
        /// <param name="RTRID">An unsigned-integer RTRID.</param>
        /// <param name="localId">An unsigned-integer Local ID.</param>
        /// <param name="isItAnsewringCatheterValidation">A boolean representing if answering catheter validaiton.</param>
        /// <param name="iscatheterValid">A boolean representing the catheter validity.</param>
        private void SendRequestedData(ICanBusCommunication communicationData, uint RTRID, uint localId, bool isItAnsewringCatheterValidation = false, bool iscatheterValid = false)
        {
            if (isItAnsewringCatheterValidation)
            {
                if (communicationData != null && communicationData.CanBusOneEventArgs != null && this.console != null)
                {
                    MessageStateId stateId = (MessageStateId)IdToMachineState.ConvertIdToSate(communicationData.CanBusOneEventArgs.Id);
                    Task.Run(() => this.console.AnswerForRemoteFrame(stateId, RTRID, localId, true, iscatheterValid));
                }
            }
            else
            {
                if (communicationData != null && communicationData.CanBusOneEventArgs != null && this.console != null)
                {
                    MessageStateId stateId = (MessageStateId)IdToMachineState.ConvertIdToSate(communicationData.CanBusOneEventArgs.Id);
                    Task.Run(() => this.console.AnswerForRemoteFrame(stateId, RTRID, localId));
                }
            }
        }

        private void SendBootRequestedData(ICanBusCommunication communicationData, uint RTRID, uint localId, bool isItAnsewringCatheterValidation = false, bool iscatheterValid = false)
        {
            if (communicationData != null && communicationData.CanBusOneEventArgs != null && this.console != null)
            {
                MessageStateId stateId = (MessageStateId)IdToMachineState.ConvertIdToSate(communicationData.CanBusOneEventArgs.Id);
                this.console.AnswerForRemoteFrame(stateId, RTRID, localId);
            }

        }

        /// <summary>
        /// Sending Patient microcontroller catheter data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SendPatientMicroControllerCatheterData()
        {
            foreach (int id in CatheterInfoIds)
            {
                foreach (MessageStateId messageStateId in Enum.GetValues(typeof(MessageStateId)))
                {
                    if (Console != null)
                    {
                        Console.SendRemoteFrame(ConsoleFiniteStateMachine.CurrentState, (uint)id);
                    }
                }
            }
        }

        /// <summary>
        /// Function that sends the LC1 threshold
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public async void SendTheLC1Thresholds()
        {
            await Task.Run(() =>
            {

                int thresholdLC1WarningAndFailId = 33;

                foreach (MessageStateId messageStateId in Enum.GetValues(typeof(MessageStateId)))
                {
                    if (messageStateId != MessageStateId.CAN_ID_STATE_UNKNOWN)
                    {
                        Console.WriteFromMicroController(messageStateId, thresholdLC1WarningAndFailId);
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }
            );
        }


        /// <summary>
        /// Send initialization data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SendInit()
        {
            if (AllowFirmwareReading)
            {
                this.Console.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ASCIIToByteConverter.Initdata);
            }
        }

        /// <summary>
        /// Send initialization data for ICB or reapeter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SendInitFORICBOrReapeter()
        {
            this.Console.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ASCIIToByteConverter.Initdata);
        }

        /// <summary>
        /// Manages the RTR for catheter.  (Used for the ID 50 or ID51)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ManageRTRforCatheter(byte[] data, ICanBusCommunication communicationData, int iD)
        {

#region Reel Catheter
            // here to validate we suppose that the catheter is not valid
            IsCatheterValid = false;
            IsUsedForEngineering = false;


            CatheterID = data[0]; //CanBusMessageConverter.ConverteInfoData(data, 0);
            CatheterSerialNumber = data[1]; //CanBusMessageConverter.ConverteInfoData(data, 2);
            CatheterLot = CanBusMessageConverter.ConverteCatheterInfoData(data, 2);

            //if (CatheterSerialNumber == this.console.ServiceDevices.EngineeringCatheter.SerialNumber &&
            //    CatheterLot == this.console.ServiceDevices.EngineeringCatheter.CatheterLot && IsCryterionUser)


            if ((EngineeringCatheterSignature & CatheterID) == EngineeringCatheterSignature && (IsCryterionUser || IsBSCADMINUser))
            {
              if (this.data.DataAccess.GetatheterInformationsAccordingToSerialNumberAndLot(CatheterSerialNumber, CatheterLot, CatheterID & (~EngineeringCatheterSignature), true) == null)
              {
                int catId = this.data.DataAccess.GetCatheterId(CatheterID & (~EngineeringCatheterSignature));
                this.data.DataAccess.AddCatheterInformation(CatheterSerialNumber, CatheterLot, 1, CatheterExpirationDate, CatheterLastUseDate, NumberOfInjections, catId, true, CatheterID);

              }

              IsCatheterValid = true;
              IsUsedForEngineering = true;
              goto BypassValidation;
            }

            // we are using exception datetime data because there is nodate time in C
            CatheterExpirationMonth = data[4];
            CatheterExpirationDay = data[5];
            CatheterExpirationYear = CanBusMessageConverter.ConverteInfoData(data, 6);

            CatheterLastUseDate = new DateTime(CatheterLastUseYear, CatheterLastUseMonth, CatheterLastUseDay, CatheterLastUseHour, 0, 0, 0);

            try
            {
                CatheterExpirationDate = new DateTime(CatheterExpirationYear, CatheterExpirationMonth, CatheterExpirationDay);
            }
            catch (Exception ex)
            {
                ex.ToString();
                CatheterExpirationDate = inavalidCatheterExpirationDate;
            }

            //first we have to verify if these catheter is already inthe database:
            CatheterInformation catheterInformation = null;
            if (this.data != null && this.data.DataAccess != null && ((EngineeringCatheterSignature & CatheterID) != EngineeringCatheterSignature))
            {
                catheterInformation = this.data.DataAccess.GetatheterInformationsAccordingToSerialNumberAndLot(CatheterSerialNumber, CatheterLot, CatheterID, false);
            }

            if (catheterInformation != null && this.catheterValidator != null)
            {
                IsCatheterValid = this.catheterValidator.ValidateCatheterWhenAlreadyUsed(CatheterID, CatheterExpirationDate, CatheterLastUseDate, catheterInformation.CatheterExpirationDate, catheterInformation.LastUseDate);
            }
            else
            {
                if (this.catheterValidator != null)
                {
                    IsCatheterValid = this.catheterValidator.ValidateCatheter(CatheterID, CatheterExpirationDate, CatheterLastUseDate);

                    NumberOfInjections = 0;

                    if (IsCatheterValid && this.data != null && this.data.DataAccess != null && ((EngineeringCatheterSignature & CatheterID) != EngineeringCatheterSignature))
                    {
                      try
                      {
                        int catId = this.data.DataAccess.GetCatheterId(CatheterID);
                        this.data.DataAccess.AddCatheterInformation(CatheterSerialNumber, CatheterLot, 1, CatheterExpirationDate, CatheterLastUseDate, NumberOfInjections, catId, false, catId);
                      }
                      catch (Exception ex)
                      {
                        LogException(ex);
                      }
                    }
                }
            }

#endregion

        BypassValidation:

            if (IsCatheterValid)
            {
              //here it is important that all catheter use a serial number
              initializeRegistersAccordingToCatheterID(CatheterID);
              catheterCommunicationData = communicationData;
              catheterEventId = (uint)iD;
            }

            SendRequestedData(communicationData, communicationData.CanBusOneEventArgs.Id, (uint)iD, true, IsCatheterValid);
        }

        /// <summary>
        /// Initializes the registers according to a catheter ID
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheterID">An integer representing a Catheter Id.</param>
        /// <id>SF-SDS-0104</id>
        private void initializeRegistersAccordingToCatheterID(int catheterID)
        {
#region patient region

            //Target Ballon pressure
            CatheterType catheterType = this.data.DataAccess.GetCatheterAccordingToCatheterId(catheterID & (~EngineeringCatheterSignature));
            //this.console.Balloon.TargetBalloonPressure = catheterType.TargetBalloonPressure;

            //Serial Number and Number Of Injections
            try
            {
                this.console.Catheter.SerialNumber = CatheterSerialNumber;
                this.console.Catheter.NumberOfInjections = NumberOfInjections;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            //Catheter Expiration Date
            if (CatheterExpirationDate != null)
            {
                this.console.Catheter.CatheterExpirationYear = CatheterExpirationDate.Year;
                this.console.Catheter.CatheterExpirationMonth = CatheterExpirationDate.Month;
                this.console.Catheter.CatheterExpirationDay = CatheterExpirationDate.Day;
            }

            //Last Use Date
            if (CatheterLastUseDate != null)
            {
                this.console.Catheter.CatheterLastUseYear = CatheterLastUseDate.Year;
                this.console.Catheter.CatheterLastUseMonth = CatheterLastUseDate.Month;
                this.console.Catheter.CatheterLastUseDay = CatheterLastUseDate.Day;
                this.console.Catheter.CatheterLastUseHour = CatheterLastUseDate.Hour;
            }

            List<PMCRegisterValue> pMCRegisterValues = this.Data.DataAccess.GetPMCRegisterValuesAccordingToCatheterID(catheterType.ID);


#region DAS balloon 

            if ((catheterID & (~EngineeringCatheterSignature)) == (int)Enumeration.CatheterType.Plus)
            {
                ChangeBalloonTypeFSM.CatheterType = Enumeration.CatheterType.Plus;
                IsSystemUsingDASBalloon = true;


            }
            else if ((catheterID & (~EngineeringCatheterSignature)) == (int)Enumeration.CatheterType.ID28mm)
            {
                ChangeBalloonTypeFSM.CatheterType = Enumeration.CatheterType.ID28mm;
                IsSystemUsingDASBalloon = false;
            }

#endregion

            // Initialize Patient Micro Controller Register. the traget ballon pressure is state independent.
            foreach (PMCRegisterValue pMCRegisterValue in pMCRegisterValues)
            {
                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;
                switch (pMCRegisterValue.StateID)
                {
                    case 1:
                        mid = MessageStateId.CAN_ID_STATE_IDLE;
                        break;

                    case 2:
                        mid = MessageStateId.CAN_ID_STATE_READY;
                        break;

                    case 3:
                        mid = MessageStateId.CAN_ID_STATE_INFLATION;
                        break;

                    case 4:
                        mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                        break;

                    case 5:
                        mid = MessageStateId.CAN_ID_STATE_ABLATION;
                        break;

                    case 6:
                        mid = MessageStateId.CAN_ID_STATE_THAWING;
                        break;

                    case 7:
                        mid = MessageStateId.CAN_ID_STATE_EXCEPTION;
                        break;
                }
                //CP1

                if (pMCRegisterValue.StateID != 7)
                {
                    this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = pMCRegisterValue.CP1PressureThresholdHighLimit;

                    if (pMCRegisterValue.StateID == 1)
                    {
                        this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = pMCRegisterValue.CP1PressureLowRangeLimit;
                    }
                    else
                    {
                        double localPressureLowRangeLimit = outerBalloonPressureThreshold.GetThershold(PT3Reading);

                        if (localPressureLowRangeLimit < -12)
                        {
                            this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = -12;
                        }
                        else if (localPressureLowRangeLimit > -6)
                        {
                            this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = -6;
                        }

                        else if (!(localPressureLowRangeLimit < -12) && !(localPressureLowRangeLimit > -6))
                        {
                            this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = localPressureLowRangeLimit;
                        }
                    }

                    this.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = pMCRegisterValue.CP1PressureHighRangeLimit;

                    //CP2
                    this.Console.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = pMCRegisterValue.CP2PressureThresholdHighLimit;
                    this.Console.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = pMCRegisterValue.CP2PressureLowRangeLimit;
                    this.Console.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = pMCRegisterValue.CP2PressureHighRangeLimit;

                    //TC1
                    this.console.ThermocoupleOneValueAccordingToTheStateMachine[mid].ThawingTemperature = pMCRegisterValue.TC1ThawingTemperature;

                    //Thawing Temperature Set Point
                    this.console.ThermocoupleOneValueAccordingToTheStateMachine[mid].ThawingTemperatureSetPoint = pMCRegisterValue.ThawingTemperatureSetPoint;
                    this.ThawingTemperatureSetPoint = pMCRegisterValue.ThawingTemperatureSetPoint;

                    //Patient Micro Controller PID
                    this.console.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].PGain = pMCRegisterValue.Pgain;
                    this.console.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].IGain = pMCRegisterValue.Igain;
                    this.console.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].DGain = pMCRegisterValue.Dgain;
                    this.console.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].Offset = pMCRegisterValue.Offset;

                    //Target Balloon Pressure
                    this.console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[mid].TargetBalloonPressure = pMCRegisterValue.TargetBalloonPressure;

                    //Blood detector
                    this.console.BloodDetectorValueAccordingToTheStateMachine[mid].LowerBloodThreshold = pMCRegisterValue.LowerBloodThreshold;
                    this.console.BloodDetectorValueAccordingToTheStateMachine[mid].UpperBloodThreshold = pMCRegisterValue.UpperBloodThreshold;


                }
            }

#endregion patient region

#region Central micro controller

            List<CMCRegisterValue> cMCRegisterValues = this.Data.DataAccess.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

            foreach (CMCRegisterValue cMCRegisterValue in cMCRegisterValues)
            {
                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;
                switch (cMCRegisterValue.StateID)
                {
                    case 1:
                        mid = MessageStateId.CAN_ID_STATE_IDLE;
                        break;

                    case 2:
                        mid = MessageStateId.CAN_ID_STATE_READY;
                        break;

                    case 3:
                        mid = MessageStateId.CAN_ID_STATE_INFLATION;
                        break;

                    case 4:
                        mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                        break;

                    case 5:
                        mid = MessageStateId.CAN_ID_STATE_ABLATION;
                        break;

                    case 6:
                        mid = MessageStateId.CAN_ID_STATE_THAWING;
                        break;

                    case 7:
                        mid = MessageStateId.CAN_ID_STATE_EXCEPTION;
                        break;
                }

                if (cMCRegisterValue.StateID != 7)
                {
                    //Target Injection Flow
                    this.console.InjectionFlowValueAccordingToTheStateMachine[mid].TargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;

                    //target Injection Pressure
                    // TO DO : change the EDMX file and update the code
                    //this.console.InjectionPressureValueAccordingToTheStateMachine[mid].TargetInjectionPressure = cMCRegisterValue

                    //Central Micro Controller PID CentralMicroControllerPIDValueAccordingToTheStateMachine
                    this.console.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].PGain = cMCRegisterValue.PGain;
                    this.console.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].IGain = cMCRegisterValue.IGain;
                    this.console.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].DGain = cMCRegisterValue.DGain;
                    this.console.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].Offset = cMCRegisterValue.Offset;

                    //PT1
                    this.console.PressureTransducerOneValueAccordingToTheStateMachine[mid].TankPressureLow = cMCRegisterValue.PT1TankPressureLow;
                    this.console.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT1PressureThresholdHighLimit;
                    this.console.PressureTransducerOneValueAccordingToTheStateMachine[mid].TankPressureTooHigh = cMCRegisterValue.PT1TankPressureTooHigh;
                    this.console.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT1PressureLowRangeLimit;
                    this.console.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT1PressureHighRangeLimit;

                    //PT2
                    this.console.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT2PressureThresholdHighLimit;
                    this.console.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT2PressureLowRangeLimit;
                    this.console.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT2PressureHighRangeLimit;

                    //PT3
                    this.console.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT3PressureThresholdHighLimit;
                    this.console.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT3PressureLowRangeLimit;
                    this.console.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT3PressureHighRangeLimit;

                    //PT4
                    this.console.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT4PressureThresholdHighLimit;
                    this.console.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT4PressureLowRangeLimit;
                    this.console.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT4PressureHighRangeLimit;

                    //TS1
                    this.console.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureThresholdHighLimit = cMCRegisterValue.TS1TemperatureThresholdHighLimit;
                    this.console.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureLowRangeLimit = cMCRegisterValue.TS1TemperatureLowRangeLimit;
                    this.console.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureHighRangeLimit = cMCRegisterValue.TS1TemperatureHighRangeLimit;

                    //FM1
                    this.console.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterThresholLowlimit = cMCRegisterValue.FM1FlowMeterThresholLowlimit;
                    this.console.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterThresholHighlimit = cMCRegisterValue.FM1FlowMeterThresholHighlimit;
                    this.console.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterLowRangeLimit = cMCRegisterValue.FM1FlowMeterLowRangeLimit;
                    this.console.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterHighRangelimit = cMCRegisterValue.FM1FlowMeterHighRangelimit;

                    //PS1
                    this.console.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PS1PressureThresholdHighLimit;
                    this.console.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PS1PressureLowRangeLimit;
                    this.console.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PS1PressureHighRangeLimit;

                    //PS2
                    this.console.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PS2PressureThresholdHighLimit;
                    this.console.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PS2PressureLowRangeLimit;
                    this.console.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PS2PressureHighRangeLimit;

                    //LC1
                    // we need to add the metal tank
                    double localMetalTank = this.Console.Tank.MetalWeight;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdWarning = cMCRegisterValue.LC1LoadCellThresholdWarning + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdFail = cMCRegisterValue.LC1LoadCellThresholdFail + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellLowRangeLimit = cMCRegisterValue.LC1LoadCellLowRangeLimit + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellHighRangeLimit = cMCRegisterValue.LC1LoadCellHighRangeLimit + localMetalTank;

                    //Target Injection Flow, Target Injection Pressure
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionPressure = cMCRegisterValue.TargetInjectionPressure;

                    if (mid == MessageStateId.CAN_ID_STATE_ABLATION)
                        baseTargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;

                    //Low Flow Value
                    this.console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionLowFlow = cMCRegisterValue.LowFlow;
                }
            }

#endregion Central micro controller

#region Ramp up and down Time


            this.InitializeDASBalloonRegisters();

#endregion
        }

        /// <summary>
        /// Function that initializes the load cell registers value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void initializeLoadCellRegisters()
        {


            List<CMCRegisterValue> cMCRegisterValues = this.Data.DataAccess.GetCMCRegisterValues();



            foreach (CMCRegisterValue cMCRegisterValue in cMCRegisterValues)
            {
                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;
                switch (cMCRegisterValue.StateID)
                {
                    case 1:
                        mid = MessageStateId.CAN_ID_STATE_IDLE;
                        break;

                    case 2:
                        mid = MessageStateId.CAN_ID_STATE_READY;
                        break;

                    case 3:
                        mid = MessageStateId.CAN_ID_STATE_INFLATION;
                        break;

                    case 4:
                        mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                        break;

                    case 5:
                        mid = MessageStateId.CAN_ID_STATE_ABLATION;
                        break;

                    case 6:
                        mid = MessageStateId.CAN_ID_STATE_THAWING;
                        break;

                    case 7:
                        mid = MessageStateId.CAN_ID_STATE_EXCEPTION;
                        break;
                }

                if (cMCRegisterValue.StateID != 7)
                {

                    //LC1
                    // we need to add the metal tank
                    double localMetalTank = this.Console.Tank.MetalWeight;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdWarning = cMCRegisterValue.LC1LoadCellThresholdWarning + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdFail = cMCRegisterValue.LC1LoadCellThresholdFail + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellLowRangeLimit = cMCRegisterValue.LC1LoadCellLowRangeLimit + localMetalTank;
                    this.console.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellHighRangeLimit = cMCRegisterValue.LC1LoadCellHighRangeLimit + localMetalTank;

                }
            }



        }

        /// <summary>
        /// Function that initializes the value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeDASBalloonRegisters()
        {
            MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

            List<BalloonParameters> ballonParameters = this.Data.DataAccess.GetDASBallonParameters();

            foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
            {

                if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                {
                    int state = 0;
                    state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                    switch (state)
                    {
                        case 1:
                            mid = MessageStateId.CAN_ID_STATE_IDLE;
                            break;

                        case 2:
                            mid = MessageStateId.CAN_ID_STATE_READY;
                            break;

                        case 3:
                            mid = MessageStateId.CAN_ID_STATE_INFLATION;
                            break;

                        case 4:
                            mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                            break;

                        case 5:
                            mid = MessageStateId.CAN_ID_STATE_ABLATION;
                            break;

                        case 6:
                            mid = MessageStateId.CAN_ID_STATE_THAWING;
                            break;

                    }

                    BalloonParameters _ballonParameters = ballonParameters[state - 1];


                    //Ballon Rum up and ramp dow timing 
                    this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep = (double)_ballonParameters.RampUpTimeByStep;
                    this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue = (double)_ballonParameters.PressureRampUpValue;
                    this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep = (double)_ballonParameters.RampDownTimeByStep;
                    this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue = (double)_ballonParameters.PressureRampDownValue;

                    this.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].DASLowFlow = _ballonParameters.DASLowFlow;


                }
            }
        }


        //public void InitializeDASBalloonRegisters()
        //{
        //    MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

        //    List<BallonParameters> ballonParameters = this.Data.DataAccess.GetDASBallonParameters();

        //    foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
        //    {

        //        if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
        //        {
        //            int state = 0;
        //            state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

        //            switch (state)
        //            {
        //                case 1:
        //                    mid = MessageStateId.CAN_ID_STATE_IDLE;
        //                    break;

        //                case 2:
        //                    mid = MessageStateId.CAN_ID_STATE_READY;
        //                    break;

        //                case 3:
        //                    mid = MessageStateId.CAN_ID_STATE_INFLATION;
        //                    break;

        //                case 4:
        //                    mid = MessageStateId.CAN_ID_STATE_TRANSITION;
        //                    break;

        //                case 5:
        //                    mid = MessageStateId.CAN_ID_STATE_ABLATION;
        //                    break;

        //                case 6:
        //                    mid = MessageStateId.CAN_ID_STATE_THAWING;
        //                    break;

        //            }

        //            foreach (BallonParameters _ballonParameters in ballonParameters)
        //            {

        //                InflateDeflateBalloonModel.MinimumPressureSetpoint = (double)_ballonParameters.MinimumPressureSetpoint;
        //                InflateDeflateBalloonModel.MaximumPressureSetPoint = (double)_ballonParameters.MaximumPressureSetPoint;

        //                InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow = (double)_ballonParameters.MaximumFlowSetPoint;

        //                InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow = (double)_ballonParameters.MinimumFlowSetPoint;

        //                //Ballon Rum up and ramp dow timing 
        //                this.console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep = (double)_ballonParameters.RampUpTimeByStep;
        //                this.console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue = (double)_ballonParameters.PressureRampUpValue;
        //                this.console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep = (double)_ballonParameters.RampDownTimeByStep;
        //                this.console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue = (double)_ballonParameters.PressureRampDownValue;
        //            }
        //        }
        //    }
        //}



        /// <summary>
        /// Gets the CMCU Status Error
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="cMCUSystemStatusError">An integer representing the CMCU System Status Error.</param>
        public void GetCMCUStatusError(Int64 cMCUSystemStatusError)
        {
            if (!IsCanOneWasInError && !IsCanTwoInError)
            {
                string codeError = string.Empty;
                string codeWarning = string.Empty;

                if (ErrorIdMessageAndSolutionList?.Count != 0)
                    ErrorIdMessageAndSolutionList.Clear();

                IsCPLDLatching = true;

#region Errors

                if ((!IsCMCUExceptionType5 && (cMCUSystemStatusError & (Int64)CMCUStatusError.ExceptionType5) == (Int64)CMCUStatusError.ExceptionType5) && !Console.GUIInMaintenanceMode)
                {

                    IsCMCUExceptionType5 = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID110, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 2- 10000000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;

                    if (codeError != string.Empty && codeError != cmcuPreviousError && !WarningMessageManager.SearchMessage(codeError))
                    {
                        if (ResetCMCUErrorStopWatchDisconnection.IsRunning)
                        {
                            if (ResetCMCUErrorStopWatchDisconnection.ElapsedMilliseconds > errorResetingMaximumTime)
                            {
                                ResetCMCUErrorStopWatchDisconnection.Reset();
                                IsSystemRested = false;
                                cmcuPreviousError = string.Empty;
                                cmcuPreviousWarning = string.Empty;
                                GenericError = string.Empty;
                            }
                        }

                        DisplayException5Message();
                    }

                }
                else
                {
                    // These code will be used in the futur
                    IsCMCUExceptionType5 = false;
                }

                //CPLD
                if (!IsCMCUCPLDWatchDogTimerError && (cMCUSystemStatusError & (Int64)CMCUStatusError.CPLDWatchDogTimerError) == (Int64)CMCUStatusError.CPLDWatchDogTimerError)
                {
                    IsCMCUCPLDWatchDogTimerError = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.CPLDWatchDogTimerError, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00000001 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUCPLDWatchDogTimerError = false;
                }


                if (!IsCMCUTwoMultiplexReadingDoesNotMatch && (cMCUSystemStatusError & (Int64)CMCUStatusError.TwoMultiplexReadingDoesNotMatch) == (Int64)CMCUStatusError.TwoMultiplexReadingDoesNotMatch)
                {
                    IsCMCUTwoMultiplexReadingDoesNotMatch = true;
                    codeError += " CMCUTwoMultiplexReadingDoesNotMatch + ";
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.TwoMultiplexReadingDoesNotMatch, (int)Enumeration.ErrorTypes.CMCU));
                }
                else
                {
                    IsCMCUTwoMultiplexReadingDoesNotMatch = false;
                }

                if (!IsCMCUFlowTooHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.FlowTooHigh) == (Int64)CMCUStatusError.FlowTooHigh)
                {
                    IsCMCUFlowTooHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.FlowTooHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00000004 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUFlowTooHigh = false;
                }

                if (!IsCMCUFlowTooLow && (cMCUSystemStatusError & (Int64)CMCUStatusError.FlowTooLow) == (Int64)CMCUStatusError.FlowTooLow)
                {
                    IsCMCUFlowTooLow = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.FlowTooLow, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00000008 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUFlowTooLow = false;
                }

                if (!IsCMCUFlowReadingOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.FlowReadingOutOfRange) == (Int64)CMCUStatusError.FlowReadingOutOfRange)
                {
                    if (!IsCanOneWasInError)
                    {
                        IsCMCUFlowReadingOutOfRange = true;
                        CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.FlowReadingOutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                        codeError += " Error 2- 00000010 " + CmcuTupleError.Item2;
                        IsCPLDLatching = false;
                        ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                    }
                }
                else
                {
                    IsCMCUFlowReadingOutOfRange = false;
                }

                if (!IsCMCULoadCellWeightFail && (cMCUSystemStatusError & (Int64)CMCUStatusError.LoadCellWeightFail) == (Int64)CMCUStatusError.LoadCellWeightFail)
                {
                    IsCMCULoadCellWeightFail = true;
                    IsUserAllowedToChangeTank = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.LoadCellWeightFail, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00000040 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCULoadCellWeightFail = false;
                    IsUserAllowedToChangeTank = false;

                }

                if (!IsCMCULoadCellReadingOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.LoadCellReadingOutOfRange) == (Int64)CMCUStatusError.LoadCellReadingOutOfRange)
                {
                    IsCMCULoadCellReadingOutOfRange = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.LoadCellReadingOutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00000080 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCULoadCellReadingOutOfRange = false;

                }

                if (!IsCMCUPressurePT1InTankReadingOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.PressurePT1InTankReadingOutOfRange) == (Int64)CMCUStatusError.PressurePT1InTankReadingOutOfRange)
                {
                    IsCMCUPressurePT1InTankReadingOutOfRange = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PressurePT1InTankReadingOutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                    //The falg was used for range
                    //codeError += " Error 2- 00000800 Tank pressure reading out of range.(PT1) + ";
                    codeError += " Error 2- 00000800 " + CmcuTupleError.Item2;

                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUPressurePT1InTankReadingOutOfRange = false;
                }

                if (!IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh) == (Int64)CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh)
                {
                    IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00001000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh = false;
                }

                if (!IsCMCUPT2ReadingOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.PT2ReadingOutOfRange) == (Int64)CMCUStatusError.PT2ReadingOutOfRange)
                {
                    IsCMCUPT2ReadingOutOfRange = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.PT2ReadingOutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += "  Error 2- 00002000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUPT2ReadingOutOfRange = false;
                }

                if (!IsCMCUReturnPressurePT3TooHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.ReturnPressurePT3TooHigh) == (Int64)CMCUStatusError.ReturnPressurePT3TooHigh)
                {
                    IsCMCUReturnPressurePT3TooHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.ReturnPressurePT3TooHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00004000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUReturnPressurePT3TooHigh = false;
                }

                if (!IsCMCUReturnPressurePT3OutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.ReturnPressurePT3OutOfRange) == (Int64)CMCUStatusError.ReturnPressurePT3OutOfRange)
                {
                    IsCMCUReturnPressurePT3OutOfRange = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.ReturnPressurePT3OutOfRange, (int)Enumeration.ErrorTypes.CMCU);

                    //The falg was used for range
                    //codeError += "  Error 2- 00008000 Return pressure reading out of range.(PT3) + ";

                    codeError += " Error 2- 00008000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUReturnPressurePT3OutOfRange = false;
                }

                if (!IsCMCUVacuumPressurePT4TooHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.VacuumPressurePT4TooHigh) == (Int64)CMCUStatusError.VacuumPressurePT4TooHigh)
                {
                    IsCMCUVacuumPressurePT4TooHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.VacuumPressurePT4TooHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00010000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUVacuumPressurePT4TooHigh = false;
                }

                if (!IsCMCUVacuumPressurePT4OutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.VacuumPressurePT4OutOfRange) == (Int64)CMCUStatusError.VacuumPressurePT4OutOfRange)
                {
                    IsCMCUVacuumPressurePT4OutOfRange = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.VacuumPressurePT4OutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                    //The falg was used for range
                    //codeError += " Error 2- 00020000 Vacuum level out of range. (PT4) + ";
                    codeError += " Error 2- 00020000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUVacuumPressurePT4OutOfRange = false;
                }


                if (!IsCMCUSubCoolerTemperatureOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.SubCoolerTemperatureOutOfRange) == (Int64)CMCUStatusError.SubCoolerTemperatureOutOfRange)
                {
                    IsCMCUSubCoolerTemperatureOutOfRange = true;

                    // codeError += " Error 2- 00080000 Subcooler temperature out of range +";
                    IsCPLDLatching = false;
                    // ErrorIdMessageAndSolutionList.Add(Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.SubCoolerTemperatureOutOfRange, (int)Enumeration.ErrorTypes.CMCU));
                }
                else
                {
                    IsCMCUSubCoolerTemperatureOutOfRange = false;
                }

                if (!IsCMCUInjectionVentPressureIsHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.InjectionVentPressureIsHigh) == (Int64)CMCUStatusError.InjectionVentPressureIsHigh)
                {
                    IsCMCUInjectionVentPressureIsHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.InjectionVentPressureIsHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00100000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUInjectionVentPressureIsHigh = false;
                }

                if (!IsCMCUScavengingPressureIsHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.ScavengingPressureIsHigh) == (Int64)CMCUStatusError.ScavengingPressureIsHigh)
                {
                    IsCMCUScavengingPressureIsHigh = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.ScavengingPressureIsHigh, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 00400000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUScavengingPressureIsHigh = false;
                }
                if (!IsCMCUPressureInTankIsHighFanToBeOn && (cMCUSystemStatusError & (Int64)CMCUStatusError.PressureInTankIsHighFanToBeOn) == (Int64)CMCUStatusError.PressureInTankIsHighFanToBeOn)
                {
                  if (IsWindowLoaded)
                  {
                    IsCMCUPressureInTankIsHighFanToBeOn = true;
                    CmcuTupleError = PressureInTankIsHighFanToBeOnSolution;
                    codeError += " Error- 00000100 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    IsCMCUPressureInTankIsHighFanToBeOn = true;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                  }
                }
                else
                {
                  IsCMCUPressureInTankIsHighFanToBeOn = false;
                }

                if ((cMCUSystemStatusError & (Int64)CMCUStatusError.FootSwitchLock) == (Int64)CMCUStatusError.FootSwitchLock)
                {
                    IsFootSwitchLocked = true;

                }
                else
                {
                    IsFootSwitchLocked = false;

                }

                //Vein Isolation VeinIsolated
                if ((cMCUSystemStatusError & (Int64)CMCUStatusError.VeinIsolated) == (Int64)CMCUStatusError.VeinIsolated)
                {
                    IsVeinIsolated = true;
                }
                else
                {
                    IsVeinIsolated = false;
                }


                // CMCU self test
                if (!IsCMCUSelfTestFail && (cMCUSystemStatusError & (Int64)CMCUStatusError.SelfTestFail) == (Int64)CMCUStatusError.SelfTestFail)
                {
                    IsCMCUSelfTestFail = true;
                    CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.SelfTestFail, (int)Enumeration.ErrorTypes.CMCU);
                    codeError += " Error 2- 04000000 " + CmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                    IsCMCUSelfTestFail = false;
                }

                if ((cMCUSystemStatusError & (Int64)CMCUStatusError.CatheterTubeConnected) == (Int64)CMCUStatusError.CatheterTubeConnected)
                {
                    IsCatheterTubeConnected = true;
                }
                else
                {
                    IsCatheterTubeConnected = false;
                }

                if ((cMCUSystemStatusError & (Int64)CMCUStatusError.CMCUReady) == (Int64)CMCUStatusError.CMCUReady)
                {
                    IsCMCUReady = true;
                }
                else
                {
                    IsCMCUReady = false;
                }

#endregion
#region Warnning

                if (!IsCMCULoadCellWeightWarning && (cMCUSystemStatusError & (Int64)CMCUStatusError.LoadCellWeightWarning) == (Int64)CMCUStatusError.LoadCellWeightWarning)
                {
                    if (IsWindowLoaded)
                    {
                        IsCMCULoadCellWeightWarning = true;
                        CmcuTupleError = LoadCellWeightWarningSolution;
                        codeWarning += " Warning- 00000020 " + CmcuTupleError.Item2;
                        IsCPLDLatching = false;
                        IsUserAllowedToChangeTank = true;
                        ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                    }
                }
                else
                {
                    IsCMCULoadCellWeightWarning = false;
                    IsUserAllowedToChangeTank = false;
                    //if (GasState != Enumeration.TankWeight.THE_TANK_WEIGHT_IS_TOO_LOW && GasState != Enumeration.TankWeight.THE_TANK_WEIGHT_IS_OF_BOUNDS)
                    //{
                    //    GasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;
                    //}
                }

                if (!IsCMCUInjectionVertPressureOutOfRange && (cMCUSystemStatusError & (Int64)CMCUStatusError.InjectionVentPressureOutOfRange) == (Int64)CMCUStatusError.InjectionVentPressureOutOfRange)
                {
                  IsCMCUInjectionVertPressureOutOfRange = true;
                  CmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)CMCUStatusError.InjectionVentPressureOutOfRange, (int)Enumeration.ErrorTypes.CMCU);
                  codeWarning += " Warning- 00200000 " + CmcuTupleError.Item2;
                  IsCPLDLatching = false;
                  ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                }
                else
                {
                  IsCMCUInjectionVertPressureOutOfRange = false;
                }
                if (!IsCMCUPressurePT1InTankIsLow && (cMCUSystemStatusError & (Int64)CMCUStatusError.PressurePT1InTankIsLow) == (Int64)CMCUStatusError.PressurePT1InTankIsLow)
                {
                    if (IsWindowLoaded)
                    {
                        IsCMCUPressurePT1InTankIsLow = true;
                        CmcuTupleError = PressurePT1InTankIsLowSolution;
                        codeWarning += " Warning- 00000200 " + CmcuTupleError.Item2;
                        IsCPLDLatching = false;

                        ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                    }
                }
                else
                {
                    IsCMCUPressurePT1InTankIsLow = false;
                }

                if (!IsCMCUPressurePT1InTankIsTooHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.PressurePT1InTankIsTooHigh) == (Int64)CMCUStatusError.PressurePT1InTankIsTooHigh)
                {
                    if (IsWindowLoaded)
                    {
                        IsCMCUPressurePT1InTankIsTooHigh = true;
                        CmcuTupleError = PressurePT1InTankIsHighSolution;
                        codeWarning += " Warning- 00000400 " + CmcuTupleError.Item2;
                        IsCPLDLatching = false;
                        ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                    }
                }
                else
                {
                  IsCMCUPressurePT1InTankIsTooHigh = false;
                }


                if (!IsCMCUSubCoolerTemperatureIsHigh && (cMCUSystemStatusError & (Int64)CMCUStatusError.SubCoolerTemperatureIsHigh) == (Int64)CMCUStatusError.SubCoolerTemperatureIsHigh)
                {
                    if (IsWindowLoaded)
                    {
                        IsCMCUSubCoolerTemperatureIsHigh = true;
                        CmcuTupleError = SubCoolerTemperatureIsHighSolution;
                        codeWarning += " Warning- 00040000 " + CmcuTupleError.Item2;
                        IsCPLDLatching = false;
                        ErrorIdMessageAndSolutionList.Add(CmcuTupleError);
                    }
                }
                else
                {
                    IsCMCUSubCoolerTemperatureIsHigh = false;
                }

                ManageGasSate(cMCUSystemStatusError);

#endregion

                // These Code Is only to help Validation Testing. it have to be changed:
                if ((codeError.Contains("Error") || codeWarning.Contains("Warning")) && !IsCPLDLatching)
                {
                    try
                    {
                        if (ResetCMCUErrorStopWatchDisconnection.IsRunning)
                        {
                            if (ResetCMCUErrorStopWatchDisconnection.ElapsedMilliseconds > errorResetingMaximumTime)
                            {
                                ResetCMCUErrorStopWatchDisconnection.Reset();
                                IsSystemRested = false;
                                cmcuPreviousError = string.Empty;
                                cmcuPreviousWarning = string.Empty;
                                GenericError = string.Empty;
                            }
                        }
                        else
                        {
                            if (((codeError != string.Empty && codeError != cmcuPreviousError && !WarningMessageManager.SearchMessage(codeError)) || (codeWarning != string.Empty && !WarningMessageManager.SearchMessage(codeWarning))) && !Console.GUIInMaintenanceMode)
                            {
                                if (IsWindowLoaded)
                                {
                                    ResetCMCUErrorStopWatchDisconnection.Start();
                                    if (CanChangeTank)
                                        ResetSystem();
                                    else
                                    {
                                        if (codeError.Contains("Error"))
                                            GenericError = codeError;
                                        else
                                        {
                                            //GenericError = codeWarning;
                                            IsSytemInWarning = true;
                                        }

                                        //foreach (Tuple<long, string, string, string> er in ErrorIdMessageAndSolutionList)
                                        //{

                                        //    this.Data.DataAccess.AddErrorLog(er.Item2, DateTime.Now, er.Item1, (int)Enumeration.ErrorTypes.CMCU, ConsoleVersionID, CatheterID, (int)SystemState, CurrentUser.Id, IsCatheterConnected, (RemoteControlFirmware == 0 ? false : true));
                                        //}


                                        List<Tuple<long, string, string, string>> copyOfErrorIdMessageAndSolutionList = new List<Tuple<long, string, string, string>>(ErrorIdMessageAndSolutionList);
                                        Task.Delay(4000).ContinueWith(t => SavErrors((int)Enumeration.ErrorTypes.CMCU, copyOfErrorIdMessageAndSolutionList, PreviousSystemState));

                                        DisplayErrorMessage(codeError, codeWarning, IsSytemInWarning, Enumeration.ErrorTypes.CMCU);

                                    }

                                    cmcuPreviousError = codeError;
                                    cmcuPreviousWarning = codeWarning;

                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        exception.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the PMCU System Status Error Code
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="pMCUS ystemStatusErrorCode">An integer representing the PMCU System Status Error Code.</param>
        private void GetPMCUStatusError(Int64 pMCUSystemStatusErrorCode)
        {
            if (!IsCanOneWasInError && !IsCanTwoInError)
            {
                string codeError = string.Empty;
                string codeWarning = string.Empty;

                if (ErrorIdMessageAndSolutionList?.Count != 0)
                    ErrorIdMessageAndSolutionList.Clear();

                IsCPLDLatching = true;

                #region error
                if (!IsPMCUCPLDWatchDogTimerError && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.CPLDWatchDogTimerError) == (Int64)PMCUStatusError.CPLDWatchDogTimerError)
                {
                    IsPMCUCPLDWatchDogTimerError = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.CPLDWatchDogTimerError, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000001 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsPMCUCPLDWatchDogTimerError = false;
                }

                if (!IsInnerBalloonPressureTooHigh && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.InnerBalloonPressureTooHigh) == (Int64)PMCUStatusError.InnerBalloonPressureTooHigh)
                {
                    IsInnerBalloonPressureTooHigh = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.InnerBalloonPressureTooHigh, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000004 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsInnerBalloonPressureTooHigh = false;
                }

                if (!IsInnerBalloonPressureTooLow && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.InnerBalloonPressureTooLow) == (Int64)PMCUStatusError.InnerBalloonPressureTooLow)
                {
                    IsInnerBalloonPressureTooLow = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.InnerBalloonPressureTooLow, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000008 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsInnerBalloonPressureTooLow = false;
                }

                if (!IsOuterBalloonPressureTooHigh && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.OuterBalloonPressureTooHigh) == (Int64)PMCUStatusError.OuterBalloonPressureTooHigh)
                {
                    IsOuterBalloonPressureTooHigh = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.OuterBalloonPressureTooHigh, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000020 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsOuterBalloonPressureTooHigh = false;
                }

                if (!IsOuterBalloonPressureReadingOutOrRange && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.OuterBalloonPressureReadingOutOrRange) == (Int64)PMCUStatusError.OuterBalloonPressureReadingOutOrRange)
                {
                    IsOuterBalloonPressureReadingOutOrRange = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.OuterBalloonPressureReadingOutOrRange, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000040 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsOuterBalloonPressureReadingOutOrRange = false;
                }

                if (!IsBalloonTipPressureTooHigh && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BalloonTipPressureTooHigh) == (Int64)PMCUStatusError.BalloonTipPressureTooHigh)
                {
                    IsBalloonTipPressureTooHigh = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BalloonTipPressureTooHigh, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000080 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBalloonTipPressureTooHigh = false;
                }

                if (!IsBalloonTipPressureTooLow && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BalloonTipPressureTooLow) == (Int64)PMCUStatusError.BalloonTipPressureTooLow)
                {
                    IsBalloonTipPressureTooLow = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BalloonTipPressureTooLow, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000100 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBalloonTipPressureTooLow = false;
                }

                if (!IsBalloonTipPressurePeadingOutOfRange && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BalloonTipPressurePeadingOutOfRange) == (Int64)PMCUStatusError.BalloonTipPressurePeadingOutOfRange)
                {
                    IsBalloonTipPressurePeadingOutOfRange = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BalloonTipPressurePeadingOutOfRange, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000200 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBalloonTipPressurePeadingOutOfRange = false;
                }

                if (!IsThawingTemperatureTooHigh && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.ThawingTemperatureTooHigh) == (Int64)PMCUStatusError.ThawingTemperatureTooHigh)
                {
                    IsThawingTemperatureTooHigh = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.ThawingTemperatureTooHigh, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000400 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsThawingTemperatureTooHigh = false;
                }

                if (!IsThawingTemperatureTooLow && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.ThawingTemperatureTooLow) == (Int64)PMCUStatusError.ThawingTemperatureTooLow)
                {
                    IsThawingTemperatureTooLow = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.ThawingTemperatureTooLow, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 00000800 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsThawingTemperatureTooLow = false;
                }
                #endregion error
          
#region add code

                if (!IsBalloonTemperatureTooHigh && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BalloonTemperatureTooHigh) == (Int64)PMCUStatusError.BalloonTemperatureTooHigh)
                {
                    IsBalloonTemperatureTooHigh = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BalloonTemperatureTooHigh, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 0001000 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBalloonTemperatureTooHigh = false;
                }

                // Blood Detected In Catheter, Please Replace The Catheter
                if (!IsBloodDetectedInCatheter && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BloodDetectedInCatheter) == (Int64)PMCUStatusError.BloodDetectedInCatheter)
                {
                    IsBloodDetectedInCatheter = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BloodDetectedInCatheter, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 0004000 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBloodDetectedInCatheter = false;
                }


                //Wire problem in the catheter
                if (!IsBloodDetectedInCatheter && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BloodDetectorOpenWires) == (Int64)PMCUStatusError.BloodDetectorOpenWires)
                {
                    IsBloodDetectorwireOpen = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BloodDetectorOpenWires, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 1- 0008000 " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsBloodDetectorwireOpen = false;
                }

                // PMCU self test
                if (!IsPMCUSelfTestFail && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.SelfTestFail) == (Int64)PMCUStatusError.SelfTestFail)
                {
                    IsPMCUSelfTestFail = true;
                    PmcuTupleError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.SelfTestFail, (int)Enumeration.ErrorTypes.PMCU);
                    codeError += " Error 2- 00000002  " + PmcuTupleError.Item2;
                    IsCPLDLatching = false;
                    ErrorIdMessageAndSolutionList.Add(PmcuTupleError);
                }
                else
                {
                    IsPMCUSelfTestFail = false;
                }



        #endregion add code

                #region warning

                if (!_isBalloonTemperatureLowWarning && (pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.BalloonTemperatureLowWarning) == (Int64)PMCUStatusError.BalloonTemperatureLowWarning)
                {
                  _isBalloonTemperatureLowWarning = true;
                 
                  var pmcuWarning = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)PMCUStatusError.BalloonTemperatureLowWarning, (int)Enumeration.ErrorTypes.PMCU);
                  codeWarning += "Warning- 00000010 " + pmcuWarning.Item2;
                  IsCPLDLatching = false;
                  ErrorIdMessageAndSolutionList.Add(pmcuWarning);

                  StopAblation();
                }
                else
                {
                  _isBalloonTemperatureLowWarning = false;
                }

                #endregion warning

                if ((pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.CatheterCableConnected) == (Int64)PMCUStatusError.CatheterCableConnected)
                {
                    if (!IsCatheterCableConnectedLastvalue)
                    {
                        IsCatheterCableConnectedLastvalue = true;

                    }
                    IsCatheterCableConnected = true;
                    CatheterStopWatchDisconnection.Start();
                }
                else
                {
                    IsCatheterCableConnectedLastvalue = false;

                    if (CatheterStopWatchDisconnection.IsRunning)
                    {
                        if (CatheterStopWatchDisconnection.ElapsedMilliseconds > catheterMaximumTimeDisconnection)
                        {
                            IsCatheterLastUseDateUpdated = false;
                            IsCatheterExpirationDateUpdated = false;
                            IsCatheterCableConnected = false;
                            IsCatheterInError = false;
                            CatheterStopWatchDisconnection.Reset();
                            CatheterStopWatchDisconnection.Stop();
                        }
                    }
                    else
                    {
                        IsCatheterLastUseDateUpdated = false;
                        IsCatheterExpirationDateUpdated = false;
                        IsCatheterCableConnected = false;
                        IsCatheterInError = false;
                    }
                    ResetCatheterInformation();
                }

                if ((pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.PMCUReady) == (Int64)PMCUStatusError.PMCUReady)
                {
                    IsPMCUReady = true;
                }
                else
                {
                    IsPMCUReady = false;
                }

                if ((codeError.Contains("Error") || codeWarning.Contains("Warning")) && !IsCPLDLatching)
                {
                    try
                    {

                        if (ResetPMCUErrorStopWatchDisconnection.IsRunning)
                        {
                            if (ResetPMCUErrorStopWatchDisconnection.ElapsedMilliseconds > errorResetingMaximumTime)
                            {
                                ResetPMCUErrorStopWatchDisconnection.Reset();
                                IsSystemRested = false;
                                pmcuPreviousError = string.Empty;
                                GenericError = string.Empty;
                            }
                        }
                        else
                        {
                            if ((codeError != string.Empty && codeError != pmcuPreviousError && !WarningMessageManager.SearchMessage(codeError) 
                                 || (codeWarning != string.Empty && !WarningMessageManager.SearchMessage(codeWarning))) && !Console.GUIInMaintenanceMode)
                            {
                                try
                                {
                                    if (IsWindowLoaded)
                                    {
                                        ResetPMCUErrorStopWatchDisconnection.Start();
                                        if (CanChangeTank)
                                            ResetSystem();
                                        else
                                        {
                                            if (codeError.Contains("Error"))
                                            {
                                              GenericError = codeError;
                                              pmcuPreviousError = codeError;
                                            } 
                                            else
                                            {
                                                IsSytemInWarning = true;
                                            }

                                            List<Tuple<long, string, string, string>> copyOfErrorIdMessageAndSolutionList = new List<Tuple<long, string, string, string>>(ErrorIdMessageAndSolutionList);
                                            Task.Delay(4000).ContinueWith(t => SavErrors((int)Enumeration.ErrorTypes.PMCU, copyOfErrorIdMessageAndSolutionList, PreviousSystemState));

                                            DisplayErrorMessage(codeError, codeWarning, IsSytemInWarning, Enumeration.ErrorTypes.PMCU);
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    exception.ToString();
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        exception.ToString();
                    }
                }
            }
        }

        private void StopAblation()
        {
          if (SystemState == MessageStateId.CAN_ID_STATE_TRANSITION ||
              SystemState == MessageStateId.CAN_ID_STATE_ABLATION)
          {
            Console.Stop();
          }
        }

    /// <summary>
    /// Gets Sole valves status
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void GetSolenoidValvesStatus(Int64 cMCUValvesStatus)
        {
            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve1ON) == (Int64)CMCUValvesStatus.SolenoidValve1ON)
            {
                IsSolenoidValve1ON = true;
            }
            else
            {
                IsSolenoidValve1ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve2ON) == (Int64)CMCUValvesStatus.SolenoidValve2ON)
            {
                IsSolenoidValve2ON = true;
            }
            else
            {
                IsSolenoidValve2ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve3ON) == (Int64)CMCUValvesStatus.SolenoidValve3ON)
            {
                IsSolenoidValve3ON = true;
            }
            else
            {
                IsSolenoidValve3ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve4ON) == (Int64)CMCUValvesStatus.SolenoidValve4ON)
            {
                IsSolenoidValve4ON = true;
            }
            else
            {
                IsSolenoidValve4ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve5ON) == (Int64)CMCUValvesStatus.SolenoidValve5ON)
            {
                IsSolenoidValve5ON = true;
            }
            else
            {
                IsSolenoidValve5ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve6ON) == (Int64)CMCUValvesStatus.SolenoidValve6ON)
            {
                IsSolenoidValve6ON = true;
            }
            else
            {
                IsSolenoidValve6ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve7ON) == (Int64)CMCUValvesStatus.SolenoidValve7ON)
            {
                IsSolenoidValve7ON = true;
            }
            else
            {
                IsSolenoidValve7ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve8ON) == (Int64)CMCUValvesStatus.SolenoidValve8ON)
            {
                IsSolenoidValve8ON = true;
            }
            else
            {
                IsSolenoidValve8ON = false;
            }

            if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve9ON) == (Int64)CMCUValvesStatus.SolenoidValve9ON)
            {
                IsSolenoidValve9ON = true;
            }
            else
            {
                IsSolenoidValve9ON = false;
            }
        }

        /// <summary>
        /// Displays error message
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="error">A string representing an Error.</param>
        /// <param name="warning">A string representing a warning.</param>
        /// <param name="_isSystemInWarning">A boolean representing if system is in warning.</param>
        internal void DisplayRemoteControlWarningMessage(string error, string warning)
        {

            StopListeningCanOneCommunication = true;
            StopListeningCanTwoCommunication = true;

            // RCWarningPopupControlPopup();
            RCWarningTimerPopupMessage();
            CanOneStopWatchCommunicationLost?.Restart();
            CanTwoStopWatchCommunicationLost?.Restart();

            StopListeningCanOneCommunication = false;
            StopListeningCanTwoCommunication = false;
        }

        /// <summary>
        /// Displays Warning message
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        public async Task RCWarningTimerPopupMessage()
        {
            Tuple<long, string, string, string> remotewarningmessage = new Tuple<long, string, string, string>(260914, "The system has detected multiple remote control buttons pressed. This may be due to a stuck button. The remote control commands will not be functional until all buttons have been successfully released.", "Disconnect and reconnect the Remote Control from the ICB. If the problem persists, contact Boston Scientific technical support and provide the message code.", "Warning 260914 - Disconnect and reconnect the Remote Control from the ICB. If the problem persists, contact Boston Scientific technical support and provide the message code.");
            if (RemoteControlIssueMessageList.Count == 0)
            {
                RemoteControlIssueMessageList.Add(remotewarningmessage);
                //  warningMessagesManager.AddMessage(RemoteControlIssueMessageList, WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.WARNING);
            }
            //else
            //{
            //    if (warningMessagesManager.WarningMessagesList.Count == 0)
            //        warningMessagesManager.AddMessage(RemoteControlIssueMessageList, WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.WARNING);

            //}
            MessagePopupHandler warningPopup = new MessagePopupHandler();
            warningPopup.Start(RemoteControlIssueMessageList);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task delay = Task.Delay(3000);
            await delay;
            sw.Stop();
            warningPopup.Stop();
        }



        /// <summary>
        /// Displays error message
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="error">A string representing an Error.</param>
        /// <param name="warning">A string representing a warning.</param>
        /// <param name="_isSystemInWarning">A boolean representing if system is in warning.</param>
        internal void DisplayErrorMessage(string error, string warning, bool _isSystemInWarning = false, Enumeration.ErrorTypes errorTypes = Enumeration.ErrorTypes.Unknown)
        {

            StopListeningCanOneCommunication = true;
            StopListeningCanTwoCommunication = true;

            string errorWithNewLine = error.Replace("+", Environment.NewLine);
            string warningWithNewLine = warning.Replace("+", Environment.NewLine);


            Application.Current.Dispatcher.Invoke((System.Action)delegate
            {
                if (error != string.Empty && !error.Contains("System Exception: Cryocable") 
                                          && (error.Contains("Error") || error.Contains("CAN1 Communication") || error.Contains("CAN2 Communication"))
                                          && ErrorIdMessageAndSolutionList.Any()
                    )
                {
                    MessagePopup messagePopup = new MessagePopup(ErrorIdMessageAndSolutionList,
                                                         MessagePopup.MessageType.ErrorMessage,
                                                         MessagePopup.ButtonType.YesNo, "", true, errorTypes);

                    try
                    {
                        if ((bool)messagePopup.ShowDialog())
                        {

                            Console.FailResetEnable();
                            System.Threading.Thread.Sleep(10);
                            Console.FailResetDisable();
                            System.Threading.Thread.Sleep(10);
                            Console.Disconnect();

                            if (errorTypes == Enumeration.ErrorTypes.CMCU)
                                ResetCMCUErrorStopWatchDisconnection.Restart();

                            else if (errorTypes == Enumeration.ErrorTypes.PMCU)
                                ResetPMCUErrorStopWatchDisconnection.Restart();

                            IsSystemRested = true;
                            IsVacuumDisconnected = true;
#if Simulator

                            CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
#endif
                        }
                        else
                        {

                            warningMessagesManager.AddMessage(ErrorIdMessageAndSolutionList, WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.ERROR, errorTypes);
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO
                        ex.ToString();
                    }
                }
                else if (_isSystemInWarning && warning != string.Empty && ErrorIdMessageAndSolutionList.Any())
                {
                    try
                    {


                        MessagePopup warningPopup = new MessagePopup(ErrorIdMessageAndSolutionList, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "", true, errorTypes);

                        Task.Delay(5000).ContinueWith(t => warningPopup.Yes_Click(warningPopup, null));

                        warningPopup.ShowDialog();

                        warningMessagesManager.AddMessage(ErrorIdMessageAndSolutionList, WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.WARNING, errorTypes);

                        if (errorTypes == Enumeration.ErrorTypes.CMCU)
                            ResetCMCUErrorStopWatchDisconnection.Restart();


                        IsSystemRested = true;


                    }
                    catch (Exception ex)
                    {
                        // TODO
                        ex.ToString();
                    }
                }
                else if (error == "RemoteControlIssue")
                {
                    DisplayRemoteControlWarningMessage("RemoteControlIssue", string.Empty);
                }
            });

            CanOneStopWatchCommunicationLost?.Restart();
            CanTwoStopWatchCommunicationLost?.Restart();
#if !DEBUG
            // Commenting this so the system doesn't reset CAN1 in this function. This is only to display an error message and not to reset system.
            //ResetCanOneStopWatch();
#endif

            StopListeningCanOneCommunication = false;
            StopListeningCanTwoCommunication = false;
        }

        /// <summary>
        /// Displays error message
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="error">A string representing an Error.</param>
        /// <param name="warning">A string representing a warning.</param>
        /// <param name="_isSystemInWarning">A boolean representing if system is in warning.</param>
        public void DisplayException5Message()
        {

            Application.Current.Dispatcher.Invoke((System.Action)delegate
            {
                ConnectCatheterPopup messagePopup = new ConnectCatheterPopup(" " + " ",
                                              ConnectCatheterPopup.MessageType.ErrorMessage);

                if ((bool)messagePopup.ShowDialog())
                {
                    this.Console.FailResetEnable();
                    System.Threading.Thread.Sleep(10);
                    this.Console.FailResetDisable();
                    System.Threading.Thread.Sleep(10);
                    this.Console.Disconnect();

                    //Here we connect the catheter
                    System.Threading.Thread.Sleep(1000);
                    this.Console.Connect();
                    IsVacuumDisconnected = false;
                    IsCMCUExceptionType5 = false;

                }

                else
                {
                    this.Console.FailResetEnable();
                    System.Threading.Thread.Sleep(10);
                    this.Console.FailResetDisable();
                    System.Threading.Thread.Sleep(10);
                    this.Console.Disconnect();
                    IsVacuumDisconnected = true;
                    IsCMCUExceptionType5 = false;
                    //List<Tuple<long, string, string, string>> listOfError = new List<Tuple<long, string, string, string>>();

                    // Tuple<long, string, string, string> umbilicalError = new Tuple<long, string, string, string>(1, "System Exception: mechanical umbilical cable ", "System Exception: mechanical umbilical cable "
                    //     , "System Exception: mechanical umbilical cable ");

                    //Tuple<long, string, string, string> umbilicalError = Data.DataAccess.GetErrorAndSolutionTranslationsForCurrentLanguage((int)Enumeration.GUIMessages.ID110, (int)Enumeration.ErrorTypes.PMCU);


                    //listOfError.Add(umbilicalError);
                    //warningMessagesManager.AddMessage(listOfError, WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.ERROR, Enumeration.ErrorTypes.CMCU);
                }
            });


        }

        /// <summary>
        /// Resets the system
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetSystem()
        {
            Console.FailResetEnable();
            System.Threading.Thread.Sleep(10);
            Console.FailResetDisable();
            System.Threading.Thread.Sleep(10);
            Console.Disconnect();
            ResetCMCUErrorStopWatchDisconnection.Restart();
            ResetPMCUErrorStopWatchDisconnection.Restart();
            IsSystemRested = true;
            IsVacuumDisconnected = true;
            IsCMCUExceptionType5 = false;
            pmcuPreviousError = string.Empty;
            cmcuPreviousError = string.Empty;
            cmcuPreviousWarning = string.Empty;

        }

        /// <summary>
        /// ResetBootLoaderSystem.(Not used)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetBootLoaderSystem()
        {
            Console.SystemResetEnable();
            System.Threading.Thread.Sleep(10);
            Console.SystemResetDisable();
            System.Threading.Thread.Sleep(10);

            Console.Disconnect();
            ResetCMCUErrorStopWatchDisconnection.Restart();
            ResetPMCUErrorStopWatchDisconnection.Restart();
            IsSystemRested = true;
            IsVacuumDisconnected = true;
            IsCMCUExceptionType5 = false;
            pmcuPreviousError = string.Empty;
            cmcuPreviousError = string.Empty;
            cmcuPreviousWarning = string.Empty;
            ErrorIdMessageAndSolutionList.Clear();
        }




        /// <summary>
        /// Resets the System and Warnning values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetSystemAndWarnning()
        {
            Console.FailResetEnable();
            System.Threading.Thread.Sleep(10);
            Console.FailResetDisable();
            System.Threading.Thread.Sleep(10);
            Console.Disconnect();
            ResetCMCUErrorStopWatchDisconnection.Restart();
            ResetPMCUErrorStopWatchDisconnection.Restart();
            IsSystemRested = true;
            IsVacuumDisconnected = true;
            IsCMCUExceptionType5 = false;
            pmcuPreviousError = string.Empty;
            cmcuPreviousError = string.Empty;
            cmcuPreviousWarning = string.Empty;
            // Reset CAN1
            ResetCanOneStopWatch();
            ErrorIdMessageAndSolutionList.Clear();
        }

        /// <summary>
        /// Starts the Ack Process
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="canStartAckProcess">A boolean representing if ack process can be started.</param>
        private void StartAckProcess(bool canStartAckProcess)
        {
            if (canStartAckProcess)
            {
                IsReadingFromMicroControllerForRegisterValidation = true;
                //ackTimer.Start();
            }
        }

        /// <summary>
        /// Update the Ablation Summary
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void UpdateAblationSummary()
        {
            int duration = 0;
            int ablationSite = (int)AblationSiteEnum.OTHER;

            //if (AblationSummary != null)
            //{
            //    if (AllAblationDataList != null)
            if (AblationSummary != null && AllAblationDataList != null)
            {
                ////AppTrace.Log("Start to update Ablation Summary", LogLevel.Debug,
                //    Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel),
                //    nameof(UpdateAblationSummary));
                // Only compute the duration in Ablation (not thawing)
                if (AllAblationDataList.Count > 0)
                {
                    List<AblationDataDetails> lastAblationDetails = AllAblationDataList[AllAblationDataList.Count - 1];

                    if (lastAblationDetails != null)
                    {
                        foreach (AblationDataDetails ablationDetails in lastAblationDetails)
                        {
                            // Keep duration for ablation only
                            if (ablationDetails.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION || ablationDetails.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION)
                            {
                                duration = ablationDetails.ID;
                                ////AppTrace.Log($"Updated Ablation ID to {duration}", LogLevel.Debug,
                                //    Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel),
                                //    nameof(UpdateAblationSummary));
                            }

                            ablationSite = ablationDetails.AblationSite;
                        }
                    }
                }

                if (AblationSummary == null)
                    AblationSummary = new AblationSummary();

                switch (ablationSite)
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

                RaisePropertyChanged("AblationSummary");
                RaisePropertyChanged("AblationList");
                ////AppTrace.Log("Updated Ablation Summary", LogLevel.Debug,
                //    Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel),
                //    nameof(UpdateAblationSummary));
                //}
            }
        }

        /// <summary>
        /// Generates the complete Ablation Summary
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool GenerateAblationSummary()
        {
            int duration = 0;
            int ablationSite = (int)AblationSiteEnum.OTHER;
            bool hasDataList = true;
            //Clears all existing data
            this.AblationSummary = new AblationSummary();

            if (this.AblationSummary != null && AllAblationDataList != null)
            {

                ////AppTrace.Log($"Generate AblationSummary in Current System State {CommonViewModel.Current.SystemState} and Previous System State {CommonViewModel.Current.PreviousSystemState}.",
                //    LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CommonViewModel), nameof(GenerateAblationSummary));


                //Only compute the duration in Ablation (not thawing)
                if (AllAblationDataList.Count > 0)
                {
                    //Generate/compute the Ablation duration (depending of the site) for each Ablations in the procedure.
                    foreach (List<AblationDataDetails> listAblationDetails in AllAblationDataList)
                    {
                        if (listAblationDetails != null)
                        {
                            //Compute the Ablation duration (stop the increment when in Thawing)
                            foreach (AblationDataDetails ablationDetails in listAblationDetails)
                            {
                                // Keep durations for ablation only
                                if (ablationDetails.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION || ablationDetails.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION)
                                {
                                    duration = ablationDetails.ID;
                                }
                                ablationSite = ablationDetails.AblationSite;
                            }

                            switch (ablationSite)
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

                            if (Enum.TryParse(ablationSite.ToString(), out AblationSiteEnum result))
                            {
                              AblationSummary.CurrentAblationSite = result;
                            }
                        }
                    }
                }
                else
                {
                    hasDataList = false;
                }

                RaisePropertyChanged("AblationSummary");
                RaisePropertyChanged("AblationList");
            }

            return hasDataList;
        }


        /// <summary>
        /// Function that resets Can One Stopwatch
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetCanOneStopWatch()
        {
            if (CanOneStopWatchCommunicationLost != null && CanOneStopWatchCommunicationLost.IsRunning)
            {
                CanOneStopWatchCommunicationLost.Restart();
                IsCanOneInError = false;
            }
        }

        /// <summary>
        /// Function that resets Can Two Stopwatch
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetCanTwoStopWatch()
        {
            if (CanTwoStopWatchCommunicationLost != null && CanTwoStopWatchCommunicationLost.IsRunning)
            {
                CanTwoStopWatchCommunicationLost.Restart();
                IsCanTwoInError = false;
            }
        }
        /// <summary>
        /// Resets catheter information
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetCatheterInformation()
        {
            try
            {
                IsCatheterLastUseDateUpdated = false;
                IsCatheterExpirationDateUpdated = false;
                IsCatheterValid = false;

                SentCatheterLastUseHour = 0;
                SentCatheterLastUseDay = 0;
                SentCatheterLastUseMonth = 0;
                SentCatheterLastUseYear = 0;
                CatheterContainerTag = string.Empty;
            }

            catch (Exception ex)
            {
                //TODO
            }

        }

        /// <summary>
        /// Read the firmware versions
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ReadTheFirmwareVersions()
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CentralMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(5);

                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, PatientMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(5);

                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
                    System.Threading.Thread.Sleep(5);

                    Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RepeaterFirmwareAndICBFirmwareId);
                    System.Threading.Thread.Sleep(5);

                    Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RemoteFirmwareId);
                    System.Threading.Thread.Sleep(5);
                }

                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                }
            }
        }

        /// <summary>
        /// Read the firmware versions
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="numberOfRetry">The number of maximum reading</param>
        public void ReadTheFirmwareVersions(int numberOfRetry)
        {
            for (int i = 0; i < numberOfRetry; i++)
            {
                try
                {
                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CentralMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, PatientMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RepeaterFirmwareAndICBFirmwareId);
                    System.Threading.Thread.Sleep(10);

                    Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RemoteFirmwareId);
                    System.Threading.Thread.Sleep(10);
                }

                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                }
            }
        }

        /// <summary>
        /// Read patient and controller firmware versions
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="numberOfRetry">The number of maximum reading.</param>
        public void ReadPMCAndCMCUFirmware(int numberOfRetry)
        {
            try
            {
                Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CentralMicroControllerFirmwareVersionId);
                System.Threading.Thread.Sleep(5);

                Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, PatientMicroControllerFirmwareVersionId);
                System.Threading.Thread.Sleep(5);
            }

            catch (Exception ex)
            {
                //TODO
            }
        }

        /// <summary>
        /// Read the ICB and repeater firmware versions
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="numberOfRetry">The number of maximum reading.</param>
        public void ReadRepeaterAndICBFirmware(int numberOfRetry)
        {
            try
            {
                Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
                System.Threading.Thread.Sleep(5);

                Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RepeaterFirmwareAndICBFirmwareId);
                System.Threading.Thread.Sleep(5);
            }

            catch (Exception ex)
            {
                //TODO
            }
        }

        /// <summary>
        /// Read the remote firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="numberOfRetry">The number of maximum reading.</param>
        public void ReadRemoteFirmware(int numberOfRetry)
        {
            try
            {
                Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RemoteFirmwareId);
                System.Threading.Thread.Sleep(5);
            }

            catch (Exception ex)
            {
                //TODO
            }
        }

        /// <summary>
        ///  Occurs when serial data recieved
        ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">LSPRO.</param>
        /// <param name="e">Serial event data.</param>
        private void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (IsLsproInitialized && IsWindowLoaded)
            {
                try
                {
                    byte[] data = e?.Data;

                    if (data?.Length >= 7)
                    {

                        if (data[7] == (byte)LSPRORequest.Authenticate_Request)
                        {
                            SpManager.SendPacket(CCMPCommand.CCMP_AUTHENTICATE, SpManager.LSPROEnumeartion.GUID);
                        }
                        else if (data[7] == (byte)LSPRORequest.Get_Timed_Value_Request)
                        {
                            //TODO
                        }
                    }
                }

                catch (Exception ex)
                {
                    ex.ToString(); //TO dO
                }
            }

        }



        /// <summary>
        /// Send to LSPRo the time and temperature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="time">Time in ablation.</param>
        /// <param name="temperature"> Balloon temperature.</param>
        /// <param name="consoleState"></param>
        /// <param name="numberOfablation">The number of ablation.</param>
        public void SendTimeAndTemperature(int time, double temperature, int consoleState, int numberOfablation)
        {

            byte[] consoleStateArray = new byte[4];
            byte[] timeArray = new byte[8];
            byte[] numberOfablationArray = new byte[4];
            byte[] temperatureArray = new byte[8];

            consoleStateArray = LSPRODataBuilder.FormatConsoleStatus(consoleState);
            timeArray = LSPRODataBuilder.FormatTime(time);
            numberOfablationArray = LSPRODataBuilder.FormatNumberOfAblation(numberOfablation);
            temperatureArray = LSPRODataBuilder.FormatTemperature(temperature);

            //Appending data
            byte[] appendTimeToConsoleStatus = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(consoleStateArray, timeArray);
            byte[] appendTemperatureToNumberOfablation = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(numberOfablationArray, temperatureArray);

            byte[] timeDatas = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(appendTimeToConsoleStatus, appendTemperatureToNumberOfablation);

            SpManager?.SendPacket(CCMPCommand.CCMP_GET_TIMED_VALUES, timeDatas, SpManager.LSPROEnumeartion.Count);

        }


        /// <summary>
        /// Reset the diaphragm reference
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0028</id>
        /// <returns>True if success</returns>
        public bool ResetDiaphragmReference()
        {

            DiaphragmConditioning.IsDiaphragmReseting = true;
            Console.IsConsoleInAblationState = false;
            MaximumAveragePacingLevel = 0;
            DiaphragmConditioning.AmplitudeReference = EcgChannel7And8Reading;

            if (MaximumAveragePacingLevel == 0)
                return true;
            return false;
        }


        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 
        /// .
        /// </summary>
        public double EtsSesnor1
        {
            get
            {
                return etsSesnor1;
            }
            set
            {

                SetProperty(ref this.etsSesnor1, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0036</id>
        public double EtsSesnor2
        {
            get
            {
                return etsSesnor2;
            }
            set
            {

                SetProperty(ref this.etsSesnor2, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0037</id>
        public double EtsSesnor3
        {
            get
            {
                return etsSesnor3;
            }
            set
            {

                SetProperty(ref this.etsSesnor3, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0038</id>
        public double EtsSesnor4
        {
            get
            {
                return etsSesnor4;
            }
            set
            {

                SetProperty(ref this.etsSesnor4, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0039</id>
        public double EtsSesnor5
        {
            get
            {
                return etsSesnor5;
            }
            set
            {

                SetProperty(ref this.etsSesnor5, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0040</id>
        public double EtsSesnor6
        {
            get
            {
                return etsSesnor6;
            }
            set
            {

                SetProperty(ref this.etsSesnor6, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0041</id>
        public double EtsSesnor7
        {
            get
            {
                return etsSesnor7;
            }
            set
            {

                SetProperty(ref this.etsSesnor7, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0042</id>
        public double EtsSesnor8
        {
            get
            {
                return etsSesnor8;
            }
            set
            {

                SetProperty(ref this.etsSesnor8, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0043</id>
        public double EtsSesnor9
        {
            get
            {
                return etsSesnor9;
            }
            set
            {

                SetProperty(ref this.etsSesnor9, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0044</id>
        public double EtsSesnor10
        {
            get
            {
                return etsSesnor10;
            }
            set
            {

                SetProperty(ref this.etsSesnor10, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0045</id>
        public double EtsSesnor11
        {
            get
            {
                return etsSesnor11;
            }
            set
            {

                SetProperty(ref this.etsSesnor11, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0046</id>
        public double EtsSesnor12
        {
            get
            {
                return etsSesnor12;
            }
            set
            {

                SetProperty(ref this.etsSesnor12, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The ETS sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0047</id>
        public double EtsSesnor13
        {
            get
            {
                return etsSesnor13;
            }
            set
            {

                SetProperty(ref this.etsSesnor13, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Time The TIP sesnor value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0048</id>
        public double TIP
        {
            get
            {
                return tIP;
            }
            set
            {

                SetProperty(ref this.tIP, value);
            }
        }

        /// <summary>
        /// This property gets/sets the list of sesnors state value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0049</id>
        public List<int> ListOfSesnorsState
        {
            get
            {
                return listOfSesnorsState;
            }
            set
            {
                listOfSesnorsState = value;
                RaisePropertyChanged("ListOfSesnorsState");
            }
        }

        /// <summary>
        /// This property gets/sets the minimum temperature value
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0050</id>
        public double MinimumTemperature
        {
            get => minimumTemperature;
            set => minimumTemperature = value;
        }

        private readonly ISubject<bool> _rcBalloonDiameterButtonPressedSubject = new Subject<bool>(); 
        public IObservable<bool> RCBalloonDiameterButtonPressedObserver => _rcBalloonDiameterButtonPressedSubject;

        /// <summary>
        /// This property gets/sets teh RepeaterFirmwareDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterFirmwareDBVersion
        {
            get => repeaterFirmwareDBVersion;
            set => repeaterFirmwareDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the ICBFirmwareDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ICBFirmwareDBVersion
        {
            get => iCBFirmwareDBVersion;
            set => iCBFirmwareDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the RemoteControlFirmwareDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RemoteControlFirmwareDBVersion
        {
            get => remoteControlFirmwareDBVersion;
            set => remoteControlFirmwareDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the PatientMicroControllerFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerFirmwareVersionDBVersion
        {
            get => patientMicroControllerFirmwareVersionDBVersion;
            set => patientMicroControllerFirmwareVersionDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the PatientMicroControllerBootLoaderFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerBootLoaderFirmwareVersionDBVersion
        {
            get => patientMicroControllerBootLoaderFirmwareVersionDBVersion;
            set => patientMicroControllerBootLoaderFirmwareVersionDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets RepeaterBootLoaderFirmwareDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterBootLoaderFirmwareDBVersion
        {
            get => repeaterBootLoaderFirmwareDBVersion;
            set => repeaterBootLoaderFirmwareDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets CpldFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CpldFirmwareVersionDBVersion
        {
            get => cpldFirmwareVersionDBVersion;
            set => cpldFirmwareVersionDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the CentralMicroControllerFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerFirmwareVersionDBVersion
        {
            get => centralMicroControllerFirmwareVersionDBVersion;
            set => centralMicroControllerFirmwareVersionDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the CentralMicroControllerBootLoaderFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerBootLoaderFirmwareVersionDBVersion
        {
            get => centralMicroControllerBootLoaderFirmwareVersionDBVersion;
            set => centralMicroControllerBootLoaderFirmwareVersionDBVersion = value;
        }

        /// <summary>
        /// This property gets/sets the CatheterFirmwareVersionDBVersion value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterFirmwareVersionDBVersion
        {
            get => catheterFirmwareVersionDBVersion;
            set => catheterFirmwareVersionDBVersion = value;
        }


        /// <summary>
        /// This read-only property returns the Software Version value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string SoftwareVersion
        {
            get
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string version = fileVersionInfo.ProductVersion;

                    return version;
                }
                catch
                {
                    //TODO :
                    return "0.0.0.0";
                }
            }
            set
            {
                RaisePropertyChanged("SoftwareVersion");
            }
        }

        public int ConsoleVersionID
        {
            get
            {
                return this.Data.DataAccess.GetLatestVersion().Id;
            }

        }

        public bool IsUserManualOpned
        {
            get => isUserManualOpned;
            set => isUserManualOpned = value;
        }
        public bool IsICBConnected
        {
            get
            {
                return isICBConnected;
            }
            set
            {
                if (value != isICBConnected)
                    isICBConnected = value;
            }
        }

        public Stopwatch ICBStopWatchDisconnection { get => iCBStopWatchDisconnection; set => iCBStopWatchDisconnection = value; }
        public long ICBMaximumTimeOut
        {
            get => iCBMaximumTimeOut;
            set => iCBMaximumTimeOut = value;
        }


        /// <summary>
        /// Create a console version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ConsoleVersion CreateAConsoleVersion()
        {

            ConsoleVersion currentConsoleVersion = new ConsoleVersion();


            currentConsoleVersion.Software = SoftwareVersion;

            currentConsoleVersion.ControlFirmware = FirmwareToString(CentralMicroControllerFirmwareVersion);
            currentConsoleVersion.ControlFirmwareBootLoader = FirmwareToString(CentralMicroControllerBootLoaderFirmwareVersion);


            currentConsoleVersion.CPLDFirmware = FirmwareToString(CpldFirmwareVersion);

            currentConsoleVersion.RemoteFirmware = FirmwareToString(RemoteControlFirmware);
            currentConsoleVersion.RemoteFirmwareBootLoader = FirmwareToString(RemoteControlBootLoaderFirmwareVersion);

            currentConsoleVersion.PatientFirmware = FirmwareToString(PatientMicroControllerFirmwareVersion);
            currentConsoleVersion.PatientFirmwareBootLoader = FirmwareToString(PatientMicroControllerBootLoaderFirmwareVersion);

            currentConsoleVersion.RepeaterFirmware = FirmwareToString(RepeaterFirmware);
            currentConsoleVersion.RepeaterFirmwareBootLoader = FirmwareToString(RepeaterBootLoaderFirmware);

            currentConsoleVersion.ICBFirmware = FirmwareToString(ICBFirmware);
            currentConsoleVersion.ICBFirmwareBootLoader = FirmwareToString(ICBBootLoaderFirmwareVersion);

            currentConsoleVersion.CatheterFirmware = FirmwareToString(CatheterFirmwareVersion);


            return currentConsoleVersion;
        }


        /// <summary>
        /// Format the firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal string FirmwareToString(int value)
        {
            string valueToConvert = string.Empty;

            valueToConvert = System.Convert.ToInt64(value).ToString("X");

            try
            {
                int lenght = valueToConvert.Length;

                if (valueToConvert.Length == 4)
                    valueToConvert = valueToConvert.Insert(3, ".").Insert(2, ".").Insert(1, ".");
                else if (valueToConvert.Length == 3)
                    valueToConvert = valueToConvert.Insert(lenght, ".").Insert(lenght - 1, ".").Insert(lenght - 2, ".") + "0";

            }

            catch (Exception ex)
            {
                ex.ToString();
            }


            return valueToConvert;
        }

        private void ManageGasSate(Int64 cMCUSystemStatusError)
        {
            if ((cMCUSystemStatusError & (Int64)CMCUStatusError.LoadCellWeightFail) == (Int64)CMCUStatusError.LoadCellWeightFail)
            {
                GasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_TOO_LOW;
                return;
            }
            else if ((cMCUSystemStatusError & (Int64)CMCUStatusError.LoadCellWeightWarning) == (Int64)CMCUStatusError.LoadCellWeightWarning)
            {
                GasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_LOW;
                return;
            }

            GasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;
        }

        public List<double> EcgSensorData { get; set; } 

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AnalyseEsophagusTemperature()
        {
            EcgSensorData = new List<double> { Math.Round(ecgChannel5And6Reading), EtsSesnor1, EtsSesnor2, EtsSesnor3, EtsSesnor4,
                                             EtsSesnor5, EtsSesnor6, EtsSesnor7, EtsSesnor8,
                                             EtsSesnor9, EtsSesnor10, EtsSesnor11, EtsSesnor12};
            ListOfSesnorsState.Clear();

            ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(EcgSensorData, out eTSMinimumTemperature);

            MinimumTemperature = eTSMinimumTemperature;
        }

        internal void SaveError(int errorTypes, string errorInfo, int errorcode, MessageStateId _SystemState)
        {

            try
            {
                bool isUsingICB = isUsingICB = (ICBFirmware == 0 ? false : true);
                int? catheterIndexId = 0;
                int? userId = 0;

                if (CurrentUser == null) userId = null;
                else userId = CurrentUser.Id;

                if (IsCatheterCableConnected)
                {
                    CatheterInformation CI = this.Data.DataAccess.GetatheterInformationsAccordingToSerialNumberAndLot(CatheterSerialNumber, CatheterLot, catheterID, IsUsedForEngineering);
                    if (CI != null)
                    {
                        catheterIndexId = CI.ID;
                        isUsingICB = true;
                    }
                    else
                    {
                        catheterIndexId = null;
                    }
                }
                else
                {
                    catheterIndexId = null;
                }
            

                this.Data.DataAccess.AddErrorLog(errorInfo, DateTime.Now, errorcode, errorTypes, ConsoleVersionID, catheterIndexId, (int)_SystemState, userId, isUsingICB, (RemoteControlFirmware == 0 ? false : true), CatheterContainerTag);
            }
            catch (Exception ex)
            {

            }
        }

        internal void SavErrors(int errorTypes, List<Tuple<long, string, string, string>> _ErrorIdMessageAndSolutionList, MessageStateId _SystemState)
        {

            bool isUsingICB = isUsingICB = (ICBFirmware == 0 ? false : true);
            int? catheterIndexId = 0;
            int? userId = 0;

            if (CurrentUser == null) userId = null;
            else userId = CurrentUser.Id;
            if (IsCatheterCableConnected)
            {
                CatheterInformation CI = this.Data.DataAccess.GetatheterInformationsAccordingToSerialNumberAndLot(CatheterSerialNumber, CatheterLot, catheterID, IsUsedForEngineering);
                if (CI != null)
                {
                    catheterIndexId = CI.ID;
                    isUsingICB = true;
                }
                else catheterIndexId = null;

            }
            else catheterIndexId = null;


            foreach (Tuple<long, string, string, string> er in _ErrorIdMessageAndSolutionList)
            {
                if (er.Item4.Contains("CAN1 Communication") || er.Item4.Contains("CAN2 Communication"))
                    this.Data.DataAccess.AddErrorLog(er.Item4.Substring(0, er.Item4.LastIndexOf("-") + 10), DateTime.Now, er.Item1, errorTypes, ConsoleVersionID, catheterIndexId, (int)_SystemState, userId, isUsingICB, (RemoteControlFirmware == 0 ? false : true), CatheterContainerTag);
                else
                    this.Data.DataAccess.AddErrorLog(er.Item4.Substring(0, er.Item4.LastIndexOf("-") + 2), DateTime.Now, er.Item1, errorTypes, ConsoleVersionID, catheterIndexId, (int)_SystemState, userId, isUsingICB, (RemoteControlFirmware == 0 ? false : true), CatheterContainerTag);
            }
        }

        private void DisconnectTheICB()
        {
            EcgChannel5And6Reading = 1000;

            EtsSesnor1 = 1000;
            EtsSesnor2 = 1000;
            EtsSesnor3 = 1000;
            EtsSesnor4 = 1000;
            EtsSesnor5 = 1000;
            EtsSesnor6 = 1000;
            EtsSesnor7 = 1000;
            EtsSesnor8 = 1000;
            EtsSesnor9 = 1000;
            EtsSesnor10 = 1000;
            EtsSesnor11 = 1000;
            EtsSesnor12 = 1000;
            EtsSesnor13 = 1000;
            TIP = 1000;

            MinimumTemperature = 1000;

            eTSMinimumTemperature = 1000;

            IsMultiEtsSesnorConnected = false;

            IsBloodPressureSensorConnected = false;
        }

        public bool IsRemoteControlInError
        {
            get => isRemoteControlInError;
            set => isRemoteControlInError = value;
        }

        private volatile bool _allowStartAblation = true;
        public void UpdateAllowStartAblationState(bool allowStart)
        {
          _allowStartAblation = allowStart; 
          Console.AllowStartAblation(allowStart);
        }

        private string _catheterContainerTag = string.Empty;

        public string CatheterContainerTag
        {
          get => _catheterContainerTag;
          set => SetProperty(ref _catheterContainerTag, value);
        }
    }
}
