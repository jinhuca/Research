using System.Linq;
using System.Reflection;
using FlowMeterComm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FieldServiceTool.FlowMeterCommUnitTests
{
  [TestClass]
  public class FlowMeterCommManagerTests
  {
    [TestMethod]
    public void GeneratePackedASCIIMessage_Test()
    {
      string tagName = "MFC-1234";
      byte[] expectedByteArray = { 0x34, 0x60, 0xed, 0xc7, 0x2c, 0xf4 };

      MethodInfo method = typeof(FlowMeterCommManager).GetMethod("GeneratePackedASCIIMessage", BindingFlags.NonPublic | BindingFlags.Static);

      var packedMessage = (byte[])method?.Invoke(null, new object[] { tagName });
      Assert.IsTrue(Enumerable.SequenceEqual(expectedByteArray, packedMessage));
    }
  }
}
