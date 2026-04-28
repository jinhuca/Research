namespace CpuModule.Models;

public class BasicInfo {
  public int BaseSpeed { get; set; }
  public int SocketNum { get; set; }
  public int NumOfPhysicalCores { get; set; }
  public int NumOfLogicalCores { get; set; }
  public bool VirtualizationEnabled { get; set; }
}