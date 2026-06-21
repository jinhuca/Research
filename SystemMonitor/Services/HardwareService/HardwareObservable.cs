using CrystalMonitor.Hardware;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using static DataStructures.Types.SensorReadingExtensions;

// Lets the test assembly call the internal, dependency-injected seams below
// (PollAllCore / QueryOnceCore / TakeSnapshot) directly with a fake IComputer,
// instead of testing a hand-written reimplementation that can drift from this
// file's actual behavior (e.g. the try/catch around TakeSnapshot below).
// NOTE: update the string if the test project's AssemblyName differs from its .csproj file name.
[assembly: InternalsVisibleTo("HardwareObservableTests")]

namespace HardwareService;

public static class HardwareObservable {
  /// <summary>
  /// Returns a cold IObservable that polls ALL hardware sensors at <paramref name="interval"/> 
  /// and emits a complete HardwareSnapshot each tick.  
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
    bool battery = true)
    => PollAllCore(() => {
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
      return (computer, computer.Close);
    }, interval);

  /// <summary>
  /// Same polling/snapshot/error-handling logic as <see cref="PollAll"/>, but the caller
  /// supplies the (already-opened) IComputer and its teardown action instead of a real,
  /// driver-backed <see cref="Computer"/>. This is what makes the polling loop — including
  /// the per-tick try/catch around <see cref="TakeSnapshot"/> — reachable from tests without
  /// admin rights or ring-0 drivers. Internal: exposed to the test assembly only.
  /// </summary>
  internal static IObservable<HardwareSnapshot> PollAllCore(
    Func<(IComputer Computer, Action Close)> factory,
    TimeSpan? interval = null,
    IScheduler? scheduler = null) {

    return Observable.Create<HardwareSnapshot>(observer => {
      var (computer, close) = factory();

      var sub = Observable
      .Interval(interval ?? TimeSpan.FromSeconds(1), scheduler ?? Scheduler.Default)
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
        close();
      });
    });
  }

  // ── Single-shot query (one snapshot, then completes) ──────────────────────
  public static IObservable<HardwareSnapshot> QueryOnce(
    bool cpu = true, bool gpu = true, bool memory = true,
    bool storage = true, bool motherboard = true,
    bool network = true, bool psu = false, bool battery = true)
    => PollAll(TimeSpan.FromSeconds(1), cpu, gpu, memory, storage, motherboard, network, psu, battery).Take(1);

  /// <summary>Testable counterpart to <see cref="QueryOnce"/> — see <see cref="PollAllCore"/>.</summary>
  internal static IObservable<HardwareSnapshot> QueryOnceCore(
    Func<(IComputer Computer, Action Close)> factory, IScheduler? scheduler = null)
    => PollAllCore(factory, TimeSpan.FromSeconds(1), scheduler).Take(1);

  // ── Flat stream of individual readings (one per sensor per tick) ──────────
  public static IObservable<SensorReading> ReadingStream(TimeSpan? interval = null)
  => PollAll(interval).SelectMany(snapshot => snapshot.Readings);

  // ── Filter helpers ────────────────────────────────────────────────────────
  public static IObservable<SensorReading> FilterBy(this IObservable<SensorReading> source, SensorType type)
    => source.Where(sensorReading => {
      ArgumentNullException.ThrowIfNull(sensorReading);
      return sensorReading.SensorType == type;
    });

  public static IObservable<SensorReading> FilterBy(this IObservable<SensorReading> source, HardwareType type)
    => source.Where(sensorReading => {
      ArgumentNullException.ThrowIfNull(sensorReading);
      return sensorReading.HardwareType == type;
    });

  // ── Internal snapshot builder ─────────────────────────────────────────────
  internal static HardwareSnapshot TakeSnapshot(IComputer computer) {
    computer.Accept(new UpdateVisitor());   // triggers hardware.Update() on every node
    var readings = new List<SensorReading>();
    foreach (var hw in computer.Hardware)
      CollectReadings(hw, readings);
    return new HardwareSnapshot(DateTimeOffset.Now, readings);
  }

  private static void CollectReadings(IHardware hw, List<SensorReading> list) {
    foreach (var sensor in hw.Sensors)
      list.Add(new SensorReading(
        HardwareName: hw.Name,
        HardwareType: hw.HardwareType,
        SensorName: sensor.Name,
        SensorType: sensor.SensorType,
        Value: sensor.Value,
        Min: sensor.Min,
        Max: sensor.Max,
        Unit: UnitFor(sensor.SensorType)));

    foreach (var sub in hw.SubHardware)
      CollectReadings(sub, list);
  }
}
