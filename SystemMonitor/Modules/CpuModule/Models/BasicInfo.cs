namespace CpuModule.Models;

public struct BasicInfo {
  public int BaseSpeed { get; init; }
  public int SocketNum { get; init; }
  public int NumOfPhysicalCores { get; init; }
  public int NumOfLogicalCores { get; init; }
  public bool VirtualizationEnabled { get; set; }
}