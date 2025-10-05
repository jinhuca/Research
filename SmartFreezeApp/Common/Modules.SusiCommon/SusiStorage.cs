using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiStorage
{
  public const UInt32 SUSI_ID_STORAGE_MAX = 3;

  public const UInt32 SUSI_ID_STORAGE_STD = 0x00000000;
  public const UInt32 SUSI_ID_STORAGE_OEM0 = 0x00000001;
  public const UInt32 SUSI_ID_STORAGE_OEM1 = 0x00000002;

  // Item ID
  public const UInt32 SUSI_ID_STORAGE_TOTAL_SIZE = 0x00000000;

  public const UInt32 SUSI_ID_STORAGE_BLOCK_SIZE = 0x00000001;
  public const UInt32 SUSI_ID_STORAGE_LOCK_STATUS = 0x00010000;
  public const UInt32 SUSI_ID_STORAGE_PSW_MAX_LEN = 0x00010001;

  // Lock status
  public const UInt32 SUSI_STORAGE_STATUS_LOCK = 1;

  public const UInt32 SUSI_STORAGE_STATUS_UNLOCK = 0;

  [DllImport("Susi4")]
  public static extern UInt32 SusiStorageGetCaps(UInt32 Id, UInt32 ItemId, out UInt32 pValue);

  [DllImport("Susi4")]
  public static extern UInt32 SusiStorageAreaRead(UInt32 Id, UInt32 Offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pBuffer, UInt32 BufLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiStorageAreaWrite(UInt32 Id, UInt32 Offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pBuffer, UInt32 BufLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiStorageAreaSetUnlock(UInt32 Id, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuffer, UInt32 BufLen);

  [DllImport("Susi4")]
  public static extern UInt32 SusiStorageAreaSetLock(UInt32 Id, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuffer, UInt32 BufLen);
}