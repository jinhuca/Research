using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiGPIO
{
  public static UInt32 SUSI_ID_GPIO(UInt32 Num)
  {
    return Num;
  }

  public static UInt32 SUSI_ID_GPIO_BANK(UInt32 BankNum)
  {
    return BankNum + 0x00010000;
  }

  public static UInt32 SUSI_ID_GPIO_PIN_BANK(UInt32 GPIO_NUM)
  {
    return (0x00010000 | ((GPIO_NUM) >> 5));
  }

  // Item ID
  public const UInt32 SUSI_ID_GPIO_INPUT_SUPPORT = 0;

  public const UInt32 SUSI_ID_GPIO_OUTPUT_SUPPORT = 1;

  [DllImport("Susi4")]
  public static extern UInt32 SusiGPIOGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  public const UInt32 SUSI_GPIO_OUTPUT = 0;
  public const UInt32 SUSI_GPIO_INPUT = 1;

  [DllImport("Susi4")]
  public static extern UInt32 SusiGPIOGetDirection(UInt32 Id, UInt32 Bitmask, out UInt32 pDirection);

  [DllImport("Susi4")]
  public static extern UInt32 SusiGPIOSetDirection(UInt32 Id, UInt32 Bitmask, UInt32 Direction);

  public const UInt32 SUSI_GPIO_LOW = 0;
  public const UInt32 SUSI_GPIO_HIGH = 1;

  [DllImport("Susi4")]
  public static extern UInt32 SusiGPIOGetLevel(UInt32 Id, UInt32 Bitmask, out UInt32 pLevel);

  [DllImport("Susi4")]
  public static extern UInt32 SusiGPIOSetLevel(UInt32 Id, UInt32 Bitmask, UInt32 Level);
}