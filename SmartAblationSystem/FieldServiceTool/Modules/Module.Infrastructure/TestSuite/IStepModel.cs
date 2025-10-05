using System.Collections.Generic;

namespace Module.Infrastructure.TestSuite
{
	public interface IStepModel
	{
		string Id { get; set; }
		string Description { get; set; }
		double ProcessedPercentage { get; set; }
		double PassedPercentage { get; set; }
		IDictionary<string, ITestModel> Tests { get; set; }
	}
}
