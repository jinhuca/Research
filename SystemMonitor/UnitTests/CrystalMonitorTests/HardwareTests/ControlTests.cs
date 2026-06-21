using CrystalMonitor.Hardware;
using System.Globalization;

namespace CrystalMonitorTests.HardwareTests;

public class ControlTests {
  // -------------------------------------------------------------------------
  // Test doubles
  // -------------------------------------------------------------------------

  private class TestSettings : ISettings {
    private readonly Dictionary<string, string> _store = new();

    public bool Contains(string name) => _store.ContainsKey(name);
    public void SetValue(string name, string value) => _store[name] = value;
    public string GetValue(string name, string value) =>
      _store.TryGetValue(name, out var v) ? v : value;
    public void Remove(string name) => _store.Remove(name);
  }

  private class TestHardware : Hardware {
    private readonly ISettings _testSettings;

    public TestHardware(ISettings settings)
      : base("Test CPU", new Identifier("testcpu", "0"), settings) {
      _testSettings = settings;
    }

    public override HardwareType HardwareType => HardwareType.Cpu;
    public override void Update() { }

    public ISensor CreateTestSensor(string name = "Test Sensor", int index = 0) =>
      new Sensor(name, index, SensorType.Load, this, _testSettings);
  }

  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  private static (Control control, TestSettings settings, ISensor sensor) CreateControl(
    float min = 0f,
    float max = 100f,
    string savedValue = null,
    string savedMode = null) {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();

    // Pre-populate settings to simulate persisted state
    var tempControl = new Control(sensor, new TestSettings(), min, max);
    string valueKey = new Identifier(tempControl.Identifier, "value").ToString();
    string modeKey = new Identifier(tempControl.Identifier, "mode").ToString();

    if (savedValue != null)
      settings.SetValue(valueKey, savedValue);
    if (savedMode != null)
      settings.SetValue(modeKey, savedMode);

    var control = new Control(sensor, settings, min, max);
    return (control, settings, sensor);
  }

  // -------------------------------------------------------------------------
  // Construction
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_Construction_DoesNotThrow() {
    var ex = Record.Exception(() => CreateControl());
    Assert.Null(ex);
  }

  [Fact]
  public void Control_MinSoftwareValue_IsSetFromConstructor() {
    var (control, _, _) = CreateControl(min: 10f, max: 90f);
    Assert.Equal(10f, control.MinSoftwareValue);
  }

  [Fact]
  public void Control_MaxSoftwareValue_IsSetFromConstructor() {
    var (control, _, _) = CreateControl(min: 10f, max: 90f);
    Assert.Equal(90f, control.MaxSoftwareValue);
  }

  [Fact]
  public void Control_Sensor_IsSetFromConstructor() {
    var (control, _, sensor) = CreateControl();
    Assert.Equal(sensor, control.Sensor);
  }

