using Xunit;
using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class IntelCpuTests : IDisposable {
  private const string SkipIfNotIntel = "Skipping: No Intel CPU detected.";
  private const string SkipIfUnix = "Skipping: MSR reads not supported on Unix.";

  private readonly Computer _computer;
  private readonly IHardware? _intelCpu;
  private readonly bool _isIntel;
  private readonly bool _isUnix;

  public IntelCpuTests() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    _isUnix = CrystalMonitor.Software.OperatingSystem.IsUnix;

    _intelCpu = _computer.Hardware
      .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu
                        && h.Name.Contains("Intel", StringComparison.OrdinalIgnoreCase));

    _isIntel = _intelCpu != null;
  }

  public void Dispose() {
    _computer.Close();
  }

  // -------------------------------------------------------------------------
  // Hardware Detection
  // -------------------------------------------------------------------------

  [Fact]
  public void IntelCpu_WhenPresent_IsDetectedAsCpuHardwareType() {
    if (!_isIntel) return; // skip gracefully

    Assert.Equal(HardwareType.Cpu, _intelCpu!.HardwareType);
  }

  [Fact]
  public void IntelCpu_WhenPresent_HasNonEmptyName() {
    if (!_isIntel) return;

    Assert.False(string.IsNullOrWhiteSpace(_intelCpu!.Name),
      "Intel CPU name should not be null or empty.");
  }

  // -------------------------------------------------------------------------
  // Update
  // -------------------------------------------------------------------------

  [Fact]
  public void IntelCpu_Update_DoesNotThrow() {
    if (!_isIntel) return;

    var ex = Record.Exception(() => _intelCpu!.Update());
    Assert.Null(ex);
  }

  [Fact]
  public void IntelCpu_MultipleUpdates_DoesNotThrow() {
    if (!_isIntel) return;

    var ex = Record.Exception(() => {
      for (int i = 0; i < 10; i++)
        _intelCpu!.Update();
    });

    Assert.Null(ex);
  }

  [Fact]
  public async Task IntelCpu_ConcurrentUpdates_IsThreadSafe() {
    if (!_isIntel) return;

    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[10];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 20; j++)
            _intelCpu!.Update();
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
  public void IntelCpu_AfterUpdate_HasAtLeastOneSensor() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    Assert.True(_intelCpu.Sensors.Length > 0, "Intel CPU should expose at least one sensor.");
  }

  [Fact]
  public void IntelCpu_AfterUpdate_HasClockSensors() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    var clocks = _intelCpu.Sensors.Where(s => s.SensorType == SensorType.Clock).ToList();
    Assert.True(clocks.Count > 0, "Intel CPU should have at least one Clock sensor.");
  }

  [Fact]
  public void IntelCpu_AfterUpdate_HasLoadSensors() {
    if (!_isIntel) return;

    _intelCpu!.Update();
    var loads = _intelCpu.Sensors.Where(s => s.SensorType == SensorType.Load).ToList();
    Assert.True(loads.Count > 0, "Intel CPU should have at least one Load sensor.");
  }

  // -------------------------------------------------------------------------
  // Sensors — values
  // -------------------------------------------------------------------------

  [Fact]
  public void IntelCpu_ClockSensors_HavePositiveValues() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    var clocks = _intelCpu.Sensors
      .Where(s => s.SensorType == SensorType.Clock && s.Value.HasValue)
      .ToList();

    Assert.All(clocks, s =>
      Assert.True(s.Value!.Value > 0,
        $"Clock sensor '{s.Name}' should have a positive value, got {s.Value}."));
  }

  [Fact]
  public void IntelCpu_LoadSensors_AreWithinValidRange() {
    if (!_isIntel) return;

    _intelCpu!.Update();
    var loads = _intelCpu.Sensors
      .Where(s => s.SensorType == SensorType.Load && s.Value.HasValue)
      .ToList();

    Assert.All(loads, s =>
      Assert.True(s.Value!.Value is >= 0 and <= 100,
        $"Load sensor '{s.Name}' value {s.Value} is outside [0, 100]."));
  }

  [Fact]
  public void IntelCpu_TemperatureSensors_AreWithinPlausibleRange() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    var temps = _intelCpu.Sensors
      .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
      .ToList();

    if (temps.Count == 0) return; // no thermal sensor on this machine, skip

    Assert.All(temps, s =>
      Assert.True(s.Value!.Value is > 0 and < 150,
        $"Temperature sensor '{s.Name}' value {s.Value}°C is implausible."));
  }

  [Fact]
  public void IntelCpu_PowerSensors_HaveNonNegativeValues() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    var powers = _intelCpu.Sensors
      .Where(s => s.SensorType == SensorType.Power && s.Value.HasValue)
      .ToList();

    if (powers.Count == 0) return; // RAPL not available, skip

    Assert.All(powers, s =>
      Assert.True(s.Value!.Value >= 0,
        $"Power sensor '{s.Name}' should not be negative, got {s.Value}."));
  }

  [Fact]
  public void IntelCpu_VoltageSensors_AreWithinPlausibleRange() {
    if (!_isIntel || _isUnix) return;

    _intelCpu!.Update();
    var voltages = _intelCpu.Sensors
      .Where(s => s.SensorType == SensorType.Voltage && s.Value.HasValue)
      .ToList();

    if (voltages.Count == 0) return; // VID not readable, skip

    Assert.All(voltages, s =>
      Assert.True(s.Value!.Value is >= 0 and <= 4,
        $"Voltage sensor '{s.Name}' value {s.Value}V is implausible."));
  }

  // -------------------------------------------------------------------------
  // GetReport
  // -------------------------------------------------------------------------

  [Fact]
  public void IntelCpu_GetReport_ContainsMicroArchitecture() {
    if (!_isIntel) return;

    _intelCpu!.Update();
    string report = _intelCpu.GetReport();

    Assert.Contains("MicroArchitecture", report, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void IntelCpu_GetReport_ContainsTimeStampCounterMultiplier() {
    if (!_isIntel) return;

    string report = _intelCpu.GetReport();
    Assert.Contains("Time Stamp Counter Multiplier", report, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void IntelCpu_GetReport_IsNotNullOrEmpty() {
    if (!_isIntel) return;

    string report = _intelCpu!.GetReport();
    Assert.False(string.IsNullOrWhiteSpace(report), "GetReport() should return a non-empty string.");
  }
}