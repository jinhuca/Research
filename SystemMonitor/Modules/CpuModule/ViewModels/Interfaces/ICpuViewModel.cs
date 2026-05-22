namespace CpuModule.ViewModels.Interfaces;

public interface ICpuViewModel {
  ICpuSummaryViewModel SummaryViewModel { get; set; }
  ICpuLiveViewModel LiveViewModel { get; set; }
}
