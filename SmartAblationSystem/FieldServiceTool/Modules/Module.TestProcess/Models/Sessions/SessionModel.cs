using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.Constants;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using static System.DateTime;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.SessionStatus;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Sessions
{
	public class SessionModel : BindableBase, ISessionModel
	{
		public SessionModel(
			IUnityContainer container,
			IEventAggregator eventAggregator,
			IMachineModel machineModel)
		{
			_eventAggregator = eventAggregator;
			_machineModel = machineModel;
			_container = container;

			SubscribePubSubEvents();
			CreateSession();
		}

		#region Public Properties

		private string _Id = string.Empty;
		public string Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}

		private SessionStatus _Status = Unknown;
		public SessionStatus Status
		{
			get => _Status;
			set => SetProperty(ref _Status, value);
		}

		public ManualResetEvent PauseResumeSignal { get; } = new ManualResetEvent(false);

		private Dictionary<StepId, IStepModel> _Steps = new Dictionary<StepId, IStepModel>();
		public Dictionary<StepId, IStepModel> Steps
		{
			get => _Steps;
			set => SetProperty(ref _Steps, value);
		}

		private DateTime? _StartTime;
		public DateTime? StartTime
		{
			get => _StartTime;
			set => SetProperty(ref _StartTime, value);
		}

		private DateTime? _FinishTime;
		public DateTime? FinishTime
		{
			get => _FinishTime;
			set => SetProperty(ref _FinishTime, value);
		}

		private bool? _Passed;
		public bool? Passed
		{
			get => _Passed;
			set => SetProperty(ref _Passed, value);
		}

		#endregion Public Properties

		#region Public Methods

		public SessionStatus Start()
		{
			Status = Starting;
      _holdingSystemException = false;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Status, Now));
			return Status;
		}

		public SessionStatus Pause()
		{
			Status = Pausing;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Status, Now));
			return Status;
		}

		public SessionStatus Resume()
		{
			Status = Resuming;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Status, Now));
			return Status;
		}

		public SessionStatus Stop()
		{
			Status = Stopped;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Status, Now));
			return Status;
		}

		#endregion Public Methods

		#region Private Fields

		private readonly IUnityContainer _container;
		private readonly IEventAggregator _eventAggregator;
		private readonly IMachineModel _machineModel;

    private bool _holdingSystemException = false;
    #endregion Private Fields

		#region Private Methods

		private void CatchConsoleException()
		{
			if(_holdingSystemException || Status == Ready || Status == Stopped)
			{
				return;
			}

			Status = SessionStatus.Exception;
			_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Status, Now));
		}

		private void SubscribePubSubEvents()
		{
			_eventAggregator.GetEvent<UserCommandEvent>().Subscribe(OnReceiveUserCommand);

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.SystemState))
        .Select( _ => _machineModel.SystemState)
        .Where( state_ => state_ == CAN_ID_STATE_EXCEPTION)
				.Subscribe(_ => CatchConsoleException());
		}

		private async void OnReceiveUserCommand((UserCommand, DateTime) userCommandMessage)
		{
			var (command, dateTime) = userCommandMessage;
			switch(command)
			{
				case StartTest:
					await Task.Run(Start);
					break;
				case PauseTest:
					if(Status == Ready || Status == Pausing || Status == Paused || Status == Stopped) return;
					await Task.Run(Pause);
					break;
				case ResumeTest:
					await Task.Run(Resume);
					break;
				case StopTest:
					if(Status == Stopped) return;
					await Task.Run(Stop);
					break;
				case GenerateReport:
					break;
				case GoSmartFreeze:
					break;
				case TurnOff:
					break;
				case EnableCmcuExceptionType5:
				case IgnoreCmcuExceptionType5:
          break;
				case HoldException:
          _holdingSystemException = true;
					break;
				case ResetHoldException:
          _holdingSystemException = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void CreateSession()
		{
			Id = Guid.NewGuid().ToString();

      var step1_ = _container.Resolve<IStepModel>();
			step1_.Entity = StepEntity.Step1;

			step1_.Tests = new Dictionary<TestId, ITestModel>()
			{
				{ TestId.VersionVerification, _container.Resolve<ITestModel>(TestEntity.VersionVerificationEntity.Id.ToString()) },
				{ TestId.InputTest, _container.Resolve<ITestModel>(TestEntity.InputTestEntity.Id.ToString()) },
				{ TestId.VisualTest, _container.Resolve<ITestModel>(TestEntity.VisualTestEntity.Id.ToString()) },
				{ TestId.AudibleTest, _container.Resolve<ITestModel>(TestEntity.AudibleTestEntity.Id.ToString()) }
			};

			var step2_ = _container.Resolve<IStepModel>();
			step2_.Entity = StepEntity.Step2;
			step2_.Tests = new Dictionary<TestId, ITestModel>
			{
				{ TestId.IdleStateCheck, _container.Resolve<ITestModel>(TestEntity.IdleStateCheckEntity.Id.ToString()) },
				{ TestId.ReadyStateCheck, _container.Resolve<ITestModel>(TestEntity.ReadyStateCheckEntity.Id.ToString()) }
			};

			var step3_ = _container.Resolve<IStepModel>();
			step3_.Entity = StepEntity.Step3;
			step3_.Tests = new Dictionary<TestId, ITestModel>
			{
				{ TestId.AblationTests, _container.Resolve<ITestModel>(TestEntity.AblationTestsEntity.Id.ToString()) },
				//{ TestId.DMSTests, _container.Resolve<ITestModel>(TestEntity.DMSTestEntity.Id.ToString()) },
				//{ TestId.ETSTests, _container.Resolve<ITestModel>(TestEntity.ETSTestEntity.Id.ToString()) },
				//{ TestId.OPSTests, _container.Resolve<ITestModel>(TestEntity.OPSTestEntity.Id.ToString()) }
			};

			Steps = new Dictionary<StepId, IStepModel>
			{
				{ StepId.Step1, step1_ }, { StepId.Step2, step2_ }, { StepId.Step3, step3_ }
			};

			Status = Ready;
		}

		#endregion Private Methods
	}
}
