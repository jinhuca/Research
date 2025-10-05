using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Models;
using Module.TestProcess.Views;
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
using Prism.Ioc;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.SessionStatus;
using static Module.Infrastructure.TestEntities.TestEntity;
using static Module.TestProcess.Constants.TestProcessMessages;
using static System.DateTime;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Tests
{
	public class Step1VersionVerificationModel : BindableBase, ITestModel
	{
		public Step1VersionVerificationModel(
			IDialogService dialogService,
			ISystemParameters systemParametersModel,
			ITestInfo testInfo,
			IVersionTestResult testResult,
			IMachineModel machineModel,
			IEventAggregator eventAggregator)
		{
			_dialogService = dialogService;
			_machineModel = machineModel;
			_eventAggregator = eventAggregator;
			_systemParametersModel = systemParametersModel;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = VersionVerificationEntity;
			Result = testResult;
		}

		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly ISystemParameters _systemParametersModel;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IVersionTestResult _result;
		public IVersionTestResult Result
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
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Stopped;
				return await Task.FromResult(Info);
			}

			CheckSessionStatus_();
			Info.Status = TestStatus.Inprogress;
			Info.Entity.Description = $"{Step1TestCaption}{VersionVerificationTitle}{TestInProgressMessage}";
			Info.StartTime = Now;

			Result = await GetFirmwareVersionsAsync_();
			InvokeResultDialog_();
			if (Info.Status == TestStatus.Aborted)
			{
				Info.Entity.Description = $"{Step1TestCaption}{VersionVerificationTitle}{TestStoppedMessage}";
				return Info;
			}
			await RetryIfNecessary_();

			Info.FinishTime = Now;
			Info.Entity.Description = $"{Step1TestCaption}{VersionVerificationTitle}{Info.Status}";
			return await Task.FromResult(Info);

			void CheckSessionStatus_()
			{
				var _waitSignalEvent = new ManualResetEvent(false);

				FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
					.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status));

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
						Info.Entity.Description = $"{Step1TestCaption}{VersionVerificationTitle}{TestPausedMessage}";
					}

					if(sessionModel.Status == Pausing)
					{
						sessionModel.Status = Paused;
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
					}

					var pause_ = _waitSignalEvent.WaitOne();
				}
			}

			async Task<IVersionTestResult> GetFirmwareVersionsAsync_()
			{
				return await Task.FromResult(Result);
			}

			void InvokeResultDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, VersionTitle },
					{ VersionParameters, Result }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(Step1VersionVerificationDialog), parameters, VersionVerificationDialogCallback_);
				});
			}

			void VersionVerificationDialogCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.Yes:
						Info.Status = TestStatus.Passed;
						Result.Passed = true;
						break;
					case ButtonResult.No:
						InvokeConfirmationDialog_();
						break;
				}
			}

			void InvokeConfirmationDialog_()
			{
				var confirmationParameters = new DialogParameters
				{
					{ DialogTitleKey, VersionTitle + WhiteSpace + ConfirmationTitle },
					{ DialogMessageKey, ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), confirmationParameters, ConfirmationDialogCallback_);
				});
			}

			void ConfirmationDialogCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						Info.Status = TestStatus.Aborted;
						Result.Passed = false;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case ButtonResult.Ignore:
						Info.Status = TestStatus.Failed;
						Result.Passed = false;
						break;
					case ButtonResult.Retry:
						Info.Status = TestStatus.Retry;
						break;
				}
			}

			async Task RetryIfNecessary_()
			{
				if(Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeRetryRationaleDialog(Rationale);
					await Start(cancellationToken, sessionModel);
				}
			}

			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleVersionVerification }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
		}
	}
}
