// SimulatedHardwareTests.cs
//
// Hardware simulation layer for CrystalMonitorLib unit tests.
//
// All target types are internal but visible to this assembly via
// [assembly: InternalsVisibleTo("CrystalMonitorTests")] in CrystalMonitorLib.csproj.
//
// Coverage targets (all previously 0 % or near-0 %):
//   Hardware   – Name setter, ActivateSensor/DeactivateSensor, Traverse,
//                Close (Closing event), SensorAdded/SensorRemoved events
//   Sensor     – Value setter (Min/Max tracking + 4-sample history accumulation),
//                ResetMin/Max, ClearValues, Accept, Traverse, ValuesTimeWindow,
//                Name setter, Identifier lazy-init, Parameters
//   Control    – constructor, SetDefault, SetSoftware,
//                ControlModeChanged/SoftwareControlValueChanged events
//   Parameter  – constructor, Value setter, IsDefault setter, Accept, Traverse
//   CompositeSensor – Value getter (reducer), Value setter throws

using System;
using System.Collections.Generic;
using System.Linq;
using CrystalMonitor.Hardware;
using Xunit;

namespace CrystalMonitorTests.HardwareTests;

// ═══════════════════════════════════════════════════════════════════════════
// Simulation infrastructure
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>In-memory ISettings implementation, no disk I/O.</summary>
internal sealed class FakeSettings : ISettings {
  private readonly Dictionary<string, string> _store = new();
  public bool Contains(string name) => _store.ContainsKey(name);
  public string GetValue(string name, string value) =>
    _store.TryGetValue(name, out string v) ? v : value;
  public void Remove(string name) => _store.Remove(name);
  public void SetValue(string name, string value) => _store[name] = value;
}

/// <summary>
/// Minimal concrete subclass of <see cref="Hardware"/> for testing.
/// Exposes the protected ActivateSensor / DeactivateSensor hooks
/// so individual tests can add/remove sensors without needing a full
/// hardware group or driver.
/// </summary>
internal sealed class FakeHardware : Hardware {
  public FakeHardware(string name, Identifier identifier, ISettings settings)
    : base(name, identifier, settings) { }

  public FakeHardware(string name = "Test CPU", ISettings settings = null)
    : base(name, new Identifier("cpu", "0"), settings ?? new FakeSettings()) { }

  public override HardwareType HardwareType => HardwareType.Cpu;
  public override void Update() { }

  // Expose protected seams
  public void Activate(ISensor sensor) => ActivateSensor(sensor);
  public void Deactivate(ISensor sensor) => DeactivateSensor(sensor);
}

// ═══════════════════════════════════════════════════════════════════════════
// Hardware base class
// ═══════════════════════════════════════════════════════════════════════════
public class SimulatedHardwareTests {
  // ── Identifier and HardwareType ──────────────────────────────────────────

  [Fact]
  public void Identifier_ReflectsConstructorArgument() {
    Identifier id = new("cpu", "0");
    FakeHardware hw = new("CPU", id, new FakeSettings());
    Assert.Equal(id, hw.Identifier);
  }

  [Fact]
  public void HardwareType_IsOverriddenValue() {
    FakeHardware hw = new();
    Assert.Equal(HardwareType.Cpu, hw.HardwareType);
  }

  // ── Name ─────────────────────────────────────────────────────────────────

  [Fact]
  public void Name_Get_ReturnsConstructorName_WhenNoSettingsOverride() {
    FakeHardware hw = new("MyProcessor");
    Assert.Equal("MyProcessor", hw.Name);
  }

  [Fact]
  public void Name_Set_NonEmpty_UpdatesName() {
    FakeHardware hw = new("Original");
    hw.Name = "Overridden";
    Assert.Equal("Overridden", hw.Name);
  }

  [Fact]
  public void Name_Set_EmptyString_RevertsToOriginalName() {
    FakeHardware hw = new("Original");
    hw.Name = "";
    Assert.Equal("Original", hw.Name);
  }

  [Fact]
  public void Name_Set_Null_RevertsToOriginalName() {
    FakeHardware hw = new("Original");
    hw.Name = null;
    Assert.Equal("Original", hw.Name);
  }

