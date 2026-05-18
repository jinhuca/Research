using OsModule.Models;
using OsModule.ViewModels;
using OsModule.Views;
using System.ComponentModel.DataAnnotations;
using SystemManagementProvider;
using SystemManagementProvider.Interfaces;
using static SharedDefinitions.RegionNames;

namespace OsModule;

public class OsModule : IModule {
  [Required]
  private readonly IRegionManager _regionManager;

  public OsModule(IRegionManager regionManager) {
    _regionManager = regionManager;
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<IOperatingSystemModel, OperatingSystemModel>();
    containerRegistry.RegisterSingleton<IOperatingSystemViewModel, OperatingSystemViewModel>();
    containerRegistry.Register<ISMProvider, SMProvider>();
    _regionManager.RegisterViewWithRegion(OsRegionName, typeof(OperatingSystemSummaryView));
  }

  public void OnInitialized(IContainerProvider containerProvider) {

  }
}
