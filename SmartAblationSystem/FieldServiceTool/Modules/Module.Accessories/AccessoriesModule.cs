using Module.Accessories.Views;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Module.Accessories
{
 public	class AccessoriesModule:IModule
 {
	 private readonly IContainerProvider _containerProvider;
	 private readonly IRegionManager _regionManager;

		public AccessoriesModule(IContainerProvider containerProvider, IRegionManager regionManager)
		{
			_containerProvider = containerProvider;
			_regionManager = regionManager;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
			_regionManager.RegisterViewWithRegion(KnownRegionNames.AccessoriesRegionName, typeof(AccessoriesView));
		}
	}
}
