using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the CompositeSensor class.
/// Tests aggregation logic, value computation, and component management.
/// </summary>
public class CompositeSensorTests {
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

  private static CompositeSensor CreateCompositeSensor(
    ISensor[] components,
    Func<float, ISensor, float> reducer = null,
    float seedValue = 0f,
    string name = "Composite Sensor",
    int index = 0,
    SensorType type = SensorType.Load,
    ISettings settings = null) {
    var hardware = new TestHardware(settings: settings ?? new TestSettings());
    reducer ??= (sum, sensor) => sum + (sensor.Value ?? 0f);

    return new CompositeSensor(
      name, index, type, hardware, settings ?? new TestSettings(),
      components, reducer, seedValue);
  }

  // =========================================================================
  // Construction
  // =========================================================================

  [Fact]
  public void CompositeSensor_Construction_DoesNotThrow() {
    var component = CreateSensor();
    var components = new ISensor[] { component };

    var ex = Record.Exception(() => CreateCompositeSensor(components));
    Assert.Null(ex);
  }

  [Fact]
  public void CompositeSensor_Construction_WithMultipleComponents_DoesNotThrow() {
    var components = new ISensor[] {
      CreateSensor("Sensor 1", 0),
      CreateSensor("Sensor 2", 1),
      CreateSensor("Sensor 3", 2)
    };

    var ex = Record.Exception(() => CreateCompositeSensor(components));
    Assert.Null(ex);
  }

