using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class DmsTestReport
	{
		public ITestInfo Information { get; set; }
		public IDmsTestResult Result { get; set; }
	}
}
