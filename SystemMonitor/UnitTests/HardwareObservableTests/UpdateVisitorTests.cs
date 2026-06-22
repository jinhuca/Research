using System.Collections.Generic;
using CrystalMonitor.Hardware;
using HardwareService;
using Xunit;

namespace HardwareObservableTests;

/// <summary>
/// Tests for <see cref="HardwareService.UpdateVisitor"/>.
///
/// Coverage targets
/// ────────────────
/// <list type="bullet">
///   <item><see cref="UpdateVisitor.VisitSensor"/>   — 1 line, was 0 % (empty body)</item>
///   <item><see cref="UpdateVisitor.VisitParameter"/> — 1 line, was 0 % (empty body)</item>
/// </list>
///
/// <see cref="UpdateVisitor.VisitComputer"/> and <see cref="UpdateVisitor.VisitHardware"/>
/// are already covered by the existing HardwareObservable / TakeSnapshot tests; the tests
/// below add explicit assertions for those paths as well so this class stands alone as a
/// complete specification of UpdateVisitor's behaviour.
///
/// Hardware-bound wrappers note
/// ────────────────────────────
/// <see cref="HardwareObservable.PollAll"/>, <see cref="HardwareObservable.QueryOnce"/>,
/// and <see cref="HardwareObservable.ReadingStream"/> (16 uncovered lines total) call
/// <c>new Computer { … }.Open()</c>, which requires ring-0 kernel-mode drivers that are
/// unavailable in CI or on machines without the target hardware.  The InternalsVisibleTo
/// seam (<see cref="HardwareObservable.PollAllCore"/> / <see cref="HardwareObservable.QueryOnceCore"/>)
/// exists precisely to make all the polling/snapshot/error-handling logic testable without
/// those wrappers.  Those 16 lines are intentionally un-covered and should stay that way.
/// </summary>
public class UpdateVisitorTests {
  // -----------------------------------------------------------------------
  // VisitSensor — empty body, must not throw for any input
  // -----------------------------------------------------------------------

  [Fact]
  public void VisitSensor_NullSensor_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitSensor(null);  // empty body: null must be silently accepted
  }

  [Fact]
  public void VisitSensor_RealSensor_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitSensor(new StubSensor());
  }

  // -----------------------------------------------------------------------
  // VisitParameter — empty body, must not throw for any input
  // -----------------------------------------------------------------------

  [Fact]
  public void VisitParameter_NullParameter_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitParameter(null);  // empty body: null must be silently accepted
  }

  [Fact]
  public void VisitParameter_RealParameter_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitParameter(new StubParameter());
  }

  // -----------------------------------------------------------------------
  // VisitComputer — delegates to computer.Traverse(this)
  // -----------------------------------------------------------------------

  [Fact]
  public void VisitComputer_CallsTraverseWithSameVisitorInstance() {
    UpdateVisitor visitor = new();
    StubComputer computer = new();

    visitor.VisitComputer(computer);

    Assert.True(computer.TraverseCalled);
    Assert.Same(visitor, computer.LastTraverseVisitor);
  }

  // -----------------------------------------------------------------------
  // VisitHardware — calls Update() then recurses into SubHardware via Accept
  // -----------------------------------------------------------------------

  [Fact]
  public void VisitHardware_CallsUpdateOnHardware() {
    UpdateVisitor visitor = new();
    StubHardware hardware = new();

    visitor.VisitHardware(hardware);

    Assert.True(hardware.UpdateCalled);
  }

  [Fact]
  public void VisitHardware_AcceptsEachSubHardwareWithSameVisitor() {
    UpdateVisitor visitor = new();
    StubHardware child1 = new();
    StubHardware child2 = new();
    StubHardware parent = new(child1, child2);

    visitor.VisitHardware(parent);

    Assert.True(child1.AcceptCalled);
    Assert.Same(visitor, child1.LastAcceptedVisitor);
    Assert.True(child2.AcceptCalled);
    Assert.Same(visitor, child2.LastAcceptedVisitor);
  }

  [Fact]
  public void VisitHardware_NoSubHardware_OnlyCallsUpdate() {
    UpdateVisitor visitor = new();
    StubHardware hardware = new();  // zero children

    visitor.VisitHardware(hardware);

    Assert.True(hardware.UpdateCalled);
    Assert.False(hardware.AcceptCalled);
  }

  [Fact]
  public void VisitHardware_UpdateCalledBeforeSubHardwareAccepted() {
    // Ensures Update() is not deferred until after the recursion.
    UpdateVisitor visitor = new();
    List<string> callOrder = new();
    StubHardware child = new() { OnAccept = _ => callOrder.Add("accept-child") };
    StubHardware parent = new(child) { OnUpdate = () => callOrder.Add("update-parent") };

    visitor.VisitHardware(parent);

    Assert.Equal((IEnumerable<string>)new[] { "update-parent", "accept-child" }, callOrder);
  }

  /// <summary>
  /// IMPORTANT: unlike <c>DataStructures.TypeDefinitions.UpdateVisitor</c>,
  /// this service-layer visitor does NOT null-guard <paramref name="hardware"/>
  /// — it will throw <see cref="NullReferenceException"/> if called with null.
  /// This test documents that current (intentional or not) behaviour so that a
  /// future defensive guard doesn't break silently.
  /// </summary>
  [Fact]
  public void VisitHardware_NullHardware_ThrowsNullReferenceException() {
    UpdateVisitor visitor = new();
    Assert.Throws<NullReferenceException>(() => visitor.VisitHardware(null));
  }

  // -----------------------------------------------------------------------
  // Minimal stubs
  // -----------------------------------------------------------------------

  private sealed class StubComputer : IComputer {
    public bool TraverseCalled;
    public IVisitor LastTraverseVisitor;

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
    public void Traverse(IVisitor v) { TraverseCalled = true; LastTraverseVisitor = v; }
    public event HardwareEventHandler HardwareAdded;
    public event HardwareEventHandler HardwareRemoved;
  }

  private sealed class StubHardware : IHardware {
    public bool UpdateCalled;
    public bool AcceptCalled;
    public IVisitor LastAcceptedVisitor;
    public Action OnUpdate;
    public Action<IVisitor> OnAccept;
    private readonly IHardware[] _sub;

    public StubHardware(params IHardware[] sub) { _sub = sub; }

    public void Update() { UpdateCalled = true; OnUpdate?.Invoke(); }
    public void Accept(IVisitor v) { AcceptCalled = true; LastAcceptedVisitor = v; OnAccept?.Invoke(v); }
    public void Traverse(IVisitor v) { }
    public IHardware[] SubHardware => _sub;

    public HardwareType HardwareType => HardwareType.Cpu;
    public Identifier Identifier => null;
    public string Name { get; set; } = "stub";
    public IHardware Parent => null;
    public ISensor[] Sensors => Array.Empty<ISensor>();
    public IDictionary<string, string> Properties => new Dictionary<string, string>();
    public string GetReport() => string.Empty;
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
    public string Name { get; set; } = "Stub Sensor";
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
