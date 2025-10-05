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
  public class FirmwareMessageProviderTests : MessageProviderTestBase
  {
    private static string CATHETHER_CONFIG_NODE_ID = "FirmwareConfig";
    private static uint MESSAGE8_ID = 8;
    private static uint MESSAGE48_ID = 48;
    private static uint MESSAGE56_ID = 56;
    private static uint MESSAGE11_ID = 11;
    private static uint MESSAGE24_ID = 24;
    private static readonly string _catheterStatusConfiguration = "<CanBusSimulatorConfiguration><FirmwareConfig>\r\n" +

    "<CMCUFirmwareVersion value =\"0x1001\"/>"+
    "<CPLDVersion value = \"0x0003\" />" +
    "<CMCUBootloaderFirmwareVersion value =\"0x1305\"/>" +

    "<PMCUFirmwareVersion value = \"0x2007\" />" +
    "<PMCUBootloaderFirmwareVersion value =\"0x1305\"/>" +

    "<CatheterFirmwareVersion value =\"0x4002\"/>" +
		   
		"<RepeaterFirmwareVersion value = \"0x2006\" />"+
    "<ICBFirmwareVersion value =\"0x1401\"/>"+
		"<RepeaterBootloaderFirmwareVersion value = \"0x1305\" />"+
    "<ICBBootloaderFirmwareVersion value =\"0x0000\"/>"+
		   
		"<RemoteFirmwareVersion value = \"0x1002\" />"+
    "<RemoteBootloaderFirmwareVersion value =\"0x3C3D\"/>"+
		"<RemoteFirmwareVersion3 value = \"0xB2F4\" />"+
    "<RemoteFirmwareVersion4 value =\"0x3C3D\"/>"+
    "</FirmwareConfig></CanBusSimulatorConfiguration>";

    private FirmwareVersionMessageProvider _provider;

    private static byte[] _expectedData8 = new byte[8] { 0x10, 0x01, 0x00, 0x03, 0x13, 0x05, 0, 0 };
    private static byte[] _expectedData48 = new byte[8] { 0x20, 0x07, 0x13, 0x05, 0, 0, 0, 0};
    private static byte[] _expectedData11 = new byte[8] { 0x20, 0x06, 0x14, 0x01, 0x13, 0x05, 0, 0};
    private static byte[] _expectedData24 = new byte[8] { 0x10, 0x02, 0x3c, 0x3d, 0xb2, 0xf4, 0x3c, 0x3d};
    private static byte[] _expectedData56 = new byte[8] { 0x40, 0x02, 0, 0, 0, 0, 0, 0};

    private static int _expectedCMCUFirmware = 4097;// 4097 in base 10 is 1001 in 16
    private static int _expectedCPLD = 0003; //03
    private static int _expectedCMCUBootloaderFirmware = 4869;//1305

    private static int _expectedPMCUFirmware = 8199; // 2007
    private static int _expectedPMCUBootloaderFirmware = 4869; //1305

    private static int _expectedRepeaterFirmware = 8198; //2006
    private static int _expectedICBFirmware = 5121; //1401
    private static int _expectedRepeaterBootloaderFirmware = 4869; //1305
    private static int _expectedICBBootloaderFirmware = 0; //0

    private static int _expectedRemoteFirmware = 4098; //1002
    private static int _expectedRemoteBootloaderFirmware = 15421; //3c3d

    private static int _expectedCatheterFirmware = 16386; //4002

    [TestMethod]
    public void FirmwareConfiguration_Parse_test()
    {
      var doc = new XmlDocument();
      doc.LoadXml(_catheterStatusConfiguration);
      var catheterStatusConfigNode = doc.SelectSingleNode("CanBusSimulatorConfiguration/" + CATHETHER_CONFIG_NODE_ID);

      var config = new FirmwareVersionMessageConfig();
      var loaded = config.Parse(catheterStatusConfigNode);

      Assert.IsTrue(loaded);
      //Console.WriteLine(config.PMCUCatheterData[1]);

      Assert.AreEqual(_expectedData48[0], config.PMCUFirmwareData[0]);
      Assert.AreEqual(_expectedData48[1], config.PMCUFirmwareData[1]);
      Assert.AreEqual(_expectedData48[2], config.PMCUFirmwareData[2]);
      Assert.AreEqual(_expectedData48[3], config.PMCUFirmwareData[3]);

      Assert.AreEqual(_expectedData8[0], config.CMCUFirmwareData[0]);
      Assert.AreEqual(_expectedData8[1], config.CMCUFirmwareData[1]);
      Assert.AreEqual(_expectedData8[2], config.CMCUFirmwareData[2]);
      Assert.AreEqual(_expectedData8[3], config.CMCUFirmwareData[3]);
      Assert.AreEqual(_expectedData8[4], config.CMCUFirmwareData[4]);
      Assert.AreEqual(_expectedData8[5], config.CMCUFirmwareData[5]);

      Assert.AreEqual(_expectedData56[0], config.CatheterFirmwareData[0]);
      Assert.AreEqual(_expectedData56[1], config.CatheterFirmwareData[1]);

      Assert.AreEqual(_expectedData11[0], config.RepeaterICBFirmwareData[0]);
      Assert.AreEqual(_expectedData11[1], config.RepeaterICBFirmwareData[1]);
      Assert.AreEqual(_expectedData11[2], config.RepeaterICBFirmwareData[2]);
      Assert.AreEqual(_expectedData11[3], config.RepeaterICBFirmwareData[3]);
      Assert.AreEqual(_expectedData11[4], config.RepeaterICBFirmwareData[4]);
      Assert.AreEqual(_expectedData11[5], config.RepeaterICBFirmwareData[5]);
      Assert.AreEqual(_expectedData11[6], config.RepeaterICBFirmwareData[6]);
      Assert.AreEqual(_expectedData11[7], config.RepeaterICBFirmwareData[7]);

      Assert.AreEqual(_expectedData24[0], config.RemoteFirmwareData[0]);
      Assert.AreEqual(_expectedData24[1], config.RemoteFirmwareData[1]);
      Assert.AreEqual(_expectedData24[2], config.RemoteFirmwareData[2]);
      Assert.AreEqual(_expectedData24[3], config.RemoteFirmwareData[3]);
      Assert.AreEqual(_expectedData24[4], config.RemoteFirmwareData[4]);
      Assert.AreEqual(_expectedData24[5], config.RemoteFirmwareData[5]);
      Assert.AreEqual(_expectedData24[6], config.RemoteFirmwareData[6]);
      Assert.AreEqual(_expectedData24[7], config.RemoteFirmwareData[7]);
    }
    // the only state is state 0 

    // TODO: test different messages separately 
    [TestMethod]
    public void ProviderInitialize_Test_8()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 8);
    }
    [TestMethod]
    public void ProviderInitialize_Test_48()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 48);
    }
    [TestMethod]
    public void ProviderInitialize_Test_56()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 56);
    }
    [TestMethod]
    public void ProviderInitialize_Test_11()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 11);
    }
    [TestMethod]
    public void ProviderInitialize_Test_24()
    {
      TestProviderWithState(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 24);
    }

    private void TestProviderWithState(CanBusMessageDefinition.MessageStateId currentState, int message)
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

      _provider = new FirmwareVersionMessageProvider(EventAggregatorMock.Object, configurationMoq.Object);
      _provider.Initialize();
      Assert.IsNotNull(handleSystemStateUpdateAction);

      handleSystemStateUpdateAction?.Invoke(new ConsoleStateMessage() { State = currentState });
      // setup sending a canbus message 
      switch (message)
      {
        case 8:
            _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 118856, Data = new byte[8] });
            break;
        case 48:
          _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 118896, Data = new byte[8] });
          break;
        case 56:
          _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 118904, Data = new byte[8] });
          break;
        case 11:
          _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 118859, Data = new byte[8] });
          break;
        case 24:
          _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 118872, Data = new byte[8] });
          break;
      };

      // TODO: test if on wrong input update parameter will be ignored
      // 2424 is fictional id that should be ignored
      _provider.UpdateParameters(new CanBusMessageParameters { MessageId = 2424, Data = new byte[8] });
      Task.Delay(100).Wait();
      _provider.Dispose();

      CanBusUpdateEventMock.Verify(x => x.Publish(It.IsAny<CanBusMessage>()), Times.Once);

      if (message == 8)
      {
        var messageId = CreateMessageId(currentState, 1, 1, MESSAGE8_ID, 0); // 8 is catheter message id , node is 1 
        assertions(canbusMessages[0], messageId, 6);
        Assert.AreEqual(_expectedCMCUFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 0));
        Assert.AreEqual(_expectedCPLD, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 2));
        Assert.AreEqual(_expectedCMCUBootloaderFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 4));
      }
      else if (message == 48)
      {

        var messageId2 = CreateMessageId(currentState, 0, 1, MESSAGE48_ID, 0);
        assertions(canbusMessages[0], messageId2, 4);
        Assert.AreEqual(_expectedPMCUFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 0));
        Assert.AreEqual(_expectedPMCUBootloaderFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 2));
      }
      else if (message == 56)
      {
        var messageId3 = CreateMessageId(currentState, 0, 1, MESSAGE56_ID, 0);
        assertions(canbusMessages[0], messageId3, 2);
        Assert.AreEqual(_expectedCatheterFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 0));
      }      
      else if (message == 11)
      {
        var messageId4 = CreateMessageId(currentState, 3, 1, MESSAGE11_ID, 0);
        Assert.IsNotNull(message);
        Assert.AreEqual(CanBusId.CanBus2, canbusMessages[0].Id);
        Assert.AreEqual(messageId4, canbusMessages[0].CanBusEventArgs.Id);
        Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
        Assert.AreEqual(_expectedRepeaterFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 0));
        Assert.AreEqual(_expectedICBFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 2));
        Assert.AreEqual(_expectedRepeaterBootloaderFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 4));
        Assert.AreEqual(_expectedICBBootloaderFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 6));
      }      
      else if (message == 24)
      {
        var messageId5 = CreateMessageId(currentState, 3, 1, MESSAGE24_ID, 0);
        Assert.IsNotNull(message);
        Assert.AreEqual(CanBusId.CanBus2, canbusMessages[0].Id);
        Assert.AreEqual(messageId5, canbusMessages[0].CanBusEventArgs.Id);
        Assert.AreEqual(8, canbusMessages[0].CanBusEventArgs.Length);
        Assert.AreEqual(_expectedRemoteFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 0));
        Assert.AreEqual(_expectedRemoteBootloaderFirmware, CanBusMessageConverter.ConverteInfoData((canbusMessages[0].CanBusEventArgs.Data), 2));
      }

    }

    private void assertions(CanBusMessage message, uint messageId, int length)
    {
      Assert.IsNotNull(message);
      Assert.AreEqual(CanBusId.CanBus1, message.Id);
      Assert.AreEqual(messageId, message.CanBusEventArgs.Id);
      Assert.AreEqual(length, message.CanBusEventArgs.Length);
    }
  }
}
