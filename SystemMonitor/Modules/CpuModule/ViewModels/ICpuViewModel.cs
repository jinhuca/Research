namespace CpuModule.ViewModels;

public interface ICpuViewModel {
  ICpuSummaryViewModel SummaryViewModel { get; set; }
  ICpuLiveViewModel LiveViewModel { get; set; }
}
