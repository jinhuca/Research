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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Module.Infrastructure.Constants;
using Module.Infrastructure.TestResults.Implementation;
using Module.TestProcess.Services;
using Unity;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.IdleStateConstants;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static Prism.Services.Dialogs.ButtonResult;
using static System.DateTime;
using static System.Reactive.Linq.Observable;
using static System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;

namespace Module.TestProcess.Models.Tests
{
	public class Step2IdleCheckModel : BindableBase, ITestModel
	{
		public Step2IdleCheckModel(
			IUnityContainer unityContainer,
			IEventAggregator eventAggregator,
			IDialogService dialogService,
			IMachineModel machineModel,
			ITestInfo testInfo,
			IIdleStateCheckResult testResult,
			IXlsxService xlsxService,
			Dictionary<string, (string, string)> errDictionary,
			Dictionary<string, (string, string)> warnDictionary)
		{
			_unityContainer = unityContainer;
			_eventAggregator = eventAggregator;
			_dialogService = dialogService;
			_machineModel = machineModel;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = TestEntity.IdleStateCheckEntity;
			Result = testResult;
			_errDictionary = errDictionary;
			_warnDictionary = warnDictionary;
			_xlsxService = xlsxService;
			_timer = new Timer();
			_timer.AutoReset = true;
			_timer.Interval = SampleIntervalInMillisecond;
		}

		private readonly IEventAggregator _eventAggregator;
		private readonly IUnityContainer _unityContainer;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private string _excelFileGenerated = string.Empty;
		private readonly IXlsxService _xlsxService;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;
		private readonly Timer _timer;
		private readonly Dictionary<string, (string, string)> _errDictionary;
		private readonly Dictionary<string, (string, string)> _warnDictionary;
		private readonly SerialDisposable _sessionStatusSubscriber = new SerialDisposable();
		private int index_ = 1;

		private string _rationale = string.Empty;
		public string Rationale
		{
			get => _rationale;
			set => SetProperty(ref _rationale, value);
		}

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IIdleStateCheckResult _result;
		public IIdleStateCheckResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			ResetResult();
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await FromResult(Info);
			}

			_sessionStatusSubject.OnNext(sessionModel.Status);

