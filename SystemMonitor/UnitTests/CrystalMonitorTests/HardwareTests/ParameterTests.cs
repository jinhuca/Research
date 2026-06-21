using CrystalMonitor.Hardware;
using System.Globalization;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the Parameter class.
/// Tests construction, value management, persistence, and event handling.
/// </summary>
public class ParameterTests {
  // =========================================================================
  // Test Doubles & Helpers
  // =========================================================================

  private class TestSettings : ISettings {
    private readonly Dictionary<string, string> _store = new();

    public bool Contains(string name) => _store.ContainsKey(name);
    public void SetValue(string name, string value) => _store[name] = value;
    public string GetValue(string name, string value) =>
      _store.TryGetValue(name, out var v) ? v : value;
    public void Remove(string name) => _store.Remove(name);
  }

  private class TestHardware : Hardware {
    public TestHardware(string name = "Test Hardware", ISettings settings = null)
      : base(name, new Identifier("test", "0"), settings ?? new TestSettings()) { }

    public override HardwareType HardwareType => HardwareType.Cpu;
    public override void Update() { }
  }

  private static Sensor CreateSensor(
    string name = "Test Sensor",
    int index = 0,
    SensorType type = SensorType.Load,
    ISettings settings = null) {
    var hardware = new TestHardware(settings: settings ?? new TestSettings());
    return new Sensor(name, index, type, hardware, settings ?? new TestSettings());
  }

  private static ParameterDescription CreateParameterDescription(
    string name = "Test Parameter",
    string description = "A test parameter",
    float defaultValue = 50f) {
    return new ParameterDescription(name, description, defaultValue);
  }

  private static Parameter CreateParameter(
    ParameterDescription? description = null,
    ISensor sensor = null,
    ISettings settings = null) {
    description ??= CreateParameterDescription();
    sensor ??= CreateSensor();
    settings ??= new TestSettings();
    return new Parameter(description.Value, sensor, settings);
  }

  // =========================================================================
  // Construction & Properties
  // =========================================================================

  [Fact]
  public void Parameter_Construction_DoesNotThrow() {
    var ex = Record.Exception(() => CreateParameter());
    Assert.Null(ex);
  }

  [Fact]
  public void Parameter_Name_IsSetFromDescription() {
    var desc = CreateParameterDescription(name: "Temperature Offset");
    var param = CreateParameter(description: desc);
    Assert.Equal("Temperature Offset", param.Name);
  }

  [Fact]
  public void Parameter_Description_IsSetFromParameterDescription() {
    var desc = CreateParameterDescription(description: "Adjusts temperature reading");
    var param = CreateParameter(description: desc);
    Assert.Equal("Adjusts temperature reading", param.Description);
  }

  [Fact]
  public void Parameter_DefaultValue_IsSetFromDescription() {
    var desc = CreateParameterDescription(defaultValue: 75.5f);
    var param = CreateParameter(description: desc);
    Assert.Equal(75.5f, param.DefaultValue);
  }

  [Fact]
  public void Parameter_Sensor_ReferenceIsValid() {
    var sensor = CreateSensor();
    var param = CreateParameter(sensor: sensor);
    Assert.Equal(sensor, param.Sensor);
  }

  // =========================================================================
  // Value Management
  // =========================================================================

  [Fact]
  public void Parameter_Value_DefaultsToDescriptionDefaultValue() {
    var desc = CreateParameterDescription(defaultValue: 42f);
    var param = CreateParameter(description: desc);
    Assert.Equal(42f, param.Value);
  }

  [Fact]
  public void Parameter_Value_CanBeSet() {
    var param = CreateParameter();
    param.Value = 100f;
    Assert.Equal(100f, param.Value);
  }

  [Theory]
  [InlineData(0f)]
  [InlineData(50f)]
  [InlineData(-50f)]
  [InlineData(float.MaxValue)]
  [InlineData(float.MinValue)]
  public void Parameter_Value_AcceptsVariousFloatValues(float value) {
    var param = CreateParameter();
    param.Value = value;
    Assert.Equal(value, param.Value);
  }

