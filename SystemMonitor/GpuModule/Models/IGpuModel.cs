using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace GpuModule.Models; 
public interface IGpuModel : INotifyPropertyChanged, INotifyCollectionChanged {
  string Name { get; set; }
  BasicInfo BasicInfo { get; set; }
}
