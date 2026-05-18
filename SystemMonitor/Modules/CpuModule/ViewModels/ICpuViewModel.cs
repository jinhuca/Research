using System;
using System.Collections.Generic;
using System.Text;

namespace CpuModule.ViewModels; 
public interface ICpuViewModel {
  ICpuSummaryViewModel SummaryViewModel { get; set; }
  ICpuLiveViewModel LiveViewModel { get; set; }
}
