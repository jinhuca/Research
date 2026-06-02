using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDefinitions; 
public static class ViewNames {
  public static string CpuViewName { get; set; } = "CpuSummaryView";
  public static string MemoryViewName { get; set; } = "MemorySummaryView";
  public static string StorageViewName { get; set; } = "StorageView";
  public static string WifiViewName { get; set; } = "WifiView";
  public static string GpuViewName { get; set; } = "GpuView";
  public static string FansViewName { get; set; } = "FansView";
}
