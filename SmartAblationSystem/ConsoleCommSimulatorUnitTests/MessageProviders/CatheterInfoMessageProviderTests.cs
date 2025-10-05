
using System;
using System.Collections.Generic;
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
  public class CatheterInfoMessageProviderTests : MessageProviderTestBase
  {
    private static string CATHETHER_CONFIG_NODE_ID = "CatheterConfig";
    private static uint CATHETHER_MESSAGE1_ID = 50;
    private static uint CATHETHER_MESSAGE2_ID = 51;
    private static readonly string _catheterStatusConfiguration = "<CanBusSimulatorConfiguration><CatheterConfig>\r\n" +
      "<CatheterId>1</CatheterId>" +
      "<CatheterSn>2</CatheterSn>" +
      "<CatheterLot>1</CatheterLot>" +
      "<CatheterMonth>1</CatheterMonth>" +
      "<CatheterDay>1</CatheterDay>" +
      "<CatheterYear>2025</CatheterYear>" +
      "<FirstUseCatheterHour>9</FirstUseCatheterHour>" +
      "<FirstUseCatheterMonth>1</FirstUseCatheterMonth>" +
      "<FirstUseCatheterDay>1</FirstUseCatheterDay>" +
      "<FirstUseCatheterYear>2022</FirstUseCatheterYear>" +
      "<CatheterInjections>0</CatheterInjections>" +
      "</CatheterConfig></CanBusSimulatorConfiguration>";

    private CatheterInfoMessageProvider _provider;

    private int _expectedCatheterId = 1;
    private int _expectedCatheterSn = 2;
    private int _expectedCatheterLot = 1;
    private int _expectedCatheterMonth = 1;
    private int _expectedCatheterDay = 1;
    private int _expectedCatheterYear = 2025;
    private int _expectedFirstUseHour = 9;
    private int _expectedFirstUseMonth = 1;
    private int _expectedFirstUseDay = 1;
    private int _expectedFirstUseYear = 2022;
    private int _expectedInjections = 0;

    private byte[] _expectedCatheterData = new byte[8] { 0x01, 0x02, 0, 0x01, 0x01, 0x01, 2025 >> 8 & 0xFF, 2025 & 0xFF };
    private byte[] _expectedFirstCatheterData = new byte[8] { 9 & 0xFF, 1 & 0xFF, 1 & 0xFF, 2022 >> 8 & 0xFF, 2022 & 0xFF, 0, 0, 0 };

    [TestMethod]
    public void CatheterConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_catheterStatusConfiguration);
      var catheterStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + CATHETHER_CONFIG_NODE_ID);

      var config = new CatheterInfoMessageConfig();
      var loaded = config.Parse(catheterStatusConfigNode);

      Assert.IsTrue(loaded);
      Assert.AreEqual(8, config.CatheterData.Length);
      Assert.AreEqual(config.CatheterData[0], _expectedCatheterData[0]);
      Assert.AreEqual(config.CatheterData[1], _expectedCatheterData[1]);
      Assert.AreEqual(config.CatheterData[2], _expectedCatheterData[2]);
      Assert.AreEqual(config.CatheterData[3], _expectedCatheterData[3]);
      Assert.AreEqual(config.CatheterData[4], _expectedCatheterData[4]);
      Assert.AreEqual(config.CatheterData[5], _expectedCatheterData[5]);
      Assert.AreEqual(config.CatheterData[6], _expectedCatheterData[6]);
      Assert.AreEqual(config.CatheterData[7], _expectedCatheterData[7]);

      Assert.AreEqual(config.FirstUseCatheterData[0], _expectedFirstCatheterData[0]);
      Assert.AreEqual(config.FirstUseCatheterData[1], _expectedFirstCatheterData[1]);
      Assert.AreEqual(config.FirstUseCatheterData[2], _expectedFirstCatheterData[2]);
      Assert.AreEqual(config.FirstUseCatheterData[3], _expectedFirstCatheterData[3]);
      Assert.AreEqual(config.FirstUseCatheterData[4], _expectedFirstCatheterData[4]);
      Assert.AreEqual(config.FirstUseCatheterData[5], _expectedFirstCatheterData[5]);
      Assert.AreEqual(config.FirstUseCatheterData[6], _expectedFirstCatheterData[6]);
      Assert.AreEqual(config.FirstUseCatheterData[7], _expectedFirstCatheterData[7]);

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
      doc.LoadXml(_catheterStatusConfiguration);

      var catheterStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + CATHETHER_CONFIG_NODE_ID);
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

      _provider = new CatheterInfoMessageProvider(EventAggregatorMock.Object, configurationMoq.Object, null);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });

      // there will be 3 messages sent, 50, 51 and 50 with 10ms delay between each
      // then 10ms delay
      // then there will be 5 RTR messages with 10ms delay between each
      Task.Delay(290).Wait();
      _provider.Dispose();

      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.Exactly(3)); // 

      Assert.IsNotNull(canbusMessages[0]);
      Assert.AreEqual(CanBusId.CanBus1, canbusMessages[0].Id);
      var messageId = CreateMessageId(currentState, 0, 1, CATHETHER_MESSAGE1_ID); // 50 is catheter message id , node is 0 
      Assert.AreEqual(messageId, canbusMessages[0].CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
      Assert.AreEqual(_expectedCatheterId, canbusMessages[0].CanBusEventArgs.Data[0]);
      Assert.AreEqual(_expectedCatheterSn, canbusMessages[0].CanBusEventArgs.Data[1]);
      Assert.AreEqual(_expectedCatheterLot, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[0].CanBusEventArgs.Data, 2));
      
      Assert.AreEqual(_expectedCatheterMonth, canbusMessages[0].CanBusEventArgs.Data[4]);
      Assert.AreEqual(_expectedCatheterDay, canbusMessages[0].CanBusEventArgs.Data[5]);
      Assert.AreEqual(_expectedCatheterYear, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[0].CanBusEventArgs.Data, 6));

      Assert.IsNotNull(canbusMessages[1]);
      Assert.AreEqual(CanBusId.CanBus1, canbusMessages[1].Id);
      var messageId2 = CreateMessageId(currentState, 0, 1, CATHETHER_MESSAGE2_ID); // 51 is first use Catheter message Id 
      Assert.AreEqual(messageId2, canbusMessages[1].CanBusEventArgs.Id);
      Assert.AreEqual(7, canbusMessages[1].CanBusEventArgs.Length);
      Assert.AreEqual(_expectedFirstUseHour, canbusMessages[1].CanBusEventArgs.Data[0]);
      Assert.AreEqual(_expectedFirstUseMonth, canbusMessages[1].CanBusEventArgs.Data[1]);
      Assert.AreEqual(_expectedFirstUseDay, canbusMessages[1].CanBusEventArgs.Data[2]);
      Assert.AreEqual(_expectedFirstUseYear, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[1].CanBusEventArgs.Data, 3));
      Assert.AreEqual(_expectedInjections, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[1].CanBusEventArgs.Data, 5));

      // 3rd, last message
      Assert.IsNotNull(canbusMessages[2]);
      Assert.AreEqual(CanBusId.CanBus1, canbusMessages[2].Id);
      Assert.AreEqual(messageId, canbusMessages[2].CanBusEventArgs.Id);
      Assert.AreEqual(8, canbusMessages[2].CanBusEventArgs.Length);
      Assert.AreEqual(_expectedCatheterId, canbusMessages[2].CanBusEventArgs.Data[0]);
      Assert.AreEqual(_expectedCatheterSn, canbusMessages[2].CanBusEventArgs.Data[1]);
      Assert.AreEqual(_expectedCatheterLot, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[2].CanBusEventArgs.Data, 2));
      Assert.AreEqual(_expectedCatheterMonth, canbusMessages[2].CanBusEventArgs.Data[4]);
      Assert.AreEqual(_expectedCatheterDay, canbusMessages[2].CanBusEventArgs.Data[5]);
      Assert.AreEqual(_expectedCatheterYear, CanBusMessageConverter.ConverteCatheterInfoData(canbusMessages[2].CanBusEventArgs.Data, 6));
    }
    
  }
}
