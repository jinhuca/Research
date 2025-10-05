using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;
using static Communication.CanBusMessageDefinition;
using System.Collections.Generic;
using ConsoleCommSimulator.Validation;
using Module.Console.Helpers;

namespace ConsoleCommSimulator.MessageProviders
{
  public class RTRMessageProvider : MessageProviderBase
  {
    private static uint CATHETER_50_MESSAGE_ID = 0x32; // 50
    private static uint TARGET_FLOW_MESSAGE_ID = 15;
    private static uint THRESHOLD_PT1_MESSAGE_ID = 17;
    private static uint THRESHOLD_PT2_MESSAGE_ID = 19;
    private static uint THRESHOLD_PT3_MESSAGE_ID = 21;
    private static uint THRESHOLD_PT4_MESSAGE_ID = 23;
    private static uint THRESHOLD_TS1_MESSAGE_ID = 25;
    private static uint THRESHOLD_FM1_MESSAGE_ID = 27;
    private static uint THRESHOLD_FM1_CURVE_MESSAGE_ID = 28;
    private static uint THRESHOLD_PS1_MESSAGE_ID = 29;
    private static uint THRESHOLD_PT5_MESSAGE_ID = 31;
    private static uint THRESHOLD_LC1_MESSAGE_ID = 33;
    private static uint THRESHOLD_TC_MESSAGE_ID = 54;
    private static uint THRESHOLD_CP_MESSAGE_ID = 53;
    private static uint CATHETER_52_MESSAGE_ID = 52; 
    private static uint CATHETER_53_MESSAGE_ID = 53; 
    private static uint CATHETER_54_MESSAGE_ID = 54; 
    private static uint CATHETER_55_MESSAGE_ID = 55; 
    private static uint CATHETER_57_MESSAGE_ID = 57;
    private static uint NODE_ID_2 = 2;
    private static uint NODE_ID_1 = 1;
    private static string RTR_CONFIG_NODE_ID = "RTRConfig";
    //private Timer _messageUpdateTimer;
    private RTRMessageConfig _RTRMessageConfig;
    private RTRThresholdEventUpdater _rtrUpdater;

