using DataStructures.Cpu.Implementations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CpuInfoServices.Methods;  
public class NativeMethods {
  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string Brand();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  [return: MarshalAs(UnmanagedType.BStr)]
  public static extern string Vendor();

  [DllImport("HardwareInfoProvider.dll", EntryPoint = "GetInstructionSetStruct", CallingConvention = CallingConvention.Cdecl)]
  public static extern CpuInstructionInfo2 GetInstructionSetStruct();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  public static extern CpuCacheInfo GetCacheSize();

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

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  public static extern double GetTotalCpuUtilization();

  [DllImport("HardwareInfoProvider.dll", CallingConvention = CallingConvention.StdCall)]
  public static extern double GetCurrentCpuSpeed();
}
