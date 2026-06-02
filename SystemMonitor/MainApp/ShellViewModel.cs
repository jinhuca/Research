using SharedDefinitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp;

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
