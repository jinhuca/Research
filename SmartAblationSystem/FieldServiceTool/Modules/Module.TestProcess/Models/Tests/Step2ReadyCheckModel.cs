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
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Communication;
using Module.Infrastructure.Constants;
using Module.Infrastructure.TestResults.Implementation;
using Unity;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.ReadyStateConstants;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static Prism.Services.Dialogs.ButtonResult;
using static System.DateTime;
using static System.Reactive.Linq.Observable;
using Application = System.Windows.Application;
using Timer = System.Timers.Timer;

namespace Module.TestProcess.Models.Tests
{
	public class Step2ReadyCheckModel : BindableBase, ITestModel
	{
		public Step2ReadyCheckModel(
			IUnityContainer unityContainer,
			IEventAggregator eventAggregator,
			IDialogService dialogService,
			IMachineModel machineModel,
			ITestInfo testInfo,
			IReadyStateCheckResult testResult,
			Stopwatch stopwatch,
			Timer timer,
			CancellationToken token,
			Dictionary<string, (string, string)> errDictionary,
			IIdleStateCheckResult idleStateCheckResult,
			Dictionary<string, (string, string)> warnDictionary)
		{
			_unityContainer = unityContainer;
			_eventAggregator = eventAggregator;
			_dialogService = dialogService;
			_machineModel = machineModel;
			Info = testInfo;
			Info.Entity = TestEntity.ReadyStateCheckEntity;
			Result = testResult;
			_errDictionary = errDictionary;
			_idleStateCheckResult = idleStateCheckResult;
			_warnDictionary = warnDictionary;
			_token = token;
			_stopwatch = stopwatch;
			_timer = timer;
			_timer.AutoReset = true;
			_timer.Interval = SampleIntervalInMillisecond;
			_eventAggregator.GetEvent<TimeEvent>().Subscribe(OnReceiveTestTime);

			IsCMCUReady = (_machineModel.CMCUSystemStatusError & (long)CMCUStatusError.CMCUReady) == (long)CMCUStatusError.CMCUReady;
			IsPMCUReady = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.PMCUReady) == (long)PMCUStatusError.PMCUReady;

			_catheterStateSubject = new BehaviorSubject<bool>(IsCatheterReady);
			_systemStateSubject = new BehaviorSubject<MessageStateId>(_machineModel.SystemState);

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(e => e.EventArgs.PropertyName == nameof(_machineModel.CMCUSystemStatusError)
										|| e.EventArgs.PropertyName == nameof(_machineModel.PMCUSystemStatusErrorCode)
										|| e.EventArgs.PropertyName == nameof(_machineModel.CatheterSerialNumber))
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(e => HandleCMCUPMCUSystemStatusError(e.EventArgs));

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(e => e.EventArgs.PropertyName == nameof(_machineModel.SystemState))
				.Subscribe(_ => _systemStateSubject.OnNext(_machineModel.SystemState));
		}

		private readonly IUnityContainer _unityContainer;
		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly IIdleStateCheckResult _idleStateCheckResult;
		private readonly Stopwatch _stopwatch;
		private readonly Timer _timer;
		private readonly CancellationToken _token;
		private readonly Dictionary<string, (string, string)> _errDictionary;
		private readonly Dictionary<string, (string, string)> _warnDictionary;
		private readonly AutoResetEvent _consoleSwitchState = new AutoResetEvent(false);
		private readonly AutoResetEvent _catheterReadyEvent = new AutoResetEvent(false);

		private readonly ISubject<bool> _catheterStateSubject;
		private readonly ISubject<bool> _cmcuExceptionType5Subject = new BehaviorSubject<bool>(false);
		private bool _isCmcuExceptionType5 = false;
		private readonly ISubject<MessageStateId> _systemStateSubject;
		private readonly ISubject<SessionStatus> _sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
		private readonly SerialDisposable _sessionStatusSubscriber = new SerialDisposable();

		public bool IsCatheterReady => IsCMCUReady && IsPMCUReady && _machineModel.CatheterSerialNumber != 0;
		public bool IsCMCUReady { get; private set; }
		public bool IsPMCUReady { get; private set; }

		private bool _IsVacuumOn;
		public bool IsVacuumOn
		{
			get => _IsVacuumOn;
			set => SetProperty(ref _IsVacuumOn, value);
		}

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private string _rationale = string.Empty;
		public string Rationale
		{
			get => _rationale;
			set => SetProperty(ref _rationale, value);
		}

