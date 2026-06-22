using System;
using System.Collections.Generic;
using CrystalMonitor.Hardware;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;
using DataStructures.Types;
using Prism.Ioc;
using Xunit;
using CpuQD = DataStructures.Cpu.Definitions.QueryDefinitions;
using MemQD = DataStructures.Memory.Definitions.QueryDefinitions;

namespace DataStructuresTests;

// =========================================================================
// DataStructureModule
// =========================================================================

/// <summary>
/// <see cref="DataStructures.DataStructureModule"/> is currently a no-op
/// Prism module stub. Both methods have empty bodies, so the only testable
/// behaviour is that they don't throw — but that's still enough to execute
/// the method bodies and push them from 0% to 100% covered.
/// </summary>
public class DataStructureModuleTests {
  [Fact]
  public void RegisterTypes_DoesNotThrow() {
    DataStructures.DataStructureModule module = new();
    module.RegisterTypes(null);   // no-op body; null is safe
  }

  [Fact]
  public void OnInitialized_DoesNotThrow() {
    DataStructures.DataStructureModule module = new();
    module.OnInitialized(null);   // no-op body; null is safe
  }
}

// =========================================================================
// UpdateVisitor
// =========================================================================

public class UpdateVisitorTests {
  [Fact]
  public void VisitComputer_CallsTraverseOnComputer() {
    UpdateVisitor visitor = new();
    StubComputer computer = new();

    visitor.VisitComputer(computer);

    Assert.True(computer.TraverseCalled);
    Assert.Same(visitor, computer.TraverseVisitor);
  }

  [Fact]
  public void VisitHardware_Null_ReturnsWithoutThrowing() {
    UpdateVisitor visitor = new();
    // The method guards with `if (hardware == null) return;`
    visitor.VisitHardware(null);   // must not throw
  }

  [Fact]
  public void VisitHardware_CallsUpdateAndAcceptsSubHardware() {
    UpdateVisitor visitor = new();
    StubHardware child = new();
    StubHardware parent = new(child);

    visitor.VisitHardware(parent);

    Assert.True(parent.UpdateCalled);
    Assert.True(child.AcceptCalled);
    Assert.Same(visitor, child.AcceptedVisitor);
  }

  [Fact]
  public void VisitHardware_NoSubHardware_OnlyCallsUpdate() {
    UpdateVisitor visitor = new();
    StubHardware hardware = new();   // zero children

    visitor.VisitHardware(hardware);

    Assert.True(hardware.UpdateCalled);
    Assert.False(hardware.AcceptCalled);
  }

  [Fact]
  public void VisitSensor_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitSensor(null);
  }

  [Fact]
  public void VisitParameter_DoesNotThrow() {
    UpdateVisitor visitor = new();
    visitor.VisitParameter(null);
  }

  // ------ stubs -----------------------------------------------------------

  private class StubComputer : IComputer {
    public bool TraverseCalled;
    public IVisitor TraverseVisitor;

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
    public event HardwareEventHandler HardwareAdded;
    public event HardwareEventHandler HardwareRemoved;
    public void Accept(IVisitor visitor) { }
    public void Traverse(IVisitor visitor) { TraverseCalled = true; TraverseVisitor = visitor; }
    public string GetReport() => string.Empty;
  }

  private class StubHardware : IHardware {
    public bool UpdateCalled;
    public bool AcceptCalled;
    public IVisitor AcceptedVisitor;
    private readonly IHardware[] _sub;

    public StubHardware(params IHardware[] sub) { _sub = sub; }

    public void Update() => UpdateCalled = true;
    public void Accept(IVisitor v) { AcceptCalled = true; AcceptedVisitor = v; }
    public void Traverse(IVisitor v) { }
    public IHardware[] SubHardware => _sub;

    // unused interface members
    public HardwareType HardwareType => HardwareType.Cpu;
    public Identifier Identifier => null;
    public string Name { get; set; } = "stub";
    public IHardware Parent => null;
    public ISensor[] Sensors => new ISensor[0];
    public IDictionary<string, string> Properties => new Dictionary<string, string>();
    public string GetReport() => string.Empty;
    public event SensorEventHandler SensorAdded;
    public event SensorEventHandler SensorRemoved;
  }
}

// =========================================================================
// Cpu.Definitions.QueryDefinitions
// =========================================================================

/// <summary>
/// The class consists solely of public static string fields initialised to
/// literal values. The constructor executes those initialisers, so
/// constructing (or reading) any member brings all 10 lines to covered.
/// </summary>
public class CpuQueryDefinitionsTests {
  [Fact]
  public void AllFields_HaveExpectedValues() {
    Assert.Equal("Bus Speed", CpuQD.CpuBusSpeed);
    Assert.Equal("CPU Total", CpuQD.CpuTotalLoad);
    Assert.Equal("CPU Core Max", CpuQD.CpuCoreMaxLoad);
    Assert.Equal("CPU Core", CpuQD.CpuCore);
    Assert.Equal("CPU Platform", CpuQD.CpuPlatform);
    Assert.Equal("CPU Package", CpuQD.CpuPackage);
    Assert.Equal("CPU Cores", CpuQD.CpuCores);
    Assert.Equal("CPU Memory", CpuQD.CPUMemory);
    Assert.Equal("Core Max", CpuQD.CoreMax);
    Assert.Equal("Core Average", CpuQD.CoreAverage);
  }

