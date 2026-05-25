using CpuModule.ViewModels.Interfaces;
using DataStructures.Cpu.Implementations;
using System.ComponentModel.DataAnnotations;

namespace CpuModule.ViewModels.Implementations;

public class CpuSummaryViewModel : BindableBase, ICpuSummaryViewModel {
  public CpuSummaryViewModel() { }

  [Required]
  public string BrandNameViewModel {
    get;
    set => SetProperty(ref field, value);
  } = string.Empty;

  [Required]
  public string VendorNameViewModel {
    get;
    set => SetProperty(ref field, value);
  } = string.Empty;

  [Required]
  public int FamilyIdViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public int ModelIdViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public int SteppingIdViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public string BaseSpeedViewModel {
    get;
    set => SetProperty(ref field, value);
  } = string.Empty;

  [Required]
  public string BusSpeedViewModel {
    get;
    set => SetProperty(ref field, value);
  } = string.Empty;

  [Required]
  public int SocketNumViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public int PhysicalCoreNumViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public int LogicalCoreNumViewModel {
    get;
    set => SetProperty(ref field, value);
  } = 0;

  [Required]
  public bool VirtualizationViewModel {
    get;
    set => SetProperty(ref field, value);
  } = false;

  [field: Required]
  public CpuCacheInfoViewModel CacheInfoViewModel {
    get;
    set => SetProperty(ref field, value);
  } = new CpuCacheInfoViewModel();

  [Required]
  public Dictionary<string, bool> CpuInstructionsViewModel {
    get;
    set => SetProperty(ref field, value);
  } = new Dictionary<string, bool>();
}