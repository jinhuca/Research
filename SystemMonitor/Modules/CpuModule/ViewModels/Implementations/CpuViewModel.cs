using Converters;
using CpuModule.Models;
using CpuModule.ViewModels.Definitions;
using CpuModule.ViewModels.Interfaces;
using System.ComponentModel;
using static Converters.ByteUnitConverters;

namespace CpuModule.ViewModels.Implementations;

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
    if (LiveViewModel == null || LiveViewModel.CpuOverallLiveViewModel == null || _model.LiveInfo.CpuOverallLiveInfo == null)
      return;

    var cpuModelValue_ = _model.LiveInfo.CpuOverallLiveInfo;
    if (cpuModelValue_ == null)
      return;

    var cpuViewModel_ = LiveViewModel.CpuOverallLiveViewModel;
    if (cpuViewModel_ == null)
      return;

    cpuViewModel_.LoadViewModel = _model.LiveInfo.CpuOverallLiveInfo.TotalLoad.Value;

    cpuViewModel_.SpeedViewModel = cpuModelValue_.CpuSpeed.Value.HasValue
      ? (float)Math.Round((double)cpuModelValue_.CpuSpeed.Value / 1000, 2) : 0.0f;

    cpuViewModel_.TemperatureViewModel = cpuModelValue_.PackageTemperature.Value;
    cpuViewModel_.VoltageViewModel = cpuModelValue_.Voltage.Value;

    cpuViewModel_.PlatformPowerValueViewModel = cpuModelValue_.PlatformPower.Value ?? cpuViewModel_.PlatformPowerValueViewModel;
    cpuViewModel_.PlatformPowerMinViewModel = cpuModelValue_.PlatformPower.Min ?? cpuViewModel_.PlatformPowerMinViewModel;
    cpuViewModel_.PlatformPowerMaxViewModel = cpuModelValue_.PlatformPower.Max ?? cpuViewModel_.PlatformPowerMaxViewModel;

    cpuViewModel_.PackagePowerValueViewModel = cpuModelValue_.PackagePower.Value ?? cpuViewModel_.PackagePowerValueViewModel;
    cpuViewModel_.PackagePowerMinViewModel = cpuModelValue_.PackagePower.Min ?? cpuViewModel_.PackagePowerMinViewModel;
    cpuViewModel_.PackagePowerMaxViewModel = cpuModelValue_.PackagePower.Max ?? cpuViewModel_.PackagePowerMaxViewModel;

    cpuViewModel_.CoresPowerValueViewModel = cpuModelValue_.CoresPower.Value ?? cpuViewModel_.CoresPowerValueViewModel;
    cpuViewModel_.CoresPowerMinViewModel = cpuModelValue_.CoresPower.Min ?? cpuViewModel_.CoresPowerMinViewModel;
    cpuViewModel_.CoresPowerMaxViewModel = cpuModelValue_.CoresPower.Max ?? cpuViewModel_.CoresPowerMaxViewModel;

    cpuViewModel_.MemoryPowerValueViewModel = cpuModelValue_.MemoryPower.Value ?? cpuViewModel_.MemoryPowerValueViewModel;
    cpuViewModel_.MemoryPowerMinViewModel = cpuModelValue_.MemoryPower.Min ?? cpuViewModel_.MemoryPowerMinViewModel;
    cpuViewModel_.MemoryPowerMaxViewModel = cpuModelValue_.MemoryPower.Max ?? cpuViewModel_.MemoryPowerMaxViewModel;

    cpuViewModel_.PackageTemperatureValueViewModel = cpuModelValue_.PackageTemperature.Value ?? cpuViewModel_.PackageTemperatureValueViewModel;
    cpuViewModel_.PackageTemperatureMinViewModel = cpuModelValue_.PackageTemperature.Min ?? cpuViewModel_.PackageTemperatureMinViewModel;
    cpuViewModel_.PackageTemperatureMaxViewModel = cpuModelValue_.PackageTemperature.Max ?? cpuViewModel_.PackageTemperatureMaxViewModel;

    cpuViewModel_.CoreAvgTemperatureValueViewModel = cpuModelValue_.CoreAvgTemperature.Value ?? cpuViewModel_.CoreAvgTemperatureValueViewModel;
    cpuViewModel_.CoreAvgTemperatureMinViewModel = cpuModelValue_.CoreAvgTemperature.Min ?? cpuViewModel_.CoreAvgTemperatureMinViewModel;
    cpuViewModel_.CoreAvgTemperatureMaxViewModel = cpuModelValue_.CoreAvgTemperature.Max ?? cpuViewModel_.CoreAvgTemperatureMaxViewModel;

    cpuViewModel_.CoreMaxTemperatureValueViewModel = cpuModelValue_.CoreMaxTemperature.Value ?? cpuViewModel_.CoreMaxTemperatureValueViewModel;
    cpuViewModel_.CoreMaxTemperatureMinViewModel = cpuModelValue_.CoreMaxTemperature.Min ?? cpuViewModel_.CoreMaxTemperatureMinViewModel;
    cpuViewModel_.CoreMaxTemperatureMaxViewModel = cpuModelValue_.CoreMaxTemperature.Max ?? cpuViewModel_.CoreMaxTemperatureMaxViewModel;
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
