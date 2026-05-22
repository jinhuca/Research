namespace CpuModule.ViewModels;

public interface ICpuLiveViewModel {
  ICpuOverallLiveViewModel CpuOverallLiveViewModel { get; set; }
  ICoreLiveViewModel CoreLiveViewModel { get; set; }
}
