using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using SystemManagementProvider;
using SystemManagementProvider.Constants;

namespace CpuModule; 
internal class ProcessorInfo {
  public Dictionary<string, (string, string)> Data = new();
  private readonly SMProvider _smProvider;

  public ProcessorInfo(SMProvider smProvider_) { 
    _smProvider = smProvider_;
  }

  public Dictionary<string, (string, string)> GetData() {
    _smProvider.Invoke_Query_Processors(Win32_Processor.Query_String);
    return Data;
  }
}
