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
  
  //public CpuModule() {
    
  //}

  public CpuModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuModel, CpuModel>();
    containerRegistry.Register<ISMProvider, SMProvider>();
    _regionManager.RegisterViewWithRegion(CpuRegionName, typeof(CpuMainView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    containerProvider.Resolve<ISMProvider>();
  }
}
