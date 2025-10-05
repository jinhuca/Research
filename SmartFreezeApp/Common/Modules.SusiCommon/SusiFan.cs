using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiFan
{
  // ID same as SUSI_ID_HWM_FAN_XXX, Example: SUSI_ID_HWM_FAN_CPU

  // Fan control value item IDs
  public const UInt32 SUSI_ID_FC_CONTROL_SUPPORT_FLAGS = 0x00000000;  // Reference "Control Support Flags"

  public const UInt32 SUSI_ID_FC_AUTO_SUPPORT_FLAGS = 0x00000001;     // Reference "Auto Support Flags"

  // Control Support Flags
  public const UInt32 SUSI_FC_FLAG_SUPPORT_OFF_MODE = (1 << 0);     // Support OFF mode

  public const UInt32 SUSI_FC_FLAG_SUPPORT_FULL_MODE = (1 << 1);      // Support FULL mode
  public const UInt32 SUSI_FC_FLAG_SUPPORT_MANUAL_MODE = (1 << 2);  // Support Manual mode
  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_MODE = (1 << 3);      // Support Auto mode

  // Auto Support Flags
  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_LOW_STOP = (1 << 0);      // Support Low Stop Behavior (Depend on Auto mode)

  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_LOW_LIMIT = (1 << 1); // Support Low Limit Behavior (Depend on Auto mode)
  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_HIGH_LIMIT = (1 << 2);  // Support High Limit Behavior (Depend on Auto mode)
  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_PWM = (1 << 8);         // Support PWM operate mode (Depend on Auto mode)
  public const UInt32 SUSI_FC_FLAG_SUPPORT_AUTO_RPM = (1 << 9);         // Support RPM operate mode (Depend on Auto mode)

  [DllImport("Susi4")]
  public static extern UInt32 SusiFanControlGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  public const UInt32 SUSI_FAN_AUTO_CTRL_OPMODE_PWM = 0;
  public const UInt32 SUSI_FAN_AUTO_CTRL_OPMODE_RPM = 1;

  public struct AutoFan
  {
    public UInt32 TmlSource;
    public UInt32 OpMode;
    public UInt32 LowStopLimit;     // Temperature (0.1 Kelvins)
    public UInt32 LowLimit;       // Temperature (0.1 Kelvins)
    public UInt32 HighLimit;    // Temperature (0.1 Kelvins)
    public UInt32 MinPWM;       // Enable when OpMode == FAN_AUTO_CTRL_OPMODE_PWM
    public UInt32 MaxPWM;       // Enable when OpMode == FAN_AUTO_CTRL_OPMODE_PWM
    public UInt32 MinRPM;       // Enable when OpMode == FAN_AUTO_CTRL_OPMODE_RPM
    public UInt32 MaxRPM;       // Enable when OpMode == FAN_AUTO_CTRL_OPMODE_RPM
  }

  // Mode
  public const UInt32 SUSI_FAN_CTRL_MODE_OFF = 0;

  public const UInt32 SUSI_FAN_CTRL_MODE_FULL = 1;
  public const UInt32 SUSI_FAN_CTRL_MODE_MANUAL = 2;
  public const UInt32 SUSI_FAN_CTRL_MODE_AUTO = 3;

  public struct SusiFanControl
  {
    public UInt32 Mode;
    public UInt32 PWM;          // Manual mode only (0 - 100%)
    public AutoFan AutoControl;     // Auto mode only
  }

  [DllImport("Susi4")]
  public static extern UInt32 SusiFanControlGetConfig(UInt32 Id, out SusiFanControl pConfig);

  [DllImport("Susi4")]
  public static extern UInt32 SusiFanControlSetConfig(UInt32 Id, ref SusiFanControl Config);
}