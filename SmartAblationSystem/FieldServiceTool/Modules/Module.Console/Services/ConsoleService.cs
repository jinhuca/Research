using Console;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Unity;

namespace Module.Console.Services
{
  public class ConsoleService : IConsoleService
  {
    public Machine ConsoleMachine { get; }

    public ConsoleService(IUnityContainer container)
    {
      ConsoleMachine = container.Resolve<Machine>();
      BalloonService = ConsoleMachine.Balloon;
      CanBusCommunicationService = ConsoleMachine.CanBusCommunication;
      CatheterService = ConsoleMachine.Catheter;
      CentralPIDService = container.Resolve<CentralMicroControllerPID>();
      IOService = ConsoleMachine.GeneralPurposeInputOutput;
      PatientPIDService = container.Resolve<PatientMicroControllerPID>();
      TankService = ConsoleMachine.Tank;
    }

    private System.Threading.Timer _timer;

    #region IConsoleService Implementation

    public ICanBusCommunication CanBusCommunicationService { get; }
    public Balloon BalloonService { get; }
    public Catheter CatheterService { get; }
    public CentralMicroControllerPID CentralPIDService { get; }
    public IGeneralPurposeInputOutput IOService { get; }
    public PatientMicroControllerPID PatientPIDService { get; }
    public Tank TankService { get; }

    #endregion IConsoleService Implementation

    #region IDisposable

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
      {
        return;
      }

      if (_timer != null)
      {
        _timer.Dispose();
      }
      _timer = null;
    }

    ~ConsoleService() => Dispose(false);

    #endregion IDisposable
  }
}
