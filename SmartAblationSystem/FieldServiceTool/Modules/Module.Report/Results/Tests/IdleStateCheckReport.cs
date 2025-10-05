using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class IdleStateCheckReport
	{
		public ITestInfo Information { get; set; }
		public IIdleStateCheckResult Result { get; set; }
	}
}
