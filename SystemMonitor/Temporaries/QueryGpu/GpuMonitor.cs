using System;
using System.Collections.Generic;
using System.Text;
using CrystalMonitor.Hardware;

namespace QueryGpu; 
public class GpuMonitor : IDisposable {
  private readonly Computer _computer;

  public GpuMonitor() {
    _computer = new Computer() {
      IsGpuEnabled = true,  // enables dedicated GPUs
      IsCpuEnabled = true,  // enables integrated GPU sensors that may be under CPU hardware
    };
    _computer.Open();
  }

  public void PrintAllGpuInfo() {
    foreach (var hardware in _computer.Hardware) {
      // Filter to GPU types only
      if (hardware.HardwareType == HardwareType.GpuIntel ||
          hardware.HardwareType == HardwareType.GpuNvidia ||
          hardware.HardwareType == HardwareType.GpuAmd) {
        hardware.Update(); // must call before reading sensors

        Console.WriteLine($"\n=== {hardware.Name} ({hardware.HardwareType}) ===");

        foreach (var sensor in hardware.Sensors) {
          Console.WriteLine($"  [{sensor.SensorType}] {sensor.Name}: {sensor.Value}");
        }
      }
    }
  }

  public void PrintSpecificMetrics() {
    foreach (var hardware in _computer.Hardware) {
      bool isIntelGpu = hardware.HardwareType == HardwareType.GpuIntel;
      bool isNvidiaGpu = hardware.HardwareType == HardwareType.GpuNvidia;

      if (!isIntelGpu && !isNvidiaGpu) continue;

      hardware.Update();

      string label = isIntelGpu ? "Intel iGPU" : "NVIDIA dGPU";
      Console.WriteLine($"\n--- {label}: {hardware.Name} ---");

      foreach (var sensor in hardware.Sensors) {
        switch (sensor.SensorType) {
          case SensorType.Load:
            Console.WriteLine($"  Load  [{sensor.Name}]: {sensor.Value:F1}%");
            break;
          case SensorType.Temperature:
            Console.WriteLine($"  Temp  [{sensor.Name}]: {sensor.Value:F1}°C");
            break;
          case SensorType.Clock:
            Console.WriteLine($"  Clock [{sensor.Name}]: {sensor.Value:F0} MHz");
            break;
          case SensorType.SmallData:
          case SensorType.Data:
            Console.WriteLine($"  VRAM  [{sensor.Name}]: {sensor.Value:F0} MB");
            break;
          case SensorType.Power:
            Console.WriteLine($"  Power [{sensor.Name}]: {sensor.Value:F1} W");
            break;
          case SensorType.Fan:
            Console.WriteLine($"  Fan   [{sensor.Name}]: {sensor.Value:F0} RPM");
            break;
        }
      }
    }
  }

  public void Dispose() {
    _computer.Close();
  }
}
