using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Helpers;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestInterfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ServiceToolApp.Definitions;
using ServiceToolApp.Models;
using ServiceToolApp.Properties;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Module.Infrastructure.Constants;
using Module.TestProcess.Views.Dialogs;
using static System.DateTime;
using static Communication.CanBusMessageDefinition;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.SessionStatus;
using static Prism.Services.Dialogs.ButtonResult;
using static ServiceToolApp.Definitions.Constants;
using static System.Reactive.Linq.Observable;
using Timer = System.Timers.Timer;

namespace ServiceToolApp.ViewModels
{
	public class ShellViewModel : BindableBase
	{
		public ShellViewModel(
			ShellModel shellModel,
			IMachineModel machineModel,
			IEventAggregator eventAggregator,
			IDialogService dialogService,
			Timer volumeControlVisibilityTimer,
			IContainerProvider containerProvider)
		{
			_shellModel = shellModel;
			_machineModel = machineModel;
			_shellModel.PropertyChanged += _shellModel_PropertyChanged;
			_eventAggregator = eventAggregator;
			_dialogService = dialogService;
			_containerProvider = containerProvider;

			_volumeControlVisibilityTimer = volumeControlVisibilityTimer;
			_volumeControlVisibilityTimer.Interval = VolumeIntervalInMillisecond;
			_volumeControlVisibilityTimer.Elapsed += _volumeControlVisibilityTimerOnElapsed;

			StartCommand = new DelegateCommand<object>(OnStartCommand).ObservesCanExecute(() => IsStartCommandEnabled);
			PauseResumeCommand = new DelegateCommand<object>(OnPauseResumeCommand).ObservesCanExecute(() => IsPauseCommandEnabled);
			StopCommand = new DelegateCommand<object>(OnStopCommand).ObservesCanExecute(() => IsStopCommandEnabled);
			InvokeVolumeControlCommand = new DelegateCommand<object>(OnInvokeVolumeControlCommand).ObservesCanExecute(() => IsVolumeControlEnabled);
			DecreaseVolumeCommand = new DelegateCommand<object>(OnDecreaseVolume).ObservesCanExecute(() => IsVolumeChangeEnabled);
			IncreaseVolumeCommand = new DelegateCommand<object>(OnIncreaseVolume).ObservesCanExecute(() => IsVolumeChangeEnabled);
			HomeCommand = new DelegateCommand<object>(OnHomeCommand).ObservesCanExecute(() => IsGoHomeEnabled);
			TurnOffCommand = new DelegateCommand<object>(OnTurnOffCommand).ObservesCanExecute(() => IsTurnOffEnabled);
			SimulateExceptionCommand = new DelegateCommand<object>(OnSimulateException).ObservesCanExecute(() => IsSimulateEnabled);
			InitializeVolume();
			IsUSBAvailableViewModel = _shellModel.IsServiceToolAvailable;
			_usbDriveName = _shellModel?.USBDriveList?.Count > 0 ? _shellModel.USBDriveList[0]?.Name : string.Empty;
			_eventAggregator.GetEvent<GenerateReportEvent>().Subscribe(OnGeneratingReportEvent);
		}

		#region private fields

		private readonly IContainerProvider _containerProvider;
		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly ISubject<SessionStatus> _sessionStatusSubject = new BehaviorSubject<SessionStatus>(Ready);
		private readonly Timer _volumeControlVisibilityTimer;
		private readonly ShellModel _shellModel;
		private string _usbDriveName;

		#endregion private fields

		#region Properties

		public string Title { get; } = Resources.ApplicationTitle;
		public string ConsoleSNTitle { get; } = Resources.ConsoleSNTitle;
		public string AppTitle { get; } = Resources.AppTitle;
		public string TesterText { get; } = Resources.TesterTitle;
		public string Tester { get; set; } = string.Empty;
		public string StartButtonContent { get; } = Resources.StartButtonContent;
		public string StopButtonContent { get; } = Resources.StopButtonContent;
		public string WaitReportGeneration { get; } = Resources.WaitForGeneratingReport;

