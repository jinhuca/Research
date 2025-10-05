using Crystal;

ICrystalContainer container = new CrystalContainer();
container.RegisterType<IService, CustomerService>();
container.RegisterType<IService, CompanyService>("name");
container.RegisterType<IService, EmailService>("other name");

var instance = container.Resolve<IService>("name");
instance.SayHello("Jon");

public interface IService
{
  void SayHello(string n);
}

public class CustomerService : IService
{
  public void SayHello(string n)
  {
    Console.WriteLine($"Hello {n} from {nameof(CustomerService)}.");
  }
}

public class CompanyService : IService
{
  public void SayHello(string name) => Console.WriteLine($"Hello {name} in {nameof(CompanyService)}.");
}

public class EmailService : IService
{
  public void SayHello(string name) => Console.WriteLine($"Hello {name} from {nameof(EmailService)}.");
}