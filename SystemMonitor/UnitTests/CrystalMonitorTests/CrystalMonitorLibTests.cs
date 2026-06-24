using System;
using System.Collections.Generic;
using CrystalMonitor.Hardware;
using CrystalMonitor.Hardware.Motherboard.Lpc.EC;
using Xunit;

namespace CrystalMonitorTests.HardwareTests;

// ═══════════════════════════════════════════════════════════════════════════
// SensorVisitor  – 0 % covered, 17 lines
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Tests for <see cref="SensorVisitor"/>.
///
/// Coverage targets
/// ────────────────
/// <list type="bullet">
///   <item>Constructor (null-guard)</item>
///   <item><see cref="SensorVisitor.VisitComputer"/> (null-guard + Traverse delegation)</item>
///   <item><see cref="SensorVisitor.VisitHardware"/> (null-guard + Traverse delegation)</item>
///   <item><see cref="SensorVisitor.VisitSensor"/> (invokes the handler)</item>
///   <item><see cref="SensorVisitor.VisitParameter"/> (empty body)</item>
/// </list>
/// </summary>
public class SensorVisitorTests {
  // ── Constructor ──────────────────────────────────────────────────────────

  [Fact]
  public void Constructor_NullHandler_ThrowsArgumentNullException() {
    Assert.Throws<ArgumentNullException>(() => new SensorVisitor(null));
  }

  [Fact]
  public void Constructor_ValidHandler_DoesNotThrow() {
    SensorVisitor visitor = new(sensor => { });
    Assert.NotNull(visitor);
  }

  // ── VisitComputer ────────────────────────────────────────────────────────

  [Fact]
  public void VisitComputer_NullComputer_ThrowsArgumentNullException() {
    SensorVisitor visitor = new(sensor => { });
    Assert.Throws<ArgumentNullException>(() => visitor.VisitComputer(null));
  }

  [Fact]
  public void VisitComputer_CallsTraverseOnComputer_WithSameVisitorInstance() {
    SensorVisitor visitor = new(sensor => { });
    StubComputer computer = new();

    visitor.VisitComputer(computer);

    Assert.True(computer.TraverseCalled);
    Assert.Same(visitor, computer.LastVisitor);
  }

  // ── VisitHardware ────────────────────────────────────────────────────────

  [Fact]
  public void VisitHardware_NullHardware_ThrowsArgumentNullException() {
    SensorVisitor visitor = new(sensor => { });
    Assert.Throws<ArgumentNullException>(() => visitor.VisitHardware(null));
  }

  [Fact]
  public void VisitHardware_CallsTraverseOnHardware_WithSameVisitorInstance() {
    SensorVisitor visitor = new(sensor => { });
    StubHardware hardware = new();

    visitor.VisitHardware(hardware);

    Assert.True(hardware.TraverseCalled);
    Assert.Same(visitor, hardware.LastVisitor);
  }

  // ── VisitSensor ──────────────────────────────────────────────────────────

  [Fact]
  public void VisitSensor_InvokesHandlerWithTheSensor() {
    ISensor received = null;
    SensorVisitor visitor = new(s => received = s);
    StubSensor sensor = new();

    visitor.VisitSensor(sensor);

    Assert.Same(sensor, received);
  }

  [Fact]
  public void VisitSensor_HandlerInvokedExactlyOnce() {
    int callCount = 0;
    SensorVisitor visitor = new(_ => callCount++);

    visitor.VisitSensor(new StubSensor());

    Assert.Equal(1, callCount);
  }

  // ── VisitParameter ───────────────────────────────────────────────────────

  [Fact]
  public void VisitParameter_DoesNotThrow() {
    SensorVisitor visitor = new(sensor => { });
    visitor.VisitParameter(null);   // empty body — null must not throw
  }

  [Fact]
  public void VisitParameter_WithRealParameter_DoesNotThrow() {
    SensorVisitor visitor = new(sensor => { });
    visitor.VisitParameter(new StubParameter());
  }

  // ── Stubs ────────────────────────────────────────────────────────────────

  private sealed class StubComputer : IComputer {
    public bool TraverseCalled;
    public IVisitor LastVisitor;

    public IList<IHardware> Hardware => new List<IHardware>();
    public bool IsBatteryEnabled => false;
    public bool IsControllerEnabled => false;
    public bool IsCpuEnabled => false;
    public bool IsGpuEnabled => false;
    public bool IsPowerMonitorEnabled => false;
    public bool IsMemoryEnabled => false;
    public bool IsMotherboardEnabled => false;
    public bool IsNetworkEnabled => false;
    public bool IsPsuEnabled => false;
    public bool IsStorageEnabled => false;
    public string GetReport() => string.Empty;
    public void Accept(IVisitor v) { }
    public void Traverse(IVisitor v) { TraverseCalled = true; LastVisitor = v; }
    public event HardwareEventHandler HardwareAdded;
    public event HardwareEventHandler HardwareRemoved;
  }

  private sealed class StubHardware : IHardware {
    public bool TraverseCalled;
    public IVisitor LastVisitor;

