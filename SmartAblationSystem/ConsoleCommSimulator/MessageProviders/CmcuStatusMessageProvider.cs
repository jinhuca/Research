using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.MessageProviders
{
  public class CmcuStatusMessageProvider : MessageProviderBase
  {
    private static string CMCU_STATUS_CONFIG_NODE_ID = "CMCUStatusConfig";
    private static uint CMCU_MESSAGE_ID = 0x23; // 35
    private static byte[] DEFAULT_CMCU_DATA = ConfigUtils.ConvertIntStringToByteArray("0x0A000000", 16);
    private IDictionary<string, byte[]> _statesDictionary;
    private Timer _messageUpdateTimer;
    private CmcuStatusMessageConfig _cmcuStatusMessageConfig;
    private byte[] _cmcuErrorData = new byte[4];

    public CmcuStatusMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(CMCU_MESSAGE_ID);
      eventAggregator?.GetEvent<ThresholdValidationFailedEvent>()?.Subscribe(HandleThresholdValidationFailedEvent);
    }

    private void HandleThresholdValidationFailedEvent(ThresholdValidationFailedEventArgs args)
    {
      // reverse endianness 
      var errorCode = BitConverter.GetBytes((uint)args.ErrorCode).Reverse().ToArray();
      if (args.StatusType == ThresholdStatusType.CLEAR_CMCU_STATUS)
      {
        _cmcuErrorData = RemoveErrorCodeFromErrorData(errorCode, _cmcuErrorData);
      } 
      else if (args.StatusType == ThresholdStatusType.CMCU_STATUS)// must be cmcu error
      {
        // error doesn't get reset
        for (int i = 0; i < 4; i++)
        {
          _cmcuErrorData[i] = (byte)(errorCode[i] | _cmcuErrorData[i]);
        }
      }
      else if (args.StatusType == ThresholdStatusType.RESET_CMCU)// must be cmcu error
      {
        // reset to default
        _cmcuErrorData = DEFAULT_CMCU_DATA;
      }
    }

    public override void Initialize()
    {
      base.Initialize();
      _cmcuStatusMessageConfig = new CmcuStatusMessageConfig();
      var loadConfig = _cmcuStatusMessageConfig.Parse(GetConfigurationNode(CMCU_STATUS_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _statesDictionary = _cmcuStatusMessageConfig.StateToMessageByteMap;
        // default value is 100
        _messageUpdateTimer = new Timer(_cmcuStatusMessageConfig.Interval <= 0 ? 100 : _cmcuStatusMessageConfig.Interval); 
        _messageUpdateTimer.Elapsed += PublishCmcuMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        LogSystem.LogService.LogInfo("Parsing configuration failed");
      }

    }
    
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    private void PublishCmcuMessage(object sender, ElapsedEventArgs e)
    {
      // bitwise OR to combine the two status messages byte arrays
      byte[] result = new byte[4];
      for (int i = 0; i < 4; i++)
      {
        result[i] = (byte)(MessageStateIdToHexValue(CurrentStateId, _statesDictionary)[i] | _cmcuErrorData[i]);
      }
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, CMCU_MESSAGE_ID), Length = 4, Data = result }
      };

      PublishCanBusMessage(message);
    }

  }

}
