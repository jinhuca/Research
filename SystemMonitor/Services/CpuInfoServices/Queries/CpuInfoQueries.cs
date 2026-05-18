using CpuInfoServices.Methods;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using static DataStructures.Cpu.Definitions.QueryDefinitions;

namespace CpuInfoServices.Queries;

public class CpuInfoQueries {
  private ICpuSummaryInfo _summaryInfo;
  private ICpuLiveInfo _liveInfo;
  private IContainerProvider _containerProvider;

  public CpuInfoQueries(ICpuSummaryInfo summaryInfo, ICpuLiveInfo liveInfo, IContainerProvider containerProvider) {
    _summaryInfo = summaryInfo;
    _liveInfo = liveInfo;
    _containerProvider = containerProvider;
  }


  [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
  public static ICpuSummaryInfo QuerySummaryInfo() {
    ICpuSummaryInfo result_ = new CpuSummaryInfo();
    Computer computer_ = new Computer { IsCpuEnabled = true };
    computer_.Open();
    try {
      var cpu_ = computer_.Hardware.FirstOrDefault(hardware => hardware.HardwareType == HardwareType.Cpu);
      if (cpu_ == null) {
        return new CpuSummaryInfo();
      }

      result_.BrandName = cpu_.Name;
      foreach (ISensor sensor_ in cpu_.Sensors) {
        if (sensor_.SensorType == SensorType.Clock && sensor_.Name.Contains("Bus Speed")) {
          result_.BusSpeed = sensor_.Value;
        }
      }

      //result_.BrandName = NativeMethods.Brand();
      result_.VendorName = NativeMethods.Vendor();
      result_.BaseSpeed = NativeMethods.GetBaseSpeed();
      result_.SocketNum = NativeMethods.GetSocketNum();
      result_.PhysicalCoreNum = NativeMethods.GetPhysicalCoreCount();
      result_.LogicalCoreNum = NativeMethods.GetLogicalCoreCount();
      result_.Virtualization = NativeMethods.VirtualizationEnabled();
      result_.InstructionSet = NativeMethods.GetInstructionSetStruct();
      result_.CacheInfo = NativeMethods.GetCacheSize();

      //using var searcher = new ManagementObjectSearcher("Select * From Win32_Processor");
      //foreach (ManagementObject obj in searcher.Get()) {
      //  var family = obj["Family"];
      //  var model = obj["Level"];
      //  var stepping = obj["Stepping"];
      //  var name = obj["Name"];
      //}

      (result_.FamilyId, result_.ModelId, result_.SteppingId) = QueryCpuId.GetCpuFamily();
    }
    catch (DllNotFoundException nfe) {

    }
    catch (UnauthorizedAccessException uae) {
    }
    catch (ManagementException mex) {
    }
    catch (Exception e) {
    }
    finally { computer_.Close(); }
    return result_;
  }

  public static ICpuOverallLiveInfo QueryCpuOverallLiveInfo() {
    ICpuOverallLiveInfo result_ = new CpuOverallLiveInfo();
    Computer computer_ = new Computer { IsCpuEnabled = true };
    computer_.Open();
    try {
      var cpu_ = computer_.Hardware.FirstOrDefault(hardware => hardware.HardwareType == HardwareType.Cpu);
      if (cpu_ == null) {
        return result_;
      }
      cpu_.Update();

      // Bus Speed
      ISensor? busSpeed_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuBusSpeed && s.SensorType == SensorType.Clock);
      result_.BusSpeed = (busSpeed_?.Value, busSpeed_?.Max);

      var cpuSpeed_ = cpu_.Sensors.Where(s => s.SensorType == SensorType.Clock).Average(sp => sp.Value);
      var cpuMax_ = cpu_.Sensors.Where(s => s.SensorType == SensorType.Clock).Max(sp => sp.Value);
      result_.CpuSpeed = (cpuSpeed_, cpuMax_);

      // Load
      ISensor? totalLoad_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuTotalLoad && s.SensorType == SensorType.Load);
      result_.TotalLoad = (totalLoad_ == null) ? (0, 0) : (totalLoad_.Value, totalLoad_.Max);

      ISensor? coreMaxLoad_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuCoreMaxLoad && s.SensorType == SensorType.Load);
      result_.CoreMaxLoad = (coreMaxLoad_ == null) ? (0, 100) : (coreMaxLoad_.Value, 100);

