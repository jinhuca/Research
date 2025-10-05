using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.TestProcess.Models;
using Module.TestProcess.Models.Tests;
using Module.TestProcess.Properties;
using Module.TestProcess.ViewModels.Sessions;
using Prism.Events;
using Prism.Mvvm;
using System.ComponentModel;
using System.Linq;
using Unity;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.SessionStatus;

namespace Module.TestProcess.ViewModels
{
	/// <summary>
	/// Class definition for <see cref="TestProcessViewModel"/>
	/// </summary>
	public class TestProcessViewModel : BindableBase
	{
		public TestProcessViewModel(
			IUnityContainer container,
			IEventAggregator eventAggregator,
			TestProcessModel testProcessModel,
			IMachineModel machineModel)
		{
			_container = container;
			_machineModel = machineModel;
			_machineModel.PropertyChanged += _machineModel_PropertyChanged;
			_model = testProcessModel;
			_model.PropertyChanged += TestProcessModel_PropertyChanged;
			_model.TestSessionModel.PropertyChanged += TestSessionModel_PropertyChanged;

			SessionViewModel = container.Resolve<ISessionViewModel>();
			InitializeTestSessionViewModel();
		}

		private void _machineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_machineModel.SystemState):
					RaisePropertyChanged(nameof(SystemState));
					break;
			}
		}

		#region Fields

		private readonly IUnityContainer _container;
		private readonly IMachineModel _machineModel;
		private readonly TestProcessModel _model;

		#endregion Fields

		#region Properties

		#region ReadOnly Properties

		public string SystemStateTitle { get; } = Resources.SystemStateTitle;
		public string StatusTitle { get; } = Resources.StatusTitle;
		public string TestTitle { get; } = Resources.TestsText;
		public string ProgressText { get; } = Resources.ProgressText;
		public string TestDetailsTitle { get; } = Resources.TestDetailsTitleText;

		#endregion ReadOnly Properties

		#region State Properties

		private IStepViewModel _CurrentStepViewModel;
		public IStepViewModel CurrentStepViewModel
		{
			get => _CurrentStepViewModel;
			set => SetProperty(ref _CurrentStepViewModel, value);
		}

		private ITestViewModel _CurrentTestViewModel;
		public ITestViewModel CurrentTestViewModel
		{
			get => _CurrentTestViewModel;
			set => SetProperty(ref _CurrentTestViewModel, value);
		}

		private double _TotalProcessedPercentage;
		public double TotalProcessedPercentage
		{
			get => _TotalProcessedPercentage;
			set => SetProperty(ref _TotalProcessedPercentage, value);
		}

		private SessionStatus _CurrentSessionStatus = Unknown;
		public SessionStatus CurrentSessionStatus
		{
			get => _CurrentSessionStatus;
			set => SetProperty(ref _CurrentSessionStatus, value);
		}

		private MessageStateId _systemState;
		public MessageStateId SystemState
		{
			get => _systemState;
			set => SetProperty(ref _systemState, value);
		}

		#endregion State Properties

		#region Step ViewModels

		private IStepViewModel _step1ViewModel;
		public IStepViewModel Step1ViewModel
		{
			get => _step1ViewModel;
			set => SetProperty(ref _step1ViewModel, value);
		}

		private IStepViewModel _step2ViewModel;
		public IStepViewModel Step2ViewModel
		{
			get => _step2ViewModel;
			set => SetProperty(ref _step2ViewModel, value);
		}

		private IStepViewModel _step3ViewModel;
		public IStepViewModel Step3ViewModel
		{
			get => _step3ViewModel;
			set => SetProperty(ref _step3ViewModel, value);
		}

		#endregion Step ViewModels

		#region Test ViewModels

		private ITestViewModel _VersionVerificationViewModel;
		public ITestViewModel VersionVerificationViewModel
		{
			get => _VersionVerificationViewModel;
			set => SetProperty(ref _VersionVerificationViewModel, value);
		}

		private ITestViewModel _inputTestTestViewModel;
		public ITestViewModel InputTestViewModel
		{
			get => _inputTestTestViewModel;
			set => SetProperty(ref _inputTestTestViewModel, value);
		}

		private ITestViewModel _VisualTestViewModel;
		public ITestViewModel VisualTestViewModel
		{
			get => _VisualTestViewModel;
			set => SetProperty(ref _VisualTestViewModel, value);
		}

		private ITestViewModel _AudibleTestViewModel;
		public ITestViewModel AudibleTestViewModel
		{
			get => _AudibleTestViewModel;
			set => SetProperty(ref _AudibleTestViewModel, value);
		}

		private ITestViewModel _IdleStateTestViewModel;
		public ITestViewModel IdleStateTestViewModel
		{
			get => _IdleStateTestViewModel;
			set => SetProperty(ref _IdleStateTestViewModel, value);
		}

		private ITestViewModel _ReadyStateTestViewModel;
		public ITestViewModel ReadyStateTestViewModel
		{
			get => _ReadyStateTestViewModel;
			set => SetProperty(ref _ReadyStateTestViewModel, value);
		}

		private ITestViewModel _AblationTestViewModel;
		public ITestViewModel AblationTestViewModel
		{
			get => _AblationTestViewModel;
			set => SetProperty(ref _AblationTestViewModel, value);
		}

		private ITestViewModel _DMSTestViewModel;
		public ITestViewModel DMSTestViewModel
		{
			get => _DMSTestViewModel;
			set => SetProperty(ref _DMSTestViewModel, value);
		}

		private ITestViewModel _ETSTestViewModel;
		public ITestViewModel ETSTestViewModel
		{
			get => _ETSTestViewModel;
			set => SetProperty(ref _ETSTestViewModel, value);
		}

		private ITestViewModel _OPSTestViewModel;
		public ITestViewModel OPSTestViewModel
		{
			get => _OPSTestViewModel;
			set => SetProperty(ref _OPSTestViewModel, value);
		}

		#endregion Test ViewModels

		#region TestSession

		private ISessionViewModel _SessionViewModel;
		public ISessionViewModel SessionViewModel
		{
			get => _SessionViewModel;
			set => SetProperty(ref _SessionViewModel, value);
		}

		#endregion TestSession

		#endregion Properties

		#region Methods

		private void InitializeTestSessionViewModel()
		{
			Step1ViewModel = SessionViewModel?.StepViewModelCollection?.FirstOrDefault(stepViewModel => stepViewModel.Entity.Id == StepEntity.Step1.Id);
			VersionVerificationViewModel = Step1ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.VersionVerificationEntity.Id);
			InputTestViewModel = Step1ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.InputTestEntity.Id);
			VisualTestViewModel = Step1ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.VisualTestEntity.Id);
			AudibleTestViewModel = Step1ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.AudibleTestEntity.Id);

			Step2ViewModel = SessionViewModel?.StepViewModelCollection?.FirstOrDefault(stepViewModel => stepViewModel.Entity.Id == StepEntity.Step2.Id);
			IdleStateTestViewModel = Step2ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.IdleStateCheckEntity.Id);
			ReadyStateTestViewModel = Step2ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.ReadyStateCheckEntity.Id);

			Step3ViewModel = SessionViewModel?.StepViewModelCollection?.FirstOrDefault(stepViewModel => stepViewModel.Entity.Id == StepEntity.Step3.Id);
			AblationTestViewModel = Step3ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.AblationTestsEntity.Id);
			DMSTestViewModel = Step3ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.DMSTestEntity.Id);
			ETSTestViewModel = Step3ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.ETSTestEntity.Id);
			OPSTestViewModel = Step3ViewModel?.TestViewModels?.FirstOrDefault(testViewModel => testViewModel.Entity.Id == TestEntity.OPSTestEntity.Id);

			_model.TestSessionModel.PropertyChanged += TestSessionModel_PropertyChanged;

			foreach(var stepModel in _model.TestSessionModel.Steps)
			{
				stepModel.Value.PropertyChanged += StepModel_PropertyChanged;
				foreach(var testModel in stepModel.Value.Tests)
				{
					testModel.Value.Info.PropertyChanged += TestResult_PropertyChanged;
				}
			}

			CurrentTestViewModel = VersionVerificationViewModel;
			CurrentStepViewModel = Step1ViewModel;
			SystemState = CAN_ID_STATE_IDLE;
		}

		#endregion Methods

		#region Event Handlers

		private void TestProcessModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(TestProcessModel.TestSessionModel):
					_model.TestSessionModel.PropertyChanged -= TestSessionModel_PropertyChanged;
					SessionViewModel = _container.Resolve<ISessionViewModel>();
					InitializeTestSessionViewModel();
					break;
				case nameof(TestProcessModel.CurrentStepModel):
					CurrentStepViewModel = SessionViewModel.StepViewModelCollection.FirstOrDefault(stepViewModel => stepViewModel.Entity.Id == _model.CurrentStepModel.Entity.Id);
					if(_model.CurrentStepModel != null && CurrentStepViewModel != null)
					{
						CurrentStepViewModel.Status = _model.CurrentStepModel.Status;
					}
					break;
				case nameof(TestProcessModel.CurrentTestModel):
					var step = SessionViewModel.StepViewModelCollection.FirstOrDefault(stepViewModel => stepViewModel.Entity.Id == _model.CurrentTestModel.Info.Entity.StepId);
					if(step != null)
					{
						CurrentTestViewModel = step.TestViewModels.FirstOrDefault(test => test.Entity.Id == _model.CurrentTestModel.Info.Entity.Id);
						CurrentTestViewModel.Status = _model.CurrentTestModel.Info.Status;
						CurrentTestViewModel.Entity = _model.CurrentTestModel.Info.Entity;
					}
					if(_model.CurrentTestModel is NullTestModel)
					{
						CurrentTestViewModel.Entity = TestEntity.NullTestEntity;
					}
					break;
				case nameof(TestProcessModel.Progress):
					TotalProcessedPercentage = _model.Progress;
					break;
				case nameof(TestProcessModel.SystemState):
					SystemState = _model.SystemState;
					break;
				case nameof(SessionStatus):
					CurrentSessionStatus = _model.TestSessionModel.Status;
					break;
			}
		}

		private void TestSessionModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(TestProcessModel.TestSessionModel.Status):
					CurrentSessionStatus = _model.TestSessionModel.Status;
					break;
			}
		}

		private void StepModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var stepModel = sender as IStepModel;
			var stepViewModel = SessionViewModel.StepViewModelCollection?.First(step => step.Entity.Id == stepModel.Entity.Id);
			switch(e.PropertyName)
			{
				case nameof(IStepModel.ProcessedPercentage):
					stepViewModel.ProcessedPercentage = stepModel.ProcessedPercentage;
					break;
				case nameof(IStepModel.PassedPercentage):
					stepViewModel.PassedPercentage = stepModel.PassedPercentage;
					break;
				case nameof(IStepModel.Status):
					stepViewModel.Status = stepModel.Status;
					break;
			}
		}

		private void TestResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var result = sender as ITestInfo;
			var stepViewModel = SessionViewModel.StepViewModelCollection.First(step => step.Entity.Id == result.Entity.StepId);
			var testViewModel = stepViewModel.TestViewModels.First(test => test.Entity.Id == result.Entity.Id);
			switch(e.PropertyName)
			{
				case nameof(ITestInfo.Status):
					testViewModel.Status = result.Status;
					break;
			}
		}

		#endregion Event Handlers
	}
}