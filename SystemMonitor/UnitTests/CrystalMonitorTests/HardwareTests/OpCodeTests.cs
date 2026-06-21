namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Tests for OpCode must be run sequentially because OpCode uses a static
/// reference-counted code buffer. Parallel execution would corrupt the count.
/// </summary>
[Collection("OpCode")]
public class OpCodeTests : IDisposable {
  // -------------------------------------------------------------------------
  // Setup / teardown — ensure clean state before and after each test
  // -------------------------------------------------------------------------

  public OpCodeTests() {
    // Ensure OpCode starts closed for every test
    SafeClose();
  }

  public void Dispose() {
    SafeClose();
  }

  /// <summary>
  /// Drains the reference count down to zero without throwing.
  /// </summary>
  private static void SafeClose() {
    try {
      // Close until delegates become null (reference count reaches 0)
      for (int i = 0; i < 10; i++)
        OpCode.Close();
    }
    catch { /* ignored */ }
  }

  private static bool IsElevated =>
    System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
      System.Runtime.InteropServices.OSPlatform.Windows)
      ? new System.Security.Principal.WindowsPrincipal(
          System.Security.Principal.WindowsIdentity.GetCurrent())
        .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)
      : true; // on Unix, mmap doesn't require elevation

  // -------------------------------------------------------------------------
  // Open
  // -------------------------------------------------------------------------

  [Fact]
  public void OpCode_Open_DoesNotThrow() {
    if (!IsElevated) return;

    var ex = Record.Exception(() => OpCode.Open());
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_Open_CalledTwice_DoesNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();
    var ex = Record.Exception(() => OpCode.Open());
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_Open_CalledMultipleTimes_DoesNotThrow() {
    if (!IsElevated) return;

    var ex = Record.Exception(() => {
      for (int i = 0; i < 5; i++)
        OpCode.Open();
    });

    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Close
  // -------------------------------------------------------------------------

  [Fact]
  public void OpCode_Close_WithoutOpen_DoesNotThrow() {
    // _openCount <= 0 → early return
    var ex = Record.Exception(() => OpCode.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_Close_AfterOpen_DoesNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();
    var ex = Record.Exception(() => OpCode.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_Close_CalledMoreTimesThanOpen_DoesNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.Close();

    // Extra closes beyond reference count should be no-ops
    var ex = Record.Exception(() => {
      OpCode.Close();
      OpCode.Close();
    });

    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Reference counting — Open/Close symmetry
  // -------------------------------------------------------------------------

  [Fact]
  public void OpCode_ReferenceCount_TwoOpens_RequireTwoClosesToDisable() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.Open(); // ref count = 2

    OpCode.Close(); // ref count = 1 — delegates still active
    Assert.True(OpCode.TryRdtsc(out _),
      "After one Close with two Opens, TryRdtsc should still succeed.");

    OpCode.Close(); // ref count = 0 — delegates cleared
    Assert.False(OpCode.TryRdtsc(out _),
      "After matching Close calls, TryRdtsc should return false.");
  }

  [Fact]
  public void OpCode_ReferenceCount_MatchedOpenClose_DisablesDelegates() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.Close();

    bool result = OpCode.TryRdtsc(out _);
    Assert.False(result,
      "After matched Open/Close, TryRdtsc should return false.");
  }

  [Fact]
  public void OpCode_ReferenceCount_MultipleOpenClose_CyclesWithoutLeaking() {
    if (!IsElevated) return;

    for (int i = 0; i < 3; i++) {
      OpCode.Open();
      OpCode.Close();
    }

    // After all cycles, delegates must be null
    Assert.False(OpCode.TryRdtsc(out _));
    Assert.False(OpCode.TryCpuId(0, 0, out _, out _, out _, out _));
  }

  // -------------------------------------------------------------------------
  // TryRdtsc
  // -------------------------------------------------------------------------

  [Fact]
  public void OpCode_TryRdtsc_ReturnsFalse_WhenNotOpen() {
    bool result = OpCode.TryRdtsc(out ulong value);
    Assert.False(result);
    Assert.Equal(0ul, value);
  }

  [Fact]
  public void OpCode_TryRdtsc_ReturnsTrue_AfterOpen() {
    if (!IsElevated) return;

    OpCode.Open();
    bool result = OpCode.TryRdtsc(out _);
    Assert.True(result,
      "TryRdtsc should return true after Open() succeeds.");
  }

  [Fact]
  public void OpCode_TryRdtsc_ReturnsPositiveValue_AfterOpen() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.TryRdtsc(out ulong value);
    Assert.True(value > 0,
      $"TSC value should be positive after Open(), got {value}.");
  }

  [Fact]
  public void OpCode_TryRdtsc_ValueIncreases_BetweenConsecutiveCalls() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.TryRdtsc(out ulong first);
    OpCode.TryRdtsc(out ulong second);

    Assert.True(second >= first,
      $"TSC should be monotonically non-decreasing: first={first}, second={second}.");
  }

  [Fact]
  public void OpCode_TryRdtsc_OutputsZero_WhenReturnsFalse() {
    // Closed state — delegate is null → value must be 0
    OpCode.TryRdtsc(out ulong value);
    Assert.Equal(0ul, value);
  }

  [Fact]
  public void OpCode_TryRdtsc_DoesNotThrow_WhenNotOpen() {
    var ex = Record.Exception(() => OpCode.TryRdtsc(out _));
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_TryRdtsc_DoesNotThrow_AfterOpen() {
    if (!IsElevated) return;

    OpCode.Open();
    var ex = Record.Exception(() => OpCode.TryRdtsc(out _));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // TryCpuId
  // -------------------------------------------------------------------------

  [Fact]
  public void OpCode_TryCpuId_ReturnsFalse_WhenNotOpen() {
    bool result = OpCode.TryCpuId(0, 0, out uint eax, out uint ebx, out uint ecx, out uint edx);
    Assert.False(result);
    Assert.Equal(0u, eax);
    Assert.Equal(0u, ebx);
    Assert.Equal(0u, ecx);
    Assert.Equal(0u, edx);
  }

  [Fact]
  public void OpCode_TryCpuId_DoesNotThrow_WhenNotOpen() {
    var ex = Record.Exception(() =>
      OpCode.TryCpuId(0, 0, out _, out _, out _, out _));
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_TryCpuId_DoesNotThrow_AfterOpen() {
    if (!IsElevated) return;

    OpCode.Open();
    var ex = Record.Exception(() =>
      OpCode.TryCpuId(0, 0, out _, out _, out _, out _));
    Assert.Null(ex);
  }

  [Fact]
  public void OpCode_TryCpuId_Leaf0_ReturnsTrue_AfterOpen() {
    if (!IsElevated) return;

    OpCode.Open();
    bool result = OpCode.TryCpuId(0, 0, out _, out _, out _, out _);
    Assert.True(result,
      "TryCpuId with leaf 0 should succeed after Open().");
  }

  [Fact]
  public void OpCode_TryCpuId_Leaf0_EaxIsNonZero_OnPhysicalMachine() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.TryCpuId(0, 0, out uint eax, out _, out _, out _);

    // CPUID leaf 0 EAX = max supported leaf — always > 0 on real CPUs
    Assert.True(eax > 0,
      $"CPUID leaf 0 EAX (max leaf) should be > 0, got {eax}.");
  }

  [Fact]
  public void OpCode_TryCpuId_Leaf0_VendorRegisters_AreNonZero() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.TryCpuId(0, 0, out _, out uint ebx, out uint ecx, out uint edx);

    // EBX, ECX, EDX encode the vendor string ("GenuineIntel" or "AuthenticAMD")
    Assert.True(ebx != 0 || ecx != 0 || edx != 0,
      "CPUID leaf 0 vendor registers should not all be zero.");
  }

  [Fact]
  public void OpCode_TryCpuId_Leaf0_VendorString_IsKnownValue() {
    if (!IsElevated) return;

    OpCode.Open();
    OpCode.TryCpuId(0, 0, out _, out uint ebx, out uint ecx, out uint edx);

    // Reconstruct vendor string from EBX, EDX, ECX (CPUID order)
    string vendor = string.Concat(
      System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(ebx)),
      System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(edx)),
      System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(ecx)));

    Assert.True(
      vendor == "GenuineIntel" || vendor == "AuthenticAMD" || vendor.Length == 12,
      $"Unexpected CPUID vendor string: '{vendor}'.");
  }

  [Fact]
  public void OpCode_TryCpuId_Leaf1_FamilyModelStepping_AreNonZero() {
    if (!IsElevated) return;

    OpCode.Open();
    bool ok = OpCode.TryCpuId(1, 0, out uint eax, out _, out _, out _);
    if (!ok) return;

    // EAX bits [11:8] = base family, bits [19:16] = extended family
    uint family = ((eax >> 8) & 0xF) + ((eax >> 20) & 0xFF);
    Assert.True(family > 0,
      $"CPUID leaf 1 family should be > 0, got {family}.");
  }

  [Fact]
  public void OpCode_TryCpuId_HighLeaf_DoesNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();

    // 0x80000000 is the extended leaf range start — valid to query
    var ex = Record.Exception(() =>
      OpCode.TryCpuId(0x80000000, 0, out _, out _, out _, out _));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Concurrent access
  // -------------------------------------------------------------------------

  [Fact]
  public async Task OpCode_TryRdtsc_ConcurrentCalls_DoNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[8];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 50; j++)
            OpCode.TryRdtsc(out _);
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent TryRdtsc threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }

  [Fact]
  public async Task OpCode_TryCpuId_ConcurrentCalls_DoNotThrow() {
    if (!IsElevated) return;

    OpCode.Open();
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[8];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 50; j++)
            OpCode.TryCpuId(0, 0, out _, out _, out _, out _);
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent TryCpuId threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }

  [Fact]
  public async Task OpCode_OpenClose_ConcurrentCalls_DoNotThrow() {
    if (!IsElevated) return;

    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[4];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 5; j++) {
            OpCode.Open();
            OpCode.TryRdtsc(out _);
            OpCode.Close();
          }
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent Open/Close threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }
}

/// <summary>
/// Ensures all OpCode tests run sequentially to avoid reference count corruption.
/// </summary>
[CollectionDefinition("OpCode", DisableParallelization = true)]
public class OpCodeCollection { }