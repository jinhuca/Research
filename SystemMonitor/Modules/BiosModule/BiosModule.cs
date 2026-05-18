using BiosModule.Models;
using BiosModule.ViewModels;
using BiosModule.Views;
using ResourceModule.Controls.Meter;
using System.ComponentModel.DataAnnotations;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;
using static SharedDefinitions.RegionNames;

namespace BiosModule; 
public class BiosModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public BiosModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<IBiosModel, BiosModel>();
    containerRegistry.RegisterSingleton<IBiosViewModel, BiosViewModel>();
    _regionManager.RegisterViewWithRegion(BiosRegionName, typeof(BiosSummaryView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {
    // resolve types ....
  }
}
