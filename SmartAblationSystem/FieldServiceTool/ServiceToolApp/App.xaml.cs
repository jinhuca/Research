using Communication;
using Console;
using DataAccessLayer;
using Module.Accessories;
using Module.Console;
using Module.Console.Interfaces;
using Module.Console.Models;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Constants;
using Module.Infrastructure.Controls;
using Module.Report;
using Module.Summary;
using Module.SystemParameters;
using Module.TestProcess;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using ServiceToolApp.Models;
using ServiceToolApp.ViewModels;
using ServiceToolApp.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Module.FlowMeterComm;
using static Module.Infrastructure.Constants.MonitorConstants;
using static Module.Infrastructure.SessionStatus;
using static ServiceToolApp.Properties.Resources;
using Dialog = Module.Infrastructure.Controls.Dialog;
using Logon = Module.Infrastructure.Controls.Logon;

namespace ServiceToolApp
{
	public partial class App
	{
		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			try
			{
				containerRegistry.RegisterSingleton<Machine>();
				containerRegistry.RegisterSingleton<ICanBusCommunication, CanBusCommunication>();
				containerRegistry.RegisterSingleton<IGeneralPurposeInputOutput, GeneralPurposeInputOutput>();
				containerRegistry.RegisterSingleton<ICacheableDataAccess, DataAccess>();
				containerRegistry.RegisterSingleton<IMachineModel, MachineModel>();
				containerRegistry.RegisterSingleton<ShellModel>();
				containerRegistry.RegisterSingleton<LogonViewModel>();
				containerRegistry.RegisterSingleton<ShellViewModel>();
				containerRegistry.RegisterDialog<Logon, LogonViewModel>();
				containerRegistry.RegisterDialog<Dialog, DialogViewModel>();
			}
			catch(Exception e)
			{
				FieldServiceTrace.LogException(e);
				throw;
			}
		}

		protected override Window CreateShell() => Container.Resolve<Shell>();

		protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
		{
			moduleCatalog.AddModule<InfrastructureModule>();
			moduleCatalog.AddModule<ConsoleModule>();
			moduleCatalog.AddModule<SystemParametersModule>();
			moduleCatalog.AddModule<AccessoriesModule>();
			moduleCatalog.AddModule<FlowMeterCommModule>();
			moduleCatalog.AddModule<TestProcessModule>();
			moduleCatalog.AddModule<SummaryModule>();
			moduleCatalog.AddModule<ReportModule>();
		}

		protected override void OnInitialized()
		{
			StartMonitorApp();
			StartLogon();
			base.OnInitialized();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
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
		}

		private void LogonCallback(IDialogResult dialogResult)
		{
			if(dialogResult.Result != ButtonResult.Yes) return;
			var logonViewModel = Container.Resolve<LogonViewModel>();
			var shellModel = Container.Resolve<ShellModel>();
			shellModel.TesterFirstName = logonViewModel.FirstName;
			shellModel.TesterLastName = logonViewModel.LastName;
			shellModel.SessionStatus = Ready;
		}

		private void StartMonitorApp()
		{
			var monitorAppFilePath_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if(string.IsNullOrEmpty(monitorAppFilePath_)) return;
			var sourcePath_ = Path.Combine(monitorAppFilePath_, MonitorAppIdentity);
			var destFile = Path.Combine(MonitorAppFolder, MonitorAppIdentity);

			if(!Directory.Exists(MonitorAppFolder))
			{
				try
				{
					Directory.CreateDirectory(MonitorAppFolder);
				}
				catch(IOException ioe)
				{
					FieldServiceTrace.LogException(ioe);
					return;
				}
			}

			try
			{
				File.Copy(sourcePath_, destFile, true);
			}
			catch(IOException ioe)
			{
				FieldServiceTrace.LogException(ioe);
				return;
			}

			var ps = new ProcessStartInfo(Path.Combine(MonitorAppFolder, MonitorAppIdentity))
			{
				WorkingDirectory = MonitorAppFolder
			};
			Process.Start(ps);
		}
	}
}
