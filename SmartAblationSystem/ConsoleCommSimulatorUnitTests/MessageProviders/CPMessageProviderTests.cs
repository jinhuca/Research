
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
  public class CPMessageProviderTests : MessageProviderTestBase
  {
    private static string CP_CONFIG_NODE_ID = "CPConfig";
    private static uint CP_MESSAGE_ID = 41;
    private static readonly string _CPStatusConfiguration = "<CanBusSimulatorConfiguration><CPConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<TargetCP>600.0</TargetCP>"+
      "<Settings>"+
      "<CPstate state = \"IDLE\">"+
				"<CPSetting name = \"CP1\" value=\"0.0\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
				"<CPSetting name = \"PIDDUTY\" value=\"0\"/>"+

			"</CPstate>"+
			"<CPstate state = \"READY\" >"+
        "<CPSetting name= \"CP1\" value=\"-13.0\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
        "<CPSetting name = \"PIDDUTY\" value=\"0\"/>" +

      "</CPstate>" +
			"<CPstate state = \"INFLATION\" >"+
        "<CPSetting name= \"CP1\" value=\"3.2\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
        "<CPSetting name = \"PIDDUTY\" value=\"31.6\"/>" +

      "</CPstate>" +
			"<CPstate state = \"TRANSITION\" >"+
        "<CPSetting name= \"CP1\" value=\"2.7\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
       "<CPSetting name = \"PIDDUTY\" value=\"51.8\"/>" +

      "</CPstate>" +
			"<CPstate state = \"ABLATION\" >"+
        "<CPSetting name= \"CP1\" value=\"2.6\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
      "<CPSetting name = \"PIDDUTY\" value=\"55.0\"/>" +

      "</CPstate>" +
			"<CPstate state = \"THAWING\" >"+
        "<CPSetting name=\"CP1\" value=\"2.5\"/>"+
				"<CPSetting name = \"CP2\" value=\"-13.2\"/>"+
				"<CPSetting name = \"CTIP\" value=\"2.0\"/>"+
      "<CPSetting name = \"PIDDUTY\" value=\"0\"/>" +

      "</CPstate>" +
			"<CPstate state = \"EXCEPTION\" >"+
        "<CPSetting name=\"CP1\" value=\"0\"/>"+
				"<CPSetting name = \"CP2\" value=\"0\"/>"+
				"<CPSetting name = \"CTIP\" value=\"0\"/>"+
      "<CPSetting name = \"PIDDUTY\" value=\"0\"/>" +

      "</CPstate>" +
		"</Settings>"+
	  "</CPConfig></CanBusSimulatorConfiguration>";

    private static StateToCPValue _expectedIdle = new StateToCPValue()
    {
      CP1 = 0.0,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 0,

    };
    private static StateToCPValue _expectedReady = new StateToCPValue()
    {
      CP1 = -13.0,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 0,

    };
    private static StateToCPValue _expectedInflation = new StateToCPValue()
    {
      CP1 = 3.2,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 31.6,

    };
    private static StateToCPValue _expectedTransition = new StateToCPValue()
    {
      CP1 = 2.7,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 51.8,

    };
    private static StateToCPValue _expectedAblation = new StateToCPValue()
    {
      CP1 = 2.6,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 55.0,

    };
    private static StateToCPValue _expectedThawing = new StateToCPValue()
    {
      CP1 = 2.5,
      CP2 = -13.2,
      CTIP = 2.0,
      PIDDUTY = 0,

    };
    private static StateToCPValue _expectedException = new StateToCPValue()
    {
      CP1 = 0,
      CP2 = 0,
      CTIP = 0,
      PIDDUTY = 0,

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

    private CPMessageProvider _provider;
    private Mock<CP1ThresholdValidation> _cp1ThresholdValidationMock;
    private Mock<CP2ThresholdValidation> _cp2ThresholdValidationMock;
    private int _expectedCPInterval = 50;

    private IDictionary<string, StateToCPValue> _expectedStateStringToCPMap = 
      new Dictionary<string, StateToCPValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToCPValue> _expectedStateToCP =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToCPValue>()
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
    public void CPConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_CPStatusConfiguration);
      var CPStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+CP_CONFIG_NODE_ID); 

      var config = new CPMessageConfig();
      var loaded = config.Parse(CPStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.AreEqual(50, config.Interval);
      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToCPMap.Count);
      Assert.AreEqual(_expectedCPInterval, config.Interval);
      foreach (var keyValuePair in config.StateToCPMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToCPMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.CP1, keyValuePair.Value.CP1);
        Assert.AreEqual(expectedValue.CP2, keyValuePair.Value.CP2);
        Assert.AreEqual(expectedValue.CTIP, keyValuePair.Value.CTIP);
        Assert.AreEqual(expectedValue.PIDDUTY, keyValuePair.Value.PIDDUTY);
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
      doc.LoadXml(_CPStatusConfiguration);

      var CPStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + CP_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(CPStatusConfigNode);    

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
      _cp1ThresholdValidationMock = new Mock<CP1ThresholdValidation>(EventAggregatorMock.Object);
      _cp2ThresholdValidationMock = new Mock<CP2ThresholdValidation>(EventAggregatorMock.Object);
      _provider = new CPMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _cp1ThresholdValidationMock.Object, _cp2ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(3000).Wait(); // wait until it stabilizes 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 0, 0, CP_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;

      Assert.AreEqual(_expectedStateToCP[currentState].CP1, CanBusMessageConverter.ConverteNegativDecimalData(data, 0));
      Assert.AreEqual(_expectedStateToCP[currentState].CP2, CanBusMessageConverter.ConverteNegativDecimalData(data, 2));
      Assert.AreEqual(_expectedStateToCP[currentState].CTIP, CanBusMessageConverter.ConverteNegativDecimalData(data, 4));
      Assert.AreEqual(_expectedStateToCP[currentState].PIDDUTY, CanBusMessageConverter.ConverteDecimalData(data, 6));
    }
  }
}
