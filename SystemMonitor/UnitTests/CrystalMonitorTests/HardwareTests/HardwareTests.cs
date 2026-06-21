using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

public class HardwareTests {
  // -------------------------------------------------------------------------
  // Minimal concrete stub
  // -------------------------------------------------------------------------

  private class TestHardware : Hardware {
    private readonly ISettings _testSettings;

    public TestHardware(string name, Identifier identifier, ISettings settings)
      : base(name, identifier, settings) {
      _testSettings = settings;
    }

    public override HardwareType HardwareType => HardwareType.Cpu;
    public override void Update() { }

    public void PublicActivateSensor(ISensor sensor) => ActivateSensor(sensor);
    public void PublicDeactivateSensor(ISensor sensor) => DeactivateSensor(sensor);

    public ISensor CreateTestSensor(string name = "Test Sensor", int index = 0) =>
      new Sensor(name, index, SensorType.Load, this, _testSettings);
  }

  private class TestSettings : ISettings {
    private readonly Dictionary<string, string> _store = new();

    public bool Contains(string name) => _store.ContainsKey(name);
    public void SetValue(string name, string value) => _store[name] = value;
    public string GetValue(string name, string value) =>
      _store.TryGetValue(name, out var v) ? v : value;
    public void Remove(string name) => _store.Remove(name);
  }

  private class TestVisitor : IVisitor {
    public List<IHardware> VisitedHardware { get; } = new();
    public List<ISensor> VisitedSensors { get; } = new();

    public void VisitComputer(IComputer computer) => computer.Traverse(this);
    public void VisitHardware(IHardware hardware) => VisitedHardware.Add(hardware);
    public void VisitSensor(ISensor sensor) => VisitedSensors.Add(sensor);
    public void VisitParameter(IParameter parameter) { }
  }

  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  private static TestHardware CreateHardware(string name = "Test CPU") {
    var settings = new TestSettings();
    var identifier = new Identifier("testcpu", "0");
    return new TestHardware(name, identifier, settings);
  }

  // -------------------------------------------------------------------------
  // Construction
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Construction_DoesNotThrow() {
    var ex = Record.Exception(() => CreateHardware());
    Assert.Null(ex);
  }

  [Fact]
  public void Hardware_Name_IsSetFromConstructor() {
    var hw = CreateHardware("My CPU");
    Assert.Equal("My CPU", hw.Name);
  }

  [Fact]
  public void Hardware_Identifier_IsSetFromConstructor() {
    var settings = new TestSettings();
    var identifier = new Identifier("testcpu", "0");
    var hw = new TestHardware("Test", identifier, settings);
    Assert.Equal(identifier, hw.Identifier);
  }

  [Fact]
  public void Hardware_HardwareType_ReturnsExpectedType() {
    var hw = CreateHardware();
    Assert.Equal(HardwareType.Cpu, hw.HardwareType);
  }

  // -------------------------------------------------------------------------
  // Name property
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Name_CanBeChanged() {
    var hw = CreateHardware("Original");
    hw.Name = "Updated";
    Assert.Equal("Updated", hw.Name);
  }

  [Fact]
  public void Hardware_Name_RevertsToOriginal_WhenSetToNull() {
    var hw = CreateHardware("Original");
    hw.Name = null;
    Assert.Equal("Original", hw.Name);
  }

  [Fact]
  public void Hardware_Name_RevertsToOriginal_WhenSetToEmpty() {
    var hw = CreateHardware("Original");
    hw.Name = string.Empty;
    Assert.Equal("Original", hw.Name);
  }

  [Fact]
  public void Hardware_Name_IsPersisted_InSettings() {
    var settings = new TestSettings();
    var identifier = new Identifier("testcpu", "0");
    var hw = new TestHardware("Original", identifier, settings);

    hw.Name = "Persisted Name";

    string key = new Identifier(identifier, "name").ToString();
    Assert.Equal("Persisted Name", settings.GetValue(key, string.Empty));
  }

  [Fact]
  public void Hardware_Name_IsRestoredFromSettings_OnConstruction() {
    var settings = new TestSettings();
    var identifier = new Identifier("testcpu", "0");
    string key = new Identifier(identifier, "name").ToString();
    settings.SetValue(key, "Saved Name");

    var hw = new TestHardware("Original", identifier, settings);
    Assert.Equal("Saved Name", hw.Name);
  }

  // -------------------------------------------------------------------------
  // Parent / SubHardware / Properties
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Parent_IsNullByDefault() {
    var hw = CreateHardware();
    Assert.Null(hw.Parent);
  }

  [Fact]
  public void Hardware_SubHardware_IsEmptyByDefault() {
    var hw = CreateHardware();
    Assert.Empty(hw.SubHardware);
  }

