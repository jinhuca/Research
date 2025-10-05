using System;
using System.Collections.Generic;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.Validation;
using Prism.Events;
using static Communication.CanBusMessageDefinition;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class PTMessageProvider : MessageProviderBase
  {
    private static string PT_CONFIG_NODE_ID = "PTConfig";
    private static uint PT_MESSAGE_ID = 0x0; // 0
    private static uint INJECTION_PT_MESSAGE_ID = 15; 

    private static MessageStateId DEFAULT_STATE = MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    // PT is the pressure in PSI * 10. Normal pressure is 14.6psi, pt3 is 146
    private static double DEFAULT_PT1 = 700;
    private static double DEFAULT_PT2 = 0;
    private static double DEFAULT_PT3 = 14.6;
    private static double DEFAULT_PT4 = 0;
    private static double SPEED_RATIO = 0.6;

    private Timer _messageUpdateTimer;
    private PTMessageConfig _ptMessageConfig;
    private IDictionary<string, StateToPTValue> _ptStates = new Dictionary<string, StateToPTValue>();
    private PT1ThresholdValidation _pt1ThresholdValidation;
    private PT2ThresholdValidation _pt2ThresholdValidation;
    private PT3ThresholdValidation _pt3ThresholdValidation;
    private PT4ThresholdValidation _pt4ThresholdValidation;

    // current PT
    private double _pt1 = DEFAULT_PT1;
    private double _pt2 = DEFAULT_PT2;
    private double _pt3 = DEFAULT_PT3;
    private double _pt4 = DEFAULT_PT4; 
    // goal pt
    private double _pt1Goal;
    private double _pt2Goal;
    private double _pt3Goal;
    private double _pt4Goal;
    private byte[] _ptData = new byte[8];

    public PTMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration,
      PT1ThresholdValidation pt1ThresholdValidation, PT2ThresholdValidation pt2ThresholdValidation, 
      PT3ThresholdValidation pt3ThresholdValidation, PT4ThresholdValidation pt4ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(PT_MESSAGE_ID);
      _pt1ThresholdValidation = pt1ThresholdValidation;
      _pt2ThresholdValidation = pt2ThresholdValidation;
      _pt3ThresholdValidation = pt3ThresholdValidation;
      _pt4ThresholdValidation = pt4ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _ptMessageConfig = new PTMessageConfig();
      var loadConfig = _ptMessageConfig.Parse(GetConfigurationNode(PT_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_ptMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _ptMessageConfig.Interval);
        _ptStates = _ptMessageConfig.StateToIntByteMap;
        UpdatePTGoal(DEFAULT_STATE); 
        _messageUpdateTimer.Elapsed += PublishPTMessage;
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
        byte[] pt2InBytes = new[] { parameters.Data[3], parameters.Data[2] };
        var pt2Target = BitConverter.ToInt16(pt2InBytes, 0);
        _ptStates[ConvertStateNumberToString(messageElements.Item3)].PT2 = pt2Target / 10.0;
        UpdatePTGoal(CurrentStateId);
      }
      
    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);

      UpdatePTGoal(message.State);

    }
    
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    } 
    private void UpdatePTGoal(MessageStateId stateNumber)
    {
      string ptlocation = ConvertStateNumberToString(stateNumber);
      _pt1Goal = _ptStates[ptlocation].PT1;
      _pt2Goal = _ptStates[ptlocation].PT2;
      _pt3Goal = _ptStates[ptlocation].PT3;
      _pt4Goal = _ptStates[ptlocation].PT4;

    }
  
    private void BuildPT()
    {
      // 14.6 becomes 146 for canbus
      _pt1 = IncrementDecimal(_pt1, _pt1Goal, SPEED_RATIO);
      _pt2 = IncrementDecimal(_pt2, _pt2Goal, SPEED_RATIO);
      _pt3 = IncrementDecimal(_pt3, _pt3Goal, SPEED_RATIO);
      _pt4 = IncrementDecimal(_pt4, _pt4Goal, SPEED_RATIO);
      // 
      if (_pt1 < 0 || _pt2 < 0 || _pt3 < 0 || _pt4 < 0 )
      {
        var errortext = "PT was negative trying to build PT";
        Log.LogInfo(errortext);
        throw new ArgumentException(errortext);

      }

      var pt1 = (int)(_pt1 * 10);
      var pt2 = (int)(_pt2 * 10);
      var pt3 = (int)(_pt3 * 10);
      var pt4 = (int)(_pt4 * 10);

      _ptData[0] = (byte)((pt1 >> 8) & 0xFF);
      _ptData[1] = (byte)(pt1 & 0xFF);
      _ptData[2] = (byte)((pt2 >> 8) & 0xFF);
      _ptData[3] = (byte)(pt2 & 0xFF);
      _ptData[4] = (byte)((pt3 >> 8) & 0xFF);
      _ptData[5] = (byte)(pt3 & 0xFF);
      _ptData[6] = (byte)((pt4 >> 8) & 0xFF);
      _ptData[7] = (byte)(pt4 & 0xFF);
      
    }

    private void PublishPTMessage(object sender, ElapsedEventArgs e)
    {
      BuildPT();
      _pt1ThresholdValidation.ValidateThresholds(_pt1, CurrentStateId);
      _pt2ThresholdValidation.ValidateThresholds(_pt2, CurrentStateId);
      _pt3ThresholdValidation.ValidateThresholds(_pt3, CurrentStateId);
      _pt4ThresholdValidation.ValidateThresholds(_pt4, CurrentStateId);
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, PT_MESSAGE_ID), Length = 8, Data = _ptData }
      };

      PublishCanBusMessage(message);
    }
  }
}
