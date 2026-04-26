namespace CpuModule.Models;

public struct BasicInfo {
  public int BaseSpeed { get; init; }
  public int SocketNum { get; init; }
  public int NumOfPhysicalCores { get; init; }
  public int NumOfLogicalCores { get; init; }
  public bool VirtualizationEnabled { get; set; }
  public uint L1CacheSize { get; init; }
  public uint L2CacheSize { get; init; }
  public uint L3CacheSize { get; init; }
}