		private string _testerFirstName;
		public string TesterFirstName
		{
			get => _testerFirstName;
			set => SetProperty(ref _testerFirstName, value);
		}

		private string _testerLastName;
		public string TesterLastName
		{
			get => _testerLastName;
			set => SetProperty(ref _testerLastName, value);
		}

		public string ConsoleSN => _shellModel.ConsoleSerialNumber;

		private uint _volume;
		public uint Volume
		{
			get => _volume;
			set => SetProperty(ref _volume, value);
		}

		private bool _isVolumeControlEnabled;
		public bool IsVolumeControlEnabled
		{
			get => _isVolumeControlEnabled;
			set => SetProperty(ref _isVolumeControlEnabled, value);
		}

		private Visibility _volumeControlVisibility;
		public Visibility VolumeControlVisibility
		{
			get => _volumeControlVisibility;
			set => SetProperty(ref _volumeControlVisibility, value);
		}

		private Visibility _volumeChangeVisibility = Visibility.Collapsed;
		public Visibility VolumeChangeVisibility
		{
			get => _volumeChangeVisibility;
			set => SetProperty(ref _volumeChangeVisibility, value);
		}

		private bool _isVolumeChangeEnabled;
		public bool IsVolumeChangeEnabled
		{
			get => _isVolumeChangeEnabled;
			set => SetProperty(ref _isVolumeChangeEnabled, value);
		}

		public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private SessionStatus _sessionStatus;
		public SessionStatus SessionStatus
		{
			get => _sessionStatus;
			set => SetProperty(ref _sessionStatus, value);
		}

		private bool _isUSBAvailableViewModel;
		public bool IsUSBAvailableViewModel
		{
			get => _isUSBAvailableViewModel;
			set => SetProperty(ref _isUSBAvailableViewModel, value);
		}

		private MessageStateId _systemStateViewModel = MessageStateId.CAN_ID_STATE_IDLE;
		public MessageStateId SystemStateViewModel
		{
			get => _systemStateViewModel;
			set => SetProperty(ref _systemStateViewModel, value);
		}

		private ISessionModel _TestSessionModel;
		public ISessionModel TestSessionModel
		{
			get => _TestSessionModel;
			set => SetProperty(ref _TestSessionModel, value);
		}

		private bool _IsGeneratingReport;
		public bool IsGeneratingReport
		{
			get => _IsGeneratingReport;
			set => SetProperty(ref _IsGeneratingReport, value);
		}

		#endregion Properties

		#region Event Handlers

