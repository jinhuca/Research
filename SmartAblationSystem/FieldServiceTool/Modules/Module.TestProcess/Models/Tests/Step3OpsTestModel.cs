using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Module.Console.Interfaces;
using Unity;
using static System.DateTime;
using static Communication.CanBusMessageDefinition;

namespace Module.TestProcess.Models.Tests
{
	public class Step3OpsTestModel : BindableBase, ITestModel
	{
		public Step3OpsTestModel(
			IDialogService dialogService,
			IUnityContainer unityContainer,
			ITestInfo testInfo,
			IMachineModel machineModel)
		{
			_dialogService = dialogService;
			_unityContainer = unityContainer;
			_info = testInfo;
			_info.Entity = TestEntity.OPSTestEntity;
			_machineModel = machineModel;
			_machineModel.PropertyChanged += MachineModel_PropertyChanged;
		}

		private readonly IUnityContainer _unityContainer;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private CancellationToken _token;

		private MessageStateId _SystemState;
		public MessageStateId SystemState
		{
			get => _SystemState;
			set => SetProperty(ref _SystemState, value);
		}

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IOpsTestResult _result;
		public IOpsTestResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		private void MachineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(_machineModel.SystemState):
					SystemState = _machineModel.SystemState;
					break;
			}
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				Info.Status = TestStatus.Aborted;
				return Info;
			}
			Info.StartTime = Now;
			Info.Status = TestStatus.Inprogress;
			await Task.Delay(1000, cancellationToken);
			Info.Status = TestStatus.Passed;
			Info.FinishTime = Now;
			return await Task.FromResult(Info);
		}
	}
}
