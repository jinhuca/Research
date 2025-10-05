
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
  public class ETSMessageProviderTests : MessageProviderTestBase
  {
    private static string ETS_CONFIG_NODE_ID = "ETSConfig";
    private static uint ETS_MESSAGE_ID5 = 5;
    private static uint ETS_MESSAGE_ID6 = 6;
    private static readonly string _ETSStatusConfiguration = "<CanBusSimulatorConfiguration><ETSConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<Settings>"+
      "<ETSstate state = \"IDLE\">"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

			"</ETSstate>"+
			"<ETSstate state = \"READY\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
			"<ETSstate state = \"INFLATION\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
			"<ETSstate state = \"TRANSITION\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
			"<ETSstate state = \"ABLATION\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
			"<ETSstate state = \"THAWING\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
			"<ETSstate state = \"EXCEPTION\" >"+
        "<ETSSetting name = \"Channel0\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel1\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel2\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel3\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel4\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel5\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel6\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel7\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel8\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel9\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel10\" value=\"64\"/>" +
        "<ETSSetting name = \"Channel11\" value=\"64\"/>" +

      "</ETSstate>" +
		"</Settings>"+
	  "</ETSConfig></CanBusSimulatorConfiguration>";

    private static byte[] _expectedIdle = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedReady = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedInflation = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedTransition = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedAblation = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedThawing = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

    };
    private static byte[] _expectedException = new byte[8]
    {
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,

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

    private ETSMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock; 
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;

    private int _expectedETSInterval = 50;

    private IDictionary<string, byte[]> _expectedStateStringToETSMap = 
      new Dictionary<string, byte[]>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, byte[]> _expectedStateToETS =
      new Dictionary<CanBusMessageDefinition.MessageStateId, byte[]>()
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
    public void ETSConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_ETSStatusConfiguration);
      var ETSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+ETS_CONFIG_NODE_ID); 

      var config = new ETSMessageConfig();
      var loaded = config.Parse(ETSStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToETSMap.Count);
      Assert.AreEqual(_expectedETSInterval, config.Interval);
      foreach (var keyValuePair in config.StateToETSMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToETSMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue[0] & 0xFF, keyValuePair.Value.Channel0);
        Assert.AreEqual(expectedValue[1] & 0xFF, keyValuePair.Value.Channel1);
        Assert.AreEqual(expectedValue[2] & 0xFF, keyValuePair.Value.Channel2);
        Assert.AreEqual(expectedValue[3] & 0xFF, keyValuePair.Value.Channel3);
        Assert.AreEqual(expectedValue[4] & 0xFF, keyValuePair.Value.Channel4);
        Assert.AreEqual(expectedValue[5] & 0xFF, keyValuePair.Value.Channel5);
        Assert.AreEqual(expectedValue[6] & 0xFF, keyValuePair.Value.Channel6);
        Assert.AreEqual(expectedValue[7] & 0xFF, keyValuePair.Value.Channel7);
        Assert.AreEqual(expectedValue[0] & 0xFF, keyValuePair.Value.Channel8);
        Assert.AreEqual(expectedValue[1] & 0xFF, keyValuePair.Value.Channel9);
        Assert.AreEqual(expectedValue[2] & 0xFF, keyValuePair.Value.Channel10);
        Assert.AreEqual(expectedValue[3] & 0xFF, keyValuePair.Value.Channel1);


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
      doc.LoadXml(_ETSStatusConfiguration);

      var ETSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + ETS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(),It.IsAny<string>()))
        .Returns(ETSStatusConfigNode);    

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

      _provider = new ETSMessageProvider(_eventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(1000).Wait(); // wait until it stabilizes 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      _canBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.First());
      Assert.AreEqual(CanBusId.CanBus2, canbusMessage.First().Id);
      var messageId = CreateMessageId(currentState, 3, 0, ETS_MESSAGE_ID5); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.First().CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessage.First().CanBusEventArgs.Length);

      // we only check first message
      var data = canbusMessage.First().CanBusEventArgs.Data;

      Assert.AreEqual(_expectedStateToETS[currentState][0], data[0]);
      Assert.AreEqual(_expectedStateToETS[currentState][1], data[1]);
      Assert.AreEqual(_expectedStateToETS[currentState][2], data[2]);
      Assert.AreEqual(_expectedStateToETS[currentState][3], data[3]);
      Assert.AreEqual(_expectedStateToETS[currentState][4], data[4]);
      Assert.AreEqual(_expectedStateToETS[currentState][5], data[5]);
      Assert.AreEqual(_expectedStateToETS[currentState][6], data[6]);
      Assert.AreEqual(_expectedStateToETS[currentState][7], data[7]);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus2, canbusMessage.Last().Id);
      var messageId6 = CreateMessageId(currentState, 3, 0, ETS_MESSAGE_ID6); // type 0 (read value)
      Assert.AreEqual(messageId6, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check last message
      var data2 = canbusMessage.Last().CanBusEventArgs.Data;

      Assert.AreEqual(_expectedStateToETS[currentState][0], data[0]);
      Assert.AreEqual(_expectedStateToETS[currentState][1], data[1]);
      Assert.AreEqual(_expectedStateToETS[currentState][2], data[2]);
      Assert.AreEqual(_expectedStateToETS[currentState][3], data[3]);
    }
  }
}
