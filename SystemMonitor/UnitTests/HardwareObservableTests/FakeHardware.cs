using CrystalMonitor.Hardware;

namespace HardwareService.Tests;

public sealed class FakeHardware : IHardware {
  public string Name { get; set; } = "Fake CPU";
  public HardwareType HardwareType { get; init; } = HardwareType.Cpu;
  public ISensor[] Sensors { get; init; } = Array.Empty<ISensor>();
  public IHardware[] SubHardware { get; init; } = Array.Empty<IHardware>();
  public IHardware Parent => null!;
  public Identifier Identifier => new(HardwareType.ToString(), "0");
  public string GetReport() => string.Empty;
  public void Update() { }
  public IDictionary<string, string> Properties => new Dictionary<string, string>();
  public void Accept(IVisitor v) => v.VisitHardware(this);
  public void Traverse(IVisitor v) {
    foreach (var s in Sensors) s.Accept(v);
    foreach (var sub in SubHardware) sub.Accept(v);
  }
#pragma warning disable CS0067
  public event SensorEventHandler? SensorAdded;
  public event SensorEventHandler? SensorRemoved;
#pragma warning restore CS0067
}