  [Fact]
  public void AllFields_AreNonNullAndNonEmpty() {
    var fields = new[] {
      CpuQD.CpuBusSpeed,   CpuQD.CpuTotalLoad,
      CpuQD.CpuCoreMaxLoad, CpuQD.CpuCore,
      CpuQD.CpuPlatform,   CpuQD.CpuPackage,
      CpuQD.CpuCores,      CpuQD.CPUMemory,
      CpuQD.CoreMax,        CpuQD.CoreAverage
    };
    foreach (string field in fields) {
      Assert.NotNull(field);
      Assert.NotEqual(string.Empty, field);
    }
  }
}

// =========================================================================
// Memory.Definitions.QueryDefinitions
// =========================================================================

public class MemoryQueryDefinitionsTests {
  [Fact]
  public void AllFields_HaveExpectedValues() {
    Assert.Equal("Memory Used", MemQD.MemoryUsed);
    Assert.Equal("Memory Available", MemQD.MemoryAvailable);
    Assert.Equal("Memory Load", MemQD.MemoryLoad);
    Assert.Equal("Memory Total", MemQD.MemoryTotal);
    Assert.Equal("Memory Used Percentage", MemQD.MemoryUsedPercentage);
  }

  [Fact]
  public void AllFields_AreNonNullAndNonEmpty() {
    var fields = new[] {
      MemQD.MemoryUsed,
      MemQD.MemoryAvailable,
      MemQD.MemoryLoad,
      MemQD.MemoryTotal,
      MemQD.MemoryUsedPercentage
    };
    foreach (string field in fields) {
      Assert.NotNull(field);
      Assert.NotEqual(string.Empty, field);
    }
  }
}

// =========================================================================
// CpuCoreLiveInfo — default-constructed state
// =========================================================================

public class CpuCoreLiveInfoTests {
  [Fact]
  public void DefaultConstructor_SetsNameToEmptyString() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(string.Empty, info.Name);
  }

  [Fact]
  public void DefaultConstructor_InitialisesVoltageReadingWithCpuHardwareTypeAndVoltageType() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(HardwareType.Cpu, info.Voltage.HardwareType);
    Assert.Equal(SensorType.Voltage, info.Voltage.SensorType);
  }

  [Fact]
  public void DefaultConstructor_InitialisesSpeedReadingWithClockType() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(SensorType.Clock, info.Speed.SensorType);
    Assert.Equal(HardwareType.Cpu, info.Speed.HardwareType);
  }

  [Fact]
  public void DefaultConstructor_InitialisesTemperatureReadingWithTemperatureType() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(SensorType.Temperature, info.Temperature.SensorType);
  }

  [Fact]
  public void DefaultConstructor_InitialisesLoadReadingWithLoadType() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(SensorType.Load, info.Load.SensorType);
  }

  [Fact]
  public void DefaultConstructor_AllReadingValuesAreZero() {
    CpuCoreLiveInfo info = new();
    Assert.Equal(0.0f, info.Voltage.Value);
    Assert.Equal(0.0f, info.Speed.Value);
    Assert.Equal(0.0f, info.Temperature.Value);
    Assert.Equal(0.0f, info.Load.Value);
  }

  [Fact]
  public void Properties_AreSettable() {
    CpuCoreLiveInfo info = new();
    SensorReading newSpeed = new("CPU 0", HardwareType.Cpu, "Core #0", SensorType.Clock, 3600f, 800f, 4200f, "MHz");

    info.Name = "Core #0";
    info.Speed = newSpeed;

    Assert.Equal("Core #0", info.Name);
    Assert.Equal(3600f, info.Speed.Value);
  }

  [Fact]
  public void ImplementsICpuCoreLiveInfo() {
    ICpuCoreLiveInfo info = new CpuCoreLiveInfo();
    Assert.NotNull(info);
  }
}

// =========================================================================
// CpuLiveInfo — default-constructed state
// =========================================================================

public class CpuLiveInfoTests {
  [Fact]
  public void DefaultConstructor_InitialisesNonNullCpuOverallLiveInfo() {
    CpuLiveInfo info = new();
    Assert.NotNull(info.CpuOverallLiveInfo);
    Assert.IsType<CpuOverallLiveInfo>(info.CpuOverallLiveInfo);
  }

  [Fact]
  public void DefaultConstructor_InitialisesEmptyCoreList() {
    CpuLiveInfo info = new();
    Assert.NotNull(info.CpuCoreLiveInfo);
    Assert.Empty(info.CpuCoreLiveInfo);
  }

  [Fact]
  public void DefaultConstructor_InitialisesNonNullOsLiveInfo() {
    CpuLiveInfo info = new();
    Assert.NotNull(info.OsLiveInfo);
    Assert.IsType<OSLiveInfo>(info.OsLiveInfo);
  }

  [Fact]
  public void CpuCoreLiveInfo_CanAddCores() {
    CpuLiveInfo info = new();
    info.CpuCoreLiveInfo.Add(new CpuCoreLiveInfo { Name = "Core #0" });
    info.CpuCoreLiveInfo.Add(new CpuCoreLiveInfo { Name = "Core #1" });

    Assert.Equal(2, info.CpuCoreLiveInfo.Count);
    Assert.Equal("Core #0", info.CpuCoreLiveInfo[0].Name);
  }

  [Fact]
  public void Properties_AreSettable() {
    CpuLiveInfo info = new();
    OSLiveInfo newOs = new() { ProcessNum = 42 };
    info.OsLiveInfo = newOs;
    Assert.Equal(42, info.OsLiveInfo.ProcessNum);
  }

  [Fact]
  public void ImplementsICpuLiveInfo() {
    ICpuLiveInfo info = new CpuLiveInfo();
    Assert.NotNull(info);
  }
}
