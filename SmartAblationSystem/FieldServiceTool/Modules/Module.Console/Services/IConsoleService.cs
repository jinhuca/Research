using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Console;

namespace Module.Console.Services
{
  public interface IConsoleService : IDisposable
  {
    Balloon BalloonService { get; }
    ICanBusCommunication CanBusCommunicationService { get; }
    Catheter CatheterService { get; }
    CentralMicroControllerPID CentralPIDService { get; }
    IGeneralPurposeInputOutput IOService { get; }
    PatientMicroControllerPID PatientPIDService { get; }
    Tank TankService { get; }
  }
}
