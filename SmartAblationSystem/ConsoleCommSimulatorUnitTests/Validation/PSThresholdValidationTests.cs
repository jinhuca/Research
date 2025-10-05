
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
  public class PSThresholdValidationTests : ThresholdValidationTestBase
  {

    private PS1ThresholdValidation _PS1ThresholdValidation;
    private PT5ThresholdValidation _PT5ThresholdValidation;

    private double _expectedPS1Fail = 980;
    private double _expectedPT5Fail = 810;

    private double _expectedPS1Pass = 780;
    private double _expectedPT5Pass = 710;
    public void DictionaryInitialSetup()
    {
      _PS1ThresholdValidation = new PS1ThresholdValidation(EventAggregatorMock.Object);
      _PT5ThresholdValidation = new PT5ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var PS1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();
      var PT5ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var PS1Thresholds = new Thresholds();
      var PT5Thresholds = new Thresholds();


      PS1Thresholds.HighValue = 850;
      PS1Thresholds.FailValue = 975;
      PS1Thresholds.LowValue = 680;

      PT5Thresholds.HighValue = 800;

      PS1ThresholdsDictionary = AssignThresholdsToStates(PS1ThresholdsDictionary, PS1Thresholds);
      PT5ThresholdsDictionary = AssignThresholdsToStates(PT5ThresholdsDictionary, PT5Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(PS1ThresholdsDictionary, ThresholdType.PS1);
      _PS1ThresholdValidation.HandleThresholdUpdateEvent(args1);
      var args2 = new UpdateThresholdEventArgs(PT5ThresholdsDictionary, ThresholdType.PT5);
      _PS1ThresholdValidation.HandleThresholdUpdateEvent(args2);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
      TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestPS1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _PS1ThresholdValidation.ValidateThresholds(_expectedPS1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _PS1ThresholdValidation.ValidateThresholds(_expectedPS1Fail, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _PS1ThresholdValidation.ValidateThresholds(_expectedPS1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));

    }
    private void TestPT5ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      _PT5ThresholdValidation.ValidateThresholds(_expectedPT5Pass, currentState);

      _PT5ThresholdValidation.ValidateThresholds(_expectedPT5Fail, currentState);
      // since ths is passing, will send another mesage resetting it to be correct
      _PT5ThresholdValidation.ValidateThresholds(_expectedPS1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));


    }
  }
}
