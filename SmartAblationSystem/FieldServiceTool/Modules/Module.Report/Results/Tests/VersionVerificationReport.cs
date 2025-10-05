using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class VersionVerificationReport
	{
		public ITestInfo Information { get; set; }
		public IVersionTestResult Result { get; set; }
	}
}
