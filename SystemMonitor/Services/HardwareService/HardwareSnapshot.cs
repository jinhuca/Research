namespace HardwareService;

public record HardwareSnapshot(
  DateTimeOffset Timestamp,
  IReadOnlyList<SensorReading> Readings
);
