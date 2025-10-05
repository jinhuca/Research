
using FlowMeterComm;
using Module.FlowMeterComm.Models;
using Module.FlowMeterComm.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace Module.FlowMeterComm
{
  public class FlowMeterCommModule : IModule
  {
    private IContainerProvider _containerProvider;

    public FlowMeterCommModule(IContainerProvider containerProvider)
    {
      _containerProvider = containerProvider;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<IFlowMeterCommManager, FlowMeterCommManager>();
      containerRegistry.RegisterSingleton<IFlowMeterParameters, FlowMeterParameters>();
      containerRegistry.RegisterSingleton<IFlowMeterDataManager, FlowMeterDataManager>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
  }
}
