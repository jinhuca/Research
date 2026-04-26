using System.Runtime.InteropServices;

namespace ConsoleApp1;

public struct ProcessorInfo {
  public string Vendor { get; init; }
  public string Brand { get; init; }
  public int BaseSpeed { get; init; }
  public int SocketNum { get; init; }
  public int NumOfPhysicalCores { get; init; }
  public int NumOfLogicalCores { get; init; }
  public bool VirtualizationEnabled { get; set; }
  public InstructionFeature Features { get; init; }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CacheSize {
  int L1_cache_size;
  int L1_cache_line_size;
  int L2_cache_size;
  int L2_cache_line_size;
  int L3_cache_size;
  int L3_cache_line_size;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct InstructionFeature {
  [MarshalAs(UnmanagedType.U1)]
  public bool _3DNOW;
  [MarshalAs(UnmanagedType.U1)]
  public bool _3DNOWEXT;
  [MarshalAs(UnmanagedType.U1)]
  bool ABM;
  [MarshalAs(UnmanagedType.U1)]
  bool ADX;
  [MarshalAs(UnmanagedType.U1)]
  bool AES;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX2;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX512CD;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX512ER;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX512F;
  [MarshalAs(UnmanagedType.U1)]
  bool AVX512PF;
  [MarshalAs(UnmanagedType.U1)]
  bool BMI1;
  [MarshalAs(UnmanagedType.U1)]
  bool BMI2;
  [MarshalAs(UnmanagedType.U1)]
  bool CLFSH;
  [MarshalAs(UnmanagedType.U1)]
  bool CMPXCHG16B;
  [MarshalAs(UnmanagedType.U1)]
  bool CX8;
  [MarshalAs(UnmanagedType.U1)]
  bool ERMS;
  [MarshalAs(UnmanagedType.U1)]
  bool F16C;
  [MarshalAs(UnmanagedType.U1)]
  bool FMA;
  [MarshalAs(UnmanagedType.U1)]
  bool FSGSBASE;
  [MarshalAs(UnmanagedType.U1)]
  bool FXSR;
  [MarshalAs(UnmanagedType.U1)]
  bool HLE;
  [MarshalAs(UnmanagedType.U1)]
  bool INVPCID;
  [MarshalAs(UnmanagedType.U1)]
  bool LAHF;
  [MarshalAs(UnmanagedType.U1)]
  bool LZCNT;
  [MarshalAs(UnmanagedType.U1)]
  bool MMX;
  [MarshalAs(UnmanagedType.U1)]
  bool MMXEXT;
  [MarshalAs(UnmanagedType.U1)]
  bool MONITOR;
  [MarshalAs(UnmanagedType.U1)]
  bool MOVBE;
  [MarshalAs(UnmanagedType.U1)]
  bool MSR;
  [MarshalAs(UnmanagedType.U1)]
  bool OSXSAVE;
  [MarshalAs(UnmanagedType.U1)]
  bool PCLMULQDQ;
  [MarshalAs(UnmanagedType.U1)]
  bool POPCNT;
  [MarshalAs(UnmanagedType.U1)]
  bool PREFETCHWT1;
  [MarshalAs(UnmanagedType.U1)]
  bool RDRAND;
  [MarshalAs(UnmanagedType.U1)]
  bool RDSEED;
  [MarshalAs(UnmanagedType.U1)]
  bool RDTSCP;
  [MarshalAs(UnmanagedType.U1)]
  bool RTM;
  [MarshalAs(UnmanagedType.U1)]
  bool SEP;
  [MarshalAs(UnmanagedType.U1)]
  bool SHA;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE2;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE3;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE41;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE42;
  [MarshalAs(UnmanagedType.U1)]
  bool SSE4a;
  [MarshalAs(UnmanagedType.U1)]
  bool SSSE3;
  [MarshalAs(UnmanagedType.U1)]
  bool SYSCALL;
  [MarshalAs(UnmanagedType.U1)]
  bool TBM;
  [MarshalAs(UnmanagedType.U1)]
  bool XOP;
  [MarshalAs(UnmanagedType.U1)]
  bool XSAVE;
}

public class NativeMethodGroup {
  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  public static extern CacheSize GetCacheSize();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string Brand();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string Vendor();

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "GetInstructionSetStruct", CallingConvention = CallingConvention.Cdecl)]
  public static extern InstructionFeature GetInstructionSetStruct();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void GetCacheInfo();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern void GetLogicalProcessorInfo();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int GetPhysicalCoreCount();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int GetLogicalCoreCount();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int GetBaseSpeed();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int GetSocketNum();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.U1)]
  public static extern bool VirtualizationEnabled();
}

internal class Program {
  static void Test() {
    try {
      ProcessorInfo info = new ProcessorInfo {
        Vendor = NativeMethodGroup.Vendor(),
        Brand = NativeMethodGroup.Brand(),
        BaseSpeed = NativeMethodGroup.GetBaseSpeed(),
        SocketNum = NativeMethodGroup.GetSocketNum(),
        NumOfPhysicalCores = NativeMethodGroup.GetPhysicalCoreCount(),
        NumOfLogicalCores = NativeMethodGroup.GetLogicalCoreCount(),
        VirtualizationEnabled = NativeMethodGroup.VirtualizationEnabled(),
        Features = NativeMethodGroup.GetInstructionSetStruct()
      };
      var cacheSize = NativeMethodGroup.GetCacheSize();
    }
    catch(Exception ex) { Console.WriteLine(ex.Message); }
  }

  public static void Main(string[] args) {
    Test();
  }
}
