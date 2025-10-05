
using System;
using ConsoleCommSimulator;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Unity;

namespace ConsoleSimulatorUnitTests
{
  [TestClass]
  public partial class ThresholdValidationTests
  {
    private ConsoleSimulator _consoleSimulator;

    private Mock<IEventAggregator> _eventAggregatorMock;
    private Mock<SystemStateUpdateEvent> _systemStateUpdateEventMock;
    private Mock<CanBusMessageUpdateEvent> _canBusUpdateEventMock;

    private IUnityContainer _unityContainer;
    private Mock<ISimulatorConfiguration> _simulatorConfigurationMock;

    private Action<ConsoleStateMessage> _systemStateUpdateEventHandler;

    private Mock<ICanBusMessageProvider> _canBusMessageProviderMock;

    [TestInitialize]
    public void Setup()
    {
      _canBusMessageProviderMock = new Mock<ICanBusMessageProvider>();
      _unityContainer = new UnityContainer();

      _unityContainer.RegisterInstance(_canBusMessageProviderMock.Object);
      
      _simulatorConfigurationMock = new Mock<ISimulatorConfiguration>();

      _eventAggregatorMock = new Mock<IEventAggregator>();
      _systemStateUpdateEventMock = new Mock<SystemStateUpdateEvent>();

      _eventAggregatorMock
        .Setup(x => x.GetEvent<SystemStateUpdateEvent>())
        .Returns(_systemStateUpdateEventMock.Object);

      _systemStateUpdateEventMock.Setup(x => x.Subscribe(It.IsAny<Action<ConsoleStateMessage>>(),
          It.IsAny<ThreadOption>(),
          It.IsAny<bool>(),
          It.IsAny<Predicate<ConsoleStateMessage>>()))
        .Callback<Action<ConsoleStateMessage>, ThreadOption, bool, Predicate<ConsoleStateMessage>>(
          (action, _, __, ___) => _systemStateUpdateEventHandler = action);

      _canBusUpdateEventMock = new Mock<CanBusMessageUpdateEvent>();
      _eventAggregatorMock
        .Setup(x => x.GetEvent<CanBusMessageUpdateEvent>())
        .Returns(_canBusUpdateEventMock.Object);
      
      _consoleSimulator = new ConsoleSimulator(_unityContainer, _eventAggregatorMock.Object, _simulatorConfigurationMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
      _consoleSimulator?.Dispose();
    }

  }
}