		private IReadyStateCheckResult _result;
		public IReadyStateCheckResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		private void HandleCMCUPMCUSystemStatusError(PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_machineModel.PMCUSystemStatusErrorCode):
					_machineModel.IsCatheterCableConnected = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.CatheterCableConnected) == (long)PMCUStatusError.CatheterCableConnected;
					if(!_machineModel.IsCatheterCableConnected)
					{
						_machineModel.CatheterSerialNumber = 0;
					}
					IsPMCUReady = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.PMCUReady) == (long)PMCUStatusError.PMCUReady;
					_catheterStateSubject.OnNext(IsCatheterReady);
					break;
				case nameof(_machineModel.CMCUSystemStatusError):
					var currentExceptionType5 = (_machineModel.CMCUSystemStatusError & (long)CMCUStatusError.ExceptionType5) == (long)CMCUStatusError.ExceptionType5;

					// Update only when the value changed
					if(_isCmcuExceptionType5 != currentExceptionType5)
					{
						_cmcuExceptionType5Subject.OnNext(currentExceptionType5);
					}

					_isCmcuExceptionType5 = currentExceptionType5;

					IsCMCUReady = (_machineModel.CMCUSystemStatusError & (long)CMCUStatusError.CMCUReady) == (long)CMCUStatusError.CMCUReady;
					_catheterStateSubject.OnNext(IsCatheterReady);
					break;
				case nameof(_machineModel.CatheterSerialNumber):
					_catheterStateSubject.OnNext(IsCatheterReady);
					break;
				default:
					break;
			}
		}

		private void OnReceiveTestTime(DateTime time) => Info.StartTime = time;

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			ResetResult_();

			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			// Initialize sessionStatus observable 
			_sessionStatusSubject.OnNext(sessionModel.Status);
			_sessionStatusSubscriber.Disposable = FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
				.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
				.Subscribe(_ =>
				{
					_sessionStatusSubject.OnNext(sessionModel.Status);
					_systemStateSubject.OnNext(_machineModel.SystemState);
				});

			CheckSessionStatus(sessionModel);
			if(sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return Info;
			}

			Info.StartTime = Now;
			Info.Status = TestStatus.Inprogress;
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{TestInProgressMessage}";

#if DEBUG
			var rnd = new Random();
			Result.FM1Avg = (23.1, null);
			Result.PT1Avg = (657.2, null);
			Result.LC1Avg = (14.3, null);
			Result.OBPAvg = (-9.1, null);
			Result.OBPMax = (-13.3, null);
			Result.IBPAvg = (-10.2, null);
			await WaitForAsync(1);
#endif
#if !DEBUG
			var _isInIdleState = await VerifyIdleStateAsync(cancellationToken, sessionModel);
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{RetrievingCatheterConnectionMessage}";
			var _isCatheterReady = await VerifyCatheterReadyAsync(cancellationToken, sessionModel);

			if(!_isCatheterReady || NeedToCancelTest_())
			{
				Info.Status = TestStatus.Aborted;
				return Info;
			}

			var _isSystemReady = VerifyReadyStateSync(sessionModel);

			if(!_isSystemReady || NeedToCancelTest_())
			{
				Info.Status = TestStatus.Aborted;
				return Info;
			}