			_sessionStatusSubscriber.Disposable = FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
				.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
				.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status));

			do
			{
				Info.StartTime = Now;
				ResetResult();

				if(!CheckSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					return Info;
				}

				Info.Status = TestStatus.Inprogress;
				Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{TestInProgressMessage}";

				await GetSampleDataAsync(cancellationToken);
				await CheckTankAsync(cancellationToken);
				await ValidateSensorDataAsync(cancellationToken, sessionModel);

				if(Info.Status == TestStatus.Aborted || !CheckSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					break;
				}

				if(_warnDictionary.Count != 0)
				{
					var warnMsgBuilder_ = new StringBuilder();
					foreach(var element in _warnDictionary)
					{
						warnMsgBuilder_.Append(element.Key + element.Value.Item1 + NewLine + element.Value.Item2 + NewLine);
					}

					var warns_ = warnMsgBuilder_.ToString();
					InvokeChangeTankDialog_(warns_);

					if(Info.Status == TestStatus.Retry)
					{
						Rationale = string.Empty;
						InvokeRetryRationaleDialog_(Rationale);
						continue;
					}
				}

				if(Info.Status == TestStatus.Aborted || !CheckSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{TestStoppedMessage}";
					return Info;
				}

				if(_errDictionary.Count != 0 && Info.Status != TestStatus.Aborted)
				{
					var errorMsgBuilder_ = new StringBuilder();
					foreach(var element in _errDictionary)
					{
						errorMsgBuilder_.Append(element.Key + element.Value.Item1 + NewLine + element.Value.Item2 + NewLine);
					}

					var errors_ = errorMsgBuilder_.ToString();
					InvokeTestFailureDialog_(errors_);

					if(Info.Status == TestStatus.Retry)
					{
						Rationale = string.Empty;
						InvokeRetryRationaleDialog_(Rationale);
						continue;
					}
				}

				if(Info.Status == TestStatus.Aborted || !CheckSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{TestStoppedMessage}";
				}
			} while(Info.Status == TestStatus.Retry);

			EvaluateResult_();
			Info.FinishTime = Now;

			var msg_ = string.Empty;
			msg_ = Result.Passed == true ? TestPassedMessage : TestFailedMessage;
			if(sessionModel.Status == Stopped)
			{
				msg_ = TestStoppedMessage;
			}

			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{msg_}";
			return await FromResult(Info);

			void EvaluateResult_()
			{
				Result.Passed = Result.FM1Avg.Passed == true && Result.TS1Avg.Passed == true;
			}

			void InvokeRetryRationaleDialog_(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleIdleStateCheck }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}

			void InvokeTestFailureDialog_(string paramIdValue)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, IdleStateCheckFailureTitle },
					{ DialogMessageKey, paramIdValue }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(StateCheckFailureDialog), parameters, ConfirmationDialogCallback_);
				});
			}

			void InvokeChangeTankDialog_(string paramIdValue)
			{
				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, ChangeTankDialogTitle },
					{ DialogMessageKey, paramIdValue }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ChangeTankDialog), parameters_, ChangeTankCallback_);
				});
			}

			void ChangeTankCallback_(IDialogResult confirmation)
			{
				switch(confirmation.Result)
				{
					case Abort:
						Info.Status = TestStatus.Aborted;
						EvaluateResult_();
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case Retry:
						Info.Status = TestStatus.Retry;
						break;
					case Ignore:
					case Cancel:
					case No:
					case None:
					case OK:
					case Yes:
					default:
						break;
				}
			}

			void ConfirmationDialogCallback_(IDialogResult confirmation)
			{
				switch(confirmation.Result)
				{
					case Abort:
						Info.Status = TestStatus.Aborted;
						EvaluateResult_();
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case Ignore:
						Info.Status = TestStatus.Failed;
						break;
					case Retry:
						Info.Status = TestStatus.Retry;
						break;
					case Cancel:
					case No:
					case None:
					case OK:
					case Yes:
					default:
						break;
				}
			}
		}

		private void ResetResult()
		{
			index_ = 1;
			Result.Passed = null;
			Result.Rationale = string.Empty;
			Result.FM1Avg = (double.NaN, null);
			Result.PT1Avg = (double.NaN, null);
			Result.LC1Avg = (double.NaN, null);
			Result.PT3Avg = (double.NaN, null);
			Result.TS1Avg = (double.NaN, null);
			Result.Details = new List<IIdleStateCheckDetails>();
		}

		private async Task GetSampleDataAsync(CancellationToken cancellationToken)
		{
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{SamplingIdleStateSensorDataMessage}";

#if DEBUG
			var rnd = new Random();
			_machineModel.SystemState = CAN_ID_STATE_IDLE;
			_machineModel.FM1Reading = Math.Round(rnd.Next(10, 30) * 1.2, 1);
			_machineModel.TS1Reading = Math.Round(rnd.Next(-24, -21) * 1.2, 1);
			_machineModel.PT1Reading = Math.Round(rnd.Next(600, 800) * 1.1, 1);
			_machineModel.LC1Reading = Math.Round(rnd.Next(10, 15) * 1.1, 1);
			_machineModel.PT3Reading = Math.Round(rnd.Next(10, 15) * 1.1, 1);
#endif

			SetVacuum(false);

			await WaitForAsync(StateSwitchIntervalInSecond);

			var count_ = 1;
			_timer.Elapsed += RecordParams_;
			_timer.Start();

			await WaitForAsync(RecordingPeriodInSecond, cancellationToken);

			_timer.Stop();
			_timer.Elapsed -= RecordParams_;

			CalculateParams();
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{SamplingIdleSensorDataFinishedMessage}";

			return;

			void RecordParams_(object s, ElapsedEventArgs e)
			{
				var item_ = new IdleStateCheckDetails
				{
					Timestamp = Now,
					Time = index_++,
					State = ParameterCheckState.IDLE,
					FM1 = _machineModel.FM1Reading,
					PT1 = _machineModel.PT1Reading,
					LC1 = _machineModel.LC1Reading,
					PT3 = _machineModel.PT3Reading,
					TS1 = _machineModel.TS1Reading
				};
				Result.Details.Add(item_);

				var msg_ = $"{Step2TestCaption}{IdleStateCheckTitle}{SamplingIdleStateSensorDataMessage} for {RecordingPeriodInSecond - 1} seconds.{Tab}{Tab}{Tab}";
				Info.Entity.Description = msg_ + count_++;
			}
		}

		private void CalculateParams()
		{
			Result.FM1Avg = (Math.Round(Result.Details.Select(x => x.FM1).Average(), RoundOneDigit), null);
			Result.PT1Avg = (Math.Round(Result.Details.Select(x => x.PT1).Average(), RoundOneDigit), null);
			Result.PT3Avg = (Math.Round(Result.Details.Select(x => x.PT3).Average(), RoundOneDigit), null);
			Result.LC1Avg = (Math.Round(Result.Details.Select(x => x.LC1).Average(), RoundOneDigit), null);
			Result.TS1Avg = (Math.Round(Result.Details.Select(x => x.TS1).Average(), RoundOneDigit), null);
		}

		private Task CheckTankAsync(CancellationToken cancellationToken)
		{
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{CheckingTankMessage}";
			_warnDictionary.Clear();

			if(Result.PT1Avg.Value < PT1AvgThreshold)
			{
				_warnDictionary.Add(PT1Identity, (Result.PT1Avg.Value + PressureUnitSymbol, PT1RangeMessage));
			}
			Result.PT1Avg = (Result.PT1Avg.Value, null);

			if(Result.LC1Avg.Value < LC1AvgThreshold)
			{
				_warnDictionary.Add(LC1Identity, (Result.LC1Avg.Value + WeightPoundSymbol, LC1RangeMessage));
			}
			Result.LC1Avg = (Result.LC1Avg.Value, null);
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{CheckedTaskMessage}";
			return CompletedTask;
		}

		private async Task ValidateSensorDataAsync(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{ValidatingIdleStateSensorDataMessage}";
			_errDictionary.Clear();

			if(Result.FM1Avg.Value > FM1AvgThreshold)
			{
				_errDictionary.Add(FM1Identity, (Result.FM1Avg.Value + FlowMeterSymbol, FM1RangeMessage));
				Result.FM1Avg = (Result.FM1Avg.Value, false);
			}
			else
			{
				Result.FM1Avg = (Result.FM1Avg.Value, true);
			}

			var timeFromStart = Now - Info.StartTime;

			while(Result.TS1Avg.Value > TS1AvgThreshold && timeFromStart != null && timeFromStart.Value.TotalSeconds < WaitingTS1TimeoutInSecond)
			{
				if(!CheckSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					return;
				}

				Info.Status = TestStatus.Inprogress;
				Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{WaitForSubCoolerTemperatureMessage}";

				await SampleTS1Value(RecordingTS1PeriodInSecond, cancellationToken);
				timeFromStart = Now - Info.StartTime;
			}

			if(Result.TS1Avg.Value > TS1AvgThreshold)
			{
				_errDictionary.Add(TS1Identity, (Result.TS1Avg.Value + CelsiusSymbol, TS1RangeMessage));
				Result.TS1Avg = (Result.TS1Avg.Value, false);
			}
			else
			{
				Result.TS1Avg = (Result.TS1Avg.Value, true);
			}

			Info.Status = _errDictionary.Count == 0 ? TestStatus.Passed : TestStatus.Failed;
			Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{ValidatingIdleStateSensorDataFinishedMessage}";
		}

		private async Task SampleTS1Value(double samplingTime, CancellationToken cancellationToken)
		{
			var count_ = 1;
			_timer.Elapsed += RecordTS1_;
			_timer.Start();

			await WaitForAsync(samplingTime, cancellationToken);

			_timer.Elapsed -= RecordTS1_;
			_timer.Stop();

			CalculateParams();
			return;

			void RecordTS1_(object s, ElapsedEventArgs e)
			{
				var item_ = new IdleStateCheckDetails
				{
					Timestamp = Now,
					Time = index_++,
					State = ParameterCheckState.IDLE,
					FM1 = _machineModel.FM1Reading,
					PT1 = _machineModel.PT1Reading,
					LC1 = _machineModel.LC1Reading,
					PT3 = _machineModel.PT3Reading,
					TS1 = _machineModel.TS1Reading
				};
				Result.Details.Add(item_);

				var msg_ = $"{Step2TestCaption}{IdleStateCheckTitle}{SampleSensorDataForTS1} in {RecordingPeriodInSecond - 1} seconds.{Tab}{Tab}{Tab}";
				Info.Entity.Description = msg_ + count_++;
			}
		}

		private void SetVacuum(bool turnOn)
		{
			if(turnOn)
			{
				_machineModel.Console.Connect();
			}
			else
			{
				_machineModel.Console.Disconnect();
			}
			_machineModel.IsVacuumDisconnected = !turnOn;
		}

		private bool CheckSessionStatus(ISessionModel sessionModel)
		{
			using(var _waitSignalEvent = new ManualResetEvent(false))
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
					Info.Entity.Description = $"{Step2TestCaption}{IdleStateCheckTitle}{TestPausedMessage}";
				}

				if(sessionModel.Status == Pausing)
				{
					sessionModel.Status = Paused;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
				}

				var pause_ = _waitSignalEvent.WaitOne();
			}

			return sessionModel.Status != Stopped && sessionModel.Status != SessionStatus.Exception;
		}
	}
}
