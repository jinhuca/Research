using System;
using System.Collections.Generic;
using System.Text;

namespace GpuModule.ViewModels; 
public interface IGpuSummaryViewModel {
  string ID { get; set; }
  string Name { get; set; }
  string Vendor {  get; set; }
  string Type {  get; set; }
  string Version { get; set; }
  string Ram {  get; set; }
}
