using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace CpuModule.Models;

public interface ICpuModel : INotifyPropertyChanged, INotifyCollectionChanged {
  public string? Name { get; set; }
  public string? Vendor { get; set; }
  public ProcessorInfo? ProcessorInfo { get; set; }
  public InstructionFeature? InstructionFeature { get; set; }
  public SystemExtendedInfo? SystemExtendedInfo { get; init; }
}