  [Fact]
  public void Name_Set_PersistsToSettings() {
    FakeSettings settings = new();
    FakeHardware hw = new("CPU", new Identifier("cpu", "0"), settings);
    hw.Name = "Custom CPU Name";
    // The new name must be stored in settings so it survives a reload.
    Assert.Equal("Custom CPU Name", hw.Name);
    // Settings should contain the key for the custom name.
    Assert.True(settings.Contains(new Identifier(hw.Identifier, "name").ToString()));
  }

  // ── Sensors / ActivateSensor / DeactivateSensor ──────────────────────────

  [Fact]
  public void Sensors_InitiallyEmpty() {
    FakeHardware hw = new();
    Assert.Empty(hw.Sensors);
  }

  [Fact]
  public void ActivateSensor_AddsSensorToActiveSet() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Temp", 0, SensorType.Temperature, hw, settings);

    hw.Activate(sensor);

    Assert.Equal(1, hw.Sensors.Length);
    Assert.Same(sensor, hw.Sensors[0]);
  }

  [Fact]
  public void ActivateSensor_SameInstanceTwice_AddedOnlyOnce() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Temp", 0, SensorType.Temperature, hw, settings);

    hw.Activate(sensor);
    hw.Activate(sensor);  // second call — HashSet deduplication

    Assert.Equal(1, hw.Sensors.Length);
  }

  [Fact]
  public void DeactivateSensor_RemovesSensorFromActiveSet() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Temp", 0, SensorType.Temperature, hw, settings);
    hw.Activate(sensor);

    hw.Deactivate(sensor);

    Assert.Empty(hw.Sensors);
  }

  [Fact]
  public void SensorAdded_EventFires_WhenActivateSensorAddsNewSensor() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Fan", 0, SensorType.Fan, hw, settings);

    ISensor received = null;
    hw.SensorAdded += s => received = s;
    hw.Activate(sensor);

    Assert.Same(sensor, received);
  }

  [Fact]
  public void SensorAdded_EventNotFired_WhenSensorAlreadyActive() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Fan", 0, SensorType.Fan, hw, settings);
    hw.Activate(sensor);

    int fired = 0;
    hw.SensorAdded += _ => fired++;
    hw.Activate(sensor);  // already active — no event

    Assert.Equal(0, fired);
  }

  [Fact]
  public void SensorRemoved_EventFires_WhenDeactivateSensorRemovesSensor() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Load", 0, SensorType.Load, hw, settings);
    hw.Activate(sensor);

    ISensor removed = null;
    hw.SensorRemoved += s => removed = s;
    hw.Deactivate(sensor);

    Assert.Same(sensor, removed);
  }

  // ── Accept / Traverse ────────────────────────────────────────────────────

  [Fact]
  public void Accept_NullVisitor_ThrowsArgumentNullException() {
    FakeHardware hw = new();
    Assert.Throws<ArgumentNullException>(() => hw.Accept(null));
  }

  [Fact]
  public void Accept_CallsVisitHardwareWithThis() {
    FakeHardware hw = new();
    IHardware visited = null;
    hw.Accept(new LambdaVisitor(visitHardware: h => visited = h));
    Assert.Same(hw, visited);
  }

  [Fact]
  public void Traverse_NoSensors_DoesNotThrow() {
    FakeHardware hw = new();
    hw.Traverse(new LambdaVisitor());  // no-op, must not throw
  }

  [Fact]
  public void Traverse_CallsAcceptOnEachActiveSensor() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor s1 = new("T1", 0, SensorType.Temperature, hw, settings);
    Sensor s2 = new("T2", 1, SensorType.Temperature, hw, settings);
    hw.Activate(s1); hw.Activate(s2);

    List<ISensor> visited = new();
    hw.Traverse(new LambdaVisitor(visitSensor: s => visited.Add(s)));

    Assert.Equal(2, visited.Count);
    Assert.True(visited.Contains(s1));
    Assert.True(visited.Contains(s2));
  }

  // ── Close ────────────────────────────────────────────────────────────────

  [Fact]
  public void Close_FiresClosingEvent() {
    FakeHardware hw = new();
    IHardware received = null;
    hw.Closing += h => received = h;

    hw.Close();

    Assert.Same(hw, received);
  }

  [Fact]
  public void Close_NoHandlers_DoesNotThrow() {
    FakeHardware hw = new();
    hw.Close();  // Closing is null — must not throw
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  private sealed class LambdaVisitor : IVisitor {
    private readonly Action<IHardware> _hw;
    private readonly Action<ISensor> _sensor;
    private readonly Action<IParameter> _param;

    public LambdaVisitor(
        Action<IHardware> visitHardware = null,
        Action<ISensor> visitSensor = null,
        Action<IParameter> visitParam = null) {
      _hw = visitHardware;
      _sensor = visitSensor;
      _param = visitParam;
    }

    public void VisitComputer(IComputer computer) { }
    public void VisitHardware(IHardware hardware) => _hw?.Invoke(hardware);
    public void VisitSensor(ISensor sensor) => _sensor?.Invoke(sensor);
    public void VisitParameter(IParameter param) => _param?.Invoke(param);
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// Sensor  (internal, visible via InternalsVisibleTo)
// ═══════════════════════════════════════════════════════════════════════════
public class SimulatedSensorTests {
  private static (FakeHardware hw, Sensor sensor) Make(
      string name = "Temp",
      SensorType type = SensorType.Temperature,
      int index = 0,
      FakeSettings cfg = null) {
    FakeSettings settings = cfg ?? new FakeSettings();
    FakeHardware hw = new("CPU", new Identifier("cpu", "0"), settings);
    Sensor sensor = new(name, index, type, hw, settings);
    return (hw, sensor);
  }

  // ── Construction ─────────────────────────────────────────────────────────

  [Fact]
  public void Constructor_SetsNameIndexAndType() {
    (_, Sensor s) = Make("CPU Temp", SensorType.Temperature, 2);
    Assert.Equal("CPU Temp", s.Name);
    Assert.Equal(2, s.Index);
    Assert.Equal(SensorType.Temperature, s.SensorType);
  }

  [Fact]
  public void Constructor_NoParameters_EmptyParametersList() {
    (_, Sensor s) = Make();
    Assert.Empty(s.Parameters);
  }

  [Fact]
  public void Constructor_WithParameters_CreatesParameterObjects() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    ParameterDescription[] descs = {
      new("Offset", "Temperature offset", 0f),
      new("Scale",  "Temperature scale",  1f),
    };
    Sensor s = new("Temp", 0, SensorType.Temperature, hw, descs, settings);
    Assert.Equal(2, s.Parameters.Count);
    Assert.Equal("Offset", s.Parameters[0].Name);
    Assert.Equal("Scale", s.Parameters[1].Name);
  }

  [Fact]
  public void Identifier_IsLazilyBuiltFromHardwareIdentifierAndType() {
    (FakeHardware hw, Sensor s) = Make("T", SensorType.Temperature, 0);
    string id = s.Identifier.ToString();
    Assert.Contains(hw.Identifier.ToString(), id);
    Assert.Contains("temperature", id);
    Assert.Contains("0", id);
  }

  [Fact]
  public void Hardware_ReferencesParentHardware() {
    (FakeHardware hw, Sensor s) = Make();
    Assert.Same(hw, s.Hardware);
  }

  [Fact]
  public void IsDefaultHidden_DefaultFalse() {
    (_, Sensor s) = Make();
    Assert.False(s.IsDefaultHidden);
  }

  [Fact]
  public void IsDefaultHidden_TrueWhenSpecified() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor s = new("H", 0, true, SensorType.Load, hw, null, settings);
    Assert.True(s.IsDefaultHidden);
  }

  // ── Value / Min / Max ────────────────────────────────────────────────────

  [Fact]
  public void Value_InitiallyNull() {
    (_, Sensor s) = Make();
    Assert.Null(s.Value);
  }

  [Fact]
  public void Value_Set_UpdatesCurrentValue() {
    (_, Sensor s) = Make();
    s.Value = 55.5f;
    Assert.Equal(55.5f, s.Value);
  }

  [Fact]
  public void Value_Set_TracksMin() {
    (_, Sensor s) = Make();
    s.Value = 70f;
    s.Value = 50f;
    s.Value = 60f;
    Assert.Equal(50f, s.Min);
  }

  [Fact]
  public void Value_Set_TracksMax() {
    (_, Sensor s) = Make();
    s.Value = 40f;
    s.Value = 80f;
    s.Value = 60f;
    Assert.Equal(80f, s.Max);
  }

  [Fact]
  public void Value_Set_NaN_DoesNotUpdateMinMax() {
    (_, Sensor s) = Make();
    s.Value = 50f;
    float? minBefore = s.Min;
    s.Value = float.NaN;
    Assert.Equal(minBefore, s.Min);  // NaN must not clobber existing min
  }

  [Fact]
  public void Value_Set_FourSamples_AppendsAveragedValueToHistory() {
    // Every 4th non-null value triggers an averaged entry in _values.
    (_, Sensor s) = Make();
    s.Value = 10f;
    s.Value = 20f;
    s.Value = 30f;
    Assert.Empty(s.Values);  // 3 samples — not yet flushed
    s.Value = 40f;           // 4th sample triggers flush: avg = (10+20+30+40)/4 = 25
    Assert.Equal(1, s.Values.Count());
    Assert.Equal(25f, s.Values.First().Value);
  }

  [Fact]
  public void Value_Set_EightSamples_AppendsTwoHistoryEntries() {
    (_, Sensor s) = Make();
    for (int i = 0; i < 8; i++) s.Value = (float)(i + 1);
    Assert.Equal(2, s.Values.Count());
  }

  // ── ResetMin / ResetMax ──────────────────────────────────────────────────

  [Fact]
  public void ResetMin_SetsMinToNull() {
    (_, Sensor s) = Make();
    s.Value = 30f;
    s.ResetMin();
    Assert.Null(s.Min);
  }

  [Fact]
  public void ResetMax_SetsMaxToNull() {
    (_, Sensor s) = Make();
    s.Value = 80f;
    s.ResetMax();
    Assert.Null(s.Max);
  }

  // ── ClearValues ──────────────────────────────────────────────────────────

  [Fact]
  public void ClearValues_EmptiesHistoryList() {
    (_, Sensor s) = Make();
    for (int i = 0; i < 4; i++) s.Value = 50f;  // flush one entry
    Assert.Equal(1, s.Values.Count());
    s.ClearValues();
    Assert.Empty(s.Values);
  }

  // ── ValuesTimeWindow ─────────────────────────────────────────────────────

  [Fact]
  public void ValuesTimeWindow_Default_IsOneDay() {
    (_, Sensor s) = Make();
    Assert.Equal(TimeSpan.FromDays(1), s.ValuesTimeWindow);
  }

  [Fact]
  public void ValuesTimeWindow_SetToZero_ClearsExistingHistory() {
    (_, Sensor s) = Make();
    for (int i = 0; i < 4; i++) s.Value = 1f;
    Assert.Equal(1, s.Values.Count());
    s.ValuesTimeWindow = TimeSpan.Zero;
    Assert.Empty(s.Values);
  }

  // ── Name setter ──────────────────────────────────────────────────────────

  [Fact]
  public void Name_Set_NonEmpty_Updates() {
    (_, Sensor s) = Make("Default");
    s.Name = "Custom";
    Assert.Equal("Custom", s.Name);
  }

  [Fact]
  public void Name_Set_Empty_RevertsToDefault() {
    (_, Sensor s) = Make("Default");
    s.Name = "Custom";
    s.Name = "";
    Assert.Equal("Default", s.Name);
  }

  // ── Accept ───────────────────────────────────────────────────────────────

  [Fact]
  public void Accept_NullVisitor_ThrowsArgumentNullException() {
    (_, Sensor s) = Make();
    Assert.Throws<ArgumentNullException>(() => s.Accept(null));
  }

  [Fact]
  public void Accept_CallsVisitSensorWithThis() {
    (_, Sensor s) = Make();
    ISensor received = null;
    s.Accept(new LambdaVisitor(visitSensor: x => received = x));
    Assert.Same(s, received);
  }

  // ── Traverse ─────────────────────────────────────────────────────────────

  [Fact]
  public void Traverse_NoParameters_DoesNotThrow() {
    (_, Sensor s) = Make();
    s.Traverse(new LambdaVisitor());
  }

  [Fact]
  public void Traverse_WithParameters_CallsAcceptOnEach() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    ParameterDescription[] descs = {
      new("A", "desc A", 0f),
      new("B", "desc B", 1f),
    };
    Sensor s = new("T", 0, SensorType.Temperature, hw, descs, settings);

    List<IParameter> visited = new();
    s.Traverse(new LambdaVisitor(visitParam: p => visited.Add(p)));

    Assert.Equal(2, visited.Count);
  }

  // ── Close saves sensor history ────────────────────────────────────────────

  [Fact]
  public void Close_WritesHistoryToSettings() {
    FakeSettings settings = new();
    FakeHardware hw = new("CPU", new Identifier("cpu", "0"), settings);
    Sensor s = new("T", 0, SensorType.Temperature, hw, settings);
    for (int i = 0; i < 4; i++) s.Value = 50f;

    hw.Close();  // fires Closing → SetSensorValuesToSettings

    // After close, settings must contain the serialised history key.
    string key = new Identifier(s.Identifier, "values").ToString();
    Assert.NotNull(settings.GetValue(key, null));
  }

  private sealed class LambdaVisitor : IVisitor {
    readonly Action<ISensor> _sensor;
    readonly Action<IParameter> _param;
    public LambdaVisitor(Action<ISensor> visitSensor = null, Action<IParameter> visitParam = null) {
      _sensor = visitSensor; _param = visitParam;
    }
    public void VisitComputer(IComputer c) { }
    public void VisitHardware(IHardware h) { }
    public void VisitSensor(ISensor s) => _sensor?.Invoke(s);
    public void VisitParameter(IParameter p) => _param?.Invoke(p);
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// Control  (internal, visible via InternalsVisibleTo)
// ═══════════════════════════════════════════════════════════════════════════
public class SimulatedControlTests {
  private static (Sensor sensor, Control control) Make(
      float min = 0f, float max = 100f) {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Fan Speed", 0, SensorType.Fan, hw, settings);
    Control control = new(sensor, settings, min, max);
    return (sensor, control);
  }

  [Fact]
  public void Constructor_InitialControlMode_IsUndefined() {
    (_, Control c) = Make();
    Assert.Equal(ControlMode.Undefined, c.ControlMode);
  }

  [Fact]
  public void Constructor_SetsMinAndMaxSoftwareValue() {
    (_, Control c) = Make(20f, 80f);
    Assert.Equal(20f, c.MinSoftwareValue);
    Assert.Equal(80f, c.MaxSoftwareValue);
  }

  [Fact]
  public void Constructor_SensorProperty_ReferencesSensorArgument() {
    (Sensor s, Control c) = Make();
    Assert.Same(s, c.Sensor);
  }

  [Fact]
  public void Identifier_IncludesSensorIdentifierAndControl() {
    (_, Control c) = Make();
    Assert.Contains("control", c.Identifier.ToString());
  }

  [Fact]
  public void SetDefault_SetsControlModeToDefault() {
    (_, Control c) = Make();
    c.SetDefault();
    Assert.Equal(ControlMode.Default, c.ControlMode);
  }

  [Fact]
  public void SetDefault_FiresControlModeChangedEvent() {
    (_, Control c) = Make();
    int fired = 0;
    c.ControlModeChanged += _ => fired++;
    c.SetDefault();
    Assert.Equal(1, fired);
  }

  [Fact]
  public void SetDefault_CalledTwice_EventFiredOnlyOnce() {
    // Second call: mode is already Default → no change → no event.
    (_, Control c) = Make();
    c.SetDefault();
    int fired = 0;
    c.ControlModeChanged += _ => fired++;
    c.SetDefault();
    Assert.Equal(0, fired);
  }

  [Fact]
  public void SetSoftware_SetsControlModeToSoftware() {
    (_, Control c) = Make();
    c.SetSoftware(75f);
    Assert.Equal(ControlMode.Software, c.ControlMode);
  }

  [Fact]
  public void SetSoftware_SetsSoftwareValue() {
    (_, Control c) = Make();
    c.SetSoftware(75f);
    Assert.Equal(75f, c.SoftwareValue);
  }

  [Fact]
  public void SetSoftware_FiresBothEvents() {
    (_, Control c) = Make();
    int modeFired = 0, valueFired = 0;
    c.ControlModeChanged += _ => modeFired++;
    c.SoftwareControlValueChanged += _ => valueFired++;
    c.SetSoftware(50f);
    Assert.Equal(1, modeFired);
    Assert.Equal(1, valueFired);
  }

  [Fact]
  public void SetSoftware_SameValueTwice_ValueEventFiredOnlyOnce() {
    (_, Control c) = Make();
    c.SetSoftware(50f);
    int valueFired = 0;
    c.SoftwareControlValueChanged += _ => valueFired++;
    c.SetSoftware(50f);  // same value → no change event
    Assert.Equal(0, valueFired);
  }

  [Fact]
  public void SetSoftware_PersistsValueToSettings() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Fan", 0, SensorType.Fan, hw, settings);
    Control c = new(sensor, settings, 0f, 100f);

    c.SetSoftware(65f);

    string key = new Identifier(c.Identifier, "value").ToString();
    Assert.Equal("65", settings.GetValue(key, null));
  }

  [Fact]
  public void SetDefault_PersistsModeToSettings() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("Fan", 0, SensorType.Fan, hw, settings);
    Control c = new(sensor, settings, 0f, 100f);

    c.SetDefault();

    string key = new Identifier(c.Identifier, "mode").ToString();
    Assert.NotNull(settings.GetValue(key, null));
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// Parameter  (internal, visible via InternalsVisibleTo)
// ═══════════════════════════════════════════════════════════════════════════
public class SimulatedParameterTests {
  private static Parameter MakeParam(
      string name = "Offset", float defaultVal = 0f, FakeSettings settings = null) {
    settings ??= new FakeSettings();
    FakeHardware hw = new();
    Sensor sensor = new("T", 0, SensorType.Temperature, hw,
      new[] { new ParameterDescription(name, "desc", defaultVal) }, settings);
    return (Parameter)sensor.Parameters[0];
  }

  [Fact]
  public void Name_ReflectsDescription() {
    Parameter p = MakeParam("Offset");
    Assert.Equal("Offset", p.Name);
  }

  [Fact]
  public void Description_ReflectsDescription() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor sensor = new("T", 0, SensorType.Temperature, hw,
      new[] { new ParameterDescription("A", "my description", 0f) }, settings);
    Assert.Equal("my description", sensor.Parameters[0].Description);
  }

  [Fact]
  public void DefaultValue_ReflectsDescription() {
    Parameter p = MakeParam(defaultVal: 42f);
    Assert.Equal(42f, p.DefaultValue);
  }

  [Fact]
  public void IsDefault_TrueWhenNoSettingsOverride() {
    Parameter p = MakeParam();
    Assert.True(p.IsDefault);
  }

  [Fact]
  public void Value_InitiallyEqualsDefaultValue() {
    Parameter p = MakeParam(defaultVal: 7f);
    Assert.Equal(7f, p.Value);
  }

  [Fact]
  public void Value_Set_UpdatesValueAndClearsIsDefault() {
    Parameter p = MakeParam(defaultVal: 0f);
    p.Value = 99f;
    Assert.Equal(99f, p.Value);
    Assert.False(p.IsDefault);
  }

  [Fact]
  public void IsDefault_Set_True_RestoresDefaultValue() {
    Parameter p = MakeParam(defaultVal: 5f);
    p.Value = 99f;
    p.IsDefault = true;
    Assert.True(p.IsDefault);
    Assert.Equal(5f, p.Value);
  }

  [Fact]
  public void IsDefault_Set_True_RemovesKeyFromSettings() {
    FakeSettings settings = new();
    Parameter p = MakeParam("Offset", 0f, settings);
    p.Value = 10f;
    string key = p.Identifier.ToString();
    Assert.True(settings.Contains(key));
    p.IsDefault = true;
    Assert.False(settings.Contains(key));
  }

  [Fact]
  public void Identifier_IncludesParameterNameAndSensorIdentifier() {
    Parameter p = MakeParam("Offset");
    string id = p.Identifier.ToString();
    Assert.Contains("parameter", id);
    Assert.Contains("offset", id);  // name is lowercased
  }

  [Fact]
  public void Accept_NullVisitor_ThrowsArgumentNullException() {
    Parameter p = MakeParam();
    Assert.Throws<ArgumentNullException>(() => p.Accept(null));
  }

  [Fact]
  public void Accept_CallsVisitParameterWithThis() {
    Parameter p = MakeParam();
    IParameter received = null;
    p.Accept(new LambdaVisitor(visitParam: x => received = x));
    Assert.Same(p, received);
  }

  [Fact]
  public void Traverse_DoesNotThrow() {
    // Traverse is an empty body on Parameter.
    Parameter p = MakeParam();
    p.Traverse(new LambdaVisitor());
  }

  private sealed class LambdaVisitor : IVisitor {
    readonly Action<IParameter> _param;
    public LambdaVisitor(Action<IParameter> visitParam = null) { _param = visitParam; }
    public void VisitComputer(IComputer c) { }
    public void VisitHardware(IHardware h) { }
    public void VisitSensor(ISensor s) { }
    public void VisitParameter(IParameter p) => _param?.Invoke(p);
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// CompositeSensor  (internal, visible via InternalsVisibleTo)
// ═══════════════════════════════════════════════════════════════════════════
public class SimulatedCompositeSensorTests {
  private static (FakeHardware hw, Sensor component, CompositeSensor composite) Make(
      Func<float, ISensor, float> reducer, float seed = 0f) {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor component = new("Core #0", 0, SensorType.Load, hw, settings);
    CompositeSensor comp = new("CPU Total", 99, SensorType.Load, hw, settings,
                                new ISensor[] { component }, reducer, seed);
    return (hw, component, comp);
  }

  [Fact]
  public void Value_Get_InvokesReducerOverComponents() {
    // reducer = sum all component Values (treating null as 0)
    (_, Sensor c, CompositeSensor comp) = Make(
      (acc, s) => acc + (s.Value ?? 0f), seed: 0f);
    c.Value = 42f;
    Assert.Equal(42f, comp.Value);
  }

  [Fact]
  public void Value_Get_WithSeedValue_AppliesSeedAsInitialAccumulator() {
    (_, Sensor c, CompositeSensor comp) = Make(
      (acc, s) => acc + (s.Value ?? 0f), seed: 10f);
    c.Value = 5f;
    Assert.Equal(15f, comp.Value);  // 10 (seed) + 5 (component)
  }

  [Fact]
  public void Value_Get_ComponentWithNullValue_ReducerHandlesNull() {
    (_, _, CompositeSensor comp) = Make(
      (acc, s) => acc + (s.Value ?? 0f), seed: 0f);
    // component.Value is still null
    Assert.Equal(0f, comp.Value);
  }

  [Fact]
  public void Value_Set_ThrowsNotImplementedException() {
    (_, _, CompositeSensor comp) = Make((acc, s) => acc);
    Assert.Throws<NotImplementedException>(() => comp.Value = 99f);
  }

  [Fact]
  public void SensorType_MatchesConstructorArgument() {
    (_, _, CompositeSensor comp) = Make((acc, s) => acc);
    Assert.Equal(SensorType.Load, comp.SensorType);
  }

  [Fact]
  public void Name_MatchesConstructorArgument() {
    (_, _, CompositeSensor comp) = Make((acc, s) => acc);
    Assert.Equal("CPU Total", comp.Name);
  }

  [Fact]
  public void MultipleComponents_ReducerReceivesAll() {
    FakeSettings settings = new();
    FakeHardware hw = new();
    Sensor c1 = new("Core #0", 0, SensorType.Load, hw, settings);
    Sensor c2 = new("Core #1", 1, SensorType.Load, hw, settings);
    Sensor c3 = new("Core #2", 2, SensorType.Load, hw, settings);
    c1.Value = 10f; c2.Value = 20f; c3.Value = 30f;

    CompositeSensor comp = new("Total", 99, SensorType.Load, hw, settings,
      new ISensor[] { c1, c2, c3 }, (acc, s) => acc + (s.Value ?? 0f), 0f);

    Assert.Equal(60f, comp.Value);
  }
}
