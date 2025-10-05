
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using SmartAblationSystem.Helpers;
using static Communication.CanBusMessageDefinition;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  [TestClass]
  public class TCThresholdValidationTests : ThresholdValidationTestBase
  {

    private TC1ThresholdValidation _TC1ThresholdValidation;

    private double _expectedTC1FailLow = -71;

    private double _expectedTC1Pass = 8;
    public void DictionaryInitialSetup()
    {
      _TC1ThresholdValidation = new TC1ThresholdValidation(EventAggregatorMock.Object);
      // put mock values into dictionary
      var TC1ThresholdsDictionary = new Dictionary<MessageStateId, Thresholds>();

      var TC1Thresholds = new Thresholds();


      TC1Thresholds.HighValue = 10;
      TC1Thresholds.LowValue = 4.7;

      TC1ThresholdsDictionary = AssignThresholdsToStates(TC1ThresholdsDictionary, TC1Thresholds);

      // update dictionary
      var args1 = new UpdateThresholdEventArgs(TC1ThresholdsDictionary, ThresholdType.TC);
      _TC1ThresholdValidation.HandleThresholdUpdateEvent(args1);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }

    private void TestTC1ThresholdValidation(CanBusMessageDefinition.MessageStateId currentState)
    {
      DictionaryInitialSetup();
      // need to check if it sends a threshold failed event when validation fails 
      // verify nothing gets sent if the values pass the validation
      _TC1ThresholdValidation.ValidateThresholds(_expectedTC1Pass, currentState);
      
      // verify validation is correct, this will fail if dict is empty
      // the validate Thresholds function only starts checking once there is more than 6 states in the dict
      _TC1ThresholdValidation.ValidateThresholds(_expectedTC1FailLow, currentState);
      // once previously, + 0 = still once
      // since ths is passing, will send another mesage resetting it to be correct
      _TC1ThresholdValidation.ValidateThresholds(_expectedTC1Pass, currentState);
      // two messages total
      ThresholdValidationFailedEventMock.Verify(x => x.Publish(It.IsAny<ThresholdValidationFailedEventArgs>()), Times.Exactly(2));
    }
    
  }
}
