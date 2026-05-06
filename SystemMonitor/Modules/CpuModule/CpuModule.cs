using CpuModule.Models;
using CpuModule.Views;
using ResourceModule.Controls.Meter;
using System.ComponentModel.DataAnnotations;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;
using static SharedDefinitions.RegionNames;

namespace CpuModule;

public class CpuModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;
  
  public CpuModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<ICpuModel, CpuModel>();
    containerRegistry.Register<ISMProvider, SMProvider>();

    _regionManager.RegisterViewWithRegion(CpuRegionName, typeof(CpuSummaryView));
    _regionManager.RegisterViewWithRegion(StatisticsRegionName, typeof(StatisticsView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    //containerProvider.Resolve<ISMProvider>();
    var cpuLoadMeter = containerProvider.Resolve<MeterControl>();
  }
}
