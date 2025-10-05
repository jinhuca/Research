
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
  public class BloodPressureMessageProviderTests : MessageProviderTestBase
  {
    private static string ICB_CONFIG_NODE_ID = "BloodPressureConfig";
    private static uint ICB_MESSAGE_ID = 7; // on canbus2
    private static double BLOOD_PRESSURE_MIN = 12;
    private static double BLOOD_PRESSURE_MAX = 76;

    private static readonly string _ICBStatusConfiguration = "<CanBusSimulatorConfiguration><BloodPressureConfig>\r\n" +
      "<ICBUpdateInterval>500</ICBUpdateInterval>" +

    "<Settings>" +

      "<BloodPressurestate state =\"IDLE\">" +
        "<ICBSetting name = \"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"25\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"25.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"READY\"> " +

        "<ICBSetting name=\"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"26\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"INFLATION\">" +

        "<ICBSetting name=\"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"27\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"TRANSITION\"> " +

        "<ICBSetting name=\"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"26\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"ABLATION\"> " +

        "<ICBSetting name=\"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"25\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"THAWING\"> " +

        "<ICBSetting name=\"Pressure01\" value=\"36.09\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"24\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90.0\"/>" +

      "</BloodPressurestate>" +
      "<BloodPressurestate state = \"EXCEPTION\" > " +

        "<ICBSetting name=\"Pressure01\" value=\"0\"/>" +
        "<ICBSetting name = \"Pressure23\" value=\"0\"/>" +
        "<ICBSetting name = \"Pressure45\" value=\"20.0\"/>" +
        "<ICBSetting name = \"Pressure67\" value=\"90\"/>" +

      "</BloodPressurestate>" +
    "</Settings>" +
    "</BloodPressureConfig></CanBusSimulatorConfiguration>";

    private static StateToICBValue _expectedIdle = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 25,
      Pressure45 = 25.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedReady = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 26,
      Pressure45 = 20.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedInflation = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 27,
      Pressure45 = 20.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedTransition = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 26,
      Pressure45 = 20.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedAblation = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 25,
      Pressure45 = 20.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedThawing = new StateToICBValue()
    {
      Pressure01 = 36.09,
      Pressure23 = 24,
      Pressure45 = 20.0,
      Pressure67 = 90.0,

    };
    private static StateToICBValue _expectedException = new StateToICBValue()
    {
      Pressure01 = 0,
      Pressure23 = 0,
      Pressure45 = 20,
      Pressure67 = 90,

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

    private BloodPressureMessageProvider _provider;
    private Mock<IEventAggregator> _eventAggregatorMock;
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;

    private int _expectedECGInterval = 500;

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
      var ICBStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + ICB_CONFIG_NODE_ID);

      var config = new BloodPressureMessageConfig();
      var loaded = config.Parse(ICBStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToBloodPressureMap.Count);
      Assert.AreEqual(_expectedECGInterval, config.ECGInterval);
      foreach (var keyValuePair in config.StateToBloodPressureMap)
      {
        //Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToICBMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.Pressure01, keyValuePair.Value.Pressure01);
        Assert.AreEqual(expectedValue.Pressure23, keyValuePair.Value.Pressure23);
        Assert.AreEqual(expectedValue.Pressure45, keyValuePair.Value.Pressure45);
        Assert.AreEqual(expectedValue.Pressure67, keyValuePair.Value.Pressure67);

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
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(ICBStatusConfigNode);

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction = null;
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

      _provider = new BloodPressureMessageProvider(_eventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(5000).Wait(); // sin wave 
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
      var _bloodPressureValue = new double[4];
      CanBusMessageConverter.ConverteBloodPressureData(data, out _bloodPressureValue);
      double[] calculatedBloodPressureValue = { 0, 0, 0, 0 };
      for (int i = 0; i < _bloodPressureValue.Length; i++)
      {
        if (_bloodPressureValue[i] >= 0)
          calculatedBloodPressureValue[i] = _bloodPressureValue[i];
        else
          calculatedBloodPressureValue[i] = 0;
      }

      Assert.IsTrue(calculatedBloodPressureValue[0] >= BLOOD_PRESSURE_MIN && calculatedBloodPressureValue[0] <= BLOOD_PRESSURE_MAX);
      Assert.IsTrue(calculatedBloodPressureValue[1] >= BLOOD_PRESSURE_MIN && calculatedBloodPressureValue[1] <= BLOOD_PRESSURE_MAX);
      Assert.IsTrue(calculatedBloodPressureValue[2] >= BLOOD_PRESSURE_MIN && calculatedBloodPressureValue[2] <= BLOOD_PRESSURE_MAX);
      Assert.IsTrue(calculatedBloodPressureValue[3] >= BLOOD_PRESSURE_MIN && calculatedBloodPressureValue[3] <= BLOOD_PRESSURE_MAX);


    }
  }
}
