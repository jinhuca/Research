using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Module.Infrastructure.AppLog;
using Prism.Ioc;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static System.DateTime;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Tests
{
	public class Step1InputTestModel : BindableBase, ITestModel
	{
		public Step1InputTestModel(
			IDialogService dialogService,
			ITestInfo testInfo,
			IInputTestResult testResult,
			IMachineModel machineModel,
			IEventAggregator eventAggregator,
			IContainerProvider containerProvider)
		{
			_dialogService = dialogService;
			_machineModel = machineModel;
			_eventAggregator = eventAggregator;
			_containerProvider = containerProvider;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = TestEntity.InputTestEntity;
			Result = testResult;
		}

		private readonly IEventAggregator _eventAggregator;
		private readonly IContainerProvider _containerProvider;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;
		private TestStatus _startPushButtonStatus;
		private TestStatus _stopPushButtonStatus;
		private TestStatus _startFootSwitchStatus;
		private TestStatus _stopFootSwitchStatus;
		private bool? _startPushButtonResult;
		private bool? _stopPushButtonResult;
		private bool? _startFootSwitchResult;
		private bool? _stopFootSwitchResult;

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IInputTestResult _result;
		public IInputTestResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		private string _rationale = string.Empty;
		public string Rationale
		{
			get => _rationale;
			set => SetProperty(ref _rationale, value);
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			CheckSessionStatus(InputTestTitle, sessionModel);
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			ResetResult_();
			Info.Status = TestStatus.Inprogress;
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{TestInProgressMessage}";
			Info.StartTime = Now;

			// Test Start Push button
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartPushButtonTest}{TestInProgressMessage}";
			try
			{
				_startPushButtonResult = await GetStartPushButtonTestResultAsync(cancellationToken);
			}
			catch(Exception ex)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartPushButtonTest}{TestStoppedByExceptionMessage}";
				FieldServiceTrace.LogException(ex);
				return await Task.FromResult(Info);
			}

			if(_startPushButtonStatus == TestStatus.Aborted || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartPushButtonTest}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}

			var startButtonStatus_ = _startPushButtonResult == true ? TestPassedMessage : TestFailedMessage;

			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartPushButtonTest}{startButtonStatus_}";

			// Test Stop Push button
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopPushButtonTest}{TestInProgressMessage}";
			try
			{
				_stopPushButtonResult = await GetStopPushButtonTestResultAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopPushButtonTest}{TestStoppedByExceptionMessage}";
				FieldServiceTrace.LogException(ex);
				return await Task.FromResult(Info);
			}

			if(_stopPushButtonStatus == TestStatus.Aborted || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopPushButtonTest}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}

			var stopButtonStatus_ = _stopPushButtonResult == true ? TestPassedMessage : TestFailedMessage;

			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopPushButtonTest}{stopButtonStatus_}";

			// Test Start Foot Switch 
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartFootSwitchTest}{TestInProgressMessage}";
			try
			{
				_startFootSwitchResult = await GetStartFootSwitchTestResultAsync(cancellationToken);
			}
			catch(Exception ex)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartFootSwitchTest}{TestStoppedByExceptionMessage}";
				FieldServiceTrace.LogException(ex);
				return await Task.FromResult(Info);
			}

			if(_startFootSwitchStatus == TestStatus.Aborted || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartFootSwitchTest}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}
			var startFWStatus_ = _startFootSwitchResult == true ? TestPassedMessage : TestFailedMessage;
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StartFootSwitchTest}{startFWStatus_}";

			// Test Stop Foot Switch 
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopFootSwitchTest}{TestInProgressMessage}";
			try
			{
				_stopFootSwitchResult = await GetStopFootSwitchTestResultAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopFootSwitchTest}{TestStoppedByExceptionMessage}";
				FieldServiceTrace.LogException(ex);
				return await Task.FromResult(Info);
			}

			if(_stopFootSwitchStatus == TestStatus.Aborted || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopFootSwitchTest}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}

			var stopFootSwitchStatus_ = _stopFootSwitchResult == true ? TestPassedMessage : TestFailedMessage;

			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{StopFootSwitchTest}{stopFootSwitchStatus_}";

			Result.Passed = _startPushButtonResult == true
											&& _stopPushButtonResult == true
											&& _startFootSwitchResult == true
											&& _stopFootSwitchResult == true;

			Info.Status = Result.Passed == true ? TestStatus.Passed : TestStatus.Failed;

			Info.FinishTime = Now;
			Info.Entity.Description = $"{Step1TestCaption}{InputTestTitle}{Info.Status}";

			return await Task.FromResult(Info);

			void ResetResult_()
			{
				Result.StartPushButtonStatus = null;
				Result.StopPushButtonStatus = null;
				Result.StartFootSwitch = null;
				Result.StopFootSwitch = null;
				Result.Passed = null;
			}
		}

		void CheckSessionStatus(string testTitle, ISessionModel sessionModel)
		{
			_sessionStatusSubject.OnNext(sessionModel.Status);
			using(var _waitSignalEvent = new ManualResetEvent(false))
			using(FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
							 .Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
							 .Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status)))
			using(_sessionStatusSubject.Subscribe(status_ =>
						{
							if(status_ != Paused && status_ != Pausing)
							{
								var pause_ = _waitSignalEvent.Set();
							}
						}))
			{
				if(sessionModel.Status == Paused)
				{
					Info.Entity.Description = $"{Step1TestCaption}{testTitle}{TestPausedMessage}";
				}
				if(sessionModel.Status == Pausing)
				{
					sessionModel.Status = Paused;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
				}

				_waitSignalEvent.WaitOne();
			}
		}

		private async Task<bool?> GetStartPushButtonTestResultAsync(CancellationToken cancellationToken)
		{
			var testStartPushButtonTaskResult_ = await TestStartPushButtonAsync(cancellationToken);
			Result.StartPushButtonStatus = testStartPushButtonTaskResult_ == TestStatus.Passed;
			return await Task.FromResult(Result.StartPushButtonStatus);
		}

		private async Task<TestStatus> TestStartPushButtonAsync(CancellationToken cancellationToken)
		{
			InvokeStartDialog_();
			_startPushButtonStatus = TestStartPushButton_();
			if(_startPushButtonStatus == TestStatus.Passed)
			{
				Result.StartPushButtonStatus = true;
				return await Task.FromResult(_startPushButtonStatus);
			}
			InvokeConfirmationDialog_();
			if(_startPushButtonStatus == TestStatus.Aborted)
			{
				Result.StartPushButtonStatus = false;
				return await Task.FromResult(_startPushButtonStatus);
			}
			if(_startPushButtonStatus == TestStatus.Retry)
			{
				Rationale = string.Empty;
				InvokeRetryRationaleDialog(Rationale);
				return await TestStartPushButtonAsync(cancellationToken);
			}

			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleStartPushButtonInputTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
			return await Task.FromResult(_startPushButtonStatus);

			void InvokeStartDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputTitle },
					{ DialogMessageKey, StartPushButtonTestMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
				});
			}
			void InvokeConfirmationDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputFailureTitle },
					{ DialogMessageKey, StartPushButtonFailureMessage + ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), parameters, ConfirmationCallback_);
				});
			}
			void ConfirmationCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						_startPushButtonStatus = TestStatus.Aborted;
						break;
					case ButtonResult.Ignore:
						_startPushButtonStatus = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						_startPushButtonStatus = TestStatus.Retry;
						break;
				}
			}
			TestStatus TestStartPushButton_()
			{
#if DEBUG
				//_machineModel.StartButtonPressed = true;
#endif
				using(var _startButtonSubject = new BehaviorSubject<bool>(_machineModel.StartButtonPressed))
				using(var _waitSignalEvent = new AutoResetEvent(false))
				using(FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(_machineModel.StartButtonPressed))
					.Subscribe(_ => _startButtonSubject.OnNext(_machineModel.StartButtonPressed)))
				using(_startButtonSubject.Subscribe(status => { if(status) { _waitSignalEvent.Set(); } }))
				{
					var _signaled = _waitSignalEvent.WaitOne(TimeSpan.FromSeconds(InputTestTimeoutInSecond));
					return _signaled ? TestStatus.Passed : TestStatus.Failed;
				}
			}
		}

		private async Task<bool?> GetStopPushButtonTestResultAsync(CancellationToken cancellationToken)
		{
			var testStopPushButtonTaskResult_ = await TestStopPushButtonAsync(cancellationToken);
			Result.StopPushButtonStatus = testStopPushButtonTaskResult_ == TestStatus.Passed;
			return await Task.FromResult(Result.StopPushButtonStatus);
		}

		private async Task<TestStatus> TestStopPushButtonAsync(CancellationToken cancellationToken)
		{
			InvokeStartDialog_();
			_stopPushButtonStatus = TestStopPushButton_();
			if(_stopPushButtonStatus == TestStatus.Passed)
			{
				Result.StopPushButtonStatus = true;
				return await Task.FromResult(_stopPushButtonStatus);
			}
			InvokeConfirmationDialog_();
			if(_stopPushButtonStatus == TestStatus.Retry)
			{
				Rationale = string.Empty;
				InvokeRetryRationaleDialog(Rationale);
				return await TestStopPushButtonAsync(cancellationToken);
			}

			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleStopPushButtonInputTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
			return await Task.FromResult(_stopPushButtonStatus);

			void InvokeStartDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputTitle },
					{ DialogMessageKey, StopPushButtonTestMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
				});
			}
			void InvokeConfirmationDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputFailureTitle },
					{ DialogMessageKey, StopPushButtonFailureMessage + ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), parameters, ConfirmationCallback_);
				});
			}
			void ConfirmationCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						_stopPushButtonStatus = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case ButtonResult.Ignore:
						_stopPushButtonStatus = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						_stopPushButtonStatus = TestStatus.Retry;
						break;
				}
			}
			TestStatus TestStopPushButton_()
			{
#if DEBUG
				//_machineModel.StopButtonPressed = true;
#endif
				using(var _stopButtonSubject = new BehaviorSubject<bool>(_machineModel.StopButtonPressed))
				using(var _waitSignalEvent = new AutoResetEvent(false))
				using(FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(_machineModel.StopButtonPressed))
					.Subscribe(_ => _stopButtonSubject.OnNext(_machineModel.StopButtonPressed)))
				using(_stopButtonSubject.Subscribe(status => { if(status) { _waitSignalEvent.Set(); } }))
				{
					var _signaled = _waitSignalEvent.WaitOne(TimeSpan.FromSeconds(InputTestTimeoutInSecond));
					return _signaled ? TestStatus.Passed : TestStatus.Failed;
				}
			}
		}

		private async Task<bool?> GetStartFootSwitchTestResultAsync(CancellationToken cancellationToken)
		{
			var testStartFootSwitchTaskResult_ = await TestStartFootSwitchAsync(cancellationToken);
			Result.StartFootSwitch = testStartFootSwitchTaskResult_ == TestStatus.Passed;
			return await Task.FromResult(Result.StartFootSwitch);
		}

		private async Task<TestStatus> TestStartFootSwitchAsync(CancellationToken cancellationToken)
		{
			InvokeStartDialog_();
			_startFootSwitchStatus = TestStartFootSwitch_();
			if(_startFootSwitchStatus == TestStatus.Passed)
			{
				Result.StartFootSwitch = true;
				return await Task.FromResult(_startFootSwitchStatus);
			}
			InvokeConfirmationDialog_();
			if(_startFootSwitchStatus == TestStatus.Retry)
			{
				Rationale = string.Empty;
				InvokeRetryRationaleDialog(Rationale);
				return await TestStartFootSwitchAsync(cancellationToken);
			}

			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleStartFootSwitchInputTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
			return await Task.FromResult(_startFootSwitchStatus);

			void InvokeStartDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputTitle },
					{ DialogMessageKey, StartFootSwitchMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
				});
			}
			void InvokeConfirmationDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputFailureTitle },
					{ DialogMessageKey, StartFootSwitchFailureMessage + ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), parameters, ConfirmationCallback_);
				});
			}
			void ConfirmationCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						_startFootSwitchStatus = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case ButtonResult.Ignore:
						_startFootSwitchStatus = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						_startFootSwitchStatus = TestStatus.Retry;
						break;
				}
			}
			TestStatus TestStartFootSwitch_()
			{
#if DEBUG
				//_machineModel.StartFootSwitchOn = true;
#endif
				_machineModel.Console.LockTheFootSwitch = false;
				using(var _waitSignalEvent = new AutoResetEvent(false))
				using(var _startFootSwitchSubject = new BehaviorSubject<bool>(_machineModel.StartFootSwitchOn))
				using(FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(_machineModel.StartFootSwitchOn))
					.Subscribe(_ => _startFootSwitchSubject.OnNext(_machineModel.StartFootSwitchOn)))
				using(_startFootSwitchSubject.Subscribe(status => { if(status) { _waitSignalEvent.Set(); } }))
				{
					var _signaled = _waitSignalEvent.WaitOne(TimeSpan.FromSeconds(InputTestTimeoutInSecond));
					_machineModel.Console.LockTheFootSwitch = true;
					return _signaled ? TestStatus.Passed : TestStatus.Failed;
				}
			}
		}

		private async Task<bool?> GetStopFootSwitchTestResultAsync(CancellationToken cancellationToken)
		{
			var testStopFootSwitchTaskResult_ = await TestStopFootSwitchAsync(cancellationToken);
			Result.StopFootSwitch = testStopFootSwitchTaskResult_ == TestStatus.Passed;
			return await Task.FromResult(Result.StopFootSwitch);
		}

		private async Task<TestStatus> TestStopFootSwitchAsync(CancellationToken cancellationToken)
		{
			InvokeStartDialog_();
			_stopFootSwitchStatus = TestStopFootSwitch_();
			if(_stopFootSwitchStatus == TestStatus.Passed)
			{
				Result.StopFootSwitch = true;
				InvokeEndDialog_();
				return await Task.FromResult(_stopFootSwitchStatus);
			}
			InvokeConfirmationDialog_();
			if(_stopFootSwitchStatus == TestStatus.Retry)
			{
				Rationale = string.Empty;
				InvokeRetryRationaleDialog(Rationale);
				return await TestStopFootSwitchAsync(cancellationToken);
			}
			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleStopFootSwitchInputTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
			return await Task.FromResult(_stopFootSwitchStatus);

			void InvokeStartDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputTitle },
					{ DialogMessageKey, StopFootSwitchMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
				});
			}
			void InvokeEndDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputTitle },
					{ DialogMessageKey, StopFootSwitchSuccessMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
				});
			}
			void InvokeConfirmationDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, InputFailureTitle },
					{ DialogMessageKey, StopFootSwitchFailureMessage + ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), parameters, ConfirmationCallback_);
				});
			}
			void ConfirmationCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						_stopFootSwitchStatus = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case ButtonResult.Ignore:
						_stopFootSwitchStatus = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						_stopFootSwitchStatus = TestStatus.Retry;
						break;
				}
			}
			TestStatus TestStopFootSwitch_()
			{
#if DEBUG
				//_machineModel.StopFootSwitchOn = true;
#endif
				_machineModel.Console.LockTheFootSwitch = false;
				using(var _waitSignalEvent = new AutoResetEvent(false))
				using(var _stopFootSwitchSubject = new BehaviorSubject<bool>(_machineModel.StopFootSwitchOn))
				using(FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(_machineModel.StopFootSwitchOn))
					.Subscribe(_ => _stopFootSwitchSubject.OnNext(_machineModel.StopFootSwitchOn)))
				using(_stopFootSwitchSubject.Subscribe(status => { if(status) { _waitSignalEvent.Set(); } }))
				{
					var _signaled = _waitSignalEvent.WaitOne(TimeSpan.FromSeconds(InputTestTimeoutInSecond));
					_machineModel.Console.LockTheFootSwitch = true;
					return _signaled ? TestStatus.Passed : TestStatus.Failed;
				}
			}
		}
	}
}
