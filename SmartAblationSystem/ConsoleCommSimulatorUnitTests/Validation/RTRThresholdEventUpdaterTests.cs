
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
  public class RTRThresholdEventUpdaterTests : MessageProviderTestBase
  {

    private RTRThresholdEventUpdater _rTRUpdater;


    [TestMethod]
    public void ProviderInitialize_Test_State_IDLE()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_READY()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_INFLATION()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION);
    }
    [TestMethod]
    public void ProviderInitialize_Test_State_TRANSITION()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_ABLATION()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
    }

    [TestMethod]
    public void ProviderInitialize_Test_State_THAWING()
    {
      TestRTRUpdate(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING);
    }


    private void TestRTRUpdate(CanBusMessageDefinition.MessageStateId currentState)
    {
      _rTRUpdater = new RTRThresholdEventUpdater(EventAggregatorMock.Object);

      // Create a list to store the published events
      List<UpdateThresholdEventArgs> publishedEvents = new List<UpdateThresholdEventArgs>();

      // Capture the published events using Callback
      UpdateThresholdEventMock
          .Setup(x => x.Publish(It.IsAny<UpdateThresholdEventArgs>()))
          .Callback<UpdateThresholdEventArgs>(args => publishedEvents.Add(args));

      var newPT1Threshold = new Dictionary<MessageStateId, Thresholds>();
      var pt1Thresholds = new Thresholds
      {
        // mock sending some values
        HighValue = 680,
        FailValue = 850,
        LowValue = 975
      };
      newPT1Threshold[currentState] = pt1Thresholds;

      var newPT2Threshold = new Dictionary<MessageStateId, Thresholds>();
      var pt2Thresholds = new Thresholds();
      // mock sending some values
      pt2Thresholds.FailValue = 800;
      newPT2Threshold[currentState] = pt2Thresholds;

      var newPT3Threshold = new Dictionary<MessageStateId, Thresholds>();
      var pt3Thresholds = new Thresholds();
      // mock sending some values
      pt3Thresholds.FailValue = 30;
      newPT3Threshold[currentState] = pt3Thresholds;

      var newPT4Threshold = new Dictionary<MessageStateId, Thresholds>();
      var pt4Thresholds = new Thresholds();
      // mock sending some values
      pt4Thresholds.FailValue = 11;
      newPT4Threshold[currentState] = pt4Thresholds;

      var args1 = new UpdateThresholdEventArgs(newPT1Threshold, ThresholdType.PT1);
      var args2 = new UpdateThresholdEventArgs(newPT2Threshold, ThresholdType.PT2);
      var args3 = new UpdateThresholdEventArgs(newPT3Threshold, ThresholdType.PT3);
      var args4 = new UpdateThresholdEventArgs(newPT4Threshold, ThresholdType.PT4);

      _rTRUpdater.PublishUpdate(args1);
      _rTRUpdater.PublishUpdate(args2);
      _rTRUpdater.PublishUpdate(args3);
      _rTRUpdater.PublishUpdate(args4);

      // Verify the number of published events
      Assert.AreEqual(4, publishedEvents.Count);

      // Verify the contents of the published events
      Assert.AreEqual(ThresholdType.PT1, publishedEvents[0].ThresholdName);
      Assert.AreEqual(ThresholdType.PT2, publishedEvents[1].ThresholdName);
      Assert.AreEqual(ThresholdType.PT3, publishedEvents[2].ThresholdName);
      Assert.AreEqual(ThresholdType.PT4, publishedEvents[3].ThresholdName);
      Assert.AreEqual(newPT1Threshold, publishedEvents[0].ThresholdDictionary);
      Assert.AreEqual(newPT2Threshold, publishedEvents[1].ThresholdDictionary);
      Assert.AreEqual(newPT3Threshold, publishedEvents[2].ThresholdDictionary);
      Assert.AreEqual(newPT4Threshold, publishedEvents[3].ThresholdDictionary);
    }


  }
}
