using System.Linq;
using System.Reflection;
using CrystalMonitor.Hardware;
using Xunit;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Covers <see cref="Computer"/> behavior that is reachable without calling
/// <see cref="Computer.Open"/>. Open() constructs real, platform-specific
/// hardware-vendor group objects (motherboard/SMBIOS access, USB/HID
/// controllers, GPU vendor APIs, etc.), so it — and the private group
/// lifecycle methods only reachable through it (Add/Remove/AddGroups and
/// friends) — are intentionally out of scope here. This file only exercises
/// the construction and pre-Open() behavior of a never-opened <see cref="Computer"/>.
/// </summary>
public class ComputerUnopenedStateTests {
  [Fact]
  public void Constructor_Parameterless_DoesNotThrow() {
    Computer computer = new();
    Assert.NotNull(computer);
  }

  [Fact]
  public void Constructor_WithNullSettings_DoesNotThrow() {
    // Computer(ISettings) falls back to an internal default Settings instance
    // via `settings ?? new Settings()`. The fallback target is a private
    // nested type with no public accessor, so this can't directly inspect
    // which instance ended up assigned — but construction must succeed, and
    // the resulting instance must behave like any other Computer afterward.
    Computer computer = new(null);
    Assert.NotNull(computer);
    Assert.Empty(computer.Hardware);
  }

  [Fact]
  public void SMBios_BeforeOpen_ThrowsInvalidOperationException() {
    Computer computer = new();
    Assert.Throws<System.InvalidOperationException>(() => { _ = computer.SMBios; });
  }

  [Fact]
  public void Accept_NullVisitor_ThrowsArgumentNullException() {
    Computer computer = new();
    Assert.Throws<System.ArgumentNullException>(() => computer.Accept(null));
  }

  [Fact]
  public void Hardware_BeforeOpen_IsEmpty() {
    Computer computer = new();
    Assert.Empty(computer.Hardware);
  }

  [Fact]
  public void Traverse_BeforeOpen_DoesNotThrow() {
    Computer computer = new();
    StubVisitor visitor = new();
    computer.Traverse(visitor);
    Assert.Equal(0, visitor.ComputerVisitCount);
  }

  [Fact]
  public void GetReport_BeforeOpen_ContainsExpectedStaticSections() {
    Computer computer = new();
    string report = computer.GetReport();

    Assert.NotNull(report);
    Assert.Contains("CrystalMonitor Report", report);
    Assert.Contains("Version:", report);
    Assert.Contains("Common Language Runtime:", report);
    Assert.Contains("Operating System:", report);
    Assert.Contains("Process Type:", report);
    Assert.Contains("Sensors", report);
    Assert.Contains("Parameters", report);
  }

  [Fact]
  public void Close_WithoutOpen_IsNoOp() {
    Computer computer = new();
    bool removedFired = false;
    computer.HardwareRemoved += _ => removedFired = true;

    computer.Close();

    Assert.False(removedFired);
  }

  [Fact]
  public void Reset_WithoutOpen_IsNoOp() {
    Computer computer = new();
    bool addedFired = false;
    bool removedFired = false;
    computer.HardwareAdded += _ => addedFired = true;
    computer.HardwareRemoved += _ => removedFired = true;

    computer.Reset();

    Assert.False(addedFired);
    Assert.False(removedFired);
    Assert.Empty(computer.Hardware);
  }

  // -----------------------------------------------------------------------
  // All ten IsXEnabled properties share the exact same pre-Open() guard
  // pattern (`if (_open && value != _xEnabled) { ... }`), so before Open()
  // is ever called, setting any of them can only toggle the backing field —
  // none of them can reach group construction. Covered generically by name
  // rather than ten near-identical copies of the same test.
  // -----------------------------------------------------------------------

  public static System.Collections.Generic.IEnumerable<object[]> EnabledFlagPropertyNames() {
    yield return new object[] { nameof(Computer.IsBatteryEnabled) };
    yield return new object[] { nameof(Computer.IsControllerEnabled) };
    yield return new object[] { nameof(Computer.IsCpuEnabled) };
    yield return new object[] { nameof(Computer.IsGpuEnabled) };
    yield return new object[] { nameof(Computer.IsPowerMonitorEnabled) };
    yield return new object[] { nameof(Computer.IsMemoryEnabled) };
    yield return new object[] { nameof(Computer.IsMotherboardEnabled) };
    yield return new object[] { nameof(Computer.IsNetworkEnabled) };
    yield return new object[] { nameof(Computer.IsPsuEnabled) };
    yield return new object[] { nameof(Computer.IsStorageEnabled) };
  }

  [Theory]
  [MemberData(nameof(EnabledFlagPropertyNames))]
  public void IsXEnabled_BeforeOpen_OnlyTogglesTheFlag(string propertyName) {
    Computer computer = new();
    PropertyInfo property = typeof(Computer).GetProperty(propertyName);

    property.SetValue(computer, true);
    Assert.True((bool)property.GetValue(computer));

    property.SetValue(computer, false);
    Assert.False((bool)property.GetValue(computer));

    // Since _open is false throughout, none of this should have constructed
    // or registered any real hardware group.
    Assert.Empty(computer.Hardware);
  }

  /// <summary>
  /// Minimal no-op <see cref="IVisitor"/> double — only used to confirm
  /// <see cref="Computer.Traverse"/> doesn't throw and doesn't call back into
  /// it when there are no hardware groups to traverse.
  /// </summary>
  private class StubVisitor : IVisitor {
    public int ComputerVisitCount;

    public void VisitComputer(IComputer computer) => ComputerVisitCount++;

    public void VisitHardware(IHardware hardware) { }

    public void VisitSensor(ISensor sensor) { }

    public void VisitParameter(IParameter parameter) { }
  }
}
