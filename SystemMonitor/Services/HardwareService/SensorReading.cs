using CrystalMonitor.Hardware;

namespace HardwareService;

public record SensorReading(
  string HardwareName,
  HardwareType HardwareType,
  string SensorName,
  SensorType SensorType,
  float? Value,
  string? Unit
);
