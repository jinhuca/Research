using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiVga
{
  public const UInt32 SUSI_ID_BACKLIGHT_MAX = 3;

  public const UInt32 SUSI_ID_BACKLIGHT_1 = 0x00000000;
  public const UInt32 SUSI_ID_BACKLIGHT_2 = 0x00000001;
  public const UInt32 SUSI_ID_BACKLIGHT_3 = 0x00000002;

  // Item ID
  public const UInt32 SUSI_ID_VGA_BRIGHTNESS_MAXIMUM = 0x00010000;

  public const UInt32 SUSI_ID_VGA_BRIGHTNESS_MINIMUM = 0x00010001;

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  public const UInt32 SUSI_BACKLIGHT_SET_OFF = 0;
  public const UInt32 SUSI_BACKLIGHT_SET_ON = 1;

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetBacklightEnable(UInt32 Id, out UInt32 pEnable);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetBacklightEnable(UInt32 Id, UInt32 Enable);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetBacklightBrightness(UInt32 Id, out UInt32 pBright);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetBacklightBrightness(UInt32 Id, UInt32 Bright);

  public const UInt32 SUSI_BACKLIGHT_LEVEL_MAXIMUM = 9;
  public const UInt32 SUSI_BACKLIGHT_LEVEL_MINIMUM = 0;

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetBacklightLevel(UInt32 Id, out UInt32 pLevel);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetBacklightLevel(UInt32 Id, UInt32 Level);

  public const UInt32 SUSI_SCREEN_ON = 1;
  public const UInt32 SUSI_SCREEN_OFF = 0;

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetScreenEnable(UInt32 Id, out UInt32 pEnable);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetScreenEnable(UInt32 Id, UInt32 Enable);

  public const UInt32 SUSI_BACKLIGHT_POLARITY_ON = 1;
  public const UInt32 SUSI_BACKLIGHT_POLARITY_OFF = 0;

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetPolarity(UInt32 Id, out UInt32 pPolarity);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetPolarity(UInt32 Id, UInt32 Polarity);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaGetFrequency(UInt32 Id, out UInt32 pFrequency);

  [DllImport("Susi4")]
  public static extern UInt32 SusiVgaSetFrequency(UInt32 Id, UInt32 Frequency);
}