using CpuModule.Views;
using static SharedDefinitions.RegionNames;

namespace CpuModule;

public class CpuModule : IModule {
  private readonly IRegionManager _regionManager;

  public CpuModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    _regionManager.RegisterViewWithRegion(CpuRegionName, typeof(CpuMainView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    //IRegion region = _regionManager.Regions[CpuRegionName];
    //var view1 = containerProvider.Resolve<CpuMainView>();
    //region.Add(view1);
  }
}
