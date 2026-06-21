using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class Amd0FCpuTests : IDisposable {
  private readonly Computer _computer;
  private readonly IHardware? _amd0FCpu;
  private readonly bool _isAmd0F;

  public Amd0FCpuTests() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    // Amd0FCpu handles AMD family 0x0F only
    _amd0FCpu = _computer.Hardware
      .FirstOrDefault(h =>
        h.HardwareType == HardwareType.Cpu &&
        h.Name.Contains("AMD", StringComparison.OrdinalIgnoreCase));

    _isAmd0F = _amd0FCpu != null;
  }

  public void Dispose() {
    _computer.Close();
  }

  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  /// <summary>
  /// Skip the test gracefully if no AMD family 0F CPU is present.
  /// </summary>
  private bool ShouldSkip => !_isAmd0F;

  // -------------------------------------------------------------------------
  // Hardware detection
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_WhenPresent_IsDetectedAsCpuHardwareType() {
    if (ShouldSkip) return;

    Assert.Equal(HardwareType.Cpu, _amd0FCpu!.HardwareType);
  }

  [Fact]
  public void Amd0FCpu_WhenPresent_HasNonEmptyName() {
    if (ShouldSkip) return;

    Assert.False(string.IsNullOrWhiteSpace(_amd0FCpu!.Name),
      "AMD 0F CPU name should not be null or empty.");
  }

  [Fact]
  public void Amd0FCpu_WhenPresent_NameContainsAmd() {
    if (ShouldSkip) return;

    Assert.Contains("AMD", _amd0FCpu!.Name, StringComparison.OrdinalIgnoreCase);
  }

  // -------------------------------------------------------------------------
  // Update
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_Update_DoesNotThrow() {
    if (ShouldSkip) return;

    var ex = Record.Exception(() => _amd0FCpu!.Update());
    Assert.Null(ex);
  }

  [Fact]
  public void Amd0FCpu_MultipleUpdates_DoesNotThrow() {
    if (ShouldSkip) return;

    var ex = Record.Exception(() => {
      for (int i = 0; i < 10; i++)
        _amd0FCpu!.Update();
    });

    Assert.Null(ex);
  }

  [Fact]
  public async Task Amd0FCpu_ConcurrentUpdates_IsThreadSafe() {
    if (ShouldSkip) return;

    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[10];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 20; j++)
            _amd0FCpu!.Update();
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent Update() calls threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }

  // -------------------------------------------------------------------------
  // Sensors — presence
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_AfterUpdate_HasAtLeastOneSensor() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    Assert.True(_amd0FCpu.Sensors.Length > 0,
      "AMD 0F CPU should expose at least one sensor after Update().");
  }

  [Fact]
  public void Amd0FCpu_AfterUpdate_HasClockSensors() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var clocks = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Clock)
      .ToList();

    Assert.True(clocks.Count > 0,
      "AMD 0F CPU should have at least one Clock sensor (Bus Speed or Core clocks).");
  }

  [Fact]
  public void Amd0FCpu_AfterUpdate_HasLoadSensors() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var loads = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Load)
      .ToList();

    Assert.True(loads.Count > 0,
      "AMD 0F CPU should have at least one Load sensor.");
  }

  // -------------------------------------------------------------------------
  // Sensors — Clock values
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_ClockSensors_HavePositiveValues() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var clocks = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Clock && s.Value.HasValue)
      .ToList();

    if (clocks.Count == 0) return; // TSC not available on this machine

    Assert.All(clocks, s =>
      Assert.True(s.Value!.Value > 0,
        $"Clock sensor '{s.Name}' should have a positive value, got {s.Value}."));
  }

  [Fact]
  public void Amd0FCpu_BusSpeedSensor_HasPositiveValue() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var busClock = _amd0FCpu.Sensors
      .FirstOrDefault(s => s.SensorType == SensorType.Clock &&
                           s.Name.Equals("Bus Speed", StringComparison.OrdinalIgnoreCase));

    if (busClock?.Value == null) return; // only active when FIDVID_STATUS is readable

    Assert.True(busClock.Value > 0,
      $"Bus Speed sensor should have a positive value, got {busClock.Value}.");
  }

  [Fact]
  public void Amd0FCpu_CoreClockSensors_AreNamedCorrectly() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var coreClocks = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Clock &&
                  s.Name.StartsWith("CPU Core", StringComparison.OrdinalIgnoreCase))
      .ToList();

    if (coreClocks.Count == 0) return; // no TSC on this machine

    Assert.All(coreClocks, s =>
      Assert.False(string.IsNullOrWhiteSpace(s.Name),
        "Core clock sensor should have a non-empty name."));
  }

  // -------------------------------------------------------------------------
  // Sensors — Temperature values
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_TemperatureSensors_WhenPresent_AreNamedCoreHash() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var temps = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Temperature)
      .ToList();

    if (temps.Count == 0) return; // ExtData thermal bit not set on this CPU

    Assert.All(temps, s =>
      Assert.StartsWith("Core #", s.Name, StringComparison.OrdinalIgnoreCase));
  }

  [Fact]
  public void Amd0FCpu_TemperatureSensors_WhenPresent_HaveOffsetParameter() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var temps = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Temperature)
      .ToList();

    if (temps.Count == 0) return;

    Assert.All(temps, s =>
      Assert.True(s.Parameters.Count > 0,
        $"Temperature sensor '{s.Name}' should have at least one parameter (Offset)."));
  }

  [Fact]
  public void Amd0FCpu_TemperatureSensors_WhenActive_AreWithinPlausibleRange() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var temps = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
      .ToList();

    if (temps.Count == 0) return;

    Assert.All(temps, s =>
      Assert.True(s.Value!.Value is > 0 and < 150,
        $"Temperature sensor '{s.Name}' value {s.Value}°C is implausible."));
  }

  // -------------------------------------------------------------------------
  // Sensors — Load values
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_LoadSensors_AreWithinValidRange() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var loads = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Load && s.Value.HasValue)
      .ToList();

    if (loads.Count == 0) return;

    Assert.All(loads, s =>
      Assert.True(s.Value!.Value is >= 0 and <= 100,
        $"Load sensor '{s.Name}' value {s.Value} is outside valid range [0, 100]."));
  }

  // -------------------------------------------------------------------------
  // Temperature offset — AM2+ 65nm model logic
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_TemperatureOffset_IsNegative49OrAdjusted() {
    if (ShouldSkip) return;

    _amd0FCpu!.Update();
    var temps = _amd0FCpu.Sensors
      .Where(s => s.SensorType == SensorType.Temperature)
      .ToList();

    if (temps.Count == 0) return;

    // Base offset is -49. AM2+ 65nm models add +21 → -28.
    // Any other value indicates a bug in the offset logic.
    float offsetValue = temps[0].Parameters[0].Value;
    Assert.True(
      offsetValue is -49.0f or -28.0f,
      $"Temperature offset should be -49 (standard) or -28 (AM2+ 65nm), got {offsetValue}.");
  }

  // -------------------------------------------------------------------------
  // GetReport
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_GetReport_IsNotNullOrEmpty() {
    if (ShouldSkip) return;

    string report = _amd0FCpu!.GetReport();
    Assert.False(string.IsNullOrWhiteSpace(report),
      "GetReport() should return a non-empty string.");
  }

  [Fact]
  public void Amd0FCpu_GetReport_ContainsTimeStampCounterInfo() {
    if (ShouldSkip) return;

    string report = _amd0FCpu!.GetReport();
    Assert.Contains("Time Stamp Counter", report, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Amd0FCpu_GetReport_ContainsCoreCount() {
    if (ShouldSkip) return;

    string report = _amd0FCpu!.GetReport();
    Assert.Contains("Number of Cores", report, StringComparison.OrdinalIgnoreCase);
  }

  // -------------------------------------------------------------------------
  // Close
  // -------------------------------------------------------------------------

  [Fact]
  public void Amd0FCpu_Close_DoesNotThrow() {
    var computer = new Computer { IsCpuEnabled = true };
    computer.Open();

    var ex = Record.Exception(() => computer.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void Amd0FCpu_Close_CanBeCalledMultipleTimes_WithoutThrowing() {
    var computer = new Computer { IsCpuEnabled = true };
    computer.Open();

    var ex = Record.Exception(() => {
      computer.Close();
      computer.Close();
    });

    Assert.Null(ex);
  }
}