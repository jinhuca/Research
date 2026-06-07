using CrystalMonitor.Hardware;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace HardwareService.Tests;

// ── Testable seam ─────────────────────────────────────────────────────────────

/// <summary>
/// Mirrors HardwareObservable logic but accepts an IComputer factory,
/// avoiding real hardware access (ring-0 drivers, admin rights).
/// </summary>
public static class TestableHardwareObservable {
  public static IObservable<HardwareSnapshot> PollWith(
      Func<IComputer> factory,
      TimeSpan? interval = null) {
    var period = interval ?? TimeSpan.FromSeconds(1);

    return Observable.Create<HardwareSnapshot>(observer =>
    {
      var computer = factory();

      var sub = Observable
          .Interval(period)
          .StartWith(-1L)
          .Select(_ => Snapshot(computer))
          .Subscribe(observer);

      return Disposable.Create(() => sub.Dispose());
    });
  }

  public static IObservable<SensorReading> ReadingStream(
      Func<IComputer> factory, TimeSpan? interval = null)
      => PollWith(factory, interval).SelectMany(s => s.Readings);

  private static HardwareSnapshot Snapshot(IComputer computer) {
    computer.Accept(new UpdateVisitor());
    var readings = new List<SensorReading>();
    foreach (var hw in computer.Hardware)
      Collect(hw, readings);
    return new HardwareSnapshot(DateTimeOffset.Now, readings);
  }

  private static void Collect(IHardware hw, List<SensorReading> list) {
    foreach (var s in hw.Sensors)
      list.Add(new SensorReading(
          hw.Name, hw.HardwareType,
          s.Name, s.SensorType,
          s.Value, UnitFor(s.SensorType)));

    foreach (var sub in hw.SubHardware)
      Collect(sub, list);
  }

  private static string? UnitFor(SensorType t) => t switch {
    SensorType.Temperature => "°C",
    SensorType.Load => "%",
    SensorType.Clock => "MHz",
    SensorType.Power => "W",
    SensorType.Voltage => "V",
    SensorType.Current => "A",
    SensorType.Fan => "RPM",
    SensorType.Flow => "L/h",
    SensorType.Control => "%",
    SensorType.Level => "%",
    SensorType.Data => "GB",
    SensorType.SmallData => "MB",
    SensorType.Throughput => "B/s",
    SensorType.TimeSpan => "s",
    SensorType.Energy => "mWh",
    _ => null,
  };
}
