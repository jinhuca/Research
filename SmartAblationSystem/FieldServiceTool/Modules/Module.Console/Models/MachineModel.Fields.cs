using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Communication;
using Console;
using DataAccessLayer;
using FileSerializer;
using MicroLibrary;
using Module.Console.Helpers;
using RS232Communication;
using Unity;

namespace Module.Console.Models
{
  /// <summary>
  /// Partial class for MachineModel - Private Fields.
  /// </summary>
  public partial class MachineModel
  {
    #region Private Fields

    private readonly IUnityContainer container = new UnityContainer();
    private readonly Machine _machine;

    private readonly DispatcherTimer _CanBusOneTimer = new DispatcherTimer();
    private readonly DispatcherTimer _CanBusTwoTimer = new DispatcherTimer();
    private DispatcherTimer _remoteControlTimer = new DispatcherTimer();
    private Thread _HeartBeatThread;

    /// <summary>
    /// Text entry type enumeration.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
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
    //private NotificationModel notificationModel = NotificationModel.Instance;

    //private InflateDeflateBalloonModel inflateDeflateBalloonModel;
    //private AncestralPasswordEncrypter ancestralPasswordEncrypter = AncestralPasswordEncrypter.Instance;

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

    // Central Micro Controller: Register Values
    private int centralMicroControllerFirmwareVersion = 0;
    private int centralMicroControllerBootLoaderFirmwareVersion = 0;

    private int cpldFirmwareVersion = 0;
    private int cpldBootLoaderFirmwareVersion = -1;

    // Connection Box Register values
    private int repeaterFirmware = 0;
    private int repeaterBootLoaderFirmware = 0;

    private int iCBFirmware = 0;

    private int remoteControlFirmware = 0;
    private int remoteControlBootLoaderFirmwareVersion = 0;
    private int databaseVersion;

    private string guiVersion = string.Empty;

    //private Machine console;
    private readonly Data data;

    //private CatheterValidator catheterValidator;
    //private ObservableCollection<Models.ActionLogRecord> actionLog;

    //private OuterBalloonPressureThreshold outerBalloonPressureThreshold;

    private double continuousThawing;
    private double cP1Reading;

    private double cP2Reading;

    private double tIPReading;

    private double patientPIDDutyCycle;

    private int cPLDErrorRegister;

    private int cPLDSystemRegister;

    private int cPLDValveRegister;

    private double dGain;

    private long pMCUSystemStatusErrorCode;

    private double fM1HighRange;

    private double fM1LowRange;

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
    private int patientMicroControllerFirmwareVersion;
    private int patientMicroControllerBootLoaderFirmwareVersion;

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


    private double pT2HighRange;
    private double pT2LowRange;


    private double pT3HighRange;
    private double pT3LowRange;


    private double pT4HighRange;
    private double pT4LowRange;

    private double pIDDutyCycle;



    private CanBusMessageDefinition.MessageStateId systemState;
    private CanBusMessageDefinition.MessageStateId simulatedSystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;

    private CanBusMessageDefinition.MessageStateId previousSystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;

    private long cMCUSystemStatusError;

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

    private double ecgChannel3And4Reading;
    private double ecgChannel5And6Reading = 1000;

    private double maxEcgChannel1And2Reading = 0;
    private double maxEcgChannel3And4Reading = 0;
    private double maxEcgChannel5And6Reading = 0;

    private double ecgChannel7And8Reading;

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


    //private DMSLogic DMSLogic;

    private int requiredAblationTimeAccordingToState = 240;

    private double ecgChannel9And10Reading;
    private double channelTipReading;
    private double channelAccelerometerReading;
    private double dASLowFlow = 7800;

    private bool canstartDiaphragmMovementMonitoring = false;
    private bool canChangeTank = false;
    private Procedure currentProcedure;
    private Ablation currentAblation;
    private Patient currentPatient;

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
    private List<List<AblationECGData>> allAblationECGDataList = new List<List<AblationECGData>>();

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

    private Enumeration.TankWeight gasState = Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

    private bool isSytemInWarning = false;

    #endregion CMCU status Error

    // Violating MVVM. but ING requirements want that
    private bool isTheUserInProcedureScreen = false;

    private bool isCatheterValid = false;

    private MicroTimer _catheterConnectedTimer;
    private DispatcherTimer ackTimer = new DispatcherTimer();
    private DispatcherTimer canOneTimer = new DispatcherTimer();
    private DispatcherTimer canTwoTimer = new DispatcherTimer();
    private DispatcherTimer remoteControlTimer = new DispatcherTimer();
    private DispatcherTimer usageTimer = new DispatcherTimer();

    private static DispatcherTimer skinToSkinAblationTimer = new DispatcherTimer();


    private List<int> patientMicroControllerRegisterIDSDynamicTable = new List<int>();
    private List<int> centralMicroControllerRegisterIDSDynamicTable = new List<int>();
    private Dictionary<int, bool> patientMicroControllerackRegistersTable = new Dictionary<int, bool>();
    private Dictionary<int, bool> centralMicroControllerAckRegistersTable = new Dictionary<int, bool>();
    private bool isReadingFromMicroControllerForRegisterValidation = false;
    private List<(double, double)> listOfValues = new List<(double, double)>();

    private List<String> ablationSiteList = new List<string>();

    private bool isWindowLoaded = false;

    //private AblationSummary ablationSummary;
    //private List<Physician> physicianList;

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

    //private LoginManager loginManager;
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

    private bool isBalloonDiameterIncreased = false;
    private bool isBalloonDiameterDecreased = true;

    private bool isUserManualOpned = false;

    private bool isRemoteControlInError = false;

    private double[] bloodPressureValue;

    ASCIIToByteConverter aSCIIToByteConverter;

    byte[] bootLoaderData;

    uint packetNumber = 0;
    private double upgradeStatus = 0;
    private double moduleKey = 0;

    Tuple<long, string, string, string> LoadCellWeightWarningSolution;
    Tuple<long, string, string, string> PressureInTankIsHighFanToBeOnSolution;
    Tuple<long, string, string, string> PressurePT1InTankIsLowSolution;
    Tuple<long, string, string, string> SubCoolerTemperatureIsHighSolution;

    private int treatmentNumber;
    #region Warrning From the DB


    #endregion


    private SerialPortManager _spManager;

    private LSPROEnumeartion lSPROEnumeartion;

    private bool isLsproInitialized = false;

    private string portName = string.Empty;

    static readonly long iCBTimeOutAtInitialization = Properties.Settings.Default.ICBTimeOutAtInitialization;

    static readonly long canTwoMaximumTimeOutReference = Properties.Settings.Default.canTwoMaximumTimeOutReference;

    private int repeaterFirmwareDBVersion = 0;

    private int iCBFirmwareDBVersion;
    
    private int iCBBootLoaderFirmwareVersion = 0;

    private int remoteControlFirmwareDBVersion;
    
    private const int RemoteFirmwareId = 24;

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
    private CatheterValidator _catheterValidator;

    private bool _startButtonPressed;
    private bool _stopButtonPressed;
    private bool _startFootSwitchOn;
    private bool _stopFootSwitchOn;
    private byte _cpldFPINStatus;

    private volatile bool _canManageRTRCatheterMessage = true; 

    #endregion Private Fields
  }
}
