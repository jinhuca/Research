using CpuInfoServices.Methods;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;
using CrystalMonitor.Hardware;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
using System.Linq;
using static DataStructures.Cpu.Definitions.QueryDefinitions;
using static DataStructures.Types.SensorReadingExtensions;
using Serilog;

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
    catch (DllNotFoundException ex) {
      Debug.WriteLine(ex.Message);
    }
    catch (UnauthorizedAccessException ex) {
      Debug.WriteLine(ex.Message);
    }
    catch (ManagementException ex) {
      Debug.WriteLine(ex.Message);
    }
    catch (Exception e) {
      Debug.WriteLine(e.Message);
    }
    finally { computer_.Close(); }
    return result_;
  }

  // Returns both the sensor list and the CPU hardware's display name.
  private static (List<ISensor> Sensors, string HardwareName) FetchSensorValues() {
    List<ISensor> sensors_ = new List<ISensor>();
    string hardwareName_ = "CPU";
    lock (_queryCpuLiveInfoLock) {
      Computer computer_ = new Computer { IsCpuEnabled = true };
      computer_.Open();
      try {
        computer_.Accept(new UpdateVisitor());
        IHardware? cpu_ = computer_.Hardware.FirstOrDefault(hardware => hardware.HardwareType == HardwareType.Cpu);
        if (cpu_ != null) {
          hardwareName_ = cpu_.Name;
          try {
            cpu_.Update();
          }
          catch (NullReferenceException e) {
            // Certain versions of the underlying library may throw a NullReferenceException here.
            // Log the full exception and return an empty sensor list to fail safely.
            Debug.WriteLine(e.ToString());
            sensors_.Clear();
            return (sensors_, hardwareName_);
          }
          catch (Exception e) {
            // Log the full exception and attempt to persist details to a file. Protect file IO with its own try/catch.
            Debug.WriteLine("cpu_.Update() threw: " + e.ToString());
            try {
              string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cpu_update_errors.log");
              var sb = new System.Text.StringBuilder();
              sb.AppendLine(DateTime.UtcNow.ToString("o") + " - cpu_.Update() threw: " + e.GetType().FullName);
              sb.AppendLine("Message: " + (e.Message ?? "(no message)"));
              sb.AppendLine("StackTrace:");
              sb.AppendLine(e.StackTrace ?? "(no stack trace)");
              try { sb.AppendLine("MachineName: " + System.Environment.MachineName); } catch { }
              try { sb.AppendLine("ProcessId: " + System.Diagnostics.Process.GetCurrentProcess().Id); } catch { }
              try { sb.AppendLine("ProcessName: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName); } catch { }
              try { sb.AppendLine("OSVersion: " + System.Environment.OSVersion); } catch { }
              try { sb.AppendLine("Framework: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription); } catch { }
              try { sb.AppendLine("OSArch: " + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture); } catch { }
              try { sb.AppendLine("ProcessArch: " + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture); } catch { }
              try { sb.AppendLine("ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId); } catch { }
              sb.AppendLine();
              try {
                System.IO.File.AppendAllText(logPath, sb.ToString());
              }
              catch (Exception fileEx) {
                Debug.WriteLine("Failed to write cpu update log: " + fileEx.ToString());
              }
            }
            catch (Exception fileEx) {
              Debug.WriteLine("Failed to write cpu update log: " + fileEx.ToString());
            }
            sensors_.Clear();
            return (sensors_, hardwareName_);
          }
        }
        else {
          sensors_.Clear();
          return (sensors_, hardwareName_);
        }
        sensors_ = cpu_.Sensors.ToList();
      }
      catch (NullReferenceException nre) {
        Debug.WriteLine(nre.ToString());
        sensors_.Clear();
      }
      catch (DllNotFoundException nfe) {
        Debug.WriteLine(nfe.ToString());
        sensors_.Clear();
      }
      catch (UnauthorizedAccessException uae) {
        Debug.WriteLine(uae.ToString());
        sensors_.Clear();
      }
      catch (ManagementException mex) {
        Debug.WriteLine(mex.ToString());
        sensors_.Clear();
      }
      catch (Exception ex) {
        Debug.WriteLine(ex.ToString());
        sensors_.Clear();
      }
      finally {
        computer_.Close();
      }
    }
    return (sensors_, hardwareName_);
  }


  private static ICpuOverallLiveInfo QueryOverallInfo(List<ISensor> sensors, string hardwareName) {
    ICpuOverallLiveInfo result_ = new CpuOverallLiveInfo();
    if (sensors == null || sensors.Count < 1) return result_;

    result_.BusSpeed = ToReading(sensors.FirstOrDefault(s => s.Name == CpuBusSpeed && s.SensorType == SensorType.Clock), hardwareName, HardwareType.Cpu);
    result_.CpuSpeed = ToReading(sensors.FirstOrDefault(s => s.SensorType == SensorType.Clock), hardwareName, HardwareType.Cpu);
    result_.TotalLoad = ToReading(sensors.FirstOrDefault(s => s.Name == CpuTotalLoad && s.SensorType == SensorType.Load), hardwareName, HardwareType.Cpu);
    result_.CoreMaxLoad = ToReading(sensors.FirstOrDefault(s => s.Name == CpuCoreMaxLoad && s.SensorType == SensorType.Load), hardwareName, HardwareType.Cpu);
    result_.Voltage = ToReading(sensors.FirstOrDefault(s => s.Name == CpuCore && s.SensorType == SensorType.Voltage), hardwareName, HardwareType.Cpu);
    result_.PlatformPower = ToReading(sensors.FirstOrDefault(s => s.Name == CpuPlatform && s.SensorType == SensorType.Power), hardwareName, HardwareType.Cpu);
    result_.PackagePower = ToReading(sensors.FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Power), hardwareName, HardwareType.Cpu);
    result_.CoresPower = ToReading(sensors.FirstOrDefault(s => s.Name == CpuCores && s.SensorType == SensorType.Power), hardwareName, HardwareType.Cpu);
    result_.MemoryPower = ToReading(sensors.FirstOrDefault(s => s.Name == CPUMemory && s.SensorType == SensorType.Power), hardwareName, HardwareType.Cpu);
    result_.CoreMaxTemperature = ToReading(sensors.FirstOrDefault(s => s.Name == CoreMax && s.SensorType == SensorType.Temperature), hardwareName, HardwareType.Cpu);
    result_.CoreAvgTemperature = ToReading(sensors.FirstOrDefault(s => s.Name == CoreAverage && s.SensorType == SensorType.Temperature), hardwareName, HardwareType.Cpu);
    result_.PackageTemperature = ToReading(sensors.FirstOrDefault(s => s.Name == CpuPackage && s.SensorType == SensorType.Temperature), hardwareName, HardwareType.Cpu);

    return result_;
  }

  private static List<ICpuCoreLiveInfo> QueryCoreInfo(List<ISensor> sensors, string hardwareName) {
    List<ICpuCoreLiveInfo> result_ = new();
    var coreGroups = sensors
    .Where(s => s.Name.Contains("Core #"))
    .GroupBy(s => Regex.Match(s.Name, @"#\d+").Value) // Extract "#1", "#2", etc.
    .Select(group => new {
      CoreIdentifier = "Core " + group.Key,
      VoltageSensor = group.FirstOrDefault(s => s.SensorType == SensorType.Voltage),
      ClockSensor = group.FirstOrDefault(s => s.SensorType == SensorType.Clock),
      TemperatureSensor = group.FirstOrDefault(s => s.SensorType == SensorType.Temperature),
      LoadSensor = group.FirstOrDefault(s => s.SensorType == SensorType.Load)
    });

    result_.AddRange(from core in coreGroups
                     select new CpuCoreLiveInfo {
                       Name = core.CoreIdentifier,
                       Voltage = ToReading(core.VoltageSensor, hardwareName, HardwareType.Cpu),
                       Temperature = ToReading(core.TemperatureSensor, hardwareName, HardwareType.Cpu),
                       Load = ToReading(core.LoadSensor, hardwareName, HardwareType.Cpu),
                       Speed = ToReading(core.ClockSensor, hardwareName, HardwareType.Cpu)
                     });
    return result_;
  }

  private static IOSLiveInfo QueryOSLiveInfo() {
    OSLiveInfo result;
    try {
      result = new OSLiveInfo {
        ProcessNum = Process.GetProcesses().Length,
        ThreadsNum = Process.GetProcesses().Sum(proc => proc.Threads.Count),
        HandlesNum = Process.GetProcesses().Sum(proc => proc.HandleCount),
        UpTime = TimeSpan.FromMilliseconds(Environment.TickCount64)
      };
    }
    catch (Exception ex) {
      Log.Logger.Error("Failed to query OS live info: {Message}", ex.Message);
      result = new OSLiveInfo();
    }
    return result;
  }

  private static ICpuLiveInfo QueryCpuInfo(List<ISensor> sensors, string hardwareName) {
    ICpuLiveInfo result_ = new CpuLiveInfo() {
      OsLiveInfo = QueryOSLiveInfo(),
      CpuOverallLiveInfo = QueryOverallInfo(sensors, hardwareName),
      CpuCoreLiveInfo = QueryCoreInfo(sensors, hardwareName)
    };
    return result_;
  }

  public static ICpuLiveInfo QueryCpuLiveInfo() {
    var (sensors, hardwareName) = FetchSensorValues();
    return QueryCpuInfo(sensors, hardwareName);
  }
}