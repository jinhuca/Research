using Crystal;

ICrystalContainer container = new CrystalContainer();

IPrinter printer1 = new Printer1();

container.RegisterInstance<IPrinter>(printer1);
container.RegisterType<IPrinter, NeighborPrinter>();

public interface IPrinter
{

}

public class Printer : IPrinter
{

}

public class Printer1 : IPrinter
{

}

public class Printer2 : IPrinter
{

}

public class NeighborPrinter : IPrinter
{

}