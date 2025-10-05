using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class AudibleTestReport
	{
		public ITestInfo Information { get; set; }
		public IAudibleTestResult Result { get; set; }
	}
}
