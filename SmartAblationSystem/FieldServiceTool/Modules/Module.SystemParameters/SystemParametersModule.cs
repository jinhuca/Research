using Module.Infrastructure;
using Module.SystemParameters.Interfaces;
using Module.SystemParameters.Models;
using Module.SystemParameters.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Module.SystemParameters
{
	public class SystemParametersModule : IModule
  {
    private readonly IContainerProvider _containerProvider;
    private readonly IRegionManager _regionManager;

    public SystemParametersModule()
    {
    }

    public SystemParametersModule(IContainerProvider containerProvider, IRegionManager regionManager)
    {
      _containerProvider = containerProvider;
      _regionManager = regionManager;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
	    containerRegistry.RegisterSingleton<ISensorParameters, SensorParametersModel>();
      containerRegistry.RegisterSingleton<ISystemParameters, SystemParametersModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
      _regionManager.RegisterViewWithRegion(KnownRegionNames.SystemParametersRegionName, typeof(SystemParametersView));
    }
  }
}
