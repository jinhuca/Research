using DataStructures.Cpu.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures.Cpu.Implementations; 
public class CpuOverallLiveInfo : ICpuOverallLiveInfo {
  public (float? val, float? max) BusSpeed { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) CpuSpeed { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) Voltage { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) PlatformPower { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) PackagePower { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) MemoryPower { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) CoresPower { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) PackageTemperature { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) CoreMaxTemperature { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) CoreAvgTemperature { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) CoreMaxLoad { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) TotalLoad { get; set; } = (0.0f, 0.0f);
}
