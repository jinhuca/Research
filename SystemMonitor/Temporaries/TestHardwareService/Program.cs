namespace TestHardwareService;

internal class Program {
  private static void Test1() {
    HardwareService.HardwareObservable.PollAll().Subscribe(snapshot => {
      Console.WriteLine("Snapshot taken:");
      foreach (var hw in snapshot.Readings) {
        Console.WriteLine($"- {hw.HardwareName} - {hw.HardwareType} - {hw.SensorName} - {hw.Value} {hw.Unit}");
      }
    });
  }

  static void Main(string[] args) {
    Test1();
  }
}
