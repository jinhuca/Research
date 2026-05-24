using CpuModule.Models;
using CpuModule.Views;
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
    containerRegistry.Register<ISMProvider, SMProvider>();
    //containerRegistry.Register<ICpuInfoGenerator, CpuInfoGenerator>();
    //containerRegistry.RegisterSingleton<ICpuModel, CpuModel>();
    containerRegistry.RegisterSingleton<ICpuModel, CpuModel>();

    _regionManager.RegisterViewWithRegion(CpuRegionName, typeof(CpuSummaryView));
    //_regionManager.RegisterViewWithRegion(StatisticsRegionName, typeof(StatisticsView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    //IObservable<ICpuSummaryInfo2> subscription_ = CpuInfoServices.Queries.CpuInfoQueries.

  }
}
