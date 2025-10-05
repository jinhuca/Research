using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public class CP1ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> CP1thresholdDictionary;
    private PMCUStatusError _previouslyHadError;
    private PMCUStatusError CPErrors = 0;
    public CP1ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      CP1thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to Validator
      if (e.ThresholdName == ThresholdType.CP)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, CP1thresholdDictionary);
      }
    }

    public override void ValidateThresholds(double currentValue, MessageStateId currentStateId)
    {
      // Send error message to CMCU message provider, if any
      

      try
      {

        // Access the dictionary, states 0 and exception are not in dict
        if (currentStateId == MessageStateId.CAN_ID_STATE_UNKNOWN || currentStateId == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          return;
        }
        if (CP1thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue > CP1thresholdDictionary[currentStateId].HighValue)
          {
            CPErrors = PMCUStatusError.InnerBalloonPressureTooHigh;
            _previouslyHadError = CPErrors;
            UpdateStateToException();
          } 
          else if  (currentValue < CP1thresholdDictionary[currentStateId].LowValue)
          {
            CPErrors = PMCUStatusError.InnerBalloonPressureTooLow;
            _previouslyHadError = CPErrors;
            UpdateStateToException();
          }
          else
          {
            CPErrors = 0; // reset
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
