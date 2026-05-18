namespace DataStructures.Cpu.Interfaces;

public interface ICpuLiveInfo {
  ICpuOverallLiveInfo CpuOverallLiveInfo { get; set; }
  List<ICpuCoreLiveInfo> CpuCoreLiveInfo { get; set; }
}
