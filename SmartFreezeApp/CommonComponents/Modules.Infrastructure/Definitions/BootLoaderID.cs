namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Boot loader ID
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum BootLoaderID
{
  CAN_ID_BOOT_XFR = 58,
  CAN_ID_BOOT_START = 59,
  CAN_ID_BOOT_INIT = 60,
  CAN_ID_BOOT_END = 61
}