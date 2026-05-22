using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;
using System.Diagnostics;
using System.Reactive.Linq;
using static CpuInfoServices.Observables.CpuInfoGenerators;

namespace CpuModule.Models;

public class CpuModel : BindableBase, ICpuModel {
  IObservable<ICpuSummaryInfo> summarySource_;
  IObservable<ICpuLiveInfo> liveSource_;

  public CpuModel() {
    summarySource_ = GenerateCpuSummaryInfo(TimeSpan.FromSeconds(0));
    liveSource_ = GenerateCpuLiveInfo(TimeSpan.FromSeconds(1));

    IDisposable cpuSummaryDisposable_ = summarySource_.Subscribe(
      newItem => { UpdateSummaryInfo(newItem); },
      ex => { Debug.WriteLine(ex.Message); },
      () => { Debug.WriteLine("query cpu summary completed."); });

    IDisposable cpuLiveDisposable_ = liveSource_.Subscribe(
      newItem => { UpdateLiveInfo(newItem); },
      ex => { Debug.WriteLine(ex.Message); },
      () => { Debug.WriteLine("Sensor update completed."); });
  }

  private void UpdateSummaryInfo(ICpuSummaryInfo info) {
    SummaryInfo.BrandName = info.BrandName ?? string.Empty;
    SummaryInfo.VendorName = info.VendorName ?? string.Empty;
    SummaryInfo.FamilyId = info.FamilyId ?? 0;
    SummaryInfo.ModelId = info.ModelId ?? 0;
    SummaryInfo.SteppingId = info.SteppingId ?? 0;
    SummaryInfo.BaseSpeed = info.BaseSpeed ?? float.NaN;
    SummaryInfo.BusSpeed = info.BusSpeed ?? float.NaN;
    SummaryInfo.SocketNum = info.SocketNum ?? 0;
    SummaryInfo.PhysicalCoreNum = info.PhysicalCoreNum ?? 0;
    SummaryInfo.LogicalCoreNum = info.LogicalCoreNum ?? 0;
    SummaryInfo.Virtualization = info.Virtualization ?? false;
    SummaryInfo.CacheInfo = info.CacheInfo;
    SummaryInfo.InstructionSet = info.InstructionSet;
    RaisePropertyChanged(nameof(SummaryInfo));
  }

