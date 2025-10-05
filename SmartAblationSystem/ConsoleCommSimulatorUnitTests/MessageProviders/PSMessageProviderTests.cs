
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
  public class PSMessageProviderTests : MessageProviderTestBase
  {
    private static string PS_CONFIG_NODE_ID = "PSConfig";
    private static uint PS_MESSAGE_ID = 1;
    private static readonly string _PSStatusConfiguration = "<CanBusSimulatorConfiguration><PSConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<Settings>"+
      "<PSstate state = \"IDLE\">"+
				"<PSSetting name = \"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"READY\" >"+
        "<PSSetting name= \"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"INFLATION\" >"+
        "<PSSetting name= \"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"TRANSITION\" >"+
        "<PSSetting name= \"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"ABLATION\" >"+
        "<PSSetting name= \"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"THAWING\" >"+
        "<PSSetting name=\"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
			"<PSstate state = \"EXCEPTION\" >"+
        "<PSSetting name=\"PS1\" value=\"0\"/>"+
				"<PSSetting name = \"PT5\" value=\"0\"/>"+

			"</PSstate>"+
		"</Settings>"+
	  "</PSConfig></CanBusSimulatorConfiguration>";

    private static StateToPSValue _expectedIdle = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedReady = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedInflation = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedTransition = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedAblation = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedThawing = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

    };
    private static StateToPSValue _expectedException = new StateToPSValue()
    {
      PS1 = 0,
      PT5 = 0,

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

    private PSMessageProvider _provider;
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;
    private Mock<PS1ThresholdValidation> _ps1ThresholdValidationMock;
    private Mock<PT5ThresholdValidation> _pt5ThresholdValidationMock;
    private int _expectedPSInterval = 50;

    private IDictionary<string, StateToPSValue> _expectedStateStringToPSMap = 
      new Dictionary<string, StateToPSValue>()
    {
      {"IDLE", _expectedIdle },
      {"READY", _expectedReady },
      {"INFLATION", _expectedInflation },
      {"TRANSITION", _expectedTransition },
      {"ABLATION", _expectedAblation },
      {"THAWING", _expectedThawing },
      {"EXCEPTION", _expectedException }
    };
    private IDictionary<CanBusMessageDefinition.MessageStateId, StateToPSValue> _expectedStateToPS =
      new Dictionary<CanBusMessageDefinition.MessageStateId, StateToPSValue>()
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
    public void PSConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_PSStatusConfiguration);
      var PSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+PS_CONFIG_NODE_ID); 

      var config = new PSMessageConfig();
      var loaded = config.Parse(PSStatusConfigNode);
      var expectedKeyList = _stateIdToStringMap.Values;

      Assert.IsTrue(loaded);
      Assert.AreEqual(7, config.StateToPSMap.Count);
      Assert.AreEqual(_expectedPSInterval, config.Interval);
      foreach (var keyValuePair in config.StateToPSMap)
      {
        Console.WriteLine(keyValuePair.Key);
        Assert.IsTrue(expectedKeyList.Contains(keyValuePair.Key));
        var expectedValue = _expectedStateStringToPSMap[keyValuePair.Key];
        Assert.AreEqual(expectedValue.PS1, keyValuePair.Value.PS1);
        Assert.AreEqual(expectedValue.PT5, keyValuePair.Value.PT5);

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
      doc.LoadXml(_PSStatusConfiguration);

      var PSStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + PS_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(PSStatusConfigNode);    

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction=null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),        
          It.IsAny<bool>(),   
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);
      
      // there will be many messages sent
      // how to verify?
      List<CanBusMessage> canbusMessage = new List<CanBusMessage>();

      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessage.Add(m));

      _ps1ThresholdValidationMock = new Mock<PS1ThresholdValidation>(EventAggregatorMock.Object);
      _pt5ThresholdValidationMock = new Mock<PT5ThresholdValidation>(EventAggregatorMock.Object);

      _provider = new PSMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _ps1ThresholdValidationMock.Object, _pt5ThresholdValidationMock.Object);
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
      var messageId = CreateMessageId(currentState, 1, 0, PS_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(4, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      Assert.AreEqual(_expectedStateToPS[currentState].PS1, CanBusMessageConverter.ConverteDecimalData(data, 0));
      Assert.AreEqual(_expectedStateToPS[currentState].PT5, CanBusMessageConverter.ConverteDecimalData(data, 2));
    }
  }
}
