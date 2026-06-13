using CrystalMonitor.Hardware;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using static DataStructures.Types.SensorReadingExtensions;
namespace HardwareService;

public static class HardwareObservable {
  /// <summary>
  /// Returns a cold IObservable that polls ALL hardware sensors at <paramref name="interval"/> 
  /// and emits a complete HardwareSnapshot each tick.  
  /// Disposes the Computer when the subscription ends.
  /// </summary>
  public static IObservable<HardwareSnapshot> PollAll(
    TimeSpan? interval = null,
    bool cpu = true,
    bool gpu = true,
    bool memory = true,
    bool storage = true,
    bool motherboard = true,
    bool network = true,
    bool psu = true,
    bool battery = true) {

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
      .Interval(interval ?? TimeSpan.FromSeconds(1))
      .StartWith(-1L)
      .SelectMany(_ => {
        try {
          return Observable.Return(TakeSnapshot(computer));
        }
        catch (Exception ex) {
          Serilog.Log.Error(ex, "Error taking hardware snapshot");
          return Observable.Empty<HardwareSnapshot>();
        }
      })
      .Subscribe(observer);

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
    => PollAll(TimeSpan.FromSeconds(1), cpu, gpu, memory, storage, motherboard, network, psu, battery).Take(1);

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
    computer.Accept(new UpdateVisitor());   // triggers hardware.Update() on every node
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
        sensor.Min,
        sensor.Max,
        UnitFor(sensor.SensorType)));

    foreach (var sub in hw.SubHardware)
      CollectReadings(sub, list);
  }

  
}
