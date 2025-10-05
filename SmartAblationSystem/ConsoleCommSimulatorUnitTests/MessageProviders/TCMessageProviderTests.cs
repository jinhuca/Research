
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Validation;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.MessageProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using SmartAblationSystem.Helpers;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class TCMessageProviderTests : MessageProviderTestBase
  {
    private static string TC_CONFIG_NODE_ID = "TCConfig";
    private static uint TC_MESSAGE_ID = 40;
    private bool DEFLATE_AFTER_THAW = false; // currently false in the MP code
    private static readonly string _TCDataConfig = "<TCSimulatorConfiguration>\r\n" +
  "<TCDataConfig name=\"default\">"+
    "<TCDataInterval>1000</TCDataInterval>"+

    "<TCData>"+
      "<Data>34,34,34,33,30,25,20,15,9,4,0,-5,-10,-14,-17,-15,-13,-14,-16,-19,-24,-29,-33,-37,-39,-43,-44,-45,-47,-48,-49,-50,-50,-51,-51,-51,-52,-52,-53,-53,-53,-53,-54,-54,-54,-55,-55,-55,-55,-55,-56,-56,-56,-56,-56,-56,-57,-57,-57,-57,-58,-57,-57,-57,-57,-57,-57,-58,-58,-58,-58,-58,-58,-58,-58,-58,-58,-59,-59,-60,-59,-60,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-59,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-60,-61,-60,-61,-61,-61,-61,-60</Data>"+
    "</TCData>"+

    "<TCFITData>"+
      "<Data>33,33,33,32,31,27,23,18,13,7,1,-4,-9,-13,-18,-16,-18,-22,-26,-31,-35,-39,-42,-45,-46,-48,-50,-51,-52,-52,-53,-54,-54,-54,-55,-55,-56,-56,-57,-58,-58,-58,-58,-58,-58,-59,-59,-59,-60,-60,-60,-61,-61,-61,-61,-61,-61,-62,-61,-61,-61,-62,-62,-62,-62,-63,-63,-63,-64,-64,-65,-64,-64,-64,-65,-65,-65,-67,-66,-66,-66,-66,-66,-65</Data>"+
    "</TCFITData>"+
  "</TCDataConfig></TCSimulatorConfiguration>";
    // don't forget to change _expectedTCTime if you change updateTIme below
    private static readonly string _TCStatusConfiguration = "<CanBusSimulatorConfiguration><TCConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
          "<TCThawingData >"+
      "<TCDataPhase name=\"_initial\" value=\"-1,0,0,1,0,1,0.5,0.5,1,1.5,2,2,2.5,3,3.5,1,4,4,3,3,2,3,2,2,1,0.5,0,0.5,1,2,2,1.5,1.5,1.5,1.5,1.5\"/>"+
      "<TCDataPhase name = \"_plateau\" value=" +
      "\"2,2,1.5,1.3,1,1,0,1,0,0,0,1,0,0,0,0.5,0,0,0,0,0.5,0,0,0,0,0.5,0,0.5,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,1,1,0,1,0,2\"/>"+
      "<TCDataPhase name = \"_end\" value=\"1,0.5,0,0.5,0,0.5,0,0.5,0,0.5,0,0,0.5,0,0,0,0,0,0,1,0,0,0,1,0,1,7,0,1,0,1,1,0,2,3\"/>"+
    "</TCThawingData>"+
      "<Settings>"+
      "<TCstate state = \"IDLE\">"+
				"<TCSetting name = \"TC1\" value=\"28.0\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
				"<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>"+

			"</TCstate>"+
			"<TCstate state = \"READY\" >"+
        "<TCSetting name= \"TC1\" value=\"36.0\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
        "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
			"<TCstate state = \"INFLATION\" >"+
        "<TCSetting name= \"TC1\" value=\"35.0\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
        "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
			"<TCstate state = \"TRANSITION\" >"+
        "<TCSetting name= \"TC1\" value=\"-7.0\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
       "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
			"<TCstate state = \"ABLATION\" >"+
        "<TCSetting name= \"TC1\" value=\"-34.5\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
      "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
			"<TCstate state = \"THAWING\" >"+
        "<TCSetting name=\"TC1\" value=\"20.0\"/>"+
				"<TCSetting name = \"TC2\" value=\"40.0\"/>"+
      "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
			"<TCstate state = \"EXCEPTION\" >"+
        "<TCSetting name=\"TC1\" value=\"36.0\"/>" +
        "<TCSetting name = \"TC2\" value=\"40.0\"/>" +
      "<TCSetting name = \"PMCUCJ\" value=\"26.0\"/>" +

      "</TCstate>" +
		"</Settings>"+
	  "</TCConfig></CanBusSimulatorConfiguration>";

    private static StateToTCValue _expectedIdle = new StateToTCValue()
    {
      TC1 = 28.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedReady = new StateToTCValue()
    {
      TC1 = 36.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedInflation = new StateToTCValue()
    {
      TC1 = 35.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedTransition = new StateToTCValue()
    {
      TC1 = -7.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedAblation = new StateToTCValue()
    {
      TC1 = -34.5,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedThawing = new StateToTCValue()
    {
      TC1 = 20.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };
    private static StateToTCValue _expectedException = new StateToTCValue()
    {
      TC1 = 36.0,
      TC2 = 40.0,
      PMCUCJ = 26.0,

    };

    private IDictionary<CanBusMessageDefinition.MessageStateId, string> _stateIdToStringMap =
      new Dictionary<CanBusMessageDefinition.MessageStateId, string>()
      {
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, "IDLE"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, "READY"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, "INFLATION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, "TRANSITION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, "ABLATION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, "THAWING"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, "EXCEPTION"}
      };
    private Mock<TC1ThresholdValidation> _tc1ThresholdValidationMock;
    private TCMessageProvider _provider;

    private int _expectedTCInterval = 50;

    private IDictionary<string, StateToTCValue> _expectedStateStringToTCMap = 
      new Dictionary<string, StateToTCValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToTCValue> _expectedStateToTC =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToTCValue>()
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
    public void TCConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_TCStatusConfiguration);
      var TCStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+TC_CONFIG_NODE_ID);

      var tcDataDoc = new XmlDocument();
      tcDataDoc.LoadXml(_TCDataConfig);

      var config = new TCMessageConfig();
      var loaded = config.Parse(TCStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToTCMap.Count);
      Assert.AreEqual(_expectedTCInterval, config.Interval);
      foreach (var keyValuePair in config.StateToTCMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToTCMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.TC1, keyValuePair.Value.TC1);
        Assert.AreEqual(expectedValue.TC2, keyValuePair.Value.TC2);
        Assert.AreEqual(expectedValue.PMCUCJ, keyValuePair.Value.PMCUCJ);
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
      // no more path in exception
      //TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION);
    }
    private void TestProviderWithState(CanBusMessageDefinition.MessageStateId currentState)
    {
      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_TCStatusConfiguration);

      var TCStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + TC_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(TCStatusConfigNode);
      _tc1ThresholdValidationMock = new Mock<TC1ThresholdValidation>(EventAggregatorMock.Object);
      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction=null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),        
          It.IsAny<bool>(),   
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);
      
      List<CanBusMessage> canbusMessage = new List<CanBusMessage>();

      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage.Add(m));

      _provider = new TCMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _tc1ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(6500).Wait(); // wait until it stabilizes 
      _provider.Dispose();
      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 0, 0, TC_MESSAGE_ID); // pmcu is 0, type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(6, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      // tests that the current TC value is higher or equal to the thawing value (after it reach 20C it should go 36C)
      if (currentState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING ||
        currentState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION ||
        currentState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION ||
        currentState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION
        )
      {
        Assert.IsTrue(_expectedStateToTC[currentState].TC1 <= CanBusMessageConverter.ConverteNegativDecimalData(data, 0));
      } 
      else
      {
        // normally they stay equal within a small margin due to the randomness 
        Assert.IsTrue(Math.Abs(CanBusMessageConverter.ConverteNegativDecimalData(data, 0) - _expectedStateToTC[currentState].TC1) <= 1);

      }
      Assert.IsTrue(Math.Abs(CanBusMessageConverter.ConverteNegativDecimalData(data, 2) - _expectedStateToTC[currentState].TC2) <= 1);
      Assert.IsTrue(Math.Abs(CanBusMessageConverter.ConverteNegativDecimalData(data, 4) - _expectedStateToTC[currentState].PMCUCJ) <= 1);
    }
  }
}
