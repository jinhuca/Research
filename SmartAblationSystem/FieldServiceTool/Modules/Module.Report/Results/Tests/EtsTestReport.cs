using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class EtsTestReport
	{
		public ITestInfo Information { get; set; }
		public IEtsTestResult Result { get; set; }
	}
}
