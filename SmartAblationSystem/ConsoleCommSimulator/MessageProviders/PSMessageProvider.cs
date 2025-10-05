using System.Collections.Generic;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;
using ConsoleCommSimulator.Validation;

namespace ConsoleCommSimulator.MessageProviders
{
  public class PSMessageProvider : MessageProviderBase
  {
    private static string PS_CONFIG_NODE_ID = "PSConfig";
    private static uint PS_MESSAGE_ID = 0x01; // 1
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    private static double DEFAULT_PS1 = 0;
    private static double DEFAULT_PT5 = 0;
    private static double SPEED_RATIO = 0.3;


    private Timer _messageUpdateTimer;
    private PSMessageConfig _psMessageConfig;
    private IDictionary<string, StateToPSValue> _psStates = new Dictionary<string, StateToPSValue>();
    private PS1ThresholdValidation _ps1ThresholdValidation;
    private PT5ThresholdValidation _pt5ThresholdValidation;
    // current PS
    private double _ps1 = DEFAULT_PS1;
    private double _pt5 = DEFAULT_PT5;

    private double _ps1Goal;
    private double _pt5Goal;

    private byte[] _psData = new byte[8];

    public PSMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration,
      PS1ThresholdValidation ps1ThresholdValidation, PT5ThresholdValidation pt5ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(PS_MESSAGE_ID);
      _ps1ThresholdValidation = ps1ThresholdValidation;
      _pt5ThresholdValidation = pt5ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _psMessageConfig = new PSMessageConfig();
      var loadConfig = _psMessageConfig.Parse(GetConfigurationNode(PS_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_psMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _psMessageConfig.Interval);
        _psStates = _psMessageConfig.StateToPSMap;
        UpdatePSGoal(DEFAULT_STATE); 
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

      UpdatePSGoal(message.State);

    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private void UpdatePSGoal(Communication.CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string pslocation = ConvertStateNumberToString(stateNumber);

      _ps1Goal = _psStates[pslocation].PS1;
      _pt5Goal = _psStates[pslocation].PT5;

    }

    private void BuildPS()
    {
      _ps1 = IncrementDecimal(_ps1, _ps1Goal, SPEED_RATIO);
      _pt5 = IncrementDecimal(_pt5, _pt5Goal, SPEED_RATIO);

      // 
      var ps1 = (int)(_ps1 * 10);
      var pt5 = (int)(_pt5 * 10);
      _psData[0] = (byte)((ps1 >> 8) & 0xFF);
      _psData[1] = (byte)(ps1 & 0xFF);
      _psData[2] = (byte)((pt5 >> 8) & 0xFF);
      _psData[3] = (byte)(pt5 & 0xFF);
      
    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildPS();
      _pt5ThresholdValidation.ValidateThresholds(_pt5, CurrentStateId);
      _ps1ThresholdValidation.ValidateThresholds(_ps1, CurrentStateId);
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, PS_MESSAGE_ID), Length = 4, Data = _psData }
      };

      PublishCanBusMessage(message);
    }

  }
}
