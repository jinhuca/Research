using System.Collections.Generic;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class cIMPMessageProvider : MessageProviderBase
  {
    private static string cIMP_CONFIG_NODE_ID = "cIMPConfig";
    private static uint cIMP_MESSAGE_ID = 42; 
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    private static double DEFAULT_BLOOD = 0;
    private static double DEFAULT_IMValue = 0;
    private static double SPEED_RATIO = 0.2;

    private Timer _messageUpdateTimer;
    private cIMPMessageConfig _cIMPMessageConfig;
    private IDictionary<string, StateTocIMPValue> _cIMPStates = new Dictionary<string, StateTocIMPValue>();
    // current PSs
    private double _BloodDetectionType = (int)DEFAULT_BLOOD;
    private double _IMValue = (int)DEFAULT_IMValue;

    private double _BloodDetectionTypeGoal;
    private double _IMValueGoal;

    private byte[] _cIMPData = new byte[8];

    public cIMPMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(cIMP_MESSAGE_ID);
    }

    public override void Initialize()
    {
      base.Initialize();
      _cIMPMessageConfig = new cIMPMessageConfig();
      var loadConfig = _cIMPMessageConfig.Parse(GetConfigurationNode(cIMP_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_cIMPMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _cIMPMessageConfig.Interval);
        _cIMPStates = _cIMPMessageConfig.StateTocIMPMap;
        UpdatecIMPGoal(DEFAULT_STATE); 
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

      UpdatecIMPGoal(message.State);

    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    private void UpdatecIMPGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string cIMPlocation = ConvertStateNumberToString(stateNumber);

      _BloodDetectionTypeGoal = _cIMPStates[cIMPlocation].BloodDetectionType;
      _IMValueGoal = _cIMPStates[cIMPlocation].IMValue;

    }
    private void BuildcIMP()
    {
      _BloodDetectionType = IncrementDecimal(_BloodDetectionType, _BloodDetectionTypeGoal, SPEED_RATIO);
      _IMValue = IncrementDecimal(_IMValue, _IMValueGoal, SPEED_RATIO);
      
      var BloodDetectionType = (int)(_BloodDetectionType * 10);
      var IMValue =(int)(_IMValue * 10);
 
      _cIMPData[0] = (byte)((BloodDetectionType >> 8) & 0xFF);
      _cIMPData[1] = (byte)(BloodDetectionType & 0xFF);
      _cIMPData[4] = (byte)((IMValue >> 8) & 0xFF);
      _cIMPData[5] = (byte)(IMValue & 0xFF);

    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildcIMP();

      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, cIMP_MESSAGE_ID), Length = 6, Data = _cIMPData }
      };

      PublishCanBusMessage(message);

    }

  }
}
