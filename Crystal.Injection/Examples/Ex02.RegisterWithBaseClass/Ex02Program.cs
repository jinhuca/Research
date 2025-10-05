using Crystal;

ICrystalContainer container = new CrystalContainer();

// Resolving types registered as base class.
container.RegisterType<MyBaseService, MyDerivedService>();
var instance = container.Resolve<MyBaseService>();
instance.SayHello("Jon");

// Resolving types registered with non-generic method.
container.RegisterType(typeof(MyBaseService), typeof(MyDerivedService));
var instance1 = (MyBaseService)container.Resolve(typeof(MyBaseService));
instance1.SayHello("Mary");

public class MyDerivedService : MyBaseService
{
  public override void SayHello(string n) => Console.WriteLine($"Hello {n} from {nameof(MyDerivedService)}.");
}

public class MyBaseService
{
  public virtual void SayHello(string n) => Console.WriteLine($"Hello {n} from {nameof(MyBaseService)}.)");
}