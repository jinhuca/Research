using DataStructures.Cpu.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures.Cpu.Implementations;  
public class CpuCoreLiveInfo : ICpuCoreLiveInfo {
  public string Name { get; set; } = string.Empty;
  public (float? val, float? max) Voltage { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) Speed { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) Temperature { get; set; } = (0.0f, 0.0f);
  public (float? val, float? max) Load { get; set; } = (0.0f, 0.0f);
}
