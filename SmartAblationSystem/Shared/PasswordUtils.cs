using System.Text;

namespace Shared
{
  public static class PasswordUtils
  {
    private static readonly byte[] encryptionCodeBytes = { 0x99, 0x98, 0xca, 0xe4, 0x59, 0x44, 0xf1, 0xea, 0x4e, 0xef, 0x73, 0x79, 0x70, 0x13, 0x4a, 0xe6 };

    public static byte[] GenerateEncryptionCode(string passCode)
    {
      var baseCodeLen = encryptionCodeBytes.Length;
      var passCodeInbytes = Encoding.UTF8.GetBytes(passCode);
      for (int i = 0; i < passCodeInbytes.Length; ++i)
      { 
        var baseIndex = i % baseCodeLen;
        passCodeInbytes[i] ^= encryptionCodeBytes[baseIndex];
      }

      return passCodeInbytes; 
    }

    public static char[] DecryptPasscode(byte[] encryptedBytes)
    {
      var baseCodeLen = encryptionCodeBytes.Length;
      var passCodeArray = new byte[encryptedBytes.Length];
      for (int i = 0; i < encryptedBytes.Length; ++i)
      {
        var baseIndex = i % baseCodeLen;
        passCodeArray[i] = (byte)(encryptedBytes[i] ^ encryptionCodeBytes[baseIndex]);
      }

      return Encoding.UTF8.GetChars(passCodeArray);
    }
  }
}
