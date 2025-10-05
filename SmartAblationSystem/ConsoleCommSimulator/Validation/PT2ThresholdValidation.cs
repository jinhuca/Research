using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Validation
{
  public class PT2ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> pt2thresholdDictionary;
    private CMCUStatusError _previouslyHadError;
    public PT2ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      pt2thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.PT2)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, pt2thresholdDictionary);
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
        if (pt2thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue > pt2thresholdDictionary[currentStateId].HighValue)
          {
            PTErrors = CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh;  
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
