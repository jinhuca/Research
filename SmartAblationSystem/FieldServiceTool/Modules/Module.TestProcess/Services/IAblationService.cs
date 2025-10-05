using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Module.TestProcess.Services
{
	public interface IAblationService
	{
		Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel);
		IAblationDataManagement DataManagement { get; set; }
		IAblationConfiguration Configuration { get; set; }
		IAblationTestResult Result { get; set; }
		int AblationCount { get; set; }
	}
}