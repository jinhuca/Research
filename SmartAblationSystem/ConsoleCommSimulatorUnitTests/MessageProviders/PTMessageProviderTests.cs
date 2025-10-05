
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
using ConsoleCommSimulator.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using SmartAblationSystem.Helpers;
using static Communication.CanBusMessageDefinition;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class PTMessageProviderTests : MessageProviderTestBase
  {
    private static string PT_CONFIG_NODE_ID = "PTConfig";
    private static uint PT_MESSAGE_ID = 0;
    private static uint THRESHOLD_PT1_MESSAGE_ID = 17;
    private static readonly string _PTStatusConfiguration = "<CanBusSimulatorConfiguration><PTConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<Settings>"+
      "<Ptstate state = \"IDLE\">"+
				"<PTSetting name = \"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"0\"/>"+
				"<PTSetting name = \"PT3\" value=\"14.6\"/>"+
				"<PTSetting name = \"PT4\" value=\"0\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"READY\" >"+
        "<PTSetting name= \"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"1.4\"/>"+
				"<PTSetting name = \"PT3\" value=\"1.2\"/>"+
				"<PTSetting name = \"PT4\" value=\"1.0\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"INFLATION\" >"+
        "<PTSetting name= \"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"150.1\"/>"+
				"<PTSetting name = \"PT3\" value=\"17.8\"/>"+
				"<PTSetting name = \"PT4\" value=\"0\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"TRANSITION\" >"+
        "<PTSetting name= \"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"475.5\"/>"+
				"<PTSetting name = \"PT3\" value=\"14.6\"/>"+
				"<PTSetting name = \"PT4\" value=\"2.8\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"ABLATION\" >"+
        "<PTSetting name= \"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"528.5\"/>"+
				"<PTSetting name = \"PT3\" value=\"14.6\"/>"+
				"<PTSetting name = \"PT4\" value=\"4.3\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"THAWING\" >"+
        "<PTSetting name=\"PT1\" value=\"758.1\"/>"+
				"<PTSetting name = \"PT2\" value=\"9.4\"/>"+
				"<PTSetting name = \"PT3\" value=\"14.6\"/>"+
				"<PTSetting name = \"PT4\" value=\"4.3\"/>"+
			"</Ptstate>"+
			"<Ptstate state = \"EXCEPTION\" >"+
        "<PTSetting name=\"PT1\" value=\"758.1\"/>" +
        "<PTSetting name = \"PT2\" value=\"9.4\"/>" +
        "<PTSetting name = \"PT3\" value=\"14.6\"/>" +
        "<PTSetting name = \"PT4\" value=\"4.3\"/>" +
      "</Ptstate>" +
		"</Settings>"+
	  "</PTConfig></CanBusSimulatorConfiguration>";

    private static StateToPTValue _expectedIdle = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 0,
      PT3 = 14.6,
      PT4 = 0
    };
    private static StateToPTValue _expectedReady = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 1.4,
      PT3 = 1.2,
      PT4 = 1.0
    };
    private static StateToPTValue _expectedInflation = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 150.1,
      PT3 = 17.8,
      PT4 = 0
    };
    private static StateToPTValue _expectedTransition = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 475.5,
      PT3 = 14.6,
      PT4 = 2.8
    };
    private static StateToPTValue _expectedAblation = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 528.5,
      PT3 = 14.6,
      PT4 = 4.3
    };
    private static StateToPTValue _expectedThawing = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 9.4,
      PT3 = 14.6,
      PT4 = 4.3
    };
    private static StateToPTValue _expectedException = new StateToPTValue()
    {
      PT1 = 758.1,
      PT2 = 9.4,
      PT3 = 14.6,
      PT4 = 4.3
    };

    private PTMessageProvider _provider;
    //private IUpdater _rtrUpdaterMock;
    private Mock<PT1ThresholdValidation> _pT1ThresholdValidationMock;
    private Mock<PT2ThresholdValidation> _pT2ThresholdValidationMock;
    private Mock<PT3ThresholdValidation> _pT3ThresholdValidationMock;
    private Mock<PT4ThresholdValidation> _pT4ThresholdValidationMock;
    private int _expectedPTInterval = 50;
    private uint _verificationMessageId;
    private byte[] _verificationData =  new byte[8]{ 0x21 & 0xFF, 0x34 & 0xFF, 0x26, 0x16, 0x1A, 0x90, 0, 0 };

  private IDictionary<string, StateToPTValue> _expectedStateStringToPTMap = 
      new Dictionary<string, StateToPTValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToPTValue> _expectedStateToPT =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToPTValue>()
      {
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedIdle},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedReady},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedInflation},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedTransition},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedAblation},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedThawing},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedException}
      };

    [TestMethod]
    public void PTConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_PTStatusConfiguration);
      var PTStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+PT_CONFIG_NODE_ID);

      var config = new PTMessageConfig();
      var loaded = config.Parse(PTStatusConfigNode);
      var expectedKeyList = StateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToIntByteMap.Count);
      Assert.AreEqual(_expectedPTInterval, config.Interval);
      foreach (var keyValuePair in config.StateToIntByteMap)
      {
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToPTMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.PT1, keyValuePair.Value.PT1);
        Assert.AreEqual(expectedValue.PT2, keyValuePair.Value.PT2);
        Assert.AreEqual(expectedValue.PT3, keyValuePair.Value.PT3);
        Assert.AreEqual(expectedValue.PT4, keyValuePair.Value.PT4);
      }
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_EXCEPTION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION);
    }

    private void TestProviderWithState(CanBusMessageDefinition.MessageStateId currentState)
    {
      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_PTStatusConfiguration);

      var PTStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + PT_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(PTStatusConfigNode);

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction=null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),        
          It.IsAny<bool>(),   
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);

      // there will be many messages sent, we will store them
      List<CanBusMessage> canbusMessage = new List<CanBusMessage>();

      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage.Add(m));

      _pT1ThresholdValidationMock = new Mock<PT1ThresholdValidation>(EventAggregatorMock.Object);
      _pT2ThresholdValidationMock = new Mock<PT2ThresholdValidation>(EventAggregatorMock.Object);
      _pT3ThresholdValidationMock = new Mock<PT3ThresholdValidation>(EventAggregatorMock.Object);
      _pT4ThresholdValidationMock = new Mock<PT4ThresholdValidation>(EventAggregatorMock.Object);

      _provider = new PTMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _pT1ThresholdValidationMock.Object, _pT2ThresholdValidationMock.Object, _pT3ThresholdValidationMock.Object, _pT4ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);
       
      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      Task.Delay(3000).Wait(); // wait until it stabilizes 

      _provider.Dispose();

      Assert.IsNotNull(canbusMessage.Last());


      // verify validate thresholds has been called
      _pT1ThresholdValidationMock.Verify(x => x.ValidateThresholds(
          It.IsAny<double>(),
          It.Is<MessageStateId>(arg => arg == currentState)
      ), Times.AtLeastOnce);
      _pT2ThresholdValidationMock.Verify(x => x.ValidateThresholds(
          It.IsAny<double>(),
          It.Is<MessageStateId>(arg => arg == currentState)
      ), Times.AtLeastOnce);
      _pT3ThresholdValidationMock.Verify(x => x.ValidateThresholds(
          It.IsAny<double>(),
          It.Is<MessageStateId>(arg => arg == currentState)
      ), Times.AtLeastOnce);
      _pT4ThresholdValidationMock.Verify(x => x.ValidateThresholds(
          It.IsAny<double>(),
          It.Is<MessageStateId>(arg => arg == currentState)
      ), Times.AtLeastOnce);
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 1, 0, PT_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      //ConverteDecimalData returns a double like 14.6 psi which is 146 / 10.0, so we * 10 to get the original value
      if (currentState == MessageStateId.CAN_ID_STATE_EXCEPTION)
      {
        // should be between the 2 values
        Assert.IsTrue(_expectedStateToPT[currentState].PT1 <= CanBusMessageConverter.ConverteDecimalData(data, 0));
        Assert.IsTrue(CanBusMessageConverter.ConverteDecimalData(data, 0) <= _expectedIdle.PT1);
      } 
      else
      {
        Assert.AreEqual(_expectedStateToPT[currentState].PT1, CanBusMessageConverter.ConverteDecimalData(data, 0));
      }
      
      Assert.AreEqual(_expectedStateToPT[currentState ].PT2, CanBusMessageConverter.ConverteDecimalData(data, 2) ); 
      Assert.AreEqual(_expectedStateToPT[currentState ].PT3, CanBusMessageConverter.ConverteDecimalData(data, 4) ); 
      Assert.AreEqual(_expectedStateToPT[currentState ].PT4, CanBusMessageConverter.ConverteDecimalData(data, 6) ); 

      // TODO:: check if current PT was injected
    }
  }
}
