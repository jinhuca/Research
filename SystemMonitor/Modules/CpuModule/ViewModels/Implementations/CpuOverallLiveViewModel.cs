using CpuModule.ViewModels.Interfaces;

namespace CpuModule.ViewModels.Implementations;

class CpuOverallLiveViewModel : BindableBase, ICpuOverallLiveViewModel {
  private float? _loadViewModel;
  public float? LoadViewModel {
    get => _loadViewModel ?? 0.0f;
    set {
      _loadViewModel = value;
      RaisePropertyChanged(nameof(LoadViewModel));
    }
  }

  private float? _temperatureViewModel;
  public float? TemperatureViewModel {
    get => _temperatureViewModel ?? 0.0f;
    set {
      _temperatureViewModel = value;
      RaisePropertyChanged(nameof(TemperatureViewModel));
    }
  }

  private float? _speedViewModel;
  public float? SpeedViewModel {
    get => _speedViewModel ?? 0.0f;
    set {
      _speedViewModel = value;
      RaisePropertyChanged(nameof(SpeedViewModel));
    }
  }

  //private float? _voltageViewModel;
  //public float? VoltageViewModel {
  //  get => _voltageViewModel;
  //  set => SetProperty(ref _voltageViewModel, value);
  //}

  private float _platformVoltageValueViewModel = 0.0f;
  public float PlatformVoltageValueViewModel {
    get => _platformVoltageValueViewModel;
    set {
      _platformVoltageValueViewModel = value;
      RaisePropertyChanged(nameof(PlatformVoltageValueViewModel));
    }
  }

  private float _platformVoltageMinViewModel;
  public float PlatformVoltageMinViewModel {
    get => _platformVoltageMinViewModel;
    set {
      _platformVoltageMinViewModel = value;
      RaisePropertyChanged(nameof(PlatformVoltageMinViewModel));
    }
  }

  private float _platformVoltageMaxViewModel;
  public float PlatformVoltageMaxViewModel {
    get => _platformVoltageMaxViewModel;
    set {
      _platformVoltageMaxViewModel = value;
      RaisePropertyChanged(nameof(PlatformVoltageMaxViewModel));
    }
  }

  private float _platformPowerValueViewModel;
  public float PlatformPowerValueViewModel {
    get => _platformPowerValueViewModel;
    set {
      _platformPowerValueViewModel = value;
      RaisePropertyChanged(nameof(PlatformPowerValueViewModel));
    }
  }

  private float _platformPowerMinViewModel;
  public float PlatformPowerMinViewModel {
    get => _platformPowerMinViewModel;
    set {
      _platformPowerMinViewModel = value;
      RaisePropertyChanged(nameof(PlatformPowerMinViewModel));
    }
  }

  private float _platformPowerMaxViewModel;
  public float PlatformPowerMaxViewModel {
    get => _platformPowerMaxViewModel;
    set {
      _platformPowerMaxViewModel = value;
      RaisePropertyChanged(nameof(PlatformPowerMaxViewModel));
    }
  }

  private float _packagePowerValueViewModel;
  public float PackagePowerValueViewModel {
    get => _packagePowerValueViewModel;
    set {
      _packagePowerValueViewModel = value;
      RaisePropertyChanged(nameof(PackagePowerValueViewModel));
    }
  }

  private float _packagePowerMinViewModel;
  public float PackagePowerMinViewModel {
    get => _packagePowerMinViewModel;
    set {
      _packagePowerMinViewModel = value;
      RaisePropertyChanged(nameof(PackagePowerMinViewModel));
    }
  }

  private float _packagePowerMaxViewModel;
  public float PackagePowerMaxViewModel {
    get => _packagePowerMaxViewModel;
    set {
      _packagePowerMaxViewModel = value;
      RaisePropertyChanged(nameof(PackagePowerMaxViewModel));
    }
  }

  private float _coresPowerValueViewModel;
  public float CoresPowerValueViewModel {
    get => _coresPowerValueViewModel;
    set {
      _coresPowerValueViewModel = value;
      RaisePropertyChanged(nameof(CoresPowerValueViewModel));
    }
  }

  private float _coresPowerMinViewModel;
  public float CoresPowerMinViewModel {
    get => _coresPowerMinViewModel;
    set {
      _coresPowerMinViewModel = value;
      RaisePropertyChanged(nameof(CoresPowerMinViewModel));
    }
  }

  private float _coresPowerMaxViewModel;
  public float CoresPowerMaxViewModel {
    get => _coresPowerMaxViewModel;
    set {
      _coresPowerMaxViewModel = value;
      RaisePropertyChanged(nameof(CoresPowerMaxViewModel));
    }
  }

