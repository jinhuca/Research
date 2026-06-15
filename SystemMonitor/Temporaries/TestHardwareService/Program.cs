using CrystalMonitor.Hardware;
using HardwareService;
using System.Collections;

namespace TestHardwareService;

internal class Program {
  private static void Test1() {
    HardwareService.HardwareObservable.QueryOnce().Subscribe(snapshot => {
      Console.WriteLine("Snapshot taken @ " + snapshot.Timestamp);
      foreach (var hw in snapshot.Readings) {
        Console.WriteLine($"- Hardware Name = {hw.HardwareName}; Hardware Type = {hw.HardwareType}; " +
          $"Sensor Name = {hw.SensorName}; Sensor Type = {hw.SensorType} " +
          $"- Value = {hw.Value}, Min = {hw.Min}, Max = {hw.Max}, Unit = {hw.Unit}");
      }
    });
  }

  private static void GpuNvidiaTest() {
    IDisposable disposable = HardwareObservable.ReadingStream(TimeSpan.FromSeconds(5))
      .FilterBy(HardwareType.GpuNvidia)
      //.FilterBy(SensorType.Load)
      .Subscribe(reading => {
        Console.WriteLine($"- Hardware Name = {reading.HardwareName}; Hardware Type = {reading.HardwareType}; " +
          $"Sensor Name = {reading.SensorName}; Sensor Type = {reading.SensorType} " +
          $"- Value = {reading.Value}, Min = {reading.Min}, Max = {reading.Max}, Unit = {reading.Unit}");
      });
  }

  private static void GpuIntelTest() {
    IDisposable disposable = HardwareObservable.ReadingStream(TimeSpan.FromSeconds(5))
      .FilterBy(HardwareType.GpuIntel)
      //.FilterBy(SensorType.Load)
      .Subscribe(reading => {
        Console.WriteLine($"- Hardware Name = {reading.HardwareName}; Hardware Type = {reading.HardwareType}; " +
          $"Sensor Name = {reading.SensorName}; Sensor Type = {reading.SensorType} " +
          $"- Value = {reading.Value}, Min = {reading.Min}, Max = {reading.Max}, Unit = {reading.Unit}");
      });
  }

  private static void MemoryTest() {
    IDisposable disposable = HardwareObservable.ReadingStream(TimeSpan.FromSeconds(5))
    .FilterBy(HardwareType.Memory)
    //.FilterBy(SensorType.Load)
    .Subscribe(reading => {
      Console.WriteLine($"- Hardware Name = {reading.HardwareName}; Hardware Type = {reading.HardwareType}; " +
        $"Sensor Name = {reading.SensorName}; Sensor Type = {reading.SensorType} " +
        $"- Value = {reading.Value}, Min = {reading.Min}, Max = {reading.Max}, Unit = {reading.Unit}");
    });
  }

  private static void test3() {
    var computer = new Computer {
      IsGpuEnabled = true
    };
    computer.Open();

    foreach (var hardware in computer.Hardware) {
      if (hardware.HardwareType == HardwareType.GpuNvidia ||
          hardware.HardwareType == HardwareType.GpuAmd ||
          hardware.HardwareType == HardwareType.GpuIntel) {
        hardware.Update();

        foreach (var sensor in hardware.Sensors) {
          if (sensor.SensorType == SensorType.SmallData) {
            Console.WriteLine($"{sensor.Name}: {sensor.Value} {(sensor.Value < 1 ? "GB" : "MB")}");
          }
        }
      }
    }
  }

  private static bool IsGpu(HardwareType type) =>
        type == HardwareType.GpuNvidia ||
        type == HardwareType.GpuAmd ||
        type == HardwareType.GpuIntel;

  public static void DumpAll() {
    var computer = new Computer {
      IsGpuEnabled = true,
      IsCpuEnabled = false,
      IsMemoryEnabled = false,
      IsMotherboardEnabled = false,
      IsStorageEnabled = false,
      IsNetworkEnabled = false
    };

    computer.Open();

    var gpus = computer.Hardware.Where(h => IsGpu(h.HardwareType)).ToList();

    if (gpus.Count == 0) {
      Console.WriteLine("No GPUs detected. (Run as Administrator?)");
      computer.Close();
      return;
    }

    // Update twice with a short delay — first read is often empty/zero
    foreach (var hw in gpus)
      hw.Update();

    System.Threading.Thread.Sleep(500);

    foreach (var hw in gpus)
      hw.Update();

    foreach (var hw in gpus) {
      Console.WriteLine();
      Console.WriteLine($"==== {hw.HardwareType} : {hw.Name} ====");
      Console.WriteLine($"Identifier: {hw.Identifier}");

      if (hw.Sensors.Length == 0) {
        Console.WriteLine("  (no sensors reported)");
      }

      // Group by sensor type for readability
      foreach (var group in hw.Sensors.GroupBy(s => s.SensorType).OrderBy(g => g.Key.ToString())) {
        Console.WriteLine($"  --- {group.Key} ---");
        foreach (var sensor in group.OrderBy(s => s.Name)) {
          var value = sensor.Value.HasValue ? sensor.Value.Value.ToString("F2") : "null";
          var max = sensor.Max.HasValue ? sensor.Max.Value.ToString("F2") : "null";
          Console.WriteLine($"    {sensor.Name,-30} value={value,-10} max={max,-10} index={sensor.Index}");
        }
      }

      // Sub-hardware (some GPUs expose sensors here)
      foreach (var sub in hw.SubHardware) {
        sub.Update();
        Console.WriteLine($"  -- SubHardware: {sub.HardwareType} : {sub.Name} --");
        foreach (var sensor in sub.Sensors.OrderBy(s => s.SensorType.ToString()).ThenBy(s => s.Name)) {
          var value = sensor.Value.HasValue ? sensor.Value.Value.ToString("F2") : "null";
          Console.WriteLine($"    {sensor.SensorType,-15} {sensor.Name,-30} value={value}");
        }
      }
    }

    computer.Close();
  }

  private static void Test4() {
    var computer = new Computer {
      IsGpuEnabled = true,
      IsCpuEnabled = true,
      IsMemoryEnabled = false,
      IsMotherboardEnabled = false,
      IsStorageEnabled = false,
      IsNetworkEnabled = false
    };

    foreach (var hw in computer.Hardware.Where(h =>
    h.HardwareType == HardwareType.GpuNvidia ||
    h.HardwareType == HardwareType.GpuAmd ||
    h.HardwareType == HardwareType.GpuIntel)) {

      hw.Update();

      var sharedTotal = hw.Sensors.FirstOrDefault(s =>
          s.SensorType == SensorType.SmallData &&
          s.Name.Equals("D3D Shared Memory Total", StringComparison.OrdinalIgnoreCase));

      if (sharedTotal?.Value != null) {
        Console.WriteLine($"{hw.Name} - Shared Memory Total: {sharedTotal.Value} MB");
      }
    }
  }

  static void Main(string[] args) {
    Test1();
    //GpuNvidiaTest();
    //GpuIntelTest();
    //MemoryTest();
    //test3();
    //DumpAll();
    //Test4();
  }
}
