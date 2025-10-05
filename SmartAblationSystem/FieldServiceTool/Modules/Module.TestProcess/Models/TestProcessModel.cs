using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestInterfaces;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.SessionStatus;
using static Module.Infrastructure.StepStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static System.DateTime;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models
{
	/// <summary>
	/// Class definition for <see cref="TestProcessModel"/>.
	/// </summary>
	public class TestProcessModel : BindableBase
	{
		public TestProcessModel(
			IUnityContainer container,
			IEventAggregator eventAggregator,
			IMachineModel machineModel,
			IStepModel stepModel,
			IDialogService dialogService)
		{
			_currentStepModel = stepModel;
			_dialogService = dialogService;
			_container = container;
			_machineModel = machineModel;
			_eventAggregator = eventAggregator;
			_systemStateSubject = new BehaviorSubject<MessageStateId>(CAN_ID_STATE_UNKNOWN);

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.SystemState))
				.Subscribe(_ => _systemStateSubject?.OnNext(_machineModel.SystemState));

			SubscribeEvents();
			InitializeSystemState();
			CreateTestSession();
		}

		#region private fields

		private readonly IMachineModel _machineModel;
		private readonly IUnityContainer _container;
		private readonly IEventAggregator _eventAggregator;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ISubject<MessageStateId> _systemStateSubject;
		private readonly AutoResetEvent _consoleSwitchState = new AutoResetEvent(false);
		private readonly IDialogService _dialogService;

		#endregion private fields

		#region Properties

		private ISessionModel _TestSessionModel;
		public ISessionModel TestSessionModel
		{
			get => _TestSessionModel;
			set => SetProperty(ref _TestSessionModel, value);
		}

		private IStepModel _currentStepModel;
		public IStepModel CurrentStepModel
		{
			get => _currentStepModel;
			set => SetProperty(ref _currentStepModel, value);
		}

		private ITestModel _currentTestModel;
		public ITestModel CurrentTestModel
		{
			get => _currentTestModel;
			set => SetProperty(ref _currentTestModel, value);
		}

		private double _Progress;
		public double Progress
		{
			get => _Progress;
			set => SetProperty(ref _Progress, value);
		}

		private MessageStateId _systemState;
		public MessageStateId SystemState
		{
			get => _systemState;
			set => SetProperty(ref _systemState, value);
		}

		#endregion Properties

		#region Methods

		private void InitializeSystemState()
		{
			_machineModel.SystemState = CAN_ID_STATE_IDLE;
		}

		private void CreateTestSession()
		{
			TestSessionModel = _container.Resolve<ISessionModel>();
			_cancellationTokenSource = _container.Resolve<CancellationTokenSource>();
		}

		private void SubscribeEvents()
		{
			_machineModel.PropertyChanged += _machineModel_PropertyChanged;
			_eventAggregator.GetEvent<SessionStatusEvent>().Subscribe(OnSessionStatusEvent);
		}

		private async void OnSessionStatusEvent((SessionStatus status, DateTime) sessionStatusEvent)
		{
			switch(sessionStatusEvent.status)
			{
				case SessionStatus.Unknown:
					break;
				case Ready:
					await SetReady();
					break;
				case Starting:
					await Start();
					break;
				case Started:
					break;
				case Pausing:
					await Pause();
					break;
				case Paused:
					break;
				case Resumed:
					break;
				case Resuming:
					await Resume();
					break;
				case Stopping:
					await Stop();
					break;
				case Stopped:
					TestSessionModel.PauseResumeSignal.Set();
					break;
				case Finishing:
					break;
				case SessionStatus.Finished:
					await Finish();
					break;
				case SessionStatus.Exception:
					await HandleConsoleException();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private async Task HandleConsoleException()
		{
			if(TestSessionModel == null)
			{
				return;
			}
			switch(TestSessionModel.Status)
			{
				case SessionStatus.Unknown:
				case Ready:
				case Paused:
				case Stopped:
					return;
				case Starting:
				case Started:
				case Pausing:
				case Resuming:
				case Resumed:
				case Stopping:
				case Finishing:
				case SessionStatus.Finished:
				case SessionStatus.Exception:
				default:
					await StopTestProcess();
					break;
			}
		}

		private async Task StartTestSessionAsync(CancellationToken cancellationToken)
		{
			await Task.Run(ResetTestSession);
			TestSessionModel.PauseResumeSignal.Set();
			_eventAggregator.GetEvent<TimeEvent>().Publish(Now);
			var totalTestCount = TestSessionModel.Steps.Sum(step => step.Value.Tests.Count);

			TestSessionModel.Status = Started;
			TestSessionModel.StartTime = Now;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((TestSessionModel.Status, TestSessionModel.StartTime.Value));

			if(!await VerifySystemStateTestable(cancellationToken))
			{
				return;
			}

			foreach(var step in TestSessionModel.Steps)
			{
				CurrentStepModel = step.Value;

				if(await ActOnSessionStatus_()) return;
				CurrentStepModel.Status = InProgress;

				foreach(var test in CurrentStepModel.Tests)
				{
					if(await ActOnSessionStatus_()) return;
					TestSessionModel.PauseResumeSignal.WaitOne();
					if(cancellationToken.IsCancellationRequested || TestSessionModel.Status == Stopped)
					{
						TestSessionModel.Status = SessionStatus.Finished;
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((SessionStatus.Finished, Now));
						return;
					}
					CurrentTestModel = test.Value;

					try
					{
						await Task.Run(() => CurrentTestModel.Start(cancellationToken, TestSessionModel));
					}
					catch(Exception ex)
					{
						CurrentTestModel.Info.Entity.Description = $"{CurrentStepModel.Entity.Title} - {CurrentStepModel.Entity.Description} - {CurrentTestModel.Info.Entity.Id} - {TestStoppedByExceptionMessage}";
						CurrentTestModel.Info.Status = TestStatus.Aborted;
						FieldServiceTrace.LogException(ex);
					}

					_eventAggregator.GetEvent<TestEvent>().Publish(CurrentTestModel);

					if(CurrentTestModel.Info.Status == TestStatus.Aborted)
					{
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((SessionStatus.Finished, Now));
						return;
					}

					CurrentStepModel.ProcessedPercentage += OnePercentage / CurrentStepModel.Tests.Count * Percentage;
					if(CurrentTestModel.Info.Status == TestStatus.Passed || CurrentTestModel.Info.Status == TestStatus.Finished)
					{
						CurrentStepModel.PassedPercentage += OnePercentage / CurrentStepModel.Tests.Count * Percentage;
					}
					Progress += OnePercentage / totalTestCount * Percentage;
					if(CurrentTestModel.Info.Status == TestStatus.Failed)
					{
						CurrentStepModel.Status = FailedInProgress;
					}
					CurrentTestModel.Info.Entity.Description = string.Empty;
				}
				CurrentStepModel.Status = CurrentStepModel.Status != FailedInProgress
					? StepStatus.Finished
					: FailedFinished;
			}

			TestSessionModel.Status = SessionStatus.Finished;
			CurrentTestModel.Info.Entity.Description = CurrentTestSessionFinishedMessage;

			TestSessionModel.Passed = TestSessionModel
				.Steps
				.Values
				.All(stepModel_ => stepModel_.Tests.Values.All(testModel_ => testModel_.Info.Status == TestStatus.Passed));

			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((TestSessionModel.Status, Now));

			async Task<bool> ActOnSessionStatus_()
			{
				if(cancellationToken.IsCancellationRequested)
				{
					return true;
				}
				if(TestSessionModel.Status == Pausing)
				{
					TestSessionModel.Status = Paused;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((TestSessionModel.Status, Now));
				}
				if(TestSessionModel.Status == Resumed)
				{
					TestSessionModel.Status = Started;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((TestSessionModel.Status, Now));
				}
				if(TestSessionModel.Status == Stopping)
				{
					await Stop();
					return true;
				}
				return false;
			}
		}

		private async Task<bool> VerifySystemStateTestable(CancellationToken cancellationToken)
		{
			bool _isConsoleTestable = false;

			if(cancellationToken.IsCancellationRequested || TestSessionModel.Status == Stopping)
			{
				return await Task.FromResult(false);
			}

			using(_systemStateSubject.ObserveOn(TaskPoolScheduler.Default).Subscribe(state_ =>
						{
							if(state_ != CAN_ID_STATE_EXCEPTION)
							{
								_isConsoleTestable = _consoleSwitchState.Set();
							}
						}))
			{
				_consoleSwitchState.WaitOne();
			}

			return await Task.FromResult(_isConsoleTestable);
		}

		private async Task Start()
		{
			try
			{
				await StartTestSessionAsync(_cancellationTokenSource.Token);
			}
			catch(Exception e)
			{
				FieldServiceTrace.LogException(e);
			}
			finally
			{
				_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Ready, Now));
			}
		}

		private async Task Resume()
		{
			TestSessionModel.Status = Started;
			await Task.Run(TestSessionModel.PauseResumeSignal.Set);
		}

		private async Task Pause()
		{
			await Task.Run(TestSessionModel.PauseResumeSignal.Reset);
		}

		private async Task Finish()
		{
			await Task.Run(CreateTestSession);
		}

		private async Task Stop()
		{
			try
			{
				_cancellationTokenSource?.Cancel();
			}
			catch(ObjectDisposedException oex)
			{
				TestSessionModel.Status = Stopped;
				FieldServiceTrace.LogException(oex);
			}
			catch(AggregateException ex)
			{
				TestSessionModel.Status = SessionStatus.Unknown;
				FieldServiceTrace.LogException(ex);
			}
			finally
			{
				TestSessionModel.Status = Stopped;
				_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Stopped, Now));
				await Task.Run(CreateTestSession);
			}
		}

		private async Task SetReady()
		{
			TestSessionModel.Status = Ready;
			foreach(var step in TestSessionModel.Steps)
			{
				step.Value.Status = StepStatus.Unknown;
			}
			await Task.CompletedTask;
		}

		private void ResetTestSession()
		{
			foreach(var step in TestSessionModel.Steps)
			{
				step.Value.PassedPercentage = 0;
				step.Value.ProcessedPercentage = 0;
				step.Value.Status = StepStatus.Unknown;
				foreach(var test in step.Value.Tests)
				{
					test.Value.Info.Status = TestStatus.Unknown;
				}
			}
			Progress = 0;
			TestSessionModel.Status = Ready;
		}

		#endregion Methods

		#region Event Handlers

		private void _machineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_machineModel.SystemState):
					SystemState = _machineModel.SystemState;
					break;
			}
		}

		private async Task StopTestProcess()
		{
			if(CurrentTestModel != null && _machineModel.SystemState == CAN_ID_STATE_EXCEPTION)
			{
				CurrentTestModel.Info.Entity.Description = ConsoleExceptionMessage;
			}
			await Stop();
		}

		#endregion Event Handlers
	}
}
