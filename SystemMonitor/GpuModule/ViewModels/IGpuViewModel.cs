using GpuModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GpuModule.ViewModels;

public interface IGpuViewModel {

  //string BrandName { get; set; }
  //Dictionary<string, BasicInfo> Summary { get; set; }

  Dictionary<string, Dictionary<string, string>> GpuSummaryList { get; set; }
}
