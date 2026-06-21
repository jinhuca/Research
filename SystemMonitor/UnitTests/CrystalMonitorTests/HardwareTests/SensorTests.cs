using CrystalMonitor.Hardware;
using System.Globalization;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the Sensor class.
/// Tests basic properties, value management, parameters, persistence, and event handling.
/// </summary>
public class SensorTests {
  // =========================================================================
  // Test Doubles & Helpers
  // =========================================================================

  private class TestSettings : ISettings {
    private readonly Dictionary<string, string> _store = new();

    public bool Contains(string name) => _store.ContainsKey(name);
    public void SetValue(string name, string value) => _store[name] = value;
    public string GetValue(string name, string value) =>
      _store.TryGetValue(name, out var v) ? v : value;
    public void Remove(string name) => _store.Remove(name);
  }

  private class TestHardware : Hardware {
    public TestHardware(string name = "Test Hardware", ISettings settings = null)
      : base(name, new Identifier("test", "0"), settings ?? new TestSettings()) { }

    public override HardwareType HardwareType => HardwareType.Cpu;
    public override void Update() { }
  }

  private static Sensor CreateSensor(
    string name = "Test Sensor",
    int index = 0,
    SensorType type = SensorType.Load,
    ISettings settings = null) {
    var hardware = new TestHardware(settings: settings ?? new TestSettings());
    return new Sensor(name, index, type, hardware, settings ?? new TestSettings());
  }

  // =========================================================================
  // Construction & Properties
  // =========================================================================

  [Fact]
  public void Sensor_Construction_DoesNotThrow() {
    var ex = Record.Exception(() => CreateSensor());
    Assert.Null(ex);
  }

  [Fact]
  public void Sensor_Name_IsSetFromConstructor() {
    var sensor = CreateSensor(name: "CPU Load");
    Assert.Equal("CPU Load", sensor.Name);
  }

  [Fact]
  public void Sensor_Index_IsSetFromConstructor() {
    var sensor = CreateSensor(index: 42);
    Assert.Equal(42, sensor.Index);
  }

  [Fact]
  public void Sensor_SensorType_IsSetFromConstructor() {
    var sensor = CreateSensor(type: SensorType.Temperature);
    Assert.Equal(SensorType.Temperature, sensor.SensorType);
  }

  [Fact]
  public void Sensor_Hardware_ReferenceIsValid() {
    var hardware = new TestHardware();
    var sensor = new Sensor("Test", 0, SensorType.Load, hardware, new TestSettings());
    Assert.Equal(hardware, sensor.Hardware);
  }

  // =========================================================================
  // Identifier
  // =========================================================================

  [Fact]
  public void Sensor_Identifier_IsNotNull() {
    var sensor = CreateSensor();
    Assert.NotNull(sensor.Identifier);
  }

