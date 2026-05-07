using MemoryModule.Models;
using MemoryModule.ViewModels;
using MemoryModule.Views;
using System.ComponentModel.DataAnnotations;
using static SharedDefinitions.RegionNames;

namespace MemoryModule;

public class MemoryModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public MemoryModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<IMemoryModel, MemoryModel>();
    containerRegistry.RegisterSingleton<IMemoryViewModel, MemoryViewModel>();
    _regionManager.RegisterViewWithRegion(MemoryRegionName, typeof(MemorySummaryView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    var memoryLoadMeter = containerProvider.Resolve<ResourceModule.Controls.Meter.MeterControl>();
  }
}
