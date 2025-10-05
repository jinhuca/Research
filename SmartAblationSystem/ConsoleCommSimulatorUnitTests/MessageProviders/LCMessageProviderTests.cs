
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.Validation;
using ConsoleCommSimulator.MessageProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using SmartAblationSystem.Helpers;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class LCMessageProviderTests : MessageProviderTestBase
  {
    private static string LC_CONFIG_NODE_ID = "LCConfig";
    private static uint LC_MESSAGE_ID = 4;
    private static readonly string _LCStatusConfiguration = "<CanBusSimulatorConfiguration><LCConfig>\r\n" +
      "<UpdateInterval>50</UpdateInterval>"+
      "<LC1>23.9</LC1>" +
      "<LC1Interval>1000</LC1Interval>"+ // set shorter for testing purposes 

    "</LCConfig></CanBusSimulatorConfiguration>";

    private LCMessageProvider _provider;
    private int _expectedLCInterval = 50;
    private int _expectedLCTankInterval = 1000;
    private double _expectedLC1 = 23.9;
    private double _expectedLC1Consumed = 23.8;
    private Mock<LC1ThresholdValidation> _lc1ThresholdValidationMock;

    [TestMethod]
    public void LCConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_LCStatusConfiguration);
      var LCStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/"+LC_CONFIG_NODE_ID); 

      var config = new LCMessageConfig();
      var loaded = config.Parse(LCStatusConfigNode);

      Assert.IsTrue(loaded);
      Assert.AreEqual(_expectedLCInterval, config.Interval);
      Assert.AreEqual(_expectedLC1, config.LC1Value);
      Assert.AreEqual(_expectedLCTankInterval, config.LCInterval);
    
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, false);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, false);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, false);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, true);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, false);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_EXCEPTION()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, false);
    }

    private void TestProviderWithState(CanBusMessageDefinition.MessageStateId currentState, bool tankReduction)
    {
      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var doc = new XmlDocument();
      doc.LoadXml(_LCStatusConfiguration);

      var LCStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + LC_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(LCStatusConfigNode);    

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
      _lc1ThresholdValidationMock = new Mock<LC1ThresholdValidation>(EventAggregatorMock.Object);
      _provider = new LCMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _lc1ThresholdValidationMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // add assert data 

      Task.Delay(1100).Wait(); // wait 1 seconds for tank weight to reduce 

      _provider.Dispose();
      Assert.IsNotNull(canbusMessage.Last());
      Assert.AreEqual(CanBusId.CanBus1, canbusMessage.Last().Id);
      var messageId = CreateMessageId(currentState, 1, 0, LC_MESSAGE_ID); // type 0 (read value)
      Assert.AreEqual(messageId, canbusMessage.Last().CanBusEventArgs.Id);
      Assert.AreEqual(2, canbusMessage.Last().CanBusEventArgs.Length);

      // we only check end values
      var data = canbusMessage.Last().CanBusEventArgs.Data;
      // waited 1 seconds 
      if (tankReduction)
      {
        // it is actually 23.8687 but the bit shifting only deletes the last couple of digits instead of rounding
        Assert.AreEqual(_expectedLC1Consumed, CanBusMessageConverter.ConverteDecimalData(data, 0));
      } 
      else
      {

        Assert.AreEqual(_expectedLC1, CanBusMessageConverter.ConverteDecimalData(data, 0));
      }

    }
  }
}
