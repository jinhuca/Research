using DataStructures.Cpu.Implementations;

namespace HardwareService.Tests;

// ── Tests ─────────────────────────────────────────────────────────────────────
// CpuSummaryInfo is a plain BindableBase model: every property is just a
// get/set pair over SetProperty. These tests are table-driven on purpose —
// 13 near-identical properties don't need 13 near-identical test methods.

public class CpuSummaryInfoTests {
  // ── Defaults ──────────────────────────────────────────────────────────────

  [Fact]
  public void NewInstance_AllPropertiesAreNull() {
    var info = new CpuSummaryInfo();

    Assert.Null(info.BrandName);
    Assert.Null(info.VendorName);
    Assert.Null(info.FamilyId);
    Assert.Null(info.ModelId);
    Assert.Null(info.SteppingId);
    Assert.Null(info.BaseSpeed);
    Assert.Null(info.BusSpeed);
    Assert.Null(info.SocketNum);
    Assert.Null(info.PhysicalCoreNum);
    Assert.Null(info.LogicalCoreNum);
    Assert.Null(info.Virtualization);
    Assert.Null(info.CacheInfo);
    Assert.Null(info.InstructionSet);
  }

  // ── Simple (primitive-backed) properties: round-trip + change notification ─

  public static IEnumerable<object[]> SimplePropertyCases() {
    yield return Case("BrandName", s => s.BrandName = "Intel Core i9-14900K", s => s.BrandName, "Intel Core i9-14900K");
    yield return Case("VendorName", s => s.VendorName = "GenuineIntel", s => s.VendorName, "GenuineIntel");
    yield return Case("FamilyId", s => s.FamilyId = 6, s => s.FamilyId, 6);
    yield return Case("ModelId", s => s.ModelId = 183, s => s.ModelId, 183);
    yield return Case("SteppingId", s => s.SteppingId = 1, s => s.SteppingId, 1);
    yield return Case("BaseSpeed", s => s.BaseSpeed = 3.2f, s => s.BaseSpeed, 3.2f);
    yield return Case("BusSpeed", s => s.BusSpeed = 100f, s => s.BusSpeed, 100f);
    yield return Case("SocketNum", s => s.SocketNum = 1700, s => s.SocketNum, 1700);
    yield return Case("PhysicalCoreNum", s => s.PhysicalCoreNum = 24, s => s.PhysicalCoreNum, 24);
    yield return Case("LogicalCoreNum", s => s.LogicalCoreNum = 32, s => s.LogicalCoreNum, 32);
    yield return Case("Virtualization", s => s.Virtualization = true, s => s.Virtualization, true);

    static object[] Case(
        string propertyName,
        Action<CpuSummaryInfo> setValue,
        Func<CpuSummaryInfo, object?> getValue,
        object expected)
        => new object[] { propertyName, setValue, getValue, expected };
  }

  [Theory]
  [MemberData(nameof(SimplePropertyCases))]
  public void SimpleProperty_RoundTrips_AndRaisesPropertyChanged(
      string propertyName,
      Action<CpuSummaryInfo> setValue,
      Func<CpuSummaryInfo, object?> getValue,
      object expected) {
    var info = new CpuSummaryInfo();
    var raisedFor = new List<string?>();
    info.PropertyChanged += (_, e) => raisedFor.Add(e.PropertyName);

    setValue(info);

    Assert.Equal(expected, getValue(info));
    Assert.Contains(propertyName, raisedFor);
  }

  // ── Complex-typed properties ─────────────────────────────────────────────
  // CpuCacheInfo / CpuInstructionInfo definitions aren't available yet, so this
  // only confirms the setter line executes without assuming a constructor shape.
  // TODO: replace with real round-trip tests once those types are shared.

  [Fact]
  public void CacheInfo_AcceptsNullAssignment() {
    var info = new CpuSummaryInfo { CacheInfo = null };
    Assert.Null(info.CacheInfo);
  }

  [Fact]
  public void InstructionSet_AcceptsNullAssignment() {
    var info = new CpuSummaryInfo { InstructionSet = null };
    Assert.Null(info.InstructionSet);
  }
}
