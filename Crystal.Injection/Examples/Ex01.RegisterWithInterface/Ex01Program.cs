using Crystal;

ICrystalContainer container = new CrystalContainer();

// Register type with a interface.
container.RegisterType<IService, CustomerService>();

// Resolve type instance with generic method.
var instance = container.Resolve<IService>();
instance.SayHello("Jon");

// Resolve type instance with non-generic method and type casting.
var instance1 = (IService)container.Resolve(typeof(IService));
instance1.SayHello("Mary");

public interface IService
{
  void SayHello(string n);
}

public class CustomerService : IService
{
  public void SayHello(string name) => Console.WriteLine($"Hello {name}!");
}