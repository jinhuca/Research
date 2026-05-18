namespace CpuModule.ViewModels;

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

  private float? _voltageViewModel;
  public float? VoltageViewModel {
    get => _voltageViewModel;
    set => SetProperty(ref _voltageViewModel, value);
  }

  private float _platformPowerValueViewModel;
  public float PlatformPowerValueViewModel {
    get => _platformPowerValueViewModel;
    set => SetProperty(ref _platformPowerValueViewModel, value);
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

  private float _memoryPowerMaxViewModel;
  public float MemoryPowerMaxViewModel {
    get => _memoryPowerMaxViewModel;
    set => SetProperty(ref _memoryPowerMaxViewModel, value);
  }
}
