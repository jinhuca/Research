using Modules.ConsoleStateMachine.States;
using Modules.Infrastructure.Definitions;

namespace Modules.ConsoleStateMachine.Tests;

public class ConsoleTestFixture
{
  [Fact]
  public void ConsoleStateTests()
  {
    var s1 = ConsoleState.Unknown;
    var s2 = ConsoleState.Idle;
    Assert.NotEqual(s1, s2);
  }


  [Fact]
  public void ConsoleStateEqualityTests()
  {
    var consoleState1 = ConsoleState.Ablation;
    var consoleState2 = ConsoleState.Ablation;
    Assert.Equal(consoleState1, consoleState2);
  }

  [Fact]
  public void ConsoleStateNameIdTests()
  {
    var consoleState1 = ConsoleState.Ready;
    Assert.Equal(Enum.GetName(ConsoleStateId.ReadyStateId), consoleState1.Name);
    Assert.Equal(ConsoleStateId.ReadyStateId, consoleState1.Id);
    Assert.Equal((ConsoleStateId)512, consoleState1.Id);
  }

  [Fact]
  public void EnumerationTests()
  {
    Assert.True(ConsoleState.GetAllStates().Any());
  }

  [Fact]
  public void SwitchConsoleStateTests()
  {
    var cs = ConsoleState.Exception;
  }

  [Fact]
  public void EnumerateStatesTests()
  {
    Assert.Contains(ConsoleState.Unknown, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Idle, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Ready, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Inflation, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Transition, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Ablation, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Thawing, ConsoleState.GetAllStates());
    Assert.Contains(ConsoleState.Exception, ConsoleState.GetAllStates());
  }

  [Fact]
  public void ExceptionTests()
  {
    var e1 = new ConsoleState(ConsoleStateId.ExceptionStateId); ;
    var e2 = new ConsoleState(ConsoleStateId.ExceptionStateId);

    Assert.Equal(e1, e2);
  }
}