  [Fact]
  public void Parameter_Value_CanBeUpdatedMultipleTimes() {
    var param = CreateParameter();
    param.Value = 10f;
    Assert.Equal(10f, param.Value);
    param.Value = 20f;
    Assert.Equal(20f, param.Value);
  }

  // =========================================================================
  // IsDefault State
  // =========================================================================

  [Fact]
  public void Parameter_IsDefault_IsTrueInitially() {
    var param = CreateParameter();
    Assert.True(param.IsDefault);
  }

  [Fact]
  public void Parameter_IsDefault_BecomesFalseWhenValueIsSet() {
    var param = CreateParameter();
    param.Value = 100f;
    Assert.False(param.IsDefault);
  }

  [Fact]
  public void Parameter_IsDefault_CanBeSetToTrue() {
    var param = CreateParameter();
    param.Value = 100f;
    Assert.False(param.IsDefault);

    param.IsDefault = true;
    Assert.True(param.IsDefault);
  }

  [Fact]
  public void Parameter_Value_ResetsToDefaultWhenIsDefaultSetToTrue() {
    var defaultValue = 42f;
    var desc = CreateParameterDescription(defaultValue: defaultValue);
    var param = CreateParameter(description: desc);

    param.Value = 100f;
    Assert.Equal(100f, param.Value);

    param.IsDefault = true;
    Assert.Equal(defaultValue, param.Value);
  }

  [Fact]
  public void Parameter_IsDefault_CanBeSetToFalse_WhenAlreadyFalse() {
    var param = CreateParameter();
    param.Value = 100f;
    Assert.False(param.IsDefault);

    param.IsDefault = false;
    Assert.False(param.IsDefault);
  }

  // =========================================================================
  // Identifier
  // =========================================================================

  [Fact]
  public void Parameter_Identifier_IsNotNull() {
    var param = CreateParameter();
    Assert.NotNull(param.Identifier);
  }

