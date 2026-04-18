using Prism.Unity;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MainApp;

public partial class App : PrismApplication {
  protected override Window CreateShell() {
    base.InitializeModules();
    return Container.Resolve<Shell>();
  }

  protected override void RegisterTypes(IContainerRegistry containerRegistry) {
  }

  protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
    base.ConfigureModuleCatalog(moduleCatalog);
    moduleCatalog.AddModule<ResourceModule.ResourceModule>();
  }
}
