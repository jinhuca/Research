using CpuModule.ViewModels.Interfaces;
using System.Collections.ObjectModel;

namespace CpuModule.ViewModels.Implementations;

public class CpuLiveViewModel : BindableBase, ICpuLiveViewModel {
  private ICpuOverallLiveViewModel _cpuOverallLiveViewModel = new CpuOverallLiveViewModel();
  public ICpuOverallLiveViewModel CpuOverallLiveViewModel {
    get => _cpuOverallLiveViewModel;
    set => SetProperty(ref _cpuOverallLiveViewModel, value);
  }

  //private ICoreLiveViewModel _coreLiveViewModel = new CoreLiveViewModel();
  //public ICoreLiveViewModel CoreLiveViewModel {
  //  get => _coreLiveViewModel;
  //  set => SetProperty(ref _coreLiveViewModel, value);
  //}

  private ObservableCollection<ICoreLiveViewModel> _coreLiveViewModel = new ObservableCollection<ICoreLiveViewModel>();
  public ObservableCollection<ICoreLiveViewModel> CoreLiveViewModel {
    get => _coreLiveViewModel;
    set => SetProperty(ref _coreLiveViewModel, value);
  }
}
