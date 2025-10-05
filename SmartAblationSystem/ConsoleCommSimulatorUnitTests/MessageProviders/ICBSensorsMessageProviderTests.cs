
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
  public class ICBSensorsMessageProviderTests : MessageProviderTestBase
  {
    private static string ICB_CONFIG_NODE_ID = "ICBConfig";
    private static uint ICB_MESSAGE_ID = 7; // on canbus2
    private static readonly string _ICBStatusConfiguration = "<CanBusSimulatorConfiguration><ICBConfig>\r\n" +
      "<ECGUpdateInterval>40</ECGUpdateInterval>"+

    "<Settings>"+

      "<ICBstate state =\"IDLE\">"+
        "<ICBSetting name = \"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"25\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"READY\"> "+

        "<ICBSetting name=\"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"26\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"INFLATION\">"+

        "<ICBSetting name=\"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"27\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"TRANSITION\"> "+

        "<ICBSetting name=\"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"26\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"ABLATION\"> "+

        "<ICBSetting name=\"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"25\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"THAWING\"> "+

        "<ICBSetting name=\"ECG12\" value=\"36.09\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"24\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"2.0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90.0\"/>"+

			"</ICBstate>"+
			"<ICBstate state = \"EXCEPTION\" > "+

        "<ICBSetting name=\"ECG12\" value=\"0\"/>"+
				"<ICBSetting name = \"DiaphragmGraph\" value=\"0\"/>"+
				"<ICBSetting name = \"ESOTEMP\" value=\"0\"/>"+
				"<ICBSetting name = \"ECG78\" value=\"90\"/>"+

			"</ICBstate>"+
		"</Settings>"+
	  "</ICBConfig></CanBusSimulatorConfiguration>";

    private static StateToICBValue _expectedIdle = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 25,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedReady = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 26,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedInflation = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 27,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedTransition = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 26,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedAblation = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 25,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedThawing = new StateToICBValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 24,
      ESOTEMP = 2.0,
      ECG78 = 90.0,

    };
    private static StateToICBValue _expectedException = new StateToICBValue()
    {
      ECG12 = 0,
      DiaphragmGraph = 0,
      ESOTEMP = 0,
      ECG78 = 90,

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

    private ICBSensorsMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock; 
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;

    private int _expectedECGInterval = 40;

    private IDictionary<string, StateToICBValue> _expectedStateStringToICBMap = 
      new Dictionary<string, StateToICBValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToICBValue> _expectedStateToICB =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToICBValue>()
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
    public void ICBConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_ICBStatusConfiguration);
      var ICBStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+ICB_CONFIG_NODE_ID); 

      var config = new ICBSensorMessageConfig();
      var loaded = config.Parse(ICBStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToICBMap.Count);
      Assert.AreEqual(_expectedECGInterval, config.ECGInterval);
      foreach (var keyValuePair in config.StateToICBMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToICBMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.ECG12, keyValuePair.Value.ECG12);
        Assert.AreEqual(expectedValue.DiaphragmGraph, keyValuePair.Value.DiaphragmGraph);
        Assert.AreEqual(expectedValue.ESOTEMP, keyValuePair.Value.ESOTEMP);
        Assert.AreEqual(expectedValue.ECG78, keyValuePair.Value.ECG78);

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
      doc.LoadXml(_ICBStatusConfiguration);

      var ICBStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + ICB_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>()))
        .Returns(ICBStatusConfigNode);    

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

      _provider = new ICBSensorsMessageProvider(_eventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(3000).Wait(); // sin wave 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      _canBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus2, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 3, 0, ICB_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      //Console.WriteLine(CanBusMessageConverter.ConverteNegativDecimalData(data, 6));
      Assert.AreEqual(_expectedStateToICB[currentState].ECG12 ,CanBusMessageConverter.ConverteECGDecimalData(data, 0, 100.0));
      Assert.IsTrue(_expectedStateToICB[currentState].DiaphragmGraph >= Math.Abs(CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0)));
      Assert.AreEqual(_expectedStateToICB[currentState].ESOTEMP , CanBusMessageConverter.ConverteECGDecimalData(data, 4));
      Assert.AreEqual(_expectedStateToICB[currentState].ECG78 , CanBusMessageConverter.ConverteNegativDecimalData(data, 6));

    }
  }
}
