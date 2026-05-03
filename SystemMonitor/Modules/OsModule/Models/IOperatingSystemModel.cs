using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace OsModule.Models; 
public interface IOperatingSystemModel : INotifyPropertyChanged, INotifyCollectionChanged {
  string Name { get; set; }
}
