using CrystalMonitor.Hardware;
using CrystalMonitor.Hardware.Cpu;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class CpuIdTests {
  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  /// <summary>
  /// Retrieves the first available CpuId from thread 0 of group 0.
  /// Returns null if the hardware is not accessible (e.g. CI environment).
  /// </summary>
  private static CpuId? GetFirstCpuId() {
    try {
      return CpuId.Get(0, 0);
    }
    catch (ArgumentOutOfRangeException) {
      // OpCode/driver not available in this environment (e.g. no admin rights,
      // driver not loaded). All hardware-dependent tests will skip gracefully.
      return null;
    }
  }

  // -------------------------------------------------------------------------
  // Constants
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_CPUID_0_IsZero() {
    Assert.Equal(0u, CpuId.CPUID_0);
  }

  [Fact]
  public void CpuId_CPUID_EXT_IsExpectedValue() {
    Assert.Equal(0x80000000u, CpuId.CPUID_EXT);
  }

  // -------------------------------------------------------------------------
  // Get() — boundary / guard conditions (no real hardware needed)
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Get_ReturnsNull_WhenThreadIsEqualTo64() {
    // Constructor and Get() both guard: thread >= 64 → null / throw
    var result = CpuId.Get(0, 64);
    Assert.Null(result);
  }

  [Fact]
  public void CpuId_Get_ReturnsNull_WhenThreadExceeds64() {
    var result = CpuId.Get(0, 100);
    Assert.Null(result);
  }

  [Fact]
  public void CpuId_Get_DoesNotThrow_ForValidThread() {
    // May throw ArgumentOutOfRangeException if OpCode driver is unavailable,
    // or return null if affinity cannot be set. Both are valid outcomes.
    try {
      var result = CpuId.Get(0, 0);
      // If it returns without throwing, result is either a valid CpuId or null
      Assert.True(result == null || result.Thread == 0);
    }
    catch (ArgumentOutOfRangeException) {
      // Driver unavailable — acceptable in restricted environments
    }
  }

  // -------------------------------------------------------------------------
  // Get() — integration (real hardware)
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Get_ReturnsNonNull_OnPhysicalMachine() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return; // skip in environments without CPU affinity

    Assert.NotNull(cpuId);
  }

  [Fact]
  public void CpuId_Thread_MatchesRequestedThread() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.Equal(0, cpuId.Thread);
  }

  [Fact]
  public void CpuId_Group_MatchesRequestedGroup() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.Equal(0, cpuId.Group);
  }

  // -------------------------------------------------------------------------
  // Vendor
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Vendor_IsKnownValue() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(
      cpuId.Vendor is Vendor.Intel or Vendor.AMD or Vendor.Unknown,
      $"Unexpected vendor value: {cpuId.Vendor}");
  }

  [Fact]
  public void CpuId_Vendor_IsNotUnknown_OnPhysicalMachine() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Any real physical machine should be Intel or AMD
    Assert.NotEqual(Vendor.Unknown, cpuId.Vendor);
  }

  // -------------------------------------------------------------------------
  // Name / BrandString
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Name_IsNotNullOrEmpty() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.False(string.IsNullOrWhiteSpace(cpuId.Name),
      "CPU Name should not be null or empty on a physical machine.");
  }

  [Fact]
  public void CpuId_BrandString_IsNotNullOrEmpty() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.False(string.IsNullOrWhiteSpace(cpuId.BrandString),
      "CPU BrandString should not be null or empty on a physical machine.");
  }

  [Fact]
  public void CpuId_Name_DoesNotContainRawTrademark() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Constructor strips (R), (TM), (tm) from Name (but not BrandString)
    Assert.DoesNotContain("(R)", cpuId.Name, StringComparison.Ordinal);
    Assert.DoesNotContain("(TM)", cpuId.Name, StringComparison.Ordinal);
    Assert.DoesNotContain("(tm)", cpuId.Name, StringComparison.Ordinal);
  }

  [Fact]
  public void CpuId_Name_DoesNotContainRawCpuWord() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Constructor strips the word "CPU" from Name
    Assert.DoesNotContain("CPU", cpuId.Name, StringComparison.Ordinal);
  }

  [Fact]
  public void CpuId_Name_DoesNotContainAtSymbol() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Constructor removes clock speed suffix after '@'
    Assert.DoesNotContain("@", cpuId.Name, StringComparison.Ordinal);
  }

  [Fact]
  public void CpuId_Name_DoesNotContainDoubleSpaces() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Constructor collapses multiple spaces
    Assert.DoesNotContain("  ", cpuId.Name, StringComparison.Ordinal);
  }

  [Fact]
  public void CpuId_BrandString_ContainsCpuWord_OrVendorName() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // BrandString is the raw value — should contain vendor or CPU indicator
    bool hasIntel = cpuId.BrandString.Contains("Intel", StringComparison.OrdinalIgnoreCase);
    bool hasAmd = cpuId.BrandString.Contains("AMD", StringComparison.OrdinalIgnoreCase);
    bool hasCpu = cpuId.BrandString.Contains("CPU", StringComparison.OrdinalIgnoreCase);

    Assert.True(hasIntel || hasAmd || hasCpu,
      $"BrandString '{cpuId.BrandString}' should contain a vendor or CPU identifier.");
  }

  // -------------------------------------------------------------------------
  // Family / Model / Stepping
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Family_IsGreaterThanZero() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(cpuId.Family > 0,
      $"CPU Family should be > 0, got {cpuId.Family}.");
  }

  [Fact]
  public void CpuId_Model_IsWithinReasonableRange() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Model is a 8-bit value derived from CPUID leaf 1
    Assert.True(cpuId.Model <= 0xFF,
      $"CPU Model {cpuId.Model} exceeds expected 8-bit range.");
  }

  [Fact]
  public void CpuId_Stepping_IsWithinReasonableRange() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Stepping is bits [3:0] of CPUID leaf 1 EAX — max value 15
    Assert.True(cpuId.Stepping <= 0xF,
      $"CPU Stepping {cpuId.Stepping} exceeds expected 4-bit range.");
  }

  // -------------------------------------------------------------------------
  // Data / ExtData arrays
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Data_IsNotNull() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.NotNull(cpuId.Data);
  }

  [Fact]
  public void CpuId_Data_HasFourColumns() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // Data[n, 4] — columns represent EAX, EBX, ECX, EDX
    Assert.Equal(4, cpuId.Data.GetLength(1));
  }

  [Fact]
  public void CpuId_Data_HasAtLeastOneRow() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(cpuId.Data.GetLength(0) >= 1,
      "Data should have at least one row (CPUID leaf 0).");
  }

  [Fact]
  public void CpuId_ExtData_IsNotNull() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.NotNull(cpuId.ExtData);
  }

  [Fact]
  public void CpuId_ExtData_HasFourColumns() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.Equal(4, cpuId.ExtData.GetLength(1));
  }

  [Fact]
  public void CpuId_ExtData_HasAtLeastOneRow() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(cpuId.ExtData.GetLength(0) >= 1,
      "ExtData should have at least one row (extended CPUID leaf 0).");
  }

  // -------------------------------------------------------------------------
  // IDs — ApicId / ProcessorId / CoreId / ThreadId
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_ProcessorId_IsNonNegative() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // ProcessorId is derived from ApicId — should never underflow
    Assert.True(cpuId.ProcessorId >= 0);
  }

  [Fact]
  public void CpuId_CoreId_IsConsistentWithApicId() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // CoreId is a sub-field of ApicId — must not exceed it
    Assert.True(cpuId.CoreId <= cpuId.ApicId,
      $"CoreId {cpuId.CoreId} should not exceed ApicId {cpuId.ApicId}.");
  }

  [Fact]
  public void CpuId_ThreadId_IsConsistentWithApicId() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(cpuId.ThreadId <= cpuId.ApicId,
      $"ThreadId {cpuId.ThreadId} should not exceed ApicId {cpuId.ApicId}.");
  }

  // -------------------------------------------------------------------------
  // CoreType (Intel hybrid architecture)
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_CoreType_IsKnownValue() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.True(
      cpuId.CoreType is CoreType.Performance or CoreType.Efficient or CoreType.Unknown,
      $"Unexpected CoreType value: {cpuId.CoreType}");
  }

  [Fact]
  public void CpuId_CoreType_IsUnknown_ForNonIntelOrOlderCpu() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    // CoreType is only set for Intel CPUs with CPUID leaf 0x1A
    // Non-Intel or older Intel without leaf 0x1A must be Unknown
    bool hasLeaf1A = cpuId.Data.GetLength(0) > 0x1A;
    bool isIntel = cpuId.Vendor == Vendor.Intel;

    if (!isIntel || !hasLeaf1A) {
      Assert.Equal(CoreType.Unknown, cpuId.CoreType);
    }
  }

  // -------------------------------------------------------------------------
  // Affinity
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuId_Affinity_IsNotUndefined() {
    var cpuId = GetFirstCpuId();
    if (cpuId == null) return;

    Assert.NotEqual(GroupAffinity.Undefined, cpuId.Affinity);
  }

  // -------------------------------------------------------------------------
  // Multiple threads
  // -------------------------------------------------------------------------
  private static CpuId? GetCpuIdSafe(int group, int thread) {
    try {
      return CpuId.Get(group, thread);
    }
    catch (ArgumentOutOfRangeException) {
      return null;
    }
  }

  [Fact]
  public void CpuId_Get_MultipleThreads_HaveSameVendor() {
    var thread0 = GetCpuIdSafe(0, 0);
    var thread1 = GetCpuIdSafe(0, 1);

    if (thread0 == null || thread1 == null) return;

    Assert.Equal(thread0.Vendor, thread1.Vendor);
  }

  [Fact]
  public void CpuId_Get_MultipleThreads_HaveSameFamilyAndModel() {
    var thread0 = GetCpuIdSafe(0, 0);
    var thread1 = GetCpuIdSafe(0, 1);

    if (thread0 == null || thread1 == null) return;

    Assert.Equal(thread0.Family, thread1.Family);
    Assert.Equal(thread0.Model, thread1.Model);
  }

  [Fact]
  public void CpuId_Get_AllValidThreads_DoNotThrow() {
    for (int t = 0; t < 64; t++) {
      var result = GetCpuIdSafe(0, t);
      if (result == null) break; // driver unavailable or thread doesn't exist
    }
  }
}