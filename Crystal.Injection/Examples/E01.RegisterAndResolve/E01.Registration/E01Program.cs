using ClassAndInterface;
using Crystal;

// Create a DI Container from Crystal:
ICrystalContainer container = new CrystalContainer();

// Register type with Interface:
container.RegisterType<IPrinter, HomePrinter>();

// Resolve type instance:
var instance= container.Resolve<IPrinter>();

// Invoke type instance member:
instance.Do("Hello");

