using System;
using System.Linq;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public static class ConfigUtils
  {
    
    public static byte[] ConvertIntStringToByteArray(string input, int fromBase)
    {
      uint intValue = 0;
      try
      {
        intValue = Convert.ToUInt32(input, fromBase);

      }
      catch (ArgumentException ae)
      {
        Log.LogException(ae);

      }

      catch (FormatException fe)
      {
        Log.LogException(fe);
      }
      catch (OverflowException oe)
      {
        Log.LogException(oe);
      }
      // intValue is little endian
      return BitConverter.GetBytes(intValue).Reverse().ToArray();
      
    }

  }
}
