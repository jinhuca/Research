
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
  public class PTThresholdValidationTests : ThresholdValidationTestBase
  {

    private PT1ThresholdValidation _pT1ThresholdValidation;
    private PT2ThresholdValidation _pT2ThresholdValidation;
    private PT3ThresholdValidation _pT3ThresholdValidation;
    private PT4ThresholdValidation _pT4ThresholdValidation;

    private double _expectedPT1Fail = 980;
    private double _expectedPT2Fail = 810;
    private double _expectedPT3Fail = 35;
    private double _expectedPT4Fail = 15;

    private double _expectedPT1Pass = 780;
    private double _expectedPT2Pass = 710;
    private double _expectedPT3Pass = 25;
    private double _expectedPT4Pass = 5;
    public void DictionaryInitialSetup()
    {
      _pT1ThresholdValidation = new PT1ThresholdValidation(EventAggregatorMock.Object);
      _pT2ThresholdValidation = new PT2ThresholdValidation(EventAggregatorMock.Object);
      _pT3ThresholdValidation = new PT3ThresholdValidation(EventAggregatorMock.Object);
      _pT4ThresholdValidation = new PT4ThresholdValidation(EventAggregatorMock.Object);

      // put mock values into dictionary
      var pt1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();
      var pt2ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();
      var pt3ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();
      var pt4ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var pt1Thresholds = new Thresholds();
      var pt2Thresholds = new Thresholds();
      var pt3Thresholds = new Thresholds();
      var pt4Thresholds = new Thresholds();

      pt1Thresholds.HighValue = 850;
      pt1Thresholds.FailValue = 975;
      pt1Thresholds.LowValue = 680;

      pt2Thresholds.HighValue = 800;

      pt3Thresholds.HighValue = 30;

      pt4Thresholds.HighValue = 11;

      pt1ThresholdsDictionary = AssignThresholdsToStates(pt1ThresholdsDictionary, pt1Thresholds);
      pt2ThresholdsDictionary = AssignThresholdsToStates(pt2ThresholdsDictionary, pt2Thresholds);
      pt3ThresholdsDictionary = AssignThresholdsToStates(pt3ThresholdsDictionary, pt3Thresholds);
      pt4ThresholdsDictionary = AssignThresholdsToStates(pt4ThresholdsDictionary, pt4Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(pt1ThresholdsDictionary, ThresholdType.PT1);
      _pT1ThresholdValidation.HandleThresholdUpdateEvent(args1);
      var args2 = new UpdateThresholdEventArgs(pt2ThresholdsDictionary, ThresholdType.PT2);
      _pT1ThresholdValidation.HandleThresholdUpdateEvent(args2);
      var args3 = new UpdateThresholdEventArgs(pt3ThresholdsDictionary, ThresholdType.PT3);
      _pT1ThresholdValidation.HandleThresholdUpdateEvent(args3);
      var args4 = new UpdateThresholdEventArgs(pt4ThresholdsDictionary, ThresholdType.PT4);
      _pT1ThresholdValidation.HandleThresholdUpdateEvent(args4);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
      TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
      TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
      TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestPT1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Fail, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));

    }
    private void TestPT2ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      _pT2ThresholdValidation.ValidateThresholds(_expectedPT2Pass, currentState);

      _pT2ThresholdValidation.ValidateThresholds(_expectedPT2Fail, currentState);
      // since ths is passing, will send another mesage resetting it to be correct
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));


    }
    private void TestPT3ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 

      _pT3ThresholdValidation.ValidateThresholds(_expectedPT3Pass, currentState);

      _pT3ThresholdValidation.ValidateThresholds(_expectedPT3Fail, currentState);
      // since ths is passing, will send another mesage resetting it to be correct
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));

    }
    private void TestPT4ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 

      _pT4ThresholdValidation.ValidateThresholds(_expectedPT4Pass, currentState);

      _pT4ThresholdValidation.ValidateThresholds(_expectedPT4Fail, currentState);
      // since ths is passing, will send another mesage resetting it to be correct
      _pT1ThresholdValidation.ValidateThresholds(_expectedPT1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));

    }
  }
}
