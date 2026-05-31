using CrystalMonitor.Hardware;

class Program {
  static void Main(string[] args) {
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