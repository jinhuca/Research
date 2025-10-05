using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Unity;

using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator
{
  public partial class ConsoleSimulator : ICanBusCommunication, IGeneralPurposeInputOutput
  {
    private readonly IUnityContainer _unityContainer;
    private readonly IEventAggregator _eventAggregator;
    private readonly ISimulatorConfiguration _simulatorConfiguration;

    private IEnumerable<ICanBusMessageProvider> _canBusMessageProviders;
    public ConsoleSimulator(IUnityContainer unityContainer,
      IEventAggregator eventAggregator,
      ISimulatorConfiguration simulatorConfiguration)
    {
      _unityContainer = unityContainer;
      _eventAggregator = eventAggregator;
      _simulatorConfiguration = simulatorConfiguration;

      Initialize();
    }
    public MessageStateId CurrentSystemState { get; set; } = MessageStateId.CAN_ID_STATE_IDLE;

    public void Dispose()
    {
      if (_disposed)
        return;

      foreach (var provider in _canBusMessageProviders)
      {
        provider.Dispose();
      }

      _disposed = true;
    }

    private void Initialize()
    {
      // 1. Load configuration
      _simulatorConfiguration?.LoadConfiguration();

      // 2. Resolve all registered ICanBusMessageProvider
      ResolveMessageProviders();

      // 3. Create CanBus message processing threads
      CreateCanBusProcessThreads();

      // 4. Subscribe CanBusMessage event
      _eventAggregator.GetEvent<CanBusMessageUpdateEvent>().Subscribe(HandleCanBusMessageUpdate);
      _eventAggregator.GetEvent<SystemStateUpdateEvent>().Subscribe(HandleSystemStateUpdate);
    }
    private void ResolveMessageProviders()
    {
      _canBusMessageProviders = _unityContainer?.ResolveAll<ICanBusMessageProvider>();
      // Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ =>
      {
        foreach (var provider in _canBusMessageProviders)
        {
          provider.Initialize();
        }
      }
      // );
    }

    private void HandleCanBusMessageUpdate(CanBusMessage message)
    {
      if (message == null) return;

      if (message.Id == CanBusId.CanBus1)
        _canBusOneMessageQueue.Enqueue(message.CanBusEventArgs);
      else if (message.Id == CanBusId.CanBus2)
        _canBusTwoMessageQueue.Enqueue(message.CanBusEventArgs);
    }

    private void HandleSystemStateUpdate(ConsoleStateMessage stateUpdateMessage)
    {
      CurrentSystemState = stateUpdateMessage.State;
    }

  }
}
