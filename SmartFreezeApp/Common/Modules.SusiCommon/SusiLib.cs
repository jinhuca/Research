using System.Runtime.InteropServices;

namespace Modules.SusiCommon;

public static class SusiLib
{
  [DllImport("Susi4")]
  public static extern UInt32 SusiLibInitialize();

  [DllImport("Susi4")]
  public static extern UInt32 SusiLibUninitialize();
}