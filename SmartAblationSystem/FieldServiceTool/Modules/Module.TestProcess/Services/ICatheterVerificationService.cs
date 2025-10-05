using System.Threading;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;

namespace Module.TestProcess.Services
{
  public interface ICatheterVerificationService
  {
    bool IsCatheterReady { get; }
    bool VerifyCatheterIsReadyAndValid(CancellationToken cancellationToken, ISessionModel sessionModel, ITestInfo info);
  }
}