public interface ICar
{
  int Run();
}

public class BMW : ICar
{
  private int _miles = 0;

  public int Run()
  {
    return ++_miles;
  }
}

public class Ford : ICar
{
  private int _miles = 0;

  public int Run()
  {
    return ++_miles;
  }
}

public class Audi : ICar
{
  private int _miles = 0;

  public int Run()
  {
    return ++_miles;
  }

}
public class Driver
{
  private ICar _car = null;

  public Driver(ICar car)
  {
    _car = car;
  }

  public void RunCar()
  {
    Console.WriteLine("Running {0} - {1} mile ", _car.GetType().Name, _car.Run());
  }
}