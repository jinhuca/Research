using CrystalMonitor.Hardware;
using System.Collections;

class Program {
  static void Main(string[] args) {
    //Test1();
    //testMemory();
    basicmemoryTest();
    
  }

  private static void testMemory() {
    // 1. Initialize Computer and enable Memory tracking
    Computer computer = new Computer {
      IsMemoryEnabled = true
    };

    computer.Open();

    // 2. Locate the Memory Hardware
    var memoryHardware = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);

    if (memoryHardware != null) {
      // Update sensors so they fetch the latest metrics
      memoryHardware.Update();

      foreach (var sensor in memoryHardware.Sensors) {
        // SensorType.Load captures percentage (e.g., 75.5%)
        if (sensor.SensorType == SensorType.Load && sensor.Name == "Memory") {
          Console.WriteLine($"Memory Load: {sensor.Value:F2}%");
        }

        // SensorType.Data captures sizes in Gigabytes (e.g., RAM Used / Available)
        if (sensor.SensorType == SensorType.Data) {
          Console.WriteLine($"{sensor.Name}: {sensor.Value:F2} GB");
        }
      }
    }

    computer.Close();
  }

  private static void MemoryTest() {
    var computer = new Computer {
      IsCpuEnabled = true,
      IsMemoryEnabled = true
    };
    foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Memory)) {
      hardware.Update();
      foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Data)) {
        // Sensor names: "Memory Used", "Memory Available"
        Console.WriteLine($"{sensor.Name}: {sensor.Value:F2} GB");
      }

      // Memory load % is also available
      foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Load)) {
        Console.WriteLine($"{sensor.Name}: {sensor.Value:F1}%");  // "Memory"
      }
    }
  }

  private static void basicmemoryTest() {
      var computer = new Computer {
      //IsCpuEnabled = true,
      IsMemoryEnabled = true
    };

    computer.Open();
    
      computer.Accept(new UpdateVisitor());

      foreach (var hardware in computer.Hardware) {
        Console.WriteLine($"Hardware: {hardware.Name} [{hardware.HardwareType}]");

        foreach (var sensor in hardware.Sensors) {
          if (sensor.SensorType == SensorType.Load)
            Console.WriteLine($"  Load:   {sensor.Name} = {sensor.Value:F1}%");

          if (sensor.SensorType == SensorType.Data)
            Console.WriteLine($"  Memory: {sensor.Name} = {sensor.Value:F2} GB");
        }
      }
      Thread.Sleep(1000); // Update every second
      Console.WriteLine("====");
    
    computer.Close();
  }

  private static void Test1() {
    // 1. Initialize the Computer object and enable Motherboard monitoring
    Computer computer = new Computer {
      IsMotherboardEnabled = true // Required for Motherboard/Super I/O data
    };

    // 2. Open the driver connection
    computer.Open();

    Console.WriteLine("Querying Motherboard Sensors...\n");

    // 3. Iterate through available hardware components
    foreach (IHardware hardware in computer.Hardware) {
      // Filter specifically for Motherboard hardware type
      if (hardware.HardwareType == HardwareType.Motherboard) {
        Console.WriteLine($"Motherboard: {hardware.Name}");

        // Force an update to refresh sensor values
        hardware.Update();

        // 4. Query sensors directly linked to the motherboard object
        PrintSensorData(hardware);

        // 5. Query sub-hardware (e.g., specific LPC/Super I/O chips like Nuvoton or ITE)
        foreach (IHardware subHardware in hardware.SubHardware) {
          Console.WriteLine($"\n  Sub-Hardware Chip: {subHardware.Name}");
          subHardware.Update();
          PrintSensorData(subHardware, "    ");
        }
      }
    }

    // 6. Close the connection to release the driver safely
    computer.Close();

    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
  }

  // Helper method to loop through and display individual sensor telemetry
  static void PrintSensorData(IHardware hardware, string indent = "  ") {
    foreach (ISensor sensor in hardware.Sensors) {
      // GetValueOrDefault avoids null errors if a sensor hasn't read yet
      float value = sensor.Value.GetValueOrDefault();

      Console.WriteLine($"{indent}Sensor: {sensor.Name} | Type: {sensor.SensorType} | Value: {value}");
    }
  }
}

// Implement an UpdateVisitor to traverse hardware
public class UpdateVisitor : IVisitor {
  public void VisitComputer(IComputer computer) { computer.Traverse(this); }
  public void VisitHardware(IHardware hardware) { hardware.Update(); foreach (var subHardware in hardware.SubHardware) subHardware.Accept(this); }
  public void VisitSensor(ISensor sensor) { }
  public void VisitParameter(IParameter parameter) { }
}