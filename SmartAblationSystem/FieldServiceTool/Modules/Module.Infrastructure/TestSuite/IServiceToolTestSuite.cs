using System.Collections.Generic;

namespace Module.Infrastructure.TestSuite
{
	public interface IServiceToolTestSuite
	{
		IDictionary<string, IStepModel> Steps { get; set; }
	}
}
