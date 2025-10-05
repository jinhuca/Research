
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
  public class FMMessageProviderTests : MessageProviderTestBase
  {
    private static bool IS_HI_FLOW = true;
    private static string FM_CONFIG_NODE_ID = "FMConfig";
    private static uint FM_MESSAGE_ID = 2;
    private static readonly string _FMStatusConfiguration = "<CanBusSimulatorConfiguration><FMConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<TargetFM>620.0</TargetFM>"+
      "<HiFlowCatheters>"+
      "<HiFlowCatheter>130</HiFlowCatheter >"+
      "</HiFlowCatheters>"+
      "<Settings>"+
      "<FMstate state = \"IDLE\">"+
				"<FMSetting name = \"FM1\" value=\"0\"/>"+
				"<FMSetting name = \"PT5\" value=\"13.7\"/>"+
				"<FMSetting name = \"PID\" value=\"0\"/>"+

			"</FMstate>"+
			"<FMstate state = \"READY\" >"+
        "<FMSetting name= \"FM1\" value=\"0\"/>"+
				"<FMSetting name = \"PT5\" value=\"13.9\"/>"+
        "<FMSetting name = \"PID\" value=\"0\"/>" +

      "</FMstate>" +
			"<FMstate state = \"INFLATION\" >"+
        "<FMSetting name= \"FM1\" value=\"0\"/>"+
				"<FMSetting name = \"PT5\" value=\"13.8\"/>"+
        "<FMSetting name = \"PID\" value=\"26.9\"/>" +

      "</FMstate>" +
			"<FMstate state = \"TRANSITION\" >"+
        "<FMSetting name= \"FM1\" value=\"6534\"/>" +
				"<FMSetting name = \"PT5\" value=\"15.3\"/>"+
       "<FMSetting name = \"PID\" value=\"26.3\"/>" +

      "</FMstate>" +
			"<FMstate state = \"ABLATION\" >"+
        "<FMSetting name= \"FM1\" value=\"8187\"/>" +
				"<FMSetting name = \"PT5\" value=\"15.6\"/>"+
      "<FMSetting name = \"PID\" value=\"25.4\"/>" +

      "</FMstate>" +
			"<FMstate state = \"THAWING\" >"+
        "<FMSetting name=\"FM1\" value=\"1466\"/>" +
				"<FMSetting name = \"PT5\" value=\"14.7\"/>"+
      "<FMSetting name = \"PID\" value=\"0\"/>" +

      "</FMstate>" +
			"<FMstate state = \"EXCEPTION\" >"+
        "<FMSetting name=\"FM1\" value=\"0\"/>"+
				"<FMSetting name = \"PT5\" value=\"0\"/>"+
      "<FMSetting name = \"PID\" value=\"0\"/>" +

      "</FMstate>" +
		"</Settings>"+
	  "</FMConfig></CanBusSimulatorConfiguration>";

    private static StateToFMValue _expectedIdle = new StateToFMValue()
    {
      FM1 = 0,
      PT5 = 13.7,
      PID = 0,

    };
    private static StateToFMValue _expectedReady = new StateToFMValue()
    {
      FM1 = 0,
      PT5 = 13.9,
      PID = 0,

    };
    private static StateToFMValue _expectedInflation = new StateToFMValue()
    {
      FM1 = 0,
      PT5 = 13.8,
      PID = 26.9,

    };
    private static StateToFMValue _expectedTransitionHI = new StateToFMValue()
    {
      FM1 = 653.4, // hi-flow catheter
      PT5 = 15.3,
      PID = 26.3,

    };
    private static StateToFMValue _expectedAblationHI = new StateToFMValue()
    {
      FM1 = 818.7,
      PT5 = 15.6,
      PID = 25.4,

    };
    private static StateToFMValue _expectedTransitionLOW = new StateToFMValue()
    {
      FM1 = 620.0, // flow limited because not a hi-flow catheter
      PT5 = 15.3,
      PID = 26.3,

    };
    private static StateToFMValue _expectedAblationLOW = new StateToFMValue()
    {
      FM1 = 620.0,
      PT5 = 15.6,
      PID = 25.4,

    };
    private static StateToFMValue _expectedThawing = new StateToFMValue()
    {
      FM1 = 146.6,
      PT5 = 14.7,
      PID = 0,

    };
    private static StateToFMValue _expectedException = new StateToFMValue()
    {
      FM1 = 0,
      PT5 = 0,
      PID = 0,

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

    private FMMessageProvider _provider;
    private Mock<FM1ThresholdValidation> _fm1ThresholdValidationMock;

    private int _expectedFMInterval = 50;

    private IDictionary<string, StateToFMValue> _expectedStateStringToFMMap = 
      new Dictionary<string, StateToFMValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransitionHI },
      {"ABLATION", _expectedAblationHI },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToFMValue> _expectedStateToFM =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToFMValue>()
      {
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, _expectedIdle},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, _expectedReady},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, _expectedInflation},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, (IS_HI_FLOW == false ? _expectedTransitionLOW : _expectedTransitionHI)},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, (IS_HI_FLOW == false ? _expectedAblationLOW : _expectedAblationHI)},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, _expectedThawing},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, _expectedException}
      };
    [TestMethod]
    public void FMConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_FMStatusConfiguration);
      var FMStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+FM_CONFIG_NODE_ID); 

      var config = new FMMessageConfig();
      var loaded = config.Parse(FMStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.AreEqual(50, config.Interval);
      Assert.AreEqual(620.0, config.TargetFM);

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToFMMap.Count);
      Assert.AreEqual(_expectedFMInterval, config.Interval);
      foreach (var keyValuePair in config.StateToFMMap)
      {
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToFMMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.FM1 * 10, keyValuePair.Value.FM1);
        Assert.AreEqual(expectedValue.PT5, keyValuePair.Value.PT5);
        Assert.AreEqual(expectedValue.PID, keyValuePair.Value.PID);

      }
      IS_HI_FLOW = true;
      foreach (var keyValuePair in config.StateToFMMap)
      {
        // testing when it is high flow
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToFMMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.FM1*10, keyValuePair.Value.FM1);
        Assert.AreEqual(expectedValue.PT5 , keyValuePair.Value.PT5);
        Assert.AreEqual(expectedValue.PID , keyValuePair.Value.PID);

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
      doc.LoadXml(_FMStatusConfiguration);

      var FMStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + FM_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(FMStatusConfigNode);    

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
      _fm1ThresholdValidationMock = new Mock<FM1ThresholdValidation>(EventAggregatorMock.Object);
      _provider = new FMMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _fm1ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      Task.Delay(3000).Wait(); // wait until it stabilizes 
      // if we wait less then the value will be between the previous value and the final value
      _provider.Dispose();
      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.AtLeastOnce);

      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 1, 0, FM_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(6, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      // tests that the current FM value is higher or equal to the transition value
      if (currentState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
      {
        Assert.IsTrue(_expectedStateToFM[currentState].FM1 <= CanBusMessageConverter.ConverteDecimalData(data, 0));
        
      } else
      {
        // normally they stay equal
        Assert.AreEqual(_expectedStateToFM[currentState].FM1, CanBusMessageConverter.ConverteDecimalData(data, 0));
      }

      Assert.AreEqual(_expectedStateToFM[currentState].PT5, CanBusMessageConverter.ConverteDecimalData(data, 2));
      Assert.AreEqual(_expectedStateToFM[currentState].PID, CanBusMessageConverter.ConverteDecimalData(data, 4));
    }
  }
}
