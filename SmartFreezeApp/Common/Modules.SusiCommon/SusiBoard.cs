using System.Runtime.InteropServices;
using System.Text;

namespace Modules.SusiCommon;

public static class SusiBoard
{
  public const UInt32 SUSI_ID_UNKNOWN = 0xFFFFFFFF;

  public const UInt32 SUSI_ID_GET_SPEC_VERSION = 0x00000000;
  public const UInt32 SUSI_ID_BOARD_BOOT_COUNTER_VAL = 0x00000001;
  public const UInt32 SUSI_ID_BOARD_RUNNING_TIME_METER_VAL = 0x00000002;
  public const UInt32 SUSI_ID_BOARD_PNPID_VAL = 0x00000003;
  public const UInt32 SUSI_ID_BOARD_PLATFORM_REV_VAL = 0x00000004;
  public const UInt32 SUSI_ID_BOARD_DRIVER_VERSION_VAL = 0x00010000;
  public const UInt32 SUSI_ID_BOARD_LIB_VERSION_VAL = 0x00010001;
  public const UInt32 SUSI_ID_BOARD_FIRMWARE_VERSION_VAL = 0x00010002;

  public const UInt32 SUSI_ID_HWM_TEMP_MAX = 10;
  public const UInt32 SUSI_ID_HWM_TEMP_CPU = 0x00020000;
  public const UInt32 SUSI_ID_HWM_TEMP_CHIPSET = SUSI_ID_HWM_TEMP_CPU + 1;
  public const UInt32 SUSI_ID_HWM_TEMP_SYSTEM = SUSI_ID_HWM_TEMP_CPU + 2;
  public const UInt32 SUSI_ID_HWM_TEMP_CPU2 = SUSI_ID_HWM_TEMP_CPU + 3;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM0 = SUSI_ID_HWM_TEMP_CPU + 4;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM1 = SUSI_ID_HWM_TEMP_CPU + 5;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM2 = SUSI_ID_HWM_TEMP_CPU + 6;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM3 = SUSI_ID_HWM_TEMP_CPU + 7;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM4 = SUSI_ID_HWM_TEMP_CPU + 8;
  public const UInt32 SUSI_ID_HWM_TEMP_OEM5 = SUSI_ID_HWM_TEMP_CPU + 9;

  public const UInt32 SUSI_ID_HWM_VOLTAGE_MAX = 23;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_VCORE = 0x00021000;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_VCORE2 = SUSI_ID_HWM_VOLTAGE_VCORE + 1;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_2V5 = SUSI_ID_HWM_VOLTAGE_VCORE + 2;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_3V3 = SUSI_ID_HWM_VOLTAGE_VCORE + 3;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_5V = SUSI_ID_HWM_VOLTAGE_VCORE + 4;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_12V = SUSI_ID_HWM_VOLTAGE_VCORE + 5;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_5VSB = SUSI_ID_HWM_VOLTAGE_VCORE + 6;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_3VSB = SUSI_ID_HWM_VOLTAGE_VCORE + 7;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_VBAT = SUSI_ID_HWM_VOLTAGE_VCORE + 8;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_5NV = SUSI_ID_HWM_VOLTAGE_VCORE + 9;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_12NV = SUSI_ID_HWM_VOLTAGE_VCORE + 10;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_VTT = SUSI_ID_HWM_VOLTAGE_VCORE + 11;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_24V = SUSI_ID_HWM_VOLTAGE_VCORE + 12;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_DC = SUSI_ID_HWM_VOLTAGE_VCORE + 13;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_DCSTBY = SUSI_ID_HWM_VOLTAGE_VCORE + 14;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_VBATLI = SUSI_ID_HWM_VOLTAGE_VCORE + 15;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_OEM0 = SUSI_ID_HWM_VOLTAGE_VCORE + 16;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_OEM1 = SUSI_ID_HWM_VOLTAGE_VCORE + 17;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_OEM2 = SUSI_ID_HWM_VOLTAGE_VCORE + 18;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_OEM3 = SUSI_ID_HWM_VOLTAGE_VCORE + 19;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_1V05 = SUSI_ID_HWM_VOLTAGE_VCORE + 20;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_1V5 = SUSI_ID_HWM_VOLTAGE_VCORE + 21;
  public const UInt32 SUSI_ID_HWM_VOLTAGE_1V8 = SUSI_ID_HWM_VOLTAGE_VCORE + 22;

  public const UInt32 SUSI_ID_HWM_FAN_MAX = 10;
  public const UInt32 SUSI_ID_HWM_FAN_CPU = 0x00022000;
  public const UInt32 SUSI_ID_HWM_FAN_SYSTEM = SUSI_ID_HWM_FAN_CPU + 1;
  public const UInt32 SUSI_ID_HWM_FAN_CPU2 = SUSI_ID_HWM_FAN_CPU + 2;
  public const UInt32 SUSI_ID_HWM_FAN_OEM0 = SUSI_ID_HWM_FAN_CPU + 3;
  public const UInt32 SUSI_ID_HWM_FAN_OEM1 = SUSI_ID_HWM_FAN_CPU + 4;
  public const UInt32 SUSI_ID_HWM_FAN_OEM2 = SUSI_ID_HWM_FAN_CPU + 5;
  public const UInt32 SUSI_ID_HWM_FAN_OEM3 = SUSI_ID_HWM_FAN_CPU + 6;
  public const UInt32 SUSI_ID_HWM_FAN_OEM4 = SUSI_ID_HWM_FAN_CPU + 7;
  public const UInt32 SUSI_ID_HWM_FAN_OEM5 = SUSI_ID_HWM_FAN_CPU + 8;
  public const UInt32 SUSI_ID_HWM_FAN_OEM6 = SUSI_ID_HWM_FAN_CPU + 9;

