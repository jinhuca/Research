using CpuModule.ViewModels.Interfaces;

namespace CpuModule.ViewModels.Implementations;

public class CpuLiveViewModel : BindableBase, ICpuLiveViewModel {
  private ICpuOverallLiveViewModel _cpuOverallLiveViewModel = new CpuOverallLiveViewModel();
  public ICpuOverallLiveViewModel CpuOverallLiveViewModel {
    get => _cpuOverallLiveViewModel;
    set => SetProperty(ref _cpuOverallLiveViewModel, value);
  }

  private ICoreLiveViewModel _coreLiveViewModel = new CoreLiveViewModel();
  public ICoreLiveViewModel CoreLiveViewModel {
    get => _coreLiveViewModel;
    set => SetProperty(ref _coreLiveViewModel, value);
  }
}
