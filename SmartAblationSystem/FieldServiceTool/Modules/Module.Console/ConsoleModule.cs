using Prism.Ioc;
using Prism.Modularity;
using Console;
using DataAccessLayer;
using MicroLibrary;
using Module.Console.Helpers;
using Module.Console.Models;
using Module.Infrastructure.Controls;
using Tank = Console.Tank;

namespace Module.Console
{
  public class ConsoleModule : IModule
  {
    private IContainerProvider _containerProvider;
    private ConsoleErrorManager _errorHandlingManager;

    public ConsoleModule(IContainerProvider containerProvider)
    {
      _containerProvider = containerProvider;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<ICacheableDataAccess, DataAccess>();
      containerRegistry.RegisterSingleton<CentralMicroControllerPID>();
      containerRegistry.RegisterSingleton<PatientMicroControllerPID>();
      containerRegistry.RegisterSingleton<CatheterValidator>();
      containerRegistry.RegisterSingleton<Tank>();
      containerRegistry.Register<MicroTimer>();
      containerRegistry.RegisterDialog<ErrorMessageDialog, ErrorMessageDialogViewModel>();
      containerRegistry.RegisterSingleton<ConsoleErrorManager>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
      _errorHandlingManager = containerProvider.Resolve<ConsoleErrorManager>();
    }
  }
}
