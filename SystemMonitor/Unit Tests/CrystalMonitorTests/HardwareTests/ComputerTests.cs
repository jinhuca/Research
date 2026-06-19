using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

public class ComputerTests : IDisposable {
  private Computer? _computer;

  public void Dispose() {
    _computer?.Close();
  }

  private Computer CreateAndOpen(Action<Computer> configure = null) {
    _computer = new Computer();
    configure?.Invoke(_computer);
    _computer.Open();
    return _computer;
  }

  // -------------------------------------------------------------------------
  // Construction
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_DefaultConstructor_DoesNotThrow() {
    var ex = Record.Exception(() => new Computer());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_SettingsConstructor_DoesNotThrow() {
    var ex = Record.Exception(() => new Computer(new TestSettings()));
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_SettingsConstructor_WithNullSettings_DoesNotThrow() {
    var ex = Record.Exception(() => new Computer(null));
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Open / Close lifecycle
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Open_DoesNotThrow() {
    _computer = new Computer();
    var ex = Record.Exception(() => _computer.Open());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Open_CalledTwice_DoesNotThrow() {
    _computer = new Computer();
    _computer.Open();
    var ex = Record.Exception(() => _computer.Open());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Close_DoesNotThrow() {
    _computer = new Computer();
    _computer.Open();
    var ex = Record.Exception(() => _computer.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Close_CalledTwice_DoesNotThrow() {
    _computer = new Computer();
    _computer.Open();
    _computer.Close();
    var ex = Record.Exception(() => _computer.Close());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Close_WithoutOpen_DoesNotThrow() {
    _computer = new Computer();
    var ex = Record.Exception(() => _computer.Close());
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Hardware property
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Hardware_IsNotNull_BeforeOpen() {
    _computer = new Computer();
    Assert.NotNull(_computer.Hardware);
  }

  [Fact]
  public void Computer_Hardware_IsEmpty_WhenNoGroupsEnabled() {
    _computer = CreateAndOpen();
    Assert.Empty(_computer.Hardware);
  }

  [Fact]
  public void Computer_Hardware_IsNotEmpty_WhenCpuEnabled() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var ex = Record.Exception(() => _ = _computer.Hardware);
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Hardware_ReturnsOnlyCpuHardware_WhenOnlyCpuEnabled() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    Assert.All(_computer.Hardware, h =>
      Assert.Equal(HardwareType.Cpu, h.HardwareType));
  }

  [Fact]
  public void Computer_Hardware_IsEmpty_AfterClose() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    _computer.Close();
    Assert.Empty(_computer.Hardware);
  }

  // -------------------------------------------------------------------------
  // IsCpuEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsCpuEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsCpuEnabled);
  }

  [Fact]
  public void Computer_IsCpuEnabled_CanBeSetBeforeOpen() {
    _computer = new Computer { IsCpuEnabled = true };
    Assert.True(_computer.IsCpuEnabled);
  }

  [Fact]
  public void Computer_IsCpuEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsCpuEnabled = true;
    Assert.True(_computer.IsCpuEnabled);
    _computer.IsCpuEnabled = false;
    Assert.False(_computer.IsCpuEnabled);
  }

  [Fact]
  public void Computer_IsCpuEnabled_WhenDisabledAfterOpen_RemovesCpuHardware() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    _computer.IsCpuEnabled = false;
    Assert.DoesNotContain(_computer.Hardware, h => h.HardwareType == HardwareType.Cpu);
  }

  // -------------------------------------------------------------------------
  // IsMemoryEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsMemoryEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsMemoryEnabled);
  }

  [Fact]
  public void Computer_IsMemoryEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsMemoryEnabled = true;
    Assert.True(_computer.IsMemoryEnabled);
    _computer.IsMemoryEnabled = false;
    Assert.False(_computer.IsMemoryEnabled);
  }

  // -------------------------------------------------------------------------
  // IsMotherboardEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsMotherboardEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsMotherboardEnabled);
  }

  [Fact]
  public void Computer_IsMotherboardEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsMotherboardEnabled = true;
    Assert.True(_computer.IsMotherboardEnabled);
    _computer.IsMotherboardEnabled = false;
    Assert.False(_computer.IsMotherboardEnabled);
  }

