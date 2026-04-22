using System.Runtime.InteropServices;

namespace ConsoleApp1;

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


  static void Main(string[] args) {
    int result = AddNumbers(1, 2);
    Console.WriteLine("Add 1 and 2 = " + result);

    string result1 = GetVendor();
    Console.WriteLine(result1);

    Console.WriteLine(GetBrand());

    Console.WriteLine("Is 3DNOW = " + Is_3DNOW());
    Console.WriteLine("Is XSAVE = " + Is_XSAVE());
  }
}
