using GpuModule.Models;
using GpuModule.ViewModels;
using GpuModule.Views;
using ResourceModule.Controls.Meter;
using System.ComponentModel.DataAnnotations;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;
using static SharedDefinitions.RegionNames;

namespace GpuModule; 
public class GpuModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public GpuModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ISMProvider, SMProvider>();
    containerRegistry.RegisterSingleton<IGpuModel, GpuModel>();
    containerRegistry.RegisterSingleton<IGpuViewModel, GpuViewModel>();
    containerRegistry.RegisterSingleton<IGpuSummaryViewModel, GpuSummaryViewModel>();

    _regionManager.RegisterViewWithRegion(GpuSummaryRegionName, typeof(GpuSummaryView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    var gpuLoadMeter = containerProvider.Resolve<MeterControl>();
  }
}
