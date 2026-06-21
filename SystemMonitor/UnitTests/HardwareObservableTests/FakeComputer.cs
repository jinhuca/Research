using CrystalMonitor.Hardware;
using System.Reactive.Linq;

namespace HardwareService.Tests;

public sealed class FakeComputer : IComputer {
  public List<FakeHardware> FakeHardware { get; } = new();
  public IList<IHardware> Hardware => FakeHardware.Cast<IHardware>().ToList();

  public bool IsBatteryEnabled => true;
  public bool IsControllerEnabled => false;
  public bool IsCpuEnabled => true;
  public bool IsGpuEnabled => true;
  public bool IsPowerMonitorEnabled => false;
  public bool IsMemoryEnabled => true;
  public bool IsMotherboardEnabled => true;
  public bool IsNetworkEnabled => true;
  public bool IsPsuEnabled => false;
  public bool IsStorageEnabled => true;

  /// <summary>When set, Accept throws this instead of visiting — used to test OnError propagation.</summary>
  public Exception? ThrowOnAccept { get; set; }

  public string GetReport() => string.Empty;
  public void Accept(IVisitor v) {
    if (ThrowOnAccept is not null) throw ThrowOnAccept;
    v.VisitComputer(this);
  }
  public void Traverse(IVisitor v) { foreach (var hw in Hardware) hw.Accept(v); }

#pragma warning disable CS0067
  public event HardwareEventHandler? HardwareAdded;
  public event HardwareEventHandler? HardwareRemoved;
#pragma warning restore CS0067
}
