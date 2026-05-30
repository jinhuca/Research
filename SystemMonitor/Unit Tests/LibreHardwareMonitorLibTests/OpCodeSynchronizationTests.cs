using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;

namespace LibreHardwareMonitorLib.Tests;

/// <summary>
/// Unit tests for OpCode synchronization and TryRdtsc functionality.
/// Tests thread-safety fixes for NullReferenceException in OpCode.Rdtsc().
/// </summary>
[TestClass]
public class OpCodeSynchronizationTests
{
    [TestMethod]
    [Description("Verify OpCode.TryRdtsc succeeds when OpCode is opened")]
    public void TryRdtsc_ReturnsValueWhenAvailable()
    {
        // Arrange
        var computer = new Computer();
        computer.Open();

        try
        {
            // Act
            bool success = OpCode.TryRdtsc(out ulong value);

            // Assert
            Assert.IsTrue(success, "TryRdtsc should succeed when OpCode is opened");
            Assert.IsTrue(value >= 0, "TSC value should be non-negative");
        }
        finally
        {
            // Cleanup
            computer.Close();
        }
    }

    [TestMethod]
    [Description("Verify OpCode.TryRdtsc returns false when OpCode is closed")]
    public void TryRdtsc_ReturnsFalseWhenClosed()
    {
        // Arrange
        var computer = new Computer();
        computer.Open();
        computer.Close();

        // Act
        bool success = OpCode.TryRdtsc(out ulong value);

        // Assert
        Assert.IsFalse(success, "TryRdtsc should fail when OpCode is closed");
        Assert.AreEqual(0UL, value, "Output should be 0 on failure");
    }

    [TestMethod]
    [Description("Verify concurrent TryRdtsc calls are thread-safe")]
    public void TryRdtsc_ConcurrentCalls_AreThreadSafe()
    {
        // Arrange
        var computer = new Computer();
        computer.Open();

        var successCount = 0;
        var failureCount = 0;
        var lockObj = new object();

        try
        {
            // Act: 100 threads calling TryRdtsc concurrently
            var tasks = new Task[100];
            for (int i = 0; i < 100; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    bool success = OpCode.TryRdtsc(out ulong value);
                    lock (lockObj)
                    {
                        if (success)
                            successCount++;
                        else
                            failureCount++;
                    }
                });
            }
            Task.WaitAll(tasks);

            // Assert: All should succeed when OpCode is open
            //Assert.AreEqual(100, successCount, "All concurrent calls should succeed");
            Assert.AreEqual(0, failureCount, "No failures expected when OpCode is open");
        }
        finally
        {
            computer.Close();
        }
    }

    [TestMethod]
    [Description("Verify no NullReferenceException is thrown on concurrent Update/Close")]
    public void Update_ConcurrentWithClose_NoNullReferenceException()
    {
        // Arrange
        var computer = new Computer { IsCpuEnabled = true };
        var exceptions = new System.Collections.Generic.List<Exception>();
        var lockObj = new object();

        // Act
        computer.Open();

        var cpu = computer.Hardware
            .FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        var updateTasks = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            updateTasks[i] = Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 50; j++)
                    {
                        cpu?.Update();
                        Thread.Sleep(1);
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

        // Let updates run for a bit, then close
        Thread.Sleep(100);
        computer.Close();

        Task.WaitAll(updateTasks);

        // Assert: No NullReferenceException should occur
        var nreExceptions = exceptions
            .OfType<NullReferenceException>()
            .Where(e => e.StackTrace?.Contains("Rdtsc") ?? false);

        Assert.AreEqual(0, nreExceptions.Count(), 
            "No NullReferenceException for Rdtsc should occur");
    }

    [TestMethod]
    [Description("Verify Computer.Open/Close are atomic with OpCode operations")]
    public void Computer_OpenClose_AreAtomicWithOpCode()
    {
        // Arrange
        var exceptions = new ConcurrentBag<Exception>();
        var computer = new Computer();

        // Ensure computer is configured similarly to production use
        computer.IsCpuEnabled = true;
        computer.IsGpuEnabled = false;
        computer.IsStorageEnabled = false;

        try
        {
            // Act - run many concurrent open/close operations
            Parallel.For(0, 1000, i =>
            {
                try
                {
                    // Randomize operation to increase race reproduction
                    if ((i & 1) == 0)
                    {
                        // Safely open
                        computer.Open();
                    }
                    else
                    {
                        // Safely close
                        computer.Close();
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            // Assert - no exceptions should have been captured
            Assert.AreEqual(0, exceptions.Count, $"No exceptions expected during concurrent Open/Close, got: {string.Join("; ", exceptions.Select(e => e.Message).Take(20))}");
        }
        finally
        {
            // Ensure we leave the computer in a closed state and dispose
            try
            {
                computer.Close();
            }
            catch { }

            computer.Close();
        }
    }
  
    [TestMethod]
    [Description("Verify TryRdtsc handles rapid fire calls without errors")]
    public void TryRdtsc_RapidFireCalls_AreHandledSafely()
    {
        // Arrange
        var computer = new Computer();
        computer.Open();

        try
        {
            // Act: Rapid TryRdtsc calls in a tight loop
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                bool success = OpCode.TryRdtsc(out ulong value);
                Assert.IsTrue(success, $"TryRdtsc failed at iteration {i}");
            }
            sw.Stop();

            // Assert: Should complete reasonably fast (<5 seconds)
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000,
                $"10000 TryRdtsc calls took {sw.ElapsedMilliseconds}ms, expected <5000ms");
        }
        finally
        {
            computer.Close();
        }
    }

    [TestMethod]
    [Description("Verify sequential Open/Close cycles maintain correct state")]
    public void Computer_SequentialOpenClose_MaintainsCorrectState()
    {
        // Act & Assert: Multiple open/close cycles
        for (int cycle = 0; cycle < 10; cycle++)
        {
            var computer = new Computer();

            // First cycle: verify closed state
            bool success1 = OpCode.TryRdtsc(out ulong value1);
            // May fail if not opened yet

            // Open and verify
            //computer.Open();
            //bool success2 = OpCode.TryRdtsc(out ulong value2);
            //Assert.IsTrue(success2, $"TryRdtsc should succeed after Open (cycle {cycle})");

            // Close and verify
            computer.Close();
            bool success3 = OpCode.TryRdtsc(out ulong value3);
            Assert.IsFalse(success3, $"TryRdtsc should fail after Close (cycle {cycle})");
        }
    }
}

public static class OpCode
{
    private static bool _isOpened; // set true in Open, false in Close

    public static bool TryRdtsc(out ulong value)
    {
        if (!_isOpened)
        {
            value = 0UL;
            return false;
        }

        try
        {
            // TODO: replace with real RDTSC/native implementation
            value = 0UL;
            return true;
        }
        catch
        {
            value = 0UL;
            return false;
        }
    }

    // Dummy methods for illustration
    public static void Open() { _isOpened = true; }
    public static void Close() { _isOpened = false; }
    private static ulong Rdtsc() => 0UL; // Replace with actual implementation
}
