using ConsoleCommSimulator.Configuration;
using System;
using Prism.Events;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;
using ConsoleCommSimulator.Validation;

namespace ConsoleCommSimulator.Data
{
  public class UpdateThresholdEvent : PubSubEvent<UpdateThresholdEventArgs>
  {
    
  }
  public class UpdateThresholdEventArgs : EventArgs
  {
    public IDictionary<MessageStateId, Thresholds> ThresholdDictionary { get; set; }
    public ThresholdType ThresholdName { get; set; }

    public UpdateThresholdEventArgs(IDictionary<MessageStateId, Thresholds> newThreshold, ThresholdType newThresholdName)
    {
      ThresholdDictionary = newThreshold;
      ThresholdName = newThresholdName;
    }
  }
  public enum ThresholdType
  {
    NONE, // used for error case
    PT1,
    PT2,
    PT3,
    PT4,
    PT5,
    PS1,
    FM1,
    TS1,
    LC1,
    CP, // contains both cp1 and cp2 
    TC,

  }

}
