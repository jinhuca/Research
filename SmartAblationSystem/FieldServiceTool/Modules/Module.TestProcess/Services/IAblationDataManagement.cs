using Module.Infrastructure.TestResults.Interfaces;
using Module.TestProcess.Helpers;
using System.Threading;

namespace Module.TestProcess.Services
{
	public interface IAblationDataManagement
  {
    IAblationTestResult GetTestResult();
    void InitializeDataManagement(IAblationTestResult testResult);
    void ProcessInflationData(int id, IAblationConfiguration ablationConfiguration, double obpInReady);
    void ProcessAblationData(int id, IAblationConfiguration ablationConfiguration, IbpAblationValidator ibpAblationValidator, Pwm2AblationValidator pwm2AblationValidator, double obpInReady);
    void ProcessThawingData(int id);
    void RecordInflationData(int id, IAblationConfiguration ablationConfiguration, CancellationToken cancellationToken, int inflationTime);
    void RecordAblationData(int id, CancellationToken cancellationToken, int ablationTime);
    void RecordThawingData(int id, CancellationToken cancellationToken, int thawingTime);
    void SampleAblationDetails(int id, IAblationConfiguration ablationConfiguration, CancellationToken cancellationToken);
    void SetAblationSummary(int id);
    void RecordFlowMeterCheckResult(int id, (double Value, bool? Passed, double?) result);
  }
}
