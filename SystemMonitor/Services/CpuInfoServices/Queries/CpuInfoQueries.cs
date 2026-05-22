using CpuInfoServices.Methods;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using static DataStructures.Cpu.Definitions.QueryDefinitions;

namespace CpuInfoServices.Queries;

public class CpuInfoQueries {
  private ICpuSummaryInfo _summaryInfo;
  private ICpuLiveInfo _liveInfo;
  private static readonly object _queryCpuSummaryLock = new object();
  private static readonly object _queryCpuLiveInfoLock = new object();

  public CpuInfoQueries(ICpuSummaryInfo summaryInfo, ICpuLiveInfo liveInfo) {
    _summaryInfo = summaryInfo;
    _liveInfo = liveInfo;
  }

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

  private static List<ISensor> FetchSensorValues() {
    List<ISensor> sensors_ = new List<ISensor>();
    lock (_queryCpuLiveInfoLock) {
      Computer computer_ = new Computer { IsCpuEnabled = true };
      computer_.Open();
      try {
        var cpu_ = computer_.Hardware.FirstOrDefault(hardware => hardware.HardwareType == HardwareType.Cpu);
        if (cpu_ == null) {
          sensors_.Clear();
          return sensors_;
        }
        cpu_.Update();
        sensors_ = [.. cpu_.Sensors];
      }
      catch (DllNotFoundException nfe) {
        Debug.WriteLine(nfe.Message);
        sensors_.Clear();
      }
      catch (UnauthorizedAccessException uae) {
        Debug.WriteLine(uae.Message);
        sensors_.Clear();
      }
      catch (ManagementException mex) {
        Debug.WriteLine(mex.Message);
        sensors_.Clear();
      }
      catch(Exception ex) {
        Debug.WriteLine(ex.Message);
        sensors_.Clear();
      }
      finally {
        computer_.Close();
      }
    }
    return sensors_;
  }

  private static ICpuOverallLiveInfo QueryOverallInfo(List<ISensor> sensors) {
    ICpuOverallLiveInfo result_ = new CpuOverallLiveInfo();
    if (sensors == null || sensors.Count < 1) return result_;

    ISensor? busSpeed_ = sensors.FirstOrDefault(s => s.Name == CpuBusSpeed && s.SensorType == SensorType.Clock);
    result_.BusSpeed = new SensorDataType { Value = busSpeed_?.Value, Min = busSpeed_?.Min, Max = busSpeed_?.Max };

    ISensor? cpuSpeed_ = sensors.FirstOrDefault(s => s.SensorType == SensorType.Clock);
    result_.CpuSpeed = new SensorDataType { Value = cpuSpeed_?.Value, Min = cpuSpeed_?.Min, Max = cpuSpeed_?.Max };

    ISensor? totalLoad_ = sensors.FirstOrDefault(s => s.Name == CpuTotalLoad && s.SensorType == SensorType.Load);
    result_.TotalLoad = new SensorDataType { Value = totalLoad_?.Value, Min = totalLoad_?.Min, Max = totalLoad_?.Max };

    ISensor? coreMaxLoad_ = sensors.FirstOrDefault(s => s.Name == CpuCoreMaxLoad && s.SensorType == SensorType.Load);
    result_.CoreMaxLoad = new SensorDataType { Value = coreMaxLoad_?.Value, Min = coreMaxLoad_?.Min, Max = coreMaxLoad_?.Max };

    ISensor? voltage_ = sensors.FirstOrDefault(s => s.Name == CpuCore && s.SensorType == SensorType.Voltage);
    result_.Voltage = new SensorDataType { Value = voltage_?.Value, Min = voltage_?.Min, Max = voltage_?.Max };

    ISensor? platformPower_ = sensors.FirstOrDefault(s => s.Name == CpuPlatform && s.SensorType == SensorType.Power);
    result_.PlatformPower = new SensorDataType { Value = platformPower_?.Value, Min = platformPower_?.Min, Max = platformPower_?.Max };

    ISensor? packagePower_ = sensors.FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Power);
    result_.PackagePower = new SensorDataType { Value = packagePower_?.Value, Min = packagePower_?.Min, Max = packagePower_?.Max };

