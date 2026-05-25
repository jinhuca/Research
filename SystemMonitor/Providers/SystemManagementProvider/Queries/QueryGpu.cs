using System.Diagnostics;
using System.Management;
using SystemManagementProvider.Interfaces;

namespace SystemManagementProvider.Queries;

public class QueryGpu : ISMQuery {
  private ManagementObjectSearcher? _searcher;
  public List<string> GpuList { get; private set; } = new List<string>();
  public Dictionary<string, Dictionary<string, (string, string)>> GpuInfoList { get; private set; }
    = new Dictionary<string, Dictionary<string, (string, string)>>();

  private static Dictionary<string, (string, string)> info = new Dictionary<string, (string, string)>();

  public QueryGpu() {
  }

  private Dictionary<string, (string, string)> GetInfo() {
    info.Clear();
    try {
      using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
      foreach(ManagementBaseObject obj in searcher.Get()) {
        var objName = obj["Name"]?.ToString() ?? $"GPU_{GpuInfoList.Count + 1}";
        if(!GpuInfoList.ContainsKey(objName)) {
          GpuInfoList[objName] = new Dictionary<string, (string, string)>();
        }

        string name = obj["Name"]?.ToString()?.Replace("(R)", "") ?? "Unknown GPU";
        GpuList.Add(name);
        info["Name"] = (name, "GPU Name");

        foreach(var property in obj.Properties) {
          string key = property.Name;
          string value = property.Value?.ToString() ?? "N/A";
          string description = $"{key} of the GPU";
          info[key] = (value, description);

          if(GpuInfoList.ContainsKey(objName)) {
            GpuInfoList[objName][key] = info[key];
          }
        }
      }
    }
    catch(Exception ex) {
      Debug.WriteLine($"Error querying GPU information: {ex.Message}");
      Debug.WriteLine(ex.StackTrace);
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
      using var searcher = new ManagementObjectSearcher(query);
      foreach(ManagementBaseObject obj in searcher.Get()) {
        string name_ = obj["Name"]?.ToString() ?? $"GPU_{GpuInfoList.Count + 1}";
        if(!GpuInfoList.ContainsKey(name_)) {
          GpuInfoList[name_] = new Dictionary<string, (string, string)>();
        }
        foreach(var property in obj.Properties) {
          string key = property.Name;
          string value = property.Value?.ToString() ?? "N/A";
          string description = $"{key} of the GPU";
          if(GpuInfoList.ContainsKey(name_)) {
            GpuInfoList[name_].Add(key, (value, description));
          }
        }
      }
    }
    catch(Exception ex) {
      Debug.WriteLine($"Error querying multiple GPU information: {ex.Message}");
      Debug.WriteLine(ex.StackTrace);
    }

    return GpuInfoList;
  }
}