using CrystalMonitor.Hardware;
using System.Reactive.Linq;

namespace GpuIObservableDemo; 
internal class Program {
  static void Main(string[] args) {
    Demo();
  }

  static void Demo() {
    GpuObservables monitor = new GpuObservables();
    var interval = TimeSpan.FromSeconds(1);

    // 1. All sensors from both GPUs
    var allSub = monitor.AllGpuSensors(interval)
        .Subscribe(r => Console.WriteLine(
            $"[{r.Timestamp:HH:mm:ss}] {r.GpuType} | {r.SensorType,-12} | {r.SensorName,-30} | {r.Value:F1}"));

    // 2. Only NVIDIA load sensors
    var nvLoadSub = monitor.NvidiaGpuSensors(interval)
        .Where(r => r.SensorType == SensorType.Load)
        .Subscribe(r => Console.WriteLine($"NVIDIA Load [{r.SensorName}]: {r.Value:F1}%"));

    // 3. Alert when either GPU exceeds 85°C
    var tempAlert = monitor.GpuTemperatures(interval)
        .Where(r => r.Value > 85f)
        .Subscribe(r => Console.WriteLine($"⚠ THERMAL ALERT: {r.GpuName} {r.Value:F1}°C"));

    // 4. Combine Intel + NVIDIA core clocks side-by-side
    var intelClock = monitor.IntelGpuSensors(interval).Where(r => r.SensorType == SensorType.Clock);
    var nvidiaClock = monitor.NvidiaGpuSensors(interval).Where(r => r.SensorType == SensorType.Clock);

    var combinedSub = Observable
        .CombineLatest(
            intelClock.Where(r => r.SensorName.Contains("Core")),
            nvidiaClock.Where(r => r.SensorName.Contains("Core")),
            (i, n) => $"iGPU: {i.Value:F0} MHz | dGPU: {n.Value:F0} MHz")
        .Subscribe(Console.WriteLine);

    // 5. Only emit when a sensor value changes meaningfully (±0.5 threshold)
    var changeSub = monitor.DistinctSensorChanges(interval, threshold: 0.5f)
        .Subscribe(r => Console.WriteLine($"Changed: {r.GpuName} / {r.SensorName} = {r.Value:F2}"));

    // 6. Rolling 10-second average temperature per GPU
    var rollingAvg = monitor.GpuTemperatures(interval)
        .GroupBy(r => r.GpuName)
        .SelectMany(g => g
            .Buffer(TimeSpan.FromSeconds(10), interval)
            .Where(buf => buf.Count > 0)
            .Select(buf => (Gpu: g.Key, Avg: buf.Average(r => r.Value))))
        .Subscribe(x => Console.WriteLine($"{x.Gpu} 10s avg temp: {x.Avg:F1}°C"));

    Console.ReadLine();

  }
}
