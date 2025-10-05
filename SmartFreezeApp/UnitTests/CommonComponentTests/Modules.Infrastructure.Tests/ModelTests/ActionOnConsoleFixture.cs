using Modules.Infrastructure.Models;

namespace Modules.Infrastructure.Tests.ModelTests;

public class ActionOnConsoleFixture
{
  [Fact]
  public void ActionEqualityTests()
  {
    var a1 = ActionOnConsole.VacuumOn;
    var a2 = ActionOnConsole.VacuumOff;
    Assert.NotEqual(a1, a2);
  }

}