
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
  public class CPThresholdValidationTests : ThresholdValidationTestBase
  {

    private CP1ThresholdValidation _CP1ThresholdValidation;
    private CP2ThresholdValidation _CP2ThresholdValidation;

    private double _expectedCP1FailHigh = 11;

    private double _expectedCP1Pass = 8;

    private double _expectedCP2FailHigh = 11000;

    private double _expectedCP2Pass = -100;
    public void DictionaryInitialSetup()
    {
      _CP1ThresholdValidation = new CP1ThresholdValidation(EventAggregatorMock.Object);
      _CP2ThresholdValidation = new CP2ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var CPThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var CP1Thresholds = new Thresholds();
      var CP2Thresholds = new Thresholds();

      CP1Thresholds.InnerValue = 4.7; // inner low 
      CP1Thresholds.HighValue = 10;
      CP2Thresholds.OuterValue = 1000;

      CPThresholdsDictionary = AssignThresholdsToStates(CPThresholdsDictionary, CP1Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(CPThresholdsDictionary, ThresholdType.CP);
      _CP1ThresholdValidation.HandleThresholdUpdateEvent(args1);
      _CP2ThresholdValidation.HandleThresholdUpdateEvent(args1);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestCP1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _CP1ThresholdValidation.ValidateThresholds(_expectedCP1Pass, currentState);

      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _CP1ThresholdValidation.ValidateThresholds(_expectedCP1FailHigh, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _CP1ThresholdValidation.ValidateThresholds(_expectedCP1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _CP2ThresholdValidation.ValidateThresholds(_expectedCP2Pass, currentState);

      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _CP2ThresholdValidation.ValidateThresholds(_expectedCP2FailHigh, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _CP2ThresholdValidation.ValidateThresholds(_expectedCP2Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(4));
    }
    
  }
}
