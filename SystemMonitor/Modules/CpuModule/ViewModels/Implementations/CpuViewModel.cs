using Converters;
using CpuModule.Models;
using CpuModule.ViewModels.Definitions;
using CpuModule.ViewModels.Interfaces;
using DataStructures.Cpu.Implementations;
using LibreHardwareMonitor.Hardware.Motherboard;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
    SummaryViewModel.BrandNameViewModel = _model.SummaryInfo.BrandName != null ? _model.SummaryInfo.BrandName : string.Empty;
    SummaryViewModel.VendorNameViewModel = ViewModelConversions.VendorNameConvert(_model.SummaryInfo.VendorName);
    SummaryViewModel.FamilyIdViewModel = _model.SummaryInfo.FamilyId.HasValue ? _model.SummaryInfo.FamilyId.Value : 0;
    SummaryViewModel.ModelIdViewModel = _model.SummaryInfo.ModelId.HasValue ? _model.SummaryInfo.ModelId.Value : 0;
    SummaryViewModel.SteppingIdViewModel = _model.SummaryInfo.SteppingId.HasValue ? _model.SummaryInfo.SteppingId.Value : 0;

    SummaryViewModel.BaseSpeedViewModel = _model.SummaryInfo.BaseSpeed.HasValue
      ? HzUnitConverter.ConvertMHzToReadableUnit((double)_model.SummaryInfo.BaseSpeed)
      : string.Empty;

    SummaryViewModel.BusSpeedViewModel = _model.SummaryInfo.BusSpeed.HasValue
      ? HzUnitConverter.ConvertMHzToReadableUnit((double)_model.SummaryInfo.BusSpeed)
      : string.Empty;

    SummaryViewModel.SocketNumViewModel = _model.SummaryInfo.SocketNum.HasValue ? _model.SummaryInfo.SocketNum.Value : 0;
    SummaryViewModel.PhysicalCoreNumViewModel = _model.SummaryInfo.PhysicalCoreNum.HasValue ? _model.SummaryInfo.PhysicalCoreNum.Value : 0;
    SummaryViewModel.LogicalCoreNumViewModel = _model.SummaryInfo.LogicalCoreNum.HasValue ? _model.SummaryInfo.LogicalCoreNum.Value : 0;
    SummaryViewModel.VirtualizationViewModel = _model.SummaryInfo.Virtualization.HasValue ? _model.SummaryInfo.Virtualization.Value : false;

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

    //SummaryViewModel.InstructionSetViewModel = _model.SummaryInfo.InstructionSet;

    CpuInstructionInfo instructionInfoValues_ = _model.SummaryInfo.InstructionSet.HasValue
      ? _model.SummaryInfo.InstructionSet.Value : new CpuInstructionInfo();

    SummaryViewModel.CpuInstructionsViewModel = new Dictionary<string, bool> {
      { nameof(instructionInfoValues_._3DNOW), instructionInfoValues_._3DNOW },
      { nameof(instructionInfoValues_._3DNOWEXT), instructionInfoValues_._3DNOWEXT },

      { nameof(instructionInfoValues_.ABM), instructionInfoValues_.ABM },
      { nameof(instructionInfoValues_.ADX), instructionInfoValues_.ADX  },
      { nameof(instructionInfoValues_.AES), instructionInfoValues_.AES },
      { nameof(instructionInfoValues_.AVX), instructionInfoValues_.AVX },
      { nameof(instructionInfoValues_.AVX2), instructionInfoValues_.AVX2 },
      { nameof(instructionInfoValues_.AVX512CD), instructionInfoValues_.AVX512CD },
      { nameof(instructionInfoValues_.AVX512ER), instructionInfoValues_.AVX512ER  },
      { nameof(instructionInfoValues_.AVX512F), instructionInfoValues_.AVX512F },
      { nameof(instructionInfoValues_.AVX512PF), instructionInfoValues_.AVX512PF },

      { nameof(instructionInfoValues_.BMI1), instructionInfoValues_.BMI1 },
      { nameof(instructionInfoValues_.BMI2), instructionInfoValues_.BMI2 },

      { nameof(instructionInfoValues_.CLFSH), instructionInfoValues_.CLFSH },
      { nameof(instructionInfoValues_.CMPXCHG16B), instructionInfoValues_.CMPXCHG16B },
      { nameof(instructionInfoValues_.CX8), instructionInfoValues_.CX8 },

      { nameof(instructionInfoValues_.ERMS), instructionInfoValues_.ERMS },
      
      { nameof(instructionInfoValues_.F16C), instructionInfoValues_.F16C },
      { nameof(instructionInfoValues_.FMA), instructionInfoValues_.FMA },
      { nameof(instructionInfoValues_.FSGSBASE), instructionInfoValues_.FSGSBASE },
      { nameof(instructionInfoValues_.FXSR), instructionInfoValues_.FXSR },

      { nameof(instructionInfoValues_.HLE), instructionInfoValues_.HLE },

      { nameof(instructionInfoValues_.INVPCID), instructionInfoValues_.INVPCID },

      { nameof(instructionInfoValues_.LAHF), instructionInfoValues_.LAHF },
      { nameof(instructionInfoValues_.LZCNT), instructionInfoValues_.LZCNT },

      { nameof(instructionInfoValues_.MMX), instructionInfoValues_.MMX },
      { nameof(instructionInfoValues_.MMXEXT), instructionInfoValues_.MMXEXT },
      { nameof(instructionInfoValues_.MONITOR), instructionInfoValues_.MONITOR },
      { nameof(instructionInfoValues_.MOVBE), instructionInfoValues_.MOVBE },
      { nameof(instructionInfoValues_.MSR), instructionInfoValues_.MSR },

      { nameof(instructionInfoValues_.OSXSAVE), instructionInfoValues_.OSXSAVE },

      { nameof(instructionInfoValues_.PCLMULQDQ), instructionInfoValues_.PCLMULQDQ },
      { nameof(instructionInfoValues_.POPCNT), instructionInfoValues_.POPCNT },
      { nameof(instructionInfoValues_.PREFETCHWT1), instructionInfoValues_.PREFETCHWT1 },

      { nameof(instructionInfoValues_.RDRAND), instructionInfoValues_.RDRAND },
      { nameof(instructionInfoValues_.RDSEED), instructionInfoValues_.RDSEED },
      { nameof(instructionInfoValues_.RDTSCP), instructionInfoValues_.RDTSCP },
      { nameof(instructionInfoValues_.RTM), instructionInfoValues_.RTM },

      { nameof(instructionInfoValues_.SEP), instructionInfoValues_.SEP },
      { nameof(instructionInfoValues_.SHA), instructionInfoValues_.SHA },
      { nameof(instructionInfoValues_.SSE), instructionInfoValues_.SSE },
      { nameof(instructionInfoValues_.SSE2), instructionInfoValues_.SSE2 },

      { nameof(instructionInfoValues_.SSE3), instructionInfoValues_.SSE3 },
      { nameof(instructionInfoValues_.SSE41), instructionInfoValues_.SSE41 },
      { nameof(instructionInfoValues_.SSE42), instructionInfoValues_.SSE42 },
      { nameof(instructionInfoValues_.SSE4a), instructionInfoValues_.SSE4a },

      { nameof(instructionInfoValues_.SSSE3), instructionInfoValues_.SSSE3 },
      { nameof(instructionInfoValues_.SYSCALL), instructionInfoValues_.SYSCALL },
      { nameof(instructionInfoValues_.TBM), instructionInfoValues_.TBM },
      { nameof(instructionInfoValues_.XOP), instructionInfoValues_.XOP },
      { nameof(instructionInfoValues_.XSAVE), instructionInfoValues_.XSAVE },
    };
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

    //cpuViewModel_.VoltageViewModel = cpuModelValue_.Voltage.Value;

    cpuViewModel_.PlatformVoltageValueViewModel = cpuModelValue_.Voltage.Value ?? cpuViewModel_.PlatformVoltageValueViewModel;
    cpuViewModel_.PlatformVoltageMinViewModel = cpuModelValue_.Voltage.Min ?? cpuViewModel_.PlatformVoltageMinViewModel;
    cpuViewModel_.PlatformVoltageMaxViewModel = cpuModelValue_.Voltage.Max ?? cpuViewModel_.PlatformVoltageMaxViewModel;

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
