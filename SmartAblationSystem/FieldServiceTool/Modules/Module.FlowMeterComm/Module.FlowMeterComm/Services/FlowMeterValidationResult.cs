using System.Collections.Generic;
using Module.FlowMeterComm.Models;

namespace Module.FlowMeterComm.Services
{
  public class FlowMeterValidationResult
  {
    public bool IsValid { get; set; }
    public double AverageOffset { get; set; }
    public double Acceptance { get; set; }
    public IList<FlowRateData> DataCollection { get; set; }
  }
}