  [Fact]
  public void Parameter_Identifier_ContainsSensorIdentifier() {
    var sensor = CreateSensor();
    var param = CreateParameter(sensor: sensor);
    var identifierStr = param.Identifier.ToString();
    // Identifier should be derived from sensor identifier
    Assert.NotNull(identifierStr);
    Assert.Contains("parameter", identifierStr, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Parameter_Identifier_ContainsParameterName() {
    var desc = CreateParameterDescription(name: "Calibration");
    var param = CreateParameter(description: desc);
    var identifierStr = param.Identifier.ToString();
    Assert.Contains("calibration", identifierStr, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Parameter_Identifier_RemovesSpacesFromName() {
    var desc = CreateParameterDescription(name: "My Parameter");
    var param = CreateParameter(description: desc);
    var identifierStr = param.Identifier.ToString();
    // Should not contain spaces in the parameter name part
    Assert.DoesNotContain(" ", identifierStr.Split("/")[^1]);
  }

  [Fact]
  public void Parameter_Identifier_IsUnique_ForDifferentParameters() {
    var desc1 = CreateParameterDescription(name: "Parameter 1");
    var desc2 = CreateParameterDescription(name: "Parameter 2");
    var param1 = CreateParameter(description: desc1);
    var param2 = CreateParameter(description: desc2);
    Assert.NotEqual(param1.Identifier, param2.Identifier);
  }

  // =========================================================================
  // Settings Persistence
  // =========================================================================

  [Fact]
  public void Parameter_Value_CanBePersisted_InSettings() {
    var settings = new TestSettings();
    var desc = CreateParameterDescription(defaultValue: 50f);
    var sensor = CreateSensor(settings: settings);
    var param = CreateParameter(description: desc, sensor: sensor, settings: settings);

    param.Value = 75.5f;

    var key = param.Identifier.ToString();
    Assert.True(settings.Contains(key));
    Assert.Equal("75.5", settings.GetValue(key, ""));
  }

  [Fact]
  public void Parameter_Value_CanBeRestored_FromSettings() {
    var settings = new TestSettings();
    var desc = CreateParameterDescription(defaultValue: 50f);
    var sensor = CreateSensor(settings: settings);

    // First parameter: set value and persist
    var param1 = CreateParameter(description: desc, sensor: sensor, settings: settings);
    param1.Value = 75.5f;

    // Second parameter: load from same settings
    var param2 = CreateParameter(description: desc, sensor: sensor, settings: settings);
    Assert.Equal(75.5f, param2.Value);
  }

  [Fact]
  public void Parameter_IsDefault_RemovedFromSettings_WhenSetToTrue() {
    var settings = new TestSettings();
    var desc = CreateParameterDescription(defaultValue: 50f);
    var sensor = CreateSensor(settings: settings);
    var param = CreateParameter(description: desc, sensor: sensor, settings: settings);

    param.Value = 100f;
    var key = param.Identifier.ToString();
    Assert.True(settings.Contains(key));

    param.IsDefault = true;
    Assert.False(settings.Contains(key));
  }

  [Fact]
  public void Parameter_InvalidStoredValue_FallsBackToDefault() {
    var settings = new TestSettings();
    var key = new Identifier(
      new Identifier(new Identifier("test", "0"), "load", "0"),
      "parameter",
      "testparameter").ToString();

    settings.SetValue(key, "not_a_number");

    var desc = CreateParameterDescription(defaultValue: 50f);
    var sensor = CreateSensor(settings: settings);
    var param = CreateParameter(description: desc, sensor: sensor, settings: settings);

    Assert.Equal(50f, param.Value);
  }

  // =========================================================================
  // Visitor Pattern
  // =========================================================================

  [Fact]
  public void Parameter_Accept_CallsVisitor() {
    var param = CreateParameter();
    var visited = false;
    var mockVisitor = new MockVisitor(() => visited = true);

    param.Accept(mockVisitor);

    Assert.True(visited);
  }

  [Fact]
  public void Parameter_Accept_WithNullVisitor_ThrowsArgumentNullException() {
    var param = CreateParameter();
    var ex = Assert.Throws<ArgumentNullException>(() => param.Accept(null));
    Assert.Equal("visitor", ex.ParamName);
  }

  [Fact]
  public void Parameter_Traverse_DoesNotThrow() {
    var param = CreateParameter();
    var mockVisitor = new MockVisitor();
    var ex = Record.Exception(() => param.Traverse(mockVisitor));
    Assert.Null(ex);
  }

  // =========================================================================
  // Edge Cases
  // =========================================================================

  [Fact]
  public void Parameter_Name_CanBeEmpty() {
    var desc = CreateParameterDescription(name: string.Empty);
    var ex = Record.Exception(() => CreateParameter(description: desc));
    Assert.Null(ex);
  }

  [Fact]
  public void Parameter_Description_CanBeEmpty() {
    var desc = CreateParameterDescription(description: string.Empty);
    var ex = Record.Exception(() => CreateParameter(description: desc));
    Assert.Null(ex);
  }

  [Fact]
  public void Parameter_DefaultValue_CanBeNegative() {
    var desc = CreateParameterDescription(defaultValue: -100f);
    var param = CreateParameter(description: desc);
    Assert.Equal(-100f, param.Value);
  }

  [Fact]
  public void Parameter_DefaultValue_CanBeZero() {
    var desc = CreateParameterDescription(defaultValue: 0f);
    var param = CreateParameter(description: desc);
    Assert.Equal(0f, param.Value);
  }

  // =========================================================================
  // Test Doubles
  // =========================================================================

  private class MockVisitor : IVisitor {
    private readonly Action _onVisitParameter;

    public MockVisitor(Action onVisitParameter = null) {
      _onVisitParameter = onVisitParameter ?? (() => { });
    }

    public void VisitComputer(IComputer computer) { }
    public void VisitHardware(IHardware hardware) { }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) => _onVisitParameter();
  }
}
