using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results.Tests
{
	public class InputTestReport
	{
		public ITestInfo Information { get; set; }
		public IInputTestResult Result { get; set; }
	}
}
