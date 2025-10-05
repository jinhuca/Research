using Crystal;

ICrystalContainer container = new CrystalContainer();
container.RegisterType<ICar, BMW>();
container.RegisterType<ICar, Audi>();

Driver driver = container.Resolve<Driver>();
driver.RunCar();