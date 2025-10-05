using Module.Report.Interfaces;
using Module.Report.Results.Tests;
using System;

namespace Module.Report.Results
{
	public class TestReport : ITestReport
	{
		public bool? Passed { get; set; }
		public DateTime? StartDateTime { get; set; }
		public DateTime? FinishDateTime { get; set; }
		public string ConsoleSerialNumber { get; set; }
		public string HospitalName { get; set; }
		public string FstVersion { get; set; }
		public Tester TesterReport { get; set; }
		public SessionSummary SessionSummaryReport { get; set; }
		public VersionVerificationReport VersionReport { get; set; }
		public InputTestReport InputReport { get; set; }
		public VisualTestReport VisualReport { get; set; }
		public AudibleTestReport AudibleReport { get; set; }
		public IdleStateCheckReport IdleStateReport { get; set; }
		public ReadyStateCheckReport ReadyStateReport { get; set; }
		public AblationTestReport AblationReport { get; set; }
		//public DmsTestReport DmsReport { get; set; }
		//public EtsTestReport EtsReport { get; set; }
		//public OpsTestReport OpsReport { get; set; }
		public RetryRationaleReport RationaleReport { get; set; }
		public ConsoleErrorReport ErrorReport { get; set; }

		public TestReport(
			Tester tester, 
			SessionSummary sessionSummary,
			VersionVerificationReport versionReport,
			InputTestReport inputReport,
			VisualTestReport visualReport,
			AudibleTestReport audibleReport,
			IdleStateCheckReport idleReport,
			ReadyStateCheckReport readyReport,
			AblationTestReport ablationReport,
			RetryRationaleReport rationaleReport,
			ConsoleErrorReport errorReport)
		{
			TesterReport = tester;
			SessionSummaryReport = sessionSummary;
			VersionReport = versionReport;
			InputReport = inputReport;
			VisualReport = visualReport;
			AudibleReport = audibleReport;
			IdleStateReport = idleReport;
			ReadyStateReport = readyReport;
			AblationReport = ablationReport;
			//DmsReport = dmsReport;
			//EtsReport = etsReport;
			//OpsReport = opsReport;
			RationaleReport = rationaleReport;
			ErrorReport = errorReport;
		}
	}
}
