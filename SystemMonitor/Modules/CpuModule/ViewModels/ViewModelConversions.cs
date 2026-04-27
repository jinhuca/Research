namespace CpuModule.ViewModels; 

internal static class ViewModelConversions {
  internal static string BrandNameConvert(string name) {
    if (name is null) throw new ArgumentNullException(nameof(name));
    if(name.Contains(Definitions.Intel, StringComparison.OrdinalIgnoreCase)) {
      return Definitions.IntelBrandName;
    }
    if(name.Contains(Definitions.Amd, StringComparison.OrdinalIgnoreCase)) {
      return Definitions.AmdBrandName;
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