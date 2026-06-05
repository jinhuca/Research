using SharedDefinitions;

namespace MainApp.ViewModels;

public class ShellViewModel : BindableBase {
  private readonly IRegionManager _regionManager;

  private HomeContentViewModel _homeContentViewModel;
  public HomeContentViewModel HomeContentVM {
    get => _homeContentViewModel;
    set => SetProperty(ref _homeContentViewModel, value);
  }

  private float _cpuLoad;
  public float CpuLoad {
    get => _cpuLoad;
    set => SetProperty(ref _cpuLoad, value);
  }

  public DelegateCommand<string> NavigateCommand { get; }

  public ShellViewModel(IRegionManager regionManager, HomeContentViewModel homeContentViewModel) {
    _regionManager = regionManager;
    _homeContentViewModel = homeContentViewModel;
    NavigateCommand = new DelegateCommand<string>(Navigate);

     
  }

  private void Navigate(string viewName) {
    if (!string.IsNullOrEmpty(viewName)) {
      _regionManager.RequestNavigate(RegionNames.MainContentRegionName, viewName);
    }
  }
}
