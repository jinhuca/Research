using Module.Report.Results;
using Module.Report.Results.Tests;
using System;

namespace Module.Report.Interfaces
{
	public interface ITestReport
	{
		bool? Passed { get; set; }
		DateTime? StartDateTime { get; set; }
		DateTime? FinishDateTime { get; set; }
		string ConsoleSerialNumber { get; set; }
		string HospitalName { get; set; }
		string FstVersion { get; set; }
		Tester TesterReport { get; set; }
		SessionSummary SessionSummaryReport { get; set; }
		VersionVerificationReport VersionReport { get; set; }
		InputTestReport InputReport { get; set; }
		VisualTestReport VisualReport { get; set; }
		AudibleTestReport AudibleReport { get; set; }
		IdleStateCheckReport IdleStateReport { get; set; }
		ReadyStateCheckReport ReadyStateReport { get; set; }
		AblationTestReport AblationReport { get; set; }
		//DmsTestReport DmsReport { get; set; }
		//EtsTestReport EtsReport { get; set; }
		//OpsTestReport OpsReport { get; set; }
		RetryRationaleReport RationaleReport { get; set; }
		ConsoleErrorReport ErrorReport { get; set; }
	}
}
