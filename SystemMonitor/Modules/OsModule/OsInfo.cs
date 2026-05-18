using SystemManagementProvider;
using SystemManagementProvider.Constants;

namespace OsModule;

public class OsInfo {
  public Dictionary<string, (string, string)> Data = new();
  private readonly SMProvider _smProvider;

  public OsInfo(SMProvider smProvider) {
    _smProvider = smProvider;
  }

  public Dictionary<string, (string, string)> GetData() {
    _smProvider.Invoke_Query_OperatingSystem(Win32_OperatingSystem.QueryString);
    return Data;
  }
}
