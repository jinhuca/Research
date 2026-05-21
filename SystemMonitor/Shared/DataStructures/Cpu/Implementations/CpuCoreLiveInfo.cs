using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;

namespace DataStructures.Cpu.Implementations;

public class CpuCoreLiveInfo : ICpuCoreLiveInfo {
  public string Name { get; set; } = string.Empty;
  public SensorDataType Voltage { get; set; } = (0.0f, 0.0f, 0.0f);
  public SensorDataType Speed { get; set; } = (0.0f, 0.0f, 0.0f);
  public SensorDataType Temperature { get; set; } = (0.0f, 0.0f, 0.0f);
  public SensorDataType Load { get; set; } = (0.0f, 0.0f, 0.0f);
}
