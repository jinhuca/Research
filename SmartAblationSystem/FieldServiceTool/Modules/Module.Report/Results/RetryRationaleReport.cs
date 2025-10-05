using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Report.Results
{
	public class RetryRationaleReport
	{
		public IRetryRationaleResult Result { get; set; }

		public RetryRationaleReport()
		{
			Result = new RetryRationaleResult();
		}
	}
}
