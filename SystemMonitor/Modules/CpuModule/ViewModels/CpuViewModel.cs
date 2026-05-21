using Converters;
using CpuModule.Models;
using CpuModule.ViewModels_V2;
using System.ComponentModel;
using static Converters.ByteUnitConverters;

namespace CpuModule.ViewModels;

public class CpuViewModel : BindableBase, ICpuViewModel {
  private CpuModel _model;
  public CpuViewModel(CpuModel model) {
    _model = model;
    _model.PropertyChanged += Model_PropertyChanged;
  }

  private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
    switch (e.PropertyName) {
      case nameof(_model.SummaryInfo):
        UpdateSummaryViewModel();
        break;
      case nameof(_model.LiveInfo):
        UpdateLiveViewModel();
        break;
    }
  }

  private void UpdateSummaryViewModel() {
    SummaryViewModel.BrandNameViewModel = _model.SummaryInfo.BrandName;
    SummaryViewModel.VendorNameViewModel = ViewModelConversions.VendorNameConvert(_model.SummaryInfo.VendorName);
    SummaryViewModel.FamilyIdViewModel = _model.SummaryInfo.FamilyId;
    SummaryViewModel.ModelIdViewModel = _model.SummaryInfo.ModelId;
    SummaryViewModel.SteppingIdViewModel = _model.SummaryInfo.SteppingId;

    SummaryViewModel.BaseSpeedViewModel = _model.SummaryInfo.BaseSpeed.HasValue
      ? HzUnitConverter.ConvertMHzToReadableUnit((double)_model.SummaryInfo.BaseSpeed)
      : string.Empty;

    SummaryViewModel.BusSpeedViewModel = _model.SummaryInfo.BusSpeed.HasValue
      ? HzUnitConverter.ConvertMHzToReadableUnit((double)_model.SummaryInfo.BusSpeed)
      : string.Empty;

    SummaryViewModel.SocketNumViewModel = _model.SummaryInfo.SocketNum;
    SummaryViewModel.PhysicalCoreNumViewModel = _model.SummaryInfo.PhysicalCoreNum;
    SummaryViewModel.LogicalCoreNumViewModel = _model.SummaryInfo.LogicalCoreNum;
    SummaryViewModel.VirtualizationViewModel = _model.SummaryInfo.Virtualization;
    if (_model.SummaryInfo.CacheInfo.HasValue) {
      var cacheViewModel_ = new CpuCacheInfoViewModel() {
        L1_Cache_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L1_cache_size),
        L1_Cache_Line_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L1_cache_line_size),
        L2_Cache_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L2_cache_size),
        L2_Cache_Line_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L2_cache_line_size),
        L3_Cache_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L3_cache_size),
        L3_Cache_Line_size = ConvertBytesToReadableUnit((ulong)_model.SummaryInfo.CacheInfo.Value.L3_cache_line_size),
      };
      SummaryViewModel.CacheInfoViewModel = cacheViewModel_;
    }

    SummaryViewModel.InstructionSetViewModel = _model.SummaryInfo.InstructionSet;
    //RaisePropertyChanged(nameof(SummaryViewModel));
  }

  private void UpdateLiveViewModel() {
    LiveViewModel?.CpuOverallLiveViewModel?.LoadViewModel = _model.LiveInfo.CpuOverallLiveInfo.TotalLoad.Value;
    LiveViewModel?.CpuOverallLiveViewModel?.SpeedViewModel = (float)Math.Round((double)_model.LiveInfo.CpuOverallLiveInfo.CpuSpeed.Value / 1000, 2);
    LiveViewModel?.CpuOverallLiveViewModel?.TemperatureViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackageTemperature.Value;
    LiveViewModel?.CpuOverallLiveViewModel?.VoltageViewModel = _model.LiveInfo.CpuOverallLiveInfo.Voltage.Value;

    LiveViewModel?.CpuOverallLiveViewModel.PlatformPowerValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.PlatformPower.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.PlatformPowerMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.PlatformPower.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.PackagePowerValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackagePower.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.PackagePowerMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackagePower.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.CoresPowerValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoresPower.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.CoresPowerMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoresPower.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.MemoryPowerValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.MemoryPower.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.MemoryPowerMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.MemoryPower.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.PackageTemperatureValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackageTemperature.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.PackageTemperatureMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackageTemperature.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.CoreAvgTemperatureValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.CoreAvgTemperatureMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature.Max.Value;

    LiveViewModel?.CpuOverallLiveViewModel.CoreMaxTemperatureValueViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature.Value.Value;
    LiveViewModel?.CpuOverallLiveViewModel.CoreMaxTemperatureMaxViewModel = _model.LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature.Max.Value;
  }

  private ICpuSummaryViewModel _summaryViewModel = new CpuSummaryViewModel();
  public ICpuSummaryViewModel SummaryViewModel {
    get => _summaryViewModel;
    set => SetProperty(ref _summaryViewModel, value);
  }
  private ICpuLiveViewModel _liveViewModel = new CpuLiveViewModel();
  public ICpuLiveViewModel LiveViewModel {
    get => _liveViewModel;
    set => SetProperty(ref _liveViewModel, value);
  }
}
