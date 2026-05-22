using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace CpuInfoServices.Methods;

public static class QueryCpuId {
  public static (int family, int model, int stepping) GetCpuFamily() {
    if (X86Base.IsSupported) {
      // Query CPUID leaf 1 (Processor Info and Feature Bits)
      var (eax, ebx, ecx, edx) = X86Base.CpuId(1, 0);

      // Extract values using bitmask operations per CPUID specification
      int stepping = eax & 0xF;
      int baseModel = (eax >> 4) & 0xF;
      int baseFamily = (eax >> 8) & 0xF;
      int extModel = (eax >> 16) & 0xF;
      int extFamily = (eax >> 20) & 0xFF;

      // Compute actual Family and Model numbers based on Intel/AMD specifications
      int family = (baseFamily == 15) ? (baseFamily + extFamily) : baseFamily;
      int model = (baseFamily == 6 || baseFamily == 15) ? ((extModel << 4) + baseModel) : baseModel;

      return (family, model, stepping);
    }
    else {
      Debug.WriteLine("X86/X64 CPUID instruction is not supported on this architecture.");
      return (0, 0, 0);
    }
  }
}
