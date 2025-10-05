using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiWDog
{
  public const UInt32 SUSI_ID_WATCHDOG_MAX = 3;

  public const UInt32 SUSI_ID_WATCHDOG_1 = 0x00000000;
  public const UInt32 SUSI_ID_WATCHDOG_2 = 0x00000001;
  public const UInt32 SUSI_ID_WATCHDOG_3 = 0x00000002;

  // Event Types
  public const UInt32 SUSI_WDT_EVENT_TYPE_NONE = 0x00000000;

  public const UInt32 SUSI_WDT_EVENT_TYPE_IRQ = 0x00000001;
  public const UInt32 SUSI_WDT_EVENT_TYPE_SCI = 0x00000002;
  public const UInt32 SUSI_WDT_EVENT_TYPE_PWRBTN = 0x00000003;

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate void SUSI_WDT_INT_CALLBACK(IntPtr context);

  // Item ID
  public const UInt32 SUSI_ID_WDT_SUPPORT_FLAGS = 0x00000000;

  public const UInt32 SUSI_ID_WDT_DELAY_MAXIMUM = 0x00000001;
  public const UInt32 SUSI_ID_WDT_DELAY_MINIMUM = 0x00000002;
  public const UInt32 SUSI_ID_WDT_EVENT_MAXIMUM = 0x00000003;
  public const UInt32 SUSI_ID_WDT_EVENT_MINIMUM = 0x00000004;
  public const UInt32 SUSI_ID_WDT_RESET_MAXIMUM = 0x00000005;
  public const UInt32 SUSI_ID_WDT_RESET_MINIMUM = 0x00000006;
  public const UInt32 SUSI_ID_WDT_UNIT_MINIMUM = 0x0000000F;
  public const UInt32 SUSI_ID_WDT_DELAY_TIME = 0x00010001;
  public const UInt32 SUSI_ID_WDT_EVENT_TIME = 0x00010002;
  public const UInt32 SUSI_ID_WDT_RESET_TIME = 0x00010003;
  public const UInt32 SUSI_ID_WDT_EVENT_TYPE = 0x00010004;

  // Support Flags
  public const UInt32 SUSI_WDT_FLAG_SUPPORT_IRQ = 0x00000002;

  public const UInt32 SUSI_WDT_FLAG_SUPPORT_SCI = 0x00000004;
  public const UInt32 SUSI_WDT_FLAG_SUPPORT_PWRBTN = 0x00000008;

  [DllImport("Susi4")]
  public static extern UInt32 SusiWDogGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  [DllImport("Susi4")]
  public static extern UInt32 SusiWDogStart(UInt32 Id, UInt32 DelayTime, UInt32 EventTime, UInt32 ResetTime, UInt32 EventType);

  [DllImport("Susi4")]
  public static extern UInt32 SusiWDogStop(UInt32 Id);

  [DllImport("Susi4")]
  public static extern UInt32 SusiWDogTrigger(UInt32 Id);

  [DllImport("Susi4")]
  public static extern UInt32 SusiWDogSetCallBack(UInt32 Id, IntPtr pfnCallback, IntPtr Context);
}
