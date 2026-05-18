using Converters;
using CpuModule.Models;
using CpuModule.ViewModels_V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
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
    LiveViewModel?.CpuOverallLiveViewModel?.LoadViewModel = _model.LiveInfo.CpuOverallLiveInfo.TotalLoad.val;
    LiveViewModel?.CpuOverallLiveViewModel?.SpeedViewModel = (float)Math.Round((double)_model.LiveInfo.CpuOverallLiveInfo.CpuSpeed.val/1000, 2);
    LiveViewModel?.CpuOverallLiveViewModel?.TemperatureViewModel = _model.LiveInfo.CpuOverallLiveInfo.PackageTemperature.val;

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
