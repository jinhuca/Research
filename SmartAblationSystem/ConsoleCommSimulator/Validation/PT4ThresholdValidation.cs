using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Validation
{
  public class PT4ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> pt4thresholdDictionary;
    private CMCUStatusError _previouslyHadError;
    public PT4ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      pt4thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.PT4)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, pt4thresholdDictionary);
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
        if (pt4thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue > pt4thresholdDictionary[currentStateId].HighValue)
          {
            PTErrors = CMCUStatusError.VacuumPressurePT4TooHigh;
            _previouslyHadError = PTErrors;
            UpdateStateToException();
          }
        }

      }
      catch (Exception ex)
      {
        // Log the exception message
        Log.LogException(ex, "An error occurred while accessing the dictionary:");
      }


      _previouslyHadError = CheckForCMCUErrors(PTErrors, _previouslyHadError, currentStateId);
      return;
    }


  }
}