  private float _memoryPowerValueViewModel;
  public float MemoryPowerValueViewModel {
    get => _memoryPowerValueViewModel;
    set {
      _memoryPowerValueViewModel = value;
      RaisePropertyChanged(nameof(MemoryPowerValueViewModel));
    }
  }

  private float _memoryPowerMinViewModel;
  public float MemoryPowerMinViewModel {
    get => _memoryPowerMinViewModel;
    set {
      _memoryPowerMinViewModel = value;
      RaisePropertyChanged(nameof(MemoryPowerMinViewModel));
    }
  }

  private float _memoryPowerMaxViewModel;
  public float MemoryPowerMaxViewModel {
    get => _memoryPowerMaxViewModel;
    set {
      _memoryPowerMaxViewModel = value;
      RaisePropertyChanged(nameof(MemoryPowerMaxViewModel));
    }
  }

  private float _packageTemperatureValueViewModel;
  public float PackageTemperatureValueViewModel {
    get => _packageTemperatureValueViewModel;
    set {
      _packageTemperatureValueViewModel = value;
      RaisePropertyChanged(nameof(PackageTemperatureValueViewModel));
    }
  }

  private float _packageTemperatureMinViewModel;
  public float PackageTemperatureMinViewModel {
    get => _packageTemperatureMinViewModel;
    set {
      _packageTemperatureMinViewModel = value;
      RaisePropertyChanged(nameof(PackageTemperatureMinViewModel));
    }
  }

  private float _packageTemperatureMaxViewModel;
  public float PackageTemperatureMaxViewModel {
    get => _packageTemperatureMaxViewModel;
    set {
      _packageTemperatureMaxViewModel = value;
      RaisePropertyChanged(nameof(PackageTemperatureMaxViewModel));
    }
  }

  private float _coreAvgTemperatureValueViewModel;
  public float CoreAvgTemperatureValueViewModel {
    get => _coreAvgTemperatureValueViewModel;
    set {
      _coreAvgTemperatureValueViewModel = value;
      RaisePropertyChanged(nameof(CoreAvgTemperatureValueViewModel));
    }
  }

  private float _coreAvgTemperatureMinViewModel;
  public float CoreAvgTemperatureMinViewModel {
    get => _coreAvgTemperatureMinViewModel;
    set {
      _coreAvgTemperatureMinViewModel = value;
      RaisePropertyChanged(nameof(CoreAvgTemperatureMinViewModel));
    }
  }

  private float _coreAvgTemperatureMaxViewModel;
  public float CoreAvgTemperatureMaxViewModel {
    get => _coreAvgTemperatureMaxViewModel;
    set {
      _coreAvgTemperatureMaxViewModel = value;
      RaisePropertyChanged(nameof(CoreAvgTemperatureMaxViewModel));
    }
  }

  private float _coreAverageTemperatureMinViewModel;
  public float CoreAverageTemperatureMinViewModel {
    get => _coreAverageTemperatureMinViewModel;
    set {
      _coreAverageTemperatureMinViewModel = value;
      RaisePropertyChanged(nameof(CoreAverageTemperatureMinViewModel));
    }
  }

  private float _coreMaxTemperatureValueViewModel;
  public float CoreMaxTemperatureValueViewModel {
    get => _coreMaxTemperatureValueViewModel;
    set {
      _coreMaxTemperatureValueViewModel = value;
      RaisePropertyChanged(nameof(CoreMaxTemperatureValueViewModel));
    }
  }

  private float _coreMaxTemperatureMinViewModel;
  public float CoreMaxTemperatureMinViewModel {
    get => _coreMaxTemperatureMinViewModel;
    set {
      _coreMaxTemperatureMinViewModel = value;
      RaisePropertyChanged(nameof(CoreMaxTemperatureMinViewModel));
    }
  }

  private float _coreMaxTemperatureMaxViewModel;
  public float CoreMaxTemperatureMaxViewModel {
    get => _coreMaxTemperatureMaxViewModel;
    set {
      _coreMaxTemperatureMaxViewModel = value;
      RaisePropertyChanged(nameof(CoreMaxTemperatureMaxViewModel));
    }
  }

  private int _processNum = 0;
  public int ProcessNum {
    get => _processNum;
    set => SetProperty(ref _processNum, value);
  }

  private int _threadNum = 0;
  public int ThreadNum {
    get => _threadNum;
    set => SetProperty(ref _threadNum, value);
  }

  private int _handleNum = 0;
  public int HandleNum {
    get => _handleNum;
    set => SetProperty(ref _handleNum, value);
  }

  private TimeSpan _upTime;
  public TimeSpan UpTime {
    get => _upTime;
    set => SetProperty(ref _upTime, value);
  }
}
