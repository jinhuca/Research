using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Console.Helpers
{
  /// <summary>
  /// Manage FirmwareBootLoader Definitions
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class Definitions
  {
    /// <summary>
    /// Board types enumeration.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum Board
    {
      unknown = 0,
      CMCU = 1,
      PMCU = 2,
      Repeater = 3,
      ICB = 4,
      Catheter = 5,
      CPLD = 6
    }
  }
}
