using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class PmcuStatusMessageProvider : MessageProviderBase
  {
    private static string PMCU_STATUS_CONFIG_NODE_ID = "PMCUStatusConfig";
    private static uint PMCU_MESSAGE_ID = 0x31; // 49
    private static byte[] DEFAULT_PMCU_DATA = ConfigUtils.ConvertIntStringToByteArray("0x09000000", 16);
    private IDictionary<string, byte[]> _statesDictionary;
    private Timer _messageUpdateTimer;
    private PmcuStatusMessageConfig _pmcuStatusMessageConfig;

    private byte[] _pmcuErrorData = new byte[4];
    public PmcuStatusMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(PMCU_MESSAGE_ID);

      eventAggregator?.GetEvent<ThresholdValidationFailedEvent>()?.Subscribe(HandleThresholdValidationFailedEvent);
    }
    private void HandleThresholdValidationFailedEvent(ThresholdValidationFailedEventArgs args)
    {
      // reverse endianness 
      var errorCode = BitConverter.GetBytes((uint)args.ErrorCode).Reverse().ToArray();
      if (args.StatusType == ThresholdStatusType.CLEAR_PMCU_STATUS)
      {
        RemoveErrorCodeFromErrorData(errorCode, _pmcuErrorData);
      }
      else if (args.StatusType == ThresholdStatusType.PMCU_STATUS)// must be pmcu error
      {
        // error doesn't get reset
        for (int i = 0; i < 4; i++)
        {
          _pmcuErrorData[i] = (byte)(errorCode[i] | _pmcuErrorData[i]);
        }
      }
      else if (args.StatusType == ThresholdStatusType.RESET_PMCU) 
      {
        // reset to default
        _pmcuErrorData = DEFAULT_PMCU_DATA;
      }
    }

    public override void Initialize()
    {
      base.Initialize();
      _pmcuStatusMessageConfig = new PmcuStatusMessageConfig();
      var loadConfig = _pmcuStatusMessageConfig.Parse(GetConfigurationNode(PMCU_STATUS_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _statesDictionary = _pmcuStatusMessageConfig.StateToMessageByteMap;
        // if the interval is zero/negative, default value is 100
        _messageUpdateTimer = new Timer(_pmcuStatusMessageConfig.Interval <= 0 ? 100 : _pmcuStatusMessageConfig.Interval);
        _messageUpdateTimer.Elapsed += PublishPmcuMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private void PublishPmcuMessage(object sender, ElapsedEventArgs e)
    {
      // bitwise OR to combine the two status messages byte arrays
      byte[] result = new byte[4];
      for (int i = 0; i < 4; i++)
      {
        result[i] = (byte)(MessageStateIdToHexValue(CurrentStateId, _statesDictionary)[i] | _pmcuErrorData[i]);
      }
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, PMCU_MESSAGE_ID), Length = 4, Data = result }
      };

      PublishCanBusMessage(message);
    }

  }
}
