using DataStructures.Cpu.Interfaces;

namespace DataStructures.Cpu.Implementations; 
public class CpuLiveInfo : ICpuLiveInfo {
  public ICpuOverallLiveInfo CpuOverallLiveInfo { get; set; } = new CpuOverallLiveInfo();
  public List<ICpuCoreLiveInfo> CpuCoreLiveInfo { get; set; } = new List<ICpuCoreLiveInfo>();
}
