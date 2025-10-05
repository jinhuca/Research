using System;
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
  public class FMMessageProvider : MessageProviderBase
  {
    private static string FM_CONFIG_NODE_ID = "FMConfig";
    private static string CATHETER_INFO_NODE_ID = "CatheterConfig";
    private static uint FM_MESSAGE_ID = 2;
    private static uint INJECTION_PT_MESSAGE_ID = 15; // 0

    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    // flow for auto-switch states
    private static double DEFAULT_SWITCH_FLOW = 4900;
    // maxflow for highflow catheter
    private static double DEFAULT_MAX_FLOW = 6200;
    private static double DEFAULT_HI_FLOW = 8000;
    private static double DEFAULT_FM1 = 0;
    private static double DEFAULT_PT5 = 0;
    private static double DEFAULT_PID = 0;
    private static double SPEED_RATIO = 0.5;
    
    private Timer _messageUpdateTimer;
    private FMMessageConfig _fmMessageConfig;
    private CatheterInfoMessageConfig _catheterInfoMessageConfig;
    private IDictionary<string, StateToFMValue> _fmStates = new Dictionary<string, StateToFMValue>();
    // the current catheter 
    private int _currentCatheterId;
    // current PSs
    private double _fm1 = DEFAULT_FM1;
    private double _pt5 = DEFAULT_PT5;
    private double _pid = DEFAULT_PID;
    // this is the max flow for non-high-flow catheters
    private double _maxflow = DEFAULT_MAX_FLOW;
    // target FM flow for autoswtiching
    private double _maxswitchflow = DEFAULT_SWITCH_FLOW;

    private double _fm1Goal;
    private double _pt5Goal;
    private double _pidGoal;
    private FM1ThresholdValidation _fm1ThresholdValidation;
    private byte[] _fmData = new byte[8];

    public FMMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration,
      FM1ThresholdValidation fm1ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(FM_MESSAGE_ID);
      _fm1ThresholdValidation = fm1ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _fmMessageConfig = new FMMessageConfig();
      _catheterInfoMessageConfig = new CatheterInfoMessageConfig();
      var loadCatheterConfig = _catheterInfoMessageConfig.Parse(GetConfigurationNode(CATHETER_INFO_NODE_ID));
      if (loadCatheterConfig)
      {
        _currentCatheterId = _catheterInfoMessageConfig.CatheterId;
      }

      var loadConfig = _fmMessageConfig.Parse(GetConfigurationNode(FM_CONFIG_NODE_ID));
      if (loadConfig)
      {
        if (_fmMessageConfig.HiFlowCatheters.Contains(_currentCatheterId)) {
          // is a hi flow catheter
          _maxflow = DEFAULT_HI_FLOW;
        }
        _maxswitchflow = _fmMessageConfig.TargetFM;
        _messageUpdateTimer = new Timer(_fmMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _fmMessageConfig.Interval);
        _fmStates = _fmMessageConfig.StateToFMMap;
        UpdateFMGoal(DEFAULT_STATE); 
        _messageUpdateTimer.Elapsed += PublishPSMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    public override void UpdateParameters(CanBusMessageParameters parameters)
    {
      base.UpdateParameters(parameters);
      var messageElements = SplitCanBusMessageId(parameters.MessageId);
      if (messageElements != null && messageElements.Item2 == INJECTION_PT_MESSAGE_ID)
      {
        // We will retrieve PT2 target values for all states 
        byte[] fm1InBytes = new[] { parameters.Data[1], parameters.Data[0] };
        var fm1Target = BitConverter.ToInt16(fm1InBytes, 0);
        _fmStates[ConvertStateNumberToString(messageElements.Item3)].FM1 = fm1Target;

        UpdateFMGoal(CurrentStateId);
      }
    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      UpdateFMGoal(message.State);

    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
 
    private void UpdateFMGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string fmlocation = ConvertStateNumberToString(stateNumber);

      // prevent flow from going too high
      _fm1Goal = _fmStates[fmlocation].FM1;
      _pt5Goal = _fmStates[fmlocation].PT5;
      _pidGoal = _fmStates[fmlocation].PID;
    }

    private void BuildFM()
    {
      _fm1 = IncrementDecimal(_fm1, _fm1Goal, SPEED_RATIO);
      _pt5 = IncrementDecimal(_pt5, _pt5Goal, SPEED_RATIO);
      _pid = IncrementDecimal(_pid, _pidGoal, SPEED_RATIO);

      var fm1 = (int)(_fm1);
      var pt5 = (int)(_pt5 * 10);
      var pid = (int)(_pid * 10);
      // 
      _fmData[0] = (byte)((fm1 >> 8) & 0xFF);
      _fmData[1] = (byte)(fm1 & 0xFF);
      _fmData[2] = (byte)((pt5 >> 8) & 0xFF);
      _fmData[3] = (byte)(pt5 & 0xFF);
      _fmData[4] = (byte)((pid >> 8) & 0xFF);
      _fmData[5] = (byte)(pid & 0xFF);

    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildFM();
      _fm1ThresholdValidation.ValidateThresholds(_fm1, CurrentStateId);
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, FM_MESSAGE_ID), Length = 6, Data = _fmData }
      };

      PublishCanBusMessage(message);

      if (CurrentStateId == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION && _fm1 >= _maxswitchflow)
      {
        UpdateStateToAblation();
      }
    }

    private void UpdateStateToAblation()
    {
      ConsoleStateMessage newStateMessage = new ConsoleStateMessage();
      newStateMessage.State = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;
      // auto switch state
      PublishSystemStateChange(newStateMessage);
    }
  }
}
