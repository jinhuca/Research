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

  /*
  public static IObservable<ICpuOverallLiveInfo> GenerateCpuOverallLiveInfo(TimeSpan interval) {
    return Observable.Defer(() => Observable.FromAsync(() => ExecuteCpuOverallLiveInfoAsync()))
      .Concat(Observable.Delay(Observable.Empty<ICpuOverallLiveInfo>(), interval))
      .Repeat();
  }

  private static async Task<ICpuOverallLiveInfo> ExecuteCpuOverallLiveInfoAsync() {
    return await Task.Run(() => CpuInfoQueries.QueryCpuOverallLiveInfo());
  }

  public static IObservable<List<ICpuCoreLiveInfo>> GenerateCpuCoreInfoObservables(TimeSpan interval) {
    return Observable.Defer(() => Observable.FromAsync(() => ExecuteCpuCoreLiveInfoQueryAsync()))
      .Concat(Observable.Delay(Observable.Empty<List<ICpuCoreLiveInfo>>(), interval))
      .Repeat();
  }

  private static async Task<List<ICpuCoreLiveInfo>> ExecuteCpuCoreLiveInfoQueryAsync() {
    return await Task.Run(() => CpuInfoQueries.QueryCpuCoreLiveInfo());
  }
  */
}