    public RTRMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration, RTRThresholdEventUpdater rTRThresholdEventUpdater) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(CATHETER_50_MESSAGE_ID);
      _rtrUpdater = rTRThresholdEventUpdater;
    }

    public override void Initialize()
    {
      base.Initialize();
      _RTRMessageConfig = new RTRMessageConfig();
      
      var loadConfig = _RTRMessageConfig.Parse(GetConfigurationNode(RTR_CONFIG_NODE_ID));
      if (loadConfig)
      {
        // start listening and waiting
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

      // if we get confirmation, send RTR
      if (messageElements.Item1 == NodeId && messageElements.Item2 == CATHETER_50_MESSAGE_ID
          && IsCatheterValid(parameters.Data))
      {
        // sending flow RTR
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdPT1RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdPT2RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdPT3RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdPT4RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdPT5RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdTS1RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdFM1RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdFMCurveRTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesThresholdLC1RTRMessages();
        System.Threading.Thread.Sleep(10);
        PublishAllStatesTargetFlowRTRMessages();
        // send RTR messages because it's valid
        System.Threading.Thread.Sleep(100);
        PublishAllStatesCatheterRTRMessages();  
      }
      // received RTR for PT thresholds
      else if (messageElements.Item2 == THRESHOLD_PT1_MESSAGE_ID ||
        messageElements.Item2 == THRESHOLD_PT2_MESSAGE_ID ||
        messageElements.Item2 == THRESHOLD_PT3_MESSAGE_ID ||
        messageElements.Item2 == THRESHOLD_PT4_MESSAGE_ID ||
        messageElements.Item2 == THRESHOLD_PT5_MESSAGE_ID)

      {
        IDictionary<MessageStateId, Thresholds> ptDictionary = new Dictionary<MessageStateId, Thresholds>();
        // Set threshold values for the corresponding PT
        double ptHigh = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 0);
        double ptFail = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 2);
        double ptLow = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 4);

        Thresholds ptThresholds;
        if (messageElements.Item2 == THRESHOLD_PT1_MESSAGE_ID)
        {
          Thresholds pt1Thresholds = new Thresholds();
          pt1Thresholds.HighValue = ptHigh;
          pt1Thresholds.FailValue = ptFail;
          pt1Thresholds.LowValue = ptLow;
          ptThresholds = pt1Thresholds;
        }
        else if (messageElements.Item2 == THRESHOLD_PT2_MESSAGE_ID)
        {
          Thresholds pt2Thresholds = new Thresholds();
          pt2Thresholds.HighValue = ptHigh;
          pt2Thresholds.FailValue = ptFail;
          pt2Thresholds.LowValue = ptLow;
          ptThresholds = pt2Thresholds;
        }
        else if (messageElements.Item2 == THRESHOLD_PT3_MESSAGE_ID)
        {
          Thresholds pt3Thresholds = new Thresholds();
          pt3Thresholds.HighValue = ptHigh;
          pt3Thresholds.FailValue = ptFail;
          pt3Thresholds.LowValue = ptLow;
          ptThresholds = pt3Thresholds;
        }
        else if (messageElements.Item2 == THRESHOLD_PT4_MESSAGE_ID)// must be PT4
        {
          Thresholds pt4Thresholds = new Thresholds();
          pt4Thresholds.HighValue = ptHigh;
          pt4Thresholds.FailValue = ptFail;
          pt4Thresholds.LowValue = ptLow;
          ptThresholds = pt4Thresholds;
        }
        else if (messageElements.Item2 == THRESHOLD_PT5_MESSAGE_ID)
        {
          Thresholds pt5Thresholds = new Thresholds();
          pt5Thresholds.HighValue = ptHigh;
          ptThresholds = pt5Thresholds;
        }
        else
        {
          // error
          return;
        }
        // Add the PT threshold to the respective dictionary
        ptDictionary[messageElements.Item3] = ptThresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(ptDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
      else if (messageElements.Item2 == THRESHOLD_TS1_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> tsDictionary = new Dictionary<MessageStateId, Thresholds>();
        double tsHigh = CanBusMessageConverter.ConverteNegativDecimalData(parameters.Data, 0);
        Thresholds ts1Thresholds = new Thresholds();
        ts1Thresholds.HighValue = tsHigh;
        tsDictionary[messageElements.Item3] = ts1Thresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(tsDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }

      else if (messageElements.Item2 == THRESHOLD_PS1_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> psDictionary = new Dictionary<MessageStateId, Thresholds>();
        // Set threshold values for the corresponding 
        double psHigh = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 0); 
        Thresholds ps1Thresholds = new Thresholds();
        ps1Thresholds.HighValue = psHigh;
        // Add the threshold to the respective dictionary
        psDictionary[messageElements.Item3] = ps1Thresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(psDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
      else if (messageElements.Item2 == THRESHOLD_FM1_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> fmDictionary = new Dictionary<MessageStateId, Thresholds>();
        double fmLow = CanBusMessageConverter.ConverteFM1NegativDecimalData(parameters.Data, 0); // 
        double fmHigh = CanBusMessageConverter.ConverteFM1NegativDecimalData(parameters.Data, 2); // 
        Thresholds fm1Thresholds = new Thresholds();
        fm1Thresholds.HighValue = fmHigh;
        fm1Thresholds.LowValue = fmLow;
        fmDictionary[messageElements.Item3] = fm1Thresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(fmDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
      else if (messageElements.Item2 == THRESHOLD_LC1_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> lcDictionary = new Dictionary<MessageStateId, Thresholds>();
        double lcWarning = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 0); 
        double lcFail = CanBusMessageConverter.ConverteDecimalData(parameters.Data, 2);  
        Thresholds fm1Thresholds = new Thresholds();
        fm1Thresholds.LowValue = lcWarning;
        fm1Thresholds.FailValue = lcFail;
        lcDictionary[messageElements.Item3] = fm1Thresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(lcDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
      else if (messageElements.Item2 == THRESHOLD_CP_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> cpDictionary = new Dictionary<MessageStateId, Thresholds>();

        double CP1High = CanBusMessageConverter.ConverteNegativDecimalData(parameters.Data, 0);
        double cpOuterHigh = CanBusMessageConverter.ConverteNegativDecimalData(parameters.Data, 2);
        double cpInnerLow = CanBusMessageConverter.ConverteNegativDecimalData(parameters.Data, 4);
        Thresholds cpThresholds = new Thresholds();
        cpThresholds.HighValue = CP1High;
        cpThresholds.InnerValue = cpInnerLow;
        cpThresholds.OuterValue = cpOuterHigh;
        cpDictionary[messageElements.Item3] = cpThresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(cpDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
      //cTC is not temperature, it is thawing temperature
      else if (messageElements.Item2 == THRESHOLD_TC_MESSAGE_ID)
      {
        IDictionary<MessageStateId, Thresholds> tcDictionary = new Dictionary<MessageStateId, Thresholds>();
        double tcLow = CanBusMessageConverter.ConverteNegativDecimalData(parameters.Data, 0);
        Thresholds tc1Thresholds = new Thresholds();
        tc1Thresholds.LowValue = tcLow;
        tcDictionary[messageElements.Item3] = tc1Thresholds;
        UpdateThresholdEventArgs args = new UpdateThresholdEventArgs(tcDictionary, GetThresholdName(messageElements.Item2));
        _rtrUpdater.PublishUpdate(args);
      }
    }
    private ThresholdType GetThresholdName(uint messageId)
    {
      ThresholdType thresholdName = ThresholdType.NONE;

      if (messageId == THRESHOLD_PT1_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PT1;
      }
      else if (messageId == THRESHOLD_PT2_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PT2;
      }
      else if (messageId == THRESHOLD_PT3_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PT3;
      }
      else if (messageId == THRESHOLD_PT4_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PT4;
      }
      else if (messageId == THRESHOLD_PT5_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PT5;
      }
      else if (messageId == THRESHOLD_FM1_MESSAGE_ID)
      {
        thresholdName = ThresholdType.FM1;
      }
      else if (messageId == THRESHOLD_LC1_MESSAGE_ID)
      {
        thresholdName = ThresholdType.LC1;
      }
      else if (messageId == THRESHOLD_PS1_MESSAGE_ID)
      {
        thresholdName = ThresholdType.PS1;
      }
      else if (messageId == THRESHOLD_CP_MESSAGE_ID)
      {
        thresholdName = ThresholdType.CP;
      }
      else if (messageId == THRESHOLD_TC_MESSAGE_ID)
      {
        thresholdName = ThresholdType.TC;
      }
      else if (messageId == THRESHOLD_TS1_MESSAGE_ID)
      {
        thresholdName = ThresholdType.TS1;
      }
      else
      {
        Log.LogInfo("Invalid threshold string");
      }
      return thresholdName;
    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      //_messageUpdateTimer?.Stop();
    }

    private bool IsCatheterValid(byte[] data)
    {
      return data[0] != 0 && data[1] != 0;
    }
    private void PublishAllStatesTargetFlowRTRMessages()
    {
      NodeId = NODE_ID_2;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message15 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, TARGET_FLOW_MESSAGE_ID, 1), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message15);
      }
    }
    private void PublishAllStatesThresholdPT1RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message17 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_PT1_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message17);
      }
    }
    private void PublishAllStatesThresholdPT2RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message19 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_PT2_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message19);
      }
    }
    private void PublishAllStatesThresholdPT3RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message21 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_PT3_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message21);
      }
    }
    private void PublishAllStatesThresholdPT4RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message23 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_PT4_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message23);
      }
    }
    private void PublishAllStatesThresholdPT5RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message31 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_PT5_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message31);
      }
    }
    private void PublishAllStatesThresholdTS1RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message25 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_TS1_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message25);
      }
    }
    private void PublishAllStatesThresholdLC1RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message33 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_LC1_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message33);
      }
    }
    private void PublishAllStatesThresholdFM1RTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message27 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_FM1_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message27);
      }
    }
    private void PublishAllStatesThresholdFMCurveRTRMessages()
    {
      NodeId = NODE_ID_1;
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          continue;
        }
        var message28 = new CanBusMessage()
        {
          // priority is 1
          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, THRESHOLD_FM1_CURVE_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        PublishCanBusMessage(message28);
      }
    }
    private void PublishAllStatesCatheterRTRMessages()
    {
      NodeId = ConvertElementToNodeOne(CATHETER_50_MESSAGE_ID);
      foreach (MessageStateId state in System.Enum.GetValues(typeof(MessageStateId)))  
      {
        if (state == MessageStateId.CAN_ID_STATE_EXCEPTION || state == MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          // skip 
          continue;
        }
        var message52 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, CATHETER_52_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        var message53 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, CATHETER_53_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        var message54 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, CATHETER_54_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        var message55 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, CATHETER_55_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        var message57 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 5, Id = CreateMessageId(state, CATHETER_57_MESSAGE_ID), Length = 0, Data = new byte[8] }
        };
        
        PublishCanBusMessage(message52);
        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message53);
        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message54);
        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message55);
        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message57);
      }
    }
  }
}
