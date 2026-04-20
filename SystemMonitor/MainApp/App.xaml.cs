using CpuModule.Models;
using CpuModule.ViewModels;
using CpuModule.Views;
using System.Windows;

namespace MainApp;

public partial class App : PrismApplication {
  protected override Window CreateShell() {
    //base.InitializeModules();
    return Container.Resolve<Shell>();
  }

  protected override void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuModel, CpuModel>();
  }

  protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
    base.ConfigureModuleCatalog(moduleCatalog);
    moduleCatalog.AddModule<ResourceModule.ResourceModule>();
    moduleCatalog.AddModule<CpuModule.CpuModule>();
  }

  protected override void ConfigureViewModelLocator() {
    base.ConfigureViewModelLocator();
    ViewModelLocationProvider.Register<CpuMainView, CpuViewModel>();
  }
}
