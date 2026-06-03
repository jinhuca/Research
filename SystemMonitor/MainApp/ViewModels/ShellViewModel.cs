using SharedDefinitions;

namespace MainApp.ViewModels;

public class ShellViewModel : BindableBase {
  private readonly IRegionManager _regionManager;
  public DelegateCommand<string> NavigateCommand { get; }

  public ShellViewModel(IRegionManager regionManager) {
    _regionManager = regionManager;
    NavigateCommand = new DelegateCommand<string>(Navigate);
  }

  private void Navigate(string viewName) {
    if (!string.IsNullOrEmpty(viewName)) {
      _regionManager.RequestNavigate(RegionNames.MainContentRegionName, viewName);
    }
  }
}
