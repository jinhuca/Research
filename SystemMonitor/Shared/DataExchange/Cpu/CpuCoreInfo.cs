using System;
using System.Collections.Generic;
using System.Text;

namespace DataExchange.Cpu;

public class CpuCoreInfo : ICpuCoreInfo {
  public string Name { get; set; } = string.Empty;
  public float? Voltage { get; set; } = float.NaN;
  public float? Speed { get; set; } = float.NaN;
  public float? Temperature { get; set; } = float.NaN;
  public float? Load { get; set; } = float.NaN;
}
