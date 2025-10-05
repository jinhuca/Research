using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiSMB
{
  public const UInt32 SUSI_SMBUS_MAX_DEVICE = 5;

  public const UInt32 SUSI_ID_SMBUS_EXTERNAL = 0x00000000;
  public const UInt32 SUSI_ID_SMBUS_OEM0 = 0x00000001;
  public const UInt32 SUSI_ID_SMBUS_OEM1 = 0x00000002;
  public const UInt32 SUSI_ID_SMBUS_OEM2 = 0x00000003;
  public const UInt32 SUSI_ID_SMBUS_OEM3 = 0x00000004;

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBReadByte(UInt32 Id, byte Addr, byte Cmd, out byte pBuffer);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBWriteByte(UInt32 Id, byte Addr, byte Cmd, byte Data);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBReadWord(UInt32 Id, byte Addr, byte Cmd, out UInt16 pBuffer);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBWriteWord(UInt32 Id, byte Addr, byte Cmd, UInt16 Data);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBReceiveByte(UInt32 Id, byte Addr, out byte pBuffer);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBSendByte(UInt32 Id, byte Addr, byte Data);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBReadQuick(UInt32 Id, byte Addr);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBWriteQuick(UInt32 Id, byte Addr);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBReadBlock(UInt32 Id, byte Addr, byte Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, ref UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBWriteBlock(UInt32 Id, byte Addr, byte Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBI2CReadBlock(UInt32 Id, byte Addr, byte Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, UInt32 Length);

  [DllImport("Susi4")]
  public static extern UInt32 SusiSMBI2CWriteBlock(UInt32 Id, byte Addr, byte Cmd, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] pBuffer, UInt32 Length);
}