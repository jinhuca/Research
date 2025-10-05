using System.Collections.Generic;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.Validation;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class TSMessageProvider : MessageProviderBase
  {
    private static string TS_CONFIG_NODE_ID = "TSConfig";
    private static uint TS_MESSAGE_ID = 3; 
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    private static double DEFAULT_TS1 = 0;
    private static double DEFAULT_CMCUCJ = 20;
    private static double DEFAULT_TN2O = -30;
    private static double SPEED_RATIO = 0.1;

    private Timer _messageUpdateTimer;
    private TSMessageConfig _tsMessageConfig;
    private IDictionary<string, StateToTSValue> _tsStates = new Dictionary<string, StateToTSValue>();
    // current PS
    private double _ts1 = DEFAULT_TS1;
    private double _cmcucj = DEFAULT_CMCUCJ;
    private double _tn2o = DEFAULT_TN2O;

    private double _ts1Goal;
    private double _cmcucjGoal;
    private double _tn2oGoal;

    private byte[] _tsData = new byte[8];

    private TS1ThresholdValidation _ts1ThresholdValidation;
    public TSMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration, TS1ThresholdValidation tS1ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(TS_MESSAGE_ID);
      _ts1ThresholdValidation = tS1ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _tsMessageConfig = new TSMessageConfig();
      var loadConfig = _tsMessageConfig.Parse(GetConfigurationNode(TS_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_tsMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _tsMessageConfig.Interval);
        _tsStates = _tsMessageConfig.StateToTSMap;
        UpdateTSGoal(DEFAULT_STATE); 
        _messageUpdateTimer.Elapsed += PublishPSMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      UpdateTSGoal(message.State);
    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    private void UpdateTSGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string tslocation = ConvertStateNumberToString(stateNumber);
      _ts1Goal = _tsStates[tslocation].TS1;
      _cmcucjGoal = _tsStates[tslocation].CMCUCJ;
      _tn2oGoal = _tsStates[tslocation].TN2O;

    }
    private void BuildTS()
    {
      _ts1 = IncrementDecimal(_ts1, _ts1Goal, SPEED_RATIO);
      _cmcucj = IncrementDecimal(_cmcucj, _cmcucjGoal, SPEED_RATIO);
      _tn2o = IncrementDecimal(_tn2o, _tn2oGoal, SPEED_RATIO);
      _ts1ThresholdValidation.ValidateThresholds(_ts1, CurrentStateId);
      // convert to the correct format for CANBus
      var ts1 = (int)(_ts1 * 10);
      var cmcucj = (int)(_cmcucj * 10);
      var tn2o = (int)(_tn2o * 10);

      _tsData[0] = (byte)((ts1 >> 8) & 0xFF);
      _tsData[1] = (byte)(ts1 & 0xFF);
      _tsData[2] = (byte)((cmcucj >> 8) & 0xFF);
      _tsData[3] = (byte)(cmcucj & 0xFF);
      _tsData[4] = (byte)((tn2o >> 8) & 0xFF);
      _tsData[5] = (byte)(tn2o & 0xFF);
    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildTS();

      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, TS_MESSAGE_ID), Length = 6, Data = _tsData }
      };

      PublishCanBusMessage(message);
    }

  }
}
