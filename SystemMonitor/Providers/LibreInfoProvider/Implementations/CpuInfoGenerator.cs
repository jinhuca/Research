using LibreHardwareMonitor.Hardware;
using LibreInfoProvider.Interfaces;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

namespace LibreInfoProvider.Implementations; 
public class CpuInfoGenerator : ICpuInfoGenerator {
  /*
  public ICpuSummaryInfo GetCpuSummaryInfo() {
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

      // Load
      ISensor? totalLoad_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Total" && s.SensorType == SensorType.Load);
      result_.TotalLoad = (totalLoad_ == null) ? (0, 0) : (totalLoad_.Value, totalLoad_.Max);

      ISensor? coreMaxLoad_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Core Max" && s.SensorType == SensorType.Load);
      result_.CoreMaxLoad = (coreMaxLoad_ == null) ? (0, 100) : (coreMaxLoad_.Value, 100);

      // Clock
      ISensor? busSpeed_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "Bus Speed" && s.SensorType == SensorType.Clock);
      result_.BusSpeed = (busSpeed_?.Value, busSpeed_?.Max);

      // Voltage
      ISensor? voltage_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Core" && s.SensorType == SensorType.Voltage);
      result_.Voltage = (voltage_?.Value, voltage_?.Max);

      // Power
      ISensor? platformPower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Package" && s.SensorType == SensorType.Power);
      result_.PlatformPower = (platformPower_?.Value, platformPower_?.Max);

      ISensor? packagePower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Package" && s.SensorType == SensorType.Power);
      result_.PackagePower = (packagePower_?.Value, packagePower_?.Max);

      ISensor? coresPower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Cores" && s.SensorType == SensorType.Power);
      result_.CoresPower = (coresPower_?.Value, coresPower_?.Max);

      ISensor? memoryPowers_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Memory" && s.SensorType == SensorType.Power);
      result_.MemoryPower = (memoryPowers_?.Value, memoryPowers_?.Max);

      // Temperature
      ISensor? coreMaxTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "Core Max" && s.SensorType == SensorType.Temperature);
      result_.CoreMaxTemperature = (coreMaxTemperature_?.Value, coreMaxTemperature_?.Max);

      ISensor? coreAvgTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "Core Average" && s.SensorType == SensorType.Temperature);
      result_.CoreAvgTemperature = (coreAvgTemperature_?.Value, coreAvgTemperature_?.Max);

      ISensor? packageTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == "CPU Package" && s.SensorType == SensorType.Temperature);
      result_.PackageTemperature = (packageTemperature_?.Value, packageTemperature_?.Max);
    }
    catch (UnauthorizedAccessException uae) {
      Debug.WriteLine(uae.Message);
      result_ = new CpuSummaryInfo();
    }
    catch (ManagementException mex) {
      Debug.WriteLine(mex.Message);
      result_ = new CpuSummaryInfo();
    }
    finally {
      computer_.Close();
    }
    return result_;
  }

  public List<ICpuCoreInfo> GetCpuCoreInfo() {
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
              CoreIdentifier = "Core " + group.Key,
              Voltage = (group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Max),
              Clock = (group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Max),
              Temperature = (group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Max),
              Load = (group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Max)
            });

        foreach (var core in coreGroups) {
          //Console.WriteLine($"{core.CoreIdentifier}: Voltage: {core.Voltage}V, Temp: {core.Temperature}°C, Load: {core.Load}%, Clock: {core.Clock}MHz");
          result_.Add(new CpuCoreInfo {
            Name = core.CoreIdentifier,
            Voltage = (core.Voltage.Value, core.Voltage.Max),
            Temperature = (core.Temperature.Value, core.Temperature.Max),
            Load = (core.Load.Value, core.Load.Max),
            Speed = (core.Clock.Value, core.Clock.Max)
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

  */
}
