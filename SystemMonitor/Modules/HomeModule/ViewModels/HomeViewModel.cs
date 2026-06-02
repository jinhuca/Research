using SharedDefinitions;
using System.Windows.Input;

namespace HomeModule.ViewModels;

public class HomeViewModel : BindableBase, IHomeViewModel {
  private readonly IRegionManager _regionManager;
  public HomeViewModel(IRegionManager regionManager) {
    _regionManager = regionManager;
    NavigateCommand = new DelegateCommand<string>(Navigate);
  }

  private void Navigate(string viewName) {
    _regionManager.RequestNavigate(RegionNames.MainContentRegionName, viewName);
  }

  public ICommand NavigateCommand {
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }
}
