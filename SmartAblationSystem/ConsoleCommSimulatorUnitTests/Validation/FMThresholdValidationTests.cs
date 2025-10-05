
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
  public class FMThresholdValidationTests : ThresholdValidationTestBase
  {

    private FM1ThresholdValidation _FM1ThresholdValidation;

    private double _expectedFM1FailHigh = 11000;
    private double _expectedFM1FailLow = -5200;

    private double _expectedFM1Pass = 1000;
    public void DictionaryInitialSetup()
    {
      _FM1ThresholdValidation = new FM1ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var FM1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var FM1Thresholds = new Thresholds();


      FM1Thresholds.HighValue = 10000;
      FM1Thresholds.LowValue = -5000;

      FM1ThresholdsDictionary = AssignThresholdsToStates(FM1ThresholdsDictionary, FM1Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(FM1ThresholdsDictionary, ThresholdType.FM1);
      _FM1ThresholdValidation.HandleThresholdUpdateEvent(args1);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestFM1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _FM1ThresholdValidation.ValidateThresholds(_expectedFM1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _FM1ThresholdValidation.ValidateThresholds(_expectedFM1FailLow, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _FM1ThresholdValidation.ValidateThresholds(_expectedFM1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));
      _FM1ThresholdValidation.ValidateThresholds(_expectedFM1FailHigh, currentState);
      _FM1ThresholdValidation.ValidateThresholds(_expectedFM1Pass, currentState);
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(4));
    }
    
  }
}