  public const UInt32 SUSI_ID_HWM_CURRENT_MAX = 3;
  public const UInt32 SUSI_ID_HWM_CURRENT_OEM0 = 0x00023000;
  public const UInt32 SUSI_ID_HWM_CURRENT_OEM1 = SUSI_ID_HWM_CURRENT_OEM0 + 1;
  public const UInt32 SUSI_ID_HWM_CURRENT_OEM2 = SUSI_ID_HWM_CURRENT_OEM0 + 2;

  public const UInt32 SUSI_ID_HWM_CASEOPEN_MAX = 3;
  public const UInt32 SUSI_ID_HWM_CASEOPEN_OEM0 = 0x00024000;
  public const UInt32 SUSI_ID_HWM_CASEOPEN_OEM1 = SUSI_ID_HWM_CASEOPEN_OEM0 + 1;
  public const UInt32 SUSI_ID_HWM_CASEOPEN_OEM2 = SUSI_ID_HWM_CASEOPEN_OEM0 + 2;
  public const UInt32 CASEOPEN_CLEAR_CMD = 0x55AA55AA;

  public const UInt32 SUSI_ID_SMBUS_SUPPORTED = 0x00030000;
  public const UInt32 SUSI_SMBUS_EXTERNAL_SUPPORTED = (1 << 0);
  public const UInt32 SUSI_SMBUS_OEM0_SUPPORTED = (1 << 1);
  public const UInt32 SUSI_SMBUS_OEM1_SUPPORTED = (1 << 2);
  public const UInt32 SUSI_SMBUS_OEM2_SUPPORTED = (1 << 3);
  public const UInt32 SUSI_SMBUS_OEM3_SUPPORTED = (1 << 4);
  public const UInt32 SUSI_ID_I2C_SUPPORTED = 0x00030100;
  public const UInt32 SUSI_I2C_EXTERNAL_SUPPORTED = (1 << 0);
  public const UInt32 SUSI_I2C_OEM0_SUPPORTED = (1 << 1);
  public const UInt32 SUSI_I2C_OEM1_SUPPORTED = (1 << 2);
  public const UInt32 SUSI_I2C_OEM2_SUPPORTED = (1 << 3);

  public const UInt32 SUSI_KELVINS_OFFSET = 2731;

  public static UInt32 SusiEncodeCelcius(UInt32 Celsius)
  {
    return (((Celsius) * 10) + SUSI_KELVINS_OFFSET);
  }

  public static float SusiDecodeCelcius(UInt32 KelvinsTenth)
  {
    return (((int)KelvinsTenth - SUSI_KELVINS_OFFSET) / 10f);
  }

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardGetValue(UInt32 Id, ref UInt32 pValue);

  public const UInt32 SUSI_ID_GET_NAME_MASK = 0xF0000000;

  public static UInt32 SUSI_ID_GET_NAME_BASE(UInt32 Id)
  {
    return (Id & SUSI_ID_GET_NAME_MASK);
  }

  public const UInt32 SUSI_ID_BASE_GET_NAME_HWM = 0x00000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_HWM_FANCONTROL = 0x10000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_I2C = 0x20000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_SMB = 0x30000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_GPIO = 0x40000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_WDT = 0x50000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_VGA_BACKLIGHT = 0x60000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_STORAGE = 0x70000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_TML = 0x80000000;
  public const UInt32 SUSI_ID_BASE_GET_NAME_INFO = 0x90000000;

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_HWM(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_HWM);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_HWM_FANCONTROL(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_HWM_FANCONTROL);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_I2C(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_I2C);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_SMB(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_SMB);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_GPIO(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_GPIO);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_WDT(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_WDT);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_VGA_BACKLIGHT(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_VGA_BACKLIGHT);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_STORAGE(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_STORAGE);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_TML(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_TML);
  }

  public static UInt32 SUSI_ID_MAPPING_GET_NAME_INFO(UInt32 Id)
  {
    return (Id | SUSI_ID_BASE_GET_NAME_INFO);
  }

  public const UInt32 SUSI_ID_BOARD_MANUFACTURER_STR = 0x00000000;
  public const UInt32 SUSI_ID_BOARD_NAME_STR = 0x00000001;
  public const UInt32 SUSI_ID_BOARD_REVISION_STR = 0x00000002;
  public const UInt32 SUSI_ID_BOARD_SERIAL_STR = 0x00000003;
  public const UInt32 SUSI_ID_BOARD_BIOS_REVISION_STR = 0x00000004;
  public const UInt32 SUSI_ID_BOARD_HW_REVISION_STR = 0x00000005;
  public const UInt32 SUSI_ID_BOARD_PLATFORM_TYPE_STR = 0x00000006;

  public const UInt32 SUSI_ID_BOARD_OEM0_STR = 0x00000010;
  public const UInt32 SUSI_ID_BOARD_OEM1_STR = 0x00000011;
  public const UInt32 SUSI_ID_BOARD_OEM2_STR = 0x00000012;

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardGetStringA(UInt32 Id, StringBuilder pBuffer, ref UInt32 pBufLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardReadIO(UInt16 Port, ref UInt32 pValue, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardWriteIO(UInt16 Port, UInt32 Value, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardReadPCI(byte Bus, byte Device, byte Function, UInt32 Offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] pData, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardWritePCI(byte Bus, byte Device, byte Function, UInt32 Offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] pData, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardReadMemory(UInt32 Address, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pData, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardWriteMemory(UInt32 Address, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pData, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardReadMSR(UInt32 index, ref UInt32 EAX, ref UInt32 EDX);

  [DllImport("Susi4")]
  public static extern UInt32 SusiBoardWriteMSR(UInt32 index, UInt32 EAX, UInt32 EDX);
}