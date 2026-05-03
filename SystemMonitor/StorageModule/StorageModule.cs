using SharedDefinitions;
using StorageModule.Models;
using StorageModule.ViewModels;
using StorageModule.Views;
using System.ComponentModel.DataAnnotations;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;
using static SharedDefinitions.RegionNames;

namespace StorageModule; 
public class StorageModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public StorageModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
   containerRegistry.RegisterSingleton<IStorageModel, StorageModel>();
    containerRegistry.Register<IStorageViewModel, StorageViewModel>();
    containerRegistry.Register<ISMProvider, SMProvider>();
    _regionManager.RegisterViewWithRegion(StorageRegionName, typeof(StorageSummaryView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    
  }
}
