
using System;
using System.Collections.Generic;
using System.Linq;
using Module.FlowMeterComm.Models;

namespace Module.FlowMeterComm.Services
{
  public class FlowMeterValidator
  {
    private static int ValidateSeconds = 120; 

    public static FlowMeterValidationResult ValidateFlowMeterResult(ICollection<FlowRateData> flowRateData, double acceptanceOffset, int dataSamplingTimeInMs)
    {
      var lastTimestamp = flowRateData.Last().Timestamp;

      var skipCount = flowRateData.Count(fd => fd.Timestamp + TimeSpan.FromSeconds(ValidateSeconds) < lastTimestamp);
      skipCount = Math.Max(0, skipCount - 1);

      // calculate average on the last 120 seconds
      var averageOffset = flowRateData
        .Skip(skipCount)
        .Select(f => Math.Abs(f.FMExt!=0?(f.FMExt -  f.FM1)/f.FMExt:0))
        .Average();
      
      return new FlowMeterValidationResult()
      {
        IsValid = averageOffset <= acceptanceOffset, 
        AverageOffset = averageOffset, 
        Acceptance = acceptanceOffset,
        DataCollection = flowRateData.Skip(skipCount).ToList()
      }; 
    }
  }
}