    public HardwareType HardwareType => HardwareType.Cpu;
    public Identifier Identifier => null;
    public string Name { get; set; } = "stub";
    public IHardware Parent => null;
    public ISensor[] Sensors => Array.Empty<ISensor>();
    public IHardware[] SubHardware => Array.Empty<IHardware>();
    public IDictionary<string, string> Properties => new Dictionary<string, string>();
    public string GetReport() => string.Empty;
    public void Update() { }
    public void Accept(IVisitor v) { }
    public void Traverse(IVisitor v) { TraverseCalled = true; LastVisitor = v; }
    public event SensorEventHandler SensorAdded;
    public event SensorEventHandler SensorRemoved;
  }

  private sealed class StubSensor : ISensor {
    public IControl Control => null;
    public IHardware Hardware => null;
    public Identifier Identifier => null;
    public int Index => 0;
    public bool IsDefaultHidden => false;
    public float? Max { get; set; }
    public float? Min { get; set; }
    public string Name { get; set; } = "stub";
    public IReadOnlyList<IParameter> Parameters => Array.Empty<IParameter>();
    public SensorType SensorType => SensorType.Temperature;
    public float? Value => null;
    public IEnumerable<SensorValue> Values => Array.Empty<SensorValue>();
    public TimeSpan ValuesTimeWindow { get; set; }
    public void ResetMin() { }
    public void ResetMax() { }
    public void ClearValues() { }
    public void Accept(IVisitor v) { }
    public void Traverse(IVisitor v) { }
  }

  private sealed class StubParameter : IParameter {
    public float DefaultValue => 0f;
    public string Description => "stub";
    public Identifier Identifier => null;
    public bool IsDefault { get; set; }
    public string Name => "stub";
    public ISensor Sensor => null;
    public float Value { get; set; }
    public void Accept(IVisitor v) { }
    public void Traverse(IVisitor v) { }
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// Identifier  – the one uncovered line: Equals() when obj is a non-null
//               non-Identifier object (the `id == null → return false` branch)
// ═══════════════════════════════════════════════════════════════════════════

public class IdentifierEqualsTests {
  [Fact]
  public void Equals_NonNullNonIdentifierObject_ReturnsFalse() {
    // This hits the previously-uncovered `if (id == null) return false;` branch
    // in Identifier.Equals(object obj).
    Identifier id = new("cpu", "0");
    Assert.False(id.Equals("not an Identifier"));
    Assert.False(id.Equals(42));
    Assert.False(id.Equals(new object()));
  }

  [Fact]
  public void Equals_NullObject_ReturnsFalse() {
    Identifier id = new("cpu", "0");
    Assert.False(id.Equals((object)null));
  }

  [Fact]
  public void Equals_SameIdentifierObject_ReturnsTrue() {
    Identifier a = new("cpu", "0");
    Identifier b = new("cpu", "0");
    Assert.True(a.Equals((object)b));
  }

  [Fact]
  public void Equals_DifferentIdentifier_ReturnsFalse() {
    Identifier a = new("cpu", "0");
    Identifier b = new("gpu", "0");
    Assert.False(a.Equals((object)b));
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// EmbeddedController exception types
// Each constructor body is 1 line; all three were at 0 % coverage.
// ═══════════════════════════════════════════════════════════════════════════

public class EmbeddedControllerExceptionTests {
  [Fact]
  public void IOException_SetsMessageWithPrefix() {
    // Constructor prepends "ACPI embedded controller I/O error: " to the message.
    EmbeddedController.IOException ex = new("timeout");
    Assert.Contains("timeout", ex.Message);
    Assert.Contains("ACPI embedded controller", ex.Message);
  }

  [Fact]
  public void IOException_IsSystemIOIOException() {
    // Verify the inheritance chain — callers catch System.IO.IOException.
    EmbeddedController.IOException ex = new("err");
    Assert.IsAssignableFrom<System.IO.IOException>(ex);
  }

  [Fact]
  public void BadConfigurationException_SetsMessageVerbatim() {
    EmbeddedController.BadConfigurationException ex = new("bad config detail");
    Assert.Equal("bad config detail", ex.Message);
  }

  [Fact]
  public void BadConfigurationException_IsException() {
    EmbeddedController.BadConfigurationException ex = new("x");
    Assert.IsAssignableFrom<Exception>(ex);
  }

  [Fact]
  public void MultipleBoardRecordsFoundException_SetsModelInMessage() {
    // Constructor formats: "Multiple board records refer to the same model '{model}'"
    EmbeddedController.MultipleBoardRecordsFoundException ex = new("ROG STRIX X570-E");
    Assert.Contains("ROG STRIX X570-E", ex.Message);
    Assert.Contains("Multiple board records", ex.Message);
  }

  [Fact]
  public void MultipleBoardRecordsFoundException_IsBadConfigurationException() {
    // Verify the inheritance chain.
    EmbeddedController.MultipleBoardRecordsFoundException ex = new("board");
    Assert.IsAssignableFrom<EmbeddedController.BadConfigurationException>(ex);
  }
}
