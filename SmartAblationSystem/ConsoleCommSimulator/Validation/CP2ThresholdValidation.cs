using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public class CP2ThresholdValidation : ThresholdValidation
  {
    private Dictionary<MessageStateId, Thresholds> CP2thresholdDictionary;
    private PMCUStatusError _previouslyHadError;
    public CP2ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      CP2thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.CP)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, CP2thresholdDictionary);
      }
    }

    public override void ValidateThresholds(double currentValue, MessageStateId currentStateId)
    {
      // Send error message to CMCU message provider, if any
      PMCUStatusError CPErrors = 0;

      try
      {

        // Access the dictionary, states 0 and exception are not in dict
        if (currentStateId == MessageStateId.CAN_ID_STATE_UNKNOWN || currentStateId == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          return;
        }
        if (CP2thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue > CP2thresholdDictionary[currentStateId].OuterValue)
          {
            CPErrors = PMCUStatusError.OuterBalloonPressureTooHigh;
            _previouslyHadError = CPErrors;
            UpdateStateToException();
          }
        }

      }
      catch (Exception ex)
      {
        // Log the exception message
        LogSystem.LogService.LogException(ex, "An error occurred while accessing the dictionary:");
      }
      _previouslyHadError = CheckForPMCUErrors(CPErrors, _previouslyHadError, currentStateId);
      return;
    }


  }
}
