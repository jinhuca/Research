using DataStructures.TypeDefinitions;

namespace DataStructures.Cpu.Interfaces;

public interface ICpuCoreLiveInfo {
  string Name { get; set; }
  SensorDataType Voltage { get; set; }
  SensorDataType Speed { get; set; }
  SensorDataType Temperature { get; set; }
  SensorDataType Load { get; set; }
}
