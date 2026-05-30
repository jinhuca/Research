using Windows.Win32;

namespace CrystalMonitor.Interop;

#if NET8_0_OR_GREATER

internal static class ByteExtensions {
  public static byte[] ToArray(this __byte_4 bytes) {
    return bytes.AsReadOnlySpan().ToArray();
  }
}

#endif
