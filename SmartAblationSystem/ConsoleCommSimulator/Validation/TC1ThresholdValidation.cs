using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public class TC1ThresholdValidation : ThresholdValidation
  {
    // dictionary specific to PT
    private Dictionary<MessageStateId, Thresholds> TC1thresholdDictionary;
    private PMCUStatusError _previouslyHadError;
    private double _minTempBalloonDefault = -70;
    public TC1ThresholdValidation(IEventAggregator eventAggregator) : base(eventAggregator)
    {
      // Initialize the internal dictionary for PTValidator
      TC1thresholdDictionary = new Dictionary<MessageStateId, Thresholds>();
    }
    public override void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      base.HandleThresholdUpdateEvent(e);
      // Update the internal dictionary with the received thresholds specific to PTValidator
      if (e.ThresholdName == ThresholdType.TC)
      {
        UpdateThresholdDictionary(e.ThresholdDictionary, TC1thresholdDictionary);
      }
    }

    public override void ValidateThresholds(double currentValue, MessageStateId currentStateId)
    {
      // Send error message to CMCU message provider, if any
      PMCUStatusError PTErrors = 0;
      try
      {
        // Access the dictionary, states 0 and exception are not in dict
        if (currentStateId == MessageStateId.CAN_ID_STATE_UNKNOWN || currentStateId == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          return;
        }
        if (TC1thresholdDictionary.ContainsKey(currentStateId))
        {
          if (currentValue < TC1thresholdDictionary[currentStateId].LowValue)
          {
            // too low
            if (currentStateId != MessageStateId.CAN_ID_STATE_TRANSITION
            && currentStateId != MessageStateId.CAN_ID_STATE_ABLATION)
            {
              // send exception error
              PTErrors = PMCUStatusError.BalloonTemperatureLowWarning;
              _previouslyHadError = PTErrors;
              UpdateStateToException();
            }
            else
            {
              // only sent a warning
              PTErrors = PMCUStatusError.BalloonTemperatureLowWarning;
              _previouslyHadError = PTErrors;
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Log the exception message
        LogSystem.LogService.LogException(ex, "An error occurred while accessing the dictionary:");
      }
      _previouslyHadError = CheckForPMCUErrors(PTErrors, _previouslyHadError, currentStateId);
      return;
    }

  }
}
