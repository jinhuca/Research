using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using static Communication.CanBusMessageDefinition;

namespace ConsoleCommSimulator.Validation
{
  public abstract class ThresholdValidation : IThresholdValidation
  {
    //private Dictionary<MessageStateId, Thresholds> thresholdDictionary;
    private readonly IEventAggregator _eventAggregator;

    public ThresholdValidation(IEventAggregator eventAggregator)
    {
      _eventAggregator = eventAggregator;
      SubscribeToThresholdUpdate();
    }

    private void SubscribeToThresholdUpdate()
    {
    _eventAggregator.GetEvent<UpdateThresholdEvent>().Subscribe(HandleThresholdUpdateEvent);
    }
    private void UpdateState(MessageStateId state)
    {
      ConsoleStateMessage newStateMessage = new ConsoleStateMessage();
      newStateMessage.State = state;
      // auto switch state
      _eventAggregator?.GetEvent<SystemStateUpdateEvent>().Publish(newStateMessage);
    }

    public void UpdateThresholdDictionary(IDictionary<MessageStateId, Thresholds> newDict, IDictionary<MessageStateId, Thresholds> internalDict)
    {
      // key value pair => kvp
      foreach (var kvp in newDict)
      {
        if (internalDict.ContainsKey(kvp.Key))
        {
          // Update the existing entry
          internalDict[kvp.Key] = kvp.Value;
        }
        else
        {
          // Add a new entry if it doesn't exist yet 
          internalDict.Add(kvp.Key, kvp.Value);
        }
      }
    }

    public virtual void ValidateThresholds(double currentValue, MessageStateId currentStateId)
    {
      // logic in pt1 validation, etc

    }

    public virtual void HandleThresholdUpdateEvent(UpdateThresholdEventArgs e)
    {
      // Update the internal dictionary with the received thresholdsp


    }
    protected PMCUStatusError CheckForPMCUErrors(PMCUStatusError PTErrors, PMCUStatusError previouslyHadError, MessageStateId currState)
    {
      if (PTErrors != 0)
      {
        // send threshold failed event
        ThresholdValidationFailedEventArgs failedArgs = new ThresholdValidationFailedEventArgs(ThresholdStatusType.PMCU_STATUS, (uint)PTErrors);
        PublishThresholdValidationFailedEvent(failedArgs);

      }
      else if (previouslyHadError != 0 && PTErrors == 0)
      {
        // the error is now fixed 
        // send threshold reset event
        ThresholdValidationFailedEventArgs resetArgs = new ThresholdValidationFailedEventArgs(ThresholdStatusType.CLEAR_PMCU_STATUS, (uint)previouslyHadError);
        PublishThresholdValidationFailedEvent(resetArgs);
        if (currState == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          // if it was a warning, state wouldn't be exception
          UpdateStateToIdle();
        }
			}
      // used to set previously had error inside pt1 threshold validation
      return PTErrors;
    }

    protected CMCUStatusError CheckForCMCUErrors(CMCUStatusError PTErrors, CMCUStatusError previouslyHadError, MessageStateId currState )
    {
      if (PTErrors != 0)
      {

        // send threshold failed event
        ThresholdValidationFailedEventArgs failedArgs = new ThresholdValidationFailedEventArgs(ThresholdStatusType.CMCU_STATUS, (uint)PTErrors);
        PublishThresholdValidationFailedEvent(failedArgs);

      }
      else if (previouslyHadError != 0 && PTErrors == 0)
      {
        // the error is now fixed 
        // send threshold reset event
        ThresholdValidationFailedEventArgs resetArgs = new ThresholdValidationFailedEventArgs(ThresholdStatusType.CLEAR_CMCU_STATUS, (uint)previouslyHadError);
        PublishThresholdValidationFailedEvent(resetArgs);
        if (currState == MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          // if it was a warning, state wouldn't be exception 
          UpdateStateToIdle();
        }
      }
      // used to set previously had error inside pt1 threshold validation
      return PTErrors;
    }
    protected void PublishThresholdValidationFailedEvent(ThresholdValidationFailedEventArgs args)
    {
      _eventAggregator?.GetEvent<ThresholdValidationFailedEvent>().Publish(args);
    }
    protected void UpdateStateToException()
    {
      UpdateState(MessageStateId.CAN_ID_STATE_EXCEPTION);
    }
    protected void UpdateStateToIdle()
    {
      // for use when an error disappears or get reset
      UpdateState(MessageStateId.CAN_ID_STATE_IDLE);
    }

  }
}
