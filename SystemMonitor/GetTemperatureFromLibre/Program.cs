using DataExchange.Cpu;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using System;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

public class UpdateVisitor : IVisitor {
  public void VisitComputer(IComputer computer) => computer.Traverse(this);
  public void VisitHardware(IHardware hardware) {
    hardware.Update();
    foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
  }
  public void VisitSensor(ISensor sensor) { }
  public void VisitParameter(IParameter parameter) { }
}

public class CpuMonitor {


  public static void fetch2() {
    Computer computer = new Computer {
      IsCpuEnabled = true, // Enable CPU monitoring
    };
    computer.Open();

    // Visitor to update sensor values
    UpdateVisitor updateVisitor = new UpdateVisitor();

    //while (true) {
    computer.Accept(updateVisitor);

    foreach (var hardware in computer.Hardware) {
      if (hardware.HardwareType == HardwareType.Cpu) {
        Console.WriteLine($"Hardware: {hardware.Name}");

        // Filter for core-specific sensors (Core #, Temperature, Load, Clock)
        List<ISensor> coreTemps = hardware.Sensors
            .Where(s => s.Name.Contains("CPU Core") && !s.Name.Contains("Tj"))
            .ToList();

        var sensors = hardware.Sensors
          .Where(s => s.SensorType == SensorType.Clock);
        //.Where(s => s.Name.Contains("CPU Core") && !s.Name.Contains("Tj"))
        //.ToList();

        foreach (var sensor in sensors) {
          Console.WriteLine($"\t{sensor.Name}: {sensor.Value}, Type ={sensor.SensorType}");
          //CpuCoreInfo coreInfo_ = new() { Temperature = sensor.Value, Name = sensor.Name };
        }

      }
    }
    //}

    //System.Threading.Thread.Sleep(2000); // Wait 2 seconds
    //Console.Clear();

  }

  public static void fetch1() {
    // 1. Initialize Computer and CPU
    Computer computer = new Computer {
      IsCpuEnabled = true
    };
    computer.Open();
    computer.Accept(new UpdateVisitor()); // Requires a visitor implementation

    // 2. Iterate and Group Sensors
    foreach (var hardware in computer.Hardware) {
      if (hardware.HardwareType == HardwareType.Cpu) {
        Console.WriteLine($"Hardware: {hardware.Name}");
        hardware.Update();

        // Group by Core
        var coreGroups = hardware.Sensors
            .Where(s => s.SensorType == SensorType.Temperature ||
                        s.SensorType == SensorType.Load ||
                        s.SensorType == SensorType.Clock)
            .GroupBy(s => s.Name.Split('#').Last()) // Groups by "1", "2", etc.
            .ToList();

        foreach (var core in coreGroups) {
          Console.WriteLine($"-- Core {core.Key} --");
          foreach (var sensor in core) {
            Console.WriteLine($"{sensor.SensorType} ({sensor.Name}): {sensor.Value}");
          }
        }
      }
    }
    computer.Close();
  }

  public static List<ICpuCoreInfo> GetCoreData() {
    List<ICpuCoreInfo> result_ = new();
    Computer computer = new Computer { IsCpuEnabled = true };
    computer.Open();
    try {
      var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
      if (cpu != null) {
        cpu.Update();

        // Group sensors by the core number found in their Name
        var coreGroups = cpu.Sensors
            .Where(s => s.Name.Contains("Core #"))
            .GroupBy(s => Regex.Match(s.Name, @"#\d+").Value) // Extract "#1", "#2", etc.
            .Select(group => new {
              CoreIdentifier = "Core # " + group.Key,
              Voltage = group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Value,
              Clock = group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Value,
              Temperature = group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value,
              Load = group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value
            });

        foreach (var core in coreGroups) {
          Console.WriteLine($"{core.CoreIdentifier}: Voltage: {core.Voltage}V, Temp: {core.Temperature}°C, Load: {core.Load}%, Clock: {core.Clock}MHz");
          result_.Add(new CpuCoreInfo {
            Name = core.CoreIdentifier,
            Voltage = core.Voltage,
            Temperature = core.Temperature,
            Load = core.Load,
            Speed = core.Clock
          });
        }
      }
    }
    catch (UnauthorizedAccessException uae) {
      //Debug.WriteLine(Messages.AccessDenied + uae.Message);
      result_.Clear();
    }
    catch (ManagementException mex) {
      result_.Clear();
    }
    catch (Exception e) {
      result_.Clear();
    }
    finally {
      computer.Close();
    }

    return result_;
  }

  public static ICpuSummaryInfo FetchCpuSummary() {
    ICpuSummaryInfo result_ = new CpuSummaryInfo();
    Computer computer_ = new Computer { IsCpuEnabled = true };
    computer_.Open();
    try {
      var cpu_ = computer_.Hardware.FirstOrDefault(hardware => hardware.HardwareType == HardwareType.Cpu);
      if (cpu_ == null) {
        return new CpuSummaryInfo();
      }
      cpu_.Update();
      result_.Name = cpu_.Name;

      ISensor? totalLoad_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Total" && s.SensorType == SensorType.Load);
      result_.TotalLoad = (totalLoad_ == null) ? (0, 0) : (totalLoad_.Value, totalLoad_.Max);

      ISensor? busSpeed_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "Bus Speed" && s.SensorType == SensorType.Clock);
      result_.BusSpeed = (busSpeed_?.Value, busSpeed_?.Max);

      ISensor? voltage_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Core" && s.SensorType == SensorType.Voltage);
      result_.Voltage = (voltage_?.Value, voltage_?.Max);

      ISensor? platformPower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Package" && s.SensorType == SensorType.Power);
      result_.PlatformPower = (platformPower_?.Value, platformPower_?.Max);
    }
    catch (UnauthorizedAccessException uae) {
    }
    catch (ManagementException mex) {
    }
    finally {
      computer_.Close();
    }
    return result_;
  }

  public static void Main(string[] args) {
    var temp = FetchCpuSummary();
    //fetch2();
    //var rrtemp = GetCoreData();
  }
}


