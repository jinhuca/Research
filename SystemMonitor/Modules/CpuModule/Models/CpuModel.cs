using DataStructures.Cpu.Interfaces;
using DataStructures.Cpu.Implementations;
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
}
