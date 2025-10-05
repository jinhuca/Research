
using Console;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Module.Console.Models
{
	public class ConsoleMonitor : BindableBase
  {
    private readonly Machine _machine;

    private readonly DispatcherTimer _CanBusOneTimer = new DispatcherTimer();
    private readonly DispatcherTimer _CanBusTwoTimer = new DispatcherTimer();
    private DispatcherTimer _remoteControlTimer = new DispatcherTimer();
    private Thread _HeartBeatThread;

    public ConsoleMonitor(Machine machine)
    {
      _machine = machine;
    }

    #region CanBus Properties

    private Stopwatch _CanBusOneStopWatchCommunicationLost;
    public Stopwatch CanBusOneStopWatchCommunicationLost
    {
      get => _CanBusOneStopWatchCommunicationLost;
      set => SetProperty(ref _CanBusOneStopWatchCommunicationLost, value);
    }

    private Stopwatch _CanBusTwoStopWatchCommunicationLost;
    public Stopwatch CanBusTwoStopWatchCommunicationLost
    {
      get => _CanBusTwoStopWatchCommunicationLost;
      set => SetProperty(ref _CanBusTwoStopWatchCommunicationLost, value);
    }

    private Stopwatch _ResetCMCUErrorStopWatchDisconnection;
    public Stopwatch ResetCMCUErrorStopWatchDisconnection
    {
      get => _ResetCMCUErrorStopWatchDisconnection;
      set => SetProperty(ref _ResetCMCUErrorStopWatchDisconnection, value);
    }

    private Stopwatch _ResetPMCUErrorStopWatchDisconnection;
    public Stopwatch ResetPMCUErrorStopWatchDisconnection
    {
      get => _ResetPMCUErrorStopWatchDisconnection;
      set => SetProperty(ref _ResetPMCUErrorStopWatchDisconnection, value);
    }

    private bool _IsCanBusOneInError;
    public bool IsCanBusOneInError
    {
      get => _IsCanBusOneInError;
      set => SetProperty(ref _IsCanBusOneInError, value);
    }

    private bool _IsCanBusTwoInError;
    public bool IsCanBusTwoInError
    {
      get => _IsCanBusTwoInError;
      set => SetProperty(ref _IsCanBusTwoInError, value);
    }

    private bool _StopListeningCanOneCommunication;
    public bool StopListeningCanOneCommunication
    {
      get => _StopListeningCanOneCommunication;
      set => SetProperty(ref _StopListeningCanOneCommunication, value);
    }

    private bool _StopListeningCanTwoCommunication;
    public bool StopListeningCanTwoCommunication
    {
      get => _StopListeningCanTwoCommunication;
      set => SetProperty(ref _StopListeningCanTwoCommunication, value);
    }

    private bool _HeartBeatActivated = true;
    public bool HeartBeatActivated
    {
      get => _HeartBeatActivated;
      set => SetProperty(ref _HeartBeatActivated, value);
    }

    private bool _IsSystemRested;
    public bool IsSystemRested
    {
      get => _IsSystemRested;
      set => SetProperty(ref _IsSystemRested, value);
    }

    private bool _IsCMCUExceptionType5;
    public bool IsCMCUExceptionType5
    {
      get => _IsCMCUExceptionType5;
      set => SetProperty(ref _IsCMCUExceptionType5, value);
    }

    private bool _isVacuumDisconnected;
    public bool IsVacuumDisconnected
    {
      get => _isVacuumDisconnected;
      set => SetProperty(ref _isVacuumDisconnected, value, nameof(IsVacuumDisconnected));
    }

    #endregion CanBus Properties

    public void SetupCanBusCommunication()
    {
      StartCanOneStopWatchCommunicationMonitoring();
      StartCanTwoStopWatchCommunicationMonitoring();
      ResetConsoleSystem();
      StartHeartBeat();
    }

    private void StartCanOneStopWatchCommunicationMonitoring()
    {
      _CanBusOneTimer.Interval = TimeSpan.FromMilliseconds(2500);
      _CanBusOneTimer.Tick += _CanBusOneTimer_Tick;
      _CanBusOneTimer.Start();
      CanBusOneStopWatchCommunicationLost?.Start();
    }

    // To-Do
    private void _CanBusOneTimer_Tick(object sender, EventArgs e)
    {
      if (!IsCanBusOneInError && !StopListeningCanOneCommunication && HeartBeatActivated)
      {

      }
    }

    private void StartCanTwoStopWatchCommunicationMonitoring()
    {
      _CanBusTwoTimer.Interval = TimeSpan.FromMilliseconds(3000);
      _CanBusTwoTimer.Tick += _CanBusTwoTimer_Tick;
    }

    // To-Do
    private void _CanBusTwoTimer_Tick(object sender, EventArgs e)
    {

    }

    private void ResetConsoleSystem()
    {
      _machine.FailResetEnable();
      Thread.Sleep(10);
      _machine.FailResetDisable();
      Thread.Sleep(10);
      _machine.Disconnect();
      ResetCMCUErrorStopWatchDisconnection?.Start();
      ResetPMCUErrorStopWatchDisconnection?.Start();

      IsSystemRested = true;
      IsVacuumDisconnected = true;
      IsCMCUExceptionType5 = false;
    }

    private void StartHeartBeat()
    {
      _HeartBeatThread = new Thread(StartHeartBeatThread);
      _HeartBeatThread.Start();
    }

    private void StartHeartBeatThread()
    {
      while (true)
      {
        _machine.SendHeartbeat();
      }
      // ReSharper disable once FunctionNeverReturns (Heartbeat watchdog thread)
    }
  }
}
