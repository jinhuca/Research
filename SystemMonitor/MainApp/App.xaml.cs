using BiosModule.ViewModels;
using BiosModule.Views;
using CpuModule.ViewModels.Implementations;
using CpuModule.Views;
using GpuModule.ViewModels;
using GpuModule.Views;
using MemoryModule.ViewModels;
using MemoryModule.Views;
using OsModule.ViewModels;
using OsModule.Views;
using StorageModule.ViewModels;
using StorageModule.Views;
using System.Windows;

namespace MainApp;

public partial class App : PrismApplication {
  protected override Window CreateShell() {
    //base.InitializeModules();
    return Container.Resolve<Shell>();
  }

  protected override void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterForNavigation<CpuSummaryView>();
    containerRegistry.RegisterForNavigation<GpuSummaryView>();
    containerRegistry.RegisterForNavigation<MemorySummaryView>();
    containerRegistry.RegisterForNavigation<OperatingSystemSummaryView>();
    containerRegistry.RegisterForNavigation<StorageSummaryView>();
    containerRegistry.RegisterForNavigation<BiosSummaryView>();
  }

  protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
    base.ConfigureModuleCatalog(moduleCatalog);
    moduleCatalog.AddModule<ResourceModule.ResourceModule>();
    moduleCatalog.AddModule<OsModule.OsModule>();
    moduleCatalog.AddModule<CpuModule.CpuModule>();
    moduleCatalog.AddModule<GpuModule.GpuModule>();
    moduleCatalog.AddModule<MemoryModule.MemoryModule>();
    moduleCatalog.AddModule<StorageModule.StorageModule>();
    moduleCatalog.AddModule<BiosModule.BiosModule>();
    moduleCatalog.AddModule<LogModule.LogModule>();
  }

  protected override void ConfigureViewModelLocator() {
    base.ConfigureViewModelLocator();
    ViewModelLocationProvider.Register<Shell, ShellViewModel>();
    ViewModelLocationProvider.Register<OperatingSystemSummaryView, OperatingSystemViewModel>();
    ViewModelLocationProvider.Register<CpuSummaryView, CpuViewModel>();
    ViewModelLocationProvider.Register<GpuSummaryView, GpuViewModel>();
    ViewModelLocationProvider.Register<MemorySummaryView, MemoryViewModel>();
    ViewModelLocationProvider.Register<StorageSummaryView, StorageViewModel>();
    ViewModelLocationProvider.Register<BiosSummaryView, BiosViewModel>();
  }
}
