using CrystalMonitor.Hardware;
using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;
using DataStructures.Types;

namespace DataStructures.Cpu.Implementations;

public class CpuCoreLiveInfo : ICpuCoreLiveInfo {
  public string Name { get; set; } = string.Empty;
  public SensorReading Voltage { get; set; } = new SensorReading(string.Empty, HardwareType.Cpu, string.Empty, SensorType.Voltage, 0.0f, 0.0f, 0.0f, null);
  public SensorReading Speed { get; set; } = new SensorReading(string.Empty, HardwareType.Cpu, string.Empty, SensorType.Clock, 0.0f, 0.0f, 0.0f, null);
  public SensorReading Temperature { get; set; } = new SensorReading(string.Empty, HardwareType.Cpu, string.Empty, SensorType.Temperature, 0.0f, 0.0f, 0.0f, null);
  public SensorReading Load { get; set; } = new SensorReading(string.Empty, HardwareType.Cpu, string.Empty, SensorType.Load, 0.0f, 0.0f, 0.0f, null);
}
