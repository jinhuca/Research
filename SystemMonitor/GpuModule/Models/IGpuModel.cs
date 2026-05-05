using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace GpuModule.Models; 
public interface IGpuModel {
  string Name { get; set; }
  BasicInfo BasicInfo { get; set; }
  float Utilization { get; set; }
  float Speed { get; set; }
  float Temperature { get; set; }
}
