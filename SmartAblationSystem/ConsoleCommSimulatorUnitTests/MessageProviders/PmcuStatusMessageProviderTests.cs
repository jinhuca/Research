
using System;
using System.Collections.Generic;
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
  public class PmcuStatusMessageProviderTests : MessageProviderTestBase
  {
    private static string PMCU_STATUS_CONFIG_NODE_ID = "PMCUStatusConfig";
    private static uint IBP_HIGH_ERROR = 0x00000004;
    private static uint PMCU_MESSAGE_ID = 49;
    private static readonly string _pmcuStatusConfiguration = "<CanBusSimulatorConfiguration><PMCUStatusConfig>\r\n" +
      "<UpdateInterval>100</UpdateInterval>" +
      "<Settings>" +
      "<Setting state=\"IDLE\" value=\"0x09000000\"/>" +
      "<Setting state=\"READY\" value=\"0x00090000\"/>" +
      "<Setting state=\"INFLATION\" value=\"0x00000900\"/>" +
      "<Setting state=\"TRANSITION\" value=\"0x00000009\"/>" +
      "<Setting state=\"ABLATION\" value=\"0x0B000000\"/>" +
      "<Setting state=\"THAWING\" value=\"0x090C0000\"/>" +
      "<Setting state=\"EXCEPTION\" value=\"0x090D0000\"/>" +
      "</Settings>" +
      "</PMCUStatusConfig></CanBusSimulatorConfiguration>";

    private IDictionary<string, byte[]> _expectedValueMap = new Dictionary<string, byte[]>() {
    { "IDLE", new byte[]{0x09, 0,0,0} },
    { "READY", new byte[]{0, 0x09, 0,0} },
    { "INFLATION", new byte[]{0, 0, 0x09, 0 } },
    { "TRANSITION", new byte[]{0, 0, 0, 0x09} },
    { "ABLATION", new byte[]{0x0b, 0,0,0} },
    { "THAWING", new byte[]{0x09, 0x0c,0,0} },
    { "EXCEPTION", new byte[]{0x09, 0x0d,0,0} },
    };
    private IDictionary<string, byte[]> _expectedErrorValueMap = new Dictionary<string, byte[]>() {
    { "IDLE", new byte[]{0x09, 0,0,04} }, // expected valuemap with the IBP_HIGH_ERROR included
    { "READY", new byte[]{0, 0x09, 0,04} },
    { "INFLATION", new byte[]{0, 0, 0x09, 04 } },
    { "TRANSITION", new byte[]{0, 0, 0, 0x0c} },
    { "ABLATION", new byte[]{0x0b, 0,0,04} },
    { "THAWING", new byte[]{0x09, 0x0c,0,0} },
    { "EXCEPTION", new byte[]{0x09, 0x0d,0,0} },
    };

    private PmcuStatusMessageProvider _PMCUprovider;

    [TestMethod]
    public void PmcuConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_pmcuStatusConfiguration);
      var pmcuStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+PMCU_STATUS_CONFIG_NODE_ID); 

      var config = new PmcuStatusMessageConfig();
      var loaded = config.Parse(pmcuStatusConfigNode); 

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
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedErrorValueMap, true);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedErrorValueMap, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_EXCEPTION()
    {
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedValueMap);
      TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedErrorValueMap, true);
    }

    private void TestPMCUProviderWithState(CanBusMessageDefinition.MessageStateId currentState, IDictionary<string, byte[]> expectedBytes, bool hasError = false)
    {
      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_pmcuStatusConfiguration);

      var pmcuStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + PMCU_STATUS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(pmcuStatusConfigNode);

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction=null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),        
          It.IsAny<bool>(),   
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);
      // Setup threshold validation failed 
      Action<ThresholdValidationFailedEventArgs> handleThresholdValidationFailedEvent = null;
      ThresholdValidationFailedEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ThresholdValidationFailedEventArgs>>(),
          It.IsAny<ThreadOption>(),
          It.IsAny<bool>(),
          It.IsAny<Predicate<ThresholdValidationFailedEventArgs>>()))
        .Callback<Action<ThresholdValidationFailedEventArgs>, ThreadOption, bool, Predicate<ThresholdValidationFailedEventArgs>>(
          (action, _, __, ___) => handleThresholdValidationFailedEvent = action);

      CanBusMessage canbusMessage = null;
      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage = m); 

      _PMCUprovider = new PmcuStatusMessageProvider(EventAggregatorMock.Object, configurationMoq.Object);
      _PMCUprovider.Initialize();
      // initialized as null, if systemupdate did not work, will stay null
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage(){State = currentState});
      if (hasError)
      {
        handleThresholdValidationFailedEvent?.Invoke(new ThresholdValidationFailedEventArgs(ThresholdStatusType.CMCU_STATUS, IBP_HIGH_ERROR));
      }
      // The Update interval is set as 100ms, we wait 100ms, and verify that the event was Published 
      Task.Delay(150).Wait();
      // dispose before the next canbus message is sent
      _PMCUprovider.Dispose();

      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage);
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Id);
      var messageId = CreateMessageId(currentState, 0, 1, PMCU_MESSAGE_ID); // 49 is pmcu message id , node is 0 because pmcu
      Assert.AreEqual(messageId, canbusMessage.CanBusEventArgs.Id);
      Assert.AreEqual(4, canbusMessage.CanBusEventArgs.Length);
      
      var expectedDataValue = _expectedValueMap[StateIdToStringMap[currentState]];
      Assert.AreEqual(expectedDataValue[0], canbusMessage.CanBusEventArgs.Data[0]);
      Assert.AreEqual(expectedDataValue[1], canbusMessage.CanBusEventArgs.Data[1]);
      Assert.AreEqual(expectedDataValue[2], canbusMessage.CanBusEventArgs.Data[2]);
      Assert.AreEqual(expectedDataValue[3], canbusMessage.CanBusEventArgs.Data[3]);
    }
  }
}
