using DataStructures.Types;

namespace CpuModule.ViewModels.Interfaces;

public interface ICoreLiveViewModel {
  string Name { get; set; }
  SensorReading Voltage { get; set; }
  SensorReading Temperature { get; set; }
  SensorReading Load { get; set; }
  SensorReading Speed { get; set; }
}
