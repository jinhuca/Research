using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results
{
	public class ConsoleErrorReport
	{
		public IConsoleErrorResult Result { get; set; }

		public ConsoleErrorReport()
		{
			Result = new ConsoleErrorResult();
		}
	}
}
