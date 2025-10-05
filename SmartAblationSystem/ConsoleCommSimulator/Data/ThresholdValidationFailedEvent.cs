using ConsoleCommSimulator.Configuration;
using System;
using Prism.Events;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Data
{
  public class ThresholdValidationFailedEvent : PubSubEvent<ThresholdValidationFailedEventArgs>
  {
  }
  public class ThresholdValidationFailedEventArgs : EventArgs
  {
    public ThresholdStatusType StatusType { get; set; }
    public uint ErrorCode { get; set; }

    public ThresholdValidationFailedEventArgs(ThresholdStatusType statusType, uint errorCode)
    {
      StatusType = statusType;
      ErrorCode = errorCode;
    }
  }

  public enum ThresholdStatusType
  {
    CMCU_STATUS,
    PMCU_STATUS,
    CLEAR_CMCU_STATUS, // removes the error code
    CLEAR_PMCU_STATUS,
    RESET_CMCU, // sets to 0 
    RESET_PMCU
  }
}