      // Voltage
      ISensor? voltage_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuCore && s.SensorType == SensorType.Voltage);
      result_.Voltage = (voltage_?.Value, voltage_?.Max);

      // Power
      ISensor? platformPower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuPlatform && s.SensorType == SensorType.Power);
      result_.PlatformPower = (platformPower_?.Value, platformPower_?.Max);

      ISensor? packagePower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Power);
      result_.PackagePower = (packagePower_?.Value, packagePower_?.Max);

      ISensor? coresPower_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuCores && s.SensorType == SensorType.Power);
      result_.CoresPower = (coresPower_?.Value, coresPower_?.Max);

      ISensor? memoryPowers_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CPUMemory && s.SensorType == SensorType.Power);
      result_.MemoryPower = (memoryPowers_?.Value, memoryPowers_?.Max);

      // Temperature
      ISensor? coreMaxTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CoreMax && s.SensorType == SensorType.Temperature);
      result_.CoreMaxTemperature = (coreMaxTemperature_?.Value, coreMaxTemperature_?.Max);

      ISensor? coreAvgTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CoreAverage && s.SensorType == SensorType.Temperature);
      result_.CoreAvgTemperature = (coreAvgTemperature_?.Value, coreAvgTemperature_?.Max);

      ISensor? packageTemperature_ = cpu_.Sensors
        .FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Temperature);
      result_.PackageTemperature = (packageTemperature_?.Value, packageTemperature_?.Max);
    }
    catch (DllNotFoundException nfe) {
      Debug.WriteLine(nfe.Message);
      result_ = new CpuOverallLiveInfo();
    }
    catch (UnauthorizedAccessException uae) {
      Debug.WriteLine(uae.Message);
      result_ = new CpuOverallLiveInfo();
    }
    catch (ManagementException mex) {
      Debug.WriteLine(mex.Message);
      result_ = new CpuOverallLiveInfo();
    }
    finally {
      computer_.Close();
    }
    return result_;
  }

  public static List<ICpuCoreLiveInfo> QueryCpuCoreLiveInfo() {
    List<ICpuCoreLiveInfo> result_ = new();
    Computer computer_ = new Computer { IsCpuEnabled = true };
    computer_.Open();
    try {
      var cpu = computer_.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
      if (cpu == null) {
        return result_;
      }
      cpu?.Update();
      var coreGroups = cpu?.Sensors
          .Where(s => s.Name.Contains("Core #"))
          .GroupBy(s => Regex.Match(s.Name, @"#\d+").Value) // Extract "#1", "#2", etc.
          .Select(group => new {
            CoreIdentifier = "Core " + group.Key,
            Voltage = (group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Max),
            Clock = (group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Max),
            Temperature = (group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Max),
            Load = (group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value, group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Max)
          });
      //Console.WriteLine($"{core.CoreIdentifier}: Voltage: {core.Voltage}V, Temp: {core.Temperature}°C, Load: {core.Load}%, Clock: {core.Clock}MHz");
      result_.AddRange(from core in coreGroups
                       select new CpuCoreLiveInfo {
                         Name = core.CoreIdentifier,
                         Voltage = (core.Voltage.Value, core.Voltage.Max),
                         Temperature = (core.Temperature.Value, core.Temperature.Max),
                         Load = (core.Load.Value, core.Load.Max),
                         Speed = (core.Clock.Value, core.Clock.Max)
                       });
      return result_;
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
      computer_.Close();
    }

    return result_;
  }

  public static ICpuLiveInfo QueryCpuLiveInfo() {
    ICpuLiveInfo result_ = new CpuLiveInfo() {
      CpuOverallLiveInfo = new CpuOverallLiveInfo(),
      CpuCoreLiveInfo = new List<ICpuCoreLiveInfo>()
    };

    result_.CpuOverallLiveInfo = QueryCpuOverallLiveInfo();
    result_.CpuCoreLiveInfo = QueryCpuCoreLiveInfo();

    return result_;
  }
}
