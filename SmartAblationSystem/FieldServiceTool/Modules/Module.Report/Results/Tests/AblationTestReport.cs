using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class AblationTestReport
	{
		public ITestInfo Information { get; set; }
		public IAblationTestResult Result { get; set; }
	}
}
