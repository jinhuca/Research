using Module.Infrastructure.Controls;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;
using Prism.Ioc;
using Prism.Modularity;

namespace Module.Infrastructure
{
	public class InfrastructureModule : IModule
  {
    private static IContainerProvider _containerProvider;

    public InfrastructureModule(IContainerProvider containerProvider)
    {
      _containerProvider = containerProvider;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
	    containerRegistry.Register<TestEntity>();
      containerRegistry.Register<ITestInfo, TestInfo>();
      containerRegistry.RegisterSingleton<IVersionTestResult, VersionTestResult>();
      containerRegistry.RegisterSingleton<SessionStatus>();
      containerRegistry.RegisterSingleton<IInputTestResult, InputTestResult>();
      containerRegistry.RegisterSingleton<IVisualTestResult, VisualTestResult>();
      containerRegistry.RegisterSingleton<IAudibleTestResult, AudibleTestResult>();
      containerRegistry.RegisterSingleton<IIdleStateCheckResult, IdleStateCheckResult>();
      containerRegistry.RegisterSingleton<IReadyStateCheckResult, ReadyStateCheckResult>();
      containerRegistry.Register<IAblationTestResult, AblationTestResult>();
      containerRegistry.Register<IDmsTestResult, DmsTestResult>();
      containerRegistry.Register<IEtsTestResult, EtsTestResult>();
      containerRegistry.Register<IOpsTestResult, OpsTestResult>();
      containerRegistry.Register<IGeneralInformation, GeneralInformation>();
      containerRegistry.Register<ITreatment, Treatment>();
      containerRegistry.RegisterDialog<ErrorMessageDialog, ErrorMessageDialogViewModel>();
      containerRegistry.RegisterDialog<CatheterMechanicalPopupView, CatheterMechanicalPopupViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
  }
}