#endif

			await WaitForAsync(DelayReadyStateDataCollectionInSecond);

			if(NeedToCancelTest_())
			{
				return Info;
			}

			await GetSampleDataAsync(cancellationToken);

			ValidateTankData(cancellationToken);
			ValidateSensorData();

			if(NeedToCancelTest_())
			{
				return Info;
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

				if(NeedToCancelTest_())
				{
					return Info;
				}

				if(Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeRetryRationaleDialog_(Rationale);
					return await Start(cancellationToken, sessionModel);
				}
			}

			if(NeedToCancelTest_())
			{
				return Info;
			}

			if(_errDictionary.Count != 0)
			{
				var errorMsgBuilder = new StringBuilder();
				foreach(var element in _errDictionary)
				{
					errorMsgBuilder.Append(element.Key + element.Value.Item1 + NewLine + element.Value.Item2 + NewLine);
				}
				var errors = errorMsgBuilder.ToString();
				InvokeCheckFailureDialog_(errors);

				if(NeedToCancelTest_())
				{
					return Info;
				}

				if(Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeRetryRationaleDialog_(Rationale);
					return await Start(cancellationToken, sessionModel);
				}
			}

			await VerifyIdleStateAsync(cancellationToken, sessionModel);
			Info.FinishTime = Now;
			return await Task.FromResult(Info);

			void ResetResult_()
			{
				Result.Passed = null;
				Result.Rationale = string.Empty;
				Result.FM1Avg = (double.NaN, null);
				Result.PT1Avg = (double.NaN, null);
				Result.LC1Avg = (double.NaN, null);
				Result.IBPAvg = (double.NaN, null);
				Result.OBPAvg = (double.NaN, null);
				Result.OBPMax = (double.NaN, null);
				Result.Details = new List<IReadyStateCheckDetails>();
			}

			bool NeedToCancelTest_()
			{
				CheckSessionStatus(sessionModel);
				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
				{
					Info.Status = TestStatus.Aborted;
					SetVacuum(false);
					Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{TestStoppedMessage}";
					return true;
				}

				return false;
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

			void InvokeCheckFailureDialog_(string paramIdValue)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, ReadyStateCheckFailureTitle },
					{ DialogMessageKey, paramIdValue }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(StateCheckFailureDialog), parameters, ConfirmationDialogCallback_);
				});
			}

			void ConfirmationDialogCallback_(IDialogResult confirmation)
			{
				switch(confirmation.Result)
				{
					case Abort:
						Info.Status = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case Ignore:
						Info.Status = TestStatus.Failed;
						break;
					case Retry:
						Info.Status = TestStatus.Retry;
						break;
				}
			}

			void InvokeRetryRationaleDialog_(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleReadyStateCheck }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}
		}

		private async Task<bool> VerifyIdleStateAsync(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{VerifySystemStateInIdle}";
			SetVacuum(false);
			await WaitForAsync(StateSwitchIntervalInSecond);

			bool _isIdleState;
			using(_systemStateSubject.ObserveOn(TaskPoolScheduler.Default).Subscribe(state_ =>
			{
				if(_machineModel.SystemState == CAN_ID_STATE_IDLE)
				{
					_consoleSwitchState.Set();
				}
			}))
			{
				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping)
				{
					Info.Status = TestStatus.Aborted;
					return false;
				}
				_isIdleState = _consoleSwitchState.WaitOne(CHECK_IDLE_STATE_TIMEOUT_IN_SEC);
			}
			if(_isIdleState)
			{
				Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{ConfirmSystemStateInIdle}";
				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping)
				{
					Info.Status = TestStatus.Aborted;
					return false;
				}
			}
			else
			{
				Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{StartingSystemIsNotIdle}";
				await VerifyIdleStateAsync(cancellationToken, sessionModel);
			}
			return await Task.FromResult(_isIdleState);
		}

		private async Task<bool> VerifyCatheterReadyAsync(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{ConnectCatheterMessage}";
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping)
			{
				await Task.FromResult(false);
			}

			while(VerifyCatheterReadySync(sessionModel))
			{
				if(VerifyCatheterType(sessionModel) || Info.Status == TestStatus.Aborted)
				{
					break;
				}

				Task.Delay(TimeSpan.FromSeconds(3)).Wait(cancellationToken);
			}

			return Info.Status != TestStatus.Aborted;
		}

		private bool VerifyCatheterReadySync(ISessionModel sessionModel)
		{
			_catheterReadyEvent.Reset();
			using(_catheterStateSubject.ObserveOn(TaskPoolScheduler.Default)
							 .Subscribe(isReady =>
							 {
								 if(isReady)
									 _catheterReadyEvent.Set();
							 }))
			{
				while(!IsCatheterReady)
				{
					if(!_machineModel.IsCatheterCableConnected)
						InvokeCatheterConnectionDialog();
					else
					{
						InvokeRetryStopDialog();
						if(Info.Status == TestStatus.Aborted)
						{
							break;
						}
					}

					CheckSessionStatus(sessionModel);

					_catheterReadyEvent.WaitOne(CHECK_CATHETER_READY_TIMEOUT_IN_MILISEC);

					if(sessionModel.Status == Stopped)
					{
						Info.Status = TestStatus.Aborted;
						return false;
					}
				}
			}

			return IsCatheterReady;
		}

		private void InvokeCatheterConnectionDialog()
		{
			var parameters = new DialogParameters {
				{ DialogTitleKey, ConnectCatheterDialogTitle },
				{ DialogMessageKey, ConnectCatheterMessage }
			};
			Application.Current.Dispatcher.Invoke(() =>
			{
				_dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
			});
		}

		private void InvokeRetryStopDialog()
		{
			var parameters = new DialogParameters
			{
				{ DialogTitleKey, CatheterInvalidDialogTitle },
				{ DialogMessageKey, CatheterConnectionFailureMessage }
			};
			Application.Current.Dispatcher.Invoke(() =>
			{
				_dialogService.ShowDialog(nameof(RetryStopDialog), parameters, RetryStopCallback);
			});
		}

		private void RetryStopCallback(IDialogResult dialogResult)
		{
			switch(dialogResult.Result)
			{
				case Retry:
					Info.Status = TestStatus.Inprogress;
					break;
				case Abort:
					Info.Status = TestStatus.Aborted;
					_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
					Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckFailureTitle}{WhiteSpace}{StopMessage}";
					_systemStateSubject.OnNext(_machineModel.SystemState);
					break;
			}
		}

		private bool VerifyReadyStateSync(ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{CheckReadyStateMessage}";
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((HoldException, Now));
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((IgnoreCmcuExceptionType5, Now));
			bool _isSystemReadyState = false;

			using(_cmcuExceptionType5Subject.ObserveOn(TaskPoolScheduler.Default)
							 .Subscribe(exceptionType5 =>
							 {
								 if(exceptionType5)
								 {
									 DisplayException5MessageDialog(sessionModel);
								 }
							 }))
			using(_systemStateSubject.ObserveOn(TaskPoolScheduler.Default)
							 .Throttle(TimeSpan.FromSeconds(4))
							 .Subscribe(state_ =>
							 {
								 if(state_ == CAN_ID_STATE_READY || Info.Status == TestStatus.Aborted)
								 {
									 _consoleSwitchState.Set();
								 }
							 }))
			{
				SetVacuum(true);

				while(!_consoleSwitchState.WaitOne(WaitForSystemToReadyInMillisecond))
				{
					CheckSessionStatus(sessionModel);
					if(Info.Status == TestStatus.Aborted || sessionModel.Status == Stopped)
					{
						break;
					}
				}

				_isSystemReadyState = _machineModel.SystemState == CAN_ID_STATE_READY;
			}

			_eventAggregator.GetEvent<UserCommandEvent>().Publish((EnableCmcuExceptionType5, Now));
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((ResetHoldException, Now));

			Info.Entity.Description = _isSystemReadyState
				? $"{Step2TestCaption}{ReadyStateCheckTitle}{SystemIsReadyMessage}"
				: $"{Step2TestCaption}{ReadyStateCheckTitle}{SystemFailsToSwitchReadyMessage}";

			return _isSystemReadyState;
		}

		private async Task GetSampleDataAsync(CancellationToken cancellationToken)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				return;
			}

			var count_ = 1;
			var time_ = 1;

			_timer.Elapsed += RecordParams_;
			_timer.Start();
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{SampleReadyStateSensorDataMessage}";

			await WaitForAsync(RecordingPeriodInSecond, cancellationToken);

			_timer.Stop();
			_timer.Elapsed -= RecordParams_;

			CalculateParams_();
			return;

			void RecordParams_(object s, ElapsedEventArgs e)
			{
				var item_ = new ReadyStateCheckDetails
				{
					Timestamp = Now,
					Time = time_++,
					State = ParameterCheckState.READY,
					FM1 = _machineModel.FM1Reading,
					PT1 = _machineModel.PT1Reading,
					LC1 = _machineModel.LC1Reading,
					IBP = _machineModel.CP1Reading,
					OBP = _machineModel.CP2Reading
				};
				Result.Details.Add(item_);

				var msg_ = $"{Step2TestCaption}{ReadyStateCheckTitle}{SampleReadyStateSensorDataMessage} in {RecordingPeriodInSecond - 1} seconds.{Tab}{Tab}";
				Info.Entity.Description = msg_ + count_++;
			}
			void CalculateParams_()
			{
				Result.FM1Avg = (Math.Round(Result.Details.Select(x => x.FM1).Average(), RoundOneDigit), null);
				Result.PT1Avg = (Math.Round(Result.Details.Select(x => x.PT1).Average(), RoundOneDigit), null);
				Result.LC1Avg = (Math.Round(Result.Details.Select(x => x.LC1).Average(), RoundOneDigit), null);
				Result.IBPAvg = (Math.Round(Result.Details.Select(x => x.IBP).Average(), RoundTwoDigits), null);
				Result.OBPAvg = (Math.Round(Result.Details.Select(x => x.OBP).Average(), RoundOneDigit), null);
				Result.OBPMax = (Math.Round(Result.Details.Select(x => x.OBP).Max(), RoundOneDigit), null);
			}
		}

		private void EvaluateResult()
		{
			Result.Passed = Result.FM1Avg.Passed == true && Result.OBPMax.Passed == true && Result.IBPAvg.Passed == true;
		}

		private void ValidateTankData(CancellationToken cancellationToken)
		{
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{CheckingTankMessage}";

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

			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{CheckedTaskMessage}";

			EvaluateResult();
		}

		private void ValidateSensorData()
		{
			Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{ValidateReadyStateSensorsDataMessage}";
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

			var ibpReadyThreshold_ = BalloonPressureThreshold - (_idleStateCheckResult.PT3Avg.Value + IBPDelta);
			if(Result.IBPAvg.Value > ibpReadyThreshold_)
			{
				var value_ = Result.IBPAvg.Value + PressureUnitSymbol;
				var msg_ = IBPRangeMessage + ibpReadyThreshold_ + PressureUnitSymbol + RightParenthesis + Period + NewLine;
				_errDictionary.Add(IBPIdentity, (value_, msg_));
				Result.IBPAvg = (Result.IBPAvg.Value, false);
			}
			else
			{
				Result.IBPAvg = (Result.IBPAvg.Value, true);
			}

			var obpReadyThreshold_ = BalloonPressureThreshold - (_idleStateCheckResult.PT3Avg.Value + IBPDelta);
			if(Result.OBPMax.Value > obpReadyThreshold_)
			{
				var value_ = Result.OBPMax.Value + PressureUnitSymbol;
				var msg_ = OBPRangeMessage + obpReadyThreshold_ + PressureUnitSymbol + RightParenthesis + Period + NewLine;
				_errDictionary.Add(OBPIdentity, (value_, msg_));
				Result.OBPMax = (Result.OBPMax.Value, false);
			}
			else
			{
				Result.OBPMax = (Result.OBPMax.Value, true);
			}
			Info.Status = _errDictionary.Count == 0 ? TestStatus.Passed : TestStatus.Failed;

			EvaluateResult();
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

		private void UpdateVolume(int volume)
		{
			_machineModel.Console.SetAudioLevel((uint)volume);
		}

		private void DisplayException5MessageDialog(ISessionModel sessionModel)
		{
			var parameters = new DialogParameters
			{
				{ CurrentVolumeParameterKey, 50 },
				{ UpdateVolumeActionParameterKey, (Action<int>)UpdateVolume }
			};

			Application.Current.Dispatcher.Invoke(() =>
			{
				_dialogService.ShowDialog(nameof(CatheterMechanicalPopupView), parameters, (result) => HandleCatheterDisconnectCallback(sessionModel, result));
			});
		}

		private void HandleCatheterDisconnectCallback(ISessionModel sessionModel, IDialogResult result)
		{
			ResetConsoleSystem();
			switch(result.Result)
			{
				case OK:
					Info.Status = TestStatus.Inprogress;
					_cmcuExceptionType5Subject.OnNext(false);
					Task.Run(() => VerifyCatheterAndTurnOnVacuum(sessionModel));
					break;
				case Cancel:
					Info.Status = TestStatus.Aborted;
					_systemStateSubject.OnNext(_machineModel.SystemState);
					break;
			}
		}

		private void VerifyCatheterAndTurnOnVacuum(ISessionModel sessionModel)
		{
			var isCatheterReady = VerifyCatheterReadySync(sessionModel);
			if(isCatheterReady)
			{
				SetVacuum(true);
			}
		}

		private void ResetConsoleSystem()
		{
			_machineModel?.Console?.FailResetEnable();
			Thread.Sleep(10);
			_machineModel?.Console?.FailResetDisable();
			Thread.Sleep(10);

			SetVacuum(false);
			Thread.Sleep(50);
		}

		private void CheckSessionStatus(ISessionModel sessionModel)
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
					Info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckTitle}{TestPausedMessage}";
				}

				if(sessionModel.Status == Pausing)
				{
					sessionModel.Status = Paused;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
				}

				var pause_ = _waitSignalEvent.WaitOne();
			}
		}

		private bool VerifyCatheterType(ISessionModel sessionModel)
		{
			if((_machineModel.CatheterID & ~_machineModel.EngineeringCatheterSignature) != POLARxFITCatheterId)
			{
				DisplayInvalidCatheterIdDialog();
				return false;
			}

			return true;
		}

		private void DisplayInvalidCatheterIdDialog()
		{
			var parameters = new DialogParameters
			{
				{ DialogTitleKey, CatheterIdVerificationDialogTitle },
				{ DialogMessageKey, POLARxFITCatheterIsExpectedMessage },
				{ RetryButtonTextKey, ContinueText }
			};
			Application.Current.Dispatcher.Invoke(() =>
			{
				_dialogService.ShowDialog(nameof(RetryStopDialog), parameters, RetryStopCallback);
			});
		}
	}
}
