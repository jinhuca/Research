using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public class LC1ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> LC1thresholdDictionary;
    private CMCUStatusError _previouslyHadError;
    public LC1ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      LC1thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.LC1)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, LC1thresholdDictionary);
      }
    }

    public override void ValidateThresholds(double currentValue, MessageStateId currentStateId)
    {
      // Send error message to CMCU message provider, if any
      CMCUStatusError PTErrors = 0;

      try
      {

        // Access the dictionary, states 0 and exception are not in dict
        if (currentStateId == MessageStateId.CAN_ID_STATE_UNKNOWN || currentStateId == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          return;
        }
        if (LC1thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue < LC1thresholdDictionary[currentStateId].FailValue)
          {
            // empty
            PTErrors = CMCUStatusError.LoadCellWeightFail;
            _previouslyHadError = PTErrors;
            UpdateStateToException();
          } 
          else if (currentValue < LC1thresholdDictionary[currentStateId].LowValue)
          {
            PTErrors = CMCUStatusError.LoadCellWeightWarning;
            _previouslyHadError = PTErrors;
          }
        }

      }
      catch (Exception ex)
      {
        // Log the exception message
        LogSystem.LogService.LogException(ex, "An error occurred while accessing the dictionary:");
      }
      _previouslyHadError = CheckForCMCUErrors(PTErrors, _previouslyHadError, currentStateId);
      return;
    }


  }
}
