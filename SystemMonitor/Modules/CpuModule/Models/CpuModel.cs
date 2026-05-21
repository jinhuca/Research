using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;
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
    var platformPowerValue_ = updatedPlatformPower_.val.HasValue
      ? updatedPlatformPower_.val
      : LiveInfo.CpuOverallLiveInfo.PlatformPower.val;
    var platformPowerMin_ = updatedPlatformPower_.Min.HasValue
      ? MathF.Min(updatedPlatformPower_.Min.Value, _minPlatformPower)
      : _minPlatformPower;
    var platformPowerMax_ = updatedPlatformPower_.Max.HasValue
      ? MathF.Max(updatedPlatformPower_.Max.Value, _maxPlatformPower)
      : _maxPlatformPower;
    LiveInfo.CpuOverallLiveInfo.PlatformPower = (platformPowerValue_, platformPowerMin_, platformPowerMax_);

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
    var packagePowerValue_ = updatedPackagePower_.val.HasValue
      ? updatedPackagePower_.val
      : LiveInfo.CpuOverallLiveInfo.PackagePower.val;
    var packagePowerMin_ = updatedPackagePower_.Min.HasValue
      ? MathF.Min(updatedPackagePower_.Min.Value, _minPackagePower)
      : _minPackagePower;
    var packagePowerMax_ = updatedPackagePower_.Max.HasValue
      ? MathF.Max(updatedPackagePower_.Max.Value, _maxPackagePower)
      : _maxPackagePower;
    LiveInfo.CpuOverallLiveInfo.PackagePower = (packagePowerValue_, packagePowerMin_, packagePowerMax_);

    /*
    _maxCoresPower = newItem.CpuOverallLiveInfo.CoresPower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoresPower.max.Value, _maxCoresPower)
      : _maxCoresPower;
    var coresPowerValue_ = newItem.CpuOverallLiveInfo.CoresPower.val.HasValue
      ? newItem.CpuOverallLiveInfo.CoresPower.val
      : LiveInfo.CpuOverallLiveInfo.CoresPower.val;
    var coresPowerMax_ = newItem.CpuOverallLiveInfo.CoresPower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoresPower.max.Value, _maxCoresPower)
      : _maxCoresPower;
    LiveInfo.CpuOverallLiveInfo.CoresPower = (coresPowerValue_, coresPowerMax_);

    _maxMemoryPower = newItem.CpuOverallLiveInfo.MemoryPower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.MemoryPower.max.Value, _maxMemoryPower)
      : _maxMemoryPower;
    var memoryPowerValue_ = newItem.CpuOverallLiveInfo.MemoryPower.val.HasValue
      ? newItem.CpuOverallLiveInfo.MemoryPower.val
      : LiveInfo.CpuOverallLiveInfo.MemoryPower.val;
    var memoryPowerMax_ = newItem.CpuOverallLiveInfo.MemoryPower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.MemoryPower.max.Value, _maxMemoryPower)
      : _maxMemoryPower;
    LiveInfo.CpuOverallLiveInfo.MemoryPower = (memoryPowerValue_, memoryPowerMax_);

    _maxPackageTemperature = newItem.CpuOverallLiveInfo.PackageTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.PackageTemperature.max.Value, _maxPackageTemperature)
      : _maxPackageTemperature;
    var packageTemperatureValue_ = newItem.CpuOverallLiveInfo.PackageTemperature.val.HasValue
      ? newItem.CpuOverallLiveInfo.PackageTemperature.val
      : LiveInfo.CpuOverallLiveInfo.PackageTemperature.val;
    var packageTemperatureMax_ = newItem.CpuOverallLiveInfo.PackageTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.PackageTemperature.max.Value, _maxPackageTemperature)
      : _maxPackageTemperature;
    LiveInfo.CpuOverallLiveInfo.PackageTemperature = (packageTemperatureValue_, packageTemperatureMax_);

    _maxCoreAvgTemperature = newItem.CpuOverallLiveInfo.CoreAvgTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoreAvgTemperature.max.Value, _maxCoreAvgTemperature)
      : _maxCoreAvgTemperature;
    var coreAvgTemperatureValue_ = newItem.CpuOverallLiveInfo.CoreAvgTemperature.val.HasValue
      ? newItem.CpuOverallLiveInfo.CoreAvgTemperature.val
      : LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature.val;
    var coreAverageTemperatureMax_ = newItem.CpuOverallLiveInfo.CoreAvgTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoreAvgTemperature.max.Value, _maxCoreAvgTemperature)
      : _maxCoreAvgTemperature;
    LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature = (coreAvgTemperatureValue_, coreAverageTemperatureMax_);

    _maxCoreMaxTemperature = newItem.CpuOverallLiveInfo.CoreMaxTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoreMaxTemperature.max.Value, _maxCoreMaxTemperature) 
      : _maxCoreMaxTemperature;
    var coreMaxTemperatureValue_ = newItem.CpuOverallLiveInfo.CoreMaxTemperature.val.HasValue
      ? newItem.CpuOverallLiveInfo.CoreMaxTemperature.val
      : LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature.val;
    var coreMaxTemperatureMax_ = newItem.CpuOverallLiveInfo.CoreMaxTemperature.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.CoreMaxTemperature.max.Value, _maxCoreMaxTemperature)
      : _maxCoreMaxTemperature;
    LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature = (coreMaxTemperatureValue_, coreMaxTemperatureMax_);
    */
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


  private static float _maxCoresPower = 0.0f;
  private static float _maxMemoryPower = 0.0f;

  private static float _maxPackageTemperature = 0.0f;
  private static float _maxCoreAvgTemperature = 0.0f;
  private static float _maxCoreMaxTemperature = 0.0f;
}
