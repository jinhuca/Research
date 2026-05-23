using CpuModule.ViewModels.Interfaces;

namespace CpuModule.ViewModels.Implementations;

class CpuOverallLiveViewModel : BindableBase, ICpuOverallLiveViewModel {
  private float? _loadViewModel;
  public float? LoadViewModel {
    get => _loadViewModel ?? 0.0f;
    set => SetProperty(ref _loadViewModel, value);
  }

  private float? _temperatureViewModel;
  public float? TemperatureViewModel {
    get => _temperatureViewModel ?? 0.0f;
    set => SetProperty(ref _temperatureViewModel, value);
  }

  private float? _speedViewModel;
  public float? SpeedViewModel {
    get => _speedViewModel ?? 0.0f;
    set => SetProperty(ref _speedViewModel, value);
  }

  //private float? _voltageViewModel;
  //public float? VoltageViewModel {
  //  get => _voltageViewModel;
  //  set => SetProperty(ref _voltageViewModel, value);
  //}

  private float _platformVoltageValueViewModel = 0.0f;
  public float PlatformVoltageValueViewModel {
    get => _platformVoltageValueViewModel;
    set => SetProperty(ref _platformVoltageValueViewModel, value);
  }

  private float _platformVoltageMinViewModel;
  public float PlatformVoltageMinViewModel {
    get => _platformVoltageMinViewModel;
    set => SetProperty(ref _platformVoltageMinViewModel, value);
  }

  private float _platformVoltageMaxViewModel;
  public float PlatformVoltageMaxViewModel {
    get => _platformVoltageMaxViewModel;
    set => SetProperty(ref _platformVoltageMaxViewModel, value);
  }

  private float _platformPowerValueViewModel;
  public float PlatformPowerValueViewModel {
    get => _platformPowerValueViewModel;
    set => SetProperty(ref _platformPowerValueViewModel, value);
  }

  private float _platformPowerMinViewModel;
  public float PlatformPowerMinViewModel {
    get => _platformPowerMinViewModel;
    set => SetProperty(ref _platformPowerMinViewModel, value);
  }

  private float _platformPowerMaxViewModel;
  public float PlatformPowerMaxViewModel {
    get => _platformPowerMaxViewModel;
    set => SetProperty(ref _platformPowerMaxViewModel, value);
  }

  private float _packagePowerValueViewModel;
  public float PackagePowerValueViewModel {
    get => _packagePowerValueViewModel;
    set => SetProperty(ref _packagePowerValueViewModel, value);
  }

  private float _packagePowerMinViewModel;
  public float PackagePowerMinViewModel {
    get => _packagePowerMinViewModel;
    set => SetProperty(ref _packagePowerMinViewModel, value);
  }

  private float _packagePowerMaxViewModel;
  public float PackagePowerMaxViewModel {
    get => _packagePowerMaxViewModel;
    set => SetProperty(ref _packagePowerMaxViewModel, value);
  }

  private float _coresPowerValueViewModel;
  public float CoresPowerValueViewModel {
    get => _coresPowerValueViewModel;
    set => SetProperty(ref _coresPowerValueViewModel, value);
  }

  private float _coresPowerMinViewModel;
  public float CoresPowerMinViewModel {
    get => _coresPowerMinViewModel;
    set => SetProperty(ref _coresPowerMinViewModel, value);
  }

  private float _coresPowerMaxViewModel;
  public float CoresPowerMaxViewModel {
    get => _coresPowerMaxViewModel;
    set => SetProperty(ref _coresPowerMaxViewModel, value);
  }

  private float _memoryPowerValueViewModel;
  public float MemoryPowerValueViewModel {
    get => _memoryPowerValueViewModel;
    set => SetProperty(ref _memoryPowerValueViewModel, value);
  }

  private float _memoryPowerMinViewModel;
  public float MemoryPowerMinViewModel {
    get => _memoryPowerMinViewModel;
    set => SetProperty(ref _memoryPowerMinViewModel, value);
  }

  private float _memoryPowerMaxViewModel;
  public float MemoryPowerMaxViewModel {
    get => _memoryPowerMaxViewModel;
    set => SetProperty(ref _memoryPowerMaxViewModel, value);
  }

  private float _packageTemperatureValueViewModel;
  public float PackageTemperatureValueViewModel {
    get => _packageTemperatureValueViewModel;
    set => SetProperty(ref _packageTemperatureValueViewModel, value);
  }

  private float _packageTemperatureMinViewModel;
  public float PackageTemperatureMinViewModel {
    get => _packageTemperatureMinViewModel;
    set => SetProperty(ref _packageTemperatureMinViewModel, value);
  }

  private float _packageTemperatureMaxViewModel;
  public float PackageTemperatureMaxViewModel {
    get => _packageTemperatureMaxViewModel;
    set => SetProperty(ref _packageTemperatureMaxViewModel, value);
  }

  private float _coreAvgTemperatureValueViewModel;
  public float CoreAvgTemperatureValueViewModel {
    get => _coreAvgTemperatureValueViewModel;
    set => SetProperty(ref _coreAvgTemperatureValueViewModel, value);
  }

  private float _coreAvgTemperatureMinViewModel;
  public float CoreAvgTemperatureMinViewModel {
    get => _coreAvgTemperatureMinViewModel;
    set => SetProperty(ref _coreAvgTemperatureMinViewModel, value);
  }

  private float _coreAvgTemperatureMaxViewModel;
  public float CoreAvgTemperatureMaxViewModel {
    get => _coreAvgTemperatureMaxViewModel;
    set => SetProperty(ref _coreAvgTemperatureMaxViewModel, value);
  }

  private float _coreAverageTemperatureMinViewModel;
  public float CoreAverageTemperatureMinViewModel {
    get => _coreAverageTemperatureMinViewModel;
    set => SetProperty(ref _coreAverageTemperatureMinViewModel, value);
  }

  private float _coreMaxTemperatureValueViewModel;
  public float CoreMaxTemperatureValueViewModel {
    get => _coreMaxTemperatureValueViewModel;
    set => SetProperty(ref _coreMaxTemperatureValueViewModel, value);
  }

  private float _coreMaxTemperatureMinViewModel;
  public float CoreMaxTemperatureMinViewModel {
    get => _coreMaxTemperatureMinViewModel;
    set => SetProperty(ref _coreMaxTemperatureMinViewModel, value);
  }

  private float _coreMaxTemperatureMaxViewModel;
  public float CoreMaxTemperatureMaxViewModel {
    get => _coreMaxTemperatureMaxViewModel;
    set => SetProperty(ref _coreMaxTemperatureMaxViewModel, value);
  }

}
