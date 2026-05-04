using GpuModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GpuModule.ViewModels;

public interface IGpuViewModel {

  List<string> GpuNameList { get; set; }
  Dictionary<string, Dictionary<string, string>> GpuSummaryList { get; set; }
}
