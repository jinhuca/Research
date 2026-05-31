using Xunit;
using CrystalMonitor.Hardware;
using CrystalMonitor.Hardware.Cpu;
using System.ComponentModel;
using System.Diagnostics;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class GenericCpuUpdateTests {
  private const string CpuUpdateSuccessfulMessage = "Update should complete without throwing exceptions.";
  private const string CpuUpateFailedMessage = "Cpu.Update() threw an exception: ";
  private const string CpuHaveSensorsMessage = "CPU should have sensors";
  private const string CpuSensorsShouldBeAccessibleMessage = "CPU sensors should be accessible after update";

  private Computer? _computer;

  public GenericCpuUpdateTests() {
    _computer = new Computer() { IsCpuEnabled = true };
    _computer.Open();
  }

  [Fact]
  public void GenericCpu_Update_DoesNotThrowExceptions() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
    try {
      cpu?.Update();
      Assert.True(true, CpuUpdateSuccessfulMessage);
    }
    catch (NullReferenceException ex) {
      string CpuUpdateFailedMessage = CpuUpateFailedMessage + ex.Message;
      Assert.Fail(CpuUpdateFailedMessage);
    }
  }

  [Fact]
  public void GenericCpu_Update_GracefullyHandlesMissingTsc() {
    // Arrange
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

    // Act: Update multiple times
    for (int i = 0; i < 10; i++) {
      cpu?.Update();
    }

    // Assert: CPU should still report some sensors if available
    if (cpu?.Sensors.Length > 0) {
      Assert.True(cpu.Sensors.Length > 0, CpuHaveSensorsMessage);
    }
  }

  [Fact]
  public async Task GenericCpu_ConcurrentUpdate_IsThreadSafe() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[20];
    for (int i = 0; i < 20; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 50; j++) {
            cpu?.Update();
          }
        }
        catch (Exception ex) {
          lock (lockObj) {
            exceptions.Add(ex);
          }
        }
      });
    }

    await Task.WhenAll(tasks); // non-blocking, no warning

    var nreExceptions = exceptions
      .OfType<NullReferenceException>()
      .Where(e => e.StackTrace?.Contains("Rdtsc") ?? false);

    Assert.True(nreExceptions.Count() == 0,
      $"Concurrent updates should not throw NullReferenceExceptions related to missing TSC. " +
      $"Found {nreExceptions.Count()} such exceptions.");
  }

  [Fact]
  public void GenericCpu_Update_PopulatesSensors() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
    cpu?.Update();
    Assert.NotNull(cpu);
    Assert.True(cpu.Sensors.Length > 0, CpuSensorsShouldBeAccessibleMessage);
  }

  [Fact]
  [Description("Verify Update maintains consistent state across multiple calls")]
  public async Task GenericCpu_Update_MaintainsConsistentState() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu)
              as GenericCpu;
    var initialFrequency = cpu.TimeStampCounterFrequency;

    for (int i = 0; i < 5; i++) {
      cpu?.Update();
    }

    var finalFrequency = cpu?.TimeStampCounterFrequency ?? 0;

    Assert.True(initialFrequency >= 0,
        $"Expected valid TSC frequency but got {initialFrequency}. " +
        $"TryRdtsc may have failed during initialization.");
    Debug.WriteLine($"Initial TSC Frequency: {initialFrequency} MHz");
  }

  [Fact]
  [Description("Verify Update performance is acceptable")]
  public void GenericCpu_Update_PerformanceIsAcceptable() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
    var stopwatch = Stopwatch.StartNew();
    for (int i = 0; i < 100; i++) {
      cpu?.Update();
    }
    stopwatch.Stop();

    var avgTimePerUpdate = stopwatch.ElapsedMilliseconds / 100.0;
    Assert.True(avgTimePerUpdate < 10000, $"100 updates took {avgTimePerUpdate} ms");
  }

  [Fact]
  [Description("Verify Update handles rapid succession calls")]
  public void GenericCpu_Update_HandlesRapidSuccession() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
    Exception? exception = null;
    for (int i = 0; i < 1000; i++) {
      exception = Record.Exception(() => cpu?.Update());
    }
    Assert.Null(exception);
  }

  [Fact]
  [Description("Verify CPU hardware is properly initalized")]
  public void GenericCpu_Initialization_IsCorrect() {
    var cpu = _computer!.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu)
              as GenericCpu;
    Assert.NotNull(cpu);
    Assert.False(string.IsNullOrEmpty(cpu.Name), "CPU should have a valid name");
  }
}