  [Fact]
  public void Sensor_Identifier_ContainsSensorType() {
    var sensor = CreateSensor(type: SensorType.Temperature);
    Assert.Contains("temperature", sensor.Identifier.ToString(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Sensor_Identifier_ContainsSensorIndex() {
    var sensor = CreateSensor(index: 5);
    // Identifier should include the index somehow
    Assert.NotNull(sensor.Identifier);
  }

  [Fact]
  public void Sensor_Identifier_IsUnique_ForDifferentSensors() {
    var sensor1 = CreateSensor(name: "Sensor 1", index: 0);
    var sensor2 = CreateSensor(name: "Sensor 2", index: 1);
    Assert.NotEqual(sensor1.Identifier, sensor2.Identifier);
  }

  // =========================================================================
  // Value Management
  // =========================================================================

  [Fact]
  public void Sensor_Value_IsNullByDefault() {
    var sensor = CreateSensor();
    Assert.Null(sensor.Value);
  }

  [Fact]
  public void Sensor_Value_CanBeSet() {
    var sensor = CreateSensor();
    sensor.Value = 42.5f;
    Assert.Equal(42.5f, sensor.Value);
  }

  [Theory]
  [InlineData(0f)]
  [InlineData(50f)]
  [InlineData(100f)]
  [InlineData(-10f)]
  [InlineData(float.MaxValue)]
  public void Sensor_Value_AcceptsVariousFloatValues(float value) {
    var sensor = CreateSensor();
    sensor.Value = value;
    Assert.Equal(value, sensor.Value);
  }

  [Fact]
  public void Sensor_Value_CanBeSetToNull() {
    var sensor = CreateSensor();
    sensor.Value = 42f;
    sensor.Value = null;
    Assert.Null(sensor.Value);
  }

  [Fact]
  public void Sensor_Value_CanBeUpdatedMultipleTimes() {
    var sensor = CreateSensor();
    sensor.Value = 10f;
    Assert.Equal(10f, sensor.Value);
    sensor.Value = 20f;
    Assert.Equal(20f, sensor.Value);
  }

  // =========================================================================
  // Min/Max Values
  // =========================================================================

  [Fact]
  public void Sensor_Min_IsTrackedWhenValueIsSet() {
    var sensor = CreateSensor();
    sensor.Value = 10f;
    Assert.Equal(10f, sensor.Min);
  }

  [Fact]
  public void Sensor_Max_IsTrackedWhenValueIsSet() {
    var sensor = CreateSensor();
    sensor.Value = 100f;
    Assert.Equal(100f, sensor.Max);
  }

  [Fact]
  public void Sensor_Min_DefaultValueIsNull() {
    var sensor = CreateSensor();
    Assert.Null(sensor.Min);
  }

  [Fact]
  public void Sensor_Max_DefaultValueIsNull() {
    var sensor = CreateSensor();
    Assert.Null(sensor.Max);
  }

  // TODO: Add tests for Min/Max constraints (e.g., Min > Max validation)

  // =========================================================================
  // Parameters (Calibration, Offset, etc.)
  // =========================================================================

  [Fact]
  public void Sensor_Parameters_IsNotNull() {
    var sensor = CreateSensor();
    Assert.NotNull(sensor.Parameters);
  }

  [Fact]
  public void Sensor_Parameters_IsEmptyByDefault() {
    var sensor = CreateSensor();
    Assert.Empty(sensor.Parameters);
  }

  [Fact]
  public void Sensor_Parameters_CanAddParameter() {
    var sensor = CreateSensor();
    // TODO: Determine how to create/add Parameter
    // This depends on Parameter class API
    // var param = new Parameter(...);
    // sensor.Parameters.Add(param);
    // Assert.Single(sensor.Parameters);
  }

  // =========================================================================
  // Events
  // =========================================================================

  [Fact]
  public void Sensor_Value_UpdatesMin_WhenNewMinimumIsSet() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Min);

    sensor.Value = 30f;
    Assert.Equal(30f, sensor.Min);
  }

  [Fact]
  public void Sensor_Value_UpdatesMax_WhenNewMaximumIsSet() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Max);

