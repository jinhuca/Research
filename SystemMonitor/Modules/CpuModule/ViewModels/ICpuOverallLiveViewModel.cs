using System;
using System.Collections.Generic;
using System.Text;

namespace CpuModule.ViewModels; 
public interface ICpuOverallLiveViewModel {
  float? LoadViewModel { get; set; }
  float? TemperatureViewModel { get; set; }
  float? SpeedViewModel { get; set; }
}