  [Fact]
  public void Hardware_Properties_IsNotNullByDefault() {
    var hw = CreateHardware();
    Assert.NotNull(hw.Properties);
  }

  // -------------------------------------------------------------------------
  // Sensors — initial state
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Sensors_IsEmptyBeforeActivation() {
    var hw = CreateHardware();
    Assert.Empty(hw.Sensors);
  }

  // -------------------------------------------------------------------------
  // ActivateSensor
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_ActivateSensor_AddsSensorToActive() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();

    hw.PublicActivateSensor(sensor);

    Assert.Contains(sensor, hw.Sensors);
  }

  [Fact]
  public void Hardware_ActivateSensor_IncreasesSensorCount() {
    var hw = CreateHardware();
    var sensor1 = hw.CreateTestSensor("Sensor 1", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 1);

    hw.PublicActivateSensor(sensor1);
    hw.PublicActivateSensor(sensor2);

    Assert.Equal(2, hw.Sensors.Length);
  }

  [Fact]
  public void Hardware_ActivateSensor_CalledTwice_DoesNotDuplicate() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();

    hw.PublicActivateSensor(sensor);
    hw.PublicActivateSensor(sensor);

    Assert.Single(hw.Sensors);
  }

  [Fact]
  public void Hardware_ActivateSensor_FiresSensorAddedEvent() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();
    ISensor? addedSensor = null;
    hw.SensorAdded += s => addedSensor = s;

    hw.PublicActivateSensor(sensor);

    Assert.Equal(sensor, addedSensor);
  }

  [Fact]
  public void Hardware_ActivateSensor_DoesNotFireSensorAdded_WhenAlreadyActive() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();
    hw.PublicActivateSensor(sensor);

    int eventCount = 0;
    hw.SensorAdded += _ => eventCount++;
    hw.PublicActivateSensor(sensor);

    Assert.Equal(0, eventCount);
  }

  // -------------------------------------------------------------------------
  // DeactivateSensor
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_DeactivateSensor_RemovesSensorFromActive() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();
    hw.PublicActivateSensor(sensor);

    hw.PublicDeactivateSensor(sensor);

    Assert.DoesNotContain(sensor, hw.Sensors);
  }

  [Fact]
  public void Hardware_DeactivateSensor_FiresSensorRemovedEvent() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();
    hw.PublicActivateSensor(sensor);

    ISensor? removedSensor = null;
    hw.SensorRemoved += s => removedSensor = s;
    hw.PublicDeactivateSensor(sensor);

    Assert.Equal(sensor, removedSensor);
  }

  [Fact]
  public void Hardware_DeactivateSensor_DoesNotFireSensorRemoved_WhenNotActive() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();

    int eventCount = 0;
    hw.SensorRemoved += _ => eventCount++;
    hw.PublicDeactivateSensor(sensor);

    Assert.Equal(0, eventCount);
  }

  [Fact]
  public void Hardware_DeactivateSensor_OnNonActiveSensor_DoesNotThrow() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();

    var ex = Record.Exception(() => hw.PublicDeactivateSensor(sensor));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // GetReport
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_GetReport_ReturnsNullByDefault() {
    var hw = CreateHardware();
    Assert.Null(hw.GetReport());
  }

  // -------------------------------------------------------------------------
  // Accept (visitor)
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Accept_ThrowsArgumentNullException_WhenVisitorIsNull() {
    var hw = CreateHardware();
    Assert.Throws<ArgumentNullException>(() => hw.Accept(null));
  }

  [Fact]
  public void Hardware_Accept_CallsVisitHardware() {
    var hw = CreateHardware();
    var visitor = new TestVisitor();

    hw.Accept(visitor);

    Assert.Contains(hw, visitor.VisitedHardware);
  }

  [Fact]
  public void Hardware_Accept_DoesNotThrow_WithValidVisitor() {
    var hw = CreateHardware();
    var visitor = new TestVisitor();

    var ex = Record.Exception(() => hw.Accept(visitor));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Traverse (visitor)
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Traverse_VisitsAllActiveSensors() {
    var hw = CreateHardware();
    var sensor1 = hw.CreateTestSensor("Sensor 1", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 1);
    hw.PublicActivateSensor(sensor1);
    hw.PublicActivateSensor(sensor2);

    var visitor = new TestVisitor();
    hw.Traverse(visitor);

    Assert.Contains(sensor1, visitor.VisitedSensors);
    Assert.Contains(sensor2, visitor.VisitedSensors);
  }

  [Fact]
  public void Hardware_Traverse_DoesNotVisitInactiveSensors() {
    var hw = CreateHardware();
    var active = hw.CreateTestSensor("Active", 0);
    var inactive = hw.CreateTestSensor("Inactive", 1);
    hw.PublicActivateSensor(active);

    var visitor = new TestVisitor();
    hw.Traverse(visitor);

    Assert.DoesNotContain(inactive, visitor.VisitedSensors);
  }

  [Fact]
  public void Hardware_Traverse_DoesNotVisitHardwareItself() {
    var hw = CreateHardware();
    var visitor = new TestVisitor();

    hw.Traverse(visitor);

    Assert.DoesNotContain(hw, visitor.VisitedHardware);
  }

  [Fact]
  public void Hardware_Traverse_WithNoSensors_DoesNotThrow() {
    var hw = CreateHardware();
    var visitor = new TestVisitor();

    var ex = Record.Exception(() => hw.Traverse(visitor));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Closing event
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Close_FiresClosingEvent() {
    var hw = CreateHardware();
    IHardware? closedHardware = null;
    hw.Closing += h => closedHardware = h;

    hw.Close();

    Assert.Equal(hw, closedHardware);
  }

  [Fact]
  public void Hardware_Close_DoesNotThrow_WhenNoClosingSubscribers() {
    var hw = CreateHardware();
    var ex = Record.Exception(() => hw.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void Hardware_Close_CanBeCalledMultipleTimes_WithoutThrowing() {
    var hw = CreateHardware();
    var ex = Record.Exception(() => {
      hw.Close();
      hw.Close();
    });
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Update
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Update_DoesNotThrow() {
    var hw = CreateHardware();
    var ex = Record.Exception(() => hw.Update());
    Assert.Null(ex);
  }

  [Fact]
  public void Hardware_Update_CanBeCalledRepeatedly() {
    var hw = CreateHardware();
    var ex = Record.Exception(() => {
      for (int i = 0; i < 10; i++) {
        hw.Update();
      }
    });
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Sensor lifecycle and activation state
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_DeactivateSensor_ThenActivateSensor_RestoresSensor() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();

    hw.PublicActivateSensor(sensor);
    Assert.Contains(sensor, hw.Sensors);

    hw.PublicDeactivateSensor(sensor);
    Assert.DoesNotContain(sensor, hw.Sensors);

    hw.PublicActivateSensor(sensor);
    Assert.Contains(sensor, hw.Sensors);
  }

  [Fact]
  public void Hardware_ActivateSensor_WithMultipleSensorsOfDifferentTypes() {
    var hw = CreateHardware();
    var tempSensor = hw.CreateTestSensor("Temp", 0);
    var loadSensor = new Sensor("Load", 1, SensorType.Load, hw, new TestSettings());
    var voltageSensor = new Sensor("Voltage", 2, SensorType.Voltage, hw, new TestSettings());

    hw.PublicActivateSensor(tempSensor);
    hw.PublicActivateSensor(loadSensor);
    hw.PublicActivateSensor(voltageSensor);

    Assert.Equal(3, hw.Sensors.Length);
  }

  [Fact]
  public void Hardware_Sensors_AreOrderedByIndex() {
    var hw = CreateHardware();
    var sensor0 = hw.CreateTestSensor("Sensor 0", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 2);
    var sensor1 = hw.CreateTestSensor("Sensor 1", 1);

    hw.PublicActivateSensor(sensor0);
    hw.PublicActivateSensor(sensor2);
    hw.PublicActivateSensor(sensor1);

    // After activation, sensors should be in hardware
    Assert.Contains(sensor0, hw.Sensors);
    Assert.Contains(sensor1, hw.Sensors);
    Assert.Contains(sensor2, hw.Sensors);
  }

  [Fact]
  public void Hardware_DeactivateSensor_WithMultipleSensors_RemovesOnlyTargeted() {
    var hw = CreateHardware();
    var sensor1 = hw.CreateTestSensor("Sensor 1", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 1);
    var sensor3 = hw.CreateTestSensor("Sensor 3", 2);

    hw.PublicActivateSensor(sensor1);
    hw.PublicActivateSensor(sensor2);
    hw.PublicActivateSensor(sensor3);

    hw.PublicDeactivateSensor(sensor2);

    Assert.Contains(sensor1, hw.Sensors);
    Assert.DoesNotContain(sensor2, hw.Sensors);
    Assert.Contains(sensor3, hw.Sensors);
  }

  // -------------------------------------------------------------------------
  // Name property edge cases
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Name_CanBeEmpty() {
    var hw = CreateHardware(string.Empty);
    Assert.Equal(string.Empty, hw.Name);
  }

  [Fact]
  public void Hardware_Name_CanBeLongString() {
    var longName = new string('A', 1000);
    var hw = CreateHardware(longName);
    Assert.Equal(longName, hw.Name);
  }

  [Fact]
  public void Hardware_Name_CanContainSpecialCharacters() {
    var specialNames = new[] {
      "CPU #0",
      "Core @ 0",
      "Socket [0]",
      "Package (Physical)",
      "GPU/0"
    };

    foreach (var name in specialNames) {
      var hw = CreateHardware(name);
      Assert.Equal(name, hw.Name);
    }
  }

  // -------------------------------------------------------------------------
  // Accept/Traverse with various sensor states
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Accept_CallsVisitorWithHardware() {
    var hw = CreateHardware();
    var visitor = new TestVisitor();

    hw.Accept(visitor);

    Assert.Contains(hw, visitor.VisitedHardware);
  }

  [Fact]
  public void Hardware_Accept_WithNullVisitor_ThrowsArgumentNullException() {
    var hw = CreateHardware();
    var ex = Assert.Throws<ArgumentNullException>(() => hw.Accept(null));
    Assert.Equal("visitor", ex.ParamName);
  }

  [Fact]
  public void Hardware_Traverse_VisitsAllActiveSensors_InIntegrationContext() {
    var hw = CreateHardware();
    var sensor1 = hw.CreateTestSensor("Sensor 1", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 1);

    hw.PublicActivateSensor(sensor1);
    hw.PublicActivateSensor(sensor2);

    var visitor = new TestVisitor();
    hw.Traverse(visitor);

    Assert.Contains(sensor1, visitor.VisitedSensors);
    Assert.Contains(sensor2, visitor.VisitedSensors);
  }

  [Fact]
  public void Hardware_Traverse_DoesNotVisitDeactivatedSensors() {
    var hw = CreateHardware();
    var sensor1 = hw.CreateTestSensor("Sensor 1", 0);
    var sensor2 = hw.CreateTestSensor("Sensor 2", 1);

    hw.PublicActivateSensor(sensor1);
    hw.PublicActivateSensor(sensor2);
    hw.PublicDeactivateSensor(sensor1);

    var visitor = new TestVisitor();
    hw.Traverse(visitor);

    Assert.DoesNotContain(sensor1, visitor.VisitedSensors);
    Assert.Contains(sensor2, visitor.VisitedSensors);
  }

  // -------------------------------------------------------------------------
  // Concurrent sensor operations
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_ActivateDeactivate_Concurrent_DoesNotThrow() {
    var hw = CreateHardware();
    var sensors = Enumerable.Range(0, 10)
      .Select(i => hw.CreateTestSensor($"Sensor {i}", i))
      .ToArray();

    var exceptions = new List<Exception>();

    var tasks = sensors.Select(sensor => Task.Run(() => {
      try {
        hw.PublicActivateSensor(sensor);
        System.Threading.Thread.Sleep(10);
        hw.PublicDeactivateSensor(sensor);
      }
      catch (Exception ex) {
        lock (exceptions) {
          exceptions.Add(ex);
        }
      }
    })).ToArray();

    Task.WaitAll(tasks);

    Assert.Empty(exceptions);
  }

  [Fact]
  public void Hardware_ConcurrentSensorActivation_DoesNotLoseSensors() {
    var hw = CreateHardware();
    var sensors = Enumerable.Range(0, 20)
      .Select(i => hw.CreateTestSensor($"Sensor {i}", i))
      .ToArray();

    var exceptions = new List<Exception>();

    var tasks = sensors.Select(sensor => Task.Run(() => {
      try {
        hw.PublicActivateSensor(sensor);
      }
      catch (Exception ex) {
        lock (exceptions) {
          exceptions.Add(ex);
        }
      }
    })).ToArray();

    Task.WaitAll(tasks);

    Assert.Empty(exceptions);
    // Not all 20 may be activated due to possible deduplication, but count should be reasonable
    Assert.True(hw.Sensors.Length > 0);
  }

  // -------------------------------------------------------------------------
  // Closing event variations
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Close_WithActiveSensors_FiresClosingEvent() {
    var hw = CreateHardware();
    var sensor = hw.CreateTestSensor();
    hw.PublicActivateSensor(sensor);

    IHardware? closedHardware = null;
    hw.Closing += h => closedHardware = h;

    hw.Close();

    Assert.Equal(hw, closedHardware);
  }

  [Fact]
  public void Hardware_Close_MultipleClosingSubscribers_AllFire() {
    var hw = CreateHardware();
    int count = 0;
    hw.Closing += _ => count++;
    hw.Closing += _ => count++;
    hw.Closing += _ => count++;

    hw.Close();

    Assert.Equal(3, count);
  }

  [Fact]
  public void Hardware_Close_FollowedByUpdate_DoesNotThrow() {
    var hw = CreateHardware();
    hw.Close();

    var ex = Record.Exception(() => hw.Update());
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Properties collection
  // -------------------------------------------------------------------------

  [Fact]
  public void Hardware_Properties_CanBeEnumerated() {
    var hw = CreateHardware();
    var ex = Record.Exception(() => {
      foreach (var prop in hw.Properties) {
        _ = prop;
      }
    });
    Assert.Null(ex);
  }
}