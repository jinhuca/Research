using DataStructures.TypeDefinitions;

namespace CpuModule.ViewModels.Interfaces;

public interface ICoreLiveViewModel {
  string Name { get; set; }
  SensorDataType Voltage { get; set; }
  SensorDataType Temperature { get; set; }
  SensorDataType Load { get; set; }
  SensorDataType Speed { get; set; }
}