		private void _shellModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_shellModel.TesterFirstName):
					TesterFirstName = _shellModel.TesterFirstName;
					break;
				case nameof(_shellModel.TesterLastName):
					TesterLastName = _shellModel.TesterLastName;
					break;
				case nameof(_shellModel.SystemStateModel):
					SystemStateViewModel = _shellModel.SystemStateModel;
					break;
				case nameof(_shellModel.IsServiceToolAvailable):
					IsUSBAvailableViewModel = _shellModel.IsServiceToolAvailable;
					_usbDriveName = IsUSBAvailableViewModel ? _shellModel.USBDriveList[0]?.Name : string.Empty;
					break;
				case nameof(_shellModel.SessionStatus):
					SessionStatus = _shellModel.SessionStatus;
					switch(SessionStatus)
					{
						case Unknown:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = true;
							IsGoHomeEnabled = true;
							break;
						case Ready:
							IsStartCommandEnabled = true;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = true;
							IsGoHomeEnabled = true;
							break;
						case Starting:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							break;
						case Started:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = true;
							IsStopCommandEnabled = true;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							break;
						case Pausing:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							break;
						case Paused:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = true;
							IsStopCommandEnabled = true;
							IsTurnOffEnabled = true;
							IsGoHomeEnabled = true;
							break;
						case Resuming:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							_shellModel.SessionStatus = Resumed;
							break;
						case Resumed:
							_shellModel.SessionStatus = Started;
							break;
						case Stopped:
							break;
						case Stopping:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							break;
						case Finishing:
							IsStartCommandEnabled = false;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = false;
							IsGoHomeEnabled = false;
							break;
						case Finished:
							IsStartCommandEnabled = true;
							IsPauseCommandEnabled = false;
							IsStopCommandEnabled = false;
							IsTurnOffEnabled = true;
							IsGoHomeEnabled = true;
							break;
						case SessionStatus.Exception:
							break;
						default:
							break;
					}
					break;
			}
		}

		private void _volumeControlVisibilityTimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			SetVolumeControlsVisibility();
		}

		private void OnGeneratingReportEvent(bool obj)
		{
			IsGeneratingReport = obj;
		}

		#endregion Event Handlers

		#region Commands

		#region StartCommand

		public DelegateCommand<object> StartCommand { get; }

		private bool _isStartCommandEnabled;
		public bool IsStartCommandEnabled
		{
			get => _isStartCommandEnabled;
			set => SetProperty(ref _isStartCommandEnabled, value);
		}

		private void OnStartCommand(object obj)
		{
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((StartTest, Now));
		}

		#endregion StartCommand

		#region PauseCommand

		public DelegateCommand<object> PauseResumeCommand { get; }

		private bool _isPauseCommandEnabled;
		public bool IsPauseCommandEnabled
		{
			get => _isPauseCommandEnabled;
			set => SetProperty(ref _isPauseCommandEnabled, value);
		}

		private void OnPauseResumeCommand(object obj)
		{
			switch(SessionStatus)
			{
				case Unknown:
				case Ready:
					break;
				case Stopped:
					break;
				case Stopping:
					break;
				case Finished:
					break;
				case Started:
				case Resumed:
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((PauseTest, Now));
					break;
				case Paused:
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((ResumeTest, Now));
					break;
				case Pausing:
				case Resuming:
					break;
				case Starting:
				case Finishing:
				case SessionStatus.Exception:
				default:
					break;
			}
		}

		#endregion PauseCommand

		#region StopCommand

		public DelegateCommand<object> StopCommand { get; }

		private const string TitleKey = DialogTitleKey;
		private const string MessageKey = DialogMessageKey;

		private const string StopTitleValue = StopTestTitleValue;
		private const string StopMessageValue = StopTestMessageValue;

		private const string FinishTitleValue = FinishTestTitleValue;
		private const string FinishMessageKey = FinishTestMessageValue;

		private bool _isStopCommandEnabled;

		public bool IsStopCommandEnabled
		{
			get => _isStopCommandEnabled;
			set => SetProperty(ref _isStopCommandEnabled, value);
		}

		private async void OnStopCommand(object obj)
		{
			switch(SessionStatus)
			{
				case Resumed:
				case Started:
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((PauseTest, Now));
					if(await Task.Run(WaitPausedStatus_))
					{
						ConfirmStopAction_();
					}
					break;
				case Paused:
					ConfirmStopAction_();
					break;
				case Finished:
				case Unknown:
				case Ready:
				case Stopped:
				case Stopping:
				case Starting:
				case Pausing:
				case Resuming:
				case Finishing:
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			void ConfirmStopAction_()
			{
				using(var sessionStatusSubject = new BehaviorSubject<SessionStatus>(SessionStatus))
				using(FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
									 .Where(arg => arg.EventArgs.PropertyName == nameof(this.SessionStatus))
									 .Subscribe(_ => sessionStatusSubject.OnNext(SessionStatus)))
				{
					var stopParameters = new DialogParameters
					{
						{ TitleKey, StopTitleValue },
						{ MessageKey, StopMessageValue },
						{ Strings.SessionStatusParameterKey, sessionStatusSubject}
					};
					Application.Current.Dispatcher.Invoke(() =>
					{
						_dialogService.ShowDialog(nameof(StopTestConfirmationDialog), stopParameters, StopTestDialogCallback);
					});
				}
			}
		}

		private void StopTestDialogCallback(IDialogResult dialogResult)
		{
			switch(dialogResult.Result)
			{
				case Yes:
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
					break;
				case No:
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((ResumeTest, Now));
					break;
				case Abort:
				case Cancel:
				case Ignore:
				case None:
				case OK:
				case Retry:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion StopCommand

		#region VolumeControlCommand

		public DelegateCommand<object> InvokeVolumeControlCommand { get; set; }
		public DelegateCommand<object> DecreaseVolumeCommand { get; set; }
		public DelegateCommand<object> IncreaseVolumeCommand { get; set; }

		private void OnInvokeVolumeControlCommand(object param)
		{
			SetVolumeControlsVisibility();
		}

		private void SetVolumeControlsVisibility()
		{
			if(VolumeChangeVisibility == Visibility.Visible)
			{
				_volumeControlVisibilityTimer.Stop();
				VolumeChangeVisibility = Visibility.Hidden;
			}
			else
			{
				_volumeControlVisibilityTimer.Start();
				VolumeChangeVisibility = Visibility.Visible;
			}

			VolumeControlVisibility = VolumeChangeVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
			IsVolumeChangeEnabled = true;
		}

		private void OnDecreaseVolume(object param)
		{
			ResetVolumeControlTimer();
			if(Volume >= 10)
			{
				Volume -= 10;
				_shellModel.RequiredVolume -= 10;
			}
		}

		private void OnIncreaseVolume(object param)
		{
			ResetVolumeControlTimer();
			if(Volume <= 90)
			{
				Volume += 10;
				_shellModel.RequiredVolume += 10;
			}
		}

		private void ResetVolumeControlTimer()
		{
			_volumeControlVisibilityTimer.Stop();
			_volumeControlVisibilityTimer.Start();
		}

		#endregion VolumeControlCommand

		#region TurnOffCommand

		public DelegateCommand<object> TurnOffCommand { get; }

		private const string TurnOffTitleKey = DialogTitleKey;
		private const string TurnOffTitleValue = Constants.TurnOffTitleValue;
		private const string TurnOffMessageKey = DialogMessageKey;
		private const string TurnOffMessageValue = Constants.TurnOffMessageValue;

		private bool _isTurnOffEnabled = true;
		public bool IsTurnOffEnabled
		{
			get => _isTurnOffEnabled;
			set => SetProperty(ref _isTurnOffEnabled, value);
		}

		private async void OnTurnOffCommand(object obj)
		{
			await Task.Run(WaitForShutdownReady);
			var parameters = new DialogParameters
				{
					{ TurnOffTitleKey, TurnOffTitleValue },
					{ TurnOffMessageKey, TurnOffMessageValue }
				};
			_dialogService.ShowDialog(nameof(Dialog), parameters, TurnOffDialogCallback);
		}

		private async void TurnOffDialogCallback(IDialogResult dialogResult)
		{
			switch(dialogResult.Result)
			{
				case Yes:
					await CopyLogToUSBDrive();
					await ShutdownConsole();
					break;
				case No:
				case Abort:
				case Cancel:
				case Ignore:
				case None:
				case OK:
				case Retry:
				default:
					break;
			}
		}

		private async Task ShutdownConsole()
		{
			// 1. Wait test session stopped
			WaitForShutdownReady();

			// 2. Terminate console communication
			await _shellModel.TerminateConsole();

			CreateOnShutDownBatchFile();

			// 3. Start console computer shutdown
			//Process.Start(ShutDownProcessCmd, ShutDownProcessArguments);
			// moved to MonitorApp

			// 4. Exit
			Environment.Exit(0);
		}

		private void CreateOnShutDownBatchFile()
		{
			using(var sw = File.CreateText(MonitorConstants.OnShutdownBatchPath))
			{
				sw.WriteLine(MonitorConstants.DeleteFSTCmd);
				sw.WriteLine(MonitorConstants.DeleteOnShutdownBatchCmd);
				//sw.WriteLine(MonitorConstants.DeleteStuffs);
			}
		}

		private void WaitForShutdownReady()
		{
			var waitSignalEvent_ = new AutoResetEvent(false);
			FromEventPattern<PropertyChangedEventArgs>(_shellModel, nameof(_shellModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_shellModel.SessionStatus))
				.Subscribe(_ => _sessionStatusSubject?.OnNext(_shellModel.SessionStatus));

			using(_sessionStatusSubject.Subscribe(status_ =>
						{
							if(status_ == Stopped || status_ == Unknown || status_ == Ready || status_ == Paused || status_ == Finished || status_ == SessionStatus.Exception)
							{
								waitSignalEvent_.Set();
							}
						}))
			{
				waitSignalEvent_.WaitOne();
			}
		}

		#endregion TurnOffCommand

		#region GoSmartFreeze

		public DelegateCommand<object> HomeCommand { get; }

		private const string GoSmartFreezeTitleKey = DialogTitleKey;
		private const string GoSmartFreezeTitleValue = Constants.GoSmartFreezeTitleValue;
		private const string GoSmartFreezeMessageKey = DialogMessageKey;
		private const string GoSmartFreezeMessageValue = Constants.GoSmartFreezeMessageValue;

		private bool _isGoHomeEnabled = true;
		public bool IsGoHomeEnabled
		{
			get => _isGoHomeEnabled;
			set => SetProperty(ref _isGoHomeEnabled, value);
		}

		private void OnHomeCommand(object arg)
		{
			var parameters = new DialogParameters
			{
				{ GoSmartFreezeTitleKey, GoSmartFreezeTitleValue },
				{ GoSmartFreezeMessageKey, GoSmartFreezeMessageValue }
			};
			_dialogService.ShowDialog(nameof(Dialog), parameters, TransitDialogCallback);
		}

		private async void TransitDialogCallback(IDialogResult dialogEnd)
		{
			switch(dialogEnd.Result)
			{
				case Yes:
					IsGoHomeEnabled = false;
					await CopyLogToUSBDrive();
					CreateOnHomeBatchFile();
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((GoSmartFreeze, Now));
					await InvokeSmartFreeze();
					await TerminateServiceToolApp();
					break;
				case No:
					break;
				case Abort:
				case Cancel:
				case Ignore:
				case None:
				case OK:
				case Retry:
				default:
					break;
			}
		}

		private void CreateOnHomeBatchFile()
		{
			using(var sw = File.CreateText(MonitorConstants.OnHomeBatchPath))
			{
				sw.WriteLine(MonitorConstants.DeleteFSTCmd);
				sw.WriteLine(MonitorConstants.DeleteOnHomeBatchCmd);
			}
		}

		#endregion GoSmartFreeze

		#region Simluate Console Exception

		public DelegateCommand<object> SimulateExceptionCommand { get; }

		private bool _IsSimulateEnabled = true;
		public bool IsSimulateEnabled
		{
			get => _IsSimulateEnabled;
			set => SetProperty(ref _IsSimulateEnabled, value);
		}

		private async void OnSimulateException(object obj)
		{
			//_machineModel.CMCUSystemStatusError = 0x00000010;
			_machineModel.CMCUSystemStatusError = 0x00400000;
			_machineModel.SystemState = MessageStateId.CAN_ID_STATE_EXCEPTION;
			await ThreadHelpers.WaitForAsync(5);
			_machineModel.CMCUSystemStatusError = 0;
			_machineModel.SystemState = MessageStateId.CAN_ID_STATE_IDLE;
		}

		#endregion Simluate Console Exception

		#endregion Commands

		#region Methods

		private void InitializeVolume()
		{
			IsVolumeControlEnabled = _shellModel.VolumeCtrlEnabled;
			VolumeChangeVisibility = Visibility.Collapsed;
			Volume = _shellModel.RequiredVolume;
		}

		private async Task DeleteServiceToolAppFolder()
		{
			var applicationPath_ = ConfigurationManager.AppSettings[ServiceToolPath];
			if(Directory.Exists(applicationPath_))
			{
				try
				{
					await Task.Run(() => Directory.Delete(applicationPath_, true));
				}
				catch(Exception ex)
				{
					FieldServiceTrace.LogException(ex);
				}
			}
		}

		private bool WaitPausedStatus_()
		{
			_sessionStatusSubject.OnNext(_shellModel.SessionStatus);
			using(FromEventPattern<PropertyChangedEventArgs>(_shellModel, nameof(_shellModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_shellModel.SessionStatus))
				.Subscribe(_ => _sessionStatusSubject?.OnNext(_shellModel.SessionStatus)))
			using(var waitSignalEvent = new ManualResetEvent(false))
			using(_sessionStatusSubject.Subscribe(status_ =>
			{
				if(status_ == Paused || status_ == SessionStatus.Exception || status_ == Stopped || status_ == Ready)
				{
					waitSignalEvent.Set();
				}
			}))
			{
				waitSignalEvent.WaitOne();
				return _shellModel.SessionStatus == Paused;
			}
		}

		private void WaitStoppedStatus_()
		{
			var waitSignalEvent_ = new ManualResetEvent(false);

			FromEventPattern<PropertyChangedEventArgs>(_shellModel, nameof(_shellModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_shellModel.SessionStatus))
				.Subscribe(_ => _sessionStatusSubject?.OnNext(_shellModel.SessionStatus));

			using(_sessionStatusSubject.Subscribe(status_ =>
						{
							if(status_ == Stopped)
							{
								var pause_ = waitSignalEvent_.Set();
							}
						}))
			{
				waitSignalEvent_.WaitOne();
			}
		}

		private async Task TerminateServiceToolApp()
		{
			await _shellModel.TerminateConsole();
			await ThreadHelpers.WaitForAsync(1);
			Environment.Exit(0);
		}

		private async Task CopyLogToUSBDrive()
		{
			var target_ = _usbDriveName;
			if(string.IsNullOrEmpty(target_))
			{
				return;
			}

			var assemblyLocation_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if(assemblyLocation_ == null)
			{
				return;
			}
			var source_ = Path.Combine(assemblyLocation_, LogFolderName);

			var logDirectory_ = new DirectoryInfo(source_);
			if(!logDirectory_.Exists)
			{
				return;
			}

			await Task.Run(() =>
			{
				var targetFolderName_ = Path.Combine(target_, LogFolderName);
				try
				{
					Directory.CreateDirectory(targetFolderName_);
					foreach(var file_ in logDirectory_.GetFiles())
					{
						var targetFilePath_ = Path.Combine(targetFolderName_, file_.Name);
						file_.CopyTo(destFileName: targetFilePath_, overwrite: true);
					}
				}
				catch(Exception ex)
				{
					FieldServiceTrace.LogException(ex);
				}
			});
		}

		private async Task InvokeSmartFreeze()
		{
			var appLocation_ = ConfigurationManager.AppSettings[SmartFreezeAppPath];
			var appName_ = ConfigurationManager.AppSettings[SmartFreezeFileName];
			var smartFreezeApp_ = Path.Combine(appLocation_, appName_);

			if(!File.Exists(smartFreezeApp_))
			{
				return;
			}

			try
			{
				using(var smProcess_ = new Process())
				{
					smProcess_.StartInfo.FileName = smartFreezeApp_;
					smProcess_.StartInfo.WorkingDirectory = Path.GetDirectoryName(smartFreezeApp_) ?? string.Empty;
					smProcess_.StartInfo.CreateNoWindow = false;
					await Task.Run(() => smProcess_.Start());
				}
			}
			catch(Exception ex)
			{
				FieldServiceTrace.LogException(ex);
			}
		}

		#endregion Methods
	}
}
