using ConsoleCommSimulator.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using static Communication.CanBusMessageDefinition;

namespace ConsoleSimulatorUnitTests
{
  public partial class ConsoleSimulatorTests
  {
    #region Connect/Disconnect
    [TestMethod]
    public void SetGPIOLevel_Connect_Then_From_Idle_To_Ready()
    {
      Test_Connect_Disconnect(MessageStateId.CAN_ID_STATE_IDLE, MessageStateId.CAN_ID_STATE_READY, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Disconnect_Then_From_Ready_To_Idle()
    {
      // the current value is DeactivateLevel by default, we set InjectionGPIO port to ActivateLevel
      // to make sure it won't publish 2 times for Disconnect test case
      _consoleSimulator.SetGPIOLevel((uint)GPIODefinitions.InjectionGPIOID, 1, (uint)ActiveLevel.ActivateLevel);
      Test_Connect_Disconnect(MessageStateId.CAN_ID_STATE_READY, MessageStateId.CAN_ID_STATE_IDLE, ActiveLevel.DeactivateLevel);
    }

    #endregion Connect/Disconnect

    #region Ablation

    [TestMethod]
    public void SetGPIOLevel_Ablate_Ready_To_Inflation()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_READY, MessageStateId.CAN_ID_STATE_INFLATION, GPIODefinitions.AblateGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Ablate_Inflation_To_Transition()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_INFLATION, MessageStateId.CAN_ID_STATE_TRANSITION, GPIODefinitions.AblateGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Ablate_Thawing_To_Transition()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_THAWING, MessageStateId.CAN_ID_STATE_TRANSITION, GPIODefinitions.AblateGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Stop_Inflation_To_Ready()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_INFLATION, MessageStateId.CAN_ID_STATE_READY, GPIODefinitions.StopGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Stop_Ablation_To_Thawing()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_ABLATION, MessageStateId.CAN_ID_STATE_THAWING, GPIODefinitions.StopGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Stop_Transition_To_Thawing()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_TRANSITION, MessageStateId.CAN_ID_STATE_THAWING, GPIODefinitions.StopGPIOID, ActiveLevel.ActivateLevel);
    }

    [TestMethod]
    public void SetGPIOLevel_Stop_Thawing_To_Ready()
    {
      Test_Ablate(MessageStateId.CAN_ID_STATE_THAWING, MessageStateId.CAN_ID_STATE_READY, GPIODefinitions.StopGPIOID, ActiveLevel.ActivateLevel);
    }

    #endregion

    #region private methods
    private void Test_Connect_Disconnect(MessageStateId initState, MessageStateId expectedState, ActiveLevel gpioLevel)
    {
      _consoleSimulator.CurrentSystemState = initState; ;

      Assert.IsNotNull(_systemStateUpdateEventHandler);

      ConsoleStateMessage publishedMessage = null;
      _systemStateUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<ConsoleStateMessage>()))
        .Callback<ConsoleStateMessage>(m => publishedMessage = m);

      _consoleSimulator.SetGPIOLevel((uint)GPIODefinitions.VacuumGPIOID, 1, (uint)gpioLevel);
      _consoleSimulator.SetGPIOLevel((uint)GPIODefinitions.InjectionGPIOID, 1, (uint)gpioLevel);

      Assert.IsNotNull(publishedMessage);
      _systemStateUpdateEventMock.Verify(x => x.Publish(It.IsAny<ConsoleStateMessage>()), Times.Once);

      Assert.AreEqual(expectedState, publishedMessage.State);

      _systemStateUpdateEventHandler.Invoke(publishedMessage);
      Assert.AreEqual(expectedState, _consoleSimulator.CurrentSystemState);
    }

    private void Test_Ablate(MessageStateId initState, MessageStateId expectedState, GPIODefinitions gpioDef,  ActiveLevel gpioLevel)
    {
      _consoleSimulator.CurrentSystemState = initState;

      Assert.IsNotNull(_systemStateUpdateEventHandler);

      ConsoleStateMessage publishedMessage = null;
      _systemStateUpdateEventMock
        .Setup(x => x.Publish(It.IsAny<ConsoleStateMessage>()))
        .Callback<ConsoleStateMessage>(m => publishedMessage = m);

      _consoleSimulator.SetGPIOLevel((uint)gpioDef, 1, (uint)gpioLevel);

      Assert.IsNotNull(publishedMessage);
      _systemStateUpdateEventMock.Verify(x => x.Publish(It.IsAny<ConsoleStateMessage>()), Times.Once);

      Assert.AreEqual(expectedState, publishedMessage.State);

      _systemStateUpdateEventHandler.Invoke(publishedMessage);
      Assert.AreEqual(expectedState, _consoleSimulator.CurrentSystemState);
    }
    #endregion private methods
  }
}