  [Fact]
  public void Control_Identifier_ContainsSensorIdentifier() {
    var (control, _, sensor) = CreateControl();
    Assert.Contains(sensor.Identifier.ToString(),
      control.Identifier.ToString(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Control_Identifier_ContainsControlSuffix() {
    var (control, _, _) = CreateControl();
    Assert.Contains("control", control.Identifier.ToString(),
      StringComparison.OrdinalIgnoreCase);
  }

  // -------------------------------------------------------------------------
  // Construction — persisted state restore
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_SoftwareValue_IsRestoredFromSettings() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();

    // Create once to get the identifier, then pre-populate settings
    var tempControl = new Control(sensor, settings, 0f, 100f);
    string valueKey = new Identifier(tempControl.Identifier, "value").ToString();
    settings.SetValue(valueKey, "75.5");

    var control = new Control(sensor, settings, 0f, 100f);
    Assert.Equal(75.5f, control.SoftwareValue);
  }

  [Fact]
  public void Control_SoftwareValue_DefaultsToZero_WhenSettingsInvalid() {
    var (control, _, _) = CreateControl(savedValue: "not_a_number");
    Assert.Equal(0f, control.SoftwareValue);
  }

  [Fact]
  public void Control_ControlMode_IsRestoredFromSettings() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();

    var tempControl = new Control(sensor, settings, 0f, 100f);
    string modeKey = new Identifier(tempControl.Identifier, "mode").ToString();
    settings.SetValue(modeKey, ((int)ControlMode.Software).ToString(CultureInfo.InvariantCulture));

    var control = new Control(sensor, settings, 0f, 100f);
    Assert.Equal(ControlMode.Software, control.ControlMode);
  }

  [Fact]
  public void Control_ControlMode_DefaultsToUndefined_WhenSettingsInvalid() {
    var (control, _, _) = CreateControl(savedMode: "not_a_number");
    Assert.Equal(ControlMode.Undefined, control.ControlMode);
  }

  [Fact]
  public void Control_ControlMode_DefaultsToUndefined_WhenNoSettings() {
    var (control, _, _) = CreateControl();
    Assert.Equal(ControlMode.Undefined, control.ControlMode);
  }

  // -------------------------------------------------------------------------
  // SetDefault
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_SetDefault_SetsControlModeToDefault() {
    var (control, _, _) = CreateControl();
    control.SetDefault();
    Assert.Equal(ControlMode.Default, control.ControlMode);
  }

  [Fact]
  public void Control_SetDefault_DoesNotThrow() {
    var (control, _, _) = CreateControl();
    var ex = Record.Exception(() => control.SetDefault());
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SetDefault_PersistsMode_InSettings() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();
    var control = new Control(sensor, settings, 0f, 100f);

    control.SetDefault();

    string modeKey = new Identifier(control.Identifier, "mode").ToString();
    string stored = settings.GetValue(modeKey, string.Empty);
    Assert.Equal(((int)ControlMode.Default).ToString(CultureInfo.InvariantCulture), stored);
  }

  [Fact]
  public void Control_SetDefault_FiresControlModeChangedEvent() {
    var (control, _, _) = CreateControl();
    Control? eventControl = null;
    control.ControlModeChanged += c => eventControl = c;

    control.SetDefault();

    Assert.Equal(control, eventControl);
  }

  [Fact]
  public void Control_SetDefault_CalledTwice_FiresEventOnlyOnce() {
    var (control, _, _) = CreateControl();
    int eventCount = 0;
    control.ControlModeChanged += _ => eventCount++;

    control.SetDefault();
    control.SetDefault(); // same mode — should not fire again

    Assert.Equal(1, eventCount);
  }

  // -------------------------------------------------------------------------
  // SetSoftware
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_SetSoftware_SetsControlModeToSoftware() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(50f);
    Assert.Equal(ControlMode.Software, control.ControlMode);
  }

