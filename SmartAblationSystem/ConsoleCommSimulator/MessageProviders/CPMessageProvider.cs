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
  public class CPMessageProvider : MessageProviderBase
  {
    private static string CP_CONFIG_NODE_ID = "CPConfig";
    private static uint CP_MESSAGE_ID = 41; 
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    private static double DEFAULT_CP1 = 0;
    private static double DEFAULT_CP2 = 0;
    private static double DEFAULT_CTIP = 0;
    private static double DEFAULT_PIDDUTY = 0;
    private static double SPEED_RATIO = 0.2;


    private Timer _messageUpdateTimer;
    private CPMessageConfig _cpMessageConfig;
    private IDictionary<string, StateToCPValue> _cpStates = new Dictionary<string, StateToCPValue>();
    // current PSs
    private double _cp1 = DEFAULT_CP1;
    private double _cp2 = DEFAULT_CP2;
    private double _ctip = DEFAULT_CTIP;
    private double _pid = DEFAULT_PIDDUTY;

    private double _cp1Goal;
    private double _cp2Goal;
    private double _ctipGoal;
    private double _pidGoal;

    private byte[] _cpData = new byte[8];
    private CP1ThresholdValidation _cp1ThresholdValidation;
    private CP2ThresholdValidation _cp2ThresholdValidation;
    public CPMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration, 
      CP1ThresholdValidation cp1ThresholdValidation, CP2ThresholdValidation cp2ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(CP_MESSAGE_ID);
      _cp1ThresholdValidation = cp1ThresholdValidation;
      _cp2ThresholdValidation = cp2ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _cpMessageConfig = new CPMessageConfig();
      var loadConfig = _cpMessageConfig.Parse(GetConfigurationNode(CP_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_cpMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _cpMessageConfig.Interval);
        _cpStates = _cpMessageConfig.StateToCPMap;
        UpdateCPGoal(DEFAULT_STATE);
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
      if (messageElements != null && messageElements.Item2 == 52)
      {
        // We will retrieve PT2 target values for all states 
        byte[] cp1InBytes = new[] { parameters.Data[1], parameters.Data[0] };
        var cp1Target = BitConverter.ToInt16(cp1InBytes, 0);
        var state = ConvertStateNumberToString(messageElements.Item3);
        _cpStates[state].CP1 = cp1Target/10.0;

        UpdateCPGoal(CurrentStateId);
      }
    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);

      UpdateCPGoal(message.State);

    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    private void UpdateCPGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string fmlocation = ConvertStateNumberToString(stateNumber);

      _cp1Goal = _cpStates[fmlocation].CP1;
      _cp2Goal = _cpStates[fmlocation].CP2;
      _ctipGoal = _cpStates[fmlocation].CTIP;
      _pidGoal = _cpStates[fmlocation].PIDDUTY;
    }
    private void BuildCP()
    {
      _cp1 = IncrementDecimal(_cp1, _cp1Goal, SPEED_RATIO);
      _cp2 = IncrementDecimal(_cp2, _cp2Goal, SPEED_RATIO);
      _ctip = IncrementDecimal(_ctip, _ctipGoal, SPEED_RATIO);
      _pid = IncrementDecimal(_pid, _pidGoal, SPEED_RATIO);

      var cp1 = (int)(_cp1 * 10);
      var cp2 = (int)(_cp2 * 10);
      var ctip = (int)(_ctip * 10);
      var pid = (int)(_pid * 10);
      // 
      _cpData[0] = (byte)((cp1 >> 8) & 0xFF);
      _cpData[1] = (byte)(cp1 & 0xFF);
      _cpData[2] = (byte)((cp2 >> 8) & 0xFF);
      _cpData[3] = (byte)(cp2 & 0xFF);
      _cpData[4] = (byte)((ctip >> 8) & 0xFF);
      _cpData[5] = (byte)(ctip & 0xFF);
      _cpData[6] = (byte)((pid >> 8) & 0xFF);
      _cpData[7] = (byte)(pid & 0xFF);

    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildCP();
      _cp1ThresholdValidation.ValidateThresholds(_cp1, CurrentStateId);
      _cp2ThresholdValidation.ValidateThresholds(_cp2, CurrentStateId);
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, CP_MESSAGE_ID), Length = 8, Data = _cpData }
      };

      PublishCanBusMessage(message);
    }
  }
}
