using Prism.Ioc;
using Prism.Modularity;

namespace Modules.ConsoleStateMachine;

public class ConsoleStateMachineModule(IContainerProvider containerProvider) : IModule
{
  private IContainerProvider _containerProvider = containerProvider;

  public void OnInitialized(IContainerProvider containerProvider)
  {

  }

  public void RegisterTypes(IContainerRegistry containerRegistry)
  {
    
  }
}