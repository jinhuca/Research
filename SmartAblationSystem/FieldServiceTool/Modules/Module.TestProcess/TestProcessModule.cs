using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.TestProcess.Models.Sessions;
using Module.TestProcess.Models.Steps;
using Module.TestProcess.Models.Tests;
using Module.TestProcess.Services;
using Module.TestProcess.ViewModels;
using Module.TestProcess.ViewModels.Dialogs;
using Module.TestProcess.ViewModels.Sessions;
using Module.TestProcess.ViewModels.Steps;
using Module.TestProcess.ViewModels.Tests;
using Module.TestProcess.Views;
using Module.TestProcess.Views.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Module.TestProcess
{
	public class TestProcessModule : IModule
	{
		private readonly IContainerProvider _containerProvider;
		private readonly IRegionManager _regionManager;

		public TestProcessModule(IContainerProvider containerProvider, IRegionManager regionManager)
		{
			_containerProvider = containerProvider;
			_regionManager = regionManager;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<ISessionModel, SessionModel>();
			containerRegistry.Register<ISessionViewModel, SessionViewModel>();

			containerRegistry.Register<IStepModel, StepModel>();
			containerRegistry.RegisterSingleton<IStepModel, NullStepModel>(StepEntity.NullStepEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, NullTestModel>(TestEntity.NullTestEntity.Id.ToString());

      containerRegistry.RegisterSingleton<ICatheterVerificationService, CatheterVerificationService>();
      containerRegistry.RegisterSingleton<IAblationDataManagement, AblationDataManagement>();
			containerRegistry.RegisterSingleton<IAblationConfiguration, AblationConfiguration>();
			containerRegistry.RegisterSingleton<IAblationService, AblationService>();

			containerRegistry.RegisterSingleton<ITestModel, Step1VersionVerificationModel>(TestEntity.VersionVerificationEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step1InputTestModel>(TestEntity.InputTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step1VisualTestModel>(TestEntity.VisualTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step1AudibleTestModel>(TestEntity.AudibleTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step2IdleCheckModel>(TestEntity.IdleStateCheckEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step2ReadyCheckModel>(TestEntity.ReadyStateCheckEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step3AblationTestModel>(TestEntity.AblationTestsEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step3DmsTestModel>(TestEntity.DMSTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step3EtsTestModel>(TestEntity.ETSTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<ITestModel, Step3OpsTestModel>(TestEntity.OPSTestEntity.Id.ToString());
			containerRegistry.RegisterSingleton<IXlsxService, XlsxService>();

			containerRegistry.Register<IStepViewModel, StepViewModel>();
			containerRegistry.RegisterSingleton<IStepViewModel, NullStepViewModel>(StepEntity.NullStepEntity.Id.ToString());
			containerRegistry.Register<ITestViewModel, TestViewModel>();
			containerRegistry.RegisterSingleton<ITestViewModel, NullTestViewModel>(TestEntity.NullTestEntity.Id.ToString());

      containerRegistry.RegisterDialog<Dialog, DialogViewModel>();
			containerRegistry.RegisterDialog<StopTestConfirmationDialog, StopTestConfirmationDialogViewModel>();
      containerRegistry.RegisterDialog<Step1VersionVerificationDialog, Step1VersionVerificationDialogViewModel>();
			containerRegistry.RegisterDialog<Step1AudibleTestDialog, Step1AudibleTestDialogViewModel>();
			containerRegistry.RegisterDialog<Step1VisualTestDialog, Step1VisualTestDialogViewModel>();
			containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();
			containerRegistry.RegisterDialog<StateCheckFailureDialog, ConfirmationDialogViewModel>();
			containerRegistry.RegisterDialog<RetryTestDialog, RetryTestDialogViewModel>();
			containerRegistry.RegisterDialog<RetryStopDialog, RetryStopDialogViewModel>();
			containerRegistry.RegisterDialog<MessageDialog, MessageDialogViewModel>();
			containerRegistry.RegisterDialog<ContinueDialog, MessageDialogViewModel>();
			containerRegistry.RegisterDialog<TestStartMsgDialog, MessageDialogViewModel>();
			containerRegistry.RegisterDialog<RationaleDialog, RationaleDialogViewModel>();
			containerRegistry.RegisterDialog<ConsoleExceptionDialog, ConsoleExceptionDialogViewModel>();
			containerRegistry.RegisterDialog<PerformanceTestFailureDialog, PerformanceTestFailureDialogViewModel>();
			containerRegistry.RegisterDialog<SmoothnessVerificationDialog, SmoothnessVerificationDialogViewModel>();
			containerRegistry.RegisterDialog<ChangeTankDialog, ChangeTankDialogViewModel>();
			containerRegistry.RegisterDialog<Step3AblationCheckDialog, Step3AblationCheckDialogViewModel>();
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
			_regionManager.RegisterViewWithRegion(KnownRegionNames.TestProcessRegionName, typeof(TestProcessView));
		}
	}
}
