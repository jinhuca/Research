using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MemoryModule.Models; 
public interface IMemoryModel : INotifyPropertyChanged, INotifyCollectionChanged {
  string Name { get; set; }
}
