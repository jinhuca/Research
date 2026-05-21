using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;

namespace DataStructures.Cpu.Implementations;

public class CpuCoreLiveInfo : ICpuCoreLiveInfo {
  public string Name { get; set; } = string.Empty;
  public SensorDataType Voltage { get; set; } = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Speed { get; set; } = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Temperature { get; set; } = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Load { get; set; } = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
}
