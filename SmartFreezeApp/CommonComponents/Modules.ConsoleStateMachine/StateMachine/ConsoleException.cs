namespace Modules.ConsoleStateMachine.StateMachine;

public class ConsoleException(int id, string name)
{
  public static readonly ConsoleException Type1ConsoleException = new(1, nameof(Type1ConsoleException));
  public static readonly ConsoleException Type2ConsoleException = new(1, nameof(Type2ConsoleException));
  public static readonly ConsoleException Type3ConsoleException = new(1, nameof(Type3ConsoleException));
  public static readonly ConsoleException Type4ConsoleException = new(1, nameof(Type4ConsoleException));
  public static readonly ConsoleException Type5ConsoleException = new(1, nameof(Type5ConsoleException));
}