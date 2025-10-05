
using Communication;

namespace ConsoleCommSimulator.Data
{
  public enum CanBusId
  {
    CanBus1,
    CanBus2
  }

  public class CanBusMessage
  {
    public CanBusId Id { get; set; }
    public CanBusEventArgs CanBusEventArgs { get; set; }
  }
}
