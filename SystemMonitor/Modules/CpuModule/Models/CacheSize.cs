using System.Runtime.InteropServices;

namespace CpuModule.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CacheSize {
  int L1_cache_size;
  int L1_cache_line_size;
  int L2_cache_size;
  int L2_cache_line_size;
  int L3_cache_size;
  int L3_cache_line_size;
}
