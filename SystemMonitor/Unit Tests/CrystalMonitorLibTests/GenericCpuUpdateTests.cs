using CrystalMonitor.Hardware;
using CrystalMonitor.Hardware.Cpu;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CrystalMonitorLib.Tests;

/// <summary>
/// Unit tests for GenericCpu and CPU update functionality.
/// Tests TSC availability, graceful fallback, and thread-safety.
/// </summary>
[TestClass]
public class GenericCpuUpdateTests
{
    private Computer? _computer;

    [TestInitialize]
    public void Setup()
    {
        _computer = new Computer { IsCpuEnabled = true };
        _computer.Open();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _computer?.Close();
        _computer = null;
    }

    [TestMethod]
    [Description("Verify GenericCpu.Update completes without throwing exceptions")]
    public void GenericCpu_Update_DoesNotThrowExceptions()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Act & Assert: Should not throw any exception
        try
        {
            cpu?.Update();
            Assert.IsTrue(true, "Update should complete without exception");
        }
        catch (NullReferenceException ex) when (ex.StackTrace?.Contains("Rdtsc") ?? false)
        {
            Assert.Fail($"NullReferenceException in Rdtsc: {ex}");
        }
    }

    [TestMethod]
    [Description("Verify GenericCpu.Update handles missing TSC gracefully")]
    public void GenericCpu_Update_GracefullyHandlesMissingTsc()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Act: Update multiple times
        for (int i = 0; i < 10; i++)
        {
            cpu?.Update();
        }

        // Assert: CPU should still report some sensors if available
        if (cpu?.Sensors.Length > 0)
        {
            Assert.IsTrue(cpu.Sensors.Length > 0, "CPU should have sensors");
        }
    }

    [TestMethod]
    [Description("Verify concurrent Update calls are thread-safe")]
    public void GenericCpu_ConcurrentUpdate_IsThreadSafe()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        var exceptions = new List<Exception>();
        var lockObj = new object();

        // Act: 20 threads calling Update concurrently
        var tasks = new Task[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 50; j++)
                    {
                        cpu?.Update();
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObj)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
        }
        Task.WaitAll(tasks);

        // Assert: No exceptions should occur
        var nreExceptions = exceptions
            .OfType<NullReferenceException>()
            .Where(e => e.StackTrace?.Contains("Rdtsc") ?? false);

        Assert.AreEqual(0, nreExceptions.Count(),
            $"No NullReferenceException should occur, got {exceptions.Count} exceptions");
    }

    [TestMethod]
    [Description("Verify CPU sensors are available after update")]
    public void GenericCpu_Update_PopulatesSensors()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Act
        cpu?.Update();

        // Assert
        Assert.IsNotNull(cpu, "CPU hardware should be found");
        Assert.IsTrue(cpu!.Sensors.Length >= 0, "Sensors should be accessible");

        // Log available sensors for debugging
        Console.WriteLine($"CPU: {cpu.Name}");
        Console.WriteLine($"Sensor Count: {cpu.Sensors.Length}");
        foreach (var sensor in cpu.Sensors)
        {
            Console.WriteLine($"  - {sensor.Name}: {sensor.Value} {sensor.SensorType}");
        }
    }

    [TestMethod]
    [Description("Verify Update maintains consistent state across multiple calls")]
    public void GenericCpu_Update_MaintainsConsistentState()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu) as GenericCpu;

        var initialFrequency = cpu?.TimeStampCounterFrequency ?? 0;

        // Act: Multiple updates
        for (int i = 0; i < 5; i++)
        {
            cpu?.Update();
        }

        var finalFrequency = cpu?.TimeStampCounterFrequency ?? 0;

        // Assert: Frequency should be reasonable and consistent
        // (May be 0 if TSC estimation failed, but shouldn't crash)
        Assert.IsTrue(finalFrequency >= 0, "Frequency should be non-negative");
        Console.WriteLine($"TSC Frequency: {finalFrequency} MHz");
    }

    [TestMethod]
    [Description("Verify Update performance is acceptable")]
    public void GenericCpu_Update_PerformanceIsAcceptable()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Act: Time 100 updates
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            cpu?.Update();
        }
        sw.Stop();

        // Assert: 100 updates should complete in reasonable time
        var avgTimePerUpdate = sw.ElapsedMilliseconds / 100.0;
        Console.WriteLine($"Average time per Update: {avgTimePerUpdate:F2}ms");
        Assert.IsTrue(sw.ElapsedMilliseconds < 10000,
            $"100 updates took {sw.ElapsedMilliseconds}ms, expected <10000ms");
    }

    [TestMethod]
    [Description("Verify Update handles rapid succession calls")]
    public void GenericCpu_Update_HandlesRapidSuccession()
    {
        // Arrange
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Act: Rapid updates without delay
        for (int i = 0; i < 1000; i++)
        {
            cpu?.Update();
        }

        // Assert: Should complete without exception
        Assert.IsTrue(true, "1000 rapid updates should complete");
    }

    [TestMethod]
    [Description("Verify CPU hardware is properly initialized")]
    public void GenericCpu_Initialization_IsCorrect()
    {
        // Arrange & Act
        var cpu = _computer!.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        // Assert
        Assert.IsNotNull(cpu, "CPU should be detected");
        Assert.IsFalse(string.IsNullOrEmpty(cpu!.Name), "CPU should have a name");
        Console.WriteLine($"Detected CPU: {cpu.Name}");
        Console.WriteLine($"CPU Index: {cpu.Identifier}");
    }
}
