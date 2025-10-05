using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Infrastructure.Definitions
{

  /// <summary>
  /// Message mask
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public enum Mask
  {
    CAN_ID_ELEMENT_MASK = 63, // 0x003f
    CAN_ID_NODE_MASK = 14336, // 0x3800
    CAN_ID_TYPE_MASK = 192,   // 0x00c0
    CAN_ID_STATE_MASK = 1792 //0x0700
  }
}
