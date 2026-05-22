namespace CpuModule.ViewModels.Interfaces;

public interface ICpuLiveViewModel {
  ICpuOverallLiveViewModel CpuOverallLiveViewModel { get; set; }
  ICoreLiveViewModel CoreLiveViewModel { get; set; }
}
