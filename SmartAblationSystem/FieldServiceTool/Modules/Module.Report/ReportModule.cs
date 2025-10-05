using Module.Report.Interfaces;
using Module.Report.Results;
using Module.Report.Results.Tests;
using Prism.Ioc;
using Prism.Modularity;
using System.ComponentModel;

namespace Module.Report
{
	public class ReportModule : IModule
	{
		private readonly IContainerProvider _containerProvider;
    private ReportAggregation _reportAggregator;
		public ReportModule(IContainerProvider containerProvider)
		{
			_containerProvider = containerProvider;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<ReportAggregation>();
			containerRegistry.Register<Tester>();
			containerRegistry.Register<ITestReport, TestReport>();

			containerRegistry.Register<VersionVerificationReport>();
			containerRegistry.Register<InputTestReport>();
			containerRegistry.Register<VisualTestReport>();
			containerRegistry.Register<AudibleTestReport>();

			containerRegistry.Register<IdleStateCheckReport>();
			containerRegistry.Register<ReadyStateCheckReport>();

			containerRegistry.Register<AblationTestReport>();
			containerRegistry.Register<DmsTestReport>();
			containerRegistry.Register<EtsTestReport>();
			containerRegistry.Register<OpsTestReport>();

			containerRegistry.Register<RetryRationaleReport>();
			containerRegistry.Register<ConsoleErrorReport>();
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
      _reportAggregator = containerProvider.Resolve<ReportAggregation>();
      _reportAggregator.Start();
		}
	}
}
