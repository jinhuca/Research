using Module.Infrastructure;
using Module.Summary.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Module.Summary
{
	public class SummaryModule : IModule
  {
    private readonly IContainerProvider _containerProvider;
    private readonly IRegionManager _regionManager;

    public SummaryModule(IContainerProvider containerProvider, IRegionManager regionManager)
    {
      _containerProvider = containerProvider;
      _regionManager = regionManager;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
      
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
      _regionManager.RegisterViewWithRegion(KnownRegionNames.SummaryRegionName, typeof(SummaryView));
    }
  }
}
