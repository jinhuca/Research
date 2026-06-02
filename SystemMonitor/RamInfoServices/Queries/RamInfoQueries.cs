using CrystalMonitor.Hardware;
using DataStructures.Ram.Implementations;
using DataStructures.TypeDefinitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace RamInfoServices.Queries; 
public class RamInfoQueries {
  private static RamSummaryInfo _summaryInfo = new();

  public RamInfoQueries() {
  }

  public static RamSummaryInfo QueryRamSummaryInfo() {
    Computer computer_ = new Computer {
      //IsCpuEnabled = true,
      //IsGpuEnabled = true,
      IsMemoryEnabled = true };
    computer_.Open();

    computer_.Accept(new UpdateVisitor());
    try {
      _summaryInfo = new RamSummaryInfo();
      float? totalRamInGB_ = null;
      float? availableRamInGB = null;
      float? usedRamInGB = null;
      float? ramLoad = null;

      foreach (var hardware in computer_.Hardware) {
        if(hardware.HardwareType == HardwareType.Memory && hardware.Name == "Total Memory") {
          foreach(ISensor sensor in hardware.Sensors) {
            if (sensor.SensorType == SensorType.Data) {
              switch(sensor.Name) {
                case "Total Memory":
                  totalRamInGB_ = sensor.Value;
                  Debug.WriteLine($"Total Memory: {totalRamInGB_} GB");
                  break;
                case "Memory Used":
                  usedRamInGB = sensor.Value;
                  Debug.WriteLine($"Memory Used: {usedRamInGB} GB");
                  break;
                case "Memory Available":
                  availableRamInGB = sensor.Value;
                  Debug.WriteLine($"Memory Available: {availableRamInGB} GB");
                  break;
              }
            }
            else if (sensor.SensorType == SensorType.Load) {
              ramLoad = sensor.Value;
            }
          }
        }
      }

      var ram_ = computer_.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
      if(ram_ == null ) { return _summaryInfo; }
      /*
      var totalRamInGB_ = ram_.Sensors.FirstOrDefault(s => s.Name == "Total Memory")?.Value;
      var availableRamInGB_ = ram_.Sensors.FirstOrDefault(s => s.Name == "Available Memory")?.Value;

      if (totalRamInGB_ != null && availableRamInGB_ != null) {
        _summaryInfo.TotalRamInGB = (int)totalRamInGB_;
        _summaryInfo.AvailableRamInGB = (float)availableRamInGB_;
        _summaryInfo.UsagePercentage = ((float)totalRamInGB_ - (float)availableRamInGB_) / (float)totalRamInGB_ * 100;
      }
      else {
        Debug.WriteLine("Total Memory or Available Memory sensor not found.");
      }
      */
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

    return _summaryInfo;
  }
}
