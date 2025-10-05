
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

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class TSMessageProviderTests : MessageProviderTestBase
  {
    private static string TS_CONFIG_NODE_ID = "TSConfig";
    private static uint TS_MESSAGE_ID = 3;
    private static readonly string _TSStatusConfiguration = "<CanBusSimulatorConfiguration><TSConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<Settings>"+
      "<TSstate state = \"IDLE\">"+
				"<TSSetting name = \"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
				"<TSSetting name = \"TN2O\" value=\"-38.0\"/>"+

			"</TSstate>"+
			"<TSstate state = \"READY\" >"+
        "<TSSetting name= \"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
        "<TSSetting name = \"TN2O\" value=\"-42.0\"/>" +

      "</TSstate>" +
			"<TSstate state = \"INFLATION\" >"+
        "<TSSetting name= \"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
        "<TSSetting name = \"TN2O\" value=\"-42.0\"/>" +

      "</TSstate>" +
			"<TSstate state = \"TRANSITION\" >"+
        "<TSSetting name= \"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
       "<TSSetting name = \"TN2O\" value=\"-41.0\"/>" +

      "</TSstate>" +
			"<TSstate state = \"ABLATION\" >"+
        "<TSSetting name= \"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
      "<TSSetting name = \"TN2O\" value=\"-38.0\"/>" +

      "</TSstate>" +
			"<TSstate state = \"THAWING\" >"+
        "<TSSetting name=\"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"27.0\"/>"+
      "<TSSetting name = \"TN2O\" value=\"-37.0\"/>" +

      "</TSstate>" +
			"<TSstate state = \"EXCEPTION\" >"+
        "<TSSetting name=\"TS1\" value=\"0\"/>"+
				"<TSSetting name = \"CMCUCJ\" value=\"0\"/>"+
      "<TSSetting name = \"TN2O\" value=\"-40\"/>" +

      "</TSstate>" +
		"</Settings>"+
	  "</TSConfig></CanBusSimulatorConfiguration>";

    private static StateToTSValue _expectedIdle = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -38.0,

    };
    private static StateToTSValue _expectedReady = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -42.0,

    };
    private static StateToTSValue _expectedInflation = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -42.0,

    };
    private static StateToTSValue _expectedTransition = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -41.0,

    };
    private static StateToTSValue _expectedAblation = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -38.0,

    };
    private static StateToTSValue _expectedThawing = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 27.0,
      TN2O = -37.0,

    };
    private static StateToTSValue _expectedException = new StateToTSValue()
    {
      TS1 = 0,
      CMCUCJ = 0,
      TN2O = -40,

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

    private TSMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock; 
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;
    private Mock<TS1ThresholdValidation> _ts1ThresholdValidationMock;
    private int _expectedTSInterval = 50;

    private IDictionary<string, StateToTSValue> _expectedStateStringToTSMap = 
      new Dictionary<string, StateToTSValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToTSValue> _expectedStateToTS =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToTSValue>()
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
    public void TSConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_TSStatusConfiguration);
      var TSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+TS_CONFIG_NODE_ID); 

      var config = new TSMessageConfig();
      var loaded = config.Parse(TSStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToTSMap.Count);
      Assert.AreEqual(_expectedTSInterval, config.Interval);
      foreach (var keyValuePair in config.StateToTSMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToTSMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.TS1, keyValuePair.Value.TS1);
        Assert.AreEqual(expectedValue.CMCUCJ, keyValuePair.Value.CMCUCJ);
        Assert.AreEqual(expectedValue.TN2O, keyValuePair.Value.TN2O);

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
      doc.LoadXml(_TSStatusConfiguration);

      var TSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + TS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(TSStatusConfigNode);    

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

      _ts1ThresholdValidationMock = new Mock<TS1ThresholdValidation>(EventAggregatorMock.Object);
      _provider = new TSMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _ts1ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(1000).Wait(); // wait until it stabilizes 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 1, 0, TS_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(6, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      Assert.AreEqual(_expectedStateToTS[currentState].TS1, CanBusMessageConverter.ConverteNegativDecimalData(data, 0));
      Assert.AreEqual(_expectedStateToTS[currentState].CMCUCJ, CanBusMessageConverter.ConverteNegativDecimalData(data, 2));
      Assert.AreEqual(_expectedStateToTS[currentState].TN2O, CanBusMessageConverter.ConverteNegativDecimalData(data, 4));
    }
  }
}
