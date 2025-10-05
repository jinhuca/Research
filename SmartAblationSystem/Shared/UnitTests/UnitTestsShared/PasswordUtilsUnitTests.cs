using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;

namespace SharedUnitTests
{
  [TestClass]
  public class PasswordUtilsUnitTests
  {
    [TestMethod]
    public void GenerateEncryptionCode_Test()
    {
      string pw1 = "dkjh!fK123L@JKLKJ#0";
      TestPasswordEncryption(pw1);
      
      string pw2 = "d";
      TestPasswordEncryption(pw2);
      
      string pw3 = "dkjh!fK123L@JKLKJ#0dkjh!fK123L@JKLKJ#0dkjh!fK123L@JKLKJ#0";
      TestPasswordEncryption(pw3);
      
      string pw4 = "012345678909756353";
      TestPasswordEncryption(pw4);
      
      string pw5 = "abcdefghijklmnopqrstuvwxyz";
      TestPasswordEncryption(pw5);
      
      string pw6 = "abcdefghijklmnopqrstuvwxyz";
      TestPasswordEncryption(pw6);
      
      string pw7 = "~!@#$%^&*()+_-=";
      TestPasswordEncryption(pw7);
    }

    private void TestPasswordEncryption(string password)
    {
      var pw1EncryptedCode = PasswordUtils.GenerateEncryptionCode(password);
      Trace.WriteLine($"{{ {string.Join(", ", pw1EncryptedCode.Select(b => $"0x{b:x}"))} }}");

      var pwDecrypted = PasswordUtils.DecryptPasscode(pw1EncryptedCode);
      Assert.AreEqual(password, new string(pwDecrypted));
    }

  }
}