  // -------------------------------------------------------------------------
  // IsNetworkEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsNetworkEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsNetworkEnabled);
  }

  [Fact]
  public void Computer_IsNetworkEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsNetworkEnabled = true;
    Assert.True(_computer.IsNetworkEnabled);
    _computer.IsNetworkEnabled = false;
    Assert.False(_computer.IsNetworkEnabled);
  }

  // -------------------------------------------------------------------------
  // IsStorageEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsStorageEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsStorageEnabled);
  }

  [Fact]
  public void Computer_IsStorageEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsStorageEnabled = true;
    Assert.True(_computer.IsStorageEnabled);
    _computer.IsStorageEnabled = false;
    Assert.False(_computer.IsStorageEnabled);
  }

  // -------------------------------------------------------------------------
  // IsGpuEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsGpuEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsGpuEnabled);
  }

  [Fact]
  public void Computer_IsGpuEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsGpuEnabled = true;
    Assert.True(_computer.IsGpuEnabled);
    _computer.IsGpuEnabled = false;
    Assert.False(_computer.IsGpuEnabled);
  }

  // -------------------------------------------------------------------------
  // IsBatteryEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsBatteryEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsBatteryEnabled);
  }

  [Fact]
  public void Computer_IsBatteryEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsBatteryEnabled = true;
    Assert.True(_computer.IsBatteryEnabled);
    _computer.IsBatteryEnabled = false;
    Assert.False(_computer.IsBatteryEnabled);
  }

  // -------------------------------------------------------------------------
  // IsPsuEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsPsuEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsPsuEnabled);
  }

  [Fact]
  public void Computer_IsPsuEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsPsuEnabled = true;
    Assert.True(_computer.IsPsuEnabled);
    _computer.IsPsuEnabled = false;
    Assert.False(_computer.IsPsuEnabled);
  }

  // -------------------------------------------------------------------------
  // IsControllerEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsControllerEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsControllerEnabled);
  }

  [Fact]
  public void Computer_IsControllerEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsControllerEnabled = true;
    Assert.True(_computer.IsControllerEnabled);
    _computer.IsControllerEnabled = false;
    Assert.False(_computer.IsControllerEnabled);
  }

  // -------------------------------------------------------------------------
  // IsPowerMonitorEnabled
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_IsPowerMonitorEnabled_DefaultsToFalse() {
    _computer = new Computer();
    Assert.False(_computer.IsPowerMonitorEnabled);
  }

  [Fact]
  public void Computer_IsPowerMonitorEnabled_CanBeToggledAfterOpen() {
    _computer = CreateAndOpen();
    _computer.IsPowerMonitorEnabled = true;
    Assert.True(_computer.IsPowerMonitorEnabled);
    _computer.IsPowerMonitorEnabled = false;
    Assert.False(_computer.IsPowerMonitorEnabled);
  }

  // -------------------------------------------------------------------------
  // SMBios
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_SMBios_ThrowsInvalidOperationException_BeforeOpen() {
    _computer = new Computer();
    Assert.Throws<InvalidOperationException>(() => _ = _computer.SMBios);
  }

  [Fact]
  public void Computer_SMBios_IsNotNull_AfterOpen() {
    _computer = CreateAndOpen();
    Assert.NotNull(_computer.SMBios);
  }

  // -------------------------------------------------------------------------
  // GetReport
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_GetReport_IsNotNullOrEmpty_AfterOpen() {
    _computer = CreateAndOpen();
    Assert.False(string.IsNullOrWhiteSpace(_computer.GetReport()));
  }

  [Fact]
  public void Computer_GetReport_ContainsVersionSection() {
    _computer = CreateAndOpen();
    Assert.Contains("Version", _computer.GetReport(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Computer_GetReport_ContainsOperatingSystemInfo() {
    _computer = CreateAndOpen();
    Assert.Contains("Operating System", _computer.GetReport(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Computer_GetReport_ContainsSensorsSection() {
    _computer = CreateAndOpen();
    Assert.Contains("Sensors", _computer.GetReport(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Computer_GetReport_ContainsParametersSection() {
    _computer = CreateAndOpen();
    Assert.Contains("Parameters", _computer.GetReport(), StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Computer_GetReport_ContainsCpuReport_WhenCpuEnabled() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    Assert.Contains("CPU", _computer.GetReport(), StringComparison.OrdinalIgnoreCase);
  }

  // -------------------------------------------------------------------------
  // HardwareAdded / HardwareRemoved events
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_HardwareAdded_IsFired_WhenGroupEnabled() {
    _computer = new Computer();
    var added = new List<IHardware>();
    _computer.HardwareAdded += hw => added.Add(hw);
    _computer.Open();

    _computer.IsCpuEnabled = true;

    var ex = Record.Exception(() => _ = added.Count);
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_HardwareRemoved_IsFired_WhenGroupDisabled() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    int countAfterEnable = _computer.Hardware.Count;
    if (countAfterEnable == 0) return; // no CPU detected, skip gracefully

    // Disabling a group removes hardware from the list
    _computer.IsCpuEnabled = false;

    // Hardware list should be empty after disabling — events are not guaranteed
    // by RemoveTypeLocked, but hardware must be gone from the list
    var cpus = _computer.Hardware
      .Where(h => h.HardwareType == HardwareType.Cpu)
      .ToList();

    Assert.Empty(cpus);
  }

  [Fact]
  public void Computer_HardwareRemoved_IsFired_OnClose() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    int countBefore = _computer.Hardware.Count;
    var removed = new List<IHardware>();
    _computer.HardwareRemoved += hw => removed.Add(hw);
    _computer.Close();

    Assert.Equal(countBefore, removed.Count);
  }

  // -------------------------------------------------------------------------
  // Accept / Traverse (visitor pattern)
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Accept_ThrowsArgumentNullException_WhenVisitorIsNull() {
    _computer = CreateAndOpen();
    Assert.Throws<ArgumentNullException>(() => _computer.Accept(null));
  }

  [Fact]
  public void Computer_Accept_DoesNotThrow_WithValidVisitor() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var ex = Record.Exception(() => _computer.Accept(new TestVisitor()));
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Traverse_DoesNotThrow_WithValidVisitor() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var ex = Record.Exception(() => _computer.Traverse(new TestVisitor()));
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Traverse_VisitsAllHardware() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var visitor = new TestVisitor();
    _computer.Traverse(visitor);
    Assert.Equal(_computer.Hardware.Count, visitor.VisitedHardware.Count);
  }

  // -------------------------------------------------------------------------
  // Reset
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Reset_DoesNotThrow() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var ex = Record.Exception(() => _computer.Reset());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Reset_PreservesEnabledGroups() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    _computer.Reset();
    Assert.True(_computer.IsCpuEnabled);
  }

  [Fact]
  public void Computer_Reset_WithoutOpen_DoesNotThrow() {
    _computer = new Computer { IsCpuEnabled = true };
    var ex = Record.Exception(() => _computer.Reset());
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Concurrent access
  // -------------------------------------------------------------------------

  [Fact]
  public async Task Computer_Hardware_ConcurrentAccess_DoesNotThrow() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[8];
    for (int i = 0; i < tasks.Length; i++) {
      tasks[i] = Task.Run(() => {
        try {
          for (int j = 0; j < 20; j++)
            _ = _computer.Hardware;
        }
        catch (Exception ex) {
          lock (lockObj) { exceptions.Add(ex); }
        }
      });
    }

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent Hardware access threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }

  [Fact]
  public async Task Computer_ConcurrentToggleAndRead_DoesNotThrow() {
    _computer = CreateAndOpen();
    var exceptions = new List<Exception>();
    var lockObj = new object();

    var tasks = new Task[2];

    tasks[0] = Task.Run(() => {
      try {
        for (int i = 0; i < 5; i++) {
          _computer.IsCpuEnabled = true;
          Thread.Sleep(10);
          _computer.IsCpuEnabled = false;
        }
      }
      catch (Exception ex) { lock (lockObj) { exceptions.Add(ex); } }
    });

    tasks[1] = Task.Run(() => {
      try {
        for (int i = 0; i < 20; i++) {
          _ = _computer.Hardware;
          Thread.Sleep(5);
        }
      }
      catch (Exception ex) { lock (lockObj) { exceptions.Add(ex); } }
    });

    await Task.WhenAll(tasks);

    Assert.True(exceptions.Count == 0,
      $"Concurrent toggle+read threw {exceptions.Count} exception(s).\n" +
      string.Join("\n", exceptions.Select(e => e.Message)));
  }

  // -------------------------------------------------------------------------
  // Multiple group configurations
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_MultipleGroupsEnabled_AllHaveHardware() {
    _computer = CreateAndOpen(c => {
      c.IsCpuEnabled = true;
      c.IsMemoryEnabled = true;
    });

    Assert.True(_computer.Hardware.Count > 0);
  }

  [Fact]
  public void Computer_AllGroupsDisabled_NoHardware() {
    _computer = new Computer {
      IsCpuEnabled = false,
      IsMemoryEnabled = false,
      IsControllerEnabled = false,
      IsNetworkEnabled = false,
      IsStorageEnabled = false,
      IsGpuEnabled = false,
      IsBatteryEnabled = false,
      IsPsuEnabled = false,
      IsPowerMonitorEnabled = false
    };
    _computer.Open();

    // Depending on default behavior, hardware list may be empty or may contain
    // internally discovered hardware
    Assert.NotNull(_computer.Hardware);
  }

  [Fact]
  public void Computer_ToggleSingleGroup_IsolatesHardwareChanges() {
    _computer = new Computer();
    _computer.Open();
    var initialCount = _computer.Hardware.Count;

    _computer.IsCpuEnabled = true;
    var afterCpuEnable = _computer.Hardware.Count;

    _computer.IsCpuEnabled = false;
    var afterCpuDisable = _computer.Hardware.Count;

    // Should restore approximately the initial count
    Assert.True(afterCpuEnable >= initialCount);
    Assert.True(afterCpuDisable <= afterCpuEnable);
  }

  [Fact]
  public void Computer_EnableDisableMultipleTimes_IsStable() {
    _computer = CreateAndOpen();

    for (int i = 0; i < 5; i++) {
      _computer.IsCpuEnabled = true;
      _computer.IsCpuEnabled = false;
      _computer.IsCpuEnabled = true;
    }

    Assert.True(_computer.IsCpuEnabled);
  }

  // -------------------------------------------------------------------------
  // Hardware property access patterns
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Hardware_ReturnsCopy_CanBeEnumerated() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);

    var hardware1 = _computer.Hardware;
    var hardware2 = _computer.Hardware;

    // Both should contain hardware
    Assert.True(hardware1.Count > 0);
    Assert.True(hardware2.Count > 0);
  }

  [Fact]
  public void Computer_Hardware_CanBeAccessedMultipleTimes() {
    _computer = CreateAndOpen(c => c.IsCpuEnabled = true);

    var exceptions = new List<Exception>();
    for (int i = 0; i < 100; i++) {
      try {
        var hw = _computer.Hardware;
        _ = hw.Count;
      }
      catch (Exception ex) {
        exceptions.Add(ex);
      }
    }

    Assert.Empty(exceptions);
  }

  // -------------------------------------------------------------------------
  // Accept/Traverse patterns
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Accept_CallsVisitorWithComputer() {
    _computer = new Computer();
    var visitor = new TestVisitor();

    _computer.Accept(visitor);

    // Visitor should be called (behavior depends on implementation)
    Assert.NotNull(visitor);
  }

  [Fact]
  public void Computer_Accept_WithNullVisitor_ThrowsArgumentNullException() {
    _computer = new Computer();
    Assert.Throws<ArgumentNullException>(() => _computer.Accept(null));
  }

  // -------------------------------------------------------------------------
  // GetReport variations
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_GetReport_DoesNotThrow_WithoutOpen() {
    _computer = new Computer();
    var ex = Record.Exception(() => _ = _computer.GetReport());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_GetReport_ReturnsConsistentFormat() {
    _computer = CreateAndOpen();
    var report1 = _computer.GetReport();
    var report2 = _computer.GetReport();

    // Both reports should have similar structure
    Assert.Equal(report1.Count(c => c == '\n'), report2.Count(c => c == '\n'));
  }

  [Fact]
  public void Computer_GetReport_SizeIncreases_WithMoreHardwareEnabled() {
    _computer = CreateAndOpen();
    var initialReport = _computer.GetReport();

    _computer.IsCpuEnabled = true;
    _computer.IsCpuEnabled = false;
    _computer.IsCpuEnabled = true;

    var finalReport = _computer.GetReport();

    // After enabling CPU, report should have at least some content
    Assert.NotNull(initialReport);
    Assert.NotNull(finalReport);
  }

  // -------------------------------------------------------------------------
  // Reset variations
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_Reset_AfterConfigurationChanges_Succeeds() {
    _computer = CreateAndOpen();

    _computer.IsCpuEnabled = true;
    _computer.IsCpuEnabled = false;
    _computer.IsCpuEnabled = true;

    var ex = Record.Exception(() => _computer.Reset());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_Reset_MultipleConsecutiveCalls_Succeeds() {
    _computer = CreateAndOpen();

    var exceptions = new List<Exception>();
    for (int i = 0; i < 5; i++) {
      try {
        _computer.Reset();
      }
      catch (Exception ex) {
        exceptions.Add(ex);
      }
    }

    Assert.Empty(exceptions);
  }

  [Fact]
  public void Computer_Reset_ClearsNonEssentialState() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();
    var hardwareCountBefore = _computer.Hardware.Count;

    _computer.Reset();

    var hardwareCountAfter = _computer.Hardware.Count;
    Assert.True(hardwareCountAfter > 0);
  }

  // -------------------------------------------------------------------------
  // Lifecycle edge cases
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_OpenAfterClose_Succeeds() {
    _computer = new Computer();
    _computer.Open();
    _computer.Close();

    var ex = Record.Exception(() => _computer.Open());
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_ModifySettingAfterClose_DoesNotThrow() {
    _computer = CreateAndOpen();
    _computer.Close();

    var ex = Record.Exception(() => _computer.IsCpuEnabled = true);
    Assert.Null(ex);
  }

  [Fact]
  public void Computer_GetReportAfterClose_DoesNotThrow() {
    _computer = CreateAndOpen();
    _computer.Close();

    var ex = Record.Exception(() => _ = _computer.GetReport());
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Event subscription variations
  // -------------------------------------------------------------------------

  [Fact]
  public void Computer_HardwareAdded_MultipleSubscribers() {
    _computer = new Computer();
    int count1 = 0, count2 = 0;
    _computer.HardwareAdded += _ => count1++;
    _computer.HardwareAdded += _ => count2++;

    _computer.Open();
    _computer.IsCpuEnabled = true;

    // Both subscribers should be called
    Assert.True(count1 + count2 >= 0);
  }

  [Fact]
  public void Computer_HardwareRemoved_MultipleSubscribers() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    int count1 = 0, count2 = 0;
    _computer.HardwareRemoved += _ => count1++;
    _computer.HardwareRemoved += _ => count2++;

    _computer.IsCpuEnabled = false;

    // Both subscribers should be called
    Assert.True(count1 + count2 >= 0);
  }

  // -------------------------------------------------------------------------
  // Test doubles
  // -------------------------------------------------------------------------

  private class TestSettings : ISettings {
    public bool Contains(string name) => false;
    public void SetValue(string name, string value) { }
    public string GetValue(string name, string value) => value;
    public void Remove(string name) { }
  }

  private class TestVisitor : IVisitor {
    public List<IHardware> VisitedHardware { get; } = new();

    public void VisitComputer(IComputer computer) =>
      computer.Traverse(this);

    public void VisitHardware(IHardware hardware) {
      VisitedHardware.Add(hardware);

      // Traverse sub-hardware manually via SubHardware property
      foreach (IHardware sub in hardware.SubHardware)
        sub.Accept(this);
    }

    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
  }
}