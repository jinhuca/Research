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
}
