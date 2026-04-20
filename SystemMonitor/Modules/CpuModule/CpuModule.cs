using CpuModule.Models;
using CpuModule.Views;
using static SharedDefinitions.RegionNames;

namespace CpuModule;

public class CpuModule : IModule {
  private readonly IRegionManager _regionManager;
  public CpuModule() {
    
  }
  public CpuModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuModel, CpuModel>();
    _regionManager.RegisterViewWithRegion(CpuRegionName, typeof(CpuMainView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
  }

  
}
