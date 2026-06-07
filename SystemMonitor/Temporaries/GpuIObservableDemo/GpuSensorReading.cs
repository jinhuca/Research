using CrystalMonitor.Hardware;
namespace GpuIObservableDemo; 

public class GpuSensorReading {
  public required string GpuName { get; init; }
  public HardwareType GpuType { get; init; }
  public required string SensorName { get; init; }
  public SensorType SensorType { get; init; }
  public float Value { get; init; }
  public DateTime Timestamp { get; init; }
}
