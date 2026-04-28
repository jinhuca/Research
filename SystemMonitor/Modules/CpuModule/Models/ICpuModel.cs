using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace CpuModule.Models;

public interface ICpuModel : INotifyPropertyChanged, INotifyCollectionChanged {
  public string BrandName { get; set; }
  public string VendorName { get; set; }
  public BasicInfo BasicInfo { get; set; }
  public InstructionInfo? InstructionInfo { get; set; }
  public ExtendedInfo? ExtendedInfo { get; set; }
  public CacheSize CacheSize { get; set; }
  //public ReadableCacheSize ReadableCacheSize { get; set; }
  public RealTimeInfo? RealTimeInfo { get; set; }

  public double Utilization { get; set; }
}
