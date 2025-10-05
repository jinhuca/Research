using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class OpsTestReport
	{
		public ITestInfo Information { get; set; }
		public IOpsTestResult Result { get; set; }
	}
}
