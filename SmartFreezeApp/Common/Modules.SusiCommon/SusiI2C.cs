using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiI2C
{
  public const UInt32 SUSI_I2C_MAX_DEVICE = 5;

  public const UInt32 SUSI_ID_I2C_EXTERNAL = 0x00000000;
  public const UInt32 SUSI_ID_I2C_OEM0 = 0x00000001;
  public const UInt32 SUSI_ID_I2C_OEM1 = 0x00000002;
  public const UInt32 SUSI_ID_I2C_OEM2 = 0x00000003;
  public const UInt32 SUSI_ID_I2C_OEM3 = 0x00000004;

  public static UInt32 SUSI_I2C_ENC_7BIT_ADDR(UInt32 addr)
  {
    return (addr << 1);
  }

  public static UInt32 SUSI_I2C_DEC_7BIT_ADDR(UInt32 addr)
  {
    return (addr >> 1);
  }

  public static UInt32 SUSI_I2C_ENC_10BIT_ADDR(UInt32 addr)
  {
    return ((addr & 0x00FF) | ((addr & 0x0300) << 1) | 0xF000);
  }

  public static UInt32 SUSI_I2C_DEC_10BIT_ADDR(UInt32 addr)
  {
    return ((addr & 0x00FF) | ((addr >> 1) & 0x0300));
  }

  public static bool SUSI_I2C_IS_10BIT_ADDR(UInt32 addr)
  {
    return ((addr & 0xF800) == 0xF000);
  }

  public static bool SUSI_I2C_IS_7BIT_ADDR(UInt32 addr)
  {
    return !SUSI_I2C_IS_10BIT_ADDR(addr);
  }

  // Bits 31 & 30 Selects Command Type
  public const UInt32 SUSI_I2C_STD_CMD = (0 << 30);

  public const UInt32 SUSI_I2C_EXT_CMD = ((UInt32)2 << 30);
  public const UInt32 SUSI_I2C_NO_CMD = ((UInt32)1 << 30);
  public const UInt32 SUSI_I2C_CMD_TYPE_MASK = ((UInt32)3 << 30);

  public static UInt32 SUSI_I2C_ENC_STD_CMD(UInt32 cmd)
  {
    return ((cmd & 0xFF) | SUSI_I2C_STD_CMD);
  }

  public static UInt32 SUSI_I2C_ENC_EXT_CMD(UInt32 cmd)
  {
    return ((cmd & 0xFFFF) | SUSI_I2C_EXT_CMD);
  }

  public static UInt32 SUSI_I2C_ENC_NO_CMD(UInt32 cmd)
  {
    return ((cmd & 0xFFFF) | SUSI_I2C_NO_CMD);
  }

  public static bool SUSI_I2C_IS_EXT_CMD(UInt32 cmd)
  {
    return ((cmd & SUSI_I2C_CMD_TYPE_MASK) == SUSI_I2C_EXT_CMD);
  }

  public static bool SUSI_I2C_IS_STD_CMD(UInt32 cmd)
  {
    return ((cmd & SUSI_I2C_CMD_TYPE_MASK) == SUSI_I2C_STD_CMD);
  }

  public static bool SUSI_I2C_IS_NO_CMD(UInt32 cmd)
  {
    return ((cmd & SUSI_I2C_CMD_TYPE_MASK) == SUSI_I2C_NO_CMD);
  }

  // Item ID
  public const UInt32 SUSI_ID_I2C_MAXIMUM_BLOCK_LENGTH = 0x00000000;

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CWriteReadCombine(UInt32 Id, byte Addr, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pWBuffer, UInt32 WriteLen, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] pRBuffer, UInt32 ReadLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CReadTransfer(UInt32 Id, UInt32 Addr, UInt32 Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, UInt32 ReadLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CWriteTransfer(UInt32 Id, UInt32 Addr, UInt32 Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, UInt32 ByteCnt);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CProbeDevice(UInt32 Id, UInt32 Addr);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CGetFrequency(UInt32 Id, out UInt32 pFreq);

  [DllImport("Susi4")]
  public static extern UInt32 SusiI2CSetFrequency(UInt32 Id, UInt32 Freq);
}