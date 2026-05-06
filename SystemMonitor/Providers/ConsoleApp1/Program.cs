using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;

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
    catch (Exception ex) { Console.WriteLine(ex.Message); }
  }

  /*
  static void GetCurrentSpeed() {
    // 1. Get the Maximum clock speed of the CPU via WMI (returned in MHz)
    uint maxClockSpeed = 0;
    using (var searcher = new ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor")) {
      foreach (var obj in searcher.Get()) {
        maxClockSpeed = (uint)obj["MaxClockSpeed"];
        break; // Assuming there's only one processor
      }
    }
    // 2. Setup the Performance Counter for current performance percentage
    // This represents the current speed as a % of the max speed
    using (var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total")) {
      // Initial call often returns 0; needs a small delay for an accurate reading
      cpuPerfCounter.NextValue();
      Thread.Sleep(1000);

      while (true) {
        float perfPercentage = cpuPerfCounter.NextValue();

        // 3. Calculate current speed in MHz
        double currentSpeedMhz = (maxClockSpeed * perfPercentage) / 100.0;
        double currentSpeedGhz = currentSpeedMhz / 1000.0;

        Console.WriteLine($"Max Speed:     {maxClockSpeed} MHz");
        Console.WriteLine($"Current Perf:  {perfPercentage:F2}%");
        Console.WriteLine($"Current Speed: {currentSpeedGhz:F2} GHz");

        Thread.Sleep(1000); // Update every second
      }
    }
  }
  */

  private static void GetAvailableMemory() {
    using (var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem")) {
      foreach (var obj in searcher.Get()) {
        ulong freeMemoryKB = (ulong)obj["FreePhysicalMemory"];
        double freeMemoryGB = freeMemoryKB / 1024.0 / 1024.0;
        Console.WriteLine($"Available Memory: {freeMemoryGB:F2} GB");
      }
    }
  }
  public static void Main(string[] args) {
    //Test();
    //GetCurrentSpeed();
    GetAvailableMemory();
  }
}
