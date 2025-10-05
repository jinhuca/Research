using Module.Infrastructure.TestResults;
using Module.Infrastructure.TestResults.Interfaces;

namespace Module.Infrastructure.TestInterfaces
{
	public interface ITestModel : ITestOperations
	{
		ITestInfo Info { get; set; }
	}
}