    ISensor? coresPower_ = sensors.FirstOrDefault(s => s.Name == CpuCores && s.SensorType == SensorType.Power);
    result_.CoresPower = new SensorDataType { Value = coresPower_?.Value, Min = coresPower_?.Min, Max = coresPower_?.Max };

    ISensor? memoryPowers_ = sensors.FirstOrDefault(s => s.Name == CPUMemory && s.SensorType == SensorType.Power);
    result_.MemoryPower = new SensorDataType { Value = memoryPowers_?.Value, Min = memoryPowers_?.Min, Max = memoryPowers_?.Max };

    ISensor? coreMaxTemperature_ = sensors.FirstOrDefault(s => s.Name == CoreMax && s.SensorType == SensorType.Temperature);
    result_.CoreMaxTemperature = new SensorDataType { Value = coreMaxTemperature_?.Value, Min = coreMaxTemperature_?.Min, Max = coreMaxTemperature_?.Max };

    ISensor? coreAvgTemperature_ = sensors.FirstOrDefault(s => s.Name == CoreAverage && s.SensorType == SensorType.Temperature);
    result_.CoreAvgTemperature = new SensorDataType { Value = coreAvgTemperature_?.Value, Min = coreAvgTemperature_?.Min, Max = coreAvgTemperature_?.Max };

    ISensor? packageTemperature_ = sensors.FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Temperature);
    result_.PackageTemperature = new SensorDataType { Value = packageTemperature_?.Value, Min = packageTemperature_?.Min, Max = packageTemperature_?.Max };

    return result_;
  }

  private static List<ICpuCoreLiveInfo> QueryCoreInfo(List<ISensor> sensors) {
    List<ICpuCoreLiveInfo> result_ = new();
    var coreGroups = sensors
    .Where(s => s.Name.Contains("Core #"))
    .GroupBy(s => Regex.Match(s.Name, @"#\d+").Value) // Extract "#1", "#2", etc.
    .Select(group => new {
      CoreIdentifier = "Core " + group.Key,
      Voltage = (group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Value,
                 group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Min,
                 group.FirstOrDefault(s => s.SensorType == SensorType.Voltage)?.Max),
      Clock = (group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Value,
               group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Min,
               group.FirstOrDefault(s => s.SensorType == SensorType.Clock)?.Max),
      Temperature = (group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value,
                     group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Min,
                     group.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Max),
      Load = (group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value,
              group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Min,
              group.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Max)
    });
    result_.AddRange(from core in coreGroups
                     select new CpuCoreLiveInfo {
                       Name = core.CoreIdentifier,
                       Voltage = new SensorDataType { Value = core.Voltage.Value, Min = core.Voltage.Min, Max = core.Voltage.Max },
                       Temperature = new SensorDataType { Value = core.Temperature.Value, Min = core.Temperature.Min, Max = core.Temperature.Max },
                       Load = new SensorDataType{ Value = core.Load.Value, Min = core.Load.Min, Max = core.Load.Max },
                       Speed = new SensorDataType{ Value = core.Clock.Value, Min = core.Clock.Min, Max = core.Clock.Max }
                     });
    return result_;
  }
  
  private static ICpuLiveInfo QueryCpuInfo(List<ISensor> sensors) {
    ICpuLiveInfo result_ = new CpuLiveInfo() {
      CpuOverallLiveInfo = QueryOverallInfo(sensors),
      CpuCoreLiveInfo = QueryCoreInfo(sensors)
    };
    return result_;
  }

  public static ICpuLiveInfo QueryCpuLiveInfo() {
    return QueryCpuInfo(FetchSensorValues());
  }
}
