using Converters;
using CpuModule.Models;
using CpuModule.ViewModels.Definitions;
using CpuModule.ViewModels.Interfaces;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using LibreHardwareMonitor.Hardware.Motherboard;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using static Converters.ByteUnitConverters;

namespace CpuModule.ViewModels.Implementations;

public class CpuViewModel : BindableBase, ICpuViewModel {
  private CpuModel _model;
  public CpuViewModel(CpuModel model) {
    _model = model;
    foreach(var core_ in _model.LiveInfo.CpuCoreLiveInfo) {
      LiveViewModel.CoreLiveViewModel.Add(new CoreLiveViewModel {
        Name = core_.Name,
        Voltage = core_.Voltage,
        Speed = core_.Speed,
        Temperature = core_.Temperature,
        Load = core_.Load
      });
    }
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

    var cpuOverallModelValue_ = _model.LiveInfo.CpuOverallLiveInfo;
    if (cpuOverallModelValue_ == null)
      return;

    ICpuOverallLiveViewModel cpuOverallViewModel_ = LiveViewModel.CpuOverallLiveViewModel;

    if (cpuOverallViewModel_ == null)
      return;

    if (_model.LiveInfo.OsLiveInfo != null) {
      cpuOverallViewModel_.ProcessNum = _model.LiveInfo.OsLiveInfo.ProcessNum;
      cpuOverallViewModel_.ThreadNum = _model.LiveInfo.OsLiveInfo.ThreadsNum;
      cpuOverallViewModel_.HandleNum = _model.LiveInfo.OsLiveInfo.HandlesNum;
      cpuOverallViewModel_.UpTime = _model.LiveInfo.OsLiveInfo.UpTime;
    }

    cpuOverallViewModel_.LoadViewModel = _model.LiveInfo.CpuOverallLiveInfo.TotalLoad.Value;

    cpuOverallViewModel_.SpeedViewModel = cpuOverallModelValue_.CpuSpeed.Value.HasValue
      ? (float)Math.Round((double)cpuOverallModelValue_.CpuSpeed.Value / 1000, 2) : 0.0f;

    cpuOverallViewModel_.TemperatureViewModel = cpuOverallModelValue_.PackageTemperature.Value;

    //cpuViewModel_.VoltageViewModel = cpuModelValue_.Voltage.Value;

    cpuOverallViewModel_.PlatformVoltageValueViewModel = cpuOverallModelValue_.Voltage.Value ?? cpuOverallViewModel_.PlatformVoltageValueViewModel;
    cpuOverallViewModel_.PlatformVoltageMinViewModel = cpuOverallModelValue_.Voltage.Min ?? cpuOverallViewModel_.PlatformVoltageMinViewModel;
    cpuOverallViewModel_.PlatformVoltageMaxViewModel = cpuOverallModelValue_.Voltage.Max ?? cpuOverallViewModel_.PlatformVoltageMaxViewModel;

    cpuOverallViewModel_.PlatformPowerValueViewModel = cpuOverallModelValue_.PlatformPower.Value ?? cpuOverallViewModel_.PlatformPowerValueViewModel;
    cpuOverallViewModel_.PlatformPowerMinViewModel = cpuOverallModelValue_.PlatformPower.Min ?? cpuOverallViewModel_.PlatformPowerMinViewModel;
    cpuOverallViewModel_.PlatformPowerMaxViewModel = cpuOverallModelValue_.PlatformPower.Max ?? cpuOverallViewModel_.PlatformPowerMaxViewModel;

    cpuOverallViewModel_.PackagePowerValueViewModel = cpuOverallModelValue_.PackagePower.Value ?? cpuOverallViewModel_.PackagePowerValueViewModel;
    cpuOverallViewModel_.PackagePowerMinViewModel = cpuOverallModelValue_.PackagePower.Min ?? cpuOverallViewModel_.PackagePowerMinViewModel;
    cpuOverallViewModel_.PackagePowerMaxViewModel = cpuOverallModelValue_.PackagePower.Max ?? cpuOverallViewModel_.PackagePowerMaxViewModel;

    cpuOverallViewModel_.CoresPowerValueViewModel = cpuOverallModelValue_.CoresPower.Value ?? cpuOverallViewModel_.CoresPowerValueViewModel;
    cpuOverallViewModel_.CoresPowerMinViewModel = cpuOverallModelValue_.CoresPower.Min ?? cpuOverallViewModel_.CoresPowerMinViewModel;
    cpuOverallViewModel_.CoresPowerMaxViewModel = cpuOverallModelValue_.CoresPower.Max ?? cpuOverallViewModel_.CoresPowerMaxViewModel;

    cpuOverallViewModel_.MemoryPowerValueViewModel = cpuOverallModelValue_.MemoryPower.Value ?? cpuOverallViewModel_.MemoryPowerValueViewModel;
    cpuOverallViewModel_.MemoryPowerMinViewModel = cpuOverallModelValue_.MemoryPower.Min ?? cpuOverallViewModel_.MemoryPowerMinViewModel;
    cpuOverallViewModel_.MemoryPowerMaxViewModel = cpuOverallModelValue_.MemoryPower.Max ?? cpuOverallViewModel_.MemoryPowerMaxViewModel;

    cpuOverallViewModel_.PackageTemperatureValueViewModel = cpuOverallModelValue_.PackageTemperature.Value ?? cpuOverallViewModel_.PackageTemperatureValueViewModel;
    cpuOverallViewModel_.PackageTemperatureMinViewModel = cpuOverallModelValue_.PackageTemperature.Min ?? cpuOverallViewModel_.PackageTemperatureMinViewModel;
    cpuOverallViewModel_.PackageTemperatureMaxViewModel = cpuOverallModelValue_.PackageTemperature.Max ?? cpuOverallViewModel_.PackageTemperatureMaxViewModel;

    cpuOverallViewModel_.CoreAvgTemperatureValueViewModel = cpuOverallModelValue_.CoreAvgTemperature.Value ?? cpuOverallViewModel_.CoreAvgTemperatureValueViewModel;
    cpuOverallViewModel_.CoreAvgTemperatureMinViewModel = cpuOverallModelValue_.CoreAvgTemperature.Min ?? cpuOverallViewModel_.CoreAvgTemperatureMinViewModel;
    cpuOverallViewModel_.CoreAvgTemperatureMaxViewModel = cpuOverallModelValue_.CoreAvgTemperature.Max ?? cpuOverallViewModel_.CoreAvgTemperatureMaxViewModel;

    cpuOverallViewModel_.CoreMaxTemperatureValueViewModel = cpuOverallModelValue_.CoreMaxTemperature.Value ?? cpuOverallViewModel_.CoreMaxTemperatureValueViewModel;
    cpuOverallViewModel_.CoreMaxTemperatureMinViewModel = cpuOverallModelValue_.CoreMaxTemperature.Min ?? cpuOverallViewModel_.CoreMaxTemperatureMinViewModel;
    cpuOverallViewModel_.CoreMaxTemperatureMaxViewModel = cpuOverallModelValue_.CoreMaxTemperature.Max ?? cpuOverallViewModel_.CoreMaxTemperatureMaxViewModel;

    var coreModelInfo_ = _model.LiveInfo.CpuCoreLiveInfo;
    var coreViewModel_ = LiveViewModel.CoreLiveViewModel;
    

    if (coreModelInfo_ != null && coreViewModel_ != null) {
      if(coreViewModel_.Count != coreModelInfo_.Count) {
        foreach (ICpuCoreLiveInfo coreInfo in coreModelInfo_) {
          Application.Current.Dispatcher.Invoke(() => {
            coreViewModel_.Add(new CoreLiveViewModel {
              Name = coreInfo.Name,
              Voltage = coreInfo.Voltage,
              Temperature = coreInfo.Temperature,
              Load = coreInfo.Load,
              Speed = coreInfo.Speed
            });
          });
        }
      }
      else {
        for (int i = 0; i < coreModelInfo_.Count; i++) {
          var coreInfo = coreModelInfo_[i];
          var coreViewModel = coreViewModel_[i];
          if (coreViewModel.Name == coreInfo.Name) {
            coreViewModel.Voltage = coreInfo.Voltage;
            coreViewModel.Speed = coreInfo.Speed;
            coreViewModel.Temperature = coreInfo.Temperature;
            coreViewModel.Load = coreInfo.Load;
          }
        }
      }
      /*
      foreach (ICpuCoreLiveInfo coreInfo in coreModelInfo_) {
        coreViewModel_?.FirstOrDefault(c => c.Name == coreInfo.Name)?.Voltage = coreInfo.Voltage;
        coreViewModel_?.FirstOrDefault(c => c.Name == coreInfo.Name)?.Speed = coreInfo.Speed;
        coreViewModel_?.FirstOrDefault(c => c.Name == coreInfo.Name)?.Temperature = coreInfo.Temperature;
        coreViewModel_?.FirstOrDefault(c => c.Name == coreInfo.Name)?.Load = coreInfo.Load;

        //coreViewModel_.Add(new CoreLiveViewModel {
        //  Name = coreInfo.Name,
        //  Voltage = coreInfo.Voltage,
        //  Speed = coreInfo.Speed,
        //  Temperature = coreInfo.Temperature,
        //  Load = coreInfo.Load
        //});
      }
      RaisePropertyChanged(nameof(LiveViewModel.CoreLiveViewModel));
      */
    }
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
