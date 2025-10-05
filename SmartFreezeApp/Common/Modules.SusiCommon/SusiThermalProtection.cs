using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiThermalProtection
{
  public const UInt32 SUSI_ID_THERMAL_MAX = 4;
  public const UInt32 SUSI_ID_THERMAL_PROTECT_1 = 0x00000000;
  public const UInt32 SUSI_ID_THERMAL_PROTECT_2 = 0x00000001;
  public const UInt32 SUSI_ID_THERMAL_PROTECT_3 = 0x00000002;
  public const UInt32 SUSI_ID_THERMAL_PROTECT_4 = 0x00000003;

  // Thermal Protection value item IDs
  public const UInt32 SUSI_ID_TP_EVENT_SUPPORT_FLAGS = 0x00000000;  // Reference "Support Flags"

  public const UInt32 SUSI_ID_TP_EVENT_TRIGGER_MAXIMUM = 0x00000001;  // Send Event
  public const UInt32 SUSI_ID_TP_EVENT_TRIGGER_MINIMUM = 0x00000002;
  public const UInt32 SUSI_ID_TP_EVENT_CLEAR_MAXIMUM = 0x00000003;
  public const UInt32 SUSI_ID_TP_EVENT_CLEAR_MINIMUM = 0x00000004;

  // Thermal Protection Support Flags
  public const UInt32 SUSI_THERMAL_FLAG_SUPPORT_SHUTDOWN = 0x00000001;

  public const UInt32 SUSI_THERMAL_FLAG_SUPPORT_THROTTLE = 0x00000002;
  public const UInt32 SUSI_THERMAL_FLAG_SUPPORT_POWEROFF = 0x00000004;

  [DllImport("Susi4")]
  public static extern UInt32 SusiThermalProtectionGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  // Thermal Protection Event type
  public const UInt32 SUSI_THERMAL_EVENT_SHUTDOWN = 0x00;

  public const UInt32 SUSI_THERMAL_EVENT_THROTTLE = 0x01;
  public const UInt32 SUSI_THERMAL_EVENT_POWEROFF = 0x02;
  public const UInt32 SUSI_THERMAL_EVENT_NONE = 0xFF;

  public struct SusiThermalProtect
  {
    public UInt32 SourceId;
    public UInt32 EventType;
    public UInt32 SendEventTemperature;   // 0.1 Kelvins
    public UInt32 ClearEventTemperature;    // 0.1 Kelvins
  }

  [DllImport("Susi4")]
  public static extern UInt32 SusiThermalProtectionSetConfig(UInt32 Id, ref SusiThermalProtect pConfig);

  [DllImport("Susi4")]
  public static extern UInt32 SusiThermalProtectionGetConfig(UInt32 Id, out SusiThermalProtect pConfig);
}