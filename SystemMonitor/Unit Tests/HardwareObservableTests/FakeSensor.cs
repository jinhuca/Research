using CrystalMonitor.Hardware;

namespace HardwareService.Tests;

// ── Fakes ─────────────────────────────────────────────────────────────────────

public sealed class FakeSensor : ISensor {
  public string Name { get; set; } = "Fake Sensor";
  public SensorType SensorType { get; set; } = SensorType.Temperature;
  public float? Value { get; set; } = 42f;
  public float? Min => Value;
  public float? Max => Value;
  public int Index => 0;
  public bool IsDefaultHidden => false;
  public IHardware Hardware => null!;
  public IHardware? Parent => null;
  public Identifier Identifier => new(SensorType.ToString(), "0");
  public IReadOnlyList<IParameter> Parameters => Array.Empty<IParameter>();
  public IEnumerable<SensorValue> Values => Enumerable.Empty<SensorValue>();
  public TimeSpan ValuesTimeWindow { get => TimeSpan.Zero; set { } }

  public IControl Control => throw new NotImplementedException();

  public void ResetMin() { }
  public void ResetMax() { }
  public void Accept(IVisitor v) => v.VisitSensor(this);
  public void Traverse(IVisitor v) => v.VisitSensor(this);

  public void ClearValues() {
    throw new NotImplementedException();
  }
#pragma warning disable CS0067
  public event SensorEventHandler? ValuesAdded;
#pragma warning restore CS0067
}
