using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace OsModule.Models; 
public interface IOperatingSystemModel : INotifyPropertyChanged {
  string Caption { get; set; }
}
