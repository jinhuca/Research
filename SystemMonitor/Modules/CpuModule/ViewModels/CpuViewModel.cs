using Converters;
using CpuModule.Models;

namespace CpuModule.ViewModels;

public class CpuViewModel : BindableBase {
  private readonly CpuModel _model;
  public CpuViewModel(CpuModel model) {
    _model = model;
    initProperties();
  }

  private void initProperties() {
    Name = _model.BrandName ?? string.Empty;
    BaseSpeed = HzUnitConverter.ConvertMHzToReadableUnit(_model.BasicInfo?.BaseSpeed ?? 0);
    SocketNum = _model.BasicInfo?.SocketNum ?? 0;
    NumOfPhysicalCores = _model.BasicInfo?.NumOfPhysicalCores ?? 0;
    NumOfLogicalCores = _model.BasicInfo?.NumOfLogicalCores ?? 0;
    VirtualizationEnabled = _model.BasicInfo?.VirtualizationEnabled ?? false;
    L1CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize?.L1_cache_size ?? 0);
    L2CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize?.L2_cache_size ?? 0);
    L3CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize?.L3_cache_size ?? 0);
  }

  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private string _baseSpeed = string.Empty;
  public string BaseSpeed {
    get => _baseSpeed;
    set => SetProperty(ref _baseSpeed, value);
  }

  private int _socketNum;
  public int SocketNum {
    get => _socketNum;
    set => SetProperty(ref _socketNum, value);
  }

  private int _numOfPhysicalCores;
  public int NumOfPhysicalCores {
    get => _numOfPhysicalCores;
    set => SetProperty(ref _numOfPhysicalCores, value);
  }

  private int _numOfLogicalCores;
  public int NumOfLogicalCores {
    get => _numOfLogicalCores;
    set => SetProperty(ref _numOfLogicalCores, value);
  }

  private bool _virtualizationEnabled;
  public bool VirtualizationEnabled {
    get => _virtualizationEnabled;
    set => SetProperty(ref _virtualizationEnabled, value);
  }

  private string? _L1CacheSize;
  public string? L1CacheSize {
    get => _L1CacheSize;
    set => SetProperty(ref _L1CacheSize, value);
  }

  private string? _L2CacheSize;
  public string? L2CacheSize {
    get => _L2CacheSize;
    set => SetProperty(ref _L2CacheSize, value);
  }

  private string? _L3CacheSize;
  public string? L3CacheSize {
    get => _L3CacheSize;
    set => SetProperty(ref _L3CacheSize, value);
  }
}
