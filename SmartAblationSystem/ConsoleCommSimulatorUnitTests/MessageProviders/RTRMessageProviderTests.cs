
using System;
using System.Collections.Generic;
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
  public class RTRMessageProviderTests : MessageProviderTestBase
  {
    private static uint TARGET_FLOW_MESSAGE_ID = 15;
    private static uint THRESHOLD_PT1_MESSAGE_ID = 17;
    /*private static uint THRESHOLD_PT2_MESSAGE_ID = 19;
    private static uint THRESHOLD_PT3_MESSAGE_ID = 21;
    private static uint THRESHOLD_PT4_MESSAGE_ID = 23;
    private static uint THRESHOLD_TS1_MESSAGE_ID = 25;
    private static uint THRESHOLD_FM1_MESSAGE_ID = 27;
    private static uint THRESHOLD_FM1_CURVE_MESSAGE_ID = 28;
    private static uint THRESHOLD_PT5_MESSAGE_ID = 31;
    private static uint THRESHOLD_LC1_MESSAGE_ID = 33;*/
    private static uint CATHETER_50_MESSAGE_ID = 50;
    private static uint CATHETER_52_MESSAGE_ID = 52;
    private static uint CATHETER_53_MESSAGE_ID = 53;
    private static uint CATHETER_54_MESSAGE_ID = 54;
    private static uint CATHETER_55_MESSAGE_ID = 55;
    private static uint CATHETER_57_MESSAGE_ID = 57;
    private static string RTR_CONFIG_NODE_ID = "RTRConfig";
    private static readonly string _RTRConfiguration = "<CanBusSimulatorConfiguration><RTRConfig>\r\n" +

"<RTRVerification value =\"0x09F60000\"/>" +
"</RTRConfig></CanBusSimulatorConfiguration>";

    private RTRMessageProvider _provider;
    // verification message 
    private uint _verificationMessageId;
    private byte[] _verificationData = new byte[8] { 0x09 & 0xFF, 0xF6 & 0xFF, 0, 0,0, 0, 0, 0 };
    // we send empty message, we should receive those RTR
    private byte[] _expectedTargetFlowCatheterData = new byte[8] { 0x13 & 0xFF, 0x88 , 0x15 & 0xFF , 0xE0, 0, 0, 0, 0 };
/*    private byte[] _expectedThresholdPT1CatheterData = new byte[8] { 0x21 & 0xFF, 0x34 , 0x26 & 0xFF , 0x16, 0x1A &0xFF, 0x90, 0, 0 };
    private byte[] _expectedThresholdPT2CatheterData = new byte[8] { 0x09 & 0xFF, 0xC4 , 0 , 0, 0, 0, 0, 0 };
    private byte[] _expectedThresholdPT3CatheterData = new byte[8] { 0x00 & 0xFF, 0xFA , 0, 0, 0, 0, 0, 0 };
    private byte[] _expectedThresholdPT4CatheterData = new byte[8] { 0x00 & 0xFF, 0x6E , 0x26 & 0xFF , 0x16, 0x1A &0xFF, 0x90, 0, 0 };
    private byte[] _expectedThresholdPT5CatheterData = new byte[8] { 0, 0xBE, 0 , 0, 0, 0, 0, 0 };
    private byte[] _expectedThresholdLC1CatheterData = new byte[8] { 0x00 & 0xFF, 0xD0, 0x00 & 0xFF, 0xC6, 0, 0, 0, 0 };
    private byte[] _expectedThresholdTS1CatheterData = new byte[8] { 0x21 & 0xFF, 0x34 , 0x26 & 0xFF , 0x16, 0x1A &0xFF, 0x90, 0, 0 };
    private byte[] _expectedThresholdFM1CatheterData = new byte[8] { 0x00 & 0xFF, 0x00, 0x27 & 0xFF, 0x10, 0x00 & 0xFF, 0x00, 0, 0 };
    private byte[] _expectedThresholdFM1CurveCatheterData = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };*/
    private byte[] _expected52CatheterData = new byte[8] { 0 & 0xFF, 0x19 , 0 , 0, 0, 0, 0, 0 };
    private byte[] _expected53CatheterData = new byte[8] { 0 & 0xFF, 0x64 , 0xFF , 0x9F, 0, 0, 0, 0 };
    private byte[] _expected54CatheterData = new byte[8] { 0xFD, 0x44 , 0 , 0xc8, 0, 0x8c, 2 & 0xFF , 0xEE };
    private byte[] _expected55CatheterData = new byte[8] { 1 & 0xFF, 0x90, 0, 0xc8, 0, 0, 0, 0xc8 };
    private byte[] _expected57CatheterData = new byte[8] { 1 & 0xFF, 0xF4, 0, 5 & 0xFF, 0, 0xc8, 0, 2 & 0xFF };

    [TestMethod]
    public void RTR_Configuration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_RTRConfiguration);
      var RTRConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + RTR_CONFIG_NODE_ID);

      var config = new RTRMessageConfig();
      var loaded = config.Parse(RTRConfigNode);

      Assert.IsTrue(loaded);

      Assert.AreEqual(_verificationData[0], config.RTR1Data[0]);
      Assert.AreEqual(_verificationData[1], config.RTR1Data[1]);
      Assert.AreEqual(_verificationData[2], config.RTR1Data[2]);
      Assert.AreEqual(_verificationData[3], config.RTR1Data[3]);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_EXCEPTION()
    {
      // normally doesn't need to test exception
      //TestRTRWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION);
    }

    private void TestRTRWithState(CanBusMessageDefinition.MessageStateId currentState)
    {
      var configurationMoq = new Mock<ISimulatorConfiguration>();
      var _rtrUpdaterMock = new Mock<RTRThresholdEventUpdater>(EventAggregatorMock.Object);
      var doc = new XmlDocument();
      doc.LoadXml(_RTRConfiguration);

      var catheterStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + RTR_CONFIG_NODE_ID);
      configurationMoq
        .Setup(x => x.LoadConfigurationSection(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(catheterStatusConfigNode);

      // Setup subscribe SystemStateUpdateEvent
      Action<ConsoleStateMessage> handleSystemStateUpdateAction = null;
      SystemStateUpdateEventMock
        .Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),
          It.IsAny<bool>(),
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => handleSystemStateUpdateAction = action);

      List<CanBusMessage> canbusMessages = new List<CanBusMessage>();

      CanBusUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<CanBusMessage>()))
        .Callback<CanBusMessage>(m => canbusMessages.Add(m));

      _provider = new RTRMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, _rtrUpdaterMock.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });
      // send confirmation message
      _verificationMessageId = CreateMessageId(currentState, 0, 1, CATHETER_50_MESSAGE_ID, 1);
      _provider.UpdateParameters(new CanBusMessageParameters { MessageId = _verificationMessageId, Data = _verificationData });
      // send a pt threshold message
      _verificationMessageId = CreateMessageId(currentState, 1, 0, THRESHOLD_PT1_MESSAGE_ID, 1);
      // update parameters for pt1
      _provider.UpdateParameters(new CanBusMessageParameters { MessageId = _verificationMessageId, Data = _verificationData });
      // add assert data 
      Task.Delay(400).Wait();
      _provider.Dispose();

      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.Exactly(90)); 
      // publish 90 times because 12 messages * 6 states sent all at once = 72
      // plus the RTR for Flow and PT1 (one per state *2 = 12) total 90
      // check if RTR exist in canbusMessages skip first 3 messages 
      foreach (var msg in canbusMessages)
      {
        if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 2, 1, CATHETER_52_MESSAGE_ID))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          //Assert.AreEqual(CanBusId.CanBus1, canbusMessages[0].Id);
          Assert.AreEqual(_expected52CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expected52CatheterData[1], msg.CanBusEventArgs.Data[1]);

        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 0, 1, CATHETER_53_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          //Assert.AreEqual(CanBusId.CanBus1, canbusMessages[0].Id);
          Assert.AreEqual(_expected53CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expected53CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expected53CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expected53CatheterData[3], msg.CanBusEventArgs.Data[3]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 0, 1, CATHETER_54_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          //Assert.AreEqual(CanBusId.CanBus1, canbusMessages[0].Id);
          Assert.AreEqual(_expected54CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expected54CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expected54CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expected54CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expected54CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expected54CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expected54CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expected54CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 0, 1, CATHETER_55_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          //Assert.AreEqual(CanBusId.CanBus1, canbusMessages[0].Id);
          Assert.AreEqual(_expected55CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expected55CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expected55CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expected55CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expected55CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expected55CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expected55CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expected55CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 0, 1, CATHETER_57_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1,  msg.Id);
          Assert.AreEqual(_expected57CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expected57CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expected57CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expected57CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expected57CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expected57CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expected57CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expected57CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, TARGET_FLOW_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedTargetFlowCatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedTargetFlowCatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        // RTR messqages are not sent from this console
/*        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_PT1_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdPT1CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }

        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_PT2_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdPT2CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }

        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_PT3_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdPT3CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }

        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_PT4_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdPT4CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }

        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_PT5_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdPT5CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_LC1_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdLC1CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_TS1_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdTS1CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_FM1_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdFM1CatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
        else if (msg.CanBusEventArgs.Id == CreateMessageId(currentState, 1, 1, THRESHOLD_FM1_CURVE_MESSAGE_ID, 1))
        {
          Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
          Assert.AreEqual(CanBusId.CanBus1, msg.Id);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[0], msg.CanBusEventArgs.Data[0]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[1], msg.CanBusEventArgs.Data[1]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[2], msg.CanBusEventArgs.Data[2]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[3], msg.CanBusEventArgs.Data[3]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[4], msg.CanBusEventArgs.Data[4]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[5], msg.CanBusEventArgs.Data[5]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[6], msg.CanBusEventArgs.Data[6]);
          Assert.AreEqual(_expectedThresholdFM1CurveCatheterData[7], msg.CanBusEventArgs.Data[7]);
        }
*/
      }
      


    }
  }
}
