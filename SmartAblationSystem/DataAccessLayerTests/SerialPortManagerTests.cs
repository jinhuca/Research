
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RS232Communication;

namespace DataAccessLayerTests
{
  [TestClass]
  public class SerialPortManagerTests
  {
    [TestMethod]
    public void BuildLSProMessageWithPaddingByte_Test()
    {
      var m1 = new byte[] { 0x0f, 0x00, 0x01 };
      var e1 = new byte[] { 0x0f, 0xfe, 0x0f, 0x00, 0x00, 0x01, 0x0f, 0xff };
      var m2 = new byte[] { 0x0f, 0x0f, 0x01 };
      var e2 = new byte[] { 0x0f, 0xfe, 0x0f, 0x00, 0x0f, 0x00, 0x01, 0x0f, 0xff };
      var m3 = new byte[] { 0x0f, 0xff, 0x01 };
      var e3 = new byte[] { 0x0f, 0xfe, 0x0f, 0x00, 0xff, 0x01, 0x0f, 0xff };
      var m4 = new byte[] { 0x01, 0x00, 0x0f };
      var e4 = new byte[] { 0x0f, 0xfe, 0x01, 0x00, 0x0f, 0x00, 0x0f, 0xff };
      var m5 = new byte[] { 0x0f, 0x00, 0x01, 0x0f, 0x00, 0x01 };
      var e5 = new byte[] { 0x0f, 0xfe, 0x0f, 0x00, 0x00, 0x01, 0x0f, 0x00, 0x00, 0x01, 0x0f, 0xff };
      var m6 = new byte[] { 0x0f, 0x00, 0x01, 0x0f, 0x0f, 0x01 };
      var e6 = new byte[] { 0x0f, 0xfe, 0x0f, 0x00, 0x00, 0x01, 0x0f, 0x00, 0x0f, 0x00, 0x01, 0x0f, 0xff };

      var method = typeof(SerialPortManager).GetMethod("BuildLSProMessageWithPaddingByte", BindingFlags.NonPublic | BindingFlags.Static);

      var r1 = method?.Invoke(null, new object[] { m1 });
      PrintTestResult(m1, r1 as List<byte>, e1);
      var r2 = method?.Invoke(null, new object[] { m2 });
      PrintTestResult(m2, r2 as List<byte>, e2);
      var r3 = method?.Invoke(null, new object[] { m3 });
      PrintTestResult(m3, r3 as List<byte>, e3);
      var r4 = method?.Invoke(null, new object[] { m4 });
      PrintTestResult(m4, r4 as List<byte>, e4);
      var r5 = method?.Invoke(null, new object[] { m5 });
      PrintTestResult(m5, r5 as List<byte>, e5);
      var r6 = method?.Invoke(null, new object[] { m6 });
      PrintTestResult(m6, r6 as List<byte>, e6);
    }

    private void PrintTestResult(byte[] message, List<byte> result, byte[] expected)
    {
      var messageStr = message.Select(m => $"0x{m:x2}");
      var resultStr = result.Select(r => $"0x{r:x2}");

      Trace.WriteLine( $"[{string.Join(", ", messageStr)}] => [{string.Join(", ", resultStr)}]");
      Assert.IsTrue(expected.SequenceEqual(result));
    }

  }
}
