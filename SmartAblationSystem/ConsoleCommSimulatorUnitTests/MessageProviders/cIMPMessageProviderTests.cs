
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
using SmartAblationSystem.Helpers;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class cIMPMessageProviderTests : MessageProviderTestBase
  {
    private static string cIMP_CONFIG_NODE_ID = "cIMPConfig";
    private static uint cIMP_MESSAGE_ID = 42;
    private static readonly string _cIMPStatusConfiguration = "<CanBusSimulatorConfiguration><cIMPConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<Settings>"+
      "<cIMPstate state = \"IDLE\" >" +
        "<cIMPSetting name = \"IMValue\" value=\"20.0\"/>" +
				"<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>"+

			"</cIMPstate>"+

			"<cIMPstate state = \"READY\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"20.0\"/>"+
        "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

			"<cIMPstate state = \"INFLATION\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"20.0\"/>"+
        "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

			"<cIMPstate state = \"TRANSITION\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"20.0\"/>"+
       "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

			"<cIMPstate state = \"ABLATION\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"19.0\"/>"+
      "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

			"<cIMPstate state = \"THAWING\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"20.0\"/>"+
      "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

			"<cIMPstate state = \"EXCEPTION\" >"+
				"<cIMPSetting name = \"IMValue\" value=\"0\"/>"+
      "<cIMPSetting name = \"BloodDetectionType\" value=\"0\"/>" +

      "</cIMPstate>" +

		"</Settings>"+
	  "</cIMPConfig></CanBusSimulatorConfiguration>";

    private static StateTocIMPValue _expectedIdle = new StateTocIMPValue()
    {
      IMValue = 20.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedReady = new StateTocIMPValue()
    {
      IMValue = 20.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedInflation = new StateTocIMPValue()
    {
      IMValue = 20.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedTransition = new StateTocIMPValue()
    {
      IMValue = 20.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedAblation = new StateTocIMPValue()
    {

      IMValue = 19.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedThawing = new StateTocIMPValue()
    {
      IMValue = 20.0,
      BloodDetectionType = 0,

    };
    private static StateTocIMPValue _expectedException = new StateTocIMPValue()
    {
      IMValue = 0,
      BloodDetectionType = 0,

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

    private cIMPMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock; 
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;

    private int _expectedcIMPInterval = 50;

    private IDictionary<string, StateTocIMPValue> _expectedStateStringTocIMPMap = 
      new Dictionary<string, StateTocIMPValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateTocIMPValue> _expectedStateTocIMP =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateTocIMPValue>()
      {
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedIdle},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedReady},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedInflation},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, _expectedTransition},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, _expectedAblation},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedThawing},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedException}
      };
    [TestInitialize]
    public void Initialize()
    {
      // mock an event, if it gets called, return event.object
      _eventAggregatorMock = new Mock<IEventAggregator>();
      _canBusUpdateEventMock = new Mock<CanBusMessageUpdateEvent>();

      _eventAggregatorMock
        .Setup(x => x.GetEvent<CanBusMessageUpdateEvent>())
        .Returns(_canBusUpdateEventMock.Object);

      _systemStateUpdateEventMock = new Mock<SystemStateUpdateEvent>();
      _eventAggregatorMock
        .Setup(x => x.GetEvent<SystemStateUpdateEvent>())
        .Returns(_systemStateUpdateEventMock.Object);
    }

    [TestMethod]
    public void cIMPConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_cIMPStatusConfiguration);
      var cIMPStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+cIMP_CONFIG_NODE_ID); 

      var config = new cIMPMessageConfig();
      var loaded = config.Parse(cIMPStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.AreEqual(50, config.Interval);

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateTocIMPMap.Count);
      Assert.AreEqual(_expectedcIMPInterval, config.Interval);
      foreach (var keyValuePair in config.StateTocIMPMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringTocIMPMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.IMValue, keyValuePair.Value.IMValue);
        Assert.AreEqual(expectedValue.BloodDetectionType, keyValuePair.Value.BloodDetectionType);

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
      doc.LoadXml(_cIMPStatusConfiguration);

      var cIMPStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + cIMP_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(cIMPStatusConfigNode);    

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction=null;
      _systemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),        
          It.IsAny<bool>(),   
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);
      
      List<CanBusMessage> canbusMessage = new List<CanBusMessage>();

      _canBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage.Add(m));

      _provider = new cIMPMessageProvider(_eventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(3000).Wait(); // wait until it stabilizes 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      _canBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 0, 0, cIMP_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(6, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      // tests that the current cIMP value is higher or equal to the transition value
      Assert.AreEqual(_expectedStateTocIMP[currentState].BloodDetectionType, CanBusMessageConverter.ConverteDecimalData(data, 0));
      Assert.AreEqual(_expectedStateTocIMP[currentState].IMValue, CanBusMessageConverter.ConverteDecimalData(data, 4));
    }
  }
}
