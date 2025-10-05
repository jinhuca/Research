using Prism.Ioc;
using Prism.Modularity;
using SmartFreezeApp.Modules.ModuleName;
using SmartFreezeApp.Services;
using SmartFreezeApp.Services.Interfaces;
using SmartFreezeApp.Views;
using System.Windows;
using Modules.CanBusCommunication;
using Modules.ConsoleStateMachine;
using Modules.Infrastructures;

namespace SmartFreezeApp
{
  public partial class App
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
    }
    
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<IMessageService, MessageService>();
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
      moduleCatalog.AddModule<InfrastructureModule>();
      moduleCatalog.AddModule<CanBusCommunication>();
      moduleCatalog.AddModule<ConsoleStateMachineModule>();
      moduleCatalog.AddModule<ModuleNameModule>();
    }

    protected override Window CreateShell()
    {
      return Container.Resolve<MainWindow>();
    }
  }
}