  private void UpdateLiveInfo(ICpuLiveInfo newInfo) {
    ICpuOverallLiveInfo currentCpuOverallInfo = LiveInfo.CpuOverallLiveInfo;
    ICpuOverallLiveInfo newCpuOverallInfo = newInfo.CpuOverallLiveInfo;

    currentCpuOverallInfo.TotalLoad = newCpuOverallInfo.TotalLoad;
    currentCpuOverallInfo.CpuSpeed = newCpuOverallInfo.CpuSpeed;
    currentCpuOverallInfo.Voltage = newCpuOverallInfo.Voltage;

    // == Update Platform Power Record
    // (1) grab the updated record
    var updatedPlatformPower_ = newInfo.CpuOverallLiveInfo.PlatformPower;

    // (2) update min and max 
    if (updatedPlatformPower_.Min.HasValue) {
      _minPlatformPower = _minPlatformPower.HasValue
        ? MathF.Min(updatedPlatformPower_.Min.Value, _minPlatformPower.Value)
        : updatedPlatformPower_.Min.Value;
    }
    else {
      _minPlatformPower = updatedPlatformPower_.Value.HasValue
        ? updatedPlatformPower_.Value.Value
        : null;
    }

    if (updatedPlatformPower_.Max.HasValue) {
      _maxPlatformPower = _maxPlatformPower.HasValue
        ? MathF.Max(updatedPlatformPower_.Max.Value, _maxPlatformPower.Value)
        : updatedPlatformPower_.Max.Value;
    }
    else {
      _maxPlatformPower = updatedPlatformPower_.Value.HasValue
        ? updatedPlatformPower_.Value.Value
        : null;
    }

    // (3) update record
    float? platformPowerValue_ = updatedPlatformPower_.Value.HasValue
      ? updatedPlatformPower_.Value
      : LiveInfo.CpuOverallLiveInfo.PlatformPower.Value;
    float? platformPowerMin_ = updatedPlatformPower_.Min.HasValue && _minPlatformPower.HasValue
      ? MathF.Min(updatedPlatformPower_.Min.Value, _minPlatformPower.Value)
      : null;
    float? platformPowerMax_ = updatedPlatformPower_.Max.HasValue && _maxPlatformPower.HasValue
      ? MathF.Max(updatedPlatformPower_.Max.Value, _maxPlatformPower.Value)
      : null;

    LiveInfo.CpuOverallLiveInfo.PlatformPower = new SensorDataType {
      Value = platformPowerValue_,
      Min = platformPowerMin_,
      Max = platformPowerMax_
    };

    // == Update Package Power Record
    // (1) grab the updated record
    var updatedPackagePower_ = newInfo.CpuOverallLiveInfo.PackagePower;

    // (2) update min and max
    if (updatedPackagePower_.Min.HasValue) {
      _minPackagePower = _minPackagePower.HasValue
        ? MathF.Min(updatedPackagePower_.Min.Value, _minPackagePower.Value)
        : updatedPackagePower_.Min.Value;
    }
    else {
      _minPackagePower = updatedPackagePower_.Value.HasValue
        ? updatedPackagePower_.Value.Value
        : null;
    }

    if (updatedPackagePower_.Max.HasValue) {
      _maxPackagePower = _maxPackagePower.HasValue
        ? MathF.Max(updatedPackagePower_.Max.Value, _maxPackagePower.Value)
        : updatedPackagePower_.Max.Value;
    }
    else {
      _maxPackagePower = updatedPackagePower_.Value.HasValue
        ? updatedPackagePower_.Value.Value
        : null;
    }

    // (3) update record
    float? packagePowerValue_ = updatedPackagePower_.Value.HasValue
      ? updatedPackagePower_.Value
      : LiveInfo.CpuOverallLiveInfo.PackagePower.Value;
    float? packagePowerMin_ = updatedPackagePower_.Min.HasValue && _minPackagePower.HasValue
      ? MathF.Min(updatedPackagePower_.Min.Value, _minPackagePower.Value)
      : null;
    float? packagePowerMax_ = updatedPackagePower_.Max.HasValue && _maxPackagePower.HasValue
      ? MathF.Max(updatedPackagePower_.Max.Value, _maxPackagePower.Value)
      : null;
    LiveInfo.CpuOverallLiveInfo.PackagePower = new SensorDataType {
      Value = packagePowerValue_,
      Min = packagePowerMin_,
      Max = packagePowerMax_
    };

    // == Update Cores Power Record
    // (1) grab the updated record
    var updatedCoresPower_ = newInfo.CpuOverallLiveInfo.CoresPower;

    // (2) update min and max
    if (updatedCoresPower_.Min.HasValue) {
      _minCoresPower = _minCoresPower.HasValue
        ? MathF.Min(updatedCoresPower_.Min.Value, _minCoresPower.Value)
        : updatedCoresPower_.Min.Value;
    }
    else {
      _minCoresPower = updatedCoresPower_.Value.HasValue
        ? updatedCoresPower_.Value.Value : null;
    }

    if (updatedCoresPower_.Max.HasValue) {
      _maxCoresPower = _maxCoresPower.HasValue
        ? MathF.Max(updatedCoresPower_.Max.Value, _maxCoresPower.Value)
        : updatedCoresPower_.Max.Value;
    }
    else {
      _maxCoresPower = updatedCoresPower_.Value.HasValue
        ? updatedCoresPower_.Value.Value : null;
    }

    // (3) update record
    float? coresPowerValue_ = updatedCoresPower_.Value.HasValue
      ? updatedCoresPower_.Value
      : LiveInfo.CpuOverallLiveInfo.CoresPower.Value;
    float? coresPowerMin_ = updatedCoresPower_.Min.HasValue && _minCoresPower.HasValue
      ? MathF.Min(updatedCoresPower_.Min.Value, _minCoresPower.Value)
      : null;
    float? coresPowerMax_ = updatedCoresPower_.Max.HasValue && _maxCoresPower.HasValue
      ? MathF.Max(updatedCoresPower_.Max.Value, _maxCoresPower.Value)
      : null;

    LiveInfo.CpuOverallLiveInfo.CoresPower = new SensorDataType {
      Value = coresPowerValue_,
      Min = coresPowerMin_,
      Max = coresPowerMax_
    };

    // == Update Memory Power Record
    // (1) grab the updated record
    var updatedMemoryPower_ = newInfo.CpuOverallLiveInfo.MemoryPower;

    // (2) update min and max
    if (updatedMemoryPower_.Min.HasValue) {
      _minMemoryPower = _minMemoryPower.HasValue
        ? MathF.Min(updatedMemoryPower_.Min.Value, _minMemoryPower.Value)
        : updatedMemoryPower_.Min.Value;
    }
    else {
      _minMemoryPower = updatedMemoryPower_.Value.HasValue
        ? updatedMemoryPower_.Value.Value : null;
    }

    if (updatedMemoryPower_.Max.HasValue) {
      _maxMemoryPower = _maxMemoryPower.HasValue
        ? MathF.Max(updatedMemoryPower_.Max.Value, _maxMemoryPower.Value)
        : updatedMemoryPower_.Max.Value;
    }
    else {
      _maxMemoryPower = updatedMemoryPower_.Value.HasValue
        ? updatedMemoryPower_.Value.Value : null;
    }

    // (3) update record
    float? memoryPowerValue_ = updatedMemoryPower_.Value.HasValue
      ? updatedMemoryPower_.Value
      : LiveInfo.CpuOverallLiveInfo.MemoryPower.Value;
    float? memoryPowerMin_ = updatedMemoryPower_.Min.HasValue && _minMemoryPower.HasValue
      ? MathF.Min(updatedMemoryPower_.Min.Value, _minMemoryPower.Value)
      : null;
    float? memoryPowerMax_ = updatedMemoryPower_.Max.HasValue && _maxMemoryPower.HasValue
      ? MathF.Max(updatedMemoryPower_.Max.Value, _maxMemoryPower.Value)
      : null;
    LiveInfo.CpuOverallLiveInfo.MemoryPower = new SensorDataType {
      Value = memoryPowerValue_,
      Min = memoryPowerMin_,
      Max = memoryPowerMax_
    };

    // == update package temperature record
    // (1) grab the updated record
    var updatedPackageTemperature_ = newInfo.CpuOverallLiveInfo.PackageTemperature;
    // (2) update min and max
    if (updatedPackageTemperature_.Min.HasValue) {
      _minPackageTemperature = _minPackageTemperature.HasValue
        ? MathF.Min(updatedPackageTemperature_.Min.Value, _minPackageTemperature.Value)
        : updatedPackageTemperature_.Min.Value;
    }
    else {
      _minPackageTemperature = updatedPackageTemperature_.Value.HasValue
        ? updatedPackageTemperature_.Value.Value : null;
    }

    if (updatedPackageTemperature_.Max.HasValue) {
      _maxPackageTemperature = _maxPackageTemperature.HasValue
        ? MathF.Max(updatedPackageTemperature_.Max.Value, _maxPackageTemperature.Value)
        : updatedPackageTemperature_.Max.Value;
    }
    else {
      _maxPackageTemperature = updatedPackageTemperature_.Value.HasValue
        ? updatedPackageTemperature_.Value.Value : null;
    }

    // (3) update record
    float? packageTemperatureValue_ = updatedPackageTemperature_.Value.HasValue
      ? updatedPackageTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.PackageTemperature.Value;
    float? packageTemperatureMin_ = updatedPackageTemperature_.Min.HasValue && _minPackageTemperature.HasValue
      ? MathF.Min(updatedPackageTemperature_.Min.Value, _minPackageTemperature.Value)
      : null;
    float? packageTemperatureMax_ = updatedPackageTemperature_.Max.HasValue && _maxPackageTemperature.HasValue
      ? MathF.Max(updatedPackageTemperature_.Max.Value, _maxPackageTemperature.Value)
      : null;
    LiveInfo.CpuOverallLiveInfo.PackageTemperature = new SensorDataType {
      Value = packageTemperatureValue_,
      Min = packageTemperatureMin_,
      Max = packageTemperatureMax_
    };

    // == update Core Avg Temperature record
    // (1) grab
    var updatedCoreAvgTemperature_ = newInfo.CpuOverallLiveInfo.CoreAvgTemperature;
    // (2) update min and max
    if (updatedCoreAvgTemperature_.Min.HasValue) {
      _minCoreAvgTemperature = _minCoreAvgTemperature.HasValue
        ? MathF.Min(updatedCoreAvgTemperature_.Min.Value, _minCoreAvgTemperature.Value)
        : updatedCoreAvgTemperature_.Min.Value;
    }
    else {
      _minCoreAvgTemperature = updatedCoreAvgTemperature_.Value.HasValue
        ? updatedCoreAvgTemperature_.Value.Value : null;
    }

    if (updatedCoreAvgTemperature_.Max.HasValue) {
      _maxCoreAvgTemperature = _maxCoreAvgTemperature.HasValue
        ? MathF.Max(updatedCoreAvgTemperature_.Max.Value, _maxCoreAvgTemperature.Value)
        : updatedCoreAvgTemperature_.Max.Value;
    }
    else {
      _maxCoreAvgTemperature = updatedCoreAvgTemperature_.Value.HasValue
        ? updatedCoreAvgTemperature_.Value.Value : null;
    }

    // (3) update record
    float? coreAvgTemperatureValue_ = updatedCoreAvgTemperature_.Value.HasValue
      ? updatedCoreAvgTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature.Value;
    float? coreAvgTemperatureMin_ = updatedCoreAvgTemperature_.Min.HasValue && _minCoreAvgTemperature.HasValue
      ? MathF.Min(updatedCoreAvgTemperature_.Min.Value, _minCoreAvgTemperature.Value)
      : null;
    float? coreAverageTemperatureMax_ = updatedCoreAvgTemperature_.Max.HasValue && _maxCoreAvgTemperature.HasValue
      ? MathF.Max(updatedCoreAvgTemperature_.Max.Value, _maxCoreAvgTemperature.Value)
      : null;
    LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature = new SensorDataType {
      Value = coreAvgTemperatureValue_,
      Min = coreAvgTemperatureMin_,
      Max = coreAverageTemperatureMax_
    };

    // == update Core Max Temperature record
    // (1) grab 
    var updatedCoreMaxTemperature_ = newInfo.CpuOverallLiveInfo.CoreMaxTemperature;
    // (2) update min and max
    if (updatedCoreMaxTemperature_.Min.HasValue) {
      _minCoreMaxTemperature = _minCoreMaxTemperature.HasValue
        ? MathF.Min(updatedCoreMaxTemperature_.Min.Value, _minCoreMaxTemperature.Value)
        : updatedCoreMaxTemperature_.Min.Value;
    }
    else {
      _minCoreMaxTemperature = updatedCoreMaxTemperature_.Value.HasValue
        ? updatedCoreMaxTemperature_.Value.Value : null;
    }

    if (updatedCoreMaxTemperature_.Max.HasValue) {
      _maxCoreMaxTemperature = _maxCoreMaxTemperature.HasValue
        ? MathF.Max(updatedCoreMaxTemperature_.Max.Value, _maxCoreMaxTemperature.Value)
        : updatedCoreMaxTemperature_.Max.Value;
    }
    else {
      _maxCoreAvgTemperature = updatedCoreMaxTemperature_.Value.HasValue
        ? updatedCoreMaxTemperature_.Value.Value : null;
    }

    // (3) update record
    float? coreMaxTemperatureValue_ = updatedCoreMaxTemperature_.Value.HasValue
      ? updatedCoreMaxTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature.Value;
    float? coreMaxTemperatureMin_ = updatedCoreMaxTemperature_.Min.HasValue && _minCoreMaxTemperature.HasValue
      ? MathF.Min(updatedCoreMaxTemperature_.Min.Value, _minCoreMaxTemperature.Value)
      : null;
    float? coreMaxTemperatureMax_ = updatedCoreMaxTemperature_.Max.HasValue && _maxCoreMaxTemperature.HasValue
      ? MathF.Max(updatedCoreMaxTemperature_.Max.Value, _maxCoreMaxTemperature.Value)
      : null;
    LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature = new SensorDataType {
      Value = coreMaxTemperatureValue_,
      Min = coreAvgTemperatureMin_,
      Max = coreMaxTemperatureMax_
    };

    // update core live info
    // ...
    var coreInfoList_ = newInfo.CpuCoreLiveInfo;
    LiveInfo.CpuCoreLiveInfo.Clear();

    Debug.WriteLine("Core info count: " + coreInfoList_.Count);
    foreach (var newCoreInfo_ in coreInfoList_) {
      ICpuCoreLiveInfo updatedCoreInfo_ = new CpuCoreLiveInfo() {
        Name = newCoreInfo_.Name,
        Load = newCoreInfo_.Load,
        Voltage = newCoreInfo_.Voltage,
        Speed = newCoreInfo_.Speed,
        Temperature = newCoreInfo_.Temperature
      };
      LiveInfo.CpuCoreLiveInfo.Add(updatedCoreInfo_);
    }

    RaisePropertyChanged(nameof(LiveInfo));
  }

  private ICpuSummaryInfo _cpuSummaryInfo = new CpuSummaryInfo();
  public ICpuSummaryInfo SummaryInfo {
    get => _cpuSummaryInfo;
    set => SetProperty(ref _cpuSummaryInfo, value);
  }

  private ICpuLiveInfo _cpuLiveInfo = new CpuLiveInfo();
  public ICpuLiveInfo LiveInfo {
    get => _cpuLiveInfo;
    set => SetProperty(ref _cpuLiveInfo, value);
  }

  private static float? _minPlatformPower;
  private static float? _maxPlatformPower;

  private static float? _minPackagePower;
  private static float? _maxPackagePower;

  private static float? _minCoresPower;
  private static float? _maxCoresPower;

  private static float? _minMemoryPower;
  private static float? _maxMemoryPower;

  private static float? _minPackageTemperature;
  private static float? _maxPackageTemperature;

  private static float? _minCoreAvgTemperature;
  private static float? _maxCoreAvgTemperature;

  private static float? _minCoreMaxTemperature;
  private static float? _maxCoreMaxTemperature;
}
