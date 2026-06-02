using DataStructures.Ram.Interfaces;

namespace DataStructures.Ram.Implementations; 
public class RamSummaryInfo : IRamSummaryInfo {
  public int? TotalRamInGB { get; set; }
  public float? AvailableRamInGB { get; set; }
  public float? UsagePercentage { get; set; }
}
