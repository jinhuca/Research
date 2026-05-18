using DataStructures.Cpu.Implementations;

namespace CpuModule.ViewModels;

public class CpuSummaryViewModel : BindableBase, ICpuSummaryViewModel {
  public CpuSummaryViewModel() { }

  private string? _brandNameViewModel;
  public string? BrandNameViewModel {
    get => _brandNameViewModel;
    set => SetProperty(ref _brandNameViewModel, value);
  }

  private string? _vendorNameViewModel;
  public string? VendorNameViewModel {
    get => _vendorNameViewModel;
    set => SetProperty(ref _vendorNameViewModel, value);
  }

  private int? _familyIdViewModel;
  public int? FamilyIdViewModel {
    get => _familyIdViewModel;
    set => SetProperty(ref _familyIdViewModel, value);
  }

  private int? _modelIdViewModel;
  public int? ModelIdViewModel {
    get => _modelIdViewModel;
    set => SetProperty(ref _modelIdViewModel, value);
  }

  private int? _steppingIdViewModel;
  public int? SteppingIdViewModel {
    get => _steppingIdViewModel;
    set => SetProperty(ref _steppingIdViewModel, value);
  }

  private string? _baseSpeedViewModel;
  public string? BaseSpeedViewModel {
    get => _baseSpeedViewModel;
    set => SetProperty(ref _baseSpeedViewModel, value);
  }

  private string? _busSpeedViewModel;
  public string? BusSpeedViewModel {
    get => _busSpeedViewModel;
    set => SetProperty(ref _busSpeedViewModel, value);
  }

  private int? _socketNumViewModel;
  public int? SocketNumViewModel {
    get => _socketNumViewModel;
    set => SetProperty(ref _socketNumViewModel, value);
  }

  private int? _physicalCoreNumViewModel;
  public int? PhysicalCoreNumViewModel {
    get => _physicalCoreNumViewModel;
    set => SetProperty(ref _physicalCoreNumViewModel, value);
  }

  private int? _logicalCoreNumViewModel;
  public int? LogicalCoreNumViewModel {
    get => _logicalCoreNumViewModel;
    set => SetProperty(ref _logicalCoreNumViewModel, value);
  }

  private bool? _virtualization;
  public bool? VirtualizationViewModel {
    get => _virtualization;
    set => SetProperty(ref _virtualization, value);
  }

  private CpuCacheInfoViewModel? _cacheInfoViewModel;
  public CpuCacheInfoViewModel? CacheInfoViewModel {
    get => _cacheInfoViewModel;
    set => SetProperty(ref _cacheInfoViewModel, value);
  }

  private CpuInstructionInfo2? _instructionInfo2ViewModel;
  public CpuInstructionInfo2? InstructionSetViewModel {
    get => _instructionInfo2ViewModel;
    set => SetProperty(ref _instructionInfo2ViewModel, value);
  }
}
