
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
  public class DMSSensorsMessageProviderTests : MessageProviderTestBase
  {
    private static string DMS_CONFIG_NODE_ID = "DMSConfig";
    private static uint DMS_MESSAGE_ID = 8; // on canbus2
    private static uint DMS_MESSAGE_ID1 = 1; // on canbus2
    private static readonly string _DMSStatusConfiguration = "<CanBusSimulatorConfiguration><DMSConfig>\r\n" +
      "<ECGUpdateInterval>40</ECGUpdateInterval>"+

    "<DMSConnected>1</DMSConnected>"+

    "<PressureConnected>1</PressureConnected> " +

    "<Series400ETSConnected>1</Series400ETSConnected> " +

    "<CircaETSConnected>1</CircaETSConnected> " +
    "<Settings>" +

      "<DMSstate state =\"IDLE\">"+
        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>"+
				"<DMSSetting name = \"DiaphragmGraph\" value=\"27\"/>"+
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"READY\"> "+
        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>" +
        "<DMSSetting name = \"DiaphragmGraph\" value=\"28\"/>" +
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"INFLATION\">"+

        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>" +
        "<DMSSetting name = \"DiaphragmGraph\" value=\"29\"/>" +
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"TRANSITION\"> "+
        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>" +
        "<DMSSetting name = \"DiaphragmGraph\" value=\"29\"/>" +
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"ABLATION\"> "+
        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>" +
        "<DMSSetting name = \"DiaphragmGraph\" value=\"28\"/>" +
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"THAWING\"> "+

        "<DMSSetting name = \"ECG12\" value=\"36.09\"/>" +
        "<DMSSetting name = \"DiaphragmGraph\" value=\"27\"/>" +
        "<DMSSetting name = \"ESOTEMP\" value=\"30\"/>" +
        "<DMSSetting name = \"ECG78\" value=\"85\"/>" +

      "</DMSstate>" +
			"<DMSstate state = \"EXCEPTION\" > "+

        "<DMSSetting name=\"ECG12\" value=\"0\"/>"+
				"<DMSSetting name = \"DiaphragmGraph\" value=\"0\"/>"+
				"<DMSSetting name = \"ESOTEMP\" value=\"30\"/>"+
				"<DMSSetting name = \"ECG78\" value=\"85\"/>"+

			"</DMSstate>"+
		"</Settings>"+
	  "</DMSConfig></CanBusSimulatorConfiguration>";

    private static byte[] _expectedDMSSetting = new byte[] { 0x0F, 0, 0, 0, 0, 0, 0, 0 };

    private static StateToDMSValue _expectedIdle = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 27,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedReady = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 28,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedInflation = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 29,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedTransition = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 29,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedAblation = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 28,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedThawing = new StateToDMSValue()
    {
      ECG12 = 36.09,
      DiaphragmGraph = 27,
      ESOTEMP = 30,
      ECG78 = 85,

    };
    private static StateToDMSValue _expectedException = new StateToDMSValue()
    {
      ECG12 = 0,
      DiaphragmGraph = 0,
      ESOTEMP = 30,
      ECG78 = 85,

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

    private DMSSensorMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock; 
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;

    private int _expectedECGInterval = 40;

    private IDictionary<string, StateToDMSValue> _expectedStateStringToDMSMap = 
      new Dictionary<string, StateToDMSValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToDMSValue> _expectedStateToDMS =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToDMSValue>()
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
    public void DMSConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_DMSStatusConfiguration);
      var DMSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+DMS_CONFIG_NODE_ID); 

      var config = new DMSSensorMessageConfig();
      var loaded = config.Parse(DMSStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToDMSMap.Count);
      Assert.AreEqual(_expectedECGInterval, config.ECGInterval);
      Assert.AreEqual(_expectedDMSSetting[0], config.DMSSetting[0]);
      Assert.AreEqual(_expectedDMSSetting[1], config.DMSSetting[1]);
      Assert.AreEqual(_expectedDMSSetting[2], config.DMSSetting[2]);
      Assert.AreEqual(_expectedDMSSetting[3], config.DMSSetting[3]);
      Assert.AreEqual(_expectedDMSSetting[4], config.DMSSetting[4]);
      Assert.AreEqual(_expectedDMSSetting[5], config.DMSSetting[5]);
      Assert.AreEqual(_expectedDMSSetting[6], config.DMSSetting[6]);
      Assert.AreEqual(_expectedDMSSetting[7], config.DMSSetting[7]);
      foreach (var keyValuePair in config.StateToDMSMap)
      {
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToDMSMap[keyValuePair.Key];
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
      // skip this test if hi resolution DMS active

      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_DMSStatusConfiguration);

      var DMSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + DMS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(DMSStatusConfigNode);    

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

      _provider = new DMSSensorMessageProvider(_eventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(3000).Wait(); // sin wave 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      _canBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);
      Assert.IsNotNull(canbusMessage.Last());
      CanBusMessage statusMessage;
      CanBusMessage ECGMessage;
      if (canbusMessage.Last().CanBusEventArgs.Id == CreateMessageId(currentState, 3, 0, DMS_MESSAGE_ID1))
      {
        statusMessage = canbusMessage.Last();
        // keep trying to get the other type of message
        while (true)
        {
          var lastIndex = canbusMessage.Count() - 1;
          canbusMessage.RemoveAt(lastIndex);
          Assert.IsNotNull(canbusMessage.Last());
          if (canbusMessage.Last().CanBusEventArgs.Id == CreateMessageId(currentState, 3, 0, DMS_MESSAGE_ID))
          {
            ECGMessage = canbusMessage.Last();
            break;
          }
        }
      }
      else
      {
        ECGMessage = canbusMessage.Last();
        // keep trying to get the other type of message
        /*while (true)
        {
          var lastIndex = canbusMessage.Count() - 1;
          canbusMessage.RemoveAt(lastIndex);
          Assert.IsNotNull(canbusMessage.Last());
          if (canbusMessage.Last().CanBusEventArgs.Id == CreateMessageId(currentState, 3, 0, DMS_MESSAGE_ID1))
          {
            statusMessage = canbusMessage.Last();
            break;
          }
        }*/
      }
      Assert.AreEqual(CanBusId.CanBus2, ECGMessage.Id);
      var messageId = CreateMessageId(currentState, 3, 0, DMS_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, ECGMessage.CanBusEventArgs.Id);
      Assert.AreEqual(8, ECGMessage.CanBusEventArgs.Length);

      // we only check end values
      var data = ECGMessage.CanBusEventArgs.Data;
      Assert.AreEqual(_expectedStateToDMS[currentState].ECG12, CanBusMessageConverter.ConverteECGDecimalData(data, 0, 100.0));
      Assert.IsTrue(Math.Abs(CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0)) >= 0);
      Assert.IsTrue(Math.Abs(CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0)) <= 1);
      Assert.AreEqual(_expectedStateToDMS[currentState].ESOTEMP, CanBusMessageConverter.ConverteECGDecimalData(data, 4));
      Assert.AreEqual(_expectedStateToDMS[currentState].ECG78, CanBusMessageConverter.ConverteNegativDecimalData(data, 6));

      // verify status messages (only one byte)
      //var statusData = statusMessage.CanBusEventArgs.Data;

      //Assert.AreEqual(_expectedDMSSetting[0], statusData[0]); // should be 0x0f

    }
  }
}
