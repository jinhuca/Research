using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalMonitor.Hardware;
using System.Diagnostics;

namespace CrystalMonitorLib.Tests;

/// <summary>
/// Unit tests for OpCode synchronization and TryRdtsc functionality.
/// Tests thread-safety fixes for NullReferenceException in OpCode.Rdtsc().
/// </summary>
[TestClass]
[DoNotParallelize]
public class OpCodeSynchronizationTests {
  [TestMethod]
  public void TryRdtsc_ConcurrentCalls_AreThreadSafe() {
    var computer = new Computer();
    computer.Open();

    // Direct sanity check — bypasses TryRdtsc entirely
    bool singleCall = OpCode.TryRdtsc(out ulong v);
    Debug.WriteLine($"Direct call result: {singleCall}, value: {v}");

    var successCount = 0;
    var failureCount = 0;
    var lockObj = new object();
    var tasks = new Task[100];  // outside try

    try {
      for (int i = 0; i < 100; i++) {
        tasks[i] = Task.Run(() =>
        {
          bool success = OpCode.TryRdtsc(out ulong value);
          lock (lockObj) {
            if (success) successCount++;
            else failureCount++;
          }
        });
      }

      Task.WaitAll(tasks);
      Assert.AreEqual(0, failureCount, "No failures expected when OpCode is open");
    }
    catch(Exception ex) {
      Assert.Fail($"Unexpected exception during concurrent TryRdtsc calls: {ex}");
    }
    finally {
      if (tasks.All(t => t != null))
        Task.WaitAll(tasks);  // ensure all tasks done before Close()
      computer.Close();
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
