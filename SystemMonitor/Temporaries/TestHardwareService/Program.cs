using CrystalMonitor.Hardware;
using HardwareService;

namespace TestHardwareService;

internal class Program {
  private static void Test1() {
    HardwareService.HardwareObservable.QueryOnce().Subscribe(snapshot => {
      Console.WriteLine("Snapshot taken @ " + snapshot.Timestamp);
      foreach (var hw in snapshot.Readings) {
        Console.WriteLine($"- {hw.HardwareName} - {hw.HardwareType} - {hw.SensorName} " +
          $"- Value = {hw.Value}, Min = {hw.Min}, Max = {hw.Max}, {hw.Unit}");
      }
    });
  }

  private static void Test2() {
    HardwareService.HardwareObservable.ReadingStream(TimeSpan.FromSeconds(5))
      .FilterBy(HardwareType.Cpu)
      .FilterBy(SensorType.Load)
      .Subscribe(reading => {
        Console.WriteLine($"{reading.HardwareName} - {reading.SensorName}: {reading.Value} {reading.Unit}");
      });
  }

  static void Main(string[] args) {
    Test1();
    //Test2();
  }
}