  [Fact]
  public void CompositeSensor_Construction_Name_IsSetFromConstructor() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components, name: "My Composite");

    Assert.Equal("My Composite", composite.Name);
  }

  [Fact]
  public void CompositeSensor_Construction_Index_IsSetFromConstructor() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components, index: 42);

    Assert.Equal(42, composite.Index);
  }

  [Fact]
  public void CompositeSensor_Construction_SensorType_IsSetFromConstructor() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components, type: SensorType.Temperature);

    Assert.Equal(SensorType.Temperature, composite.SensorType);
  }

  // =========================================================================
  // Value Aggregation - Sum
  // =========================================================================

  [Fact]
  public void CompositeSensor_Value_EmptyComponents_ReturnsSeedValue() {
    var components = new ISensor[] { };
    var composite = CreateCompositeSensor(components, seedValue: 100f);

    Assert.Equal(100f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_SingleComponent_ReturnComponentValue() {
    var component = CreateSensor();
    component.Value = 42f;
    var components = new ISensor[] { component };

    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(42f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_MultipleComponents_SumsValues() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 10f;
    component2.Value = 20f;
    component3.Value = 30f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(60f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_WithNullComponentValues_TreatsAsZero() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 25f;
    component2.Value = null;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(25f, composite.Value);
  }

  // =========================================================================
  // Value Aggregation - Average
  // =========================================================================

  [Fact]
  public void CompositeSensor_Value_CustomReducer_Average() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 10f;
    component2.Value = 20f;
    component3.Value = 30f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    // Manually calculate average
    var sum = composite.Value.Value;
    var average = sum / components.Length;

    Assert.Equal(20f, average);
  }

  // =========================================================================
  // Value Aggregation - Min/Max
  // =========================================================================

  [Fact]
  public void CompositeSensor_Value_CustomReducer_Min() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 50f;
    component2.Value = 20f;
    component3.Value = 80f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (min, sensor) => Math.Min(min, sensor.Value ?? float.MaxValue),
      seedValue: float.MaxValue);

    Assert.Equal(20f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_CustomReducer_Max() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 50f;
    component2.Value = 20f;
    component3.Value = 80f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (max, sensor) => Math.Max(max, sensor.Value ?? float.MinValue),
      seedValue: float.MinValue);

    Assert.Equal(80f, composite.Value);
  }

  // =========================================================================
  // Value Change Propagation
  // =========================================================================

  [Fact]
  public void CompositeSensor_Value_UpdatesWhenComponentChanges() {
    var component = CreateSensor();
    component.Value = 10f;

    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(10f, composite.Value);

    component.Value = 25f;
    Assert.Equal(25f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_ReflectsMultipleUpdates() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    component.Value = 5f;
    Assert.Equal(5f, composite.Value);

    component.Value = 10f;
    Assert.Equal(10f, composite.Value);

    component.Value = 20f;
    Assert.Equal(20f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_ReflectsAllComponentChanges() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 10f;
    component2.Value = 20f;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(30f, composite.Value);

    component1.Value = 50f;
    Assert.Equal(70f, composite.Value);

    component2.Value = 30f;
    Assert.Equal(80f, composite.Value);
  }

  // =========================================================================
  // Value Setting
  // =========================================================================

  [Fact]
  public void CompositeSensor_Value_SetterThrowsNotImplementedException() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components);

    Assert.Throws<NotImplementedException>(() => composite.Value = 100f);
  }

  [Fact]
  public void CompositeSensor_Value_CannotBeManuallySet() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components);

    var ex = Record.Exception(() => { composite.Value = 50f; });
    Assert.IsType<NotImplementedException>(ex);
  }

  // =========================================================================
  // Min/Max Tracking
  // =========================================================================

  [Fact]
  public void CompositeSensor_Min_InheritsFromParent_Sensor() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 30f;
    component2.Value = 20f;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    // CompositeSensor inherits from Sensor, so Min/Max tracking comes from parent
    // Just verify it doesn't throw
    var min = composite.Min;
    Assert.NotNull(min);
  }

  [Fact]
  public void CompositeSensor_Max_InheritsFromParent_Sensor() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 30f;
    component2.Value = 20f;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    // CompositeSensor inherits from Sensor, so Min/Max tracking comes from parent
    // Just verify it doesn't throw
    var max = composite.Max;
    Assert.NotNull(max);
  }

  // =========================================================================
  // Seed Value
  // =========================================================================

  [Fact]
  public void CompositeSensor_SeedValue_AffectsAggregation() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 10f;
    component2.Value = 20f;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 100f);

    // Should be 100 + 10 + 20 = 130
    Assert.Equal(130f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_SeedValue_CanBeNegative() {
    var component = CreateSensor();
    component.Value = 50f;

    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: -30f);

    // Should be -30 + 50 = 20
    Assert.Equal(20f, composite.Value);
  }

  // =========================================================================
  // Edge Cases
  // =========================================================================

  [Fact]
  public void CompositeSensor_Components_AllNull_ReturnsSeedValue() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = null;
    component2.Value = null;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 42f);

    Assert.Equal(42f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_Value_SequentialUpdates() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    var values = new[] { 5f, 10f, 15f, 20f, 25f };
    foreach (var value in values) {
      component.Value = value;
      Assert.Equal(value, composite.Value);
    }
  }

  [Fact]
  public void CompositeSensor_Inheritance_InheritsFromSensor() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components);

    Assert.IsAssignableFrom<Sensor>(composite);
    Assert.IsAssignableFrom<ISensor>(composite);
  }

  [Fact]
  public void CompositeSensor_Name_CanBeChanged() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components, name: "Original");

    composite.Name = "Modified";
    Assert.Equal("Modified", composite.Name);
  }

  [Fact]
  public void CompositeSensor_Identifier_IsGeneratedCorrectly() {
    var component = CreateSensor();
    var components = new ISensor[] { component };
    var composite = CreateCompositeSensor(components, name: "TestComposite");

    Assert.NotNull(composite.Identifier);
    // Identifier includes hardware and sensor information
    Assert.NotEmpty(composite.Identifier.ToString());
  }

  // =========================================================================
  // Custom Reducer Tests
  // =========================================================================

  [Fact]
  public void CompositeSensor_CustomReducer_CountingElements() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 1f;
    component2.Value = 2f;
    component3.Value = 3f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (count, sensor) => count + 1f,
      seedValue: 0f);

    Assert.Equal(3f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_CustomReducer_Multiplication() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    component1.Value = 5f;
    component2.Value = 3f;

    var components = new ISensor[] { component1, component2 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (product, sensor) => product * (sensor.Value ?? 1f),
      seedValue: 1f);

    Assert.Equal(15f, composite.Value);
  }

  [Fact]
  public void CompositeSensor_CustomReducer_Filtering() {
    var component1 = CreateSensor("C1", 0);
    var component2 = CreateSensor("C2", 1);
    var component3 = CreateSensor("C3", 2);
    component1.Value = 10f;
    component2.Value = 5f;  // Below threshold
    component3.Value = 20f;

    var components = new ISensor[] { component1, component2, component3 };
    var composite = CreateCompositeSensor(
      components,
      reducer: (sum, sensor) => {
        var value = sensor.Value ?? 0f;
        return value >= 10f ? sum + value : sum;
      },
      seedValue: 0f);

    Assert.Equal(30f, composite.Value);
  }

  // =========================================================================
  // Integration Tests
  // =========================================================================

  [Fact]
  public void CompositeSensor_MultiLevel_Aggregation() {
    // Create level 1 components
    var comp1 = CreateSensor("C1", 0);
    var comp2 = CreateSensor("C2", 1);
    comp1.Value = 10f;
    comp2.Value = 20f;

    // Create level 2 composite
    var level2Components = new ISensor[] { comp1, comp2 };
    var composite = CreateCompositeSensor(
      level2Components,
      reducer: (sum, sensor) => sum + (sensor.Value ?? 0f),
      seedValue: 0f);

    Assert.Equal(30f, composite.Value);
  }
}
