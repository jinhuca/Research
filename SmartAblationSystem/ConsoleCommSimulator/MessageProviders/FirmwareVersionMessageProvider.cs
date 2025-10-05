using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class FirmwareVersionMessageProvider : MessageProviderBase
  {
    private static string CATHETER_CONFIG_NODE_ID = "FirmwareConfig";
    private static uint CMCU_FIRMWARE_MESSAGE_ID = 8;
    private static uint PMCU_FIRMWARE_MESSAGE_ID = 48;
    private static uint CATHETER_FIRMWARE_MESSAGE_ID = 56;
    private static uint REPEATER_ICB_FIRMWARE_MESSAGE_ID = 11;
    private static uint REMOTE_FIRMWARE_MESSAGE_ID = 24;
    private static uint INCOMING_MSG_NODE_ID = 2;
    //private static uint INCOMING_MSG_NODE_ID3 = 3;
    //
    private FirmwareVersionMessageConfig _cathetherFirmwareMessageConfig;

    public FirmwareVersionMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      // we have to add new nodes for pmcu message and future messages
      // moved to compute right before message sending so it's accurate
      //NodeId = ConvertElementToNodeOne(CMCU_FIRMWARE_MESSAGE_ID);
    }

    public override void Initialize()
    {
      base.Initialize();

      _cathetherFirmwareMessageConfig = new FirmwareVersionMessageConfig();
      var loadConfig = _cathetherFirmwareMessageConfig.Parse(GetConfigurationNode(CATHETER_CONFIG_NODE_ID));
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
      // check if received a message elemen 8 with empty firmware, then reply a message elem 8 with the correct firmware like an answering system, ditto for 48
      base.UpdateParameters(parameters);
      var messageElements = SplitCanBusMessageId(parameters.MessageId);
      // check which message was received

      if (messageElements.Item1 == INCOMING_MSG_NODE_ID && messageElements.Item2 == CMCU_FIRMWARE_MESSAGE_ID)
      {
        NodeId = ConvertElementToNodeOne(CMCU_FIRMWARE_MESSAGE_ID);
        PublishCMCUFirmwareMessage();
      } 
      else if (messageElements.Item1 == INCOMING_MSG_NODE_ID && messageElements.Item2 == PMCU_FIRMWARE_MESSAGE_ID)
      {
        NodeId = ConvertElementToNodeOne(PMCU_FIRMWARE_MESSAGE_ID);
        PublishPMCUFirmwareMessage();
      }
      else if (messageElements.Item1 == INCOMING_MSG_NODE_ID && messageElements.Item2 == CATHETER_FIRMWARE_MESSAGE_ID)
      {
        NodeId = ConvertElementToNodeOne(CATHETER_FIRMWARE_MESSAGE_ID);
        PublishCatheterMessage();
      }
      else if (messageElements.Item1 == INCOMING_MSG_NODE_ID && messageElements.Item2 == REPEATER_ICB_FIRMWARE_MESSAGE_ID)
      {
        NodeId = ConvertElementToNodeTwo(REPEATER_ICB_FIRMWARE_MESSAGE_ID); // 3 
        //NodeId = 3;
        PublishRepeaterICBFirmwareMessage();
      }
      else if (messageElements.Item1 == INCOMING_MSG_NODE_ID && messageElements.Item2 == REMOTE_FIRMWARE_MESSAGE_ID)
      {
        NodeId = ConvertElementToNodeTwo(REMOTE_FIRMWARE_MESSAGE_ID); // 3
        //NodeId = 3;
        PublishRemoteFirmwareMessage();
      }
    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      //_messageUpdateTimer?.Stop();
    }

    private void PublishCMCUFirmwareMessage()
    {

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreatePriority0MessageId(CurrentStateId, CMCU_FIRMWARE_MESSAGE_ID), Length = 6, Data = _cathetherFirmwareMessageConfig.CMCUFirmwareData }
      };

      PublishCanBusMessage(message);
    }
    private void PublishPMCUFirmwareMessage()
    {

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreatePriority0MessageId(CurrentStateId, PMCU_FIRMWARE_MESSAGE_ID), Length = 4, Data = _cathetherFirmwareMessageConfig.PMCUFirmwareData }
      };

      PublishCanBusMessage(message);
    }
    private void PublishCatheterMessage()
    {

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreatePriority0MessageId(CurrentStateId, CATHETER_FIRMWARE_MESSAGE_ID), Length = 2, Data = _cathetherFirmwareMessageConfig.CatheterFirmwareData }
      };

      PublishCanBusMessage(message);
    }
    private void PublishRepeaterICBFirmwareMessage()
    {

      var message = new CanBusMessage()
      {
        // message 11 and 24 are register type
        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreatePriority0MessageIdTwo(CurrentStateId, REPEATER_ICB_FIRMWARE_MESSAGE_ID, 1), Length = 8, Data = _cathetherFirmwareMessageConfig.RepeaterICBFirmwareData }
      };

      PublishCanBusMessage(message);
    }
    private void PublishRemoteFirmwareMessage()
    {

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreatePriority0MessageIdTwo(CurrentStateId, REMOTE_FIRMWARE_MESSAGE_ID, 1), Length = 8, Data = _cathetherFirmwareMessageConfig.RemoteFirmwareData }
      };

      PublishCanBusMessage(message);
    }
  }
}
