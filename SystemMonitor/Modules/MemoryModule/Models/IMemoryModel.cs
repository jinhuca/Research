using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MemoryModule.Models; 
public interface IMemoryModel {
  string Name { get; set; }
  double Capacity {  get; set; }
  double Speed { get; set; }
}
