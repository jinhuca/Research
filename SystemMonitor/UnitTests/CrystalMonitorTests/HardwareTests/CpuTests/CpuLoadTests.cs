using CrystalMonitor.Hardware.Cpu;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class CpuLoadTests {
  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  /// <summary>
  /// Gets real CpuId[][] from the machine for integration tests.
  /// Returns null if hardware access is unavailable.
  /// </summary>
  private static CpuId[][]? GetRealCpuId() {
    try {
      // Try to get thread 0 from group 0 — represents one physical core
      var thread = CpuId.Get(0, 0);
      if (thread == null) return null;

      // Wrap as a single-processor, single-core, single-thread topology
      return [[thread]];
    }
    catch (ArgumentOutOfRangeException) {
      return null;
    }
  }

  /// <summary>
  /// Builds a minimal CpuId[][] topology with the given thread count
  /// using real CpuId instances. Returns null if hardware unavailable.
  /// </summary>
  private static CpuId[][]? GetRealCpuIdWithThreads(int threadCount) {
    try {
      var threads = new List<CpuId>();
      for (int t = 0; t < threadCount; t++) {
        var cpuId = CpuId.Get(0, t);
        if (cpuId == null) break;
        threads.Add(cpuId);
      }

      if (threads.Count == 0) return null;

      // Each thread gets its own "core" slot: [[t0], [t1], ...]
      return threads.Select(t => new[] { t }).ToArray();
    }
    catch (ArgumentOutOfRangeException) {
      return null;
    }
  }

  // -------------------------------------------------------------------------
  // Construction — IsAvailable
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_IsAvailable_IsTrueOnPhysicalMachine() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Assert.True(cpuLoad.IsAvailable,
      "CpuLoad.IsAvailable should be true when OS timing APIs are accessible.");
  }

  [Fact]
  public void CpuLoad_Construction_DoesNotThrow() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var ex = Record.Exception(() => new CpuLoad(cpuId));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Initial state before first Update()
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_GetTotalLoad_ReturnsZero_BeforeFirstUpdate() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Assert.Equal(0.0, cpuLoad.GetTotalLoad());
  }

  [Fact]
  public void CpuLoad_GetThreadLoad_ReturnsZero_BeforeFirstUpdate() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Assert.Equal(0.0, cpuLoad.GetThreadLoad(0));
  }

  // -------------------------------------------------------------------------
  // Thread count — GetThreadLoad bounds
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_GetThreadLoad_SupportsAllThreadIndices() {
    var cpuId = GetRealCpuIdWithThreads(4);
    if (cpuId == null) return;

    int totalThreads = cpuId.Sum(core => core.Length);
    var cpuLoad = new CpuLoad(cpuId);

    for (int i = 0; i < totalThreads; i++) {
      var ex = Record.Exception(() => cpuLoad.GetThreadLoad(i));
      Assert.Null(ex);
    }
  }

  [Fact]
  public void CpuLoad_ThreadLoadArray_SizeMatchesTotalThreadCount() {
    var cpuId = GetRealCpuIdWithThreads(2);
    if (cpuId == null) return;

    int expected = cpuId.Sum(core => core.Length);
    var cpuLoad = new CpuLoad(cpuId);

    // _threadLoads has exactly Sum(threads) entries — verify via valid access
    var ex = Record.Exception(() => cpuLoad.GetThreadLoad(expected - 1));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Update — does not throw
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_Update_DoesNotThrow() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    var ex = Record.Exception(() => cpuLoad.Update());
    Assert.Null(ex);
  }

  [Fact]
  public void CpuLoad_MultipleUpdates_DoNotThrow() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    var ex = Record.Exception(() => {
      for (int i = 0; i < 10; i++) {
        Thread.Sleep(50); // allow time delta to exceed minDiff threshold
        cpuLoad.Update();
      }
    });

    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Update — value ranges after updating
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_GetTotalLoad_IsWithinValidRange_AfterUpdate() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200); // ensure time delta exceeds minDiff (100000 on Windows)
    cpuLoad.Update();

    double total = cpuLoad.GetTotalLoad();
    Assert.True(total is >= 0.0 and <= 100.0,
      $"Total load {total} is outside valid range [0, 100].");
  }

  [Fact]
  public void CpuLoad_GetThreadLoad_IsWithinValidRange_AfterUpdate() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update();

    double load = cpuLoad.GetThreadLoad(0);
    Assert.True(load is >= 0.0 and <= 100.0,
      $"Thread 0 load {load} is outside valid range [0, 100].");
  }

  [Fact]
  public void CpuLoad_AllThreadLoads_AreWithinValidRange_AfterUpdate() {
    var cpuId = GetRealCpuIdWithThreads(4);
    if (cpuId == null) return;

    int totalThreads = cpuId.Sum(core => core.Length);
    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update();

    for (int i = 0; i < totalThreads; i++) {
      double load = cpuLoad.GetThreadLoad(i);
      Assert.True(load is >= 0.0 and <= 100.0,
        $"Thread {i} load {load} is outside valid range [0, 100].");
    }
  }

  [Fact]
  public void CpuLoad_GetTotalLoad_IsRoundedToTwoDecimalPlaces() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update();

    double total = cpuLoad.GetTotalLoad();
    double rounded = Math.Round(total, 2);
    Assert.True(rounded == total,
      $"TotalLoad {total} should be rounded to 2 decimal places.");
  }

  [Fact]
  public void CpuLoad_GetThreadLoad_IsRoundedToTwoDecimalPlaces() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update();

    double load = cpuLoad.GetThreadLoad(0);
    double rounded = Math.Round(load, 2);
    Assert.True(rounded == load,
      $"ThreadLoad {load} should be rounded to 2 decimal places.");
  }

  // -------------------------------------------------------------------------
  // Update — idempotent when time delta is too small
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuLoad_Update_DoesNotChangePreviousValues_WhenTimeDeltaTooSmall() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update(); // first real update

    double totalAfterFirst = cpuLoad.GetTotalLoad();
    double threadAfterFirst = cpuLoad.GetThreadLoad(0);

    // Immediately call Update() again — delta will be below minDiff, so
    // values must remain unchanged
    cpuLoad.Update();

    Assert.Equal(totalAfterFirst, cpuLoad.GetTotalLoad());
    Assert.Equal(threadAfterFirst, cpuLoad.GetThreadLoad(0));
  }

  // -------------------------------------------------------------------------
  // Concurrent access
  // -------------------------------------------------------------------------

  [Fact]
  public async Task CpuLoad_ConcurrentUpdates_DoNotThrow() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[8];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 10; j++) {
            Thread.Sleep(20);
            cpuLoad.Update();
          }
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

  [Fact]
  public async Task CpuLoad_ConcurrentReads_DoNotThrow() {
    var cpuId = GetRealCpuId();
    if (cpuId == null) return;

    var cpuLoad = new CpuLoad(cpuId);
    Thread.Sleep(200);
    cpuLoad.Update();

    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[8];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 50; j++) {
            _ = cpuLoad.GetTotalLoad();
            _ = cpuLoad.GetThreadLoad(0);
          }
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent reads threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }
}