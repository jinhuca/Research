using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class ReadyStateCheckReport
	{
		public ITestInfo Information { get; set; }
		public IReadyStateCheckResult Result { get; set; }
	}
}
