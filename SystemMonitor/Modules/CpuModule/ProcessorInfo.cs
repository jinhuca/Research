using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using SystemManagementProvider;

namespace CpuModule; 
internal class ProcessorInfo {
  public Dictionary<string, (string, string)> Data = new();
  public ProcessorInfo() { }
  public Dictionary<string, (string, string)> GetData() {
    SMProvider.Invoke_Query_Processors();
    return Data;
  }
}
