using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;

namespace Module.Console.Models
{
  /// <summary>
  /// This class is for DMS Logic
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public static class DMSLogic
  {
    private const double noPacing = -2;
    private const double thereIsPacing = -1;

    /// <summary>
    /// Gets DMS State
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dmsValue">dms value</param>
    /// <param name="systemSate">system sate</param>
    /// <returns></returns>
    public static bool GetDMSState(double dmsValue, CanBusMessageDefinition.MessageStateId systemSate)
    {
      switch ((int)systemSate)
      {

        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
          if (dmsValue == noPacing)
          {
            return false;
          }

          else if (dmsValue == thereIsPacing)
          {
            return true;
          }
          break;


        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
        case (int)CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
          if (dmsValue == noPacing)
          {
            return false;
          }

          else if (dmsValue >= 0)
          {
            return true;
          }
          break; ;

        default:
          return false;

      }

      return false;
    }
  }
}
