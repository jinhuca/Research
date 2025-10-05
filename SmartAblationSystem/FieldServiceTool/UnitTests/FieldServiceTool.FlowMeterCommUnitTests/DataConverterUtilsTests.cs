using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FlowMeterComm;
using Module.FlowMeterComm.Models;

namespace FieldServiceTool.FlowMeterCommUnitTests
{
  [TestClass]
  public class DataConverterUtilsTests
  {
    [TestMethod]
    public void ToStructure_UniqueIdMessage_Test()
    {
      byte[] message = { 0x0e, 0x00, 0x00, 0xfe, 0x0a, 0x50, 0x05, 0x05, 0x01, 0x01, 0x01, 0x01, 0x3e, 0xeb, 0x09, 0x7b };
      var structData = message.ToStructure<DeviceIdMessage>();
      Assert.AreEqual(0x0e, structData.MessageSize);
      Assert.IsTrue(structData.Status.SequenceEqual(new byte[] { 0x00, 0x00 }));
      Assert.AreEqual(0xfe, structData.Reserved1);
      Assert.AreEqual(0x0a, structData.ManufacturerCode);
      Assert.AreEqual(0x50, structData.DeviceTypeCode);
      Assert.AreEqual(0x05, structData.NumOfPreambles);
      Assert.IsTrue(structData.RevisionNumbers.SequenceEqual(new byte[] { 0x05, 0x01, 0x01, 0x01 }));
      Assert.IsTrue(structData.DeviceId.SequenceEqual(new byte[] { 0x3e, 0xeb, 0x09 }));
      Assert.AreEqual(0x01, structData.Flags);
      Assert.AreEqual(0x7b, structData.CheckSum);
    }

    [TestMethod]
    public void ToStructure_FlowRateMessage_Test()
    {
      float flowVal = 8029.5f;
      var flowBytes = BitConverter.GetBytes(flowVal);

      byte[] message = new byte[] { 0x06, 0x00, 0x00, 0x11 }
        .Concat(flowBytes)
        .Concat(new byte[] { 0x00 })
        .ToArray();

      var structData = message.ToStructure<FlowRateMessage>();
      Assert.AreEqual(0x06, structData.MessageSize);
      Assert.AreEqual(0x11, structData.Unit);
      Assert.AreEqual(flowVal, structData.FlowRate);
    }
  }
}
