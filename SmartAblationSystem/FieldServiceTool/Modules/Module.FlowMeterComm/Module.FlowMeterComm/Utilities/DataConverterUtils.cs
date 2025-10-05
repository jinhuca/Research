using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlowMeterComm
{
  public static class DataConverterUtils
  {
    public static T ToStructure<T>(this byte[] byteArray) where T : struct
    {
      var packet = new T();
      // var bufferSize = Marshal.SizeOf(packet);
      var bufferSize = byteArray.Length; 
      IntPtr handle = Marshal.AllocHGlobal(bufferSize);
      Marshal.Copy(byteArray, 0, handle, bufferSize);

      return Marshal.PtrToStructure<T>(handle);
    }
    
    public static string GetLast(this string source, int numberOfChars)
    {
      if (numberOfChars >= source.Length)
        return source;
      return source.Substring(source.Length - numberOfChars);
    }
  }
}
