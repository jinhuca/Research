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

    //var runningMax = liveSource_.Select(static reading => reading.CpuOverallLiveInfo.PlatformPower.max.Value)
    //  .Scan((currentMax, nextValue) => Math.Max(currentMax, nextValue));
    //runningMax.Subscribe(max => Debug.WriteLine($"Highest value so far: {max}"));

    IDisposable cpuSummaryDisposable_ = summarySource_.Subscribe(
      newItem => { UpdateSummaryInfo(newItem); },
      ex => { Debug.WriteLine(ex.Message); },
      () => { Debug.WriteLine("query cpu summary completed."); });

    IDisposable cpuLiveDisposable_ = liveSource_.Subscribe(
      newItem => { UpdateLiveInfo(newItem); },
      ex => { },
      () => { });
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

  private void UpdateLiveInfo(ICpuLiveInfo newItem) {
    LiveInfo.CpuOverallLiveInfo.TotalLoad = newItem.CpuOverallLiveInfo.TotalLoad;

    LiveInfo.CpuOverallLiveInfo.CpuSpeed = newItem.CpuOverallLiveInfo.CpuSpeed;

    LiveInfo.CpuOverallLiveInfo.Voltage = newItem.CpuOverallLiveInfo.Voltage;

    // == Update Platform Power Record
    // (1) grab the updated record
    var updatedPlatformPower_ = newItem.CpuOverallLiveInfo.PlatformPower;
    // (2) update min and max 
    _minPlatformPower = updatedPlatformPower_.Min.HasValue
      ? MathF.Min(updatedPlatformPower_.Min.Value, _minPlatformPower)
      : _minPlatformPower;
    _maxPlatformPower = updatedPlatformPower_.Max.HasValue
      ? MathF.Max(updatedPlatformPower_.Max.Value, _maxPlatformPower)
      : _maxPlatformPower;
    // (3) update record
    var platformPowerValue_ = updatedPlatformPower_.Value.HasValue
      ? updatedPlatformPower_.Value
      : LiveInfo.CpuOverallLiveInfo.PlatformPower.Value;
    var platformPowerMin_ = updatedPlatformPower_.Min.HasValue
      ? MathF.Min(updatedPlatformPower_.Min.Value, _minPlatformPower)
      : _minPlatformPower;
    var platformPowerMax_ = updatedPlatformPower_.Max.HasValue
      ? MathF.Max(updatedPlatformPower_.Max.Value, _maxPlatformPower)
      : _maxPlatformPower;
    LiveInfo.CpuOverallLiveInfo.PlatformPower = new SensorDataType { Value = platformPowerValue_, Min = platformPowerMin_, Max = platformPowerMax_ };

    // == Update Package Power Record
    // (1) grab the updated record
    var updatedPackagePower_ = newItem.CpuOverallLiveInfo.PackagePower;
    // (2) update min and max
    _minPackagePower = updatedPackagePower_.Min.HasValue
      ? MathF.Min(updatedPackagePower_.Min.Value, _minPackagePower)
      : _minPackagePower;
    _maxPackagePower = updatedPackagePower_.Max.HasValue
      ? MathF.Max(updatedPackagePower_.Max.Value, _maxPackagePower)
      : _maxPackagePower;
    // (3) update record
    var packagePowerValue_ = updatedPackagePower_.Value.HasValue
      ? updatedPackagePower_.Value
      : LiveInfo.CpuOverallLiveInfo.PackagePower.Value;
    var packagePowerMin_ = updatedPackagePower_.Min.HasValue
      ? MathF.Min(updatedPackagePower_.Min.Value, _minPackagePower)
      : _minPackagePower;
    var packagePowerMax_ = updatedPackagePower_.Max.HasValue
      ? MathF.Max(updatedPackagePower_.Max.Value, _maxPackagePower)
      : _maxPackagePower;
    LiveInfo.CpuOverallLiveInfo.PackagePower = new SensorDataType { Value = packagePowerValue_, Min = packagePowerMin_, Max = packagePowerMax_ };

    // == Update Cores Power Record
    // (1) grab the updated record
    var updatedCoresPower_ = newItem.CpuOverallLiveInfo.CoresPower;
    // (2) update min and max
    _minCoresPower = updatedCoresPower_.Min.HasValue
      ? MathF.Min(updatedCoresPower_.Min.Value, _minCoresPower)
      : _minCoresPower;
    _maxCoresPower = updatedCoresPower_.Max.HasValue
      ? MathF.Max(updatedCoresPower_.Max.Value, _maxCoresPower)
      : _maxCoresPower;
    // (3) update record
    var coresPowerValue_ = updatedCoresPower_.Value.HasValue
      ? updatedCoresPower_.Value
      : LiveInfo.CpuOverallLiveInfo.CoresPower.Value;
    var coresPowerMin_ = updatedCoresPower_.Min.HasValue
      ? MathF.Min(updatedCoresPower_.Min.Value, _minCoresPower)
      : _minCoresPower;
    var coresPowerMax_ = updatedCoresPower_.Max.HasValue
      ? MathF.Max(updatedCoresPower_.Max.Value, _maxCoresPower)
      : _maxCoresPower;
    LiveInfo.CpuOverallLiveInfo.CoresPower = new SensorDataType { Value = coresPowerValue_, Min = coresPowerMin_, Max = coresPowerMax_ };

    // == Update Memory Power Record
    // (1) grab the updated record
    var updatedMemoryPower_ = newItem.CpuOverallLiveInfo.MemoryPower;
    // (2) update min and max
    _minMemoryPower = updatedMemoryPower_.Min.HasValue
      ? MathF.Min(updatedMemoryPower_.Min.Value, _minMemoryPower)
      : _minMemoryPower;
    _maxMemoryPower = updatedMemoryPower_.Max.HasValue
      ? MathF.Max(updatedMemoryPower_.Max.Value, _maxMemoryPower)
      : _maxMemoryPower;
    // (3) update record
    var memoryPowerValue_ = updatedMemoryPower_.Value.HasValue
      ? updatedMemoryPower_.Value
      : LiveInfo.CpuOverallLiveInfo.MemoryPower.Value;
    var memoryPowerMin_ = updatedMemoryPower_.Min.HasValue
      ? MathF.Min(updatedMemoryPower_.Min.Value, _minMemoryPower)
      : _minMemoryPower;
    var memoryPowerMax_ = updatedMemoryPower_.Max.HasValue
      ? MathF.Max(updatedMemoryPower_.Max.Value, _maxMemoryPower)
      : _maxMemoryPower;
    LiveInfo.CpuOverallLiveInfo.MemoryPower = new SensorDataType { Value = memoryPowerValue_, Min = memoryPowerMin_, Max = memoryPowerMax_ };

    // == update package temperature record
    // (1) grab 
    var updatedPackageTemperature_ = newItem.CpuOverallLiveInfo.PackageTemperature;
    // (2) update min and max
    _minPackageTemperature = updatedPackageTemperature_.Min.HasValue
      ? MathF.Min(updatedPackageTemperature_.Min.Value, _minPackageTemperature) : _minPackageTemperature;
    _maxPackageTemperature = updatedPackageTemperature_.Max.HasValue
      ? MathF.Max(updatedPackageTemperature_.Max.Value, _maxPackageTemperature) : _maxPackageTemperature;
    // (3) update record
    var packageTemperatureValue_ = updatedPackageTemperature_.Value.HasValue
      ? updatedPackageTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.PackageTemperature.Value;
    var packageTemperatureMin_ = updatedPackageTemperature_.Min.HasValue
      ? MathF.Min(updatedPackageTemperature_.Min.Value, _minPackageTemperature)
      : _minPackageTemperature;
    var packageTemperatureMax_ = updatedPackageTemperature_.Max.HasValue
      ? MathF.Max(updatedPackageTemperature_.Max.Value, _maxPackageTemperature)
      : _maxPackageTemperature;
    LiveInfo.CpuOverallLiveInfo.PackageTemperature = new SensorDataType { Value = packageTemperatureValue_, Min = packageTemperatureMin_, Max = packageTemperatureMax_ };

    // == update Core Avg Temperature record
    // (1) grab
    var updatedCoreAvgTemperature_ = newItem.CpuOverallLiveInfo.CoreAvgTemperature;
    // (2) update min and max
    _minCoreAvgTemperature = updatedCoreAvgTemperature_.Min.HasValue
      ? MathF.Min(updatedCoreAvgTemperature_.Min.Value, _minCoreAvgTemperature) : _minCoreAvgTemperature;
    _maxCoreAvgTemperature = updatedCoreAvgTemperature_.Max.HasValue
      ? MathF.Max(updatedCoreAvgTemperature_.Max.Value, _maxCoreAvgTemperature) : _maxCoreAvgTemperature;
    // (3) update record
    var coreAvgTemperatureValue_ = updatedCoreAvgTemperature_.Value.HasValue
      ? updatedCoreAvgTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature.Value;
    var coreAvgTemperatureMin_ = updatedCoreAvgTemperature_.Min.HasValue
      ? MathF.Min(updatedCoreAvgTemperature_.Min.Value, _minCoreAvgTemperature)
      : _minCoreAvgTemperature;
    var coreAverageTemperatureMax_ = updatedCoreAvgTemperature_.Max.HasValue
      ? MathF.Max(updatedCoreAvgTemperature_.Max.Value, _maxCoreAvgTemperature)
      : _maxCoreAvgTemperature;
    LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature = new SensorDataType { Value = coreAvgTemperatureValue_, Min = coreAvgTemperatureMin_, Max = coreAverageTemperatureMax_ };

    // == update package temperature record
    // (1) grab 
    var updatedCoreMaxTemperature_ = newItem.CpuOverallLiveInfo.CoreMaxTemperature;
    // (2) update min and max
    _minCoreMaxTemperature = updatedCoreMaxTemperature_.Min.HasValue
      ? MathF.Min(updatedCoreMaxTemperature_.Min.Value, _minCoreMaxTemperature) : _minCoreMaxTemperature;
    _maxCoreMaxTemperature = updatedCoreMaxTemperature_.Max.HasValue
      ? MathF.Max(updatedCoreMaxTemperature_.Max.Value, _maxCoreMaxTemperature) : _maxCoreMaxTemperature;
    // (3) update record
    var coreMaxTemperatureValue_ = updatedCoreMaxTemperature_.Value.HasValue
      ? updatedCoreMaxTemperature_.Value
      : LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature.Value;
    var coreMaxTemperatureMin_ = updatedCoreMaxTemperature_.Min.HasValue
      ? MathF.Min(updatedCoreMaxTemperature_.Min.Value, _minCoreMaxTemperature) : _minCoreMaxTemperature;
    var coreMaxTemperatureMax_ = updatedCoreMaxTemperature_.Max.HasValue
      ? MathF.Max(updatedCoreMaxTemperature_.Max.Value, _maxCoreMaxTemperature) : _maxCoreMaxTemperature;
    LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature = new SensorDataType { Value = coreMaxTemperatureValue_, Min = coreAvgTemperatureMin_, Max = coreMaxTemperatureMax_ };

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

  private static float _minPlatformPower = 0.0f;
  private static float _maxPlatformPower = 0.0f;

  private static float _minPackagePower = 0.0f;
  private static float _maxPackagePower = 0.0f;

  private static float _minCoresPower = 0.0f;
  private static float _maxCoresPower = 0.0f;

  private static float _minMemoryPower = 0.0f;
  private static float _maxMemoryPower = 0.0f;

  private static float _minPackageTemperature = 0.0f;
  private static float _maxPackageTemperature = 0.0f;

  private static float _minCoreAvgTemperature = 0.0f;
  private static float _maxCoreAvgTemperature = 0.0f;

  private static float _minCoreMaxTemperature = 0.0f;
  private static float _maxCoreMaxTemperature = 0.0f;
}
