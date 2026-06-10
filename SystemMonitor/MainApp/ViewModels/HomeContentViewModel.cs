using CpuModule.ViewModels.Definitions;
using DataStructures.Cpu.Interfaces;
using SharedDefinitions;

namespace MainApp.ViewModels;

public class HomeContentViewModel : BindableBase {
  private readonly IRegionManager _regionManager;
  private IObservable<ICpuSummaryInfo> _cpuSummaryInfoObservable;
  private IObservable<ICpuLiveInfo> _cpuLiveInfoObservable;
  public DelegateCommand<string> NavigateCommand { get; }

  public HomeContentViewModel(IRegionManager regionManager) {
    _regionManager = regionManager;
    NavigateCommand = new DelegateCommand<string>(Navigate);

    _cpuSummaryInfoObservable = CpuInfoServices.Observables.CpuInfoGenerators.GenerateCpuSummaryInfo(TimeSpan.FromSeconds(0));
    _cpuSummaryInfoObservable.Subscribe(
      info => {
        CpuVendor = info.VendorName != null ? ViewModelConversions.VendorNameConvert(info.VendorName) : string.Empty;
        CpuBrand = info.BrandName ?? string.Empty;
      },
      ex => { },
      () => { });

    _cpuLiveInfoObservable = CpuInfoServices.Observables.CpuInfoGenerators.GenerateCpuLiveInfo(TimeSpan.FromSeconds(1));
    _cpuLiveInfoObservable.Subscribe(
      info => {
        CpuClock = info.CpuOverallLiveInfo.CpuSpeed.Value.HasValue ? MathF.Round(info.CpuOverallLiveInfo.CpuSpeed.Value.Value/1000, 2) : 0.0f;
        CpuLoad = info.CpuOverallLiveInfo.TotalLoad.Value.HasValue ? MathF.Round(info.CpuOverallLiveInfo.TotalLoad.Value.Value, 2) : 0.0f;
      },
      ex => { },
      () => { });
  }

  private void Navigate(string viewName) {
    if (!string.IsNullOrEmpty(viewName)) {
      _regionManager.RequestNavigate(RegionNames.MainContentRegionName, viewName);
    }
  }

  private string _cpuVendor = string.Empty;
  public string CpuVendor {
    get => _cpuVendor;
    set => SetProperty(ref _cpuVendor, value);
  }

  private string _cpuBrand = string.Empty;
  public string CpuBrand {
    get => _cpuBrand;
    set => SetProperty(ref _cpuBrand, value);
  }

  private float _cpuClock = 0.0f;
  public float CpuClock {
    get => _cpuClock;
    set => SetProperty(ref _cpuClock, value);
  }

  private float _cpuLoad = 0.0f;
  public float CpuLoad {
    get => _cpuLoad;
    set => SetProperty(ref _cpuLoad, value);
  }
}