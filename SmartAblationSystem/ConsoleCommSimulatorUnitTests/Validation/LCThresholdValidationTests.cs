
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
  public class LCThresholdValidationTests : ThresholdValidationTestBase
  {

    private LC1ThresholdValidation _LC1ThresholdValidation;

    private double _expectedLC1Fail = 2;
    private double _expectedLC1Warn = 3;

    private double _expectedLC1Pass = 5;
    public void DictionaryInitialSetup()
    {
      _LC1ThresholdValidation = new LC1ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var LC1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var LC1Thresholds = new Thresholds();

      LC1Thresholds.LowValue = 3.5;
      LC1Thresholds.FailValue = 2.5;

      LC1ThresholdsDictionary = AssignThresholdsToStates(LC1ThresholdsDictionary, LC1Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(LC1ThresholdsDictionary, ThresholdType.LC1);
      _LC1ThresholdValidation.HandleThresholdUpdateEvent(args1);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestLC1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _LC1ThresholdValidation.ValidateThresholds(_expectedLC1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _LC1ThresholdValidation.ValidateThresholds(_expectedLC1Fail, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _LC1ThresholdValidation.ValidateThresholds(_expectedLC1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));
      // test if sends a warning message 
      _LC1ThresholdValidation.ValidateThresholds(_expectedLC1Warn, currentState);
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(3));
    }
  }
}
