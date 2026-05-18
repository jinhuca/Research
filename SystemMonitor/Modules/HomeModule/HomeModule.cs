
using HomeModule.Models;
using HomeModule.ViewModels;
using HomeModule.Views;
using System.ComponentModel.DataAnnotations;
using static SharedDefinitions.RegionNames;

namespace HomeModule;

public class HomeModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public HomeModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<IHomeModel, HomeModel>();
    containerRegistry.RegisterSingleton<IHomeViewModel, HomeViewModel>();
    _regionManager.RegisterViewWithRegion(HomeRegionName, typeof(HomeBarView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
  }
}
