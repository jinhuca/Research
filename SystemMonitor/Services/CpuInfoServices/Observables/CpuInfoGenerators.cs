using CpuInfoServices.Queries;
using DataStructures.Cpu.Interfaces;
using System.Reactive.Linq;

namespace CpuInfoServices.Observables;

public static class CpuInfoGenerators {
  public static IObservable<ICpuSummaryInfo> GenerateCpuSummaryInfo(TimeSpan interval) {
    return Observable.Defer(() => Observable.FromAsync(async token => await Task.Run(() => CpuInfoQueries.QuerySummaryInfo()))
    .Concat(Observable.Delay(Observable.Empty<ICpuSummaryInfo>(), interval)));
  }

  public static IObservable<ICpuLiveInfo> GenerateCpuLiveInfo(TimeSpan interval) {
    return Observable.Defer(() => Observable.FromAsync(async token => await Task.Run(() => CpuInfoQueries.QueryCpuLiveInfo())))
      .Concat(Observable.Delay(Observable.Empty<ICpuLiveInfo>(), interval))
      .Repeat();
  }
}
