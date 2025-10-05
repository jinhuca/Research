
using System;

namespace Module.FlowMeterComm.Models
{
  public class FlowRateData
  {
    public DateTime Timestamp { get; set; }
    public long Index { get; set; }
    public double FM1 { get; set; }
    public double FMExt { get; set; }

    public override string ToString()
    {
      return $"{Index},{FM1},{FMExt}";
    }
  }
}
