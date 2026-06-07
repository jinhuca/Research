namespace LHMtoObservables;

using CrystalMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// ── Data models ────────────────────────────────────────────────────────────────

public record SensorReading(
  string HardwareName,
  HardwareType HardwareType,
  string SensorName,
  SensorType SensorType,
  float? Value,
  string? Unit
);

public record HardwareSnapshot(
  DateTimeOffset Timestamp,
  IReadOnlyList<SensorReading> Readings
);

// ── Core factory ──────────────────────────────────────────────────────────────

public static class HardwareMonitorObservable {
  /// <summary>
  /// Returns a cold IObservable that polls ALL hardware sensors at
  /// <paramref name="interval"/> and emits a complete HardwareSnapshot
  /// each tick.  Disposes the Computer when the subscription ends.
  /// </summary>
  public static IObservable<HardwareSnapshot> PollAll(
    TimeSpan? interval = null,
    bool cpu = true,
    bool gpu = true,
    bool memory = true,
    bool storage = true,
    bool motherboard = true,
    bool network = true,
    bool psu = false,
    bool battery = true) {

    var period = interval ?? TimeSpan.FromSeconds(1);

    return Observable.Create<HardwareSnapshot>(observer => {
      var computer = new Computer {
        IsCpuEnabled = cpu,
        IsGpuEnabled = gpu,
        IsMemoryEnabled = memory,
        IsStorageEnabled = storage,
        IsMotherboardEnabled = motherboard,
        IsNetworkEnabled = network,
        IsPsuEnabled = psu,
        IsBatteryEnabled = battery,
      };

      computer.Open();

      var sub = Observable
          .Interval(period)
          .StartWith(-1L)
          .Select(_ => TakeSnapshot(computer))
          .Subscribe(observer);

      // returned IDisposable is called on unsubscribe
      return Disposable.Create(() => {
        sub.Dispose();
        computer.Close();
      });
    });
  }

  // ── Single-shot query (one snapshot, then completes) ──────────────────────
  public static IObservable<HardwareSnapshot> QueryOnce(
    bool cpu = true, bool gpu = true, bool memory = true,
    bool storage = true, bool motherboard = true,
    bool network = true, bool psu = false, bool battery = true)
    => PollAll(TimeSpan.MaxValue, cpu, gpu, memory, storage, motherboard, network, psu, battery).Take(1);

  // ── Flat stream of individual readings (one per sensor per tick) ──────────
  public static IObservable<SensorReading> ReadingStream(TimeSpan? interval = null)
  => PollAll(interval).SelectMany(snapshot => snapshot.Readings);

  // ── Filter helpers ────────────────────────────────────────────────────────
  public static IObservable<SensorReading> FilterBy(this IObservable<SensorReading> source, SensorType type)
    => source.Where(r => r.SensorType == type);

  public static IObservable<SensorReading> FilterBy(this IObservable<SensorReading> source, HardwareType type)
    => source.Where(r => r.HardwareType == type);

  // ── Internal snapshot builder ─────────────────────────────────────────────
  private static HardwareSnapshot TakeSnapshot(IComputer computer) {
    computer.Accept(new LHMtoObservables. UpdateVisitor());   // triggers hardware.Update() on every node
    var readings = new List<SensorReading>();
    foreach (var hw in computer.Hardware)
      CollectReadings(hw, readings);
    return new HardwareSnapshot(DateTimeOffset.Now, readings);
  }

  private static void CollectReadings(IHardware hw, List<SensorReading> list) {
    foreach (var sensor in hw.Sensors)
      list.Add(new SensorReading(
        hw.Name,
        hw.HardwareType,
        sensor.Name,
        sensor.SensorType,
        sensor.Value,
        UnitFor(sensor.SensorType)));

    foreach (var sub in hw.SubHardware)
      CollectReadings(sub, list);
  }

  private static string? UnitFor(SensorType type) => type switch {
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
    _ => null
  };
}

// ── Example usage (console app entry-point) ───────────────────────────────────

internal class Program {
  // 1.  Poll everything every second, print temperatures:
  static void Test1_Get_Temperature_Per_Second() {
    using IDisposable _ = HardwareMonitorObservable
      .ReadingStream(TimeSpan.FromSeconds(1))
      .FilterBy(SensorType.Temperature)
      .Subscribe(r => Console.WriteLine($"[{r.HardwareType}] {r.HardwareName} / {r.SensorName}: {r.Value}{r.Unit}"));
  }

  // 2.  Single LINQ query — one snapshot, group by hardware:
  static async Task Test2_Async_Snapshot() {
    var snapshot = await HardwareMonitorObservable.QueryOnce().FirstAsync();
    var grouped = snapshot.Readings
      .GroupBy(r => r.HardwareName)
      .Select(g => new {
        Hardware = g.Key,
        Sensors = g.ToList()
      });

    foreach (var hw in grouped) {
      Console.WriteLine($"\n── {hw.Hardware} ──");
      foreach (var s in hw.Sensors)
        Console.WriteLine($"  {s.SensorName,-30} {s.Value,8:F2} {s.Unit}");
    }
  }

  // 3.  CPU load average over a 10-second window:
  static async Task Test3_Async_10_second_window() {
    var avgLoad = await HardwareMonitorObservable
    .ReadingStream(TimeSpan.FromMilliseconds(500))
    .FilterBy(HardwareType.Cpu)
    .FilterBy(SensorType.Load)
    .Where(r => r.SensorName == "CPU Total")
    .Select(r => r.Value ?? 0f)
    .Buffer(TimeSpan.FromSeconds(10))
    .Select(buf => buf.Count > 0 ? buf.Average() : 0f)
    .FirstAsync();

    Console.WriteLine($"10-second average CPU load: {avgLoad:F1}%");
  }

  static async Task Main(string[] args) {
    //Test1_Get_Temperature_Per_Second();
    await Test2_Async_Snapshot();
    //await Test3_Async_10_second_window();
  }
}
