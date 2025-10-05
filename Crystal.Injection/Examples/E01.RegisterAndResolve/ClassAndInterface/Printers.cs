using static System.Console;

namespace ClassAndInterface;

public interface IPrinter
{
  void Do(string text);
}

public class HomePrinter : IPrinter
{
  public void Do(string text) => WriteLine($"Print {text} on {nameof(HomePrinter)}");
}

public class OfficePrinter : IPrinter
{
  public void Do(string text) => WriteLine($"Print {text} on {nameof(OfficePrinter)}");
}

