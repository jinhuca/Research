
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.MessageProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class CmcuStatusMessageProviderTests : MessageProviderTestBase
  {
    private static string CMCU_STATUS_CONFIG_NODE_ID = "CMCUStatusConfig";
    private static uint SUBCOOLER_ERROR = 0x00040000;
    private static readonly string _cmcuStatusConfiguration = "<CanBusSimulatorConfiguration><CMCUStatusConfig>\r\n" +
      "<UpdateInterval>100</UpdateInterval>" +
      "<Settings>" +
      "<Setting state=\"IDLE\" value=\"0x0A000000\"/>" +
      "<Setting state=\"READY\" value=\"0x000A0000\"/>" +
      "<Setting state=\"INFLATION\" value=\"0x00000A00\"/>" +
      "<Setting state=\"TRANSITION\" value=\"0x0000000A\"/>" +
      "<Setting state=\"ABLATION\" value=\"0x0B000000\"/>" +
      "<Setting state=\"THAWING\" value=\"0x0A0C0000\"/>" +
      "<Setting state=\"EXCEPTION\" value=\"0x0A0D0000\"/>" +
      "</Settings>" +
      "</CMCUStatusConfig></CanBusSimulatorConfiguration>";

    private IDictionary<string, byte[]> _expectedValueMap = new Dictionary<string, byte[]>() {
    { "IDLE", new byte[]{0x0a, 0,0,0} },
    { "READY", new byte[]{0, 0x0a, 0,0} },
    { "INFLATION", new byte[]{0, 0, 0x0a, 0 } },
    { "TRANSITION", new byte[]{0, 0, 0, 0x0a} },
    { "ABLATION", new byte[]{0x0b, 0,0,0} },
    { "THAWING", new byte[]{0x0a, 0x0c,0,0} },
    { "EXCEPTION", new byte[]{0x0a, 0x0d,0,0} },
    };
    private IDictionary<string, byte[]> _expectedErrorValueMap = new Dictionary<string, byte[]>() {
    { "IDLE", new byte[]{0x0a, 04,0,0} },
    { "READY", new byte[]{0, 0x0e, 0,0} },
    { "INFLATION", new byte[]{0, 04, 0x0a, 0 } },
    { "TRANSITION", new byte[]{0, 04, 0, 0x0a} },
    { "ABLATION", new byte[]{0x0b, 04,0,0} },
    { "THAWING", new byte[]{0x0a, 0x0c,0,0} },
    { "EXCEPTION", new byte[]{0x0a, 0x0d,0,0} },
    };

    private CmcuStatusMessageProvider _provider;

    [TestMethod]
    public void CmcuConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_cmcuStatusConfiguration);
      var cmcuStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+CMCU_STATUS_CONFIG_NODE_ID); 

      var config = new CmcuStatusMessageConfig();
      var loaded = config.Parse(cmcuStatusConfigNode); 

      Assert.IsTrue(loaded); 
      Assert.AreEqual(100, config.Interval);
      Assert.AreEqual(7, config.StateToMessageByteMap.Count);

      var expectedKeyList = _expectedValueMap.Keys;  
      // {"IDLE", "READY", "INFLATION", "TRANSITION", "ABLATION", "THAWING", "EXCEPTION" };
      
      foreach (var keyValuePair in config.StateToMessageByteMap)
      {
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        Assert.IsTrue(keyValuePair.Value.Length == 4);

        var expectedValue = _expectedValueMap[keyValuePair.Key]; 
        Assert.AreEqual(expectedValue[0], keyValuePair.Value[0]);
        Assert.AreEqual(expectedValue[1], keyValuePair.Value[1]);
        Assert.AreEqual(expectedValue[2], keyValuePair.Value[2]);
        Assert.AreEqual(expectedValue[3], keyValuePair.Value[3]);
      }

    }

    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedErrorValueMap, true);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_EXCEPTION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedValueMap);
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedErrorValueMap, true);
    }

    private void TestProviderWithState(CanBusMessageDefinition.MessageStateId currentState, IDictionary<string, byte[]> expectedBytes, bool hasError=false)
    {

      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_cmcuStatusConfiguration);

      var cmcuStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + CMCU_STATUS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(cmcuStatusConfigNode);

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction = null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),
          It.IsAny<bool>(),
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);

      // Setup threshold validation failed
      Action<ThresholdValidationFailedEventArgs> handleThresholdValidationFailedEvent = null;

      // Setup threshold validation failed 
      //ThresholdValidationFailedEventArgs thresholdValidationFailedEventArgs = null;
      ThresholdValidationFailedEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ThresholdValidationFailedEventArgs>>(),
          It.IsAny<ThreadOption>(),
          It.IsAny<bool>(),
          It.IsAny<Predicate<ThresholdValidationFailedEventArgs>>()))
        .Callback<Action<ThresholdValidationFailedEventArgs>, ThreadOption, bool, Predicate<ThresholdValidationFailedEventArgs>>(
          (action, _, __, ___) => handleThresholdValidationFailedEvent = action);

      List<CanBusMessage> canbusMessage = new List<CanBusMessage>();
      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage.Add(m));

      _provider = new CmcuStatusMessageProvider(EventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();

      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });
      if (hasError)
      {
        handleThresholdValidationFailedEvent?.Invoke(new ThresholdValidationFailedEventArgs(ThresholdStatusType.CMCU_STATUS, SUBCOOLER_ERROR));
      }
      // The Update interval is set as 100ms, we wait 100ms, and verify that the event was Published 
      Task.Delay(150).Wait();
      _provider.Dispose();

      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);
      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 1, 1, 35);
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(4, canbusMessage.Last().CanBusEventArgs.Length);

      var expectedDataValue = expectedBytes[StateIdToStringMap[currentState]];
      Assert.AreEqual(expectedDataValue[0], canbusMessage.Last().CanBusEventArgs.Data[0]);
      Assert.AreEqual(expectedDataValue[1], canbusMessage.Last().CanBusEventArgs.Data[1]);
      Assert.AreEqual(expectedDataValue[2], canbusMessage.Last().CanBusEventArgs.Data[2]);
      Assert.AreEqual(expectedDataValue[3], canbusMessage.Last().CanBusEventArgs.Data[3]);
    }
  }
}
