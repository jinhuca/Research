using CpuModule.Models;
using CpuModule.ViewModels;
using CpuModule.Views;
using GpuModule;
using GpuModule.ViewModels;
using GpuModule.Views;
using MemoryModule.ViewModels;
using MemoryModule.Views;
using StorageModule.ViewModels;
using StorageModule.Views;
using System.Windows;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;

namespace MainApp;

public partial class App : PrismApplication {
  protected override Window CreateShell() {
    //base.InitializeModules();
    return Container.Resolve<Shell>();
  }

  protected override void RegisterTypes(IContainerRegistry containerRegistry) {
    //containerRegistry.Register<ICpuModel, CpuModel>();
    //containerRegistry.Register<ISMProvider, SMProvider>();
  }

  protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
    base.ConfigureModuleCatalog(moduleCatalog);
    moduleCatalog.AddModule<ResourceModule.ResourceModule>();
    moduleCatalog.AddModule<CpuModule.CpuModule>();
    moduleCatalog.AddModule<GpuModule.GpuModule>();
    moduleCatalog.AddModule<MemoryModule.MemoryModule>();
    moduleCatalog.AddModule<StorageModule.StorageModule>();
  }

  protected override void ConfigureViewModelLocator() {
    base.ConfigureViewModelLocator();
    ViewModelLocationProvider.Register<CpuSummaryView, CpuViewModel>();
    ViewModelLocationProvider.Register<GpuSummaryView, GpuViewModel>();
    ViewModelLocationProvider.Register<MemorySummaryView, MemoryViewModel>();
    ViewModelLocationProvider.Register<StorageSummaryView, StorageViewModel>();
  }
}

