using DataStructures.Cpu.Implementations;

namespace CpuModule.ViewModels;

public interface ICpuSummaryViewModel {
  string? BrandNameViewModel { get; set; }
  string? VendorNameViewModel { get; set; }
  int? FamilyIdViewModel { get; set; }
  int? ModelIdViewModel { get; set; }
  int? SteppingIdViewModel { get; set; }
  string? BaseSpeedViewModel { get; set; }
  string? BusSpeedViewModel { get; set; }
  int? SocketNumViewModel { get; set; }
  int? PhysicalCoreNumViewModel { get; set; }
  int? LogicalCoreNumViewModel { get; set; }
  bool? VirtualizationViewModel { get; set; }
  CpuCacheInfoViewModel? CacheInfoViewModel { get; set; }
  CpuInstructionInfo2? InstructionSetViewModel { get; set; }
}
