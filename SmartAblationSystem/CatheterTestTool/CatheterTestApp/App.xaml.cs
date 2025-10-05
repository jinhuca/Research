using Communication;
using Console;
using Module.Console.Interfaces;
using Module.Console.Models;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Constants;
using Module.Infrastructure.Controls;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using CatheterTestlApp;
using DataAccessLayer;
using Module.CatheterTestTool;
using Module.CatheterTestTool.Configuration;
using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.Services;
using Module.CatheterTestTool.ViewModels;
using Module.CatheterTestTool.Views;
using Module.Console;
using Module.SystemParameters.Interfaces;
using Prism.Modularity;
using static CatheterTestApp.Properties.Resources;
using Dialog = Module.Infrastructure.Controls.Dialog;
using Logon = Module.Infrastructure.Controls.Logon;

namespace CatheterTestApp
{
  public partial class App
  {
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      try
      {
        containerRegistry.RegisterSingleton<ICanBusCommunication, CanBusCommunication>();
        containerRegistry.RegisterSingleton<IGeneralPurposeInputOutput, GeneralPurposeInputOutput>();
        containerRegistry.RegisterSingleton<ICacheableDataAccess, DataAccess>();
        containerRegistry.RegisterSingleton<Machine>();
        containerRegistry.RegisterSingleton<IMachineModel, MachineModel>();
        containerRegistry.RegisterSingleton<ISensorParameters, CatheterTestSensorParametersModel>();

        containerRegistry.RegisterDialog<MessageDialog, MessageDialogViewModel>();

        containerRegistry.RegisterSingleton<ICatheterTestConfiguration, CatheterTestConfiguration>();
        containerRegistry.RegisterSingleton<ITestDataValidationService, TestDataValidationService>();
        containerRegistry.Register<ITestDataFileManager, TestDataFileManager>();
        containerRegistry.Register<ITestDataManager, TestDataManager>();
        containerRegistry.RegisterSingleton<ICatheterVisualTestService, CatheterVisualTestService>();
        containerRegistry.RegisterSingleton<ICatheterTestService, CatheterTestService>();

        containerRegistry.RegisterSingleton<LogonViewModel>();
        containerRegistry.RegisterSingleton<CatheterTestMainWindowViewModel>();
        containerRegistry.RegisterDialog<Logon, LogonViewModel>();
        containerRegistry.RegisterDialog<Dialog, DialogViewModel>();
        containerRegistry.RegisterDialog<ErrorMessageDialog, ErrorMessageDialogViewModel>();
      }
      catch (Exception e)
      {
        FieldServiceTrace.LogException(e);
        throw;
      }
    }

    protected override Window CreateShell() => Container.Resolve<CatheterTestMainWindow>();

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
      moduleCatalog.AddModule<ConsoleModule>();
      moduleCatalog.AddModule<CatheterTestToolModule>();
      base.ConfigureModuleCatalog(moduleCatalog);
    }

    protected override void OnInitialized()
    {
      StartLogon();
      base.OnInitialized();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      FieldServiceTrace.Log($"CatheterTestApp Started.");

      SetupExceptionTrace();
    }

    private void SetupExceptionTrace()
    {
      AppDomain.CurrentDomain.UnhandledException += (s, e) =>
      {
        FieldServiceTrace.LogException((Exception)e.ExceptionObject);
        LogUnhandledException();
      };

      Application.Current.DispatcherUnhandledException += (s, e) =>
      {
        FieldServiceTrace.LogException(e.Exception);
        e.Handled = true;
        LogUnhandledException();
      };

      TaskScheduler.UnobservedTaskException += (s, e) =>
      {
        FieldServiceTrace.LogException(e.Exception);
        e.SetObserved();
        LogUnhandledException();
      };
    }

    private void LogUnhandledException()
    {
      FieldServiceTrace.Log(UnhandledExceptionMsg, Level.Fatal);
    }

    private void StartLogon()
    {
      var _dialogService = Container.Resolve<IDialogService>();
      var parameters = new DialogParameters { { Strings.DialogTitleKey, Strings.DialogName } };
      _dialogService.ShowDialog(nameof(Logon), parameters, LogonCallback);
      FieldServiceTrace.Log($"Logon completed.", Level.Debug);
    }

    private void LogonCallback(IDialogResult dialogResult)
    {
      if (dialogResult.Result != ButtonResult.Yes) return;
    }
  }
}
