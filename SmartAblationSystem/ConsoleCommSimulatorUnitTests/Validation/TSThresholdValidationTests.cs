
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.MessageProviders;
using ConsoleCommSimulator.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using SmartAblationSystem.Helpers;
using static Communication.CanBusMessageDefinition;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class TSThresholdValidationTests : ThresholdValidationTestBase
  {

    private TS1ThresholdValidation _TS1ThresholdValidation;

    private double _expectedTS1Fail = -5;

    private double _expectedTS1Pass = -15;
    public void DictionaryInitialSetup()
    {
      _TS1ThresholdValidation = new TS1ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var TS1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var TS1Thresholds = new Thresholds();


      TS1Thresholds.HighValue = -10;

      TS1ThresholdsDictionary = AssignThresholdsToStates(TS1ThresholdsDictionary, TS1Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(TS1ThresholdsDictionary, ThresholdType.TS1);
      _TS1ThresholdValidation.HandleThresholdUpdateEvent(args1);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestTS1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _TS1ThresholdValidation.ValidateThresholds(_expectedTS1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _TS1ThresholdValidation.ValidateThresholds(_expectedTS1Fail, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _TS1ThresholdValidation.ValidateThresholds(_expectedTS1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));

    }
    
  }
}
