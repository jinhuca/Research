using CrystalMonitor.Hardware;
using DataStructures.Types;

namespace DataStructures.Tests;

// ── Tests ─────────────────────────────────────────────────────────────────────

public class SensorReadingExtensionsTests {
  // ── UnitFor — one InlineData per switch arm in the real source ─────────────

  [Theory]
  [InlineData(SensorType.Voltage, "V")]
  [InlineData(SensorType.Clock, "MHz")]
  [InlineData(SensorType.Temperature, "°C")]
  [InlineData(SensorType.Load, "%")]
  [InlineData(SensorType.Power, "W")]
  [InlineData(SensorType.Fan, "RPM")]
  [InlineData(SensorType.Flow, "L/h")]
  [InlineData(SensorType.Control, "%")]
  [InlineData(SensorType.Level, "%")]
  [InlineData(SensorType.Factor, "")]
  [InlineData(SensorType.Data, "GB")]
  [InlineData(SensorType.SmallData, "MB")]
  [InlineData(SensorType.Throughput, "B/s")]
  [InlineData(SensorType.Frequency, "Hz")]
  [InlineData(SensorType.Energy, "mWh")]
  [InlineData(SensorType.Current, "A")]
  [InlineData(SensorType.Humidity, "%")]
  public void UnitFor_KnownSensorType_ReturnsExpectedUnit(SensorType type, string expected) {
    Assert.Equal(expected, SensorReadingExtensions.UnitFor(type));
  }

  [Fact]
  public void UnitFor_UnknownSensorType_ReturnsNull() {
    Assert.Null(SensorReadingExtensions.UnitFor((SensorType)9999));
  }

  [Fact]
  public void UnitFor_TimeSpanSensorType_ReturnsNull() {
    // TimeSpan is a real, defined SensorType but has no switch arm here, unlike
    // every other documented sensor type — same gap surfaced from the
    // HardwareService side earlier. Pinning current behavior on purpose.
    Assert.Null(SensorReadingExtensions.UnitFor(SensorType.TimeSpan));
  }

  // ── ToReading ─────────────────────────────────────────────────────────────
  // NOTE: DataStructures.Types.SensorReading's own source wasn't available when
  // writing this. These tests build the "expected" object through the exact
  // same positional constructor ToReading itself calls internally, and rely on
  // record structural equality to compare — so they don't need to know
  // SensorReading's property names. If SensorReading turns out NOT to be a
  // record (no value equality), Assert.Equal below will fail with a clear
  // side-by-side dump of both objects — that's the signal to swap these for
  // named-property assertions once we see the type definition.

  [Fact]
  public void ToReading_WithSensor_MapsAllFields() {
    var sensor = new FakeSensor {
      Name = "Core Temp",
      SensorType = SensorType.Temperature,
      Value = 65.5f,
      Min = 30f,
      Max = 90f,
    };

    var expected = new SensorReading(
        "Intel CPU", HardwareType.Cpu, "Core Temp", SensorType.Temperature, 65.5f, 30f, 90f, "°C");

    var actual = SensorReadingExtensions.ToReading(sensor, "Intel CPU", HardwareType.Cpu);

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void ToReading_NullSensor_FallsBackToEmptyPlaceholder() {
    var expected = new SensorReading(
        "Intel CPU", HardwareType.Cpu, string.Empty, SensorType.Load, null, null, null, null);

    var actual = SensorReadingExtensions.ToReading(null, "Intel CPU", HardwareType.Cpu);

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void ToReading_SensorTypeWithNoUnit_OmitsUnit() {
    var sensor = new FakeSensor { SensorType = SensorType.TimeSpan };

    var expected = new SensorReading(
        "Board", HardwareType.Motherboard, sensor.Name, SensorType.TimeSpan, sensor.Value, sensor.Min, sensor.Max, null);

    var actual = SensorReadingExtensions.ToReading(sensor, "Board", HardwareType.Motherboard);

    Assert.Equal(expected, actual);
  }
}
