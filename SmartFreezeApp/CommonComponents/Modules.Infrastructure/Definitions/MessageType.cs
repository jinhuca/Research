using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Message type register or value
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum MessageType
{
  readValues = 0,
  registerRomValue = 1, // 0x00c0
}