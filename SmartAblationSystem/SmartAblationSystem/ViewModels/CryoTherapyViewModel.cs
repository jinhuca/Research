using Communication;
using Console;
using DataAccessLayer;
using FileSerializer;
using Prism.Commands;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CustomControls.UserControls;
using Shared;
using static Communication.CanBusMessageDefinition;
using static LogSystem.LogService;
using LogLevel = LogSystem.LogLevel;
using Unity;
using static Shared.SharedConstants;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static SmartAblationSystem.ViewModels.CommonViewModel;

namespace SmartAblationSystem.ViewModels
{
  using MicroLibrary;
  using System.Reactive.Disposables;
  using System.Windows;

  /// <summary>
  /// This class is the Cryotherapy View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class CryoTherapyViewModel : BindableBase, IAblationSiteAware, ICryoTherapyViewModel
  {
    private List<AblationDataDetails> singleAblationDatasList = new List<FileSerializer.AblationDataDetails>();
    private HardwareInformations hardwareInformations = new HardwareInformations();

    private readonly VitalParametersAlerts vitalParametersAlerts = new VitalParametersAlerts();

    private Helpers.Enumeration.TankWeight gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;
    private bool canStartTheTimer = false;

    public event EventHandler<AblationEventArgs> SystemStateEvent;

    public event EventHandler ReadyStateEvent;
    public event EventHandler<InflationEventArgs> InflationStateEvent;

    private AblationEventArgs ablationEvent;
    private InflationEventArgs inflationEvent;
    private EcgEventArgs ecgEvent;

    public event EventHandler StopAblation;

    public event EventHandler PlaybackModeEvent;

    public event EventHandler TipOrBalloonPressureSelectionChangedEvent;

    public event EventHandler DiaphragmMovementUnitChangedEvent;

    public event EventHandler TemperatureChartTypeChangedEvent;

    public event EventHandler DiaphragmSensorGainChangedEvent;

    public event EventHandler ResetTherapyEvent;

    public event EventHandler ChangeTankInCryotherapyEvent;

    public event EventHandler<OcclusionPressureGraphAxisYEventArgs> OcclusionPressureGraphAxisYChangedEvent;

    public event EventHandler OcclusionPressureGraphSweepSpeedChangedEvent;

    public event EventHandler ClearOcclusionPressureGraphRequestEvent;

    private double bloodPressureMaximumValueDuringOneSecond = 120;

    private int cryoTherapyTime = 0;

    //private int timeTTI = 0;
    private int lastCryoTherapyTime = 0;
    private int totalCryoTherapyTime = 0;
    private int elapsedTime = 0;
    private int elapsedTimeLastValue = 0;
    private int elapsedTimeLastValueForFlowReading = 0;
    private int elapsedTimeLastValueForIBPReading = 0;
    private bool previousCanStartTherapy = false;

    private int ecgTime = 0;

    private bool isVisible = true;
    private int ablationNumber = 0;
    private DataAccess dataAccess;
    private bool isTheProcedureEnded = false;

    private volatile bool isAblating = false; // private DispatcherTimer timerAblation = new DispatcherTimer(); 
    private volatile bool isThawing = false; // private DispatcherTimer timerThawing = new DispatcherTimer();

    //    private DispatcherTimer timerLoading = new DispatcherTimer();

    private bool isCatheterConnected = false;

    private int requiredAblationTimePlueMargin = 500;

    private int requiredTargetTemperature = -30;
    private int lowAblationTemperatureAlarm = -45;
    private int highAblationTemperatureAlarm = 30;
    private int thawTimerToTemperature = 0;
    private int esophagusTemperature = 20;
    private int diaphragmAmplitude = 80;
    private double dmsDetectionThreshold = 1;
    private int dmsDetectionThresholdvalue = 1;
    private int diaphragmSensorGain = 100;
    private string hospitalName = "";
    private string error = "";

    private double temperatureRate = 0;
    private double previousTemperature = 0;
    private double maxTemperatureRate = 1000;
    private int timeToTargetTemperature = 0;
    private int timeToThaw = 0;
    private int treatmentNumber = 0;
    private int previousTreatmentNumber = 0;
    private int totalTreatmentNumber = 0;
    private int maxElapsedTime = 99999;
    private int thawingElapsedTime = 0;
    private const int expectedThawingTime = 60;

    private int ablationTimer = 0;
    private int totalAblationDuration = 0;


    private int veinIsolationDuration = 0;
    private int lastVeinIsolationDuration = 0;
    private int expectedTimeToVeinIsolation = 60;
    private int newAblationTimer = 240;
    private const int maximumAblationTimer = 240;
    private const int minimumAblationTimer = 30;

    private int veinIsolationStratTime = 0;
    private int veinIsolationEndTime = 0;
    private int exceptionStateTime = 0;

    private int durationExpectedVeinIsolationTime = 0;
    private int maxAblationTimerUsingDurationMode = 240;

    private int ablationTimerTTI = 180;
    private int newAblationTimerTTI = 240;

    private int ablationTimerTTIFixed = 180;
    private int newAblationTimerTTIFixed = 240;

    private Enumeration.AblationDurationType ablationDurationType = 0;

    private int requiredAblationTimeAccordingToState = 240;

    //Resting Compter Timer;
    int timePreviousRefrence = 0;
    int timingFiliter = 0;

    private int thawTemperature = 20;
    readonly int SkinToSkinTemperature = Properties.Settings.Default.SkinToSkinTemperature;
    private const double AbsolutePressureError = 0.5;
    private int decreasingCompter = 0;
    private double previousTC1Reading = 0;
    private double tempIIT = 0;

    private bool TemperatureReachedRequiredAblationTemperature = false;

    private Communication.CanBusMessageDefinition.MessageStateId PreviousSystemState =
      CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;

    private string previousGenericError = string.Empty;

    private bool keepDisplayTimeToThaw = false;
    private bool keepDisplayTimeToTemperature = false;
    private bool esophagusTemperatureThresholdReached = false;
    private bool diaphragmAmplitudeThresholdReached = false;
    private bool isDiaphragmMovementDetected = false;
    private bool isTimeToTargetTemperatureVisible = false;

    private bool isTimeToThawTemperatureVisible = false;
    private bool isSnowFlakeVisible = false;
    private bool isTreatmentNumberAndPlayBackVisible = false;
    private bool isLastAblationDataLoaded = false;
    private bool displayThawingBallon = true;

    private bool diaphragmMovementPercentageSelected = true;
    private short temperatureChartType;
    private short refrigerantLevelUnit;

    private bool isIsolatingVein = false;

    private bool isVeinIsolationDurationVisible = false;

    private bool isStatusAbllationBallonVisible = false;

    private bool isSqaureVisible = false;

    private bool isDiaphragmMovementVisible = true;

    private bool isEsophagusTemperatureVisible = true;
    private bool isEsophagusTemperatureInRange = true;

    private bool isEsophagusTemperatureConditionAlertsMeet = false;

    private bool isThawTemperatureReached = false;

    private bool isTargetTemperatureReached = false;

    private bool isDMSSettingPopupShow = false;
    private bool isBloodPressureSettingsPopupShow = false;
    private const string FILESTORAGE = "FileStore\\";

    private NotificationModel notificationModel = NotificationModel.Instance;

    #region states properties

    private bool isSystemInIdle = false;
    private bool isSystemInReady = false;
    private bool isSystemInInflation = false;
    private bool isSystemInTransition = false;
    private bool isSystemInAblation = false;
    private bool isSystemInThawing = false;
    private bool isSystemInException = false;

    #endregion

    private bool isLanguageChanged = false;

    private bool displayAblationSiteWarning = false;

    private bool isRequiredAblationTimeVisible = true;
    private bool isAblationTimeVisibale = false;

    private bool isFixedTimerSelected = true;
    private bool iSTTIFixedTimerSelected = false;
    private bool iSTTIDurationTimerSelected = false;
    private bool iSTTISelected = false;
    private bool cryoDurationChanged = true;
    private bool isUserAllowedToChangeAblationTimers = true;
    private short alertDurationValue = 0;
    private const short AlertMaximumDurationValue = 10;

    private double lastDiaphragmMovementPercentageOrGReadingValue = 0;
    private double lastFlowReadingValue = 0;
    private double lastIBPReadingValue = 0;
    private bool isDMSDetectionThresholdValid = false;

    private bool isSystemMonitoringDiaphragmAlert = false;

    private bool isLoadingAbortedAblation = true;

    private bool iSThePressureSetPointReached = false;
    private bool allowPSPChangeDuringThawing = false;

    private bool dataLoading = false;

    private bool skinToSkinCountStarted = false;

    private bool isAllowedToSetPlayBack = false;

    //private bool isSiteUsingDefalteAfterThaw = false;
    private bool isThawingTemperatureSetPointReached = false;

    private bool isMonitoringBloodPressure = false;

    private bool displayBloodPressure = false;

    private int databaseVersion;

    private string gUIVersion = string.Empty;

    private double TC1ReadingErrorValue = 40;

    private bool enabledIsBloodPressureSensorConnected = false;
    private bool isAblationSiteChanged = false;
    private bool allowUserToActivateLowFlow = false;

    private bool isSavedToDB = false;
    // private bool isAudioAlertMuteShow = false;

    private uint notificationValueIndex = 5;
    private uint maximumNotificationValueIndex = 5;

    private Physician currentPhysician = null;

    private int timeInAblationMax = 0;
    private bool isUsingCirca = true; //true;   //false;   //

    private List<int> lowestTempChannelNum = new List<int>(); // {2,4,5} ;

    // private int tipValue = 0;
    private bool hasTip = true;
    private int cryotherapyEndTime = 0;
    List<string> invalidPortComList = new List<string> { "COM1", "COM2", "COM3", "COM4" };

    private List<int> listOfSesnorsStatePlayback = new List<int>();

    private const int CatheterFirmwareVersionId = 56;
    public bool playbackOffTimeReset = false;

    private Stopwatch _highResDmsDataStopWatcher = new Stopwatch();

    private double _maximumHRAveragePacingLevel;
    private double _maximumAveragePacingLevel;

    private readonly List<double[]> _highResDmsReading = new List<double[]>();
    private readonly List<double[]> _bloodPressureReading = new List<double[]>();
    private readonly List<double> _ecgChannel3And4Reading = new List<double>();

    private readonly ISubject<bool> _dmsQuickSettingsRefreshSubject = new Subject<bool>();
    private readonly SerialDisposable _dmsQuickSettingsPopupDisposible = new SerialDisposable();

    private readonly ISubject<bool> _occlusionPressureSettingsRefreshSubject = new Subject<bool>();
    private readonly SerialDisposable _occlusionPressureSettingsPopupDisposible = new SerialDisposable();

    private readonly ISubject<bool> _notifyAblationSiteChangedSubject = new Subject<bool>(); 

    public bool IsFromReturnToProcedure { get; set; }
    public double[] DmsData => HighResDmsSignalDetected ? HighResDmsData : EcgDmsData;

    public void ClearDmsData()
    {
      lock (_highResDmsReading)
      {
        _highResDmsReading.Clear();
      }

      lock (_ecgChannel3And4Reading)
      {
        _ecgChannel3And4Reading.Clear();
      }
    }

    public double[] HighResDmsData
    {
      get
      {
        lock (_highResDmsReading)
        {
          var data = _highResDmsReading.SelectMany(d => d).ToArray();
          _highResDmsReading.Clear();
          return data;
        }
      }
    }

    public double[] EcgDmsData
    {
      get
      {
        lock (_ecgChannel3And4Reading)
        {
          var data = _ecgChannel3And4Reading.ToArray();
          _ecgChannel3And4Reading.Clear();
          return data;
        }
      }
    }

    public void ClearBloodPressureData()
    {
      lock (_bloodPressureReading)
      {
        _bloodPressureReading.Clear();
      }
    }

    public double[] BloodPressureData
    {
      get
      {
        lock (_bloodPressureReading)
        {
          var data = _bloodPressureReading.SelectMany(d => d).ToArray();
          _bloodPressureReading.Clear();
          return data;
        }
      }
    }

    private bool _isInitializing;

    private readonly HashSet<string> _settingsPropertyNameList = new HashSet<string>()
    {
      nameof(RequiredTargetTemperature),
      nameof(ThawTimerToTemperature),
      nameof(LowAblationTemperatureAlarm),
      nameof(HighAblationTemperatureAlarm),
      nameof(EsophagusTemperature),
      nameof(DiaphragmAmplitude),
      nameof(DMSDetectionThreshold),
      nameof(DMSDetectionThresholdValue),
      nameof(DiaphragmSensorGain),
      nameof(IgnoreMinimumDiaphragmMovementValue),
      nameof(IsUsingAudioAlertSetting),
      nameof(EnableFastInflationMode),
      nameof(EnabaleEnhancedAudio),
      nameof(IsUsingAutoPlayback),
      nameof(TemperatureChartType),
      nameof(RefrigerantLevelUnit),
      nameof(DeflateAfterThaw),
      nameof(CanDisplayShadowGraph),
      nameof(RequiredVolume),
      nameof(AblationDurationType),
      nameof(ExpectedTimeToVeinIsolation),
      nameof(AblationTimerTTIFixed),
      nameof(NewAblationTimerTTIFixed),
      nameof(DurationExpectedVeinIsolationTime),
      nameof(AblationTimerTTI),
      nameof(NewAblationTimerTTI),
      nameof(AblationTimer)
    };

    public ETSTemperatureGraph.ChartDisplayMode EtsGraphDisplayMode
    {
      get
      {
        var systemState = CommonViewModel.Current.SystemState;
        var mode = ETSTemperatureGraph.ChartDisplayMode.None;

        switch (systemState)
        {
          case MessageStateId.CAN_ID_STATE_TRANSITION:
          case MessageStateId.CAN_ID_STATE_ABLATION:
          case MessageStateId.CAN_ID_STATE_THAWING:
            mode = ETSTemperatureGraph.ChartDisplayMode.Realtime;
            break;

          default:
            // all other states not in Playback mode, return None, otherwise, return playback mode 
            mode = SensorReadingMananger.AreSensorsConnected
              ? ETSTemperatureGraph.ChartDisplayMode.None
              : ETSTemperatureGraph.ChartDisplayMode.Playback;
            break;
        }

        return mode;
      }
    }

    private List<double> _etsPlaybackData = null;

    public List<double> EtsPlaybackData
    {
      get => _etsPlaybackData;
      set => SetProperty(ref _etsPlaybackData, value);
    }

    #region Command

    public ICommand LastAblationCommand { get; private set; }
    public ICommand AblationNumberForwardCommand { get; private set; }
    public ICommand AblationNumberBackwardCommand { get; private set; }
    public ICommand ConnectCommand { get; private set; }
    public ICommand StartCommand { get; private set; }
    public ICommand StopCommand { get; private set; }
    public ICommand NotificationsCommand { get; private set; }
    public ICommand NotificationsChangeCommand { get; private set; }
    public ICommand CloseDMSQuickSettingsCommand { get; private set; }
    public ICommand GoToMoreSettingsCommand { get; private set; }

    public ICommand OcclusionPressureSettingsChangeCommand { get; private set; }
    public ICommand IncreaseTimeCommand { get; private set; }
    public ICommand DecreaseTimeCommand { get; private set; }
    public ICommand AblationSiteCommand { get; private set; }
    public ICommand TreatmentNotesCommand { get; private set; }
    public ICommand DeflateAfterThawCommand { get; private set; }
    public ICommand VeinIsolatedCommand { get; private set; }

    public ICommand UpdateVeinIsolationDurationCommand { get; private set; }
    public ICommand ChangeTankCommand { get; private set; }

    //   public ICommand TestCommand { get; private set; }
    public ICommand LockTheFootSwitchCommand { get; private set; }

    public ICommand EnableDASBallonCommand { get; private set; }

    public ICommand ResetLSPROCommand { get; private set; }

    public ICommand ActivateLowFlowCommand { get; private set; }

    public ICommand ResetDiaphragmCommand { get; private set; }

    public ICommand SaveDMSSettingCommand { get; private set; }

    public ICommand VolumeControlOnCommand { get; private set; }

    public ICommand VolumeControlOffCommand { get; private set; }
    public ICommand CloseOcclusionPressureSettingsCommand { get; private set; }
    public ICommand SaveOcclusionPressureGraphSettingsCommand { get; private set; }

    public ICommand TareOcclusionPressureGraphCommand { get; private set; }
    public ICommand ResetTareOcclusionPressureGraphCommand { get; private set; }

    #endregion

    /// <summary>
    /// This constructor initializes the Cryotherapy View Model's properties and commands
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public CryoTherapyViewModel(IUnityContainer containerRegistry)
    {
      containerRegistry.RegisterInstance<ICryoTherapyViewModel>(this);

      this.LastAblationCommand = new DelegateCommand<object>(this.OnLastAblation, this.CanLastAblation);

      AblationNumberForwardCommand = new DelegateCommand<object>(OnAblationNumberForward).ObservesCanExecute(() => CanAblationNumberForward);

      AblationNumberBackwardCommand = new DelegateCommand<object>(OnAblationNumberBackward).ObservesCanExecute(() => CanAblationNumberBackward);

      this.ConnectCommand = new DelegateCommand<object>(this.OnConnectCommand, this.CanConnectCommand);

      this.StartCommand = new DelegateCommand<object>(this.OnStartCommand, this.CanStartCommand);

      StopCommand = new DelegateCommand<object>(OnStopCommand, CanStopCommand);

      // this.TestCommand = new DelegateCommand<object>(this.OnTestCommand, this.CanTestCommand);

      this.NotificationsCommand =
        new DelegateCommand<object>(this.OnNotificationsCommand, this.CanNotificationsCommand);
      CloseDMSQuickSettingsCommand = new DelegateCommand(ExecuteCloseDMSQuickSettings, () => true); 
      this.NotificationsChangeCommand =
        new DelegateCommand<object>(this.OnNotificationsChangeCommand, this.CanNotificationsChangeCommand);
      GoToMoreSettingsCommand = new DelegateCommand(ExecuteGotoMoreSettingsCommand, () => true);

      this.OcclusionPressureSettingsChangeCommand = new DelegateCommand<object>(
        this.OnOcclusionPressureSettingsChangeCommand, this.CanOcclusionPressureSettingsChangeCommand);

      this.IncreaseTimeCommand = new DelegateCommand<object>(this.OnIncreaseTimeCommand, this.CanIncreaseTimeCommand);

      this.DecreaseTimeCommand = new DelegateCommand<object>(this.OnDecreaseTimeCommand, this.CanDecreaseTimeCommand);

      this.AblationSiteCommand = new DelegateCommand<object>(this.OnAblationSiteCommand, this.CanAblationSiteCommand);

      this.TreatmentNotesCommand =
        new DelegateCommand<object>(this.OnTreatmentNotesCommand, this.CanTreatmentNotesCommand);

      this.DeflateAfterThawCommand =
        new DelegateCommand<object>(this.OnDeflateAfterThawCommand, this.CanDeflateAfterThawCommand);

      this.VeinIsolatedCommand = new DelegateCommand<object>(this.OnVeinIsolatedCommand, this.CanVeinIsolatedCommand);

      this.UpdateVeinIsolationDurationCommand = new DelegateCommand<object>(this.OnUpdateVeinIsolationDurationCommand,
        this.CanUpdateVeinIsolationDurationCommand);

      this.ChangeTankCommand = new DelegateCommand<object>(this.OnChangeTankCommand, this.CanChangeTankCommand);

      this.LockTheFootSwitchCommand =
        new DelegateCommand<object>(this.OnLockTheFootSwitchCommand, this.CanLockTheFootSwitchCommand);

      this.EnableDASBallonCommand =
        new DelegateCommand<bool?>(this.OnEnableDASBalloonCommand).ObservesCanExecute(() => CanExecuteEnableDASBalloonCommand);

      this.ResetLSPROCommand = new DelegateCommand<object>(this.OnResetLSPROCommand, this.CanResetLSPROCommand);

      this.ActivateLowFlowCommand =
        new DelegateCommand<object>(this.OnActivateLowFlowCommand, this.CanActivateLowFlowCommand);

      this.ResetDiaphragmCommand =
        new DelegateCommand<object>(this.OnResetDiaphragmCommand, this.CanResetDiaphragmCommand);

      this.SaveDMSSettingCommand =
        new DelegateCommand<object>(this.OnSaveDMSSettingCommand, this.CanSaveDMSSettingCommand);

      this.VolumeControlOnCommand =
        new DelegateCommand<object>(this.OnVolumeControlOnCommand, this.CanVolumeControlOnCommand);

      this.VolumeControlOffCommand =
        new DelegateCommand<object>(this.OnVolumeControlOffCommand, this.CanVolumeControlOffCommand);

      this.SaveOcclusionPressureGraphSettingsCommand = new DelegateCommand<object>(
        this.OnSaveOcclusionPressureGraphSettingsCommand, this.CanSaveOcclusionPressureGraphSettingsCommand);
      CloseOcclusionPressureSettingsCommand = new DelegateCommand(ExecuteCloseOcclusionPressureSettingsCommand, () => true);

      this.TareOcclusionPressureGraphCommand = new DelegateCommand<object>(this.OnTareOcclusionPressureGraphCommand,
        this.CanTareOcclusionPressureGraphCommand);

      this.ResetTareOcclusionPressureGraphCommand = new DelegateCommand<object>(
        this.OnResetTareOcclusionPressureGraphCommand, this.CanResetTareOcclusionPressureGraphCommand);

      this.dataAccess = CommonViewModel.Current.Data.DataAccess;

      CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;

      TimerProcedureElapsedTime.Interval = 1000_000;
      TimerProcedureElapsedTime.MicroTimerElapsed += timerProcedureElapsedTime_Tick;

      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.HighResolutionDmsReading))
        .Select(_ => CommonViewModel.Current.HighResolutionDmsReading)
        .Subscribe(d =>
        {
          lock (_highResDmsReading)
          {
            if (_highResDmsReading.Count >= 250) _highResDmsReading.Clear();
            _highResDmsReading.Add(d);
          }

          HighResDmsSignalDetected = true;
          _highResDmsDataStopWatcher.Restart();
        });

      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.HighResolutionDmsReading))
        .SelectMany(_ => CommonViewModel.Current.HighResolutionDmsReading)
        .Window(TimeSpan.FromMilliseconds(1000), TaskPoolScheduler.Default)
        .Subscribe(CalculateMaxAvgHRPacingLevel);

      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.CurrentBloodPressureValue))
        .Select(_ => CommonViewModel.Current.CurrentBloodPressureValue)
        .Subscribe(d =>
        {
          lock (_bloodPressureReading)
          {
            if (_bloodPressureReading.Count >= 100) _bloodPressureReading.Clear();
            _bloodPressureReading.Add(d);
          }
        });

      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.EcgChannel3And4Reading))
        .Select(_ => CommonViewModel.Current.EcgChannel3And4Reading)
        .Subscribe(d =>
        {
          lock (_ecgChannel3And4Reading)
          {
            if (!_highResDmsSignalDetected)
            {
              if (_ecgChannel3And4Reading.Count >= 100) _ecgChannel3And4Reading.Clear();
              _ecgChannel3And4Reading.Add(d);
            }
          }

          if (HighResDmsSignalDetected &&
              _highResDmsDataStopWatcher.IsRunning && _highResDmsDataStopWatcher.ElapsedMilliseconds > 3000)
          {
            HighResDmsSignalDetected = false;
            _highResDmsDataStopWatcher.Reset();
            var clear = HighResDmsData;
          }
        });

      SubscribeBloodPressureSensorDataUpdate();

      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.EcgChannel3And4Reading))
        .Select(_ => CommonViewModel.Current.EcgChannel3And4Reading)
        .Window(TimeSpan.FromMilliseconds(1000), TaskPoolScheduler.Default)
        .Subscribe(CalculateMaxAvgPacingLevel);

      if (this.ecgEvent == null)
      {
        this.ecgEvent = new EcgEventArgs();
      }

      Languages.LanguageChangedEvent += Languages_LanguageChangedEvent;

      DatabaseVersion = CommonViewModel.Current.DatabaseVersion;

      GUIVersion = CommonViewModel.Current.GuiVersion;

      // _canDisplayShadowGraph shall be read from preference. initialize it to true here for default, if will be updated in view loaded event handler
      _canDisplayShadowGraph = true;
      _ablationSiteObservable = new BehaviorSubject<AblationSiteEnum>(AblationSite);

      _ablationSiteObservable
        .Throttle(TimeSpan.FromSeconds(3.0))
        .Subscribe(site => { ValidateUpdatingShadowTemperatureGraph(); });

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => (e.EventArgs.PropertyName == nameof(DiaphragmAmplitude) ||
                     e.EventArgs.PropertyName == nameof(EsophagusTemperature) ||
                     e.EventArgs.PropertyName == nameof(IgnoreMinimumDiaphragmMovementValue) ||
                     e.EventArgs.PropertyName == nameof(RequiredTargetTemperature) ||
                     e.EventArgs.PropertyName == nameof(ThawTimerToTemperature) ||
                     e.EventArgs.PropertyName == nameof(IsUsingAudioAlertSetting)) &&
                    !isTreatmentNumberAndPlayBackVisible)
        .Select(e => e.EventArgs.PropertyName)
        .Subscribe(UpdateBindingProperties);

      ResetDisplayWithPhysicianPreferences();

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => !IsSettingsDirty && !_isInitializing &&
                    _settingsPropertyNameList.Contains(e.EventArgs.PropertyName))
        // .Where(e => !_isInitializing && _settingsPropertyNameList.Contains(e.EventArgs.PropertyName))
        .Subscribe(e => IsSettingsDirty = true);

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(DiaphragmAmplitude) ||
                    e.EventArgs.PropertyName == nameof(DiaphragmSensorGain) ||
                    e.EventArgs.PropertyName == nameof(DMSDetectionThresholdValue) ||
                    e.EventArgs.PropertyName == nameof(EsophagusTemperature) ||
                    e.EventArgs.PropertyName == nameof(IgnoreMinimumDiaphragmMovementValue) ||
                    e.EventArgs.PropertyName == nameof(IsUsingAudioAlertSetting) || 
                    e.EventArgs.PropertyName == nameof(IsUsingAudioAlertMute))
        .Subscribe(_ =>
            {
              if (IsDMSSettingPopupShow)
                this._dmsQuickSettingsRefreshSubject.OnNext(true);
            });


      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(OcclusionPressureGraphAxisYMaximum) ||
                    e.EventArgs.PropertyName == nameof(OcclusionPressureGraphAxisYMinimum) ||
                    e.EventArgs.PropertyName == nameof(OcclusionPressureGraphSweepSpeed))
        .Subscribe(_ =>
          {
            if (IsBloodPressureSettingsPopupShow)
              this._occlusionPressureSettingsRefreshSubject.OnNext(true);
          });

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(TipOrBalloonPressureReading))
        .Subscribe( _ => EvaluateIBPWithPressureSetPoint(CommonViewModel.Current.CP1Reading));

      // Update the Ecg Multi-sensor data and status every 250ms for performance  
      Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(_ =>
          {
            if (IsMultiEtsSesnorConnected && !CommonViewModel.Current.AreSensorsInPlayBackMode)
            {
              this.UpdateEcgSensorDataPropertyChanged();
              EcgSensorDataPlayback = CommonViewModel.Current.EcgSensorData; 
            }
          });

      _notifyAblationSiteChangedSubject
        .Throttle(TimeSpan.FromSeconds(maximumNotificationValueIndex))
        .Subscribe(_ => { IsAblationSiteChanged = false; });

      CommonViewModel.Current.RCBalloonDiameterButtonPressedObserver
        .Subscribe(increase =>
        {
          if (CanExecuteEnableDASBalloonCommand && ((increase && !DASBalloonEnabled) || (!increase && DASBalloonEnabled)))
          {
            EnableDASBallonCommand?.Execute(increase);
          }
        });

      CurrentUser = CommonViewModel.Current.CurrentUser;
      RaisePropertyChanged(nameof(CurrentUser));
    }

    private void UpdateBindingProperties(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(DiaphragmAmplitude):
          DiaphragmBindingAmplitude = DiaphragmAmplitude;
          break;
        case nameof(EsophagusTemperature):
          EsophagusBindingTemperature = EsophagusTemperature;
          break;
        case nameof(IgnoreMinimumDiaphragmMovementValue):
          IgnoreMinimumDiaphragmMovementBindingValue = IgnoreMinimumDiaphragmMovementValue;
          break;
        case nameof(RequiredTargetTemperature):
          RequiredTargetTemperatureBinding = RequiredTargetTemperature;
          break;
        case nameof(ThawTimerToTemperature):
          ThawTimerToTemperatureBinding = ThawTimerToTemperature;
          break;
        case nameof(IsUsingAudioAlertSetting):
          IsUsingAudioAlert = IsUsingAudioAlertSetting;
          break;
      }

    }

    /// <summary>
    /// Setup tracing log for sending data to LSPro.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void SetupTrace()
    {
      string logFileName = "LSProTrace" + "_" + DateTime.UtcNow.Date.Year.ToString() + "_" +
                           DateTime.UtcNow.Date.Month.ToString() + DateTime.UtcNow.Date.Day + "_" +
                           DateTime.UtcNow.Ticks.ToString() + ".txt";
      string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
      string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
      var pathLogFile = Path.Combine(strWorkPath, "LSProLogs");

      if (!Directory.Exists(pathLogFile))
      {
        Directory.CreateDirectory(pathLogFile);
      }

      string[] path = { strWorkPath, "LSProLogs", logFileName };
      var pathString = Path.Combine(path);
      var fileListener = new TextWriterTraceListener(pathString);

      Debug.Listeners.Add(fileListener);
      Debug.AutoFlush = true;
      Debug.WriteLine("LS Pro Trace Log:", DateTime.Now.ToString());
    }

    /// <summary>
    /// Log trace for sending data to LSPro.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dt">Timestamp</param>
    /// <param name="ablationTime">Ablation Time</param>
    /// <param name="balloonTemperature">Balloon Temperature</param>
    /// <param name="stateId">Current State</param>
    private void LogTraceMsg(DateTime dt, int ablationTime, double balloonTemperature, MessageStateId stateId)
    {
      string tagAblation = "  Ablation Time: " + ablationTime.ToString() + ", ";
      string tagBollonTemperature = "Balloon Temperature: " + balloonTemperature.ToString("F0") + ", ";
      string tagSystemState = "System State: " + stateId;
      string msg = dt.ToString() + tagAblation + tagBollonTemperature + tagSystemState;
      Debug.WriteLine(msg);
    }

    /// <summary>
    /// This property gets the Minimum DMS Detection Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double DMSDetectionMinValue
    {
      get { return CommonViewModel.Current.ConnectionBox.DiaphragmeMinimumValue; }
    }

    /// <summary>
    /// This property gets the Minimum DMS Detection Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double DMSDetectionMaxValue
    {
      get { return CommonViewModel.Current.ConnectionBox.DiaphragmeMaximumValue; }
    }

    private bool _highResDmsSignalDetected;

    /// <summary>
    /// This property gets/sets if high resolution DMS signal received 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool HighResDmsSignalDetected
    {
      get => _highResDmsSignalDetected;
      set => SetProperty(ref _highResDmsSignalDetected, value);
    }

    private bool _isInDASBalloonTransition = false;

    public bool CanExecuteEnableDASBalloonCommand => AllowPSPChangeDuringThawing 
                                                     && !_isInDASBalloonTransition 
                                                     && (!DASBalloonEnabled || IsBalloonRampDownActivated) ; 

    public bool IsInDASBalloonTransition
    {
      get => _isInDASBalloonTransition;
      set
      {
        SetProperty(ref _isInDASBalloonTransition, value);
        RaisePropertyChanged(nameof(CanExecuteEnableDASBalloonCommand));
        CommonViewModel.Current.UpdateAllowStartAblationState(!value);
      }
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void Languages_LanguageChangedEvent(object sender, EventArgs e)
    {
      //IsLanguageChanged = true;
    }

    /// <summary>
    /// This function is trigerred at each Ablation Timer tick to notify its listeners that an ablation event
    /// occurred.  It allows Temperature Chart's serie to be updated in "real time"
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The ablation event arguments.</param>
    protected virtual void OnSystemStateChanged(object sender, AblationEventArgs e)
    {
      SystemStateEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// This function is trigerred when the system state falls in Ready.  It invokes the ReadyStateEvent
    /// that will be used to reset the display for the occlusion pressure graph (clears the Occlusion Pressure Chart serie)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The inflation event arguments.</param>
    protected virtual void OnReady(object sender, EventArgs e)
    {
      ReadyStateEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// This function is trigerred when the system state falls in Inflation.  It invokes the InflationStateEvent
    /// that will be used to reset the display for an ablation (clears the Temperature Chart serie)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The inflation event arguments.</param>
    protected virtual void OnInflation(object sender, InflationEventArgs e)
    {
      InflationStateEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// This function is called at each Timer Ablation's Tick.  It is used to update Cryotherapy
    /// screen display controls and manages to procedure stop by evaluating its timers
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void timerAblation_Tick(object sender, EventArgs e)
    {
      // we use that to don t load the view buy many event that are handled automatcily by prism
      double TC1Reading = CommonViewModel.Current.TC1Reading;
      Communication.CanBusMessageDefinition.MessageStateId currentState = CommonViewModel.Current.SystemState;
      string error = CommonViewModel.Current.GenericError;
      IsSnowFlakeVisible = true;

      if (this.ablationEvent == null)
      {
        this.ablationEvent = new AblationEventArgs();
      }

      this.CryoTherapyTime++;
      this.TotalCryoTherapyTime++;

      // Temperature saved in a temp variable to help with the temperature delay display on the LSPro
      double temperatureAtTickLSPro = TC1Reading;

      this.ablationEvent.Temperature = TC1Reading;
      this.ablationEvent.Compter = this.CryoTherapyTime;
      this.ablationEvent.AblationID = this.AblationNumber;

      try
      {
        //Here we want to be sure that the Ablation object is already built
        if (CurrentAblation != null)
        {
          //this.dataAccess.AddAblationData(CurrentAblation.ID, this.TC1Reading, this.CryoTherapyTime);
          OnSystemStateChanged(null, ablationEvent);
        }

        TemperatureRate = TC1Reading - previousTemperature;
        previousTemperature = TC1Reading;
        if (TC1Reading < MaxTemperatureRate)
        {
          MaxTemperatureRate = TC1Reading;
        }

        if (TC1Reading <= RequiredTargetTemperature && TimeToTargetTemperature == 0)
        {
          TimeToTargetTemperature = this.CryoTherapyTime;
          IsTimeToTargetTemperatureVisible = true;
          IsTargetTemperatureReached = true;
        }

        if (currentState == MessageStateId.CAN_ID_STATE_THAWING && !IsThawTemperatureReached)
        {
          TimeToThawTemperature++;

          if (TC1Reading >= ThawTimerToTemperature)
          {
            IsThawTemperatureReached = true;
          }
        }

        //Capture data to save
        if (TreatmentNumberRefrence != 0)
        {
          AddAblationData();
        }

        // Send data to LSPro
        if (CommonViewModel.Current.IsLsproInitialized)
        {
          if (currentState == MessageStateId.CAN_ID_STATE_TRANSITION ||
              currentState == MessageStateId.CAN_ID_STATE_ABLATION)
          {
            CommonViewModel.Current.SendTimeAndTemperature(CryoTherapyTime, temperatureAtTickLSPro,
              (int)Enumeration.LSPROConsoleStatus.start, AblationNumber);

            cryotherapyEndTime = CryoTherapyTime;
          }
        }

        if (CryoTherapyTime >= RequiredAblationTime && (currentState == MessageStateId.CAN_ID_STATE_TRANSITION ||
                                                        currentState == MessageStateId.CAN_ID_STATE_ABLATION)
                                                    && CommonViewModel.Current.IsAblationProcedureStarted)
        {
          //Stop the ablation procedure, but keep the ablation timer running.
          StopAblationProcedure();
        }
      }
      catch (Exception exception)
      {
        LogException(exception);
        DispatcherBeginInvoke(
          () =>
            {
              Tuple<long, string, string, string> genericMessage =
                Models.Languages.ErrorsAndCryterionSolutionTranslations(
                  (int)Enumeration.GUIMessages.ID3,
                  (int)Enumeration.ErrorTypes.GUI);
              MessagePopup dialogPopup = new MessagePopup(
                genericMessage,
                MessagePopup.MessageType.ErrorMessage,
                MessagePopup.ButtonType.Ok);
              dialogPopup.ShowDialog();
            });
      }

      //Verify if the Vein is isolated and take action
      HandleAblationTimerAccordingToveinIsolationLogic();
    }

    /// <summary>
    /// This function is called at each Timer Procedure Elapsed Time's Tick.  It manages the procedure's elapsed time
    /// counter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments (not used in this function).</param>
    private void timerProcedureElapsedTime_Tick(object sender, EventArgs e)
    {
      if (CPUTimeWatchdog.TotalMillisconds == 0)
        CPUTimeWatchdog.StartTimeMonitoring();

      if (ElapsedTime > 5 &&
          ((CPUTimeWatchdog.StopwatchVerificator.Elapsed.TotalMilliseconds - CPUTimeWatchdog.TotalMillisconds) < 850) &&
          !isAblating && !isThawing)
      {
        CPUTimeWatchdog.TotalMillisconds = 0;
        return;
      }
      else
      {
        CPUTimeWatchdog.TotalMillisconds = CPUTimeWatchdog.StopwatchVerificator.Elapsed.TotalMilliseconds;
      }

      if (isAblating)
        timerAblation_Tick(null, null);

      if (isThawing)
        timerThawing_Tick(null, null);

      // Send data to LSPro
      if (CommonViewModel.Current.IsLsproInitialized)
      {
        if (CommonViewModel.Current.SystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION &&
            CommonViewModel.Current.SystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
        {
          CommonViewModel.Current.SendTimeAndTemperature(cryotherapyEndTime, CommonViewModel.Current.tC1LSProReading,
            (int)Enumeration.LSPROConsoleStatus.stop, AblationNumber);
#if DEBUG || Simulator
          //LogTraceMsg(DateTime.UtcNow, cryotherapyEndTime, CommonViewModel.Current.tC1LSProReading, CommonViewModel.Current.SystemState);
#endif
        }
      }

      ElapsedTime += 1;

      if (ElapsedTime >= maxElapsedTime)
        ElapsedTime = 0;

      RaisePropertyChanged("CurrentTime");
      //RaisePropertyChanged("ElapsedTimeMinute");

      if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_INFLATION ||
          CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_TRANSITION ||
          CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_ABLATION ||
          CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_THAWING)
      {
        if (IsEsophagusTemperatureConditionAlertsMeet ||
            ((DiaphragmAmplitudeThresholdReached || EsophagusTemperatureThresholdReached) &&
             CommonViewModel.Current.SystemState !=
             Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION
             && CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_THAWING))
        {
          if (IsSqaureVisible)
          {
            IsSqaureVisible = false;
            IsStatusAbllationBallonVisible = false;
          }
          else
          {
            IsSqaureVisible = true;
            IsStatusAbllationBallonVisible = true;
          }
        }
        else
        {
          if (isAblating && CommonViewModel.Current.SystemState !=
              Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
          {
            if (IsSqaureVisible)
            {
              IsSqaureVisible = false;
              IsStatusAbllationBallonVisible = false;

            }
            else
            {
              IsSqaureVisible = true;
              IsStatusAbllationBallonVisible = true;
            }
          }
          else
          {
            IsSqaureVisible = true;
          }
        }

        if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_THAWING)
        {
          if (TC1Reading >= thawTemperature)
          {
            AllowPSPChangeDuringThawing = true;
          }

          ThawingElapsedTime++;

          if (TC1Reading >= thawTemperature) //    if (ThawingElapsedTime > expectedThawingTime)
          {
            DisplayThawingBallon = !DisplayThawingBallon;
          }
        }

        if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_ABLATION && !IsLowFlowActivated)
        {
          AllowUserToActivateLowFlow = true;
        }
      }

      if ((CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_IDLE ||
           CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_READY ||
           CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_EXCEPTION ||
           CommonViewModel.Current.GenericError != string.Empty) && !ProcedureLogModel.CanReloadProcudreInformation)
      {
        if (TotalTreatmentNumber != 0 && (PreviuosTotalTreatmentNumber != TotalTreatmentNumber))
        {

          if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_EXCEPTION &&
              CommonViewModel.Current.GenericError == string.Empty && TimingFiliter == 0)
          {
            TimingFiliter = 1;
            return;
          }

          IsSystemMonitoringDiaphragmAlert = false;
          StopAblationTimer();
          StopThawAndTimer();

          if (CommonViewModel.Current.GenericError != string.Empty)
          {
            CommonViewModel.Current.SendTimeAndTemperature(CryoTherapyTime, TC1ReadingErrorValue,
              (int)Enumeration.LSPROConsoleStatus.stop, AblationNumber);
            CommonViewModel.Current.SendTimeAndTemperature(CryoTherapyTime, TC1ReadingErrorValue,
              (int)Enumeration.LSPROConsoleStatus.stop, AblationNumber);
            ExceptionStateTime = ++CryoTherapyTime;
            if (TreatmentNumberRefrence != 0)
            {
              AddAblationData();
            }
          }

          IsWritingDataToFile = true;
          IsWritingECGDataToFile = true;

          PreviuosTotalTreatmentNumber = TotalTreatmentNumber;
          PreviousSystemState = CommonViewModel.Current.SystemState;

          WriteAblationDataToFileAsync();

          IsWritingDataToFile = false;
          IsWritingECGDataToFile = false;

          AllowUserToActivateLowFlow = false;
          IsLowFlowActivated = false;

          CommonViewModel.Current.IsUsingAutoPlayback = IsUsingAutoPlayback;

          ManageExceptionDataLoading("AblationEnded");
        }
        else if (TotalTreatmentNumber != 0 && !CommonViewModel.Current.IsPlayBackModeDeactivted)
        {
          if (IsLoadingAbortedAblation)
          {
            IsLoadingAbortedAblation = false;
            SensorReadingMananger.ConnectSensors();
            CommonViewModel.Current.AreSensorsInPlayBackMode = false;
            ResetCryoTherapyPlayBackData();
            CommonViewModel.Current.IsPlayBackModeDeactivted = true;
          }
        }

        if ((CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_IDLE ||
             CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_READY) && IsLoadingAbortedAblation)
        {
          IsLoadingAbortedAblation = false;
          SetPlayBackMode();
        }
      }

      UpdateSkinToSkinTime();
    }

    /// <summary>
    /// This function is called at each Timer Thawing's tick.  It manages the Thaw timer and ablation data list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments (not used in this function).</param>
    private void timerThawing_Tick(object sender, EventArgs e)
    {
      IsIsolatingVein = false;
      MessageStateId currentState = CommonViewModel.Current.SystemState;
    }

    /// <summary>
    /// This function manages the Exception when loading data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal void ManageExceptionDataLoading(object param)
    {
      try
      {
        if (SensorReadingMananger.AllowPlayback)
        {
          if (param != null && param.ToString() == "AblationEnded")
          {
            ResetAblationTimeSettings();
            WasAblationTimeManuallyChanged = false;

            if (IsUsingAutoPlayback)
            {
              IsTreatmentNumberAndPlayBackVisible = true;
              IsAblationTimeVisibale = true;

              SensorReadingMananger.DisconnectSensors();
            }
            else
            {
              AblationInformation.IsThereAbltionHistoricalData = true;
              IsTreatmentNumberAndPlayBackVisible = false;
              IsAblationTimeVisibale = false;
              CommonViewModel.Current.IsPlayBackModeDeactivted = true;

              SensorReadingMananger.ConnectSensors();
            }
          }
          else
          {
            IsTreatmentNumberAndPlayBackVisible = true;
            IsAblationTimeVisibale = true;
            SensorReadingMananger.DisconnectSensors();
          }
        }

        if (!IsLastAblationDataLoaded)
        {
          IsLastAblationDataLoaded = true;
          // No need to reload all ablation data everytime
          // LoadAllAblationDataFromFile();  

          LoadLastAblationData();

          if ((param == null || param.ToString() == "AblationEnded") && !ProcedureLogModel.CanReloadProcudreInformation)
          {
            CommonViewModel.Current.GenerateAblationSummary();
          }
        }

        IsStatusAbllationBallonVisible = false;

#if Simulator
        //Set the system state to IDLE when an exception occurs to reproduces the console behavior when it happens.
        CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
#endif
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// This property gets/sets Current Ablation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Ablation CurrentAblation
    {
      get => CommonViewModel.Current.CurrentAblation;
      set => RaisePropertyChanged("CurrentAblation");
    }

    /// <summary>
    /// This property gets/sets display warning value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DisplayAblationSiteWarning
    {
      get { return this.displayAblationSiteWarning; }
      set
      {
        this.displayAblationSiteWarning = value;
        RaisePropertyChanged("DisplayAblationSiteWarning");
      }
    }

    /// <summary>
    /// This read-only returns the Ablation Summary value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AblationSummary AblationSummary => CommonViewModel.Current.AblationSummary;

    /// <summary>
    /// This property gets/sets the Tip Pressure Selected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool TipPressureSelected
    {
      get { return TipBalloonPressureSelection.TipPressureSelected; }
      set
      {
        if (TipBalloonPressureSelection.TipPressureSelected != value)
        {
          TipBalloonPressureSelection.TipPressureSelected = value;
          RaisePropertyChanged("TipPressureSelected");
          TipOrBalloonPressureSelectionChangedEvent?.Invoke(null, null);
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Movement Percentage Selected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DiaphragmMovementPercentageSelected
    {
      get { return diaphragmMovementPercentageSelected; }
      set
      {
        if (this.diaphragmMovementPercentageSelected != value)
        {
          this.diaphragmMovementPercentageSelected = value;
          RaisePropertyChanged("DiaphragmMovementPercentageSelected");
          DiaphragmMovementUnitChangedEvent?.Invoke(null, null);
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Temperature Chart Type value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public short TemperatureChartType
    {
      get { return temperatureChartType; }
      set
      {
        if (temperatureChartType != value)
        {
          this.temperatureChartType = value;
          RaisePropertyChanged(nameof(TemperatureChartType));
          TemperatureChartTypeChangedEvent?.Invoke(value, null);
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Refrigerant Level Unit value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public short RefrigerantLevelUnit
    {
      get { return refrigerantLevelUnit; }
      set
      {
        this.refrigerantLevelUnit = value;
        RaisePropertyChanged("RefrigerantLevelUnit");
      }
    }

    /// <summary>
    /// This property gets/sets the Is Diaphragm movement detected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDiaphragmMovementDetected
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          return (CommonViewModel.Current.IsDiaphragmMovementDetected); // & IsSystemMonitoringDiaphragmAlert);
        }
        else
        {
          return true;
        }
      }
      set
      {
        isDiaphragmMovementDetected = value;
        RaisePropertyChanged("IsDiaphragmMovementDetected");
      }
    }

    private bool _ignoreDiaphragmMovementBindingValue;

    public bool IgnoreMinimumDiaphragmMovementBindingValue
    {
      get => _ignoreDiaphragmMovementBindingValue;
      set => SetProperty(ref _ignoreDiaphragmMovementBindingValue, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the diaphragm movement is monitored
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IgnoreMinimumDiaphragmMovementValue
    {
      get { return CommonViewModel.Current.IgnoreMinimumDiaphragmMovementValue; }

      set
      {
        if (IsSavedToDB) IsSavedToDB = false;
        CommonViewModel.Current.IgnoreMinimumDiaphragmMovementValue = value;
        if (!isTreatmentNumberAndPlayBackVisible)
          IgnoreMinimumDiaphragmMovementBindingValue = value;
        RaisePropertyChanged(nameof(IgnoreMinimumDiaphragmMovementValue));
      }
    }

    /// <summary>
    /// This read-only property returns the Tip or Balloon Pressure Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TipOrBalloonPressureReading
    {
      get
      {
        return TipPressureSelected ? EcgChannel1And2Reading : CP1Reading;
      }
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Movement Percentage or G Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double DiaphragmMovementPercentageOrGReading
    {
      get
      {
        if (!CommonViewModel.Current.AreSensorsInPlayBackMode)
        {
          if (ElapsedTime - ElapsedTimeLastValue >= 1)
          {
            ElapsedTimeLastValue = ElapsedTime;
            LastDiaphragmMovementPercentageOrGReadingValue = DiaphragmMovementPercentageSelected
              ? EcgChannel7And8Reading
              : (int)EcgChannel3And4Reading;
            
            EcgChannel7And8ReadingPlayback = LastDiaphragmMovementPercentageOrGReadingValue; 

            return LastDiaphragmMovementPercentageOrGReadingValue;
          }
          else
          {
            return LastDiaphragmMovementPercentageOrGReadingValue;
          }
        }
        else
        {
          //When in playback
          return EcgChannel7And8ReadingPlayback;
        }
      }
      set { RaisePropertyChanged("DiaphragmMovementPercentageOrGReading"); }
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Maximum movement value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaximumAveragePacingLevel
    {
      get => _maximumAveragePacingLevel;
      set => SetProperty(ref _maximumAveragePacingLevel, value);
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Maximum movement value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaximumHRAveragePacingLevel
    {
      get => _maximumHRAveragePacingLevel;
      set => SetProperty(ref _maximumHRAveragePacingLevel, value);
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
    /// This function stops the Thaw procedure
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StopThawAndTimer()
    {
      if (isThawing) //if (timerThawing.IsEnabled)
      {
        isThawing = false; //this.timerThawing.Stop();
      }
    }

    /// <summary>
    /// This function Stops the Ablation Timer and updates the Snow Flake visibility flag
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StopAblationTimer()
    {
      if (isAblating) //if (timerAblation.IsEnabled)
      {
        isAblating = false; //timerAblation.Stop();

        IsSnowFlakeVisible = false;
        IsStatusAbllationBallonVisible = false;
      }
    }

    public double TEMPTTI
    {
      get { return tempIIT; }
      set
      {
        tempIIT = value;
        RaisePropertyChanged("TEMPTTI");
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
        if (SensorReadingMananger.AreSensorsConnected)
        {
          double tC1Reading = CommonViewModel.Current.TC1Reading;
          IsThawingTemperatureSetPointReached = tC1Reading >= CommonViewModel.Current.ThawingTemperatureSetPoint;
          TC1ReadingPlayback = tC1Reading;
          return tC1Reading;
        }
        else
          return TC1ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.TC1Reading = value;
        else
          TC1ReadingPlayback = value;
        RaisePropertyChanged("TC1Reading");
      }
    }

    private bool catheterIsConnecting = false;

    /// <summary>
    /// This property gets/sets the Catheter is connecting value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool CatheterIsConnecting
    {

      get { return catheterIsConnecting; }
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
    /// This property gets/sets the CP1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP1Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          CP1ReadingPlayback = CommonViewModel.Current.CP1Reading;
          return CommonViewModel.Current.CP1Reading;
        }
        else
          return CP1ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          CommonViewModel.Current.CP1Reading = value;
        }
        else
          CP1ReadingPlayback = value;

        RaisePropertyChanged("CP1Reading");
        RaisePropertyChanged("TipOrBalloonPressureReading");
      }
    }

    /// <summary>
    /// This property gets/sets the PT2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT2Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.PT2Reading;
        else
          return PT2ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PT2Reading = value;
        else
          PT2ReadingPlayback = value;

        RaisePropertyChanged("PT2Reading");
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
        if (SensorReadingMananger.AreSensorsConnected)
        {
          if (ElapsedTime - ElapsedTimeLastValueForFlowReading >= 1)
          {
            ElapsedTimeLastValueForFlowReading = ElapsedTime;
            LastFlowReadingValue = CommonViewModel.Current.FM1Reading;
            return LastFlowReadingValue;
          }
          else
          {
            return LastFlowReadingValue;
          }
        }
        else
          return FM1ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.FM1Reading = value;
        else
          FM1ReadingPlayback = value;

        RaisePropertyChanged("FM1Reading");
      }
    }

    /// <summary>
    /// Gets/sets the CP2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP2Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.CP2Reading;
        else
          return CP2ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.CP2Reading = value;
        else
          CP2ReadingPlayback = value;

        RaisePropertyChanged("CP2Reading");
      }
    }

    /// <summary>
    /// This property gets/sets the Max Ecg Channel 1 And 2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel1And2Reading
    {
      get
      {
        double localMaxEcgChannel1And2Reading = CommonViewModel.Current.MaxEcgChannel1And2Reading;
        CommonViewModel.Current.MaxEcgChannel1And2Reading = 0;

        if (SensorReadingMananger.AreSensorsConnected)
          return localMaxEcgChannel1And2Reading;
        else
          return MaxEcgChannel1And2ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.MaxEcgChannel1And2Reading = value;
        else
          MaxEcgChannel1And2ReadingPlayback = value;

        RaisePropertyChanged("MaxEcgChannel1And2Reading");
      }
    }

    /// <summary>
    /// This property gets/sets the Ecg Channel 1 And 2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel1And2Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.EcgChannel1And2Reading;
        else
          return EcgChannel1And2ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel1And2Reading = value;
        else
          EcgChannel1And2ReadingPlayback = value;

        RaisePropertyChanged("EcgChannel1And2Reading");
        RaisePropertyChanged("TipOrBalloonPressureReading");
      }
    }

    /// <summary>
    /// This property gets/sets the Ecg Channel 3 And 4 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel3And4Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.EcgChannel3And4Reading;
        else
          return EcgChannel3And4ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel3And4Reading = value;
        else
          EcgChannel3And4ReadingPlayback = value;

        RaisePropertyChanged("EcgChannel3And4Reading");
      }
    }

    /// <summary>
    /// This property gets/sets the Max Ecg Channel 3 And 4 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel3And4Reading
    {
      get
      {
        double localMaxEcgChannel3And4Reading = CommonViewModel.Current.MaxEcgChannel3And4Reading;
        CommonViewModel.Current.MaxEcgChannel3And4Reading = 0;

        if (SensorReadingMananger.AreSensorsConnected)
          return localMaxEcgChannel3And4Reading;
        else
          return MaxEcgChannel3And4ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.MaxEcgChannel3And4Reading = value;
        else
          MaxEcgChannel3And4ReadingPlayback = value;

        RaisePropertyChanged("MaxEcgChannel3And4Reading");
        RaisePropertyChanged("DiaphragmMovementPercentageOrGReading");
      }
    }

    /// <summary>
    /// This property gets/sets the Ecg Channel 5 And 6 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel5And6Reading
    {
      get
      {
        Random rnd = new Random();
        LowestTempChannelNum.Clear();
        if (SensorReadingMananger.AreSensorsConnected)
        {
          double _EcgChannel5And6Reading = CommonViewModel.Current.EcgChannel5And6Reading;
          if (_EcgChannel5And6Reading < 10 || _EcgChannel5And6Reading > 40)
          {
            IsEsophagusTemperatureInRange = true;
          }
          else
          {
            IsEsophagusTemperatureInRange = false;
          }

          EcgChannel5And6ReadingPlayback = _EcgChannel5And6Reading;

          return _EcgChannel5And6Reading;
        }
        else
          return EcgChannel5And6ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel5And6Reading = value;
        else
          EcgChannel5And6ReadingPlayback = value;

        RaisePropertyChanged("EcgChannel5And6Reading");
      }
    }

    public List<double> EcgSensorDataPlayback { get; set; }

    public List<double> EcgSensorData
    {
      get => SensorReadingMananger.AreSensorsConnected
           ? CommonViewModel.Current.EcgSensorData
           : EcgSensorDataPlayback;
      set
      {
        if (!SensorReadingMananger.AreSensorsConnected)
        {
          EcgSensorDataPlayback = value;
        }
        RaisePropertyChanged();
      }
    }

    private List<bool> _channelStatus;  
    public List<bool> EcgChannelStatus
    {
      get
      {
        lock (ETSdataSortingAndStatus.ChannelStatus)
        {
          _channelStatus = new List<bool>(ETSdataSortingAndStatus.ChannelStatus);
        }

        return _channelStatus;
      }
      set
      {
        this.RaisePropertyChanged();
      }
    }

    private void UpdateEcgSensorDataPropertyChanged()
    {
      this.RaisePropertyChanged(nameof(this.EcgSensorData));
      this.RaisePropertyChanged(nameof(EcgChannelStatus));
    } 

    public List<int> ListOfSesnorsState
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          return CommonViewModel.Current.ListOfSesnorsState;
        }
        else
        {
          return ListOfSesnorsStatePlayback;
        }
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          CommonViewModel.Current.ListOfSesnorsState = value;
        }
        else
        {
          ListOfSesnorsStatePlayback = value;
        }

        RaisePropertyChanged("ListOfSesnorsState");
      }
    }


    /// <summary>
    /// This property gets/sets the list of sesnors state playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<int> ListOfSesnorsStatePlayback { get; set; }



    /// <summary>
    /// This property gets/sets the Ecg Channel 7 And 8 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel7And8Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.EcgChannel7And8Reading;
        else
          return EcgChannel7And8ReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.EcgChannel7And8Reading = value;
        else
          EcgChannel7And8ReadingPlayback = value;

        RaisePropertyChanged("EcgChannel7And8Reading");
        RaisePropertyChanged("DiaphragmMovementPercentageOrGReading");
      }
    }

    /// <summary>
    /// This property gets/sets the Temperature Rate value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TemperatureRate
    {
      get { return temperatureRate; }
      set
      {
        temperatureRate = value;
        RaisePropertyChanged("TemperatureRate");
      }
    }

    /// <summary>
    /// This property gets/sets the Max Temperature Rate value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxTemperatureRate
    {
      get { return maxTemperatureRate; }
      set
      {
        maxTemperatureRate = value;
        if (value < 1000)
          RaisePropertyChanged("MaxTemperatureRate");
      }
    }

    /// <summary>
    /// This property gets/sets the Time To Target Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToTargetTemperature
    {
      get { return timeToTargetTemperature; }
      set
      {
        timeToTargetTemperature = value;
        RaisePropertyChanged("TimeToTargetTemperature");
      }
    }

    /// <summary>
    /// This property gets/sets the Time to Thaw Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToThawTemperature
    {
      get { return timeToThaw; }
      set
      {
        timeToThaw = value;
        RaisePropertyChanged("TimeToThawTemperature");
      }
    }

    /// <summary>
    /// This property gets/sets the Keep Time to Thaw value (used for display purpose only)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool KeepTimeToThaw
    {
      get { return keepDisplayTimeToThaw; }
      set { keepDisplayTimeToThaw = value; }
    }

    /// <summary>
    /// This property gets/sets the Keep Time To Temperature value (used for display purpose only)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool KeepTimeToTemperature
    {
      get { return keepDisplayTimeToTemperature; }
      set { keepDisplayTimeToTemperature = value; }
    }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature Threshold Reached boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EsophagusTemperatureThresholdReached
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          bool _EsophagusTemperatureThresholdReached =
            CommonViewModel.Current.EcgChannel5And6Reading < EsophagusTemperature;

          if (!_EsophagusTemperatureThresholdReached)
            IsEsophagusTemperatureConditionAlertsMeet = false;

          return _EsophagusTemperatureThresholdReached;
        }
        else
        {
          return EsophagusTemperatureThresholdReachedPlayback;
        }
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          esophagusTemperatureThresholdReached = value;
        else
          EsophagusTemperatureThresholdReachedPlayback = value;

        RaisePropertyChanged("EsophagusTemperatureThresholdReached");
      }
    }


    /// <summary>
    /// This property gets/sets the Esophagus Temperature Threshold Reached boolean flag value in playback mode
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EsophagusTemperatureThresholdReachedPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Amplitude Threshold Reached boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DiaphragmAmplitudeThresholdReached
    {
      get
      {
        if (!CommonViewModel.Current.AreSensorsInPlayBackMode)
        {
          if (CommonViewModel.Current.IgnoreMinimumDiaphragmMovementValue)
          {
            return false;
          }
          else
          {
            if (CommonViewModel.Current.EcgChannel7And8Reading < 0)
              return false;
            return (CommonViewModel.Current.EcgChannel7And8Reading <= DiaphragmAmplitude &&
                    (IsSystemInAblation || IsSystemInTransition)); // && IsDiaphragmMovementDetected);
          }
        }
        else
        {
          return (diaphragmAmplitudeThresholdReached && IsSystemMonitoringDiaphragmAlert);
        }
      }
      set
      {
        diaphragmAmplitudeThresholdReached = value;
        RaisePropertyChanged("DiaphragmAmplitudeThresholdReached");
      }
    }

    /// <summary>
    /// This property gets/sets the Gas State value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Helpers.Enumeration.TankWeight GasState
    {
      get { return CommonViewModel.Current.GasState; }
      set
      {
        gasState = value;
        RaisePropertyChanged("GasState");
      }
    }

    /// <summary>
    /// This property gets/sets the LC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1Reading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          return CommonViewModel.Current.LC1Reading;
        }
        else
        {
          return LC1ReadingPlayback;
        }
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.LC1Reading = value;
        else
          LC1ReadingPlayback = value;

        RaisePropertyChanged("LC1Reading");
      }
    }


    /// <summary>
    /// This property gets/sets the LC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// NB: These value is reading the TS1 temperature. the firmware junction is changed.
    /// </summary>
    public double TN2OReading
    {
      get { return CommonViewModel.Current.TN2OReading; }

      set
      {
        CommonViewModel.Current.TN2OReading = value;
        RaisePropertyChanged("TN2OReading");
      }
    }


    #region cold junction

    /// <summary>
    /// This property gets/sets the CMCU cold junction Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CMCUCJReading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.CMCUCJReading;
        else
          return CMCUCJReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.CMCUCJReading = value;
        else
          CMCUCJReadingPlayback = value;

        RaisePropertyChanged("CMCUCJReading");
      }
    }

    /// <summary>
    /// This property gets/sets the PMCU cold junction Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PMCUCJReading
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.PMCUCJReading;
        else
          return PMCUCJReadingPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.PMCUCJReading = value;
        else
          PMCUCJReadingPlayback = value;

        RaisePropertyChanged("PMCUCJReading");
      }
    }


    #endregion

    /// <summary>
    /// Gets or sets the blood detector impedance
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <id>SF-SDS-0003</id>
    public int BloodDetecorImValue
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.BloodDetecorImValue;
        else
          return BloodDetecorImValuePlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.BloodDetecorImValue = value;
        else
          BloodDetecorImValuePlayback = value;

        RaisePropertyChanged("BloodDetecorImValue");
      }
    }



    public bool IsBloodPressureSensorConnected
    {
      get
      {
        //return true;
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.IsBloodPressureSensorConnected; // && IsUsingBloodPressureSensor;
        else
          return IsBloodPressureSensorConnectedPlayback;
      }
      set
      {
        //CommonViewModel.Current.IsBloodPressureSensorConnected = true;
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.IsBloodPressureSensorConnected = value;
        else
          IsBloodPressureSensorConnectedPlayback = value;

        RaisePropertyChanged("IsBloodPressureSensorConnected");
        RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");

      }
    }

    public bool IsMultiEtsSesnorConnected
    {
      get
      {
        //#if Simulator

        //                return false;  //true;

        //#endif
        //return true;
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.IsMultiEtsSesnorConnected;
        else
          return IsMultiEtsSesnorConnectedPlayback;
      }
      set
      {

        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.IsMultiEtsSesnorConnected = value;
        else
          IsMultiEtsSesnorConnectedPlayback = value;

        RaisePropertyChanged("IsMultiEtsSesnorConnected");

      }
    }

    /// <summary>
    /// This property gets/sets the Ablation Site value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AblationSiteEnum AblationSite
    {
      get
      {
        AblationSiteEnum value = CommonViewModel.Current.AblationSite;

        if (value.IsValidAblationSite() && PreviousAblationSite != value)
        {
          IsAblationSiteChanged = true;
          _notifyAblationSiteChangedSubject?.OnNext(true);
          PreviousAblationSite = value;
        }

        return CommonViewModel.Current.AblationSite;
      }
      set
      {
        CommonViewModel.Current.AblationSite = value;
        RaisePropertyChanged();

        if (value.IsValidAblationSite() && PreviousAblationSite != value)
        {
          IsAblationSiteChanged = true;
          _notifyAblationSiteChangedSubject?.OnNext(true);
          PreviousAblationSite = value;
        }
      }
    }

    /// <summary>
    /// This property gets/sets the previous ablation Site value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AblationSiteEnum PreviousAblationSite { get; set; } = AblationSiteEnum.OTHER;

    /// <summary>
    /// This property gets/sets the list of Ablation Data Details for a single Ablation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<AblationDataDetails> SingleAblationDatasList
    {
      get
      {
        {
          return singleAblationDatasList;
        }
      }
      set
      {
        {
          singleAblationDatasList = value;
        }
      }
    }

    /// <summary>
    /// This property gets/sets the CP1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the TC1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the CP2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the FM1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double FM1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the PT2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the LC1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Required ablation time Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredAblationTimePlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Max ECG Channel 1 and 2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Max ECG Channel 3 and 4 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxEcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 1 and 2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 3 and 4 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 5 and 6 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel5And6ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 7 and 8 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel7And8ReadingPlayback { get; set; }

    public double PressureSetPointPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the CMCU System Status Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Int64 CMCUSystemStatusError
    {
      get { return CommonViewModel.Current.CMCUSystemStatusError; }
      set
      {
        CommonViewModel.Current.CMCUSystemStatusError = value;
        switch (value)
        {
          case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_LOW:

            gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_LOW;

            break;

          case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_TOO_LOW:

            gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_TOO_LOW;

            break;

          case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_OF_BOUNDS:

            gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_OF_BOUNDS;

            break;
        }

        RaisePropertyChanged("CMCUSystemStatusError");
      }
    }

    /// <summary>
    /// This property gets/sets the System State value.  It manages transition between states : timers,
    /// ablation cycles, cathether connection, playback and display elements value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public MessageStateId SystemState
    {
      get => CommonViewModel.Current.SystemState;
      set => RaisePropertyChanged(nameof(SystemState));
    }

    private void SystemStatePropertyUpdated()
    {
      lock (this)
      {
        var systemState = CommonViewModel.Current.SystemState;
        
        if (systemState != MessageStateId.CAN_ID_STATE_INFLATION)
        {
          IsInDASBalloonTransition = false;
        } 

        try
        {
          #region Here we are in IDLE State

          if (systemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
          {
            SetIdleModelParameters();
          }

          #endregion Here we are in IDLE State

          #region Here we are in Ready State

          if (systemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY
              && PreviousSystemState != MessageStateId.CAN_ID_STATE_READY)
          {
            SetReadyModelParameters();
          }

          #endregion

          #region Here we are in inflation state

          if (systemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION
              && PreviousSystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
          {
            SetInflationModelParameters();
          }

          #endregion Here we are in inflation state

          #region here we are in Transition state

          if (systemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION &&
              PreviousSystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
          {

            if (!isAblating) //if (!timerAblation.IsEnabled)
            {
              InitializeForStartAblation();
              SensorReadingMananger.ConnectSensors();
            }

            IsTreatmentNumberAndPlayBackVisible = false;
            IsCatheterConnectedAndInIReadyState = false;
            IsSnowFlakeVisible = true;
            IsAblationTimeVisibale = true;
            IsSystemMonitoringDiaphragmAlert = true;

            UpdateSystemStateProperties(MessageStateId.CAN_ID_STATE_TRANSITION);
            ClearOcclusionPressureGraphRequestEvent?.Invoke(this, null);
          }

          #endregion here we are in Transition state

          else
          {
            //Here we avoid to manipulate data if someone  want to display saved data
            if (!IsTreatmentNumberAndPlayBackVisible
                && systemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION
                && systemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
            {
              CanStartTheTimer = false;


              if (systemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING
                  && PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
              {

                if (!isThawing) //if (!timerThawing.IsEnabled)
                {
                  WasAblationTimeManuallyChanged = false;
                  StartThawingCycle();
                }

                IsAblationTimeVisibale = false;

                IsSystemMonitoringDiaphragmAlert = false;
              }
            }
          }

          #region Previous state Thawing end current is ablation or transition

          if (PreviousSystemState == MessageStateId.CAN_ID_STATE_THAWING
              && (systemState == MessageStateId.CAN_ID_STATE_TRANSITION || systemState == MessageStateId.CAN_ID_STATE_ABLATION))
          {
            IsWritingDataToFile = true;
            IsWritingECGDataToFile = true;
            PreviuosTotalTreatmentNumber = TotalTreatmentNumber;
            PreviousSystemState = systemState;

            WriteAblationDataToFileAsync();

            IsWritingDataToFile = false;
            IsWritingECGDataToFile = false;

            StopAblationTimer();
            StopThawAndTimer();

            if (!IsLastAblationDataLoaded)
            {
              CommonViewModel.Current.GenerateAblationSummary(); //SCB-464
              IsLastAblationDataLoaded = true;
            }

            if (!isAblating) //if (!timerAblation.IsEnabled)
            {
              InitializeForStartAblation();

              RefreshModeldata();
            }

            IsCatheterConnectedAndInIReadyState = false;
            IsTreatmentNumberAndPlayBackVisible = false;
            IsSnowFlakeVisible = true;

            //Reset
            MaxTemperatureRate = TC1Reading;
            TemperatureRate = 0;

            CryoTherapyTime = 0;
            TimeInAblationMax = 0;
            LastCryoTherapyTime = 0;

            if (ISTTIDurationTimerSelected)
              RequiredAblationTime = maxAblationTimerUsingDurationMode;

            IsAblationTimeVisibale = true;

            IsSystemMonitoringDiaphragmAlert = true;

            UpdateSystemStateProperties(CommonViewModel.Current.SystemState);
          }

          #endregion Previous state Thawing end current is ablation or transition

          #region Exception state

          if (systemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
          {
            StopAblationTimer();
            CurrentAblation = null;
            IsDiaphragmMovementVisible = true;
            IsEsophagusTemperatureVisible = true;
            IsIsolatingVein = false;

            if (PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION
                || PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION
                || PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
              if (TotalTreatmentNumber != PreviuosTotalTreatmentNumber)
                IsTreatmentNumberAndPlayBackVisible = true;
              IsAblationTimeVisibale = true;
            }

            IsCatheterConnectedAndInIReadyState = false;

            IsSnowFlakeVisible = false;
            IsAblationTimeVisibale = false;

            IsSystemMonitoringDiaphragmAlert = false;
            IsSystemInException = true;

          }

          #endregion Exception state
        }
        catch (Exception ex)
        {
          LogException(ex);
        }
        finally
        {
          PreviousSystemState = systemState;
          RaisePropertyChanged(nameof(EtsGraphDisplayMode));

          // Clear/Initialize the Temperature Shadow Graph in states that not Ablation nor Thawing.
          // The shadow graph shall be initialized in Transition state and cleared in other states (kept in Ablation and Thawing state)   
          if (systemState != MessageStateId.CAN_ID_STATE_ABLATION && systemState != MessageStateId.CAN_ID_STATE_THAWING)
          {
            ValidateUpdatingShadowTemperatureGraph();
          }
        }
      }
    }

    private void CreateNewAblationData()
    {
      IsLoadingAbortedAblation = false;
      this.AblationNumber++;
      TotalTreatmentNumber++;
      TreatmentNumber = TotalTreatmentNumber;
      TreatmentNumberRefrence = TotalTreatmentNumber;
      //Allows to start to write the Ablation List infos when the ablation starts only
      CommonViewModel.Current.IsAblationProcedureStarted = true;
      EcgTime = 0;

      Ablation ablation = new Ablation();
      ablation.PatientID = CurrentPatient.ID;
      ablation.ProcedureId = CommonViewModel.Current.CurrentProcedure.Id;
      ablation.AblationNumber = this.AblationNumber;
      ablation.Description = CommonViewModel.Current.CurrentProcedure.Description + "_" + AblationNumber;
      ablation.TreatmentNote = string.Empty;
      ablation.DataFile = string.Empty;

      CommonViewModel.Current.CurrentAblation = dataAccess.AddAblation(ablation);
    }

    private void InitializeForStartAblation()
    {
      SingleAblationDatasList = new List<AblationDataDetails>();

      lock (CommonViewModel.Current.AllAblationDataList)
      {
        CommonViewModel.Current.AllAblationDataList.Add(SingleAblationDatasList);
      }

      TreatmentNumberRefrence = 0;
      TimePreviousRefrence = 0;
      CryoTherapyTime = 0;
      TimeInAblationMax = 0;

      StartAblationCycle();
      CreateNewAblationData();
      // Work for Jira PLX-1483, take a snapshot (ID=0), in case user clicks Start->Stop very shortly in 1 second, no data is recorded  
      this.AddAblationData();
    }

    /// <summary>
    /// This property gets/sets the Is Playback Mode Deactivated value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsPlayBackModeDeactivted
    {
      get { return CommonViewModel.Current.IsPlayBackModeDeactivted; }
      set
      {
        CommonViewModel.Current.IsPlayBackModeDeactivted = value;
        RaisePropertyChanged("IsPlayBackModeDeactivted");
      }
    }

    /// <summary>
    /// This functions manages the Ablation Start Cycle.  It manages timers start, ablation/treatment numbers incrementations,
    /// ablation and procedure creation, boolean flags and ablation data lists
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StartAblationCycle()
    {
      TimePreviousRefrence = 0;
      IsMonitoringBloodPressure = false;

      IsLoadingAbortedAblation = true;

      isAblating = true; // timerAblation.Start();

      CanStartTheTimer = true;
      TimeToTargetTemperature = 0;
      MaxTemperatureRate = TC1Reading;
      TemperatureRate = 0;
      TemperatureReachedRequiredAblationTemperature = false;

      ActualAblationTime = 0;
      RequiredAblationTimeAccordingToState = RequiredAblationTime;

      IsLastAblationDataLoaded = false; //reset the flag (used when in READY mode)
      IsTimeToTargetTemperatureVisible = false;

      //Here we collecte vein  ablation start time
      VeinIsolationDuration = 0;
      VeinIsolationStratTime = ElapsedTime;
      IsIsolatingVein = true;
      IsVeinIsolationDurationVisible = false;

      PreviousGenericError = string.Empty;
      CommonViewModel.Current.GenericError = string.Empty;

      ExceptionStateTime = 0;

      IsThawTemperatureReached = false;
      IsTargetTemperatureReached = false;
      this.Error = string.Empty;
      this.PreviousGenericError = string.Empty;
      AlertDurationValue = 0;
      CryoDurationChanged = false;

      AblationInformation.IsThereAbltionHistoricalData = true;

      // These part is used to Reset the Temperature rate calculation
      previousTemperature = TC1Reading;

      //Reset the  thaw time
      TimeToThawTemperature = 0;

      TotalAblationDuration = 0;
      PreviousTreatmentNumber = 0;

      //Reset Minimum Diaphragm Movement Last Value and Minimum Esophagus Temperature Last Value
      MinimumDiaphragmMovementLastValue = 1000;
      MinimumEsophagusTemperatureLastValue = 1000;

      AllowPSPChangeDuringThawing = false;
      ThawingElapsedTime = 0;

      DisplayThawingBallon = true;

      AblationInformation.IsThereAbltionHistoricalData = false;
      TimingFiliter = 0;
      isThawing = false;
      ProcedureLogModel.CanReloadProcudreInformation = false;

      AllowUserToActivateLowFlow = false;
      IsLowFlowActivated = false;
    }

    /// <summary>
    /// This functions manages the Thawing Start Cycle.  It restets flags and starts the Thawing timer
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StartThawingCycle()
    {
      if (this.cryoTherapyTime >= 1)
      {
        timeToThaw = 0;
        IsStatusAbllationBallonVisible = false;
        isThawing = true; // timerThawing.Start();
        UpdateSystemStateProperties(MessageStateId.CAN_ID_STATE_THAWING);
        ThawingElapsedTime = 0;
      }
      else

      {
        CommonViewModel.Current.Console.Stop();
        CommonViewModel.Current.LogUserAction(Enumeration.Actions.StopCommand);
        IsIsolatingVein = false;
        IsVeinIsolationDurationVisible = true;
        isThawing = false;
      }

      AllowPSPChangeDuringThawing = false;
      AllowUserToActivateLowFlow = false;
    }

    /// <summary>
    /// This property gets/sets the Start The Timer boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool CanStartTheTimer
    {
      get { return canStartTheTimer; }
      set { canStartTheTimer = value; }
    }

    /// <summary>
    /// This property gets/sets the Cryotherapy value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CryoTherapyTime
    {
      get { return cryoTherapyTime; }
      set
      {
        cryoTherapyTime = value;
        CommonViewModel.Current.CryoTherapyTime = value;

        if (CommonViewModel.Current.SystemState !=
            Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
        {
          RaisePropertyChanged("CryoTherapyTime");
        }
      }
    }




    ///// <summary>
    ///// This property gets/sets the Cryotherapy value
    ///// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    ///// </summary>
    //public int TimeTTI
    //{
    //    get
    //    {
    //        return timeTTI;
    //    }
    //    set
    //    {
    //        timeTTI = value;
    //        RaisePropertyChanged("TimeTTI");
    //    }
    //}






    /// <summary>
    /// This property gets/sets the Total Cryotherapy Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TotalCryoTherapyTime
    {
      get { return totalCryoTherapyTime; }

      set
      {
        totalCryoTherapyTime = value;
        RaisePropertyChanged("TotalCryoTherapyTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Elapsed Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ElapsedTime
    {
      get { return elapsedTime; }
      set
      {
        elapsedTime = value;
        RaisePropertyChanged("ElapsedTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Elapsed Time in minute value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ElapsedTimeMinute
    {
      get { return elapsedTime / 60; }
    }

    public int InBodyTime
    {
      get { return SkinToSkinDuration / 60; }
    }

    /// <summary>
    /// This property gets/sets the Current Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DateTime CurrentTime
    {
      get { return DateTime.Now; }
    }

    /// <summary>
    /// This property gets/sets the Is Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsVisible
    {
      get { return isVisible; }
      set
      {
        isVisible = value;
        RaisePropertyChanged("IsVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Current Patient value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public DataAccessLayer.Patient CurrentPatient
    {
      get { return CommonViewModel.Current.CurrentPatient; }
      set { RaisePropertyChanged("CurrentPatient"); }
    }

    /// <summary>
    /// This property gets/sets the Ablation Number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationNumber
    {
      get { return ablationNumber; }
      set
      {
        ablationNumber = value;
        RaisePropertyChanged("AblationNumber");
      }
    }

    /// <summary>
    /// This property gets/sets the Procedure Ended boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTheProcedureEnded
    {
      get { return isTheProcedureEnded; }
      set { isTheProcedureEnded = value; }
    }

    /// <summary>
    /// This property gets/sets the ECG Time value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int EcgTime
    {
      get { return ecgTime; }

      set
      {
        ecgTime = value;
        RaisePropertyChanged("EcgTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Catheter Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCatheterConnected
    {
      get { return CommonViewModel.Current.IsCatheterConnected; }

      set { SetProperty(ref this.isCatheterConnected, value); }
    }

    /// <summary>
    /// This property gets/sets the Catheter Cable Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCatheterCableConnected
    {
      get
      {
#if DEBUG
        return true;
#else
                bool _IsCatheterCableConnected = CommonViewModel.Current.IsCatheterCableConnected;

                if (_IsCatheterCableConnected && (!CommonViewModel.Current.IsCMCUReady || !CommonViewModel.Current.IsPMCUReady))
                    CatheterIsConnecting = true;
                else
                    CatheterIsConnecting = false;

                return _IsCatheterCableConnected;
#endif
      }
      set { RaisePropertyChanged("IsCatheterCableConnected"); }
    }

    /// <summary>
    /// This property gets/sets the Catheter Tube Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCatheterTubeConnected
    {
      get { return CommonViewModel.Current.IsCatheterTubeConnected; }
      set { RaisePropertyChanged("IsCatheterTubeConnected"); }
    }

    /// <summary>
    /// This property gets/sets the Required Ablation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>

    public int RequiredAblationTime
    {
      get { return CommonViewModel.Current.RequiredAblationTime; }
      set
      {

        CommonViewModel.Current.RequiredAblationTime = value;

        RequiredAblationTimeAccordingToState = CommonViewModel.Current.RequiredAblationTime;

        RaisePropertyChanged("RequiredAblationTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Temporary Ablation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TemporaryManualAblationTime
    {
      get { return CommonViewModel.Current.TemporaryManualAblationTime; }
      set
      {
        CommonViewModel.Current.TemporaryManualAblationTime = value;
        RaisePropertyChanged("TemporaryManualAblationTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Was Ablation Time Manually Changed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool WasAblationTimeManuallyChanged
    {
      get { return CommonViewModel.Current.WasAblationTimeManuallyChanged; }
      set
      {
        CommonViewModel.Current.WasAblationTimeManuallyChanged = value;
        RaisePropertyChanged("WasAblationTimeManuallyChanged");
      }
    }

    private int _requiredTargetTemperatureBinding;

    public int RequiredTargetTemperatureBinding
    {
      get => _requiredTargetTemperatureBinding;
      set => SetProperty(ref _requiredTargetTemperatureBinding, value);
    }

    /// <summary>
    /// This property gets/sets the Required Target Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredTargetTemperature
    {
      get { return requiredTargetTemperature; }

      set
      {
        requiredTargetTemperature = value;
        RaisePropertyChanged("RequiredTargetTemperature");
      }
    }

    /// <summary>
    /// This property gets/sets the Low Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int LowAblationTemperatureAlarm
    {
      get { return lowAblationTemperatureAlarm; }
      set
      {
        lowAblationTemperatureAlarm = value;
        RaisePropertyChanged("LowAblationTemperatureAlarm");
      }
    }

    /// <summary>
    /// This property gets/sets the High Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int HighAblationTemperatureAlarm
    {
      get { return highAblationTemperatureAlarm; }
      set
      {
        highAblationTemperatureAlarm = value;
        RaisePropertyChanged("HighAblationTemperatureAlarm");
      }
    }

    private int _thawTimerToTemperatureBinding;

    public int ThawTimerToTemperatureBinding
    {
      get => _thawTimerToTemperatureBinding;
      set => SetProperty(ref _thawTimerToTemperatureBinding, value);
    }

    /// <summary>
    /// This property gets/sets the Thaw Timer To Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ThawTimerToTemperature
    {
      get { return thawTimerToTemperature; }
      set
      {
        thawTimerToTemperature = value;
        RaisePropertyChanged("ThawTimerToTemperature");
      }
    }

    private int _esophagusBindingTemperature;

    public int EsophagusBindingTemperature
    {
      get => _esophagusBindingTemperature;
      set => SetProperty(ref _esophagusBindingTemperature, value);
    }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int EsophagusTemperature
    {
      get => esophagusTemperature;
      set => SetProperty(ref esophagusTemperature, value);
    }

    private int __diaphragmBindingAmplitude;

    public int DiaphragmBindingAmplitude
    {
      get => __diaphragmBindingAmplitude;
      set => SetProperty(ref __diaphragmBindingAmplitude, value);
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Amplitude value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DiaphragmAmplitude
    {
      get => diaphragmAmplitude;
      set
      {
        if (IsSavedToDB) IsSavedToDB = false;
        SetProperty(ref diaphragmAmplitude, value);
      }
    }

    public double DMSDetectionThreshold
    {
      get { return dmsDetectionThreshold; }
      set
      {
        if (dmsDetectionThreshold != value)
        {
          DMSDetectionThresholdValue = ConvertTheDMSTOTenBase(value);
          dmsDetectionThreshold = value;
          RaisePropertyChanged("DMSDetectionThreshold");
        }
      }
    }

    public int DMSDetectionThresholdValue
    {
      get { return dmsDetectionThresholdvalue; }
      set
      {
        if (dmsDetectionThresholdvalue == value) return;

        dmsDetectionThresholdvalue = value;
        CommonViewModel.Current.DMSDetectionThreshold = ConvertTheTenBaseTODMS(value);
        DMSDetectionThreshold = CommonViewModel.Current.DMSDetectionThreshold;
        RaisePropertyChanged("DMSDetectionThresholdValue");
      }

    }


    /// <summary>
    /// This property gets/sets the Diaphragm Sensor Gain value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DiaphragmSensorGain
    {
      get { return diaphragmSensorGain; }
      set
      {
        if (IsSavedToDB) IsSavedToDB = false;

        diaphragmSensorGain = value;
        RaisePropertyChanged(nameof(DiaphragmSensorGain));
        DiaphragmSensorGainChangedEvent?.Invoke(diaphragmSensorGain, null);
      }
    }

    /// <summary>
    /// This property gets/sets the Occlusion Pressure Graph Y-Axis Maximum
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int OcclusionPressureGraphAxisYMaximum
    {
      get { return CommonViewModel.Current.OcclusionPressureGraphAxisYMaximum; }
      set
      {
        if (VerifyOcclusionPressureGraphNewAxisValue("Maximum", value))
        {
          CommonViewModel.Current.OcclusionPressureGraphAxisYMaximum = value;
          RaisePropertyChanged("OcclusionPressureGraphAxisYMaximum");
          OcclusionPressureGraphAxisYChangedEvent?.Invoke(value, new OcclusionPressureGraphAxisYEventArgs("Maximum"));
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Occlusion Pressure Graph Y-Axis Maximum
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int OcclusionPressureGraphAxisYMinimum
    {
      get { return CommonViewModel.Current.OcclusionPressureGraphAxisYMinimum; }
      set
      {
        if (VerifyOcclusionPressureGraphNewAxisValue("Minimum", value))
        {
          CommonViewModel.Current.OcclusionPressureGraphAxisYMinimum = value;
          RaisePropertyChanged("OcclusionPressureGraphAxisYMinimum");
          OcclusionPressureGraphAxisYChangedEvent?.Invoke(value, new OcclusionPressureGraphAxisYEventArgs("Minimum"));
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Blood Pressure Graph Sweep Speed
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int OcclusionPressureGraphSweepSpeed
    {
      get { return CommonViewModel.Current.OcclusionPressureGraphSweepSpeed; }
      set
      {
        CommonViewModel.Current.OcclusionPressureGraphSweepSpeed = value;
        RaisePropertyChanged("OcclusionPressureGraphSweepSpeed");
        OcclusionPressureGraphSweepSpeedChangedEvent?.Invoke(value, null);
      }
    }

    /// <summary>
    /// This property gets/sets the Required Ablation Time Blue Margin value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredAblationTimePlueMargin
    {
      get { return requiredAblationTimePlueMargin; }

      set
      {
        requiredAblationTimePlueMargin = value;
        RaisePropertyChanged("RequiredAblationTimePlueMargin");
      }
    }

    /// <summary>
    /// This property gets/sets if the DMS Detection Threshold is valid
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDMSDetectionThresholdValid
    {
      get { return isDMSDetectionThresholdValid; }
      set
      {
        if (IsSavedToDB) IsSavedToDB = false;

        isDMSDetectionThresholdValid = value;
        RaisePropertyChanged("IsDMSDetectionThresholdValid");
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
        return CommonViewModel.Current.TreatmentNumber;
        //return treatmentNumber;   
      }

      set
      {
        treatmentNumber = value;
        CommonViewModel.Current.TreatmentNumber = value;
        RaisePropertyChanged("TreatmentNumber");
      }
    }

    /// <summary>
    /// This property gets/sets the Treatment Number reference value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TreatmentNumberRefrence { get; set; }


    /// <summary>
    /// This property gets/sets the Total Treatment Number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TotalTreatmentNumber
    {
      get { return totalTreatmentNumber; }

      set
      {
        totalTreatmentNumber = value;
        RaisePropertyChanged("TotalTreatmentNumber");
      }
    }

    /// <summary>
    /// This property gets/sets the Catheter Electrically Connected And In Idle State boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCatheterElectricallyConnectedAndInIdleState
    {
      get
      {

        if (CommonViewModel.Current.IsVacuumDisconnected)
          return (IsCatheterCableConnected &&
                  CommonViewModel.Current.SystemState ==
                  Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE &&
                  CommonViewModel.Current.IsCatheterValid
                  && CommonViewModel.Current.IsCMCUReady && CommonViewModel.Current.IsPMCUReady);
        else
          return (IsCatheterCableConnected &&
                  CommonViewModel.Current.SystemState ==
                  Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE &&
                  CommonViewModel.Current.IsCatheterValid &&
                  TotalTreatmentNumber == 0);
      }
      set
      {
        RaisePropertyChanged(nameof(IsCatheterConnectedAndInIReadyState));
        RaisePropertyChanged(nameof(IsCatheterElectricallyConnectedAndInIdleState));
      }
    }

    /// <summary>
    /// This property gets/sets the Catheter Connected And In Ready State boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0091</id>
    public bool IsCatheterConnectedAndInIReadyState
    {
      get
      {
        return (CommonViewModel.Current.SystemState ==
                Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY &&
                CommonViewModel.Current.IsCatheterValid);
      }
      set
      {
        RaisePropertyChanged(nameof(IsCatheterConnectedAndInIReadyState));
        RaisePropertyChanged(nameof(IsCatheterElectricallyConnectedAndInIdleState));
      }
    }

    /// <summary>
    /// This property gets/sets the Time To Target Temperature Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTimeToTargetTemperatureVisible
    {
      get { return isTimeToTargetTemperatureVisible; }
      set
      {
        if (value != isTimeToTargetTemperatureVisible)
        {
          isTimeToTargetTemperatureVisible = value;
          RaisePropertyChanged("IsTimeToTargetTemperatureVisible");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Snow Flake Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSnowFlakeVisible
    {
      get { return (isSnowFlakeVisible && IsCatheterCableConnected); }
      set
      {
        isSnowFlakeVisible = value;
        RaisePropertyChanged("IsSnowFlakeVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Deflate After Thaw boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DeflateAfterThaw
    {
      get { return CommonViewModel.Current.DeflateAfterThaw; }
      set
      {
        if (IsSiteUsingDefalteAfterThaw)
          CommonViewModel.Current.DeflateAfterThaw = true;
        else
          CommonViewModel.Current.DeflateAfterThaw = value;
        RaisePropertyChanged("DeflateAfterThaw");
      }
    }

    private bool _isConsoleUsingDeflateAfterThawing;

    public bool IsConsoleUsingDeflateAfterThawing
    {
      get => _isConsoleUsingDeflateAfterThawing;
      set => SetProperty(ref _isConsoleUsingDeflateAfterThawing, value);
    }

    /// <summary>
    /// This property gets/sets the Enable Slow Inflation Mode value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnableFastInflationMode
    {
      get { return CommonViewModel.Current.Console.EnableFastInflationMode; }

      set
      {
        CommonViewModel.Current.Console.EnableFastInflationMode = value;
        RaisePropertyChanged(nameof(EnableFastInflationMode));
      }
    }

    public uint RequiredVolume
    {
      get => CommonViewModel.Current.RequiredVolume;
      set
      {
        if (value == CommonViewModel.Current.RequiredVolume) return;

        CommonViewModel.Current.RequiredVolume = value;
        RaisePropertyChanged(nameof(RequiredVolume));
      }
    }

    private bool _isUsingAudioAlertSetting;

    public bool IsUsingAudioAlertSetting
    {
      get => _isUsingAudioAlertSetting;
      set => SetProperty(ref _isUsingAudioAlertSetting, value);
    }

    /// <summary>
    /// This property gets/sets the Is Using Audi Alert value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAudioAlert
    {
      get { return CommonViewModel.Current.Console.IsUsingAudioAlert; }

      set
      {
        if (CommonViewModel.Current.Console.IsUsingAudioAlert != value)
          IsUsingAudioAlertMute = false;
        CommonViewModel.Current.Console.IsUsingAudioAlert = value;
        RaisePropertyChanged("IsUsingAudioAlert");

      }
    }


    /// <summary>
    /// This property gets/sets the Is Using Auto Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAutoPlayback
    {
      get
      {
        return CommonViewModel.Current.IsUsingAutoPlayback;
        //return CommonViewModel.Current.Console.IsUsingAutoPlayback;
      }

      set
      {

        //CommonViewModel.Current.Console.IsUsingAutoPlayback = value;
        CommonViewModel.Current.IsUsingAutoPlayback = value;
        RaisePropertyChanged("IsUsingAutoPlayback");

      }
    }



    /// <summary>
    /// This property gets/sets the Is Using Audi Alert value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAudioAlertMute
    {
      get { return CommonViewModel.Current.Console.IsUsingAudioAlertMute; }

      set
      {
        CommonViewModel.Current.Console.IsUsingAudioAlertMute = value;
        RaisePropertyChanged("IsUsingAudioAlertMute");
        RaisePropertyChanged("IsUsingAudioAlert");
      }
    }


    /// <summary>
    /// This property gets/sets the Is Save to DB value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSavedToDB
    {
      get { return isSavedToDB; }

      set
      {
        isSavedToDB = value;
        RaisePropertyChanged("IsSavedToDB");
      }
    }

    /// <summary>
    /// Gets or sets the Lock the foot switch boolean value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool LockTheFootSwitch
    {
      get { return CommonViewModel.Current.LockTheFootSwitch; }

      set
      {
        CommonViewModel.Current.LockTheFootSwitch = value;
        RaisePropertyChanged("LockTheFootSwitch");
      }
    }

    /// <summary>
    /// This property gets/sets the Treatment Number And Playback Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTreatmentNumberAndPlayBackVisible
    {
      get { return isTreatmentNumberAndPlayBackVisible; }
      set
      {
        if (value != isTreatmentNumberAndPlayBackVisible)
        {
          isTreatmentNumberAndPlayBackVisible = value;
          IsAblationTimeVisibale = value;
          CommonViewModel.Current.AreSensorsInPlayBackMode = value;
          TTIFSM.AreSensorsInPlayBackMode = value;

          if (value) IsMonitoringBloodPressure = false;

          RaisePropertyChanged("IsTreatmentNumberAndPlayBackVisible");
          RaisePropertyChanged(nameof(PressureSetPoint));
          RaisePropertyChanged(nameof(DASBalloonEnabled));
          RaisePropertyChanged(nameof(IsLowFlowActivated));
          RaisePropertyChanged(nameof(CanAblationNumberForward));
          RaisePropertyChanged(nameof(CanAblationNumberBackward));
          RaisePropertyChanged(nameof(EtsGraphDisplayMode));

          if (!value)
          {
            RaisePropertyChanged("IsEsophagusTemperatureConditionAlertsMeet");
            RaisePropertyChanged("EcgChannel5And6Reading");
          }
        }

      }
    }

    /// <summary>
    /// This property gets/sets the Last Ablation Data Loaded boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLastAblationDataLoaded
    {
      get { return isLastAblationDataLoaded; }
      set { isLastAblationDataLoaded = value; }
    }

    /// <summary>
    /// This property gets/sets the Timer Procedure Elapsed Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private MicroTimer TimerProcedureElapsedTime { get; set; } = new MicroTimer();

    /// <summary>
    /// This property gets/sets the Notification Model value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public NotificationModel NotificationModel
    {
      get { return notificationModel; }
      set { notificationModel = value; }
    }

    /// <summary>
    /// This property gets/sets the Vein Isolation Duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int VeinIsolationDuration
    {
      get { return veinIsolationDuration; }

      set
      {
        veinIsolationDuration = value;
        RaisePropertyChanged("VeinIsolationDuration");
        RaisePropertyChanged("IsVeinIsolated");
      }
    }

    /// <summary>
    /// This property gets/sets is the Vein is Isolated value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsVeinIsolated
    {
      get { return VeinIsolationDuration > 0; }
    }

    /// <summary>
    /// This property gets/sets the Expected Time To Vein Isolation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ExpectedTimeToVeinIsolation
    {
      get { return expectedTimeToVeinIsolation; }

      set
      {
        expectedTimeToVeinIsolation = value;
        RaisePropertyChanged("ExpectedTimeToVeinIsolation");
      }
    }

    /// <summary>
    /// This property gets/sets the Ablation Timer value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationTimer
    {
      get { return ablationTimer; }

      set
      {
        ablationTimer = value;
        RaisePropertyChanged("AblationTimer");
      }
    }

    /// <summary>
    /// This property gets/sets the Exceeded Expected Time To Vein Isolation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int NewAblationTimer
    {
      get { return newAblationTimer; }

      set
      {
        newAblationTimer = value;
        RaisePropertyChanged("NewAblationTimer");
      }
    }

    /// <summary>
    /// This property gets/sets the Vein Isolation Start Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int VeinIsolationStratTime
    {
      get { return veinIsolationStratTime; }

      set { veinIsolationStratTime = value; }
    }

    /// <summary>
    /// This property gets/sets the Vein Isolation End Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int VeinIsolationEndTime
    {
      get { return veinIsolationEndTime; }

      set { veinIsolationEndTime = value; }
    }

    /// <summary>
    /// This property gets/sets the Is Isolating Vein value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0094</id>
    public bool IsIsolatingVein
    {
      get { return isIsolatingVein; }

      set
      {
        if (value != isIsolatingVein)
        {
          isIsolatingVein = value;
          RaisePropertyChanged("IsIsolatingVein");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the IsUsingBloodPressureSensor value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingBloodPressureSensor
    {
      get { return CommonViewModel.Current.Console.IsUsingBloodPressureSensor; }

      set
      {
        CommonViewModel.Current.Console.IsUsingBloodPressureSensor = value;
        RaisePropertyChanged("IsUsingBloodPressureSensor");
      }
    }


    /// <summary>
    /// This property gets/sets the EnabledIsBloodPressureSensorConnected value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnabledIsBloodPressureSensorConnected
    {
      get { return IsUsingBloodPressureSensor && IsBloodPressureSensorConnected; }
      set
      {
        enabledIsBloodPressureSensorConnected = value;
        RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");

      }
    }

    /// <summary>
    /// This property gets/sets the Is Vein Isolation Duration Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsVeinIsolationDurationVisible
    {
      get { return isVeinIsolationDurationVisible; }

      set
      {
        if (value != isVeinIsolationDurationVisible)
        {
          isVeinIsolationDurationVisible = value;
          RaisePropertyChanged("IsVeinIsolationDurationVisible");
        }
      }
    }


    public object VisibilityValue
    {
      get
      {
        BooleanToVisibilityConverter procedurevalue = new BooleanToVisibilityConverter();
        return procedurevalue.Convert(IsStatusAbllationBallonVisible, null, "CatheterAndBallonState", null);
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
        CommonViewModel.Current.Console
            .PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine
              [ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure =
          CommonViewModel.Current.TargetBalloonPressure;
        return CommonViewModel.Current.TargetBalloonPressure;
      }

      set
      {
        try
        {
          CommonViewModel.Current.TargetBalloonPressure = value;
          CommonViewModel.Current.Console
            .PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[
              ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = value;
          RaisePropertyChanged("TargetBalloonPressure");
        }
        catch
        {
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Is Status Ablation Balloon visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsStatusAbllationBallonVisible
    {
      get { return isStatusAbllationBallonVisible; }

      set
      {

        isStatusAbllationBallonVisible = value;
        RaisePropertyChanged("IsStatusAbllationBallonVisible");
        RaisePropertyChanged("VisibilityValue");
      }
    }

    /// <summary>
    /// This property gets/sets the Is Square Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSqaureVisible
    {
      get { return isSqaureVisible; }

      set
      {
        isSqaureVisible = value;

        if (DiaphragmAmplitudeThresholdReached && (CommonViewModel.Current.SystemState ==
                                                   Communication.CanBusMessageDefinition.MessageStateId
                                                     .CAN_ID_STATE_TRANSITION ||
                                                   CommonViewModel.Current.SystemState == Communication
                                                     .CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION
            ))
        {
          IsDiaphragmMovementVisible = isSqaureVisible;
        }

        else
        {
          IsDiaphragmMovementVisible = true;
        }


        //Esophagus Temperature
        if (EsophagusTemperatureThresholdReached && (CommonViewModel.Current.SystemState ==
                                                     Communication.CanBusMessageDefinition.MessageStateId
                                                       .CAN_ID_STATE_INFLATION ||
                                                     CommonViewModel.Current.SystemState == Communication
                                                       .CanBusMessageDefinition.MessageStateId
                                                       .CAN_ID_STATE_TRANSITION ||
                                                     CommonViewModel.Current.SystemState == Communication
                                                       .CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION ||
                                                     CommonViewModel.Current.SystemState == Communication
                                                       .CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING))
        {
          IsEsophagusTemperatureVisible = isSqaureVisible;
          IsEsophagusTemperatureConditionAlertsMeet = true;
        }

        else
        {
          IsEsophagusTemperatureVisible = true;
        }

        RaisePropertyChanged("IsSqaureVisible");
      }
    }

    /// <summary>
    /// This property gets/sets the Is Diaphragm Movement Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0092</id>
    public bool IsDiaphragmMovementVisible
    {
      get { return isDiaphragmMovementVisible; }

      set
      {
        if (value != isDiaphragmMovementVisible)
        {
          isDiaphragmMovementVisible = value;
          RaisePropertyChanged("IsDiaphragmMovementVisible");
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Is Esophagus Temperature Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0093</id>
    public bool IsEsophagusTemperatureVisible
    {
      get { return isEsophagusTemperatureVisible; }

      set
      {
        if (value != isEsophagusTemperatureVisible)
        {
          isEsophagusTemperatureVisible = value;
          RaisePropertyChanged("IsEsophagusTemperatureVisible");
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether esophagus temperature condition alerts meet or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0098</id>
    public bool IsEsophagusTemperatureConditionAlertsMeet
    {

      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return isEsophagusTemperatureConditionAlertsMeet;
        else
          return IsEsophagusTemperatureConditionAlertsMeetPlayback;

      }

      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
        {
          if (value != isEsophagusTemperatureConditionAlertsMeet)
          {
            isEsophagusTemperatureConditionAlertsMeet = value;
            RaisePropertyChanged("IsEsophagusTemperatureConditionAlertsMeet");
          }
        }
        else
        {
          IsEsophagusTemperatureConditionAlertsMeetPlayback = value;
          RaisePropertyChanged("IsEsophagusTemperatureConditionAlertsMeet");
        }
      }

    }


    /// <summary>
    /// Gets or sets a value indicating whether esophagus temperature condition alerts meet or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsEsophagusTemperatureConditionAlertsMeetPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Exception State Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ExceptionStateTime
    {
      get { return exceptionStateTime; }

      set
      {
        exceptionStateTime = value;
        RaisePropertyChanged("ExceptionStateTime");
      }
    }

    /// <summary>
    /// This property gets/sets the Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string Error
    {
      get { return error; }

      set { error = value; }
    }

    /// <summary>
    /// This property gets/sets the Previous Generic Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string PreviousGenericError
    {
      get { return previousGenericError; }

      set { previousGenericError = value; }
    }

    /// <summary>
    /// This property gets/sets the Previous Generic Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsThawTemperatureReached
    {
      get { return isThawTemperatureReached; }

      set
      {
        isThawTemperatureReached = value;
        RaisePropertyChanged("IsThawTemperatureReached");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether target temperature reached or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTargetTemperatureReached
    {
      get { return isTargetTemperatureReached; }

      set
      {
        isTargetTemperatureReached = value;
        RaisePropertyChanged("IsTargetTemperatureReached");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the system in thawing or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInThawing
    {
      get { return isSystemInThawing; }

      set { isSystemInThawing = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether language changed or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLanguageChanged
    {
      get { return isLanguageChanged; }

      set
      {
        isLanguageChanged = value;
        RaisePropertyChanged("IsLanguageChanged");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is an cryterion user or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCryterionUser
    {
      get
      {
        // since we added the BSC ADMIN we will give the same right
        return (CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser);
      }
    }

    private User _currentUser = CommonViewModel.Current.CurrentUser;

    public User CurrentUser
    {
      get => _currentUser;
      set => SetProperty(ref _currentUser, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether fixed time selected or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsFixedTimerSelected
    {
      get { return CommonViewModel.Current.IsFixedTimerSelected; }
      set
      {

        CommonViewModel.Current.IsFixedTimerSelected = value;
        RaisePropertyChanged("IsFixedTimerSelected");
        if (CommonViewModel.Current.IsFixedTimerSelected)
        {
          ISTTIFixedTimerSelected = false;
          ISTTIDurationTimerSelected = false;
          ISTTISelected = false;
          CommonViewModel.Current.CanUpadteRequiredAblationTime = true;

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
      get { return CommonViewModel.Current.ISTTIFixedTimerSelected; }
      set
      {
        CommonViewModel.Current.ISTTIFixedTimerSelected = value;
        RaisePropertyChanged("ISTTIFixedTimerSelected");
        if (CommonViewModel.Current.ISTTIFixedTimerSelected)
        {
          IsFixedTimerSelected = false;
          ISTTIDurationTimerSelected = false;
          ISTTISelected = true;
          CommonViewModel.Current.CanUpadteRequiredAblationTime = false;

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
      get { return CommonViewModel.Current.ISTTIDurationTimerSelected; }
      set
      {
        CommonViewModel.Current.ISTTIDurationTimerSelected = value;
        RaisePropertyChanged("ISTTIDurationTimerSelected");
        if (CommonViewModel.Current.ISTTIDurationTimerSelected)
        {
          IsFixedTimerSelected = false;
          ISTTIFixedTimerSelected = false;
          ISTTISelected = true;
          CommonViewModel.Current.CanUpadteRequiredAblationTime = false;

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
      get { return CommonViewModel.Current.ISTTISelected; }
      set
      {

        CommonViewModel.Current.ISTTISelected = value;
        RaisePropertyChanged("ISTTISelected");
      }

    }

    public bool ISTTISelectedPlayback { get; set; }


    /// <summary>
    /// Gets or sets alert duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public short AlertDurationValue
    {
      get { return alertDurationValue; }

      set { alertDurationValue = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether required ablation time is visible or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsRequiredAblationTimeVisible
    {
      get { return isRequiredAblationTimeVisible; }

      set
      {
        if (isRequiredAblationTimeVisible != value)
        {
          isRequiredAblationTimeVisible = value;
          RaisePropertyChanged("IsRequiredAblationTimeVisible");
        }
      }
    }

    /// <summary>
    /// Gets or sets duration expected vein isolation time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DurationExpectedVeinIsolationTime
    {
      get { return durationExpectedVeinIsolationTime; }

      set
      {

        durationExpectedVeinIsolationTime = value;
        RaisePropertyChanged("DurationExpectedVeinIsolationTime");
      }
    }


    /// <summary>
    /// Gets or sets ablation timer TTI value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationTimerTTI
    {
      get { return ablationTimerTTI; }

      set
      {

        ablationTimerTTI = value;
        RaisePropertyChanged("AblationTimerTTI");
      }
    }

    /// <summary>
    /// Gets or sets new ablation timer TTI value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int NewAblationTimerTTI
    {
      get { return newAblationTimerTTI; }

      set
      {
        newAblationTimerTTI = value;
        RaisePropertyChanged("NewAblationTimerTTI");

      }
    }


    /// <summary>
    /// Gets or sets Max value of Time In Ablation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeInAblationMax
    {
      get { return timeInAblationMax; }

      set
      {
        timeInAblationMax = value;
        RaisePropertyChanged("TimeInAblationMax");

      }
    }



    /// <summary>
    /// Gets or sets a value indicating whether cryo duration changed or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool CryoDurationChanged
    {
      get { return cryoDurationChanged; }
      set
      {
        cryoDurationChanged = value;
        RaisePropertyChanged("CryoDurationChanged");
      }
    }

    /// <summary>
    /// Gets or sets last diaphragm movement percentage or reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LastDiaphragmMovementPercentageOrGReadingValue
    {
      get => lastDiaphragmMovementPercentageOrGReadingValue;
      set => lastDiaphragmMovementPercentageOrGReadingValue = value;
    }

    /// <summary>
    /// Gets or sets elapsed time last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ElapsedTimeLastValue
    {
      get => elapsedTimeLastValue;
      set => elapsedTimeLastValue = value;
    }


    /// <summary>
    /// Gets or sets last vein isolation duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int LastVeinIsolationDuration
    {
      get => lastVeinIsolationDuration;
      set => lastVeinIsolationDuration = value;
    }

    /// <summary>
    /// Gets or sets ablation timer TTI fixed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationTimerTTIFixed
    {
      get { return ablationTimerTTIFixed; }

      set
      {
        ablationTimerTTIFixed = value;
        RaisePropertyChanged("AblationTimerTTIFixed");
      }
    }

    /// <summary>
    /// Gets or sets new ablation timer TTI fixed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int NewAblationTimerTTIFixed
    {
      get { return newAblationTimerTTIFixed; }

      set
      {
        newAblationTimerTTIFixed = value;
        RaisePropertyChanged("NewAblationTimerTTIFixed");
      }
    }

    /// <summary>
    /// Gets the value indicating whether user is allowed to change ablation timers or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUserAllowedToChangeAblationTimers => CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_TRANSITION &&
                                                       CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_ABLATION &&
                                                       CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_THAWING &&
                                                       !CommonViewModel.Current.AreSensorsInPlayBackMode;

    /// <summary>
    /// Gets the value indicating whether user is allowed to change cooling and thaw to temperature or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUserAllowedToChangeCoolingAndThawToTemperature => !CommonViewModel.Current.AreSensorsInPlayBackMode;

    /// <summary>
    /// This property gets the foot switch state.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void LockAndLockFootSwitch()
    {
      if (CommonViewModel.Current.IsFootSwitchLocked)
        LockTheFootSwitch = !LockTheFootSwitch;
    }


    /// <summary>
    /// Gets/sets the value indicating whether the foot switch was locked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool IsFootSwitchLockedPreviuosValue { get; set; } = false;

    /// <summary>
    /// Gets/sets the previuos total treatment number value 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int PreviuosTotalTreatmentNumber { get; set; } = 0;

    /// <summary>
    /// Gets/sets the value indicating whether the software is saving ablation data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsWritingDataToFile { get; set; } = false;

    /// <summary>
    /// Gets/sets the value indicating whether the software is saving the ECG data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsWritingECGDataToFile { get; set; } = false;


    /// <summary>
    /// Gets/sets the value indicating whether ablation time is visible or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsAblationTimeVisibale
    {
      get { return isAblationTimeVisibale; }
      set
      {

        isAblationTimeVisibale = value;
        RaisePropertyChanged("IsAblationTimeVisibale");
      }
    }

    /// <summary>
    /// Gets/sets the value indicating whether is system monitoring diaphragm alert or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0095</id>
    public bool IsSystemMonitoringDiaphragmAlert
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return isSystemMonitoringDiaphragmAlert;
        else
          return IsSystemMonitoringDiaphragmAlertPlayback;
      }
      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          isSystemMonitoringDiaphragmAlert = value;
        else
          IsSystemMonitoringDiaphragmAlertPlayback = value;

        RaisePropertyChanged("IsSystemMonitoringDiaphragmAlert");
      }
    }

    /// <summary>
    /// Gets/sets the value indicating whether is system monitoring diaphragm alert playback or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemMonitoringDiaphragmAlertPlayback { get; set; }

    /// <summary>
    /// Gets/sets required ablation time according to state value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredAblationTimeAccordingToState
    {
      get
      {
        return CommonViewModel.Current.RequiredAblationTimeAccordingToState; // requiredAblationTimeAccordingToState;
      }
      set
      {
        if (!IsTreatmentNumberAndPlayBackVisible)
        {
          CommonViewModel.Current.RequiredAblationTimeAccordingToState = value;
          RaisePropertyChanged("RequiredAblationTimeAccordingToState");

        }
      }
    }

    /// <summary>
    /// Gets/sets previous treatment number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int PreviousTreatmentNumber
    {
      get => previousTreatmentNumber;
      set => previousTreatmentNumber = value;
    }

    /// <summary>
    /// Gets/sets total ablation duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TotalAblationDuration
    {
      get => totalAblationDuration;
      set => SetProperty(ref totalAblationDuration, value);
    }

    /// <summary>
    /// Gets/sets the value indicating whether is ablation loading aborted or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLoadingAbortedAblation
    {
      get { return isLoadingAbortedAblation; }

      set { isLoadingAbortedAblation = value; }
    }

    /// <summary>
    /// Gets/sets the minimum diaphragm movement last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int MinimumDiaphragmMovementLastValue { get; set; } = 1000;

    /// <summary>
    /// Gets/sets the minimum esophagus temperature last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int MinimumEsophagusTemperatureLastValue { get; set; } = 1000;

    /// <summary>
    /// Gets/sets the value indicating whether is system using DAS balloon or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemUsingDASBalloon
    {
      get => CommonViewModel.Current.IsSystemUsingDASBalloon && CommonViewModel.Current.IsCatheterCableConnected;
    }

    public bool IsUsingDASBalloon
    {
      get => SensorReadingMananger.AreSensorsConnected
          ? CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled
          : PressureSetPointPlayback >= 3.0;

    }

    private bool _isDASBallonEnabledForPlayback;
    /// <summary>
    /// Gets/sets the value indicating whether is DAS balloon enabled or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0099</id>
    public bool DASBalloonEnabled
    {
      get => IsTreatmentNumberAndPlayBackVisible 
        ? _isDASBallonEnabledForPlayback
        : CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled;

      set
      {
        if (IsTreatmentNumberAndPlayBackVisible)
        {
          _isDASBallonEnabledForPlayback = value;
        }

        RaisePropertyChanged(nameof(DASBalloonEnabled));
        RaisePropertyChanged(nameof(PressureSetPoint));
        RaisePropertyChanged(nameof(IsBalloonRampDownActivated));
        RaisePropertyChanged(nameof(BloodDetecorImValue));
        RaisePropertyChanged(nameof(IsUsingDASBalloon));
        RaisePropertyChanged(nameof(CanExecuteEnableDASBalloonCommand));
      }
    }

    /// <summary>
    /// Gets/sets the value indicating whether is balloon ramp down activated or not.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsBalloonRampDownActivated
    {
      get
      {
        return (CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled &&
                BalloonRampDown.IsBalloonRampDownActivated);
      }

    }

    /// <summary>
    /// Gets/sets the value for pressure set point or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PressureSetPoint
    {
      get
      {
        if (SensorReadingMananger.AreSensorsConnected)
          return CommonViewModel.Current.ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentPressureSetpoint;
        else
        {
          if (PressureSetPointPlayback == 0)
            PressureSetPointPlayback = CommonViewModel.Current.ChangeBalloonTypeFSM.InflateDeflateBalloonModel
              .CurrentPressureSetpoint;

          return PressureSetPointPlayback;
        }
      }

      set
      {
        if (SensorReadingMananger.AreSensorsConnected)
          CommonViewModel.Current.ChangeBalloonTypeFSM.InflateDeflateBalloonModel.CurrentPressureSetpoint = value;
        else
        {
          PressureSetPointPlayback = value;
          RaisePropertyChanged(nameof(IsUsingDASBalloon));
        }

        RaisePropertyChanged("PressureSetPoint");
      }
    }

    /// <summary>
    /// Gets/sets the value indicating whether is the pressure set point reached or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool ISThePressureSetPointReached
    {
      get { return iSThePressureSetPointReached; }

      set
      {
        iSThePressureSetPointReached = value;
        RaisePropertyChanged("ISThePressureSetPointReached");
      }

    }

    /// <summary>
    /// Gets/sets the value indicating whether allow PSP change during thawing or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0096</id>
    public bool AllowPSPChangeDuringThawing
    {
      get { return allowPSPChangeDuringThawing; }

      set
      {
        allowPSPChangeDuringThawing = value;
        RaisePropertyChanged(nameof(AllowPSPChangeDuringThawing));
        RaisePropertyChanged(nameof(CanExecuteEnableDASBalloonCommand));
      }
    }

    private void StartProcedureElapsedTimer()
    {
      if (!TimerProcedureElapsedTime.Enabled)
      {
        lock (TimerProcedureElapsedTime)
        {
          if(!TimerProcedureElapsedTime.Enabled)
            TimerProcedureElapsedTime.Start();
        }
      }
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      CommonViewModel commonviewmodel = sender as CommonViewModel;

      if (!previousCanStartTherapy && commonviewmodel.CanStartTherapy && !CPUTimeWatchdog.IsTimerStarted)
      {
        CPUTimeWatchdog.IsTimerStarted = true;
        ElapsedTime = 0;
        SkinToSkinDuration = 0;
        RaisePropertyChanged("InBodyTime");
        StartProcedureElapsedTimer();
      }
      else if (!commonviewmodel.CanStartTherapy)
      {
        //TimerProcedureElapsedTime.Stop();
        CPUTimeWatchdog.StopTimeMonitoring();
      }

      if (commonviewmodel.CanStartTherapy)
      {
        switch (e.PropertyName)
        {
          case "ISTTISelected":
            RaisePropertyChanged("ISTTISelected");
            break;
          case "TC1Reading":
            RaisePropertyChanged("TC1Reading");
            break;

          case "PT2Reading":
            RaisePropertyChanged("PT2Reading");
            break;

          case "CP1Reading":
            RaisePropertyChanged("TipOrBalloonPressureReading");
            RaisePropertyChanged("CP1Reading");
            RaisePropertyChanged("CP2Reading");
            break;

          case "CP2Reading":
            RaisePropertyChanged("CP2Reading");
            break;

          case "FM1Reading":
            RaisePropertyChanged("FM1Reading");
            break;

          case "LC1Reading":
            RaisePropertyChanged("LC1Reading");
            RaisePropertyChanged("GasState");
            break;

          case "CMCUSystemStatusError":
            RaisePropertyChanged("CMCUSystemStatusError");
            break;

          case "SystemState":
            //Exception state occurred when in Ablation / Thawing / Transition
            //Save the data to file before cleaning.
            // VerifyAndHandleExceptionState();
            SystemStatePropertyUpdated();

            RaisePropertyChanged("SystemState");
            RaisePropertyChanged("IsSystemUsingDASBalloon");
            RaisePropertyChanged("CatheterType");
            RaisePropertyChanged("DiaphragmAmplitudeThresholdReached");
            break;

          case "CurrentPatient":
            RaisePropertyChanged("CurrentPatient");
            break;

          case "IsAblationProcedureEnded":
            EndProcedure();
            break;

          case "IsCatheterCableConnected":
            ActivateCatheterIfConditionsApply();
            break;

          case "IsUsedForEngineering":
            RaisePropertyChanged("IsUsedForEngineering");
            break;

          case "IsCatheterTubeConnected":
            ActivateCatheterIfConditionsApply();
            break;

          case "GasState":
            RaisePropertyChanged("GasState");
            break;

          case "AblationSite":
            if (PreviousAblationSite != AblationSite && AblationSite.IsValidAblationSite())
            {
              _ablationSiteObservable.OnNext(AblationSite);
            }

            RaisePropertyChanged("AblationSite");
            break;

          case "EcgChannel1And2Reading":
            RaisePropertyChanged("EcgChannel1And2Reading");
            RaisePropertyChanged("TipOrBalloonPressureReading");
            break;

          case "MaxEcgChannel1And2Reading":
            RaisePropertyChanged("TipOrBalloonPressureReading");
            break;

          case "EcgChannel3And4Reading":
            RaisePropertyChanged("EcgChannel3And4Reading");
            RaisePropertyChanged("DiaphragmMovementPercentageOrGReading");
            RaisePropertyChanged("DiaphragmAmplitudeThresholdReached");
            CheckvitalParametersAlerts();
            break;

          case "MaxEcgChannel3And4Reading":
            RaisePropertyChanged("MaxEcgChannel3And4Reading");
            RaisePropertyChanged("DiaphragmAmplitudeThresholdReached");
            CheckvitalParametersAlerts();
            break;

          case "EcgChannel5And6Reading":
            RaisePropertyChanged("EcgChannel5And6Reading");
            RaisePropertyChanged("EsophagusTemperatureThresholdReached");
            CheckvitalParametersAlerts();
            if (IsMultiEtsSesnorConnected)
            {
              RaisePropertyChanged("ListOfSesnorsState");
            }

            break;

          case "EcgChannel7And8Reading":
            RaisePropertyChanged("DiaphragmMovementPercentageOrGReading");
            RaisePropertyChanged("DiaphragmAmplitudeThresholdReached");
            break;

          case "IsDiaphragmMovementDetected":
            RaisePropertyChanged("IsDiaphragmMovementDetected");
            CheckvitalParametersAlerts();
            break;

          case "AblationSummary":
            RaisePropertyChanged("AblationSummary");
            break;

          case "CurrentAblation":
            RaisePropertyChanged("CurrentAblation");
            break;

          case "AreSensorsInPlayBackMode":
            SetPlayBackMode();
            break;

          case "RequiredAblationTime":
            RaisePropertyChanged("RequiredAblationTime");
            break;
          case "RequiredAblationTimeAccordingToState":
            RaisePropertyChanged("RequiredAblationTimeAccordingToState");
            break;

          case "TreatmentNumber":
            RaisePropertyChanged("TreatmentNumber");
            break;

          #region catheter ready

          case "IsCMCUReady":
          case "IsPMCUReady":
            ActivateCatheterIfConditionsApply();
            break;

          case "IsCatheterValid":
            ActivateCatheterIfConditionsApply();
            break;

          #endregion

          #region Vein Isolation

          case "IsVeinIsolated":
            OnVeinIsolatedCommand(null);
            break;

          #endregion

          #region Foot Switch Lock

          case "IsFootSwitchLocked":
            LockAndLockFootSwitch();
            break;

          #endregion


          #region Ignore Minimum Diaphragm Movemen tValue

          case "IgnoreMinimumDiaphragmMovementValue":
            RaisePropertyChanged("IgnoreMinimumDiaphragmMovementValue");
            break;

          #endregion


          #region CRT user

          case "AblationSymmary":
            RaisePropertyChanged("IsCryterionUser");
            break;

          #endregion

          #region DAS

          case "IsSystemUsingDASBalloon":
            RaisePropertyChanged("IsSystemUsingDASBalloon");
            break;

          #endregion

          #region Cold junction

          case "CMCUCJReading":
            RaisePropertyChanged("CMCUCJReading");
            break;

          case "PMCUCJReading":
            RaisePropertyChanged("PMCUCJReading");
            break;

          #endregion

          case "BloodDetecorImValue":
            RaisePropertyChanged("BloodDetecorImValue");
            break;

          #region blood pressure Sensor

          case "IsBloodPressureSensorConnected":
            RaisePropertyChanged("IsBloodPressureSensorConnected");
            RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");
            break;

          #endregion

          #region low flow

          case "IsLowFlowActivated":
            RaisePropertyChanged("IsLowFlowActivated");
            break;

          case "IsUsingLowFlow":
            RaisePropertyChanged("IsUsingLowFlow");
            break;


          #endregion

          case "IsMultiEtsSesnorConnected":
            RaisePropertyChanged("IsMultiEtsSesnorConnected");
            break;

          case "ListOfSesnorsState":
            //if (ElapsedTime % 2 == 0)
            RaisePropertyChanged("ListOfSesnorsState");
            break;
        }
      }
      else
      {
        switch (e.PropertyName)
        {
          case nameof(commonviewmodel.SystemState):
            // Keep the track of PreviousSystemState even CryoTherapy view is not visible   
            PreviousSystemState = commonviewmodel.SystemState;
            break;
          case nameof(commonviewmodel.CurrentUser):
            CurrentUser = commonviewmodel.CurrentUser;
            break;
          case nameof(commonviewmodel.IsUsingLowFlow):
            RaisePropertyChanged(nameof(IsUsingLowFlow));
            break;
          default:
            break;
        }
      }

      previousCanStartTherapy = commonviewmodel.CanStartTherapy;
    }

    /// <summary>
    /// This function handles the Playback Mode activation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void SetPlayBackMode()
    {
      if (CommonViewModel.Current.IsAllowedToSetPlayBack == false)
        return;


      IsTreatmentNumberAndPlayBackVisible = CommonViewModel.Current.AreSensorsInPlayBackMode;

      if (this.inflationEvent == null)
      {
        this.inflationEvent = new InflationEventArgs();
      }

      OnInflation(null, inflationEvent);

      // Refresh TC1
      if (!IsTreatmentNumberAndPlayBackVisible)
      {
        CommonViewModel.Current.PT2Reading =
          1; // used to as reference value the hardware will change the value if is different than 1
        CommonViewModel.Current.CP2Reading =
          1; // used to as reference value the hardware will change the value if is different than 1

        IsAblationTimeVisibale = false;
        DataLoading = false;
        RaisePropertyChanged("TC1Reading");
        RaisePropertyChanged("FM1Reading");
        RaisePropertyChanged("PT2Reading");
        RaisePropertyChanged("CP2Reading");
        RaisePropertyChanged("PressureSetPoint");
        RaisePropertyChanged(nameof(DASBalloonEnabled));
        RaisePropertyChanged(nameof(IsLowFlowActivated));
      }
    }

    public void RefreshTheInBodyTime()
    {
      RaisePropertyChanged("InBodyTime");
    }

    /// <summary>
    /// This function handles the End Procedure operations.  It stops several timers, resets counters and
    /// reset properties for the next procedure
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void EndProcedure()
    {
      isAblating = false; //this.timerAblation.Stop();
      isThawing = false; //this.timerThawing.Stop();
      IsStatusAbllationBallonVisible = false;
      CommonViewModel.Current.IsAblationProcedureStarted = false;
      CommonViewModel.Current.IsAblationProcedureEnded = true;
      this.CryoTherapyTime = 0;
      this.TimeInAblationMax = 0;
      this.LastCryoTherapyTime = 0;
      this.TotalCryoTherapyTime = 0;
      IsSnowFlakeVisible = false;
      CommonViewModel.Current.DeflateAfterThaw = false;
    }

    /// <summary>
    /// This function handles the Catheter Activation when conditions are met
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ActivateCatheterIfConditionsApply()
    {
      CommonViewModel localCommonViewModel = CommonViewModel.Current;
      if (localCommonViewModel.IsCMCUReady && localCommonViewModel.IsPMCUReady)
        CatheterIsConnecting = false;

      RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
      RaisePropertyChanged("IsCatheterCableConnected");
      RaisePropertyChanged("IsCatheterTubeConnected");
      RaisePropertyChanged("IsSnowFlakeVisible");
      RaisePropertyChanged("IsSystemUsingDASBalloon");
      RaisePropertyChanged("IsUsedForEngineering");
      RaisePropertyChanged("CatheterType");
      RaisePropertyChanged("CP1Reading");
      RaisePropertyChanged("CP2Reading");
      RaisePropertyChanged("TipOrBalloonPressureReading");
      RaisePropertyChanged("BloodDetecorImValue");

      if (localCommonViewModel.IsCatheterCableConnected && localCommonViewModel.IsCatheterTubeConnected)
      {
        IsCatheterConnected = true;
      }

      thawTemperature = (int)CommonViewModel.Current.ThawingTemperatureSetPoint;
    }

    /// <summary>
    /// Function/Command that handles the Ablation Number Forward operation when the Ablation Forward
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="param">The command's parameter (not used in this function).</param>
    private async void OnAblationNumberForward(object param)
    {
      TreatmentNumber++;
      if (TreatmentNumber > TotalTreatmentNumber)
      {
        TreatmentNumber = TreatmentNumber >= TotalTreatmentNumber ? TotalTreatmentNumber : TreatmentNumber;
        return;
      }

      TreatmentNumber = TreatmentNumber >= TotalTreatmentNumber ? TotalTreatmentNumber : TreatmentNumber;
      if (TreatmentNumber <= TotalTreatmentNumber)
      {
        try
        {
          DataLoading = true;
          await Task.Delay(500);
          Forward();
        }
        catch (Exception ex)
        {
          LogException(ex);
          throw;
        }
        finally
        {
          DataLoading = false;
        }
      }
    }

    /// <summary>
    /// Called by OnAblationNumberForward function.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void Forward()
    {
      try
      {
        TotalAblationDuration = 0;
        PreviousGenericError = string.Empty;
        
        List<AblationDataDetails> ablationDatasListItems =
          SingleAblationDatasList.FindAll(a => a.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION);
        TotalAblationDuration = ablationDatasListItems.Count;
        if (ablationDatasListItems.Count > 0)
        {
          RequiredAblationTime = ablationDatasListItems[ablationDatasListItems.Count - 1].RequiredAblationTime;
          ISTTISelected = ablationDatasListItems[ablationDatasListItems.Count - 1].ISTTISelected;
          requiredAblationTimeAccordingToState = RequiredAblationTime;
          RaisePropertyChanged("RequiredAblationTimeAccordingToState");
          PreviousTreatmentNumber = TreatmentNumber;
        }
        LoadPlaybackMode(TreatmentNumber);
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    public bool CanAblationNumberForward => !DataLoading && IsTreatmentNumberAndPlayBackVisible;

    /// <summary>
    /// Function/Command that handles the Last Ablation operation when the Last Ablation
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="param">The command's parameter (not used in this function).</param>
    private void OnLastAblation(object param)
    {
      if (!VerifyAllAblationDataListNotEmpty())
      {
        LoadAllAblationDataFromFile();
        LoadLastAblationData();
      }

      ManageExceptionDataLoading(param);
      TreatmentNumber = TotalTreatmentNumber;

      if (TreatmentNumber <= TotalTreatmentNumber)
      {
        PreviousGenericError = string.Empty;
        LoadPlaybackMode(TreatmentNumber);
      }

      ManageExceptionDataLoading(param);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Last Ablation command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanLastAblation(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Ablation Number Backward operation when the Ablation Backward
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="param">The command's parameter (not used in this function).</param>
    private async void OnAblationNumberBackward(object OnSavePatient)
    {
      TreatmentNumber--;
      if (TreatmentNumber <= 0)
      {
        TreatmentNumber = TreatmentNumber <= 0 ? 1 : TreatmentNumber;
        return;
      }

      TreatmentNumber = TreatmentNumber <= 0 ? 1 : TreatmentNumber;

      if (TreatmentNumber <= TotalTreatmentNumber)
      {
        try
        {
          DataLoading = true;
          await Task.Delay(500);
          Backward();
        }
        catch (Exception ex)
        {
          LogException(ex);
          throw;
        }
        finally
        {
          DataLoading = false;
        }
      }
    }

    /// Called by OnAblationNumberBackward function.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void Backward()
    {
      try
      {
        TotalAblationDuration = 0;
        PreviousGenericError = string.Empty;

        List<AblationDataDetails> ablationDatasListItems =
          SingleAblationDatasList.FindAll(a => a.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION);
        TotalAblationDuration = ablationDatasListItems.Count;
        if (ablationDatasListItems.Count > 0)
        {
          RequiredAblationTime = ablationDatasListItems[ablationDatasListItems.Count - 1].RequiredAblationTime;
          ISTTISelected = ablationDatasListItems[ablationDatasListItems.Count - 1].ISTTISelected;
          requiredAblationTimeAccordingToState = RequiredAblationTime;
          RaisePropertyChanged("RequiredAblationTimeAccordingToState");
          PreviousTreatmentNumber = TreatmentNumber;
        }
        LoadPlaybackMode(TreatmentNumber);
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// Function that Loads the Playback mode.  It loads single ablation data and ECG data lists
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="treatmentNumber"></param>
    public void LoadPlaybackMode(int treatmentNumber)
    {
      if (treatmentNumber <= 0)
        return;
      double eTSMinimumTemperature = 1000;
      int jsonEtsSensorsStatusType = 0;

      if (CommonViewModel.Current.AllAblationDataList.Count >= treatmentNumber)
      {
        SingleAblationDatasList = CommonViewModel.Current.AllAblationDataList[treatmentNumber - 1];

        if (SingleAblationDatasList != null && SingleAblationDatasList.Count > 0)
        {
          AblationDataDetails lastAblationData = SingleAblationDatasList[SingleAblationDatasList.Count - 1];
          this.AblationSite = (AblationSiteEnum)lastAblationData.AblationSite;
          TimeInAblationMax = lastAblationData.TimeInAblation;
          ISTTISelected = lastAblationData.ISTTISelected;

          CryoTherapyTime = SingleAblationDatasList
            .DistinctBy(item => item.TimeInAblation)
            .Count(item => item.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION || 
                           item.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION);
          
          ActualAblationTime = CryoTherapyTime;

          ThawingElapsedTime = SingleAblationDatasList
            .Count(item => item.SystemState == (int)MessageStateId.CAN_ID_STATE_THAWING);

          if (ListOfSesnorsState != null)
          {
            ListOfSesnorsState.Clear();
          }

          List<double> EtsSesnors = new List<double>
          {
            lastAblationData.EtsSensor13, lastAblationData.EtsSensor1, lastAblationData.EtsSensor2,
            lastAblationData.EtsSensor3, lastAblationData.EtsSensor4,
            lastAblationData.EtsSensor5, lastAblationData.EtsSensor6, lastAblationData.EtsSensor7,
            lastAblationData.EtsSensor8,
            lastAblationData.EtsSensor9, lastAblationData.EtsSensor10, lastAblationData.EtsSensor11,
            lastAblationData.EtsSensor12
          };

          jsonEtsSensorsStatusType = JsonEtsSensorsStatusType(EtsSesnors);

          if (jsonEtsSensorsStatusType == 2)
          {
            IsMultiEtsSesnorConnected = false;
          }
          else
          {
            List<double> sesnors = EtsSesnors;
            ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(sesnors, out eTSMinimumTemperature);
            CommonViewModel.Current.MinimumTemperature = eTSMinimumTemperature;
            IsMultiEtsSesnorConnected = true;
          }

          EtsPlaybackData = SingleAblationDatasList.Select(ab => (double)ab.EcgChannel5And6Reading).ToList(); 
        }
      }

      // Update Treatment # x of x.
      PlaybackModeEvent?.Invoke(null, null);
      IsReloadingPreviuosProcdure = false;
    }


    private int JsonEtsSensorsStatusType(List<double> EtsSensors) //EmilyTest
    {
      int sensorDataType = 0;
      bool hasValidEtsSValue = false;
      bool IsAlways1000 = true;
      bool IsEts13Valid = false;
      var validEts = EtsSensors.Where(x => x > 0 && x < 50).ToList();
      if (validEts.Count > 0)
        hasValidEtsSValue = true;
      else
      {
        var NoEts = EtsSensors.Where(x => x == 1000).ToList();
        if (NoEts.Count == 12)
        {
          IsAlways1000 = true;
          if (EtsSensors[0] < 50 && EtsSensors[0] > 0)
          {
            IsEts13Valid = true;
          }
        }
      }

      if (hasValidEtsSValue == true)
        sensorDataType = 1;
      else if (IsAlways1000 == true)
      {
        if (IsEts13Valid == true)
          sensorDataType = 3;
        else
          sensorDataType = 2;
      }

      return sensorDataType;
    }

    public bool CanAblationNumberBackward => !DataLoading && IsTreatmentNumberAndPlayBackVisible;

    /// <summary>
    /// Function/Command that handles the Console Connection when the Connect
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="param">The command's parameter (not used in this function).</param>
    private void OnConnectCommand(object param)
    {
      if (CommonViewModel.Current.SystemState ==
          Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
      {
        CommonViewModel.Current.Console.Disconnect();
        CommonViewModel.Current.IsVacuumDisconnected = true;
      }
      else if (IsCatheterCableConnected)
      {
#if !DEBUG
                CommonViewModel.Current.ReadRepeaterAndICBFirmware(1);
#endif
        CommonViewModel.Current.Console.Connect();
        CommonViewModel.Current.IsVacuumDisconnected = false;
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
    /// Function/Command that handles the Console Start when the Start command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="param">The command's parameter (not used in this function).</param>
    private void OnStartCommand(object param)
    {
      if (CommonViewModel.Current.IsUserManualOpned)
        return;

      if (IsTreatmentNumberAndPlayBackVisible)
      {
        ResetAblationTimeSettings();
        ResetPlaybackSettings();
      }

      if (CommonViewModel.Current.IsSystemInDataError)
      {
        Tuple<long, string, string, string> genericMessage =
          Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID111,
            (int)Enumeration.ErrorTypes.GUI);
        MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.ErrorMessage,
          Views.MessagePopup.ButtonType.Ok, "", true);
        MessagePopup.ShowDialog();
        // MessagePopup dialogPopup = new MessagePopup("An error occurred while loading the treatment file in memory. Please exit to the Home screen and then return to the Therapy screen.", MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: "SYSTEM ERROR");
        //  dialogPopup.ShowDialog();
        return;
      }

      CommonViewModel.Current.Console.Start();
      CommonViewModel.Current.LogUserAction(Enumeration.Actions.StartCommand);
      IsPlayBackModeDeactivted = false;
      DataLoading = false;

      // To detect the Blood Pressure Sensor connection status when Start command is called during Playback mode.
      SensorReadingMananger.ConnectSensors();
      RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");
      RaisePropertyChanged("IsMultiEtsSesnorConnected");

      if (CommonViewModel.Current.SystemState ==
         Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
      {
        IsLoadingAbortedAblation = true;
      }

      if(CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_TRANSITION ||
          CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_ABLATION)
      {
        ActualAblationTime = 0;
      }

      if(WasAblationTimeManuallyChanged)
      {
        RequiredAblationTime = TemporaryManualAblationTime;
      }

#if Simulator
      //Simalutates that when in Thawing and START is pressed, start another Ablation
      if(CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
      {
        CommonViewModel.Current.SystemState =
 Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
        Thread.Sleep(10);
        CommonViewModel.Current.SystemState =
 Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;
      }
#endif

      //PlayAudioFile();
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

    private int _actualAblationTime;
    public int ActualAblationTime
    {
      get => _actualAblationTime;
      set => SetProperty(ref _actualAblationTime, value);
    }

    /// <summary>
    /// Function/Command that handles the Console Stop when the Stop command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnStopCommand(object arg)
    {
      if (TotalTreatmentNumber <= 0)
      {
        IsPlayBackModeDeactivted = false;
      }
      
      if (SystemState == MessageStateId.CAN_ID_STATE_TRANSITION || SystemState == MessageStateId.CAN_ID_STATE_ABLATION)
      {
        ActualAblationTime = cryoTherapyTime;
      } 
      else if (SystemState == MessageStateId.CAN_ID_STATE_THAWING)
      {
        ThawingElapsedTime = cryoTherapyTime - ActualAblationTime;
      }

      CommonViewModel.Current.Console.Stop();
      CommonViewModel.Current.LogUserAction(Enumeration.Actions.StopCommand);
      IsIsolatingVein = false;
      IsVeinIsolationDurationVisible = true;
      IgnoreMinimumDiaphragmMovementBindingValue = IgnoreMinimumDiaphragmMovementValue;

#if Simulator

      CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
#endif
    }

    private async Task WriteAblationDetailDataToFileAsync(List<AblationDataDetails> updatedDetailData, Ablation ablation)
    {
      await Task.Run(() => WriteAblationDetailDataToFile(updatedDetailData, ablation));
    }

    private void WriteAblationDetailDataToFile(List<AblationDataDetails> updatedDetailData, Ablation ablation)
    {
      try
      {

        if (updatedDetailData != null && updatedDetailData.Count > 0)
        {

          var ablationFileStruct = AblationFileDataStruct.ConvertAblationDataDetailsToFileStruct(updatedDetailData);

          var jsonFileManager = new JsonManager();
          jsonFileManager.SerializeAndWriteToFile(ablationFileStruct,
            Path.Combine(GetBasePath(), FILESTORAGE + ablation.Description));

          if (jsonFileManager.FileNameAndLocation != "")
          {
            ablation.DataFile = jsonFileManager.FileNameAndLocation;
            this.dataAccess.UpdateAblation(ablation);
          }
        }
      }
      catch (Exception exception)
      {
        LogException(exception);
        TreatmentNumberRefrence = 0;
        CommonViewModel.Current.IsSystemInDataError = true;

        AbortAblation();
        Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID4, (int)Enumeration.ErrorTypes.GUI);
        
        DispatcherBeginInvoke(() =>
            {
              MessagePopup dialogPopup = new MessagePopup(
                exception.Message,
                MessagePopup.MessageType.ErrorMessage,
                MessagePopup.ButtonType.Ok,
                messageTitle: genericMessage.Item2);
              dialogPopup.ShowDialog();
            });
      }
    }

    private void DispatcherBeginInvoke(System.Action action)
    {
      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action); 
    }

    private void ValidateAndUpdateForTimeToThawTemperature(List<AblationDataDetails> ablationDataDetails)
    {
      // 1. Make sure the last AblationData in DetailData is Thawing State; 
      // 2. Check if the thaw temperature setting is 20°C but IsReached is false;
      var lastData = ablationDataDetails?.LastOrDefault();
      if (lastData == null
          || lastData.SystemState != (int)MessageStateId.CAN_ID_STATE_THAWING
          || ThawTimerToTemperature < (int)CommonViewModel.Current.ThawingTemperatureSetPoint
          || lastData.IsThawTemperatureReached)
      {
        return;
      }

      // 3. Verify current TC1+1 is >= 20°C;
      if ((int)(TC1Reading + 1) < ThawTimerToTemperature)
      {
        return;
      }

      // 4. Update last TC1 to current TC1 or TC1+1, and set IsReached to true. 
      IsThawTemperatureReached = true;
      lastData.TC1Reading = TC1Reading >= ThawTimerToTemperature ? TC1Reading : TC1Reading + 1;
      lastData.IsThawTemperatureReached = true;
    }

    /// <summary>
    /// Function that serializes (to JSON) the single ablation data list, write it to a file and
    /// add the filepath in the database
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private async void WriteAblationDataToFileAsync()
    {
      if (CommonViewModel.Current.CurrentAblation == null ||
          CommonViewModel.Current.CurrentAblation.Description == null)
        return;

      // Work for Jira PLX-1483, if recorded more than 1 AblationData,
      // filter out the first snapshot (ID==0) for backward compatibility (ID starts from 1)      
      if (SingleAblationDatasList != null && SingleAblationDatasList.Count > 1 && SingleAblationDatasList[0].ID == 0)
      {
        SingleAblationDatasList.RemoveAt(0);
      }

      int duration = 0;
      var updatedDetailData = SingleAblationDatasList
        .DistinctBy(ab => ab.ID)
        .Select(ab =>
      {
        if (ab.SystemState != (int)Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
        {
          duration = ab.ID;
        }

        ab.TimeInAblation = duration;
        return ab;
      }).ToList();

      ValidateAndUpdateForTimeToThawTemperature(updatedDetailData);
      SingleAblationDatasList = updatedDetailData;
      CommonViewModel.Current.AllAblationDataList[TotalTreatmentNumber - 1] = SingleAblationDatasList;

      await WriteAblationDetailDataToFileAsync(updatedDetailData, CommonViewModel.Current.CurrentAblation);
    }

    /// <summary>
    /// Function that returns the application base path + bin folder
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>Returns the path.</returns>
    private string GetBasePath()
    {
      string thePath = "";

      String path = AppDomain.CurrentDomain.BaseDirectory;
      String[] extract = Regex.Split(path, "bin"); //split it in bin
      thePath = extract[0];

      return thePath;
    }

    /// <summary>
    /// Function that returns if the system can invoke the Stop command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanStopCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Notification window display when the Notifications command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnNotificationsCommand(object arg)
    {
      //When gO to nofification during we update the lC1 play back
      LC1ReadingPlayback = CommonViewModel.Current.LC1Reading;
      TTIFSM.IsUserCancelingTTISettings = false;


      TTIFSM.IsFixedTimerSelected = this.IsFixedTimerSelected;

      TTIFSM.ISTTIFixedTimerSelected = this.ISTTIFixedTimerSelected;

      TTIFSM.ISTTIDurationTimerSelected = this.ISTTIDurationTimerSelected;

      TTIFSM.ISTTISelected = this.ISTTISelected;

      TTIFSM.AblationTimer = this.AblationTimer;

      TTIFSM.DurationExpectedVeinIsolationTime = this.DurationExpectedVeinIsolationTime;

      TTIFSM.AblationTimerTTI = this.AblationTimerTTI;

      TTIFSM.NewAblationTimerTTI = this.NewAblationTimerTTI;

      TTIFSM.AblationTimerTTIFixed = this.AblationTimerTTIFixed;

      TTIFSM.NewAblationTimerTTIFixed = this.NewAblationTimerTTIFixed;

      TTIFSM.AblationDurationType = this.AblationDurationType;

      TTIFSM.RequiredAblationTime = this.RequiredAblationTime;

      try
      {
        _isInitializing = true;
        Notifications notifications = new Notifications();

        var dialogResult = notifications.ShowDialog() ?? false;
        if (dialogResult && IsUserAllowedToChangeAblationTimers)
        {
          ResetAblationTimeSettings();

          if (IsUserAllowedToChangeAblationTimers)
            WasAblationTimeManuallyChanged = false;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        _isInitializing = false;
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Notifications command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanNotificationsCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Notification window display when the Notifications command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnNotificationsChangeCommand(object arg)
    {
      IsDMSSettingPopupShow = true;
      IsSavedToDB = false;

      this._dmsQuickSettingsPopupDisposible.Disposable = this._dmsQuickSettingsRefreshSubject
        .Throttle(TimeSpan.FromSeconds(6.0))
        .Subscribe(_ => this.ExecuteCloseDMSQuickSettings());

      this._dmsQuickSettingsRefreshSubject.OnNext(true);
    }

    private void ExecuteCloseDMSQuickSettings()
    {
      IsDMSSettingPopupShow = false;
      if (_dmsQuickSettingsPopupDisposible.Disposable != null || !_dmsQuickSettingsPopupDisposible.IsDisposed)
      {
        _dmsQuickSettingsPopupDisposible.Disposable?.Dispose();
        _dmsQuickSettingsPopupDisposible.Disposable = null; 
      } 
    }

    private void ExecuteGotoMoreSettingsCommand()
    {
      ExecuteCloseDMSQuickSettings(); 
      NotificationsCommand?.Execute(null);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Notifications command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanNotificationsChangeCommand(object arg)
    {
      return true;
    }

    public bool IsDMSSettingPopupShow
    {
      get => isDMSSettingPopupShow; 
      set => SetProperty(ref isDMSSettingPopupShow, value); 
    }

    private bool _isTTIPopupShow;

    public bool IsTTIPopupShow
    {
      get => _isTTIPopupShow;
      set => SetProperty(ref _isTTIPopupShow, value);
    }


    /// <summary>
    /// Function/Command that handles the Blood Pressure settings window display when the Blood Pressure Settings command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnOcclusionPressureSettingsChangeCommand(object arg)
    {
      IsBloodPressureSettingsPopupShow = true;
      if (this._occlusionPressureSettingsPopupDisposible != null
          && !this._occlusionPressureSettingsPopupDisposible.IsDisposed)
      {
        _occlusionPressureSettingsPopupDisposible.Disposable =
          this._occlusionPressureSettingsRefreshSubject
            .Throttle(TimeSpan.FromSeconds(6.0))
            .Subscribe(_ => this.ExecuteCloseOcclusionPressureSettingsCommand());
      }
      this._occlusionPressureSettingsRefreshSubject.OnNext(true);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Blood Pressure Setttings command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanOcclusionPressureSettingsChangeCommand(object arg)
    {
      return true;
    }

    public bool IsReloadingPreviuosProcdure { get; set; }

    public bool IsBloodPressureSettingsPopupShow
    {
      get => isBloodPressureSettingsPopupShow;
      set => SetProperty(ref isBloodPressureSettingsPopupShow, value);
    }

    /// <summary>
    /// Function/Command that handles the Ablation Site change when the Ablation Site command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnAblationSiteCommand(object arg)
    {
      DisplayAblationSiteWarning = CommonViewModel.Current?.AreSensorsInPlayBackMode ?? true;
      var ablationSiteWindow_ = new AblationSiteWindow(this)
      {
        Left = 20,
        Top = 200
      };
      ablationSiteWindow_.ShowDialog();
    }

    /// <summary>
    /// Function that returns if the system can invoke the Ablation Site command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanAblationSiteCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Treatment Notes Entry when the Treatment Notes command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnTreatmentNotesCommand(object arg)
    {
      // we do not allow the user to enter Note if there is no Ablation
      if (AblationNumber > 0)
      {
        List<Ablation> ablationList =
          CommonViewModel.Current?.Data?.DataAccess?.GetAllAblationByProcedureId(CurrentAblation.ProcedureId);
        CommonViewModel.Current.CurrentAblation = ablationList[TreatmentNumber - 1];
        var treatmentNotes = new TextEntryPopupNew(this, CommonViewModel.TextEntryType.TreatmentNotes);
        treatmentNotes.ShowDialog();

        this.dataAccess.UpdateAblation(CurrentAblation);
        RaisePropertyChanged("CurrentAblation");
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Deflate After Thaw command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanDeflateAfterThawCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Deflation after Thaw when the Deflate After Thaw command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnDeflateAfterThawCommand(object arg)
    {
      //Save physician's preferences in the database
      NotificationModel notificationModel = NotificationModel.Instance;
      if (notificationModel != null && notificationModel.CurrentPhysician != null &&
          notificationModel.CurrentPhysician.preference != null)
      {
        bool isUsingAutoDeflation = notificationModel.CurrentPhysician.preference.IsUsingAutoDeflation;
        DeflateAfterThaw = !isUsingAutoDeflation;

        notificationModel.CurrentPhysician.preference.IsUsingAutoDeflation = DeflateAfterThaw;
        notificationModel.SaveNotification();
      }


    }

    /// <summary>
    /// Function that returns if the system can invoke the Treatment Notes command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanTreatmentNotesCommand(object arg)
    {
      return true;
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
      TemporaryManualAblationTime = RequiredAblationTime;
      WasAblationTimeManuallyChanged = true;
      //NotificationModel notificationModel = NotificationModel.Instance;
      //if (notificationModel != null && notificationModel.CurrentPhysician != null && notificationModel.CurrentPhysician.preference != null)
      //{
      //notificationModel.CurrentPhysician.preference.AblationDurationType = (short)Helpers.Enumeration.AblationDurationType.FixedTimer;
      //notificationModel.SaveNotification();

      //SaveTheLastLastUsedTTitype();

      ISTTISelected = false;
      ISTTIDurationTimerSelected = false;
      ISTTIFixedTimerSelected = false;
      IsFixedTimerSelected = true;
      // notificationModel.CurrentPhysician.preference.AblationDurationType = (short)Helpers.Enumeration.AblationDurationType.FixedTimer;
      //}


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
      int requiredAblationTimeReference = 0;

      requiredAblationTimeReference = RequiredAblationTime;

      if (Current.SystemState == CAN_ID_STATE_TRANSITION || Current.SystemState == CAN_ID_STATE_ABLATION)
      {
        if ((requiredAblationTimeReference - 30) > CryoTherapyTime)
        {
          RequiredAblationTime -= 30;
          ISTTISelected = false;
          ISTTIDurationTimerSelected = false;
          ISTTIFixedTimerSelected = false;
          IsFixedTimerSelected = true;
        }
      }
      else
      {
        RequiredAblationTime -= 30;
        if (RequiredAblationTime <= 0)
          RequiredAblationTime = 30;
      }

      TemporaryManualAblationTime = RequiredAblationTime;
      WasAblationTimeManuallyChanged = true;
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
    /// Function/Command that handles the Vein Isolated Command when the
    /// Vein Isolated command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnVeinIsolatedCommand(object arg)
    {
      TEMPTTI = TC1Reading;
      VeinIsolationDuration = CryoTherapyTime;
      var ablationData = SingleAblationDatasList.FindLast(x => x.ID == VeinIsolationDuration);
      if (ablationData != null)
        ablationData.TimeToVeinIsolation = VeinIsolationDuration;

      if (ISTTIDurationTimerSelected)
        CryoDurationChanged = true;

      HandleAblationTimerAccordingToveinIsolationLogic();

      IsVeinIsolationDurationVisible = true;
    }

    /// <summary>
    /// This property gets the Can Vein Isolated Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanVeinIsolatedCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Change Tank Command when the
    /// Change Tank command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnChangeTankCommand(object arg)
    {
      CommonViewModel.Current.AccessedChangeTankFromCryotherapy = true;
      ChangeTankInCryotherapyEvent?.Invoke(null, null);
    }

    /// <summary>
    /// This property gets the Can Change Tank Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanChangeTankCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// This property gets the lock the foot swithch value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnLockTheFootSwitchCommand(object arg)
    {
      string parameter = arg?.ToString();

      if (parameter == "LockTheFootSwitch")
      {
        LockTheFootSwitch = true;
      }
      else
      {
        LockTheFootSwitch = false;
      }

    }

    /// <summary>
    /// This property gets the Can Change Tank Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanLockTheFootSwitchCommand(object arg)
    {
      return true;
    }

    /// Gets/sets the value indicating whether can enable DAS ballon command or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanEnableDASBallonCommand(object arg)
    {
      return true;
      //return CommonViewModel.Current.ChangeBalloonTypeFSM.CatheterType == Enumeration.CatheterType.ID_28_mm;


    }

    /// <summary>
    /// Function/Command that handles the Enable DAS Ballon Command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnEnableDASBalloonCommand(bool? isIncreaseBalloonDiameter)
    {
      CommonViewModel localCommonViewModel = CommonViewModel.Current;

      if (localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_INFLATION ||
          localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_THAWING)
      {

        if (localCommonViewModel.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING &&
            TC1Reading < thawTemperature)
        {

          Tuple<long, string, string, string> genericMessage =
            Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID93,
              (int)Enumeration.ErrorTypes.GUI);
          Tuple<long, string, string, string> dasMessage =
            Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID94,
              (int)Enumeration.ErrorTypes.GUI);

          MessagePopup MessagePopup = new Views.MessagePopup(genericMessage,
            Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, dasMessage.Item2);
          MessagePopup.ShowDialog();
          return;
        }
        
        if (isIncreaseBalloonDiameter == null || !isIncreaseBalloonDiameter.HasValue)
        {
          localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled =
            !localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled;
        }
        else
        {
          localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled = isIncreaseBalloonDiameter.Value;
        }

        DASBalloonEnabled = localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled;

        // Dispable the Ablate button during DAS transition period   
        ISThePressureSetPointReached = false;
        IsInDASBalloonTransition = true;
      }
    }

    /// <summary>
    /// This property gets the Can reset LSPRO Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanResetLSPROCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles that reset LSPRO command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    internal void OnResetLSPROCommand(object obj)
    {
      if (IsPortComValid())
      {
        CommonViewModel.Current.SpManager.StopListening();
        Thread.Sleep(2500);
        CommonViewModel.Current.SpManager.StartListening();
      }
    }


    //SaveDMSSettingCommand

    /// <summary>
    /// This property gets the CanSaveDMSSettingCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanSaveDMSSettingCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles that save DMS setting command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnSaveDMSSettingCommand(object obj)
    {
      try
      {

        //Save physician's preferences in the database
        NotificationModel notificationModel = NotificationModel.Instance;
        if (notificationModel != null && notificationModel.CurrentPhysician != null &&
            notificationModel.CurrentPhysician.preference != null)
        {

          notificationModel.CurrentPhysician.preference.DiaphragmAmplitude = DiaphragmAmplitude;
          notificationModel.CurrentPhysician.preference.DiaphragmSensorGain = Convert.ToInt16(DiaphragmSensorGain);
          notificationModel.CurrentPhysician.preference.DMSDetectionThreshold =
            ConvertTheTenBaseTODMS(DMSDetectionThresholdValue);
          notificationModel.CurrentPhysician.preference.EsophagusTemperature = EsophagusTemperature;
          notificationModel.CurrentPhysician.preference.IgnoreDiaphragmMovement = IgnoreMinimumDiaphragmMovementValue;
          notificationModel.CurrentPhysician.preference.IsUsingAudioAlert = IsUsingAudioAlert;
          notificationModel.SaveNotification();
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        CloseDMSQuickSettingsCommand?.Execute(null);
        IsSettingsDirty = IsPreferenceSettingsChanged();
      }
    }

    private bool IsPreferenceSettingsChanged()
    {
      bool ablationTimerSettingsChanged = false;
      currentPhysician = currentPhysician ?? NotificationModel.Instance.CurrentPhysician;
      if (currentPhysician != null && currentPhysician.preference != null)
      {
        var preference = currentPhysician.preference;

        ablationTimerSettingsChanged =
          // Ablation Timer Settings
          preference.AblationDurationType != (short)AblationDurationType ||
          preference.ExpectedVeinIsolationTime != ExpectedTimeToVeinIsolation ||
          preference.AblationTimerTTIFixed != AblationTimerTTIFixed ||
          preference.NewAblationTimerTTIFixed != NewAblationTimerTTIFixed ||
          preference.DurationExpectedVeinIsolationTime != DurationExpectedVeinIsolationTime ||
          preference.AblationTimerTTI != AblationTimerTTI ||
          preference.NewAblationTimerTTI != NewAblationTimerTTI ||
          preference.AblationTimer != AblationTimer ||
          // Timer Target 
          preference.CoolingRequiredTargetTemperature != RequiredTargetTemperature ||
          preference.ThawTimerToTemperature != ThawTimerToTemperature ||
          // Notification Settings
          preference.LowAblationTemperatureAlarm != LowAblationTemperatureAlarm ||
          preference.HighAblationTemperatureAlarm != HighAblationTemperatureAlarm ||
          preference.EsophagusTemperature != EsophagusTemperature ||
          preference.DiaphragmAmplitude != DiaphragmAmplitude ||
          preference.DMSDetectionThreshold != DMSDetectionThreshold ||
          preference.DiaphragmSensorGain != (short)DiaphragmSensorGain ||
          preference.IgnoreDiaphragmMovement != IgnoreMinimumDiaphragmMovementValue ||
          preference.IsUsingAudioAlert != IsUsingAudioAlert ||
          // System Settings  
          preference.IsUsingInflationFastSpeed != !EnableFastInflationMode || 
          preference.EnabaleEnhancedAudio != EnabaleEnhancedAudio ||
          preference.IsUsingAutoPlayback != IsUsingAutoPlayback ||
          preference.CurveStyle != TemperatureChartType ||
          preference.RefrigerantLevelUnit != (short)RefrigerantLevelUnit ||
          preference.IsUsingShadowing != CanDisplayShadowGraph ||
          preference.VolumeLevel != (short)RequiredVolume;

        if (!IsSiteUsingDefalteAfterThaw)
          ablationTimerSettingsChanged |= preference.IsUsingAutoDeflation != DeflateAfterThaw;
      }

      return ablationTimerSettingsChanged; 
    }

    /// <summary>
    /// Function/Command that handles that volume setting command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnVolumeControlOnCommand(object obj)
    {

      IsUsingAudioAlertMute = true;
      IsUsingAudioAlert = true;

    }


    /// <summary>
    /// This property gets the CanVolumeControlOnCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanVolumeControlOnCommand(object arg)
    {
      return true;
    }


    /// <summary>
    /// Function/Command that handles that volume setting command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnVolumeControlOffCommand(object obj)
    {
      IsUsingAudioAlertMute = false;
      IsUsingAudioAlert = true;

    }


    /// <summary>
    /// This property gets the CanVolumeControlOffCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanVolumeControlOffCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// This property gets the Can reset LSPRO Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanActivateLowFlowCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles that reset LSPRO command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    internal void OnActivateLowFlowCommand(object obj)
    {
      IsLowFlowActivated = true;
      CommonViewModel.Current.SendLowFlowValue();
      AllowUserToActivateLowFlow = false;
    }


    /// <summary>
    /// This property gets the Can reset  Command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanResetDiaphragmCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles that reset the diaphragm
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    internal void OnResetDiaphragmCommand(object obj)
    {
      this._dmsQuickSettingsRefreshSubject.OnNext(true);
      if (HighResDmsSignalDetected)
      {
        MaximumHRAveragePacingLevel = 0;
      }
      else
      {
        MaximumAveragePacingLevel = 0;
      }

      CommonViewModel.Current.ResetDiaphragmReference();
      CommonViewModel.Current.LogUserAction(Enumeration.Actions.DiaphragmReset);
    }

    /// <summary>
    /// Function/Command that handles the Update Vein Isolation Command when the
    /// Vein Isolated command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnUpdateVeinIsolationDurationCommand(object arg)
    {
      if (!isTreatmentNumberAndPlayBackVisible)
      {
        return;
      }
      try
      {
        var updateVeinDuration = new UpdateVeinIsolationDuration(
          SingleAblationDatasList[SingleAblationDatasList.Count - 1].TimeToVeinIsolation,
          SingleAblationDatasList[SingleAblationDatasList.Count - 1].TimeInAblation);
        if ((bool)updateVeinDuration.ShowDialog())
        {
          if (IsTreatmentNumberAndPlayBackVisible)
          {
            int duration;
            bool isNumeric =
              int.TryParse(((UpdateVeinIsolationDurationViewModel)updateVeinDuration.DataContext).VeinIsolationDuration,
                out duration);

            if (isNumeric)
            {
              //update the vein isolation duration
              foreach (AblationDataDetails ablationDetail in SingleAblationDatasList)
              {
                if (ablationDetail.TimeInAblation >= duration)
                {
                  ablationDetail.TimeToVeinIsolation = duration;
                }
              }

              this.VeinIsolationDuration = duration;




              if (SingleAblationDatasList.Count > 0)
              {
                int tmpCount = SingleAblationDatasList.Count - 1;
                this.TimeInAblationMax = SingleAblationDatasList[tmpCount].TimeInAblation;
                if (duration > 0)
                  this.TEMPTTI = SingleAblationDatasList[duration].TC1Reading;
              }

              //Update the json file with the new vein isolation duration
              CommonViewModel.Current.UpdateAblationData(SingleAblationDatasList, TreatmentNumber);
              //Refresh the display with the new chart
              LoadPlaybackMode(TreatmentNumber);
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// This property gets the Can Update Vein Isolated Command value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanUpdateVeinIsolationDurationCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// This property gets the CanTareOcclusionPressureGraphCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanTareOcclusionPressureGraphCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the tare for the Occlusion pressure graph command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnTareOcclusionPressureGraphCommand(object obj)
    {
      _occlusionPressureSettingsRefreshSubject.OnNext(true);
      try
      {
        /* Tare value is the last blood pressure value read */
        CommonViewModel.Current.OcclusionPressureTareValue = -CommonViewModel.Current.RawEcgChannel1And2Reading;
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// This property gets the CanResetTareOcclusionPressureGraphCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanResetTareOcclusionPressureGraphCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the reset tare for the Occlusion pressure graph command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnResetTareOcclusionPressureGraphCommand(object obj)
    {
      _occlusionPressureSettingsRefreshSubject.OnNext(true);
      try
      {
        /* Reset Tare value is the last blood pressure value read */
        CommonViewModel.Current.OcclusionPressureTareValue = 0;
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// This property gets the CanSaveOcclusionPressureGraphSettingsCommand value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private bool CanSaveOcclusionPressureGraphSettingsCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the save occlusion pressure graph settings command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj"></param>
    private void OnSaveOcclusionPressureGraphSettingsCommand(object obj)
    {
      try
      {
        //Save physician's preferences in the database
        NotificationModel notificationModel = NotificationModel.Instance;
        if (notificationModel != null && notificationModel.CurrentPhysician != null
                                      && notificationModel.CurrentPhysician.preference != null)
        {
          notificationModel.CurrentPhysician.preference.OcclusionPressureGraphAxisYMaximum =
            OcclusionPressureGraphAxisYMaximum;
          notificationModel.CurrentPhysician.preference.OcclusionPressureGraphAxisYMinimum =
            OcclusionPressureGraphAxisYMinimum;
          notificationModel.CurrentPhysician.preference.OcclusionPressureGraphSweepSpeed =
            OcclusionPressureGraphSweepSpeed;
          notificationModel.SaveNotification();
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        this.ExecuteCloseOcclusionPressureSettingsCommand();
      }
    }

    private void ExecuteCloseOcclusionPressureSettingsCommand()
    {
      IsBloodPressureSettingsPopupShow = false;
      if ( _occlusionPressureSettingsPopupDisposible.Disposable != null &&  !_occlusionPressureSettingsPopupDisposible.IsDisposed)
      {
        _occlusionPressureSettingsPopupDisposible.Disposable?.Dispose();
        _occlusionPressureSettingsPopupDisposible.Disposable = null; 
      }
    }

    /// <summary>
    /// Function that verifies if the newly entered occlusion pressure graph axis value is valid or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool VerifyOcclusionPressureGraphNewAxisValue(string limitID, int limitValue)
    {
      if (limitID == "Maximum" && limitValue > OcclusionPressureGraphAxisYMinimum && limitValue >= 0)
        return true;
      else if (limitID == "Minimum" && limitValue < OcclusionPressureGraphAxisYMaximum && limitValue >= 0)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Function that adds a single ablation data to the Ablation Data list
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void AddAblationData()
    {
      if (!IsWritingDataToFile)
      {
        CommonViewModel localCommonViewModel = CommonViewModel.Current;
        AblationDataDetails ablationData = new FileSerializer.AblationDataDetails();

        //Timestamp
        // ablationData.TimeStamp = DateTime.Now.ToString("MMM dd yyyy HH:mm:ss.fff");
        ablationData.TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
        //Ablation Time
        ablationData.ID = this.CryoTherapyTime;


        LastCryoTherapyTime = this.CryoTherapyTime;

        //Ablation ID
        ablationData.AblationID = this.ablationNumber;

        //System State 
        ablationData.SystemState = CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_IDLE 
          ? (int)MessageStateId.CAN_ID_STATE_THAWING 
          : (int)CommonViewModel.Current.SystemState;

        //Ablation Site

        if (!AblationSite.IsValidAblationSite())
        {
          ablationData.AblationSite = (int)this.PreviousAblationSite;
        }
        else
        {
          ablationData.AblationSite = (int)this.AblationSite;
          PreviousAblationSite = this.AblationSite;
        }


        //Temperature Rate
        ablationData.TemperatureRate = this.TemperatureRate;

        //Minimum Temperature 
        ablationData.MaxTemperatureRate = this.MaxTemperatureRate;
        ablationData.RequiredTargetTemperature = this.RequiredTargetTemperature;

        //Catheter ID
        ablationData.CatheterId = CommonViewModel.Current.CatheterID;

        //Catheter Lot
        ablationData.CatheterLot = CommonViewModel.Current.CatheterLot;

        ablationData.CatheterSerialNumber = CommonViewModel.Current.CatheterSerialNumber;
        ablationData.CatheterContainer = CommonViewModel.Current.CatheterContainerTag; 
        ablationData.IsUsedForEngineering = CommonViewModel.Current.IsUsedForEngineering;

        ablationData.IsTargetTemperatureReached = this.IsTargetTemperatureReached;
        ablationData.TimeToTargetTemperature = this.TimeToTargetTemperature;

        //Ablation Duration Setpoint 
        ablationData.RequiredAblationTime = this.RequiredAblationTime;

        //Time to Vein Isolation 
        ablationData.TimeToVeinIsolation = this.VeinIsolationDuration;


        ablationData.ExceptionStateTime = this.ExceptionStateTime;

        //Thaw Time 
        ablationData.TimeToThaw = this.timeToThaw;

        // Thaw Timer Setpoint (°C) 
        ablationData.ThawTimerToTemperature = this.ThawTimerToTemperature;
        ablationData.IsThawTemperatureReached = this.IsThawTemperatureReached;

        //Tank Pressure (PT1)
        ablationData.PT1Reading = localCommonViewModel.PT1Reading;


        //Injection Pressure (PT2)
        ablationData.PT2Reading = this.PT2Reading;

        //Return Line Pressure (PT3)
        ablationData.PT3Reading = localCommonViewModel.PT3Reading;

        //Vacuum Line Pressure (PT4)
        ablationData.PT4Reading = localCommonViewModel.PT4Reading;

        //Scavenging Line Pressure (PT5)
        ablationData.PT5Reading = localCommonViewModel.PT5Reading;

        //Vent Line Switch (PS1)
        ablationData.PS1Reading = localCommonViewModel.PS1Reading;

        //Flow (FM1)
        ablationData.FM1Reading = localCommonViewModel.FM1Reading;

        //Sub-Cooler Temperature (TS1); TS1 CJ 
        ablationData.TS1Reading = localCommonViewModel.TN2OReading; // because the N2O sensor is removed 

        //TN2O 
        ablationData.TN2OReading = localCommonViewModel.TS1Reading;

        //Tank Weight (LC1)
        ablationData.LC1Reading = localCommonViewModel.LC1Reading;

        //Balloon Temperature (TC1)
        ablationData.TC1Reading = this.TC1Reading;


        ablationData.TIPReading = localCommonViewModel.TIPReading;

        //Inner Balloon Pressure (IBP)
        ablationData.CP1Reading = localCommonViewModel.CP1Reading; //IBP

        //Outer Balloon Pressure (OBP)
        ablationData.CP2Reading = localCommonViewModel.CP2Reading; //OBP
                                                                   //saveData.CIMP1Reading = localCommonViewModel.CIMP1Reading; //TODO: Alex will implement it.  It's Still not in place

        //Balloon PWM
        ablationData.PWMBAL = localCommonViewModel.PatientPIDDutyCycle;

        //Injection PWM
        ablationData.PWMINJ = localCommonViewModel.PIDDutyCycle;

        ablationData.ProcedureId = localCommonViewModel.CurrentProcedure.Id;

        //Add the hospital name
        if (String.IsNullOrWhiteSpace(hospitalName))
        {
          try
          {
            hospitalName = dataAccess.GetHospitalName();
          }
          catch (Exception exception)
          {
            LogException(exception);
          }
        }

        ablationData.Hospital = hospitalName;
        ablationData.Error = localCommonViewModel.GenericError;

        // Setting the Minimum Diaphragm Movement Value
        int currentDiaphragmMovementPercentageOrGReading = (int)DiaphragmMovementPercentageOrGReading;

        if (currentDiaphragmMovementPercentageOrGReading < MinimumDiaphragmMovementLastValue &&
            currentDiaphragmMovementPercentageOrGReading >= 0)
        {

          MinimumDiaphragmMovementLastValue = currentDiaphragmMovementPercentageOrGReading;
        }

        ablationData.MinimumDiaphragmMovementValue = MinimumDiaphragmMovementLastValue;

        //Setting the Minimum Esophagus Temperature Value
        int currentMinimumEsophagusTemperatureLastValue =
          (int)(Math.Round(EcgChannel5And6Reading)); // Convert.ToInt32(EcgChannel5And6Reading);  Emily 

        if (currentMinimumEsophagusTemperatureLastValue < MinimumEsophagusTemperatureLastValue &&
            currentMinimumEsophagusTemperatureLastValue > 0)
        {

          MinimumEsophagusTemperatureLastValue = currentMinimumEsophagusTemperatureLastValue;
        }

        ablationData.MinimumEsophagusTemperatureValue = MinimumEsophagusTemperatureLastValue;

        //Skin to Skin duration
        ablationData.SkinToSkinDuration = SkinToSkinDuration;

        //Adding the CJs values; TC1 CJ
        ablationData.CMCUCJReading = CMCUCJReading;

        ablationData.PMCUCJReading = PMCUCJReading;

        //Add Hardware information
        ablationData.CMCUFirmware = hardwareInformations.CMCUFirmware;
        ablationData.PMCUFirmware = hardwareInformations.PMCUFirmware;
        ablationData.RepeaterFirmware = hardwareInformations.RepeaterFirmware;
        ablationData.ICBFirmware = hardwareInformations.ICBFirmware;
        ablationData.CatheterFirmware = hardwareInformations.CatheterFirmware;
        ablationData.CPLDFirmware = hardwareInformations.CPLDFirmware;
        ablationData.ConsoleSerialNumber = hardwareInformations.ConsoleSerialNumber;
        ablationData.RemoteFirmware = hardwareInformations.RemoteFirmware;

        //Database version 
        ablationData.DatabaseVersion = DatabaseVersion;
        ablationData.GUIVersion = GUIVersion;

        /***ECG Data***/
        //DMS Value (G) 
        ablationData.EcgChannel3And4Reading = localCommonViewModel.EcgChannel3And4Reading;

        //DMS Value (%) 
        ablationData.EcgChannel7And8Reading = localCommonViewModel.EcgChannel7And8Reading;

        //ESO Temp 
        ablationData.EcgChannel5And6Reading = (int)(Math.Round(localCommonViewModel.EcgChannel5And6Reading));

        //BDI
        ablationData.BloodDetecorImValue = BloodDetecorImValue;

        //Pressure set point
        ablationData.PressureSetPoint = PressureSetPoint;

        //ETS sesnors
        if (IsMultiEtsSesnorConnected)
        {
          ablationData.EtsSensor1 = localCommonViewModel.EtsSesnor1;

          ablationData.EtsSensor2 = localCommonViewModel.EtsSesnor2;

          ablationData.EtsSensor3 = localCommonViewModel.EtsSesnor3;

          ablationData.EtsSensor4 = localCommonViewModel.EtsSesnor4;

          ablationData.EtsSensor5 = localCommonViewModel.EtsSesnor5;

          ablationData.EtsSensor6 = localCommonViewModel.EtsSesnor6;

          ablationData.EtsSensor7 = localCommonViewModel.EtsSesnor7;

          ablationData.EtsSensor8 = localCommonViewModel.EtsSesnor8;

          ablationData.EtsSensor9 = localCommonViewModel.EtsSesnor9;

          ablationData.EtsSensor10 = localCommonViewModel.EtsSesnor10;

          ablationData.EtsSensor11 = localCommonViewModel.EtsSesnor11;

          ablationData.EtsSensor12 = localCommonViewModel.EtsSesnor12;

          ablationData.EtsSensor13 = Math.Round(localCommonViewModel.EtsSesnor13);
        }
        else
        {
          ablationData.EtsSensor1 = 1000;

          ablationData.EtsSensor2 = 1000;

          ablationData.EtsSensor3 = 1000;

          ablationData.EtsSensor4 = 1000;

          ablationData.EtsSensor5 = 1000;

          ablationData.EtsSensor6 = 1000;

          ablationData.EtsSensor7 = 1000;

          ablationData.EtsSensor8 = 1000;

          ablationData.EtsSensor9 = 1000;

          ablationData.EtsSensor10 = 1000;

          ablationData.EtsSensor11 = 1000;

          ablationData.EtsSensor12 = 1000;

          ablationData.EtsSensor13 = 150;
        }

        ablationData.ISTTISelected = this.ISTTISelected;

        //ECG data 
        ablationData.EsophagusTemperatureThresholdReached = EsophagusTemperatureThresholdReached;
        ablationData.EsophagusTemperature = EsophagusTemperature;
        ablationData.IsDiaphragmMovementDetected = IsDiaphragmMovementDetected;
        ablationData.DiaphragmAmplitude = DiaphragmAmplitude;
        ablationData.DiaphragmAmplitudeThresholdReached = DiaphragmAmplitudeThresholdReached;
        ablationData.IgnoreMinimumDiaphragmMovement = IgnoreMinimumDiaphragmMovementValue;
        ablationData.DiaphragmSensorGain = DiaphragmSensorGain;
        ablationData.IsSystemMonitoringDiaphragmAlert = IsSystemMonitoringDiaphragmAlert;

        ablationData.BalloonSize = BalloonSizeFromPressureSetPoint(PressureSetPoint);
        ablationData.IsLowFlowActivated = IsLowFlowActivated;

        SingleAblationDatasList.Add(ablationData);
      }
    }

    /// <summary>
    /// Function that Loads the last Ablation Data for the procedure Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void LoadLastAblationData()
    {

      if (TotalTreatmentNumber != 0 && (TotalTreatmentNumber == TreatmentNumber))
        LoadPlaybackMode(TotalTreatmentNumber);
    }

    /// <summary>
    /// Function that Loads all the Ablation Data from a File for the procedure Playback.
    /// It gets the file content and deserialize it in the Ablation Data List
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void LoadAllAblationDataFromFile()
    {
      Stopwatch timeOutStopWatch = new Stopwatch();

      if (!ProcedureLogModel.CanReloadProcudreInformation)
      {
        CommonViewModel.Current.AllAblationDataList?.Clear();
        SingleAblationDatasList.Clear();
      }


      //Load all procedure's ablation treatments in a list
      if (treatmentNumber > 0)
      {
        for (int i = 1; i <= TotalTreatmentNumber; i++)
        {
          if (timeOutStopWatch.ElapsedMilliseconds > 5000)
            CommonViewModel.Current.GUIIsRunning = false;
          try
          {
            var fileLocation = GetTreatmentDetailsFile(i);

            if (fileLocation != "")
            {
              LoadAblationDataFromFile(fileLocation);
            }
          }
          catch (Exception exception)
          {
            LogException(exception);

            TreatmentNumberRefrence = 0;
            AbortAblation();
            CommonViewModel.Current.IsSystemInDataError = true;

            Tuple<long, string, string, string> genericMessage =
              Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID7, (int)Enumeration.ErrorTypes.GUI);
            
            DispatcherBeginInvoke(() =>
                {
                  MessagePopup dialogPopup = new MessagePopup(
                    exception.Message,
                    MessagePopup.MessageType.ErrorMessage,
                    MessagePopup.ButtonType.Ok,
                    messageTitle: genericMessage.Item2);
                  dialogPopup.ShowDialog();
                });
          }
        }
      }
    }

    /// <summary>
    /// Function that retrieves a given ablation file name from the database
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="treatmentNumber">The treatment number in the database.</param>
    /// <returns></returns>
    private string GetTreatmentDetailsFile(int treatmentNumber)
    {
      //Get the treatment file name to load the data
      string datafile = "";

      if (treatmentNumber > 0 && CommonViewModel.Current.CurrentPatient.HospitalPatientId != "" &&
          CommonViewModel.Current.CurrentProcedure.Id > 0)
      {
        try
        {
          datafile = dataAccess.GetAblationFileName(treatmentNumber,
            CommonViewModel.Current.CurrentPatient.HospitalPatientId, CommonViewModel.Current.CurrentProcedure.Id);
        }
        catch (Exception exception)
        {
          LogException(exception);
          throw;
        }
      }

      return datafile;
    }

    /// <summary>
    /// Function that Loads a Single Ablation Data from a File for the procedure Playback.
    /// It gets the file content and deserialize it in the Ablation Data List
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="filename">The ablation filename.</param>
    private void LoadAblationDataFromFile(string filename)
    {
      FileSerializer.JsonManager fs = new FileSerializer.JsonManager();
      List<AblationDataDetails> ablationDatasList;
      List<AblationECGData> ablationECGDataList;

      try
      {
        AblationFileDataStruct ablationData = fs.DeserializeAblationData<AblationFileDataStruct>(filename);
        ablationDatasList = ablationData.ConvertToAblationDataDetails();

        if (!ProcedureLogModel.CanReloadProcudreInformation)
          CommonViewModel.Current.AllAblationDataList.Add(ablationDatasList);
      }
      catch (FileNotFoundException e)
      {
        LogException(e);
        throw new Exception(
          "An error occurred while loading the treatment file in memory. Please exit to the Home screen and then return to the Therapy screen.",
          e);
      }
      catch (Exception exception)
      {
        LogException(exception);
        throw new Exception(
          "An error occurred while loading the treatment file in memory. Please exit to the Home screen and then return to the Therapy screen.",
          exception);
      }
    }

    /// <summary>
    /// Function that resets the cryotherapy counters, properties objects and lists.  It also invokes
    /// the reset cryotherapy event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetCryoTherapy()
    {
      try
      {
        CommonViewModel localCommonViewModel = CommonViewModel.Current;


        if (!ProcedureLogModel.CanReloadProcudreInformation)
        {

          localCommonViewModel.CurrentAblation = null;

          localCommonViewModel.AblationSummary = null;

          localCommonViewModel.AblationSummary = new AblationSummary(); //Clear Ablation Summary values

          RaisePropertyChanged("AblationSummary");

          AblationSite = AblationSiteEnum.OTHER;

          //Reload Last Information 

          SingleAblationDatasList?.Clear();
          localCommonViewModel.AllAblationDataList?.Clear();
        }

        AblationNumber = 0;
        TemperatureRate = 0;
        previousTemperature = 0;
        MaxTemperatureRate = 0;
        TimeToTargetTemperature = 0;
        TimeToThawTemperature = 0;
        VeinIsolationDuration = 0;
        TreatmentNumber = 0;
        TotalTreatmentNumber = 0;

        TemperatureReachedRequiredAblationTemperature = false;

        keepDisplayTimeToThaw = false;
        keepDisplayTimeToTemperature = false;
        IsTimeToTargetTemperatureVisible = false;
        IsVeinIsolationDurationVisible = false;

        TreatmentNumberRefrence = 0;

        //Update the displayed Patient Name
        RaisePropertyChanged("CurrentPatient");

        ResetDisplayWithPhysicianPreferences();

        //This call will trigger a display reset (charts, data) that is handled in the code-behind.
        this.ResetTherapyEvent?.Invoke(null, null);
      }
      catch (Exception exception)
      {
        LogException(exception);
        throw;
      }
    }

    /// <summary>
    /// Function that resets the display using the Physician's preferences
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetDisplayWithPhysicianPreferences()
    {
      //Physician currentPhysician = null;
      CommonViewModel localCommonViewModel = CommonViewModel.Current;

      try
      {
        if (NotificationModel != null &&
            NotificationModel.Instance != null &&
            NotificationModel.Instance.CurrentPhysician != null)
        {
          currentPhysician = NotificationModel.Instance.CurrentPhysician;

          //Reset display elements Physician's preferences
          if (currentPhysician != null && currentPhysician.preference != null)
          {
            var preference = currentPhysician.preference;
            AblationTimer = preference.AblationTimer;
            RequiredTargetTemperature = (int)preference.CoolingRequiredTargetTemperature;
            ThawTimerToTemperature = (int)preference.ThawTimerToTemperature;

            LowAblationTemperatureAlarm = (int)preference.LowAblationTemperatureAlarm;
            HighAblationTemperatureAlarm = (int)preference.HighAblationTemperatureAlarm;

            EsophagusTemperature = (int)preference.EsophagusTemperature;
            DiaphragmAmplitude = (int)preference.DiaphragmAmplitude;
            DMSDetectionThreshold = Math.Max(preference.DMSDetectionThreshold, Constants.MaxDMSDetectionThreshold);
            DiaphragmMovementPercentageSelected = preference.DiaphragmAmplitudeType == 1 ? true : false;
            DiaphragmSensorGain = Math.Min((int)preference.DiaphragmSensorGain, MaxDiaphragmSensorGain);
            RefrigerantLevelUnit = preference.RefrigerantLevelUnit;
            IsUsingAudioAlertSetting = preference.IsUsingAudioAlert;
            IsUsingAudioAlert = preference.IsUsingAudioAlert;
            IgnoreMinimumDiaphragmMovementValue = preference.IgnoreDiaphragmMovement;
            TipPressureSelected = preference.TipPressureSelected;

            EnabaleEnhancedAudio = preference.EnabaleEnhancedAudio;
            OcclusionPressureGraphAxisYMaximum = preference.OcclusionPressureGraphAxisYMaximum;
            OcclusionPressureGraphAxisYMinimum = preference.OcclusionPressureGraphAxisYMinimum;
            OcclusionPressureGraphSweepSpeed = preference.OcclusionPressureGraphSweepSpeed;
            if (IsSiteUsingDefalteAfterThaw)
            {
              DeflateAfterThaw = true;
            }
            else
            {
              DeflateAfterThaw = preference.IsUsingAutoDeflation;
            }

            EnableFastInflationMode = !preference.IsUsingInflationFastSpeed; // For historical reason, inflation speed is saved reversed in DB

            RequiredVolume = (uint)preference.VolumeLevel;
            IsUsingAutoPlayback = preference.IsUsingAutoPlayback;

            TemperatureChartType = preference.CurveStyle;
            CanDisplayShadowGraph = preference.IsUsingShadowing;

            AblationDurationType = (Enumeration.AblationDurationType)preference.AblationDurationType;
            IsFixedTimerSelected = AblationDurationType == Enumeration.AblationDurationType.FixedTimer;
            ISTTIFixedTimerSelected = AblationDurationType == Enumeration.AblationDurationType.TTIFixedTimer;
            ISTTIDurationTimerSelected = AblationDurationType == Enumeration.AblationDurationType.TTIDurationTimer;

            WasAblationTimeManuallyChanged = false;
            if (IsFixedTimerSelected)
            {
              RequiredAblationTime = (int)preference.AblationTimer;
            }
            else if (ISTTIFixedTimerSelected)
            {
              RequiredAblationTime = preference.NewAblationTimerTTIFixed;
            }
            else if (ISTTIDurationTimerSelected)
            {
              RequiredAblationTime = 240;
            }

            // Initialize Default timer settings for all possible settings 
            ExpectedTimeToVeinIsolation = preference.ExpectedVeinIsolationTime;
            AblationTimerTTIFixed = preference.AblationTimerTTIFixed;
            NewAblationTimerTTIFixed = preference.NewAblationTimerTTIFixed;

            DurationExpectedVeinIsolationTime = preference.DurationExpectedVeinIsolationTime;
            AblationTimerTTI = preference.AblationTimerTTI;
            NewAblationTimerTTI = preference.NewAblationTimerTTI;

            TemporaryManualAblationTime = RequiredAblationTime;
            ResetPlaybackSettings();
            IsSettingsDirty = false;
          }
        }
      }
      catch (Exception exception)
      {
        LogException(exception);
        throw;
      }
    }

    private void ResetAblationTimeSettings()
    {
      if (AblationDurationType == Enumeration.AblationDurationType.FixedTimer)
      {
        RequiredAblationTime = AblationTimer;

        IsFixedTimerSelected = true;
        ISTTIFixedTimerSelected = false;
        ISTTIDurationTimerSelected = false;
      }

      else if (AblationDurationType == Enumeration.AblationDurationType.TTIFixedTimer)
      {
        RequiredAblationTime = NewAblationTimerTTIFixed;

        IsFixedTimerSelected = false;
        ISTTIFixedTimerSelected = true;
        ISTTIDurationTimerSelected = false;
      }
      else if (AblationDurationType == Enumeration.AblationDurationType.TTIDurationTimer)
      {
        RequiredAblationTime = 240;

        IsFixedTimerSelected = false;
        ISTTIFixedTimerSelected = false;
        ISTTIDurationTimerSelected = true;
      }
    }

    private void ResetPlaybackSettings()
    {
      IgnoreMinimumDiaphragmMovementBindingValue = IgnoreMinimumDiaphragmMovementValue;
      EsophagusBindingTemperature = EsophagusTemperature;
      DiaphragmBindingAmplitude = DiaphragmAmplitude;

      RequiredTargetTemperatureBinding = RequiredTargetTemperature;
      ThawTimerToTemperatureBinding = ThawTimerToTemperature;
    }

    /// <summary>
    /// Function that invokes the Reset Therapy event when the Playback data is called
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ResetCryoTherapyPlayBackData()
    {
      //  Reset the  play back 
      IsTreatmentNumberAndPlayBackVisible = false;
      PressureSetPoint = 2.5;
      CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
      DASBalloonEnabled = false;
      //   RefreshWeightData();
      if (ListOfSesnorsState != null)
        ListOfSesnorsState.Clear();

      // Re-activate the Blood Pressure graph if system is in ready state and occlusion pressure sensor is connected and enabled.
      if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE
          || CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY
          || CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
      {
        IsMonitoringBloodPressure = true;
        RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");
      }

      RaisePropertyChanged("IsMultiEtsSesnorConnected");

      ResetPlaybackSettings();

      this.ResetTherapyEvent?.Invoke(null, null);

      if (WasAblationTimeManuallyChanged)
      {
        RequiredAblationTime = TemporaryManualAblationTime;
        ISTTISelected = false;
      }
      else
      {
        ResetAblationTimeSettings();
      }
    }

    /// <summary>
    /// Function that invokes the Reset the balloon seize
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <Id>SF-SDS-0140</Id>
    public void ResetDASBalloonSize()
    {

      PressureSetPoint = 2.5;
      CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
      DASBalloonEnabled = false;

    }

    /// <summary>
    /// Handles ablation timer according to vein isolation logic
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private void HandleAblationTimerAccordingToveinIsolationLogic()
    {

      if (ISTTIFixedTimerSelected)
      {
        if ((VeinIsolationDuration < ExpectedTimeToVeinIsolation && VeinIsolationDuration != 0) &&
            !CryoDurationChanged && (RequiredAblationTime != AblationTimerTTIFixed))
        {
          RequiredAblationTime = AblationTimerTTIFixed;
          AlertDurationValue = AlertMaximumDurationValue;
          CryoDurationChanged = true;
        }
        else if (VeinIsolationDuration >= ExpectedTimeToVeinIsolation &&
                 RequiredAblationTime != NewAblationTimerTTIFixed)
        {
          RequiredAblationTime = NewAblationTimerTTIFixed;
          AlertDurationValue = AlertMaximumDurationValue;
          CryoDurationChanged = true;
        }
      }

      else if (ISTTIDurationTimerSelected)
      {
        if (CryoDurationChanged)
        {
          if (VeinIsolationDuration < DurationExpectedVeinIsolationTime)
          {
            RequiredAblationTime = VeinIsolationDuration + AblationTimerTTI;

            if (RequiredAblationTime > maxAblationTimerUsingDurationMode)
              RequiredAblationTime = maxAblationTimerUsingDurationMode;

            AlertDurationValue = AlertMaximumDurationValue;
          }

          else
          {
            RequiredAblationTime = VeinIsolationDuration + NewAblationTimerTTI;

            if (RequiredAblationTime > maxAblationTimerUsingDurationMode)
              RequiredAblationTime = maxAblationTimerUsingDurationMode;

            AlertDurationValue = AlertMaximumDurationValue;
          }

          CryoDurationChanged = false;


        }
      }

      if (AlertDurationValue != 0)
      {
        AlertDurationValue--;
        if (IsRequiredAblationTimeVisible)
          IsRequiredAblationTimeVisible = false;
        else
        {
          IsRequiredAblationTimeVisible = true;
        }
      }

      else if (AlertDurationValue <= 0)
      {
        AlertDurationValue = 0;
        IsRequiredAblationTimeVisible = true;
      }
    }

    /// <summary>
    /// Checks vital parameters alerts
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void CheckvitalParametersAlerts()
    {
      CommonViewModel localCommonViewModel = CommonViewModel.Current;

      if (SensorReadingMananger.AreSensorsConnected)
      {
        // we don't send the data (3 beeps) in these situations:
        // 1. if we are not using the audio alert; or 
        // 2. if using AudioAlertMute is set; or
        // 3. if is in Simple View;
        if (!IsUsingAudioAlert || IsUsingAudioAlertMute || IsSimpleTherapyViewVisible)
        {
          localCommonViewModel.ActivateDiaphragmAndEsophagusAudioAlerts = false;
          return;
        }

        if (vitalParametersAlerts.ShouldDiaphragmMovementAlertTrigged(IsDiaphragmMovementDetected,
              DiaphragmAmplitudeThresholdReached, localCommonViewModel.SystemState,
              LastDiaphragmMovementPercentageOrGReadingValue)
            || vitalParametersAlerts.ShouldEsophagusTemperatureAlertTrigged(EsophagusTemperatureThresholdReached,
              localCommonViewModel.SystemState))
        {
          localCommonViewModel.ActivateDiaphragmAndEsophagusAudioAlerts = true;
        }

        else
        {
          localCommonViewModel.ActivateDiaphragmAndEsophagusAudioAlerts = false;
        }

      }
      else
      {
        localCommonViewModel.ActivateDiaphragmAndEsophagusAudioAlerts = false;
      }
    }

    /// <summary>
    /// Function that verifies if any list in AllAblationDataList has a count of 0
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <return>If the AllAblationDataList is not empty.</return>
    private bool VerifyAllAblationDataListNotEmpty()
    {
      bool isValid = true;

      if (CommonViewModel.Current.AllAblationDataList != null && CommonViewModel.Current.AllAblationDataList.Count > 0)
      {
        foreach (List<AblationDataDetails> ablationDataList in CommonViewModel.Current.AllAblationDataList)
        {
          if (ablationDataList.Count < 1)
          {
            isValid = false;
            break;
          }
        }
      }

      return isValid;
    }

    /// <summary>
    /// Set required ablation time according to state
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void SetrequiRedAblationTimeAccordingToState(int _redAblationTime)
    {
      RequiredAblationTimeAccordingToState = _redAblationTime;
      RaisePropertyChanged("RequiredAblationTimeAccordingToState");

      RequiredAblationTime = _redAblationTime;
    }

    /// <summary>
    /// Function Refresh Ablation Time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void RefreshModeldata()
    {
      RequiredAblationTime = (RequiredAblationTime);
    }

    /// <summary>
    /// Gets or sets a value indicating whether is pressure set point reached or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool EvaluateIBPWithPressureSetPoint(double ibpValue)
    {
      if (IsInDASBalloonTransition)
      {
        if (DASBalloonEnabled) //Ramp up to 7.5
        {
          if (ibpValue >= (PressureSetPoint - AbsolutePressureError))
          {
            IsInDASBalloonTransition = false;
          }
        }
        else //Ramp down to 2.5
        {
          if (ibpValue <= (PressureSetPoint + AbsolutePressureError) &&
              ibpValue >= (PressureSetPoint - AbsolutePressureError))
          {
            IsInDASBalloonTransition = false;
          }
        }
      }

      var systemState = CommonViewModel.Current.SystemState;
      if (!ISThePressureSetPointReached && ibpValue >= (PressureSetPoint - AbsolutePressureError)
          && (systemState == MessageStateId.CAN_ID_STATE_THAWING ||
              systemState == MessageStateId.CAN_ID_STATE_ABLATION ||
              systemState == MessageStateId.CAN_ID_STATE_TRANSITION ||
              systemState == MessageStateId.CAN_ID_STATE_INFLATION))
      {
        ISThePressureSetPointReached = true;
      }
      else
      {
        if (systemState == MessageStateId.CAN_ID_STATE_IDLE || 
                               SystemState == MessageStateId.CAN_ID_STATE_READY ||
                               SystemState == MessageStateId.CAN_ID_STATE_EXCEPTION) 
          ISThePressureSetPointReached = false;
      }

      return ISThePressureSetPointReached;
    }

    /// <summary>
    /// Gets or sets a value indicating whether is used for engineering or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsedForEngineering
    {

      get { return (CommonViewModel.Current.IsUsedForEngineering && IsCatheterCableConnected); }

      set
      {
        CommonViewModel.Current.IsUsedForEngineering = value;
        RaisePropertyChanged("IsUsedForEngineering");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is data loading or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DataLoading
    {
      get { return dataLoading; }
      set
      {
        if (value != dataLoading)
        {
          dataLoading = value;
          //  SetProperty(ref dataLoading, value);
          RaisePropertyChanged("DataLoading");
          RaisePropertyChanged(nameof(CanAblationNumberForward));
          RaisePropertyChanged(nameof(CanAblationNumberBackward));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value for thawing elapsed time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ThawingElapsedTime
    {
      get => thawingElapsedTime;
      set => SetProperty(ref thawingElapsedTime, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether display thawing ballon or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DisplayThawingBallon
    {
      get { return displayThawingBallon; }
      set
      {
        displayThawingBallon = value;
        RaisePropertyChanged("DisplayThawingBallon");
      }
    }

    /// <summary>
    /// Gets or sets a value for skin to skin duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int SkinToSkinDuration
    {
      get { return CommonViewModel.Current.SkinToSkinDuration; }
      set { CommonViewModel.Current.SkinToSkinDuration = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether skin to skin count started or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool SkinToSkinCountStarted
    {
      get { return skinToSkinCountStarted; }
      set { skinToSkinCountStarted = value; }
    }

    /// <summary>
    /// Gets or sets a value for catheter type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Enumeration.CatheterType CatheterType
    {
      get
      {

        if (IsCatheterCableConnected)
        {
          if (IsSystemUsingDASBalloon)
          {
            return Enumeration.CatheterType.Plus;
          }
          else
          {
            return Enumeration.CatheterType.ID28mm;
          }
        }

        else
        {
          return Enumeration.CatheterType.ID_UNKNOWN_mm;
        }

      }
    }

    /// <summary>
    /// Gets or sets a value for decreasing compter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DecreasingCompter
    {
      get { return decreasingCompter; }
      set { decreasingCompter = value; }
    }

    /// <summary>
    /// Gets or sets a value for previous TC1 reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PreviousTC1Reading
    {
      get { return previousTC1Reading; }
      set { previousTC1Reading = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in idle or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInIdle
    {
      get { return isSystemInIdle; }
      set { isSystemInIdle = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in ready or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0097</id>
    public bool IsSystemInReady
    {
      get { return isSystemInReady; }
      set { isSystemInReady = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in inflation or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInInflation
    {
      get { return isSystemInInflation; }
      set { isSystemInInflation = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in transition or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInTransition
    {
      get { return isSystemInTransition; }
      set { isSystemInTransition = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in ablation or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInAblation
    {
      get { return isSystemInAblation; }
      set { isSystemInAblation = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is system in exception or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemInException
    {
      get { return isSystemInException; }
      set { isSystemInException = value; }
    }

    /// <summary>
    /// Gets or sets a value for CMCUCJ Reading Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CMCUCJReadingPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value for PMCUCJ Reading Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PMCUCJReadingPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value for blood detecor impedance value in playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int BloodDetecorImValuePlayback { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether the blood pressure sensor connected in playback.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsBloodPressureSensorConnectedPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Multi ETS sensor connected in playback.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsMultiEtsSesnorConnectedPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value for Last elapsed time for flow reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ElapsedTimeLastValueForFlowReading
    {
      get { return elapsedTimeLastValueForFlowReading; }

      set { elapsedTimeLastValueForFlowReading = value; }
    }

    /// <summary>
    /// Gets or sets a value for Last elapsed time for IBP reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ElapsedTimeLastValueForIBPReading
    {
      get { return elapsedTimeLastValueForIBPReading; }

      set { elapsedTimeLastValueForIBPReading = value; }
    }

    /// <summary>
    /// Gets or sets a value for last flow reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LastFlowReadingValue
    {
      get { return lastFlowReadingValue; }
      set { lastFlowReadingValue = value; }
    }

    /// <summary>
    /// Gets or sets a value for last IBP reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LastIBPReadingValue
    {
      get { return lastIBPReadingValue; }
      set { lastIBPReadingValue = value; }
    }

    /// <summary>
    /// Gets or sets the time previous refrence
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimePreviousRefrence
    {
      get { return timePreviousRefrence; }
      set { timePreviousRefrence = value; }
    }

    /// <summary>
    /// Gets or sets last cryoTherapy time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int LastCryoTherapyTime
    {
      get { return lastCryoTherapyTime; }

      set { lastCryoTherapyTime = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the system is allowed to set playBack mode
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsAllowedToSetPlayBack
    {
      get { return isAllowedToSetPlayBack; }

      set { isAllowedToSetPlayBack = value; }
    }

    /// <summary>
    /// Gets or sets the timing filter value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimingFiliter
    {
      get { return timingFiliter; }
      set { timingFiliter = value; }
    }

    /// <summary>
    /// Gets or sets the database version value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DatabaseVersion
    {
      get { return databaseVersion; }
      set { databaseVersion = value; }
    }


    /// <summary>
    /// Gets or sets the database ablation duration type value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Enumeration.AblationDurationType AblationDurationType
    {
      get { return ablationDurationType; }
      set
      {

        ablationDurationType = value;
        RaisePropertyChanged("AblationDurationType");
      }
    }


    /// <summary>
    /// Gets or sets the GUI version value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string GUIVersion
    {
      get { return gUIVersion; }
      set { gUIVersion = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the esophagus temperature is in range
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsEsophagusTemperatureInRange
    {
      get { return isEsophagusTemperatureInRange; }
      set
      {
        isEsophagusTemperatureInRange = value;
        RaisePropertyChanged("IsEsophagusTemperatureInRange");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether we are monitoring blood pressure.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsMonitoringBloodPressure
    {
      get { return isMonitoringBloodPressure; }
      set
      {
        isMonitoringBloodPressure = value;
        RaisePropertyChanged("IsMonitoringBloodPressure");
      }
    }

    public bool DisplayBloodPressure
    {
      get { return displayBloodPressure; }
      set
      {
        displayBloodPressure = value;
        RaisePropertyChanged("DisplayBloodPressure");
      }
    }



    /// <summary>
    /// Gets or sets the blood pressure maximum value during one second.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double BloodPressureMaximumValueDuringOneSecond
    {
      get { return Math.Floor(bloodPressureMaximumValueDuringOneSecond); }
      set
      {
        bloodPressureMaximumValueDuringOneSecond = value;
        RaisePropertyChanged("BloodPressureMaximumValueDuringOneSecond");
      }
    }

    /// <summary>
    /// Gets or sets the thawing temperature set point.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSiteUsingDefalteAfterThaw
    {
      get { return CommonViewModel.Current.Console.EnableDefalteAfterThaw; }
      set
      {
        CommonViewModel.Current.Console.EnableDefalteAfterThaw = value;
        RaisePropertyChanged("IsSiteUsingDefalteAfterThaw");
      }
    }

    /// <summary>
    /// Gets or sets the thawing temperature set point.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsThawingTemperatureSetPointReached
    {
      get { return isThawingTemperatureSetPointReached; }
      set
      {
        isThawingTemperatureSetPointReached = value;
        RaisePropertyChanged("IsThawingTemperatureSetPointReached");
      }
    }

    /// <summary>
    /// Gets/sets the value indicating whether enabale enhanced audio.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnabaleEnhancedAudio
    {
      get { return CommonViewModel.Current.Console.EnabaleEnhancedAudio; }

      set
      {
        CommonViewModel.Current.Console.EnabaleEnhancedAudio = value;
        RaisePropertyChanged("EnabaleEnhancedAudio");
      }

    }

    /// <summary>
    /// Sets idle model parameters
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void SetIdleModelParameters()
    {
      StopAblationTimer();
      CurrentAblation = null;
      IsIsolatingVein = false;
      CryoDurationChanged = true;
      IsTimeToTargetTemperatureVisible = true;
      IsVeinIsolationDurationVisible = true;
      IsDiaphragmMovementVisible = true;
      IsEsophagusTemperatureVisible = true;
      IsCatheterConnectedAndInIReadyState = false;
      IsSnowFlakeVisible = false;
      if (!IsTreatmentNumberAndPlayBackVisible)
      {
        IsAblationTimeVisibale = false;
      }

      IsSystemMonitoringDiaphragmAlert = false;
      AllowPSPChangeDuringThawing = false;
      IsRequiredAblationTimeVisible = true;

      UpdateSystemStateProperties(MessageStateId.CAN_ID_STATE_IDLE);

      if (PreviousSystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
      {
        IsTreatmentNumberAndPlayBackVisible = false;
      }

      if (SensorReadingMananger.AreSensorsConnected)
      {
        IsEsophagusTemperatureConditionAlertsMeet = false;
      }

      IsMonitoringBloodPressure = SensorReadingMananger.AreSensorsConnected;

      RaisePropertyChanged("IsBloodPressureSensorConnected");
      RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");

      AllowUserToActivateLowFlow = false;
      if (!isTreatmentNumberAndPlayBackVisible)
      {
        IsLowFlowActivated = false;
      }

      RaisePropertyChanged("IsMultiEtsSesnorConnected");

      if (playbackOffTimeReset && !IsUsingAutoPlayback && !CommonViewModel.Current.AreSensorsInPlayBackMode)
      {
        if (AblationDurationType == Enumeration.AblationDurationType.FixedTimer)
        {
          RequiredAblationTime = AblationTimer; //240;
          TemporaryManualAblationTime = RequiredAblationTime;

          IsFixedTimerSelected = true;
          ISTTIFixedTimerSelected = false;
          ISTTIDurationTimerSelected = false;
        }

        else if (AblationDurationType == Enumeration.AblationDurationType.TTIFixedTimer)
        {
          RequiredAblationTime = NewAblationTimerTTIFixed;
          TemporaryManualAblationTime = RequiredAblationTime;

          IsFixedTimerSelected = false;
          ISTTIFixedTimerSelected = true;
          ISTTIDurationTimerSelected = false;
        }
        else if (AblationDurationType == Enumeration.AblationDurationType.TTIDurationTimer)
        {
          RequiredAblationTime = 240;
          TemporaryManualAblationTime = RequiredAblationTime;

          IsFixedTimerSelected = false;
          ISTTIFixedTimerSelected = false;
          ISTTIDurationTimerSelected = true;
        }

        playbackOffTimeReset = false;
      }
    }

    /// <summary>
    /// Set Ready Model Parameters and avoid to set balloon pressure to 6.5 PSI
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <return>If the AllAblationDataList is not empty.</return>
    /// <Id>SF-SDS-0137</Id>
    private void SetReadyModelParameters()
    {
      StopAblationTimer();
      CurrentAblation = null;
      IsCatheterConnectedAndInIReadyState = true;
      IsDiaphragmMovementVisible = true;
      IsEsophagusTemperatureVisible = true;
      IsIsolatingVein = false;
      IsSnowFlakeVisible = false;

      IsUsingAudioAlertMute = false;
      if (!IsTreatmentNumberAndPlayBackVisible)
        IsAblationTimeVisibale = false;

      IsSystemMonitoringDiaphragmAlert = false;
      AllowPSPChangeDuringThawing = false;
      IsRequiredAblationTimeVisible = true;

      if (PreviousSystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY &&
          !IsTreatmentNumberAndPlayBackVisible)
      {
        CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
        DASBalloonEnabled = false;
      }

      UpdateSystemStateProperties(MessageStateId.CAN_ID_STATE_READY);

      if (SensorReadingMananger.AreSensorsConnected)
      {
        IsEsophagusTemperatureConditionAlertsMeet = false;
      }

      IsThawingTemperatureSetPointReached = false;

      //Verify if reading the firmware does not create issue for system during treatment. these line of code can be be moved to inflation state

      //#if !DEBUG
      //           CommonViewModel.Current.ReadRepeaterAndICBFirmware(1);
      //#endif

      // Display or hide depending on if Playback mode is On or Off
      IsMonitoringBloodPressure = SensorReadingMananger.AreSensorsConnected;

      RaisePropertyChanged("IsBloodPressureSensorConnected");
      RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");

      if (CommonViewModel.Current.CatheterFirmwareVersion == 0)
      {
        CommonViewModel.Current.Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE,
          CatheterFirmwareVersionId);
      }

      AllowUserToActivateLowFlow = false;
      if (!IsTreatmentNumberAndPlayBackVisible)
      {
        IsLowFlowActivated = false;
      }

      playbackOffTimeReset = true;

      StartProcedureElapsedTimer();

      OnReady(null, null);
    }


    /// <summary>
    /// Sets inflation model parameters
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void SetInflationModelParameters()
    {
      CommonViewModel localCommonViewModel = CommonViewModel.Current;

      ProcedureLogModel.CanReloadProcudreInformation = false;
      if (IsTreatmentNumberAndPlayBackVisible)
      {
        ResetCryoTherapyPlayBackData();
      }

      if (TotalTreatmentNumber != 0)
        AblationInformation.IsThereAbltionHistoricalData = true;
      else
        AblationInformation.IsThereAbltionHistoricalData = false;

      StopAblationTimer();

      if (PreviousSystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
      {
        SensorReadingMananger.ConnectSensors();
        IsTreatmentNumberAndPlayBackVisible = false;

        if (this.inflationEvent == null)
        {
          this.inflationEvent = new InflationEventArgs();
        }

        PressureSetPoint = 2.5;

        OnInflation(null, inflationEvent);
      }

      CurrentAblation = null;

      IsTimeToTargetTemperatureVisible = true;
      IsVeinIsolationDurationVisible = true;

      //Reset
      MaxTemperatureRate = 0;
      TemperatureRate = 0;
      CryoTherapyTime = 0;
      TimeInAblationMax = 0;
      LastCryoTherapyTime = 0;
      VeinIsolationDuration = 0;
      // TimeTTIStartPoint = 0;  //Emily
      IsCatheterConnectedAndInIReadyState = false;
      IsTreatmentNumberAndPlayBackVisible = false;
      IsSnowFlakeVisible = true;
      IsAblationTimeVisibale = false;
      IsSystemMonitoringDiaphragmAlert = false;
      AllowPSPChangeDuringThawing = true;
      //wasAblationTimeManuallyChanged = false;

      UpdateSystemStateProperties(MessageStateId.CAN_ID_STATE_INFLATION);

      //Perpare The Hardware Information

      try
      {
        hardwareInformations.CMCUFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.CentralMicroControllerFirmwareVersion);
        hardwareInformations.PMCUFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.PatientMicroControllerFirmwareVersion);
        hardwareInformations.RepeaterFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.RepeaterFirmware);
        hardwareInformations.ICBFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.ICBFirmware);
        hardwareInformations.CatheterFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.CatheterFirmwareVersion);
        hardwareInformations.CPLDFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.CpldFirmwareVersion);
        hardwareInformations.ConsoleSerialNumber = this.dataAccess.GetConsoleSerialNumber();
        hardwareInformations.RemoteFirmware =
          FirmwareConverter.ConvertToMicrosfotversioning(localCommonViewModel.RemoteControlFirmware);
      }

      catch (Exception ex)
      {
        LogException(ex);
      }

      IsThawingTemperatureSetPointReached = false;
      IsMonitoringBloodPressure = true;
      RaisePropertyChanged("IsBloodPressureSensorConnected");
      RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");

      StartProcedureElapsedTimer();
    }

    /// <summary>
    /// Update skin to skin time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void UpdateSkinToSkinTime()
    {
      double catheterTemperature = CommonViewModel.Current.CatheterTemperature;

      if ((catheterTemperature >= SkinToSkinTemperature || SkinToSkinDuration != 0) && IsCatheterCableConnected)
      {
        if (CommonViewModel.Current.SystemState !=
            Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY &&
            CommonViewModel.Current.SystemState !=
            Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
        {
          DecreasingCompter = 0;
          SkinToSkinDuration++;
          RaisePropertyChanged("InBodyTime");
        }
        else
        {
          if (catheterTemperature >= SkinToSkinTemperature)
          {
            DecreasingCompter = 0;
            SkinToSkinDuration++;
            RaisePropertyChanged("InBodyTime");
          }
          else
          {
            if (SkinToSkinDuration != 0)
            {

              if (DecreasingCompter == 0)
              {
                PreviousTC1Reading = catheterTemperature;
              }
              else if (DecreasingCompter > 30)
              {
                double DeltaTemperature = PreviousTC1Reading - catheterTemperature;
                if (DeltaTemperature > 3)
                  return;
              }

              DecreasingCompter++;
              SkinToSkinDuration++;
              RaisePropertyChanged("InBodyTime");
            }

          }

        }
      }
    }


    /// <summary>
    /// Abort ablation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void AbortAblation()
    {
      CommonViewModel.Current.Console.Stop();
      CommonViewModel.Current.LogUserAction(Enumeration.Actions.StopCommand);
      IsIsolatingVein = false;
      IsVeinIsolationDurationVisible = true;
    }


    /// <summary>
    /// Refresh the N2O weight and unit(kg/lbs)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void RefreshWeightData()
    {
      RaisePropertyChanged("LC1Reading");
    }

    /// <summary>
    /// Validate the com port.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <returns>true if the port is valide </returns>
    private bool IsPortComValid()
    {
      string[] allPortComList = SerialPort.GetPortNames();
      if (PortName != string.Empty && allPortComList.Contains(PortName) && !InvalidPortComList.Contains(PortName))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets the port name.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string PortName
    {
      get { return CommonViewModel.Current.PortName; }

      set
      {
        CommonViewModel.Current.PortName = value;
        RaisePropertyChanged("PortName");


      }

    }

    private bool _isLowFlowActivatedForPlayback; 

    /// <summary>
    /// Gets or sets a value indicating whether the low flow is activated
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLowFlowActivated
    {
      get => IsTreatmentNumberAndPlayBackVisible 
                 ? _isLowFlowActivatedForPlayback
                 : CommonViewModel.Current.IsLowFlowActivated;
      set
      {
        if (IsTreatmentNumberAndPlayBackVisible)
          _isLowFlowActivatedForPlayback = value;
        else 
          CommonViewModel.Current.IsLowFlowActivated = value;
        
        RaisePropertyChanged("IsLowFlowActivated");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the console is using low flow
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingLowFlow
    {
      get { return CommonViewModel.Current.IsUsingLowFlow; }

      set
      {
        //CommonViewModel.Current.IsUsingLowFlow = value;
        //RaisePropertyChanged("IsUsingLowFlow");

      }

    }



    /// <summary>
    /// Gets or sets a value indicating whether the can allow a user to use low flow 
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool AllowUserToActivateLowFlow
    {
      get
      {
        
          return allowUserToActivateLowFlow;
        
      }

      set
      {
        allowUserToActivateLowFlow = value;
        RaisePropertyChanged("AllowUserToActivateLowFlow");

      }

    }

    /// <summary>
    /// Gets or sets the invalid Port COM list.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<string> InvalidPortComList
    {
      get => invalidPortComList;
      set => invalidPortComList = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ablation site is changed.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsAblationSiteChanged
    {
      get => isAblationSiteChanged; 
      set => SetProperty(ref isAblationSiteChanged, value); 
    }

    internal async void PlayAudioFile()
    {
      try
      {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "Documentation", "speech.wav");
        await Task.Run(() =>
        {
          System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
          player.Play();
        });
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// Gets or sets the lowest temp channel number.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<int> LowestTempChannelNum
    {
      get { return lowestTempChannelNum; }
      set
      {

        lowestTempChannelNum = value;
        RaisePropertyChanged("LowestTempChannelNum");
      }
    }

    /// <summary>
    /// Convert the DMS to ten base
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="DMSDetectionThreshold"> the value to convert</param>
    /// <returns> to dms base value</returns>
    private int ConvertTheDMSTOTenBase(double DMSDetectionThreshold)
    {
      double dMSTOTenBase = -100 * DMSDetectionThreshold + 11;

      return (int)Math.Round(dMSTOTenBase, 0);
    }

    private double ConvertTheTenBaseTODMS(int value)
    {
      double TenBase = (double)(11 - value) / 100;
      return Math.Round(TenBase, 2);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Circa is using.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingCirca
    {
      get { return isUsingCirca; }
      set
      {
        isUsingCirca = value;
        RaisePropertyChanged("IsUsingCirca");
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether tip value is using.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool HasTip
    {
      get { return hasTip; }
      set
      {
        hasTip = value;
        RaisePropertyChanged("HasTip");
      }
    }

    private string LastUsedTTitype { get; set; }

    private void SaveTheLastLastUsedTTitype()
    {
      if (ISTTIDurationTimerSelected && !IsFixedTimerSelected && ISTTISelected)
        TTIMemoryState.LastUsedTTitype = "ISTTIDurationTimerSelected";
      if (ISTTIFixedTimerSelected && !IsFixedTimerSelected && ISTTISelected)
        TTIMemoryState.LastUsedTTitype = "ISTTIDurationTimerSelected";
    }

    private int _TTIResetCount;

    public int TTIResetCount
    {
      get => _TTIResetCount;
      set => SetProperty(ref _TTIResetCount, value);
    }

    public void OnVeinCmd(bool? moveTTI)
    {
      if (moveTTI == true)
      {
        OnVeinIsolatedCommand(moveTTI);
      }
      else
      {
        VeinIsolationDuration = 0;
        CryoDurationChanged = false;

        if (!WasAblationTimeManuallyChanged)
          ResetAblationTimeSettings();
      }

      TTIResetCount = 0;
    }

    private void UpdateSystemStateProperties(MessageStateId currentState)
    {
      IsSystemInIdle = currentState == MessageStateId.CAN_ID_STATE_IDLE;
      IsSystemInReady = currentState == MessageStateId.CAN_ID_STATE_READY;
      IsSystemInInflation = currentState == MessageStateId.CAN_ID_STATE_INFLATION;
      IsSystemInTransition = currentState == MessageStateId.CAN_ID_STATE_TRANSITION;
      IsSystemInAblation = currentState == MessageStateId.CAN_ID_STATE_ABLATION;
      IsSystemInThawing = currentState == MessageStateId.CAN_ID_STATE_THAWING;
      IsSystemInException = currentState == MessageStateId.CAN_ID_STATE_EXCEPTION;
    }

    private void CalculateMaxAvgHRPacingLevel(IObservable<double> dmsData)
    {
      if (!HighResDmsSignalDetected)
      {
        MaximumHRAveragePacingLevel = 0;
      }
      else
      {
        CalculateMaxAvgPacingLevel(dmsData, (d) => MaximumHRAveragePacingLevel = d, true);
      }
    }

    private void CalculateMaxAvgPacingLevel(IObservable<double> dmsData)
    {
      CalculateMaxAvgPacingLevel(dmsData, (d) => MaximumAveragePacingLevel = d, false);
    }

    private void CalculateMaxAvgPacingLevel(IObservable<double> dmsData, Action<double> updateMaxProperty, bool isHighResDms = false)
    {
      if (CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_ABLATION &&
          CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_TRANSITION &&
          CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_THAWING)
      {
        updateMaxProperty(0);
        return;
      }

      try
      {
        dmsData
          .Max()
          .Subscribe(d => updateMaxProperty(Math.Max(isHighResDms ? MaximumHRAveragePacingLevel : MaximumAveragePacingLevel, Math.Min(d, Constants.PacingLevelMaxvalue))),
            _ => { },
            () => { });
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    private void SubscribeBloodPressureSensorDataUpdate()
    {
      Observable.FromEventPattern<PropertyChangedEventArgs>(CommonViewModel.Current, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.Current.EcgChannel1And2Reading))
        .Select(_ => CommonViewModel.Current.EcgChannel1And2Reading)
        .Window(TimeSpan.FromMilliseconds(1000), TaskPoolScheduler.Default)
        .Subscribe((window) =>
        {
          try
          {
            window.Max()
              .Subscribe(value => BloodPressureMaximumValueDuringOneSecond = value,
                _ => { },
                () => { });
          }
          catch (Exception ex)
          {
            LogException(ex);
          }
        });
    }

    public int MaxDiaphragmSensorGain => Constants.MaxDiaphragmSensorGain;
    public int MaxDMSDetectionThreshold => ConvertTheDMSTOTenBase(Constants.MaxDMSDetectionThreshold);

    public bool IsSimpleTherapyViewVisible
    {
      get => !NotificationModel.Instance.CurrentPhysician.preference.IsShowAdvancedView;
      set
      {
        if (value == !NotificationModel.Instance.CurrentPhysician.preference.IsShowAdvancedView)
        {
          return;
        }

        if (NotificationModel?.CurrentPhysician?.preference != null)
        {
          NotificationModel.CurrentPhysician.preference.IsShowAdvancedView = !value;
          try
          {
            NotificationModel.SaveNotification();
          }
          catch (Exception e)
          {
            LogException(e);
          }
          finally
          {
            RaisePropertyChanged();
          }
        }
      }
    }

    public bool IsPatientNameVisible
    {
      get => NotificationModel.Instance.CurrentPhysician.preference.IsShowPatientInfo;
      set
      {
        if (value == NotificationModel.Instance.CurrentPhysician.preference.IsShowPatientInfo)
        {
          RaisePropertyChanged();
          return;
        }

        if (NotificationModel?.CurrentPhysician?.preference != null)
        {
          NotificationModel.CurrentPhysician.preference.IsShowPatientInfo = value;
          try
          {
            NotificationModel.SaveNotification();
          }
          catch (Exception e)
          {
            LogException(e);
          }
          finally
          {
            RaisePropertyChanged();
          }
        }
      }
    }

    #region Shadowing Temperature Graph

    private readonly ISubject<AblationSiteEnum> _ablationSiteObservable;
    private readonly ISubject<bool> _updateShadowTemperatureGraphObservable = new BehaviorSubject<bool>(false);
    private bool _canDisplayShadowGraph = false;

    public bool CanDisplayShadowGraph
    {
      get => _canDisplayShadowGraph;
      set
      {
        SetProperty(ref _canDisplayShadowGraph, value);
        ValidateUpdatingShadowTemperatureGraph();
      }
    }

    // public IObservable<AblationSummary.AblationSiteEnum> AblationSiteObservable => _ablationSiteObservable;
    public IObservable<bool> UpdateShadowTemperatureGraphObservable => _updateShadowTemperatureGraphObservable;

    private async void ValidateUpdatingShadowTemperatureGraph()
    {
      var currentState = CommonViewModel.Current.SystemState;
      if (_canDisplayShadowGraph &&
          (currentState == MessageStateId.CAN_ID_STATE_TRANSITION ||
           currentState == MessageStateId.CAN_ID_STATE_ABLATION ||
           currentState == MessageStateId.CAN_ID_STATE_THAWING))
      {
        await Task.Run(() =>
        {
          UpdateHistoricalAblationDataByAblationSite(AblationSite);
          _updateShadowTemperatureGraphObservable.OnNext(true);
        });
      }
      else
      {
        _updateShadowTemperatureGraphObservable.OnNext(false);
      }
    }

    public List<List<AblationDataDetails>> HistoricalAblationData { get; set; }

    private void UpdateHistoricalAblationDataByAblationSite(AblationSiteEnum ablationSite)
    {
      lock (CommonViewModel.Current.AllAblationDataList)
      {
        HistoricalAblationData = CommonViewModel.Current.AllAblationDataList?
          .Where(ab => ab != SingleAblationDatasList &&  ab.LastOrDefault()?.AblationSite == (int)ablationSite)
          .ToList();
      }
    }

    #endregion Shadowing Temperature Graph

    public bool IsSettingsDirty { get; set; }

    public void UpdateAblationSiteChanged(AblationSiteEnum newAblationSite)
    {
      if (CommonViewModel.Current.AreSensorsInPlayBackMode)
      {
        CommonViewModel.Current.UpdateAblationSite(TreatmentNumber, newAblationSite);
        CommonViewModel.Current.GenerateAblationSummary(); // Do not use UpdateSummary, it needs to be regenerated
        CommonViewModel.Current.AblationSite = newAblationSite;
      }
    }

    public void RefreshUIProperties()
    {
      RaisePropertyChanged(nameof(SystemState));
      RaisePropertyChanged(nameof(LC1Reading));
      SystemStatePropertyUpdated();
    }

    public void ToggleIsUsingAudioAlertMute()
    {
      IsUsingAudioAlertMute = !IsUsingAudioAlertMute;
    }
  }
}
