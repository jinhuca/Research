using CrystalMonitor.Hardware;

namespace DataStructures.Types;

public record SensorReading(
    string HardwareName,
    HardwareType HardwareType,
    string SensorName,
    SensorType SensorType,
    float? Value,
    float? Min,
    float? Max,
    string? Unit);

