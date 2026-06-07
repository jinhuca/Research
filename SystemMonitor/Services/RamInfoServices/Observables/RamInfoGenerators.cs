using RamInfoServices.Queries;
using DataStructures.Ram.Interfaces;
using System.Reactive.Linq;
using DataStructures.Ram.Implementations;

namespace RamInfoServices.Observables; 
public static class RamInfoGenerators {
  public static IObservable<RamSummaryInfo> GenerateRamSummaryInfo(TimeSpan interval) {
    return Observable.Defer(() => Observable.FromAsync(async token => await Task.Run(() => RamInfoQueries.QueryRamSummaryInfo()))
    .Concat(Observable.Delay(Observable.Empty<RamSummaryInfo>(), interval))
    .Repeat());
  }
}
