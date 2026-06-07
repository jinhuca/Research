using CrystalMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace GpuIObservableDemo;

public class GpuObservables : IDisposable {
  private readonly Computer _computer;

  public GpuObservables() {
    _computer = new Computer() {
      IsGpuEnabled = true,  // enables dedicated GPUs
      IsCpuEnabled = true,  // enables integrated GPU sensors that may be under CPU hardware
    };
    _computer.Open();
  }

  // ── Core observable: polls all GPU sensors at given interval ──────────────
  public IObservable<GpuSensorReading> AllGpuSensors(TimeSpan? interval = null) =>
      Observable.Create<GpuSensorReading>(observer => {
        var timer = Observable
              .Interval(interval ?? TimeSpan.FromSeconds(1))
              .StartWith(-1L)                         // emit immediately
              .Subscribe(_ => {
                try {
                  foreach (var hw in _computer.Hardware) {
                    if (hw.HardwareType is not (HardwareType.GpuIntel
                                               or HardwareType.GpuNvidia
                                               or HardwareType.GpuAmd))
                      continue;

                    hw.Update();

                    foreach (var sensor in hw.Sensors) {
                      if (!sensor.Value.HasValue) continue;

                      observer.OnNext(new GpuSensorReading {
                        GpuName = hw.Name,
                        GpuType = hw.HardwareType,
                        SensorName = sensor.Name,
                        SensorType = sensor.SensorType,
                        Value = sensor.Value.Value,
                        Timestamp = DateTime.UtcNow,
                      });
                    }
                  }
                }
                catch (Exception ex) { observer.OnError(ex); }
              });

        return Disposable.Create(() => timer.Dispose());
      })
      .Publish()
      .RefCount();                                    // share one timer across subscribers

  // ── Filtered by GPU type ──────────────────────────────────────────────────
  public IObservable<GpuSensorReading> IntelGpuSensors(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.GpuType == HardwareType.GpuIntel);

  public IObservable<GpuSensorReading> NvidiaGpuSensors(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.GpuType == HardwareType.GpuNvidia);

  // ── Filtered by sensor type ───────────────────────────────────────────────
  public IObservable<GpuSensorReading> GpuLoads(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.SensorType == SensorType.Load);

  public IObservable<GpuSensorReading> GpuTemperatures(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.SensorType == SensorType.Temperature);

  public IObservable<GpuSensorReading> GpuClocks(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.SensorType == SensorType.Clock);

  public IObservable<GpuSensorReading> GpuPower(TimeSpan? interval = null) =>
      AllGpuSensors(interval).Where(r => r.SensorType == SensorType.Power);

  // ── Aggregated snapshot: all sensors grouped per GPU per tick ─────────────
  public IObservable<IGroupedObservable<string, GpuSensorReading>> GpuSnapshots(
      TimeSpan? interval = null) =>
      AllGpuSensors(interval).GroupBy(r => r.GpuName);

  // ── Convenience: latest value per named sensor (no duplicates flooding) ───
  public IObservable<GpuSensorReading> DistinctSensorChanges(
      TimeSpan? interval = null,
      float threshold = 0.5f) =>
      AllGpuSensors(interval)
          .GroupBy(r => (r.GpuName, r.SensorName))
          .SelectMany(g => g.DistinctUntilChanged(
              r => MathF.Round(r.Value / threshold) * threshold));  // bucket changes

  public void Dispose() => _computer.Close();
}