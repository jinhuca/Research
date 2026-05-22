using DataStructures.Cpu.Implementations;

namespace CpuModule.ViewModels.Definitions;

internal static class ViewModelConversions {
  internal static string VendorNameConvert(string name) {
    if (name is null) throw new ArgumentNullException(nameof(name));
    if (name.Contains(Definitions.Intel, StringComparison.OrdinalIgnoreCase)) {
      return Definitions.Intel;
    }
    if (name.Contains(Definitions.Amd, StringComparison.OrdinalIgnoreCase)) {
      return Definitions.Amd;
    }
    return Definitions.UnknownBrandName;
  }
}

internal class Definitions {
  internal const string Intel = "Intel";
  internal const string IntelBrandName = "Intel Corporation";
  internal const string Amd = "AMD";
  internal const string AmdBrandName = "Advanced Micro Devices, Inc.";
  internal const string UnknownBrandName = "Unknown";
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
  public static ReadableCacheSize ToReadableCacheSize(this CpuCacheInfo cacheSize) {
    return new ReadableCacheSize {
      L1_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L1_cache_size),
      L1_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L1_cache_line_size),
      L2_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L2_cache_size),
      L2_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L2_cache_line_size),
      L3_cache_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L3_cache_size),
      L3_cache_line_size = Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)cacheSize.L3_cache_line_size)
    };
  }
}