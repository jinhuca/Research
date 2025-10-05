using Communication;
using ConsoleCommSimulator.Data;

namespace ConsoleCommSimulator
{
  public partial class ConsoleSimulator
  {
    private readonly GPIOState _gpioState = new GPIOState();

    public void SetGPIOLevel(uint Id, uint mask, uint level)
    {
      bool setToActivateLevel = level == (int)ActiveLevel.ActivateLevel;

      switch ((GPIODefinitions)Id)
      {
        case GPIODefinitions.StopGPIOID:
          _gpioState.StopGPIO = setToActivateLevel;
          HandleStopRequest(setToActivateLevel);
          break;

        case GPIODefinitions.AblateGPIOID:
          _gpioState.AblateGPIO = setToActivateLevel;
          HandleAblationRequest(setToActivateLevel);
          break;

        case GPIODefinitions.FailResetGPIOID:
          _gpioState.FailResetStatus = setToActivateLevel;
          HandleFailResetRequest();
          break;
        case GPIODefinitions.InjectionGPIOID:
          _gpioState.InjectionResetGPIO = setToActivateLevel;
          HandleConnectRequest();
          break;
        case GPIODefinitions.VacuumGPIOID:
          _gpioState.VacuumResetGPIO = setToActivateLevel;
          HandleConnectRequest();
          break;
        case GPIODefinitions.WatchdogResetGPIOID:
          _gpioState.WatchdogResetGPIO = setToActivateLevel;
          break;
        case GPIODefinitions.SystemResetGPIOID:
          _gpioState.SystemResetGPIO = setToActivateLevel;
          break;
        case GPIODefinitions.ChangeTankGPIOID:
          _gpioState.ChangeTankGPIO = setToActivateLevel;
          break;
      }
    }

    public void SetGPIODirection(uint Id, uint mask, uint level)
    {
    }

    private void HandleStopRequest(bool active)
    {
      // we will receive a pulse (false -> true -> false), will ignore false
      if (active)
      {
        // Inflation -> Ready; Ablation/Transition -> Thawing; Thawing -> Ready 
        var newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;
        switch (CurrentSystemState)
        {
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
            break;
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
            break;
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
            break;
          default:
            break;
        }

        if (newState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          _eventAggregator
            .GetEvent<SystemStateUpdateEvent>()
            .Publish(new ConsoleStateMessage() { State = newState });
        }
      }
    }

    private void HandleAblationRequest(bool active)
    {
      // we will receive a pulse (false -> true -> false), will ignore false
      if (active)
      {
        // Ready -> Inflation; Inflation -> Transition; Thawing -> Transition 
        var newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;
        switch (CurrentSystemState)
        {
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;
            break;
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            break;
          case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
            newState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            break;
          default:
            break;
        }

        if (newState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          _eventAggregator
            .GetEvent<SystemStateUpdateEvent>()
            .Publish(new ConsoleStateMessage() { State = newState });
        }
      }
    }

    private void HandleConnectRequest()
    {
      if (_gpioState.VacuumResetGPIO && _gpioState.InjectionResetGPIO)
      {
        // Connect request, switch to Ready state if current is Idle
        if (CurrentSystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
        {
          _eventAggregator
            .GetEvent<SystemStateUpdateEvent>()
            .Publish(new ConsoleStateMessage() { State = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY });
        }
      }
      else if (!_gpioState.VacuumResetGPIO && !_gpioState.InjectionResetGPIO)
      {
        // Disconnect request, switch to Idle if current is not Exception
        if (CurrentSystemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          _eventAggregator
            .GetEvent<SystemStateUpdateEvent>()
            .Publish(new ConsoleStateMessage() { State = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE });
        }
      }
    }
    private void HandleFailResetRequest()
    {
      if (CurrentSystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
      {
        _eventAggregator
          .GetEvent<SystemStateUpdateEvent>()
          .Publish(new ConsoleStateMessage() { State = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE });
        _eventAggregator
          .GetEvent<ThresholdValidationFailedEvent>()
          .Publish(new ThresholdValidationFailedEventArgs(ThresholdStatusType.CLEAR_CMCU_STATUS, 0) );
        _eventAggregator
          .GetEvent<ThresholdValidationFailedEvent>()
          .Publish(new ThresholdValidationFailedEventArgs(ThresholdStatusType.CLEAR_PMCU_STATUS, 0) );
      }
    }

  }
}