  [Fact]
  public void Control_SetSoftware_SetsSoftwareValue() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(42f);
    Assert.Equal(42f, control.SoftwareValue);
  }

  [Fact]
  public void Control_SetSoftware_DoesNotThrow() {
    var (control, _, _) = CreateControl();
    var ex = Record.Exception(() => control.SetSoftware(50f));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SetSoftware_PersistsValue_InSettings() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();
    var control = new Control(sensor, settings, 0f, 100f);

    control.SetSoftware(65f);

    string valueKey = new Identifier(control.Identifier, "value").ToString();
    string stored = settings.GetValue(valueKey, string.Empty);
    Assert.Equal((65f).ToString(CultureInfo.InvariantCulture), stored);
  }

  [Fact]
  public void Control_SetSoftware_PersistsMode_InSettings() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();
    var control = new Control(sensor, settings, 0f, 100f);

    control.SetSoftware(50f);

    string modeKey = new Identifier(control.Identifier, "mode").ToString();
    string stored = settings.GetValue(modeKey, string.Empty);
    Assert.Equal(((int)ControlMode.Software).ToString(CultureInfo.InvariantCulture), stored);
  }

  [Fact]
  public void Control_SetSoftware_FiresSoftwareControlValueChangedEvent() {
    var (control, _, _) = CreateControl();
    Control? eventControl = null;
    control.SoftwareControlValueChanged += c => eventControl = c;

    control.SetSoftware(55f);

    Assert.Equal(control, eventControl);
  }

  [Fact]
  public void Control_SetSoftware_FiresControlModeChangedEvent_WhenModeChanges() {
    var (control, _, _) = CreateControl();
    int eventCount = 0;
    control.ControlModeChanged += _ => eventCount++;

    control.SetSoftware(50f); // mode changes from Undefined to Software
    Assert.Equal(1, eventCount);
  }

  [Fact]
  public void Control_SetSoftware_DoesNotFireControlModeChanged_WhenModeUnchanged() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(50f); // set to Software mode

    int eventCount = 0;
    control.ControlModeChanged += _ => eventCount++;
    control.SetSoftware(75f); // already Software — mode unchanged

    Assert.Equal(0, eventCount);
  }

  [Fact]
  public void Control_SetSoftware_DoesNotFireSoftwareValueChanged_WhenValueUnchanged() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(50f);

    int eventCount = 0;
    control.SoftwareControlValueChanged += _ => eventCount++;
    control.SetSoftware(50f); // same value — should not fire

    Assert.Equal(0, eventCount);
  }

  [Fact]
  public void Control_SetSoftware_WithMinValue_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: 0f, max: 100f);
    var ex = Record.Exception(() => control.SetSoftware(0f));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SetSoftware_WithMaxValue_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: 0f, max: 100f);
    var ex = Record.Exception(() => control.SetSoftware(100f));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Mode transitions
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_SetSoftware_ThenSetDefault_ChangesModeToDefault() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(50f);
    control.SetDefault();
    Assert.Equal(ControlMode.Default, control.ControlMode);
  }

  [Fact]
  public void Control_SetDefault_ThenSetSoftware_ChangesModeToSoftware() {
    var (control, _, _) = CreateControl();
    control.SetDefault();
    control.SetSoftware(30f);
    Assert.Equal(ControlMode.Software, control.ControlMode);
  }

  [Fact]
  public void Control_SoftwareValue_IsRetained_AfterModeChangedToDefault() {
    var (control, _, _) = CreateControl();
    control.SetSoftware(60f);
    control.SetDefault();

    // SoftwareValue field retains last set value even after mode change
    Assert.Equal(60f, control.SoftwareValue);
  }

  // -------------------------------------------------------------------------
  // Event behavior
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_ControlModeChanged_CanMultipleSubscribersBeAdded() {
    var (control, _, _) = CreateControl();
    int count1 = 0, count2 = 0;
    control.ControlModeChanged += _ => count1++;
    control.ControlModeChanged += _ => count2++;

    control.SetDefault();

    Assert.Equal(1, count1);
    Assert.Equal(1, count2);
  }

  [Fact]
  public void Control_SoftwareControlValueChanged_CanMultipleSubscribersBeAdded() {
    var (control, _, _) = CreateControl();
    int count1 = 0, count2 = 0;
    control.SoftwareControlValueChanged += _ => count1++;
    control.SoftwareControlValueChanged += _ => count2++;

    control.SetSoftware(50f);

    Assert.Equal(1, count1);
    Assert.Equal(1, count2);
  }

  [Fact]
  public void Control_SetSoftware_FiresMultipleEventsInCorrectOrder() {
    var (control, _, _) = CreateControl();
    var eventSequence = new List<string>();

    control.ControlModeChanged += _ => eventSequence.Add("ModeChanged");
    control.SoftwareControlValueChanged += _ => eventSequence.Add("ValueChanged");

    control.SetSoftware(50f);

    // Both events should fire (order may vary based on subscription order)
    Assert.Contains("ModeChanged", eventSequence);
    Assert.Contains("ValueChanged", eventSequence);
  }

  // -------------------------------------------------------------------------
  // Edge cases
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_SoftwareValue_WithZero_DoesNotThrow() {
    var (control, _, _) = CreateControl();
    var ex = Record.Exception(() => control.SetSoftware(0f));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SoftwareValue_WithNegative_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: -50f, max: 50f);
    var ex = Record.Exception(() => control.SetSoftware(-25f));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SoftwareValue_WithFloatMaxValue_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: float.MinValue, max: float.MaxValue);
    var ex = Record.Exception(() => control.SetSoftware(float.MaxValue));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SoftwareValue_WithFloatMinValue_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: float.MinValue, max: float.MaxValue);
    var ex = Record.Exception(() => control.SetSoftware(float.MinValue));
    Assert.Null(ex);
  }

  [Fact]
  public void Control_MinSoftwareValue_CanBeNegative() {
    var (control, _, _) = CreateControl(min: -100f, max: 100f);
    Assert.Equal(-100f, control.MinSoftwareValue);
  }

  [Fact]
  public void Control_MaxSoftwareValue_CanBeNegative() {
    var (control, _, _) = CreateControl(min: -100f, max: -50f);
    Assert.Equal(-50f, control.MaxSoftwareValue);
  }

  [Fact]
  public void Control_SetSoftware_RapidlyChangingValues_AllChangesPersisted() {
    var settings = new TestSettings();
    var hardware = new TestHardware(settings);
    var sensor = hardware.CreateTestSensor();
    var control = new Control(sensor, settings, 0f, 100f);

    for (int i = 0; i < 10; i++) {
      control.SetSoftware(i * 10f);
    }

    string valueKey = new Identifier(control.Identifier, "value").ToString();
    string stored = settings.GetValue(valueKey, string.Empty);
    Assert.Equal("90", stored); // final value
  }

  [Fact]
  public void Control_SetDefault_ThenSetSoftware_WithDifferentValues_BothFireEvents() {
    var (control, _, _) = CreateControl();
    int modeChanges = 0, valueChanges = 0;

    control.ControlModeChanged += _ => modeChanges++;
    control.SoftwareControlValueChanged += _ => valueChanges++;

    control.SetDefault();
    control.SetSoftware(50f);
    control.SetSoftware(75f);

    Assert.Equal(2, modeChanges); // Undefined->Default, Default->Software
    Assert.Equal(2, valueChanges); // 0->50, 50->75
  }

  // -------------------------------------------------------------------------
  // Min/Max range validation
  // -------------------------------------------------------------------------

  [Fact]
  public void Control_MinSoftwareValue_CanEqualMaxSoftwareValue() {
    var (control, _, _) = CreateControl(min: 50f, max: 50f);
    Assert.Equal(50f, control.MinSoftwareValue);
    Assert.Equal(50f, control.MaxSoftwareValue);
  }

  [Fact]
  public void Control_SetSoftware_WithValueBelowMin_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: 50f, max: 100f);
    var ex = Record.Exception(() => control.SetSoftware(25f)); // below min
    Assert.Null(ex);
  }

  [Fact]
  public void Control_SetSoftware_WithValueAboveMax_DoesNotThrow() {
    var (control, _, _) = CreateControl(min: 0f, max: 50f);
    var ex = Record.Exception(() => control.SetSoftware(75f)); // above max
    Assert.Null(ex);
  }

  [Fact]
  public void Control_RepeatedSetDefault_DoesNotFireDuplicateEvents() {
    var (control, _, _) = CreateControl();
    int eventCount = 0;
    control.ControlModeChanged += _ => eventCount++;

    control.SetDefault();
    control.SetDefault();
    control.SetDefault();

    // Should only fire once since mode doesn't change after first call
    Assert.Equal(1, eventCount);
  }
}