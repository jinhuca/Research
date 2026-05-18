using System;
using System.Collections.Generic;
using System.Text;

namespace StorageModule.Interfaces; 
public class DiskInfo : BindableBase {
  public string? Name { get; set; }
  public string? DeviceId { get; set; }
  public string? VolumeName { get; set; }

  public string? SerialNum { get; set; }
  public string? MediaType { get; set; }
  public string? PhysicalType { get; set; }
  public ulong Capacity { get; set; }
  public ulong FormattedCapacity { get; set; }
  public bool SystemDisk { get; set; }
  public bool PageFile { get; set; }

  public string? Health { get; set; }
  public int TotalActiveTimePercentage { get; set; }
  public int ActiveTimePercentage { get; set; }
  public string? FileSystem { get; internal set; }
}

public enum DiskType { 

}
