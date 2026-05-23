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
  }

  [Required] 
  public string VendorNameViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public int FamilyIdViewModel {
    get;
    set => SetProperty(ref field, value);
  }
  
  [Required]
  public int ModelIdViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public int SteppingIdViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public string BaseSpeedViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public string BusSpeedViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public int SocketNumViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public int PhysicalCoreNumViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public int LogicalCoreNumViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [Required]
  public bool VirtualizationViewModel {
    get;
    set => SetProperty(ref field, value);
  }

  [field: Required]
  public CpuCacheInfoViewModel CacheInfoViewModel {
    get;
    set => SetProperty(ref field, value);
  } = new CpuCacheInfoViewModel();

  //private CpuInstructionInfo? _instructionInfo2ViewModel;
  //public CpuInstructionInfo? InstructionSetViewModel {
  //  get => _instructionInfo2ViewModel;
  //  set => SetProperty(ref _instructionInfo2ViewModel, value);
  //}

  [Required]
  public Dictionary<string, bool> CpuInstructionsViewModel {
    get;
    set => SetProperty(ref field, value);
  }
}
