using Communication;
using Prism.Ioc;
using RS232Communication;
using Shared;
using SmartAblationSystem.ViewModels;
using SmartAblationSystem.Views;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static LogSystem.LogService;

namespace SmartAblationSystem
{
  /// <summary>
  /// Interaction logic for App.xaml.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public partial class App
  {
    private static readonly string _simulationModeParameter = "SIMULATION";

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
      SetupExceptionTracing();
      CleanUpToolsFolders();
      SetupLogService();
    }
    
    private static void SetupLogService()
    {
      Task.Run(CreateLogFile).ContinueWith(delegate { SubscribeCleanupLog(); });
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      if (IsInSimulationMode())
      {
        //Register Simulator components in simulation mode 
        RegisterSimulatorTypes(containerRegistry);
      }
      else
      {
        containerRegistry.Register<ICanBusCommunication, CanBusCommunication>();
        containerRegistry.Register<IGeneralPurposeInputOutput, GeneralPurposeInputOutput>();
        containerRegistry.Register<IDisplayConfigurationMonitor, DisplayConfigurationMonitor>();
      }

      containerRegistry.Register<ISerialPortManager, SerialPortManager>();
      containerRegistry.RegisterSingleton<CommonViewModel>();
      containerRegistry.RegisterSingleton<CryoTherapyViewModel>();
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

		private void SetupExceptionTracing()
    {
      AppDomain.CurrentDomain.UnhandledException += (s, e) =>
      {
        LogException((Exception)e.ExceptionObject);
      };

      Current.DispatcherUnhandledException += (s, e) =>
      {
        LogException(e.Exception);
        e.Handled = true;
      };

      TaskScheduler.UnobservedTaskException += (s, e) =>
      {
        LogException(e.Exception);
        e.SetObserved();
      };
    }

    private void CleanUpToolsFolders()
    {
      Task.Run(() => CleanUp(ConfigurationManager.AppSettings["ServiceToolPath"]));
      Task.Run(() => CleanUp(ConfigurationManager.AppSettings["CatheterToolPath"]));
      Task.Run(() => CleanUp(ConfigurationManager.AppSettings["SystemUpdaterPath"]));
    }

    private void CleanUp(string folderpath)
    {
      if (folderpath != null && Directory.Exists(folderpath))
      {
        try
        {
          Directory.Delete(folderpath, true);
        }
        catch(Exception e)
        {
          LogException(e);
        }
      }
    }
  }
}