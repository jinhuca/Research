
using ConsoleCommSimulator.Data;

namespace ConsoleCommSimulator.Interfaces
{
  public interface ICanBusMessageProvider
  {
    void Initialize();
    void UpdateParameters(CanBusMessageParameters parameters);
    void Dispose();
  }
}
