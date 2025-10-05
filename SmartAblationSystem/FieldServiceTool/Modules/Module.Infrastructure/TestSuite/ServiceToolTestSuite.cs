using System.Collections.Generic;

namespace Module.Infrastructure.TestSuite
{
	public class ServiceToolTestSuite : IServiceToolTestSuite
	{
		public IDictionary<string, IStepModel> Steps { get; set; }
		public ServiceToolTestSuite(Dictionary<string, IStepModel> testSteps)
		{
			Steps = testSteps;
		}
	}
}
