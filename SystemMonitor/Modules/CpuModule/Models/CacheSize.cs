using System.Runtime.InteropServices;

namespace CpuModule.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CacheSize {
  public int L1_cache_size;
  public int L1_cache_line_size;
  public int L2_cache_size;
  public int L2_cache_line_size;
  public int L3_cache_size;
  public int L3_cache_line_size;
}

public struct ReadableCacheSize {
  public string L1_cache_size;
  public string L1_cache_line_size;
  public string L2_cache_size;
  public string L2_cache_line_size;
  public string L3_cache_size;
  public string L3_cache_line_size;
}

public static class CacheSizeExtension {
  public static ReadableCacheSize ToReadableCacheSize(this CacheSize cacheSize) {
    return new ReadableCacheSize {
      L1_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L1_cache_size),
      L1_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L1_cache_line_size),
      L2_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L2_cache_size),
      L2_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L2_cache_line_size),
      L3_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L3_cache_size),
      L3_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit(cacheSize.L3_cache_line_size)
    };
  }
}