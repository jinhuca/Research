
using System.Collections.Generic;
using Communication;
using ConsoleCommSimulator.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  public abstract class MessageProviderTestBase
  {

    protected Mock<IEventAggregator> EventAggregatorMock { get; set; }
    protected Mock<CanBusMessageUpdateEvent> CanBusUpdateEventMock { get; set; }
    protected Mock<SystemStateUpdateEvent> SystemStateUpdateEventMock { get; set; }
    protected Mock<UpdateThresholdEvent> UpdateThresholdEventMock { get; set; }
    protected Mock<ThresholdValidationFailedEvent> ThresholdValidationFailedEventMock { get; set; }


    protected static IDictionary<CanBusMessageDefinition.MessageStateId, string> StateIdToStringMap =>
      new Dictionary<CanBusMessageDefinition.MessageStateId, string>()
      {
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, "IDLE"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, "READY"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, "INFLATION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, "TRANSITION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, "ABLATION"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, "THAWING"},
        {CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, "EXCEPTION"}
      };

    [TestInitialize]
    public virtual void Initialize()
    {
      // mock an event, if it gets called, return event.object
      EventAggregatorMock = new Mock<IEventAggregator>();
      CanBusUpdateEventMock = new Mock<CanBusMessageUpdateEvent>();

      EventAggregatorMock
        .Setup(x => x.GetEvent<CanBusMessageUpdateEvent>())
        .Returns(CanBusUpdateEventMock.Object);

      SystemStateUpdateEventMock = new Mock<SystemStateUpdateEvent>();
      EventAggregatorMock
        .Setup(x => x.GetEvent<SystemStateUpdateEvent>())
        .Returns(SystemStateUpdateEventMock.Object);

      UpdateThresholdEventMock = new Mock<UpdateThresholdEvent>();
      EventAggregatorMock
        .Setup(x => x.GetEvent<UpdateThresholdEvent>())
        .Returns(UpdateThresholdEventMock.Object);

      ThresholdValidationFailedEventMock = new Mock<ThresholdValidationFailedEvent>();
      EventAggregatorMock
        .Setup(x => x.GetEvent<ThresholdValidationFailedEvent>())
        .Returns(ThresholdValidationFailedEventMock.Object);
    }

    protected static uint CreateMessageId(CanBusMessageDefinition.MessageStateId stateId, uint nodeId, uint typeId, uint messageId, uint priorityId = 3)
    {
      // the messageId is an int but to understand we need binary
      // priority is always 3 in normal conditions (by default)
      // input messageId is the elementId
      // we can get the type and node based on elementId
      uint priorityid = priorityId << 14;
      uint nodeid = nodeId << 11;
      uint typeid = typeId << 6;
      uint elementid = messageId & 0x3F;

      return priorityid | nodeid | (uint)stateId | typeid | elementid;
    }

  }
}
