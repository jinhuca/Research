using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Module.CatheterTestTool.Models;
using Module.Console.Interfaces;
using Module.Infrastructure.AppLog;

using static Communication.CanBusMessageDefinition;

namespace Module.CatheterTestTool.Services
{
  public abstract class CatheterTestServiceBase
  {
    private readonly ISubject<MessageStateId> _systemStateSubject = new BehaviorSubject<MessageStateId>(MessageStateId.CAN_ID_STATE_UNKNOWN);
    private readonly ManualResetEvent _cancelTestSessionEvent = new ManualResetEvent(false);
    private readonly AutoResetEvent _consoleSwitchState = new AutoResetEvent(false);
    private readonly IMachineModel _machineModel;
    
    protected CatheterTestServiceBase(IMachineModel machineModel)
    {
      _machineModel = machineModel;

      Observable.FromEventPattern<PropertyChangedEventArgs>(machineModel, "PropertyChanged")
        .Where(arg => arg.EventArgs.PropertyName == "SystemState")
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(_ => SystemStateSubject?.OnNext(MachineModel.SystemState));

      SystemStateSubject?.OnNext(MachineModel.SystemState);

    }

    protected IMachineModel MachineModel => _machineModel;
    protected ISubject<MessageStateId> SystemStateSubject => _systemStateSubject;
    protected ManualResetEvent CancelTestSessionEvent => _cancelTestSessionEvent;
    protected AutoResetEvent ConsoleSwitchState => _consoleSwitchState;

    protected bool IsCancelEventSet => _cancelTestSessionEvent.WaitOne(0);

    protected virtual bool SwitchToReadyState()
    {
      bool succeeded;
      FieldServiceTrace.Log("SwitchToReadyState");

      //Subscribe on State change to Ready
      using (var switchToReady = _systemStateSubject?
               .ObserveOn(TaskPoolScheduler.Default)
               .Subscribe(state =>
               {
                 if (state == MessageStateId.CAN_ID_STATE_READY || IsCancelEventSet)
                   _consoleSwitchState.Set();
               }))
      {
        // Wait for State change to Ready for 10 seconds if it is currently not in Ready state
        if (_machineModel.SystemState != MessageStateId.CAN_ID_STATE_READY)
        {
          //Turn on Vacuum if it is not on
          // _machineModel.Console.VacuumEnable();
          SetVacuum(true);
          succeeded = _consoleSwitchState.WaitOne(CatheterTestConstants.IDLE_TO_READY_STATE_TIMEOUT_IN_SEC);
        }
        else
          succeeded = true; 
      }

      // Switching Console to READY state successfully
      return succeeded && !IsCancelEventSet;
    }

    protected virtual bool SwitchToInflation()
    {
      bool succeeded;
      using (var switchToInflation = _systemStateSubject?
               .ObserveOn(TaskPoolScheduler.Default)
               .Subscribe(state =>
               {
                 if (state == MessageStateId.CAN_ID_STATE_INFLATION || IsCancelEventSet)
                   _consoleSwitchState.Set();
               }))
      {
        // Currently, it should be in Ready State,
        // and sending Start command should notify console to switch to Inflation state
        _machineModel.Console.Start();
        succeeded = _consoleSwitchState.WaitOne(CatheterTestConstants.READY_TO_INFLATION_STATE_TIMEOUT_IN_SEC);
      }

      return succeeded && !IsCancelEventSet; 
    }
   
    protected void SetVacuum(bool enable)
    {
      if (enable)
        _machineModel.Console.Connect();
      else
        _machineModel.Console.Disconnect();

      _machineModel.IsVacuumDisconnected = !enable;
    }
  }
}
