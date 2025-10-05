using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
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
using static System.DateTime;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Tests
{
	public class Step1VisualTestModel : BindableBase, ITestModel
	{
		public Step1VisualTestModel(
			IDialogService dialogService,
			ITestInfo testInfo,
			IVisualTestResult testResult,
			IMachineModel machineModel,
			IEventAggregator eventAggregator)
		{
			_dialogService = dialogService;
			_machineModel = machineModel;
			_eventAggregator = eventAggregator;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = TestEntity.VisualTestEntity;
			Result = testResult;
		}

		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IVisualTestResult _result;
		public IVisualTestResult Result
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
			CheckSessionStatus_();
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			ResetResult_();
			Info.Status = TestStatus.Inprogress;
			Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{TestInProgressMessage}";
			Info.StartTime = Now;

			do
			{
				await TestLEDsAsync_();

				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping ||
						sessionModel.Status == Stopped)
				{
					Info.Status = TestStatus.Aborted;
					break;
				}

				RetryLEDsIfNecessary_();
			} while(Info.Status == TestStatus.Retry);

			if(Info.Status == TestStatus.Aborted)
			{
				Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{LEDTestMsgTitle}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}

			CheckSessionStatus_();

			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			do
			{
				TestScreenAsync_();
				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping ||
						sessionModel.Status == Stopped)
				{
					Info.Status = TestStatus.Aborted;
					break;
				}

				RetryScreenIfNecessary_();

			} while(Info.Status == TestStatus.Retry);

			if(Info.Status == TestStatus.Aborted)
			{
				Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{ScreenTestMsgTitle}{TestStoppedMessage}";
				return await Task.FromResult(Info);
			}

			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			Result.Passed = Result.LEDsStatus == true && Result.ScreenStatus == true;
			Info.Status = Result.Passed == true ? TestStatus.Passed : TestStatus.Failed;

			Info.FinishTime = Now;
			Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{Info.Status}";
			return await Task.FromResult(Info);

			void CheckSessionStatus_()
			{
				_sessionStatusSubject.OnNext(sessionModel.Status);
				using(var waitSignalEvent = new ManualResetEvent(false))
				using(FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
						.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
						.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status)))
				using(_sessionStatusSubject.Subscribe(status_ =>
				{
					if(status_ != Paused && status_ != Pausing)
					{
						var pause_ = waitSignalEvent.Set();
					}
				}))
				{
					if(sessionModel.Status == Paused)
					{
						Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{TestPausedMessage}";
					}

					if(sessionModel.Status == Pausing)
					{
						sessionModel.Status = Paused;
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
					}

					var pause_ = waitSignalEvent.WaitOne();
				}
			}

			void ResetResult_()
			{
				Result.LEDsStatus = null;
				Result.ScreenStatus = null;
				Result.Passed = null;
			}

			#region LEDs Test

			async Task TestLEDsAsync_()
			{
				_machineModel.Console.GUIIsReady = false;
				await WaitForAsync(DelayForConsoleWarningTestInSecond, cancellationToken);
				_machineModel.Console.SetAudioLevel(0);
				InvokeLEDsResultDialog_();
				_machineModel.Console.GUIIsReady = true;
			}

			void InvokeLEDsResultDialog_()
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, LEDsTestTitle },
					{ DialogMessageKey, LEDsParameters }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(Step1VisualTestDialog), parameters_, LEDsTestDialogCallback_);
				});
			}

			void LEDsTestDialogCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.Yes:
						Result.LEDsStatus = true;
						Info.Status = TestStatus.Inprogress;
						break;
					case ButtonResult.No:
						InvokeLEDsConfirmationDialog_();
						break;
				}
			}

			void InvokeLEDsConfirmationDialog_()
			{
				var confirmationParameters_ = new DialogParameters
				{
					{ DialogTitleKey, LEDsTestTitle + WhiteSpace + ConfirmationTitle },
					{ DialogMessageKey, ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), confirmationParameters_, LEDsConfirmationDialogCallback_);
				});
			}

			void LEDsConfirmationDialogCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						Result.LEDsStatus = false;
						Info.Status = TestStatus.Aborted;
						Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{LEDTestMsgTitle}{TestStoppedMessage}";
						break;
					case ButtonResult.Ignore:
						Result.LEDsStatus = false;
						Info.Status = TestStatus.Inprogress;
						break;
					case ButtonResult.Retry:
						Info.Status = TestStatus.Retry;
						break;
				}
			}

			void RetryLEDsIfNecessary_()
			{
				if(Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeLEDsRetryRationaleDialog(Rationale);
				}
			}

			void InvokeLEDsRetryRationaleDialog(string rationale)
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleLEDsTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters_, null);
				});
			}

			#endregion LEDs Test

			#region Screen Test

			void TestScreenAsync_()
			{
				InvokeScreenResultDialog_();
			}

			void InvokeScreenResultDialog_()
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, ScreenTestTitle },
					{ DialogMessageKey, ScreenParameters }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(Step1VisualTestDialog), parameters_, ScreenTestDialogCallback_);
				});
			}

			void ScreenTestDialogCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.Yes:
						Result.ScreenStatus = true;
						Info.Status = TestStatus.Inprogress;
						break;
					case ButtonResult.No:
						InvokeScreenConfirmationDialog();
						break;
				}
			}

			void InvokeScreenConfirmationDialog()
			{
				var confirmationParameters_ = new DialogParameters
				{
					{ DialogTitleKey, ScreenTestTitle + WhiteSpace + ConfirmationTitle },
					{ DialogMessageKey, ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), confirmationParameters_, ScreenConfirmationDialogCallback_);
				});
			}

			void ScreenConfirmationDialogCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						Result.ScreenStatus = false;
						Info.Status = TestStatus.Aborted;
						Info.Entity.Description = $"{Step1TestCaption}{VisualTestTitle}{ScreenTestMsgTitle}{TestStoppedMessage}";
						break;
					case ButtonResult.Ignore:
						Result.ScreenStatus = false;
						Info.Status = TestStatus.Inprogress;
						break;
					case ButtonResult.Retry:
						Info.Status = TestStatus.Retry;
						break;
				}
			}

			void RetryScreenIfNecessary_()
			{
				if(Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeScreenRetryRationaleDialog_(Rationale);
				}
			}

			void InvokeScreenRetryRationaleDialog_(string rationale)
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleScreenTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters_, null);
				});
			}

			#endregion Screen Test
		}
	}
}
