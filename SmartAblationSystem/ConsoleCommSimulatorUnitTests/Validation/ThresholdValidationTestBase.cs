using ConsoleCommSimulator.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.CanBusMessageDefinition;

namespace ConsoleSimulatorUnitTests.MessageProviders
{
  public class ThresholdValidationTestBase : MessageProviderTestBase
  {
    protected Dictionary<MessageStateId, Thresholds> AssignThresholdsToStates(Dictionary<MessageStateId, Thresholds> thresholdsDictionary, Thresholds thresholds)
    {
      MessageStateId[] states = new MessageStateId[]
      {
        MessageStateId.CAN_ID_STATE_READY,
        MessageStateId.CAN_ID_STATE_INFLATION,
        MessageStateId.CAN_ID_STATE_TRANSITION,
        MessageStateId.CAN_ID_STATE_ABLATION,
        MessageStateId.CAN_ID_STATE_THAWING,
        MessageStateId.CAN_ID_STATE_IDLE
      };

      foreach (var state in states)
      {
        thresholdsDictionary[state] = thresholds;
      }
      return thresholdsDictionary;
    }
  }
}
