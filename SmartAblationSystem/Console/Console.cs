using Communication;
using Console.Configurations;
using Console.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using static Communication.CanBusMessageDefinition;
using static LogSystem.LogService;

namespace Console
{
  /// <summary>
  /// Represents the console device
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class Machine //:IDisposable
  {
    private readonly double tENN_POUND_TANK_METAL_WEIGHT = 15; //14.77  ID 1
    private readonly double fIFTEEN_POUND_TANK_METAL_WEIGHT = 18.8; //ID 2

    private ICanBusCommunication canBusCommunication;
    private IGeneralPurposeInputOutput generalPurposeInputOutput;

    // Pressure Transducer
    private PressureTransducerOne pressureTransducerOne;

    private PressureTransducerTwo pressureTransducerTwo;
    private PressureTransducerThree pressureTransducerThree;
    private PressureTransducerFour pressureTransducerFour;
    private List<IPressureTransducer> listOfPressureTransducer;
    private Dictionary<MessageStateId, PressureTransducerOne> pressureTransducerOneValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PressureTransducerTwo> pressureTransducerTwoValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PressureTransducerThree> pressureTransducerThreeValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PressureTransducerFour> pressureTransducerFourValueAccordingToTheStateMachine;

    private PatientPressureTransducerOne patientPressureTransducerOne;
    private PatientPressureTransducerTwo patientPressureTransducerTwo;
    private List<IPressureTransducer> listOfPatientPressureTransducer;
    private Dictionary<MessageStateId, PatientPressureTransducerOne> patientPressureTransducerOneValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PatientPressureTransducerTwo> patientPressureTransducerTwoValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, InjectionFlow> injectionFlowValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, InjectionPressure> injectionPressureValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, InjectionFlow> lowInjectionFlowValueAccordingToTheStateMachine;

    // Pressure Switch (PS1 and PS2)
    private PressureSwitchOne pressureSwitchOne;

    private PressureSwitchTwo pressureSwitchTwo;
    private List<IPressureSwitch> listOfPressureSwitch;
    private Dictionary<MessageStateId, PressureSwitchOne> pressureSwitchOneValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PressureSwitchTwo> pressureSwitchTwoValueAccordingToTheStateMachine;

    // Thermocouples and Temperature Sensor
    private ThermocoupleOne thermocoupleOne;

    private ThermocoupleTwo thermocoupleTwo;
    private List<IThermocouple> listOfThermocouple;
    private Dictionary<MessageStateId, ThermocoupleOne> thermocoupleOneValueAccordingToTheStateMachine;

    private TemperatureSensorOne temperatureSensorOne;
    private List<IThermocouple> listOfTemperatureSensor;
    private Dictionary<MessageStateId, TemperatureSensorOne> temperatureSensorOneValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, PatientMicroControllerPID> patientMicroControllerPIDValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, CentralMicroControllerPID> centralMicroControllerPIDValueAccordingToTheStateMachine;

    private Dictionary<MessageStateId, PatientMicroControllerBalloonPressureRegulator> patientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine;
    private Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator> centralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine;

    //Ramp up and Ramp down Configuration
    private Dictionary<MessageStateId, CryoBalloonConfiguration> patientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine;

    // Flow meter
    private FlowMeter flowMeterOne;

    private Dictionary<MessageStateId, FlowMeterOne> flowMeterOneValueAccordingToTheStateMachine;

    // Load cell
    private LoadCellOne loadCellOne;

    // Blood blood detector
    private BloodDetector bloodDetector;

    private Dictionary<MessageStateId, BloodDetector> bloodDetectorValueAccordingToTheStateMachine;

    //Ballon
    private Balloon balloon;

    //Catheter
    private Catheter catheter;

    // PIDs
    private PatientMicroControllerPID patientMicroControllerPID;

    private CentralMicroControllerPID centralMicroControllerPID;

    private Tank tank;

    private ServiceDevices serviceDevices;

    // Regulators
    private PatientMicroControllerBalloonPressureRegulator patientMicroControllerBalloonPressureRegulator;

    private CentralMicroControllerFlowAndPressureRegulator centralMicroControllerFlowAndPressureRegulator;

    private Dictionary<MessageStateId, LoadCellOne> loadCellOneValueAccordingToTheStateMachine;

    //Pressure Transducer Event Args
    private PressureTransducerEventArgs _PressureTransducerEventArgs;

    private PressureTransducerEventArgs _PatientPressureTransducerEventArgs;

    // Thermocouple Event Args
    private ThermocoupleEventArgs _ThermocoupleEvent;

    private ThermocoupleEventArgs _TemperatureSensorEvent;

    // Pressure Switch Event Args
    private PressureSwitchEventArgs _PressureSwitchEvent;

    // Flow meter event arg
    private FlowMeterEventArgs FlowMeterOneEvent;

    // Load cell event arg
    private LoadCellEventArgs LoadCellOneEvent;

    //Blood Detector Event arg
    private BloodDetectorEventArgs BloodDetectorOneEventArgs;

    // Register Event
    private RegisterValuesEventArgs _RegisterValuesEvent;

    private RegisterValuesEventArgs _RegisterTwoValuesEvent;

    private EcgEventArgs _EcgEventArgs;

    private RemoteControlMembraneSwitchStateEventArgs _RemoteControlMembraneSwitchStateEventArgs;

    private BloodPressureSensorEventArgs _BloodPressureSensorEventArgs;

    private ProbeEventArgs _ProbeFirstGroupsensorsEventArgs;

    private ProbeEventArgs _ProbeSecondGroupsensorsEventArgs;

    //Event handler
    public event EventHandler<PressureTransducerEventArgs> pressureTransducerEvent;

    public event EventHandler<ThermocoupleEventArgs> thermocoupleEvent;

    public event EventHandler<PressureSwitchEventArgs> pressureSwitchEvent;

    public event EventHandler<FlowMeterEventArgs> flowMeterEvent;

    public event EventHandler<LoadCellEventArgs> loadCellEvent;

    public event EventHandler<BloodDetectorEventArgs> bloodDetectorEvent;

    public event EventHandler<RegisterValuesEventArgs> registerEvent;

    public event EventHandler<EcgEventArgs> ecgEventArgs;

    public event EventHandler<RemoteControlMembraneSwitchStateEventArgs> remoteControlMembraneSwitchStateEventArgs;

    public event EventHandler<BloodPressureSensorEventArgs> bloodPressureSensorStateEventArgs;

    public event EventHandler<ProbeEventArgs> probeEventArgs;

    public event EventHandler<RegisterValuesEventArgs> canTwoRegisterEvent;

    private uint activateLevel = 1;
    private uint deactivateLevel = 0;

    private uint StopGPIOID = 0;
    private uint WatchdogResetGPIOID = 1;
    private uint SystemResetGPIOID = 2;
    private uint FailResetGPIOID = 3;
    private uint InjectionGPIOID = 4;
    private uint AblateGPIOID = 5;
    private uint VacuumGPIOID = 6;
    private uint ChangeTankGPIOID = 7;

    //private Thread heartbeatThread;
    private Thread wakeUpbeatThread; // Used for can 1 and can 2

    #region HeartbeatStatus

    private bool gUIIsReady = false;
    private bool gUIInMaintenanceMode = false;
    private bool noErrorReportMode = false;
    private bool gUIInTestMode = false;
    private bool enableOrDisablePIDManualMode = false;
    private bool enableOrDisablePressureFlowMode = false;
    private bool deflateAfterThaw = false;
    private bool isConsoleInAblationState = false;
    private bool askForVitalParameters = false;
    private bool enableFastInflationMode = true;
    private bool isUsingAudioAlert = false;
    private bool isUsingAudioAlertMute = false;
    private bool isUsingAutoPlayback = true;

    #endregion HeartbeatStatus

    #region HBV

    uint _gUIIsReady = 0;
    uint _gUIInMaintenanceMode = 0;
    uint _noErrorReportMode = 0;
    uint _gUIInTestMode = 0;
    uint _enabaleEnhancedAudio = 0;
    uint _nOT_USED_32 = 0;
    uint _nOT_USED_64 = 0;
    uint _nOT_USED_128 = 0;

    uint _enableOrDisablePIDManualMode = 0;
    uint _enableOrDisablePressureFlowMode = 0;
    uint _deflateAfterThaw = 0;
    uint _enableFastInflationMode = 0;
    uint _purgeTheConsole = 0;
    uint _deactivateFeatuers = 0;
    uint _lockTheFootSwitch = 0;
    uint _diaphragmAudio = 0;


    uint _consoleInAblationState = 0;
    uint _vitalParameters = 0;

    #endregion

    uint remoteControlSystemStateId = 25;

    /// <summary>
    /// Creates the Console class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Machine(ICanBusCommunication canBusCommunication_, IGeneralPurposeInputOutput generalPurposeInputOutput_)
    {
      canBusCommunication = canBusCommunication_;
      generalPurposeInputOutput = generalPurposeInputOutput_;

      // Pressure Transducer Init
      pressureTransducerOne = new PressureTransducerOne();
      pressureTransducerTwo = new PressureTransducerTwo();
      pressureTransducerThree = new PressureTransducerThree();
      pressureTransducerFour = new PressureTransducerFour();
      listOfPressureTransducer = new List<IPressureTransducer>();

      listOfPressureTransducer.Add(pressureTransducerOne);
      listOfPressureTransducer.Add(pressureTransducerTwo);
      listOfPressureTransducer.Add(pressureTransducerThree);
      listOfPressureTransducer.Add(pressureTransducerFour);

      patientPressureTransducerOne = new PatientPressureTransducerOne();
      patientPressureTransducerTwo = new PatientPressureTransducerTwo();
      listOfPatientPressureTransducer = new List<IPressureTransducer>();

      listOfPatientPressureTransducer.Add(patientPressureTransducerOne);
      listOfPatientPressureTransducer.Add(patientPressureTransducerTwo);

      _PressureTransducerEventArgs = new PressureTransducerEventArgs(listOfPressureTransducer);
      _PressureTransducerEventArgs.Type = PressureTransducerEventArgs.PressureType.TP;

      _PatientPressureTransducerEventArgs = new PressureTransducerEventArgs(listOfPatientPressureTransducer);
      _PatientPressureTransducerEventArgs.Type = PressureTransducerEventArgs.PressureType.CP;

      // Thermocouple and temperature sensor Iint
      thermocoupleOne = new ThermocoupleOne();
      thermocoupleTwo = new ThermocoupleTwo();

      listOfThermocouple = new List<IThermocouple>();

      listOfThermocouple.Add(thermocoupleOne);
      listOfThermocouple.Add(thermocoupleTwo);

      temperatureSensorOne = new TemperatureSensorOne();
      listOfTemperatureSensor = new List<IThermocouple>();
      listOfTemperatureSensor.Add(temperatureSensorOne);

      _ThermocoupleEvent = new ThermocoupleEventArgs(listOfThermocouple);
      _ThermocoupleEvent.Type = ThermocoupleEventArgs.ThermocoupleType.TC;

      _TemperatureSensorEvent = new ThermocoupleEventArgs(listOfTemperatureSensor);
      _TemperatureSensorEvent.Type = ThermocoupleEventArgs.ThermocoupleType.TS;

      // Pressusre switch init
      pressureSwitchOne = new PressureSwitchOne();
      pressureSwitchTwo = new PressureSwitchTwo();

      listOfPressureSwitch = new List<IPressureSwitch>();

      listOfPressureSwitch.Add(pressureSwitchOne);
      listOfPressureSwitch.Add(pressureSwitchTwo);

      _PressureSwitchEvent = new PressureSwitchEventArgs(listOfPressureSwitch);

      // Flow meter init
      flowMeterOne = new FlowMeter();
      FlowMeterOneEvent = new FlowMeterEventArgs(flowMeterOne);

      // Load cell Init
      loadCellOne = new LoadCellOne();
      LoadCellOneEvent = new LoadCellEventArgs(loadCellOne);

      // Blood detector one Init
      bloodDetector = new BloodDetector();
      BloodDetectorOneEventArgs = new BloodDetectorEventArgs(bloodDetector);

      //Ballon Init
      Balloon = new Balloon();

      //Catheter Init
      Catheter = new Catheter();

      // PID Init
      patientMicroControllerPID = new PatientMicroControllerPID();

      centralMicroControllerPID = new CentralMicroControllerPID();

      //Tank
      Tank = new Tank();

      ServiceDevices = new ServiceDevices();

      // Regulators
      patientMicroControllerBalloonPressureRegulator = new PatientMicroControllerBalloonPressureRegulator();
      centralMicroControllerFlowAndPressureRegulator = new CentralMicroControllerFlowAndPressureRegulator();

      // Registers Init
      _RegisterValuesEvent = new RegisterValuesEventArgs();

      _RegisterTwoValuesEvent = new RegisterValuesEventArgs();

      // Ecg Init
      _EcgEventArgs = new EcgEventArgs();

      //Remote Control Membrane Switch State Event Args Init
      _RemoteControlMembraneSwitchStateEventArgs = new RemoteControlMembraneSwitchStateEventArgs();

      _BloodPressureSensorEventArgs = new BloodPressureSensorEventArgs();

      _ProbeFirstGroupsensorsEventArgs = new ProbeEventArgs();

      _ProbeSecondGroupsensorsEventArgs = new ProbeEventArgs();

      // Value initialization  according to the state machime
      IntializeSystemDatas();

      // Initialize Outputs
      InitializeOutputs();

      //Start application Heart beat
      //GUIIsReady = true;
      //heartbeatThread = new Thread(new ThreadStart(StartHeartbeatThread));
      //heartbeatThread.Start();

      wakeUpbeatThread = new Thread(new ThreadStart(StartWakeUpbeatThread));
      wakeUpbeatThread.Start();

      CanBusCommunication.MessageReceivedOne += new EventHandler<CanBusEventArgs>(canBusCommunication_MessageReceived);
      ConnectTheCanTwo();

    }

    /// <summary>
    /// Handles pressure changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain pressure transducer event data</param>
    protected virtual void OnPressureChanged(object sender, PressureTransducerEventArgs e)
    {
      pressureTransducerEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the thermocouple temperature changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain thermocouple event data</param>
    protected virtual void OnThermocoupleTemperatureChanged(object sender, ThermocoupleEventArgs e)
    {
      thermocoupleEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles pressure switch changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain pressure switch event data</param>
    protected virtual void OnPressureSwitchChanged(object sender, PressureSwitchEventArgs e)
    {
      pressureSwitchEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the flow changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain flow meter event data</param>
    protected virtual void OnFlowChanged(object sender, FlowMeterEventArgs e)
    {
      flowMeterEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the load changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain load cell event data</param>
    protected virtual void OnLoadChanged(object sender, LoadCellEventArgs e)
    {
      loadCellEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the blood detector changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain blood detector event data</param>
    protected virtual void OnBloodDetectorChanged(object sender, BloodDetectorEventArgs e)
    {
      bloodDetectorEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the register value changed event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain register values event data</param>
    protected virtual void OnRegisterValueChanged(object sender, RegisterValuesEventArgs e)
    {
      registerEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Handles the ecg value changed event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain ecg event data</param>
    protected virtual void OnEcgValueChanged(object sender, EcgEventArgs e)
    {
      ecgEventArgs?.Invoke(sender, e);
    }

    protected virtual void OnMembraneSwitchStateValueChanged(object sender, RemoteControlMembraneSwitchStateEventArgs e)
    {
      remoteControlMembraneSwitchStateEventArgs?.Invoke(sender, e);
    }

    protected virtual void BloodPressureSensorConnectionValueChanged(object sender, BloodPressureSensorEventArgs e)
    {
      bloodPressureSensorStateEventArgs?.Invoke(sender, e);
    }

    protected virtual void ProbeSensorConnectionValueChanged(object sender, ProbeEventArgs e)
    {
      probeEventArgs?.Invoke(sender, e);
    }


    /// <summary>
    /// Handles the CAN 2 register value changed
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain register values event data</param>
    protected virtual void OnCanTwoRegisterValueChanged(object sender, RegisterValuesEventArgs e)
    {
      canTwoRegisterEvent?.Invoke(sender, e);
    }

    /// <summary>
    /// Gets or sets the CAN bus communication
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ICanBusCommunication CanBusCommunication
    {
      get
      {
        return canBusCommunication;
      }

      set
      {
        canBusCommunication = value;
      }
    }

    /// <summary>
    /// Gets or sets the pressure transducer one value according to the state machine event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureTransducerOne> PressureTransducerOneValueAccordingToTheStateMachine
    {
      get
      {
        return pressureTransducerOneValueAccordingToTheStateMachine;
      }

      set
      {
        pressureTransducerOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets the pressure transducer two value according to the state machine event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureTransducerTwo> PressureTransducerTwoValueAccordingToTheStateMachine
    {
      get
      {
        return pressureTransducerTwoValueAccordingToTheStateMachine;
      }

      set
      {
        pressureTransducerTwoValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets the pressure transducer three value according to the state machine event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureTransducerThree> PressureTransducerThreeValueAccordingToTheStateMachine
    {
      get
      {
        return pressureTransducerThreeValueAccordingToTheStateMachine;
      }

      set
      {
        pressureTransducerThreeValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets the pressure transducer four value according to the state machine event
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureTransducerFour> PressureTransducerFourValueAccordingToTheStateMachine
    {
      get
      {
        return pressureTransducerFourValueAccordingToTheStateMachine;
      }

      set
      {
        pressureTransducerFourValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets temperature sensor one value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, TemperatureSensorOne> TemperatureSensorOneValueAccordingToTheStateMachine
    {
      get
      {
        return temperatureSensorOneValueAccordingToTheStateMachine;
      }

      set
      {
        temperatureSensorOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets flow meter one value according to the state machine
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, FlowMeterOne> FlowMeterOneValueAccordingToTheStateMachine
    {
      get
      {
        return flowMeterOneValueAccordingToTheStateMachine;
      }

      set
      {
        flowMeterOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets pressure switch one value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureSwitchOne> PressureSwitchOneValueAccordingToTheStateMachine
    {
      get
      {
        return pressureSwitchOneValueAccordingToTheStateMachine;
      }

      set
      {
        pressureSwitchOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets pressure switch two value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PressureSwitchTwo> PressureSwitchTwoValueAccordingToTheStateMachine
    {
      get
      {
        return pressureSwitchTwoValueAccordingToTheStateMachine;
      }

      set
      {
        pressureSwitchTwoValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets load cell one value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, LoadCellOne> LoadCellOneValueAccordingToTheStateMachine
    {
      get
      {
        return loadCellOneValueAccordingToTheStateMachine;
      }

      set
      {
        loadCellOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets general purpose IO
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public IGeneralPurposeInputOutput GeneralPurposeInputOutput
    {
      get
      {
        return generalPurposeInputOutput;
      }

      set
      {
        generalPurposeInputOutput = value;
      }
    }

    /// <summary>
    /// Gets or sets patient pressure transducer one value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PatientPressureTransducerOne> PatientPressureTransducerOneValueAccordingToTheStateMachine
    {
      get
      {
        return patientPressureTransducerOneValueAccordingToTheStateMachine;
      }

      set
      {
        patientPressureTransducerOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets patient pressure transducer two value according to the state machine
    ///. Safety classification: Non injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PatientPressureTransducerTwo> PatientPressureTransducerTwoValueAccordingToTheStateMachine
    {
      get
      {
        return patientPressureTransducerTwoValueAccordingToTheStateMachine;
      }

      set
      {
        patientPressureTransducerTwoValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets thermocouple one value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, ThermocoupleOne> ThermocoupleOneValueAccordingToTheStateMachine
    {
      get
      {
        return thermocoupleOneValueAccordingToTheStateMachine;
      }

      set
      {
        thermocoupleOneValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets injection flow value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, InjectionFlow> InjectionFlowValueAccordingToTheStateMachine
    {
      get
      {
        return injectionFlowValueAccordingToTheStateMachine;
      }

      set
      {
        injectionFlowValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets patient microController PID value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PatientMicroControllerPID> PatientMicroControllerPIDValueAccordingToTheStateMachine
    {
      get
      {
        return patientMicroControllerPIDValueAccordingToTheStateMachine;
      }

      set
      {
        patientMicroControllerPIDValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets central microController PID value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, CentralMicroControllerPID> CentralMicroControllerPIDValueAccordingToTheStateMachine
    {
      get
      {
        return centralMicroControllerPIDValueAccordingToTheStateMachine;
      }

      set
      {
        centralMicroControllerPIDValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets balloon
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Balloon Balloon
    {
      get
      {
        return balloon;
      }

      set
      {
        balloon = value;
      }
    }

    /// <summary>
    /// Gets or sets catheter
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Catheter Catheter
    {
      get
      {
        return catheter;
      }

      set
      {
        catheter = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the GUI is ready
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool GUIIsReady
    {
      get
      {
        return gUIIsReady;
      }

      set
      {
        gUIIsReady = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the GUI is in maintenance mode
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool GUIInMaintenanceMode
    {
      get
      {
        return gUIInMaintenanceMode;
      }

      set
      {
        gUIInMaintenanceMode = value;
      }
    }

    /// <summary>
    /// Gets or sets whether we are in no error report mode
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool NoErrorReportMode
    {
      get
      {
        return noErrorReportMode;
      }

      set
      {
        noErrorReportMode = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the GUI is in test mode
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool GUIInTestMode
    {
      get
      {
        return gUIInTestMode;
      }

      set
      {
        gUIInTestMode = value;
      }
    }

    /// <summary>
    /// Gets or sets the injection pressure value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, InjectionPressure> InjectionPressureValueAccordingToTheStateMachine
    {
      get
      {
        return injectionPressureValueAccordingToTheStateMachine;
      }

      set
      {
        injectionPressureValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets the low injection Flow Value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, InjectionFlow> LowInjectionFlowValueAccordingToTheStateMachine
    {
      get
      {
        return lowInjectionFlowValueAccordingToTheStateMachine;
      }

      set
      {
        lowInjectionFlowValueAccordingToTheStateMachine = value;
      }
    }


    /// <summary>
    /// Gets or sets the patient microController balloon pressure regulator value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, PatientMicroControllerBalloonPressureRegulator> PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine
    {
      get
      {
        return patientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine;
      }

      set
      {
        patientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets the central microController flow and pressure regulator value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator> CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine
    {
      get
      {
        return centralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine;
      }

      set
      {
        centralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the PID manual mode is enabled
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnableOrDisablePIDManualMode
    {
      get
      {
        return enableOrDisablePIDManualMode;
      }

      set
      {
        enableOrDisablePIDManualMode = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the pressure flow mode is enabled
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnableOrDisablePressureFlowMode
    {
      get
      {
        return enableOrDisablePressureFlowMode;
      }

      set
      {
        enableOrDisablePressureFlowMode = value;
      }
    }

    /// <summary>
    /// Gets or sets whether deflate after thaw is enabled
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DeflateAfterThaw
    {
      get
      {
        return deflateAfterThaw;
      }

      set
      {
        deflateAfterThaw = value;
      }
    }

    /// <summary>
    /// Gets or sets whether the console is in ablation state
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsConsoleInAblationState
    {
      get
      {
        return isConsoleInAblationState;
      }

      set
      {
        if (value != isConsoleInAblationState)
          isConsoleInAblationState = value;
      }
    }

    /// <summary>
    /// Gets or sets tank
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Tank Tank
    {
      get
      {
        return tank;
      }

      set
      {
        tank = value;
      }
    }

    /// <summary>
    /// Gets or sets the ten pound tank metal weight
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TENN_POUND_TANK_METAL_WEIGHT
    {
      get
      {
        return tENN_POUND_TANK_METAL_WEIGHT;
      }
    }

    /// <summary>
    /// Gets or sets the fifteen pound tank metal weight
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double FIFTEEN_POUND_TANK_METAL_WEIGHT
    {
      get
      {
        return fIFTEEN_POUND_TANK_METAL_WEIGHT;
      }
    }

    /// <summary>
    /// Gets or sets the isCanTwoAttached boolean value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsCanTwoAttached
    {
      get
      {
        return isCanTwoAttached;
      }

      set
      {
        isCanTwoAttached = value;
      }
    }

    /// <summary>
    /// Gets or sets askForVitalParameters boolean value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool AskForVitalParameters
    {
      get
      {
        return askForVitalParameters;
      }

      set
      {
        askForVitalParameters = value;
      }
    }

    /// <summary>
    /// Gets or sets inflation mode
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnableFastInflationMode
    {
      get => enableFastInflationMode;
      set => enableFastInflationMode = value;
    }

    /// <summary>
    /// Gets or sets if the GUI is using audio alert
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAudioAlert
    {
      get
      {
        return isUsingAudioAlert;
      }

      set
      {
        isUsingAudioAlert = value;
      }
    }



    /// <summary>
    /// Gets or sets if the GUI is using auto playback
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAutoPlayback
    {
      get
      {
        return isUsingAutoPlayback;
      }

      set
      {
        isUsingAutoPlayback = value;
      }
    }


    /// <summary>
    /// Gets or sets if the GUI is using audio alert
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingAudioAlertMute
    {
      get
      {
        return isUsingAudioAlertMute;
      }

      set
      {
        isUsingAudioAlertMute = value;
      }
    }




    /// <summary>
    /// Gets or sets the service devices
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public ServiceDevices ServiceDevices { get => serviceDevices; set => serviceDevices = value; }

    /// <summary>
    /// Gets or sets the foot switch state.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool LockTheFootSwitch { get; set; } = true;

    /// <summary>
    /// Gets or sets the purge console mode
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool PurgeTheConsole { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the features are deactivated
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EnableDefalteAfterThaw { get; set; } = false;


    /// <summary>
    /// Gets or sets whether the features are deactivated
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingBloodPressureSensor { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the features are deactivated
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemUsingLowFlow { get; set; } = false;


    /// <summary>
    /// Gets or sets whether the featuers are deactivated
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DeactivateFeatuers { get; set; } = false;

    /// <summary>
    /// Gets or sets the Dms detection threshold
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double DmsDetectionThreshold { get; set; } = 0.003;

    /// <summary>
    /// Gets or sets a value indicating whether the diaphragm and esophagus audio alerts are activated.
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool ActivateDiaphragmAndEsophagusAudioAlerts { get; set; } = false;

    public bool EnabaleEnhancedAudio { get; set; } = false;
    public bool NOT_USED_32 { get; set; } = false;
    public bool NOT_USED_64 { get; set; } = false;
    public bool NOT_USED_128 { get; set; } = false;

    /// <summary>
    /// Gets or sets the patient microController cryoBalloon configuration value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, CryoBalloonConfiguration> PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine
    {
      get
      {
        return patientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine;
      }

      set
      {
        patientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine = value;
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the heart beat is activated
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool HeartbeatActivated
    {
      get => heartbeatActivated;
      set => heartbeatActivated = value;
    }

    /// <summary>
    /// Gets or sets the blood detector value according to the state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Dictionary<MessageStateId, BloodDetector> BloodDetectorValueAccordingToTheStateMachine
    {
      get
      {
        return bloodDetectorValueAccordingToTheStateMachine;
      }

      set
      {
        bloodDetectorValueAccordingToTheStateMachine = value;
      }
    }


    /// <summary>
    /// Intializes system datas
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void IntializeSystemDatas()
    {
      #region Injection Flow

      // Target Injection Flow: we have to set default value 100 it is only a number
      InjectionFlowValueAccordingToTheStateMachine = new Dictionary<MessageStateId, InjectionFlow>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new   InjectionFlow{TargetInjectionFlow = 100} },
                {MessageStateId.CAN_ID_STATE_READY, new InjectionFlow { TargetInjectionFlow = 100 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new InjectionFlow { TargetInjectionFlow = 100} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new InjectionFlow { TargetInjectionFlow = 100 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new InjectionFlow { TargetInjectionFlow = 100} },
                {MessageStateId.CAN_ID_STATE_THAWING, new InjectionFlow { TargetInjectionFlow = 100} },
            };

      #endregion Injection Flow

      InjectionPressureValueAccordingToTheStateMachine = new Dictionary<MessageStateId, InjectionPressure>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new   InjectionPressure{TargetInjectionPressure = 100} },
                {MessageStateId.CAN_ID_STATE_READY, new InjectionPressure { TargetInjectionPressure = 100 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new InjectionPressure { TargetInjectionPressure = 100} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new InjectionPressure { TargetInjectionPressure = 100 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new InjectionPressure { TargetInjectionPressure = 100} },
                {MessageStateId.CAN_ID_STATE_THAWING, new InjectionPressure { TargetInjectionPressure = 100} },
            };

      #region Pressure Transducers

      //PT1
      PressureTransducerOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureTransducerOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,  new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 }},
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 }},
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 }},
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureTransducerOne { TankPressureLow =600,  PressureThresholdHighLimit=800,  TankPressureTooHigh =1000, PressureLowRangeLimit=50, PressureHighRangeLimit=1200 }},
            };

      //PT2
      PressureTransducerTwoValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureTransducerTwo>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PressureTransducerTwo  { PressureThresholdHighLimit=20,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureTransducerTwo { PressureThresholdHighLimit=20,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureTransducerTwo { PressureThresholdHighLimit=100,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureTransducerTwo { PressureThresholdHighLimit=650,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureTransducerTwo { PressureThresholdHighLimit=650,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureTransducerTwo { PressureThresholdHighLimit=20,  PressureLowRangeLimit=int.MinValue, PressureHighRangeLimit=800 } },
            };

      //PT3
      PressureTransducerThreeValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureTransducerThree>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PressureTransducerThree  { PressureThresholdHighLimit=int.MaxValue,  PressureLowRangeLimit=1, PressureHighRangeLimit=29 } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureTransducerThree { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=29} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureTransducerThree { PressureThresholdHighLimit=22,  PressureLowRangeLimit=1, PressureHighRangeLimit=29 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureTransducerThree { PressureThresholdHighLimit=22,  PressureLowRangeLimit=1, PressureHighRangeLimit=29 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureTransducerThree { PressureThresholdHighLimit=6,  PressureLowRangeLimit=1, PressureHighRangeLimit=29 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureTransducerThree { PressureThresholdHighLimit=22,  PressureLowRangeLimit=1, PressureHighRangeLimit=29 } },
            };

      //PT4
      PressureTransducerFourValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureTransducerFour>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PressureTransducerFour  { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20 } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureTransducerFour { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureTransducerFour { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureTransducerFour { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureTransducerFour { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureTransducerFour { PressureThresholdHighLimit=5,  PressureLowRangeLimit=1, PressureHighRangeLimit=20 } },
            };

      #endregion Pressure Transducers

      // TS1
      TemperatureSensorOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, TemperatureSensorOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new TemperatureSensorOne  { TemperatureThresholdHighLimit =int.MaxValue ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
                {MessageStateId.CAN_ID_STATE_READY, new TemperatureSensorOne { TemperatureThresholdHighLimit =-30 ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new TemperatureSensorOne { TemperatureThresholdHighLimit =-30 ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new TemperatureSensorOne { TemperatureThresholdHighLimit =-30 ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new TemperatureSensorOne { TemperatureThresholdHighLimit =-30 ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new TemperatureSensorOne { TemperatureThresholdHighLimit =-30 ,  TemperatureLowRangeLimit = -60, TemperatureHighRangeLimit = 40 } },
            };

      //FM1
      FlowMeterOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, FlowMeterOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new FlowMeterOne  { FlowMeterThresholLowlimit = int.MinValue ,  FlowMeterThresholHighlimit = 100, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
                {MessageStateId.CAN_ID_STATE_READY, new FlowMeterOne { FlowMeterThresholLowlimit = int.MinValue ,  FlowMeterThresholHighlimit = 100, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new FlowMeterOne { FlowMeterThresholLowlimit = int.MinValue ,  FlowMeterThresholHighlimit = int.MaxValue, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new FlowMeterOne { FlowMeterThresholLowlimit = int.MinValue ,  FlowMeterThresholHighlimit = 7000, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new FlowMeterOne { FlowMeterThresholLowlimit = 4000 ,  FlowMeterThresholHighlimit = 8000, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new FlowMeterOne { FlowMeterThresholLowlimit = int.MinValue ,  FlowMeterThresholHighlimit = int.MaxValue, FlowMeterLowRangeLimit = int.MinValue, FlowMeterHighRangelimit = 10000 } },
            };

      //PS1
      PressureSwitchOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureSwitchOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PressureSwitchOne  { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureSwitchOne { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureSwitchOne { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureSwitchOne { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureSwitchOne { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureSwitchOne { PressureThresholdHighLimit = 100, PressureLowRangeLimit =  int.MinValue, PressureHighRangeLimit = int.MaxValue } },
            };

      //PS2
      PressureSwitchTwoValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PressureSwitchTwo>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PressureSwitchTwo  { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
                {MessageStateId.CAN_ID_STATE_READY, new PressureSwitchTwo { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PressureSwitchTwo { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PressureSwitchTwo { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PressureSwitchTwo { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
                {MessageStateId.CAN_ID_STATE_THAWING, new PressureSwitchTwo { PressureThresholdHighLimit = 7, PressureLowRangeLimit =  1, PressureHighRangeLimit = 20 } },
            };

      //LC1
      LoadCellOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, LoadCellOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new LoadCellOne  { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
                {MessageStateId.CAN_ID_STATE_READY, new LoadCellOne { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new LoadCellOne { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new LoadCellOne { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new LoadCellOne { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
                {MessageStateId.CAN_ID_STATE_THAWING, new LoadCellOne { LoadCellThresholdWarning = 4.5, LoadCellThresholdFail = 3.5, LoadCellLowRangeLimit = 12, LoadCellHighRangeLimit = 24} },
            };

      //CP1
      PatientPressureTransducerOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PatientPressureTransducerOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PatientPressureTransducerOne  { PressureThresholdHighLimit=int.MaxValue,  PressureLowRangeLimit=int.MaxValue, PressureHighRangeLimit=int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_READY, new PatientPressureTransducerOne { PressureThresholdHighLimit=int.MaxValue,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PatientPressureTransducerOne { PressureThresholdHighLimit=15,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PatientPressureTransducerOne { PressureThresholdHighLimit=15,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PatientPressureTransducerOne { PressureThresholdHighLimit=15,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17} },
                {MessageStateId.CAN_ID_STATE_THAWING, new PatientPressureTransducerOne { PressureThresholdHighLimit=15,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
            };

      //CP2
      PatientPressureTransducerTwoValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PatientPressureTransducerTwo>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PatientPressureTransducerTwo  { PressureThresholdHighLimit=int.MaxValue,  PressureLowRangeLimit=int.MaxValue, PressureHighRangeLimit=int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_READY, new PatientPressureTransducerTwo { PressureThresholdHighLimit=int.MaxValue,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PatientPressureTransducerTwo { PressureThresholdHighLimit=-2,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PatientPressureTransducerTwo { PressureThresholdHighLimit=-2,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PatientPressureTransducerTwo { PressureThresholdHighLimit=-2,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17} },
                {MessageStateId.CAN_ID_STATE_THAWING, new PatientPressureTransducerTwo { PressureThresholdHighLimit=-2,  PressureLowRangeLimit=-17, PressureHighRangeLimit=17 } },
            };

      // TC1 ThermocoupleOneValueAccordingToTheStateMachine
      ThermocoupleOneValueAccordingToTheStateMachine = new Dictionary<MessageStateId, ThermocoupleOne>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new ThermocoupleOne  {ThawingTemperature = int.MaxValue} },
                {MessageStateId.CAN_ID_STATE_READY, new ThermocoupleOne { ThawingTemperature = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new ThermocoupleOne { ThawingTemperature = int.MaxValue} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new ThermocoupleOne { ThawingTemperature = int.MaxValue } },
                {MessageStateId.CAN_ID_STATE_ABLATION, new ThermocoupleOne { ThawingTemperature = int.MaxValue} },
                {MessageStateId.CAN_ID_STATE_THAWING, new ThermocoupleOne { ThawingTemperature = 20} },
            };

      #region Patient Micro Controller PID

      PatientMicroControllerPIDValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PatientMicroControllerPID>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PatientMicroControllerPID  {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_READY, new PatientMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PatientMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PatientMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PatientMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_THAWING, new PatientMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
            };

      //Central MicroController PID Value
      CentralMicroControllerPIDValueAccordingToTheStateMachine = new Dictionary<MessageStateId, CentralMicroControllerPID>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerPID  {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
                {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerPID {  PGain= 14, IGain = 13, DGain = 12, Offset = 24} },
            };

      #endregion Patient Micro Controller PID

      #region Flow and pressure Regulator

      PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine = new Dictionary<MessageStateId, PatientMicroControllerBalloonPressureRegulator>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
                {MessageStateId.CAN_ID_STATE_READY, new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
                {MessageStateId.CAN_ID_STATE_THAWING, new PatientMicroControllerBalloonPressureRegulator {  TargetBalloonPressure =17} },
            };

      CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
                {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
                {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 5000, TargetInjectionPressure = 5000} },
            };

      #endregion Flow and pressure Regulator

      #region Cryo Ballon Configuration
      PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine = new Dictionary<MessageStateId, CryoBalloonConfiguration>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CryoBalloonConfiguration  {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24} },
                {MessageStateId.CAN_ID_STATE_READY, new CryoBalloonConfiguration {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CryoBalloonConfiguration {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CryoBalloonConfiguration {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CryoBalloonConfiguration {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24}},
                {MessageStateId.CAN_ID_STATE_THAWING, new CryoBalloonConfiguration {  RampUpTimeByStep= 14, PressureRampUpValue = 13, RampDownTimeByStep = 12, PressureRampDownValue = 24} },
            };

      #endregion


      #region Blood Detector

      BloodDetectorValueAccordingToTheStateMachine = new Dictionary<MessageStateId, BloodDetector>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
                {MessageStateId.CAN_ID_STATE_READY, new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
                {MessageStateId.CAN_ID_STATE_THAWING, new BloodDetector {  LowerBloodThreshold = 17, UpperBloodThreshold = 75} },
            };

      #endregion
    }

    /// <summary>
    /// Initializes outputs
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void InitializeOutputs()
    {
      XmlDocument xDoc = new XmlDocument();

      if (xDoc != null)
      {
        xDoc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Console.configuration.xml"));

        XmlNode GeneralInformationNode = xDoc.SelectSingleNode("GPIO");
        foreach (XmlNode node in GeneralInformationNode)
        {
          if (node.Name == "GPIO8")
          {
            ChangeTankGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO7")
          {
            VacuumGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO6")
          {
            AblateGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO5")
          {
            InjectionGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO4")
          {
            FailResetGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO3")
          {
            SystemResetGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO2")
          {
            WatchdogResetGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }

          if (node.Name == "GPIO1")
          {
            StopGPIOID = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
          }
        }
      }

      StopDisable();

      SystemResetDisable();

      FailResetDisable();

      InjectionDisable();

      VacuumDisable();

      AblateDisable();

      ChangeTankDisable();
    }

    /// <summary>
    /// Starts heart beat thread
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void SendHeartbeat()
    {

      try
      {
        Thread.Sleep(100);
        WatchdogResetEnable();

        Thread.Sleep(100);

        WatchdogResetDisable();
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// Starts wake up beat thread
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StartWakeUpbeatThread()
    {
      // AppTrace.Log("Starting WakeUp Beat thread.");
      while (true)
      {
        try
        {
          Thread.Sleep(250);
          WakeUpWDT();
          Thread.Sleep(250);
        }
        catch (Exception ex)
        {
          LogException(ex);
          throw;
        }
      }
    }

    /// <summary>
    /// Handles the CAN bus events
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Refrence to the sender</param>
    /// <param name="e">Represents the base class for classes that contain can bus event data</param>
    private void canBusCommunication_MessageReceived(object sender, CanBusEventArgs e)
    {
      try
      {
        int messageIdentifier = Convert.ToInt32(e.Id) & ((Int32)Mask.CAN_ID_ELEMENT_MASK | (Int32)Mask.CAN_ID_NODE_MASK | (Int32)Mask.CAN_ID_TYPE_MASK);

        switch ((CanBusMessageIdentifier)messageIdentifier)
        {
          #region Central Micro Controller: Read Values

          case CanBusMessageIdentifier.CentralMicroControllerPT1PT2PT3PT4Reading:
            OnPressureChanged(sender, _PressureTransducerEventArgs);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPS1PS2Reading:
            OnPressureSwitchChanged(sender, _PressureSwitchEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerFM1Reading:
            OnFlowChanged(sender, FlowMeterOneEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerTS1Reading:
            OnThermocoupleTemperatureChanged(sender, _TemperatureSensorEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerLC1Reading:
            OnLoadChanged(sender, LoadCellOneEvent);

            break;

          #endregion Central Micro Controller: Read Values

          #region Patient Micro Controller: Read Values

          case CanBusMessageIdentifier.PatientMicroControllerTC1TC2Reading:
            OnThermocoupleTemperatureChanged(sender, _ThermocoupleEvent);
            break;

          case CanBusMessageIdentifier.PatientMicroControllerCP1CP2TipReading:
            OnPressureChanged(sender, _PatientPressureTransducerEventArgs);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerCIMP1CIMP2Reading:
            OnBloodDetectorChanged(sender, BloodDetectorOneEventArgs);
            break;

          #endregion Patient Micro Controller: Read Values

          #region Central Micro Controller: Register Values

          case CanBusMessageIdentifier.CentralMicroControllerFirmwareVersion:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 8;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerCPLDErrorRegister:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 9;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerCPLDValveRegister:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 10;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerCPLDSystemRegister:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 11;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerSystemState:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 12;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerAblationTime:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 13;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerContinuousThawing:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 14;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerTargetInjectionFlow:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 15;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPIDParameter:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 16;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT1Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 17;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT1Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 18;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT2Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 19;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT2Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 20;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT3Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 21;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT3Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 22;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT4Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 23;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPT4Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 24;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerTSThreshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 25;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerTSRange:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 26;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerFM1Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 27;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerFM1Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 28;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPS1Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 29;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPS1Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 30;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPS2Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 31;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerPS2Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 32;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerLC1Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 33;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerLC1Range:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 34;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerSatutsAndErrorData:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 35;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CentralMicroControllerCPLDRegisterValvesState:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 36;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          #endregion Central Micro Controller: Register Values

          #region Patient Micro Controller: Register Values

          case CanBusMessageIdentifier.PatientMicroControllerFirmwareVersion:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 48;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerStatusAndErrorCode:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 49;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerCatheterExtenedCatheterSNAndLotNumber:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 5;
            LogInfo($"Console received Extended Catheter Message : ID = {messageIdentifier}; Data= [{string.Join(", ", e.Data.Select(b => $"0X{b:X2}"))}]");
            OnRegisterValueChanged(sender, _RegisterValuesEvent);
            break;

          case CanBusMessageIdentifier.PatientMicroControllerCatheterIDAndSerialNumberAndFirmwareVersion:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 50;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerCatheterExpirationDateLastUseDateNumberOfInjections:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 51;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerTargetBalloonPressure:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 52;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerCP1CP2Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 53;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerTCT1CT2Threshold:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 54;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerPIDParameter:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 55;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.CatheterFirmware:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 56;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerBallonSizeConfiguration:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 57;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          #endregion Patient Micro Controller: Register Values

          #region CentralMicroControllerLoadCellCalibration 

          case CanBusMessageIdentifier.CentralMicroControllerLoadCellCalibration:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterValuesEvent.ID = 62;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          #endregion CentralMicroControllerLoadCellCalibration

          #region Connection Box Micro Controller: Read Values

          case CanBusMessageIdentifier.BloodPressureSensorConnection:
            _BloodPressureSensorEventArgs.ID = 1;
            BloodPressureSensorConnectionValueChanged(sender, _BloodPressureSensorEventArgs);

            break;

          case CanBusMessageIdentifier.ProbeFirstGroupsensors:
            _ProbeFirstGroupsensorsEventArgs.ID = 5;
            ProbeSensorConnectionValueChanged(sender, _ProbeFirstGroupsensorsEventArgs);

            break;

          case CanBusMessageIdentifier.ProbeSecondGroupsensors:
            _ProbeSecondGroupsensorsEventArgs.ID = 6;
            ProbeSensorConnectionValueChanged(sender, _ProbeSecondGroupsensorsEventArgs);

            break;

          case CanBusMessageIdentifier.BloodPressureSensorData:
            _BloodPressureSensorEventArgs.ID = 7;
            BloodPressureSensorConnectionValueChanged(sender, _BloodPressureSensorEventArgs);

            break;

          case CanBusMessageIdentifier.EcgChannel1And2Channel3And4Channel5And6Channel7And8:
            _EcgEventArgs.ID = 8;
            OnEcgValueChanged(sender, _EcgEventArgs);
            break;

          case CanBusMessageIdentifier.EcgChannel9And10ChannelTipChannelAccelerometer:
            _EcgEventArgs.ID = 9;
            OnEcgValueChanged(sender, _EcgEventArgs);
            break;

          case CanBusMessageIdentifier.HighResolutionECGChannel:
            _EcgEventArgs.ID = 32;
            OnEcgValueChanged(sender, _EcgEventArgs);
            break;

          #endregion Connection Box Micro Controller: Read Values

          #region Remote Control
          case CanBusMessageIdentifier.MembraneSwitchState:
            _RemoteControlMembraneSwitchStateEventArgs.ID = 26;
            OnMembraneSwitchStateValueChanged(sender, _RemoteControlMembraneSwitchStateEventArgs);

            break;

          case CanBusMessageIdentifier.RemoteControlHeartbeat:
            _RemoteControlMembraneSwitchStateEventArgs.ID = 28;
            OnMembraneSwitchStateValueChanged(sender, _RemoteControlMembraneSwitchStateEventArgs);

            break;
          #endregion

          #region CAN2 RegisterValue

          case CanBusMessageIdentifier.RepeaterFirmwareAndICBFirmware:
            _RegisterTwoValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterTwoValuesEvent.ID = 11;
            OnCanTwoRegisterValueChanged(sender, _RegisterTwoValuesEvent);
            break;

          case CanBusMessageIdentifier.RemoteControlFirmware:
            _RegisterTwoValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterTwoValuesEvent.ID = 24;
            OnCanTwoRegisterValueChanged(sender, _RegisterTwoValuesEvent);
            break;

          #endregion CAN2 RegisterValue

          #region CMCU PMCU and RMCU RTR Boot Data 
          case CanBusMessageIdentifier.CentralMicroControllerRTRBootData:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 58;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerRTRBootData:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 58;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.RepeaterMicroControllerRTRBootData:
            _RegisterTwoValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterTwoValuesEvent.ID = 58;
            OnCanTwoRegisterValueChanged(sender, _RegisterTwoValuesEvent);

            break;


          case CanBusMessageIdentifier.CentralMicroControllerRTRBootDataStart:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 59;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerRTRBootDataStart:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 59;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);
            break;

          case CanBusMessageIdentifier.RepeaterMicroControllerRTRBootDataStart:
            _RegisterTwoValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterTwoValuesEvent.ID = 59;
            OnCanTwoRegisterValueChanged(sender, _RegisterTwoValuesEvent);
            break;

          case CanBusMessageIdentifier.CentralMicroControllerRTRBootDataInit:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.MainMicrocontroller;
            _RegisterValuesEvent.ID = 60;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.PatientMicroControllerRTRBootDataInit:
            _RegisterValuesEvent.Type = RegisterValuesEventArgs.RegisterType.PatientMicrocontroller;
            _RegisterValuesEvent.ID = 60;
            OnRegisterValueChanged(sender, _RegisterValuesEvent);

            break;

          case CanBusMessageIdentifier.RepeaterMicroControllerRTRBootDataInit:
            _RegisterTwoValuesEvent.Type = RegisterValuesEventArgs.RegisterType.ConnectionBox;
            _RegisterTwoValuesEvent.ID = 60;
            OnCanTwoRegisterValueChanged(sender, _RegisterTwoValuesEvent);

            break;
            #endregion
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        //throw;
      }
    }

    /// <summary>
    /// Calibrates component
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="ComponentId">Component Id</param>
    public void CalibrateComponent(CalibrationComponentANDCPLDRegister.CalibrationComponentId ComponentId, int calibrationFactor)
    {
      byte[] data = BuildDataBytes.ConvertCalibrationComponentDataToByte((int)ComponentId, calibrationFactor);

      // 0 is indicating the calibartion ID, i have to correct that do not hard code
      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.Calibration, data, false);
    }

    public void ReadCalibrateComponent()
    {
      byte[] data = new byte[8];
      Array.Clear(data, 0, 8);
      // 0 is indicating the calibartion ID, i have to correct that do not hard code
      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.Calibration, data, false);
    }

    /// <summary>
    /// Sets solenoid valves
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="svLevels">levels of solenoid </param>
    public void SetCPLDSVLevel(uint svLevels)
    {
      byte[] data = BuildDataBytes.ConvertCalibrationComponentDataToByte((int)svLevels);

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.CPLDValve, data, false);
    }

    /// <summary>
    /// Sets the audio level
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="audioLevel">audio level</param>
    public void SetAudioLevel(uint audioLevel)
    {
      byte[] data = BuildDataBytes.ConvertAudioComponentDataToByte((int)audioLevel);

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.AudioControl, data, false);
    }

    /// <summary>
    /// Wakes up the watchdog
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void WakeUpWDT()
    {
      if (HeartbeatActivated)
      {
        try
        {
          uint valuesCombinationTosend = 0;
          uint canTwoValuesCombinationTosend = 0;

          _gUIIsReady = (GUIIsReady == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.GUI_is_Ready : (uint)0;
          _gUIInMaintenanceMode = (GUIInMaintenanceMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.GUI_in_Maintenance_Mode : (uint)0;

          _noErrorReportMode = (NoErrorReportMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.No_Error_Report_Mode : (uint)0;

          _gUIInTestMode = (GUIInTestMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.GUI_In_Test_Mode : (uint)0;

          _enabaleEnhancedAudio = (EnabaleEnhancedAudio == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.Enhanced_Audio : (uint)0;
          _nOT_USED_32 = (NOT_USED_32 == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.NOT_USED_32 : (uint)0;
          _nOT_USED_64 = (NOT_USED_64 == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.NOT_USED_64 : (uint)0;

          _nOT_USED_128 = (NOT_USED_128 == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.NOT_USED_128 : (uint)0;

          _enableOrDisablePIDManualMode = (EnableOrDisablePIDManualMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.PID_MANUAL_MODE : (uint)0;

          _enableOrDisablePressureFlowMode = (EnableOrDisablePressureFlowMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.PRESSURE_FLOW_MODE : (uint)0;

          _deflateAfterThaw = (DeflateAfterThaw == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.AUTO_DEFLATION : (uint)0;

          _enableFastInflationMode = (EnableFastInflationMode == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.SLOW_FAST_INFLATION : (uint)0;

          _purgeTheConsole = (PurgeTheConsole == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.SYSTEM_PURGE : (uint)0;

          _deactivateFeatuers = (DeactivateFeatuers == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.DEACTIVATE_FEATURES : (uint)0;

          _lockTheFootSwitch = (LockTheFootSwitch == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.FOOT_SWITCH : (uint)0;

          _diaphragmAudio = (ActivateDiaphragmAndEsophagusAudioAlerts == true) ? (uint)CalibrationComponentANDCPLDRegister.HeartbeatStatus.DIAPHRAGM_ESOPHAGUS_AUDIO_ALERTS : (uint)0;

          valuesCombinationTosend = _gUIIsReady | _gUIInMaintenanceMode | _noErrorReportMode | _gUIInTestMode | _enabaleEnhancedAudio | _nOT_USED_32 | _nOT_USED_64 | _nOT_USED_128 |
                                    _enableOrDisablePIDManualMode | _enableOrDisablePressureFlowMode | _deflateAfterThaw | _enableFastInflationMode | _purgeTheConsole |
                                    _deactivateFeatuers | _lockTheFootSwitch | _diaphragmAudio;

          byte[] data = BuildDataBytes.ConvertHeartbeatStatusDataToByte((int)valuesCombinationTosend);

          CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.WDTWakeUpSignal, data, false);


          _consoleInAblationState = (IsConsoleInAblationState == true) ? (uint)CalibrationComponentANDCPLDRegister.CanTwoHeartbeatStatus.CONSOLE_IS_IN_ABLATION_STATE : (uint)0;
          _vitalParameters = (AskForVitalParameters == true) ? (uint)CalibrationComponentANDCPLDRegister.CanTwoHeartbeatStatus.VITAL_PARAMETERS : (uint)0;

          canTwoValuesCombinationTosend = _consoleInAblationState | _vitalParameters;

          byte[] canTwodata = BuildDataBytes.ConvertHeartbeatStatusDataToByte((int)canTwoValuesCombinationTosend, DmsDetectionThreshold);

          CanBusCommunication.SendDataToCanBusTwo((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANTWOMessage.ElementId.ConnectionBoxState, canTwodata, false);
        }
        catch (Exception ex)
        {
          LogException(ex);
          //throw;
        }
      }
    }

    /// <summary>
    /// Powers off the console message
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void PowerOffMessage()
    {
      GUIIsReady = false;
      byte[] data = new byte[8];
      Array.Clear(data, 0, 8);

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)MessageStateId.CAN_ID_STATE_IDLE << 8) | (uint)SBCCANOneMessage.ElementId.PowerOffSignal, data, false);
    }

    /// <summary>
    /// Sends RTR data
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">console state </param>
    /// <param name="Id">message ID</param>
    public void SendRemoteFrame(MessageStateId stateId, uint Id)
    {
      byte[] data = DataToSend(stateId, (int)Id);

      // here is the code that i am using before .  yong want an ansewr with same ID so change it. perhapese it will carte a problem when i want to ask data
      // my self when i want to read
      //CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) |Id, data, true);
      CanBusCommunication.SendDataToCanBus(Id, data, true);
    }

    /// <summary>
    /// Answers remote frame
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">console state </param>
    /// <param name="RTRId">RTR message id </param>
    /// <param name="localId"></param>
    /// <param name="isItAnsewringCatheterValidation">Is it ansewring catheter validation</param>
    /// <param name="iscatheterValid">Is catheter valid</param>
    public void AnswerForRemoteFrame(MessageStateId stateId, uint RTRId, uint localId, bool isItAnsewringCatheterValidation = false, bool iscatheterValid = false)
    {
      if (isItAnsewringCatheterValidation)
      {
        byte[] data = DataToSend(stateId, (int)localId, isItAnsewringCatheterValidation, iscatheterValid);

        // here is the code that i am using before .  yong want an ansewr with same ID so change it. perhapese it will carte a problem when i want to ask data
        // my self when i want to read
        //CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) |Id, data, true);
        CanBusCommunication.SendDataToCanBus(RTRId, data, false);
      }
      else
      {
        byte[] data = DataToSend(stateId, (int)localId);

        // here is the code that i am using before .  yong want an ansewr with same ID so change it. perhapese it will carte a problem when i want to ask data
        // my self when i want to read
        //CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) |Id, data, true);
        CanBusCommunication.SendDataToCanBus(RTRId, data, false);
      }
    }

    /// <summary>
    /// Answers remote frame
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">state ID</param>
    /// <param name="RTRId">RTR ID</param>
    /// <param name="data">message data</param>
    public void AnswerRemotFrameForBootLoader(MessageStateId stateId, uint RTRId, byte[] data)
    {
      CanBusCommunication.SendDataToCanBus(RTRId, data, false);
    }

    /// <summary>
    /// Writes from microController
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">Console state </param>
    /// <param name="Id">Message id </param>
    /// <id>SF-SDS-0071</id>
    public void WriteFromMicroController(MessageStateId stateId, int Id)
    {
      byte[] data = DataToSend(stateId, Id);

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | (uint)Id, data, false, true);
    }

    /// <summary>
    /// Sends boot message
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">state ID</param>
    /// <param name="Id">ID</param>
    /// <param name="data">message data</param>
    public void SendBootMessage(MessageStateId stateId, int Id, byte[] data)
    {

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | (uint)Id, data, false, true);
    }

    /// <summary>
    /// Answers RTR boot message
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">state ID</param>
    /// <param name="Id">ID</param>
    /// <param name="data">data</param>
    public void AnswerRTRBootMessage(uint stateId, int Id, byte[] data)
    {

      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | (stateId << 8) | (uint)Id, data, false, true);
    }

    #region Boot load for ICB and Reapeter

    /// <summary>
    /// Send boot message for ICB Or reapeter
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">state ID</param>
    /// <param name="Id">ID</param>
    /// <param name="data">data message</param>
    public void SendBootMessageForICBOrReapeter(MessageStateId stateId, int Id, byte[] data)
    {

      CanBusCommunication.SendDataToCanBusTwo((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | (uint)Id, data, false);
    }

    /// <summary>
    /// Answer RTR boot message for ICB or reapeter
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">state ID</param>
    /// <param name="Id">ID</param>
    /// <param name="data">data message</param>
    public void AnswerRTRBootMessageForICBOrReapeter(uint stateId, int Id, byte[] data)
    {

      CanBusCommunication.SendDataToCanBusTwo((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | (stateId << 8) | (uint)Id, data, false);
    }

    #endregion

    /// <summary>
    /// Reads from microcontroller
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">Console state</param>
    /// <param name="Id">Message id </param>
    /// <id>SF-SDS-0072</id>
    public void ReadFromMicroController(MessageStateId stateId, int Id)
    {
      byte[] data = new byte[8];
      Array.Clear(data, 0, 8);
      CanBusCommunication.SendDataToCanBus((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | (uint)Id, data, true, true);
    }

    /// <summary>
    /// Reads from microController on CAN 2
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">Console state</param>
    /// <param name="Id">Message id</param>
    public void ReadFromMicroControllerOnCanTwo(MessageStateId stateId, int Id)
    {
      byte[] data = new byte[8];
      Array.Clear(data, 0, 8);
      CanBusCommunication.SendDataToCanBusTwo((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | (uint)Id, data, true);
    }

    int compteur = 0;

    /// <summary>
    /// Builds data to send to the microcontroller
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="stateId">Console state</param>
    /// <param name="Id">Message id </param>
    /// <param name="isItAnsewringCatheterValidation">Is it answering catheter validation</param>
    /// <param name="iscatheterValid">Is catheter valid</param>
    /// <returns></returns>
    private byte[] DataToSend(MessageStateId stateId, int Id, bool isItAnsewringCatheterValidation = false, bool iscatheterValid = false)
    {
      byte[] data = new byte[8];

      MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;
      switch ((int)stateId)
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
      //PressureTransducerTwo xx = PressureTransducerTwoValueAccordingToTheStateMachine[stateId];
      switch (Id)
      {
        #region Central Micro Controller: Register Values

        case 8:

          break;

        case 9:

          break;

        case 10:

          break;

        case 11:

          break;

        case 12:

          break;

        case 13:

          break;

        case 14:

          break;

        case 15:
          data = BuildDataBytes.ConvertInjectionFlowDataToByte(CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid], CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid]);
          break;

        case 16:
          data = BuildDataBytes.ConvertPIDDataToByte(CentralMicroControllerPIDValueAccordingToTheStateMachine[mid]);
          break;

        case 17:

          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerOneValueAccordingToTheStateMachine[mid]);
          break;

        case 18:

          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerOneValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 19:
          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerTwoValueAccordingToTheStateMachine[mid]);

          break;

        case 20:
          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerTwoValueAccordingToTheStateMachine[mid], "Range");
          break;

        case 21:
          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerThreeValueAccordingToTheStateMachine[mid]);
          break;

        case 22:
          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerThreeValueAccordingToTheStateMachine[mid], "Range");
          break;

        case 23:

          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerFourValueAccordingToTheStateMachine[mid]);

          break;

        case 24:
          data = BuildDataBytes.ConvertPressureTransducerDataToBytes(PressureTransducerFourValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 25:
          data = BuildDataBytes.ConvertTemperatureSensorOneDataToBytes(TemperatureSensorOneValueAccordingToTheStateMachine[mid]);

          break;

        case 26:
          data = BuildDataBytes.ConvertTemperatureSensorOneDataToBytes(TemperatureSensorOneValueAccordingToTheStateMachine[mid], "Range");
          break;

        case 27:

          data = BuildDataBytes.ConvertFlowMeterOneDataToBytes(FlowMeterOneValueAccordingToTheStateMachine[mid]);

          break;

        case 28:
          data = BuildDataBytes.ConvertFlowMeterOneDataToBytes(FlowMeterOneValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 29:
          data = BuildDataBytes.ConvertPressureSwitchDataToBytes(pressureSwitchOneValueAccordingToTheStateMachine[mid]);

          break;

        case 30:

          data = BuildDataBytes.ConvertPressureSwitchDataToBytes(pressureSwitchOneValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 31:

          data = BuildDataBytes.ConvertPressureSwitchDataToBytes(pressureSwitchTwoValueAccordingToTheStateMachine[mid]);

          break;

        case 32:

          data = BuildDataBytes.ConvertPressureSwitchDataToBytes(pressureSwitchTwoValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 33:
          compteur++;

          data = BuildDataBytes.ConvertLoadCellOneDataToBytes(LoadCellOneValueAccordingToTheStateMachine[mid]);

          break;

        case 34:

          data = BuildDataBytes.ConvertLoadCellOneDataToBytes(LoadCellOneValueAccordingToTheStateMachine[mid], "Range");

          break;

        case 35:

          break;

        #endregion Central Micro Controller: Register Values

        #region Patient Micro Controller: Register Values

        case 48:
          // here we build the catheter firmware version. i suppose these version is read only

          break;

        case 49:
          //we are only reading value PMCU System Status/Error Code
          break;

        case 50:
          data = BuildDataBytes.ConvertCatheterValidationDataToByte(this.Catheter, iscatheterValid);
          //data = BuildDataBytes.ConvertCatheterIDSerialNumberExpirationDateDataToByte(this.Catheter);
          break;

        case 51:
          data = BuildDataBytes.ConvertCatheterLastUseDateNumberOfInjectionsDataToByte(this.Catheter);
          break;

        case 52:
          data = BuildDataBytes.ConvertBallonDataToByte(PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[mid]);
          break;

        case 53:

          //here the Naming is changed Threshold cPInner High,    cPOuter High,    cPTip High
          data = BuildDataBytes.ConvertPatientPressureTransducerOneDataToBytes(PatientPressureTransducerOneValueAccordingToTheStateMachine[mid]);

          break;

        case 54:
          data = BuildDataBytes.ConvertThermocoupleOneAndBloodDetectorDataToBytes(ThermocoupleOneValueAccordingToTheStateMachine[mid], BloodDetectorValueAccordingToTheStateMachine[mid]);

          break;

        case 55:

          data = BuildDataBytes.ConvertPIDDataToByte(PatientMicroControllerPIDValueAccordingToTheStateMachine[mid]);

          break;


        case 56:

          break;

        case 57:
          data = BuildDataBytes.ConvertBalloonSizeDataToByte(PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid]);

          break;

          #endregion Patient Micro Controller: Register Values
      }

      return data;
    }

    #region outpouts activation

    /// <summary>
    /// GUI stop message
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0073</id>
    public void StopEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(StopGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// GUI watchdog reset
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0074</id>
    public void WatchdogResetEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(WatchdogResetGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Reset the system
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0075</id>
    public void SystemResetEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(SystemResetGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Reset the failure
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0076</id>
    public void FailResetEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(FailResetGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Injection enable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0077</id>
    public void IinjectionEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(InjectionGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Vacuum enable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0078</id>
    public void VacuumEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(VacuumGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Ablate enable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0079</id>
    public void AblateEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(AblateGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Change tank enable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// /// <id>SF-SDS-0080</id>
    public void ChangeTankEnable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(ChangeTankGPIOID, 1, activateLevel);
    }

    #endregion outpouts activation

    #region outpouts deactivation

    /// <summary>
    /// Stop disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0081</id>
    public void StopDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(StopGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Watchdog reset disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0082</id>
    public void WatchdogResetDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(WatchdogResetGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// System reset disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0083</id>
    public void SystemResetDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(SystemResetGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Fail reset disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0084</id>

    public void FailResetDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(FailResetGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Injection disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0085</id>
    public void InjectionDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(InjectionGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Vacuum disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0086</id>
    public void VacuumDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(VacuumGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Ablate disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0087</id>
    public void AblateDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(AblateGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Change tank disable
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0088</id>
    public void ChangeTankDisable()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(ChangeTankGPIOID, 1, deactivateLevel);
    }

    #endregion outpouts deactivation

    #region Connect and Disconnect

    /// <summary>
    /// Disconnect Vacuum
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void Disconnect()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(InjectionGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(VacuumGPIOID, 1, deactivateLevel);
    }

    /// <summary>
    /// Connect vacuum
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void Connect()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(InjectionGPIOID, 1, activateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(VacuumGPIOID, 1, activateLevel);
    }

    /// <summary>
    /// Strat ablation
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void Start()
    {
      AblateEnable();
      Thread.Sleep(10);
      AblateDisable();
    }

    /// <summary>
    /// Stop  ablation
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void Stop()
    {
      StopEnable();
      Thread.Sleep(10);
      StopDisable();
    }

    /// <summary>
    /// Update GPIO Allow ablation status
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void AllowStartAblation(bool allow)
    {
      GeneralPurposeInputOutput.SetGPIOLevel(ChangeTankGPIOID, 1, allow ? deactivateLevel : activateLevel);
    } 

    /// <summary>
    /// Deactivate all general-purpose input output
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void DeactivateAllIOS()
    {
      this.GeneralPurposeInputOutput.SetGPIOLevel(StopGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(WatchdogResetGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(SystemResetGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(FailResetGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(InjectionGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(VacuumGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(AblateGPIOID, 1, deactivateLevel);
      this.GeneralPurposeInputOutput.SetGPIOLevel(ChangeTankGPIOID, 1, deactivateLevel);
    }

    #endregion Connect and Disconnect

    #region disconnect and connecte the  the can 2

    /// <summary>
    /// Connects the CAN two
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ConnectTheCanTwo()
    {
      if (!IsCanTwoAttached)
      {
        IsCanTwoAttached = true;
        CanBusCommunication.MessageReceivedTwo += new EventHandler<CanBusEventArgs>(canBusCommunication_MessageReceived);

      }
    }

    /// <summary>
    /// Disconnects the CAN two
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void DisconnectTheCanTwo()
    {
      if (IsCanTwoAttached)
      {
        IsCanTwoAttached = false;
        CanBusCommunication.MessageReceivedTwo -= new EventHandler<CanBusEventArgs>(canBusCommunication_MessageReceived);

      }
    }

    private bool isCanTwoAttached = false;

    private bool heartbeatActivated = true;

    #endregion

    public void SendStateToRemoteCotrol(MessageStateId stateId, byte[] data)
    {

      CanBusCommunication.SendDataToCanBusTwo((uint)CanBusMessageIdentifier.SingleBoardComputerRegisterValues | ((uint)stateId << 8) | remoteControlSystemStateId, data, false);
    }
  }
}