using System.Runtime.InteropServices;

namespace ConsoleApp1;

[StructLayout(LayoutKind.Sequential)]
public struct MyData {
  public bool is3DNOW;
  public bool is3DNOWEXT;
  //public bool isXSAVE;
}

class NativeMethods {
  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void GetData(ref MyData data);
}

internal class Program {
  [DllImport("HardwareInfoProvider.dll", EntryPoint = "AddNumbers")]
  public static extern int AddNumbers(int a, int b);

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "Is_3DNOW")]
  public static extern bool Is_3DNOW();

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "Is_XSAVE")]
  public static extern bool Is_XSAVE();

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "GetVendor")]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string GetVendor();

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "GetBrand")]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string GetBrand();

  static void f() {
    MyData result = new MyData();
    NativeMethods.GetData(ref result);
    Console.WriteLine($"3DNOW = {result.is3DNOW}, 3DNOWEXT = {result.is3DNOWEXT}");
  }

  static void Main(string[] args) {
    string result1 = GetVendor();
    Console.WriteLine(result1);
    Console.WriteLine(GetBrand());

    Console.WriteLine("Is 3DNOW = " + Is_3DNOW());
    Console.WriteLine("Is XSAVE = " + Is_XSAVE());
    f();
  }
}
