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
      ex => { },
      () => { });

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
    LiveInfo.CpuOverallLiveInfo.PackageTemperature = newItem.CpuOverallLiveInfo.PackageTemperature;
    LiveInfo.CpuOverallLiveInfo.CpuSpeed = newItem.CpuOverallLiveInfo.CpuSpeed;

    LiveInfo.CpuOverallLiveInfo.Voltage = newItem.CpuOverallLiveInfo.Voltage;

    _maxPlatformPower = newItem.CpuOverallLiveInfo.PlatformPower.max.HasValue 
      ? MathF.Max(newItem.CpuOverallLiveInfo.PlatformPower.max.Value, _maxPlatformPower)
      : _maxPlatformPower;
    var platformPowerValue_ = newItem.CpuOverallLiveInfo.PlatformPower.val.HasValue 
      ? newItem.CpuOverallLiveInfo.PlatformPower.val 
      : LiveInfo.CpuOverallLiveInfo.PlatformPower.val;
    var platformPowerMax_ = newItem.CpuOverallLiveInfo.PlatformPower.max.HasValue 
      ? MathF.Max(newItem.CpuOverallLiveInfo.PlatformPower.max.Value, _maxPlatformPower) 
      : _maxPlatformPower;
    LiveInfo.CpuOverallLiveInfo.PlatformPower = (platformPowerValue_, platformPowerMax_);

    _maxPackagePower = newItem.CpuOverallLiveInfo.PackagePower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.PackagePower.max.Value, _maxPackagePower) 
      : _maxPackagePower;
    var packagePowerValue_ = newItem.CpuOverallLiveInfo.PackagePower.val.HasValue
      ? newItem.CpuOverallLiveInfo.PackagePower.val
      : LiveInfo.CpuOverallLiveInfo.PackagePower.val;
    var packagePowerMax_ = newItem.CpuOverallLiveInfo.PackagePower.max.HasValue
      ? MathF.Max(newItem.CpuOverallLiveInfo.PackagePower.max.Value, _maxPackagePower)
      : _maxPackagePower;
    LiveInfo.CpuOverallLiveInfo.PackagePower = (packagePowerValue_, packagePowerMax_);

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

    LiveInfo.CpuOverallLiveInfo.CoreAvgTemperature = newItem.CpuOverallLiveInfo.CoreAvgTemperature;
    LiveInfo.CpuOverallLiveInfo.CoreMaxTemperature = newItem.CpuOverallLiveInfo.CoreMaxTemperature;

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

  private float _maxPlatformPower = 0.0f;
  private float _maxPackagePower = 0.0f;
  private float _maxCoresPower = 0.0f;
  private float _maxMemoryPower = 0.0f;
}
