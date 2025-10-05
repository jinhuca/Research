namespace Modules.Infrastructure.Models;

public class ActionOnConsole(int id, string name)
{
  public static readonly ActionOnConsole VacuumOff = new(0, nameof(VacuumOff));
  public static readonly ActionOnConsole VacuumOn = new(1, nameof(VacuumOn));

  public static readonly ActionOnConsole Start = new(3, nameof(Start));
  public static readonly ActionOnConsole Stop = new(4, nameof(Stop));
  
  public static readonly ActionOnConsole Inflate = new(5, nameof(Inflate));
  public static readonly ActionOnConsole Ablate = new(6, nameof(Ablate));
}