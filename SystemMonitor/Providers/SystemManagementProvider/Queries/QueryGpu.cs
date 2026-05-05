using System.Diagnostics;
using System.Management;
using SystemManagementProvider.Interfaces;

namespace SystemManagementProvider.Queries; 
public class QueryGpu : ISMQuery {
  private ManagementObjectSearcher _searcher;
  public List<string> GpuList { get; private set; } = new List<string>();
  public Dictionary<string, Dictionary<string, (string, string)>> GpuInfoList { get; private set; } 
    = new Dictionary<string, Dictionary<string, (string, string)>>();

  private static Dictionary<string, (string, string)> info = [];

  public QueryGpu() {
    
  }

  private Dictionary<string, (string, string)> GetInfo() {
    try {
      _searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
      foreach (ManagementBaseObject? obj in _searcher.Get()) {
        //Debug.WriteLine($"GPU Count: {_searcher.Get().Count}");
        //Debug.WriteLine($"GPU Name: {obj["Name"]}");
        
        GpuInfoList.Add(obj["Name"]?.ToString() ?? $"GPU_{GpuInfoList.Count + 1}", new Dictionary<string, (string, string)>());

        string name = obj["Name"]?.ToString()?.Replace("(R)", "") ?? "Unknown GPU";
        GpuList.Add(name);
        info["Name"] = (name, "GPU Name");

        var temp = obj["Name"].ToString();
        foreach (var property in obj.Properties) {
          string key = property.Name;
          string value = property.Value?.ToString() ?? "N/A";
          string description = $"{key} of the GPU";
          info[key] = (value, description);

          if (GpuInfoList.ContainsKey(obj["Name"]?.ToString() ?? $"GPU_{GpuInfoList.Count + 1}")) {
            GpuInfoList[obj["Name"]?.ToString() ?? $"GPU_{GpuInfoList.Count + 1}"].Add(key, info[key]);
          }
        }

      }
    }
    catch (Exception ex) {
      Debug.WriteLine($"Error querying GPU information: {ex.Message}"); 
      Debug.WriteLine(ex.StackTrace);
      // Handle exceptions gracefully, perhaps log them
      info["Name"] = ("Unknown GPU", "GPU Name");
    }
    return info;
  }

  public Dictionary<string, (string, string)> Query(string query) {
    info = GetInfo();
    return info;
  }

  public Dictionary<string, Dictionary<string, (string, string)>> QueryMultiple(string query) {
    try {
      _searcher = new ManagementObjectSearcher(query);
      foreach (var obj in _searcher.Get()) {
        string name_ = obj["Name"].ToString() ?? $"GPU_{GpuInfoList.Count + 1}";
        GpuInfoList.Add(name_, new Dictionary<string, (string, string)>());
        foreach (var property in obj.Properties) {
          string key = property.Name;
          string value = property.Value?.ToString() ?? "N/A";
          string description = $"{key} of the GPU";
          if (GpuInfoList.ContainsKey(name_)) {
            GpuInfoList[name_].Add(key, (value, description));
          }
        }
      }
      _searcher.Dispose();
    }
    catch (Exception ex) {
      Debug.WriteLine($"Error querying multiple GPU information: {ex.Message}"); 
      Debug.WriteLine(ex.StackTrace);
      // Handle exceptions gracefully, perhaps log them
    }

    return GpuInfoList;
  }
}
