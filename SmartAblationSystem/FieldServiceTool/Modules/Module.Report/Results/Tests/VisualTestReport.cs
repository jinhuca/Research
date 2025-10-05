using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class VisualTestReport
	{
		public ITestInfo Information { get; set; }
		public IVisualTestResult Result { get; set; }
	}
}