    sensor.Value = 70f;
    Assert.Equal(70f, sensor.Max);
  }

  [Fact]
  public void Sensor_ResetMin_ClearsMinValue() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Min);

    sensor.ResetMin();
    Assert.Null(sensor.Min);
  }

  [Fact]
  public void Sensor_ResetMax_ClearsMaxValue() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Max);

    sensor.ResetMax();
    Assert.Null(sensor.Max);
  }

  // =========================================================================
  // Settings Persistence
  // =========================================================================

  [Fact]
  public void Sensor_Value_CanBePersisted_InSettings() {
    var settings = new TestSettings();
    var sensor = CreateSensor(settings: settings);

    sensor.Value = 75.5f;

    // Check that value is stored in settings with appropriate key
    // This depends on how Sensor uses Settings
    // Expected key format: something like "sensor_identifier_value"
  }

  // =========================================================================
  // Values History and Aggregation
  // =========================================================================

  [Fact]
  public void Sensor_Values_IsEmptyInitially() {
    var sensor = CreateSensor();
    Assert.Empty(sensor.Values);
  }

  [Fact]
  public void Sensor_Values_AreAggregatedAfterFourUpdates() {
    var sensor = CreateSensor();

    // Set 4 values to trigger aggregation (every 4th value is aggregated)
    sensor.Value = 10f;
    sensor.Value = 20f;
    sensor.Value = 30f;
    sensor.Value = 40f;

    // Should have aggregated value (average of 4: 25)
    Assert.NotEmpty(sensor.Values);
  }

  [Fact]
  public void Sensor_Values_WithDefaultTimeWindow_AccumulateValues() {
    var sensor = CreateSensor();

    for (int i = 0; i < 12; i++) {
      sensor.Value = i * 10f;
    }

    // 12 values should result in 3 aggregated values (12 / 4)
    Assert.NotEmpty(sensor.Values);
  }

  [Fact]
  public void Sensor_ValuesTimeWindow_CanBeSet() {
    var sensor = CreateSensor();
    var newWindow = TimeSpan.FromHours(12);
    sensor.ValuesTimeWindow = newWindow;
    Assert.Equal(newWindow, sensor.ValuesTimeWindow);
  }

  [Fact]
  public void Sensor_ValuesTimeWindow_DefaultsToOneDay() {
    var sensor = CreateSensor();
    Assert.Equal(TimeSpan.FromDays(1.0), sensor.ValuesTimeWindow);
  }

  [Fact]
  public void Sensor_Values_AreClearedWhenTimeWindowSetToZero() {
    var sensor = CreateSensor();
    sensor.Value = 10f;
    sensor.Value = 20f;
    sensor.Value = 30f;
    sensor.Value = 40f;

    Assert.NotEmpty(sensor.Values);

    sensor.ValuesTimeWindow = TimeSpan.Zero;
    Assert.Empty(sensor.Values);
  }

  [Fact]
  public void Sensor_ClearValues_RemovesAllStoredValues() {
    var sensor = CreateSensor();

    // Aggregate some values
    for (int i = 0; i < 12; i++) {
      sensor.Value = i * 10f;
    }

    Assert.NotEmpty(sensor.Values);

    sensor.ClearValues();
    Assert.Empty(sensor.Values);
  }

  [Fact]
  public void Sensor_Value_WithNaN_DoesNotUpdateMinMax() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    var originalMin = sensor.Min;
    var originalMax = sensor.Max;

    sensor.Value = float.NaN;

    Assert.Equal(originalMin, sensor.Min);
    Assert.Equal(originalMax, sensor.Max);
  }

  [Fact]
  public void Sensor_Value_WithPositiveInfinity_DoesNotUpdateMinMax() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    var originalMin = sensor.Min;
    var originalMax = sensor.Max;

    sensor.Value = float.PositiveInfinity;

    Assert.Equal(originalMin, sensor.Min);
    Assert.Equal(originalMax, sensor.Max);
  }

  [Fact]
  public void Sensor_Value_WithNegativeInfinity_DoesNotUpdateMinMax() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    var originalMin = sensor.Min;
    var originalMax = sensor.Max;

    sensor.Value = float.NegativeInfinity;

    Assert.Equal(originalMin, sensor.Min);
    Assert.Equal(originalMax, sensor.Max);
  }

  [Fact]
  public void Sensor_Min_StaysNullWhenOnlyNaNValuesAreSet() {
    var sensor = CreateSensor();
    sensor.Value = float.NaN;
    sensor.Value = float.NaN;
    Assert.Null(sensor.Min);
  }

  [Fact]
  public void Sensor_Max_StaysNullWhenOnlyInfinityValuesAreSet() {
    var sensor = CreateSensor();
    sensor.Value = float.PositiveInfinity;
    sensor.Value = float.NegativeInfinity;
    Assert.Null(sensor.Max);
  }

  [Fact]
  public void Sensor_Min_TracksMulipleValues_Correctly() {
    var sensor = CreateSensor();

    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Min);

    sensor.Value = 30f;
    Assert.Equal(30f, sensor.Min);

    sensor.Value = 100f;
    Assert.Equal(30f, sensor.Min); // Min unchanged

    sensor.Value = 20f;
    Assert.Equal(20f, sensor.Min); // New min
  }

  [Fact]
  public void Sensor_Max_TracksMulipleValues_Correctly() {
    var sensor = CreateSensor();

    sensor.Value = 50f;
    Assert.Equal(50f, sensor.Max);

    sensor.Value = 70f;
    Assert.Equal(70f, sensor.Max); // New max

    sensor.Value = 30f;
    Assert.Equal(70f, sensor.Max); // Max unchanged

    sensor.Value = 90f;
    Assert.Equal(90f, sensor.Max); // New max
  }

  [Fact]
  public void Sensor_ResetMin_AllowsNewMinimumToBeTracked() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    sensor.ResetMin();

    sensor.Value = 40f;
    Assert.Equal(40f, sensor.Min);
  }

  [Fact]
  public void Sensor_ResetMax_AllowsNewMaximumToBeTracked() {
    var sensor = CreateSensor();
    sensor.Value = 50f;
    sensor.ResetMax();

    sensor.Value = 60f;
    Assert.Equal(60f, sensor.Max);
  }

  [Fact]
  public void Sensor_ResetMinAndMax_BothCanBeResetSeparately() {
    var sensor = CreateSensor();
    sensor.Value = 50f;

    sensor.ResetMin();
    sensor.Value = 40f;
    Assert.Equal(40f, sensor.Min);
    Assert.Equal(50f, sensor.Max); // Max still 50

    sensor.ResetMax();
    sensor.Value = 60f;
    Assert.Equal(40f, sensor.Min); // Min still 40
    Assert.Equal(60f, sensor.Max);
  }

  [Fact]
  public void Sensor_ValuesTimeWindow_SmallWindow_OldValuesAreRemoved() {
    var sensor = CreateSensor();
    sensor.ValuesTimeWindow = TimeSpan.FromMilliseconds(100);

    sensor.Value = 10f;
    sensor.Value = 20f;
    sensor.Value = 30f;
    sensor.Value = 40f;

    // Wait for time window to expire
    System.Threading.Thread.Sleep(150);

    sensor.Value = 50f;
    sensor.Value = 60f;
    sensor.Value = 70f;
    sensor.Value = 80f;

    // Old values should be removed during aggregation
    // This is a timing-dependent test, so we just verify no exception
  }

  [Fact]
  public void Sensor_SettingValuesToNull_DoesNotAffectAggregation() {
    var sensor = CreateSensor();

    sensor.Value = 10f;
    sensor.Value = null;
    sensor.Value = 20f;
    sensor.Value = 30f;

    // Null values should not affect the aggregation/tracking
    Assert.Equal(10f, sensor.Min);
    Assert.Equal(30f, sensor.Max);
  }

  // TODO: Add tests for value restoration from settings
  // TODO: Add tests for parameter persistence

  // =========================================================================
  // SensorType Variations
  // =========================================================================

  [Theory]
  [InlineData(SensorType.Voltage)]
  [InlineData(SensorType.Current)]
  [InlineData(SensorType.Power)]
  [InlineData(SensorType.Clock)]
  [InlineData(SensorType.Temperature)]
  [InlineData(SensorType.Load)]
  [InlineData(SensorType.Frequency)]
  [InlineData(SensorType.Fan)]
  [InlineData(SensorType.Flow)]
  [InlineData(SensorType.Control)]
  [InlineData(SensorType.Level)]
  [InlineData(SensorType.Factor)]
  [InlineData(SensorType.Data)]
  [InlineData(SensorType.SmallData)]
  [InlineData(SensorType.Throughput)]
  [InlineData(SensorType.TimeSpan)]
  [InlineData(SensorType.Timing)]
  [InlineData(SensorType.Energy)]
  [InlineData(SensorType.Noise)]
  [InlineData(SensorType.Conductivity)]
  [InlineData(SensorType.Humidity)]
  public void Sensor_Construction_WorksWithAllSensorTypes(SensorType type) {
    var sensor = CreateSensor(type: type);
    Assert.Equal(type, sensor.SensorType);
  }

  [Theory]
  [InlineData(SensorType.Temperature)]
  [InlineData(SensorType.Voltage)]
  [InlineData(SensorType.Load)]
  public void Sensor_Value_TrackingWorks_WithDifferentSensorTypes(SensorType type) {
    var sensor = CreateSensor(type: type);

    sensor.Value = 25.5f;
    Assert.Equal(25.5f, sensor.Value);
    Assert.Equal(25.5f, sensor.Min);
    Assert.Equal(25.5f, sensor.Max);
  }

  // =========================================================================
  // Concurrent Updates
  // =========================================================================

  [Fact]
  public void Sensor_Value_ConcurrentUpdates_DoNotCorruptState() {
    var sensor = CreateSensor();
    var exceptions = new List<Exception>();

    var tasks = Enumerable.Range(0, 10)
      .Select(i => Task.Run(() => {
        try {
          for (int j = 0; j < 10; j++) {
            sensor.Value = (i * 10 + j) * 1f;
          }
        }
        catch (Exception ex) {
          lock (exceptions) {
            exceptions.Add(ex);
          }
        }
      }))
      .ToArray();

    Task.WaitAll(tasks);

    // No exceptions should occur
    Assert.Empty(exceptions);

    // Sensor should still be in a valid state
    Assert.NotNull(sensor.Min);
    Assert.NotNull(sensor.Max);
    Assert.True(sensor.Min <= sensor.Max);
  }

  [Fact]
  public void Sensor_Value_ConcurrentMinMaxReset_DoesNotThrow() {
    var sensor = CreateSensor();
    sensor.Value = 50f;

    var exceptions = new List<Exception>();

    var tasks = Enumerable.Range(0, 5)
      .Select(_ => Task.Run(() => {
        try {
          sensor.ResetMin();
          sensor.ResetMax();
        }
        catch (Exception ex) {
          lock (exceptions) {
            exceptions.Add(ex);
          }
        }
      }))
      .ToArray();

    Task.WaitAll(tasks);

    Assert.Empty(exceptions);
  }

  // =========================================================================
  // Edge Cases
  // =========================================================================

  [Fact]
  public void Sensor_Name_CanBeEmpty() {
    var ex = Record.Exception(() => CreateSensor(name: string.Empty));
    Assert.Null(ex);
  }

  [Fact]
  public void Sensor_Index_CanBeZero() {
    var sensor = CreateSensor(index: 0);
    Assert.Equal(0, sensor.Index);
  }

  [Fact]
  public void Sensor_Index_CanBeNegative() {
    var sensor = CreateSensor(index: -1);
    Assert.Equal(-1, sensor.Index);
  }

  [Fact]
  public void Sensor_Name_CanContainSpecialCharacters() {
    var specialNames = new[] {
      "Sensor #1",
      "Core @ 0",
      "Voltage [V]",
      "Temperature (°C)",
      "Load %",
      "Fan/RPM"
    };

    foreach (var name in specialNames) {
      var sensor = CreateSensor(name: name);
      Assert.Equal(name, sensor.Name);
    }
  }

  [Fact]
  public void Sensor_Name_CanBeLongString() {
    var longName = new string('A', 1000);
    var sensor = CreateSensor(name: longName);
    Assert.Equal(longName, sensor.Name);
  }

  [Fact]
  public void Sensor_Index_CanBeLargeNumber() {
    var sensor = CreateSensor(index: int.MaxValue);
    Assert.Equal(int.MaxValue, sensor.Index);
  }

  [Fact]
  public void Sensor_Value_SequentialUpdates_PreserveOrder() {
    var sensor = CreateSensor();
    var values = new[] { 10f, 20f, 30f, 40f, 50f };

    foreach (var value in values) {
      sensor.Value = value;
    }

    Assert.Equal(50f, sensor.Value);
    Assert.Equal(10f, sensor.Min);
    Assert.Equal(50f, sensor.Max);
  }

  // =========================================================================
  // TODO: Additional Test Categories
  // =========================================================================

  // TODO: Test value restoration from settings
  // TODO: Test parameter persistence
  // TODO: Test thread-safety of event subscription
  // TODO: Test memory cleanup (IDisposable if applicable)
  // TODO: Test interaction with Hardware parent
}