using System;
using System.Collections.Generic;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.MessageProviders
{
  public abstract class MessageProviderBase : ICanBusMessageProvider
  {
    private readonly IEventAggregator _eventAggregator;
    private readonly ISimulatorConfiguration _configuration;

    protected MessageProviderBase(IEventAggregator eventAggregator, ISimulatorConfiguration configuration)
    {
      _eventAggregator = eventAggregator;
      _configuration = configuration;
    }

    protected MessageStateId CurrentStateId { get; set; } = MessageStateId.CAN_ID_STATE_IDLE;
    protected uint NodeId { get; set; }
    protected uint FlowCatheterId { get; set; }

    public virtual void Initialize()
    {
      _eventAggregator.GetEvent<SystemStateUpdateEvent>()?.Subscribe(HandleSystemStateUpdate);
    }

    public virtual void UpdateParameters(CanBusMessageParameters parameters)
    {
    }

    public void Dispose()
    {
      DisposeMessageProvider();
    }

    protected byte[] MessageStateIdToHexValue(MessageStateId CurrentStateId, IDictionary<string, byte[]> statesDictionary)
    {
      switch (CurrentStateId)
      {
        case MessageStateId.CAN_ID_STATE_IDLE:
          return statesDictionary["IDLE"];
        case MessageStateId.CAN_ID_STATE_READY:
          return statesDictionary["READY"];
        case MessageStateId.CAN_ID_STATE_INFLATION:
          return statesDictionary["INFLATION"];
        case MessageStateId.CAN_ID_STATE_TRANSITION:
          return statesDictionary["TRANSITION"];
        case MessageStateId.CAN_ID_STATE_ABLATION:
          return statesDictionary["ABLATION"];
        case MessageStateId.CAN_ID_STATE_THAWING:
          return statesDictionary["THAWING"];
        default: // unknown, EXCEPTION or maintenance state
          return statesDictionary["EXCEPTION"];
      }
    }
    protected void PublishCanBusMessage(CanBusMessage message)
    {
      _eventAggregator?.GetEvent<CanBusMessageUpdateEvent>().Publish(message);
    }
    protected void PublishSystemStateChange(ConsoleStateMessage message)
    {
      _eventAggregator?.GetEvent<SystemStateUpdateEvent>().Publish(message);
    }
    protected XmlNode GetConfigurationNode(string nodeId, string name="default")
    {
      return _configuration?.LoadConfigurationSection(nodeId, name);
    }

    protected virtual void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      CurrentStateId = message.State;
    }

    protected virtual void DisposeMessageProvider()
    {
    }

    public Tuple<uint, uint, MessageStateId> SplitCanBusMessageId(uint canBusMessageId)
    {
      uint nodeId = (canBusMessageId >> 11) & 0x07;
      uint elementId = canBusMessageId & 0x3f;
      uint state = (canBusMessageId) & (0x07 << 8);

      return new Tuple<uint, uint, MessageStateId>(nodeId, elementId, (MessageStateId)state);
    }
    protected uint CreatePriority0MessageId(MessageStateId stateId, uint messageId)
    {
      uint priorityid = 0x00 << 14;
      uint nodeid = NodeId << 11;
      // 1 << 8 = 256, no need to convert state to int 
      //uint stateid = ConvertStateToInt(stateId) << 8;
      uint typeid = (NodeId <= 2 ? ConvertElementToType(messageId) : 0) << 6;
      uint elementid = messageId & 0x3F;

      // use bit shifting instead
      uint newid = priorityid | nodeid | (uint)stateId | typeid | elementid;
      return newid;
    }
    protected uint CreatePriority0MessageIdTwo(MessageStateId stateId, uint messageId, uint typeId)
    {
      // for canbus 2 
      uint priorityid = 0x00 << 14;
      uint nodeid = NodeId << 11;
      // 1 << 8 = 256, no need to convert state to int 
      //uint stateid = ConvertStateToInt(stateId) << 8;
      uint typeid = typeId << 6;
      uint elementid = messageId & 0x3F;

      // use bit shifting instead
      uint newid = priorityid | nodeid | (uint)stateId | typeid | elementid;
      return newid;
    }
    protected uint CreateMessageId(MessageStateId stateId, uint messageId, uint priorityId = 3)
    {
      // the messageId is an int but to understand we need binary
      // priority is always 3 in normal conditions
      // input messageId is the elementId
      // we can get the type and node based on elementId
      uint priorityid = priorityId << 14;
      uint nodeid = NodeId << 11;
      // 1 << 8 = 256, no need to convert state to int 
      //uint stateid = ConvertStateToInt(stateId) << 8;
      uint typeid = (NodeId <= 2 ? ConvertElementToType(messageId) : 0) << 6;
      uint elementid = messageId & 0x3F;

      // use bit shifting instead
      uint newid = priorityid | nodeid | (uint)stateId | typeid | elementid;
      return newid;
    }

    protected static double IncrementDecimal(double PT, double PTGoal, double SpeedRatio)
    {
      // speedratio decides how fast the changes happen
      double finalPT;
      // could replace with a function in configUtils that will generate "curves" for all future data like this
      // decimal PT
      double dPT = PT;
      if (PTGoal > dPT)
      {
        if ((PTGoal - dPT) > 1000)
        {
          finalPT = dPT + 1000 * SpeedRatio;
        }
        else if ((PTGoal - dPT) > 100)
        {
          finalPT = dPT + 100 * SpeedRatio;
        }
        else if ((PTGoal - dPT) > 10)
        {
          finalPT = dPT + 10 * SpeedRatio;
        }
        else
        // if the difference is smaller than 10, then we have reached PTGoal
        {
          finalPT = PTGoal;
        }
      }
      else // means we are reducing dPT
      {
        if ((dPT - PTGoal) > 1000)
        {
          finalPT = dPT - 100;
        }
        else if ((dPT - PTGoal) > 100)
        {
          finalPT = dPT - 10;
        }
        else if ((dPT - PTGoal) > 10)
        {
          finalPT = dPT - 1;
        }
        else
        // if the difference is smaller than 10, then we have reached PTGoal
        {
          finalPT = PTGoal;
        }
      }
      return finalPT;
    }

    protected static uint ConvertStateToInt(MessageStateId stateId)
    {
      switch (stateId)
      {
        case MessageStateId.CAN_ID_STATE_IDLE:
          return 1;
        case MessageStateId.CAN_ID_STATE_READY:
          return 2;
        case MessageStateId.CAN_ID_STATE_INFLATION:
          return 3;
        case MessageStateId.CAN_ID_STATE_TRANSITION:
          return 4;
        case MessageStateId.CAN_ID_STATE_ABLATION:
          return 5;
        case MessageStateId.CAN_ID_STATE_THAWING:
          return 6;
        case MessageStateId.CAN_ID_STATE_EXCEPTION:
          return 7;
        default: // unknown or maintenance state
          return 0;
      }
    }
    protected static uint ConvertElementToType(uint elementid)
    {   // read values, including pmcu ones
      if (elementid == 0 || elementid == 1 || elementid == 2 || elementid == 3 || elementid == 4 || elementid == 40 || elementid == 41 || elementid == 42)
      {
        return 0;
      }
      else
      { // register/non volatile
        return 1;
      }
    }
    // for canbus one
    protected static uint ConvertElementToNodeOne(uint elementid)
    {
      if (elementid <= 37 && elementid >= 0)
      { // cmcu
        return 1;
      }
      else if (elementid <= 57 && elementid >= 40)
      { // pmcu
        return 0;
      }
      else
      { // sbc
        return 2;
      }
    }
    // for canbus 2
    protected static uint ConvertElementToNodeTwo(uint elementid)
    {
      if (elementid <= 31 && elementid >= 0)
      { // ICB
        return 3;
      }

      else
      { // not icb
        return 4;
      }
    }
    protected static MessageStateId ConvertStateToString(string stateString)
    {
      MessageStateId result;
      switch (stateString)
      {
        case "IDLE":
          result = MessageStateId.CAN_ID_STATE_IDLE;
          break;
        case "READY":
          result = MessageStateId.CAN_ID_STATE_READY;
          break;
        case "INFLATION":
          result = MessageStateId.CAN_ID_STATE_INFLATION;
          break;
        case "TRANSITION":
          result = MessageStateId.CAN_ID_STATE_TRANSITION;
          break;
        case "ABLATION":
          result = MessageStateId.CAN_ID_STATE_ABLATION;
          break;
        case "THAWING":
          result = MessageStateId.CAN_ID_STATE_THAWING;
          break;
        default:
          result = MessageStateId.CAN_ID_STATE_EXCEPTION;
          break;
      }
      return result;
    }
    protected static string ConvertStateNumberToString(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string result;
      switch (stateNumber)
      {

        case MessageStateId.CAN_ID_STATE_IDLE:
          result = "IDLE";
          break;
        case MessageStateId.CAN_ID_STATE_READY:
          result = "READY";
          break;
        case MessageStateId.CAN_ID_STATE_INFLATION:
          result = "INFLATION";
          break;
        case MessageStateId.CAN_ID_STATE_TRANSITION:
          result = "TRANSITION";
          break;
        case MessageStateId.CAN_ID_STATE_ABLATION:
          result = "ABLATION";
          break;
        case MessageStateId.CAN_ID_STATE_THAWING:
          result = "THAWING";
          break;
        default: // should be an unknown/maintenance state but I combined into exception
          result = "EXCEPTION";
          break;
      }
      return result;
    }
    protected static MessageStateId ConvertStringToState(string stateString)
    {
      MessageStateId result;
      switch (stateString)
      {
        case "IDLE":
          result = MessageStateId.CAN_ID_STATE_IDLE;
          break;
        case "READY":
          result = MessageStateId.CAN_ID_STATE_READY;
          break;
        case "INFLATION":
          result = MessageStateId.CAN_ID_STATE_INFLATION;
          break;
        case "TRANSITION":
          result = MessageStateId.CAN_ID_STATE_TRANSITION;
          break;
        case "ABLATION":
          result = MessageStateId.CAN_ID_STATE_ABLATION;
          break;
        case "THAWING":
          result = MessageStateId.CAN_ID_STATE_THAWING;
          break;
        default:
          result = MessageStateId.CAN_ID_STATE_EXCEPTION;
          break;
      }
      return result;
    }

    protected byte[] RemoveErrorCodeFromErrorData(byte[] errorCode, byte[] errorData)
    {

      // Remove the error code from _cmcuErrorData
      for (int i = 0; i < 4; i++)
      {
        errorData[i] = (byte)(errorData[i] & ~errorCode[i]);
      }

      return errorData;
    }
  }
}

