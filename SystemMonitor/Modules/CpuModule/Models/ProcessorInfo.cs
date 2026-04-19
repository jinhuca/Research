using SystemManagementProvider;
using SystemManagementProvider.Constants;

namespace CpuModule.Models; 
public class ProcessorInfo {
  public Dictionary<string, (string, string)> Data = new();
  private readonly SMProvider _smProvider;

  public ProcessorInfo(SMProvider smProvider_) { 
    _smProvider = smProvider_;
  }

  public Dictionary<string, (string, string)> GetData() {
    _smProvider.Invoke_Query_Processors(Win32_Processor.QueryString);
    return Data;
  }
}
