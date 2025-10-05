using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public class PS1ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> ps1thresholdDictionary;
    private CMCUStatusError _previouslyHadError;
    public PS1ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      ps1thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.PS1)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, ps1thresholdDictionary);
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
        if (ps1thresholdDictionary.ContainsKey(currentStateId))
        {
          // can add more errors here using else if statements
          if (currentValue > ps1thresholdDictionary[currentStateId].HighValue)
          {
            PTErrors = CMCUStatusError.InjectionVentPressureIsHigh;

            _previouslyHadError = PTErrors;
            UpdateStateToException();
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
