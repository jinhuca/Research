
using CpuInfoServices.Observables;
using static CpuInfoServices.Observables.CpuInfoGenerators;

namespace InfoGeneratorTests;  
internal class Program {

  private static void TestCpuInfoQueries() {
    IDisposable cpuObservables = GenerateCpuSummaryInfo(TimeSpan.FromSeconds(0))
      .Subscribe(info_ => Console.WriteLine(info_.BrandName));

    //IDisposable cpuOverall = CpuInfoGenerators.GenerateCpuOverallLiveInfo(TimeSpan.FromSeconds(1))
    //  .Subscribe(info_ => Console.WriteLine($"Bus Speed: {info_.BusSpeed}"));

    //IDisposable coreInfo = CpuInfoGenerators.GenerateCpuCoreInfoObservables(TimeSpan.FromSeconds(1))
    //  .Subscribe(info_ => Console.WriteLine($"Core Temp: {info_[0].Temperature}"));

    IDisposable cpuLive_ = GenerateCpuLiveInfo(TimeSpan.FromSeconds(1))
      .Subscribe(info_ => Console.WriteLine(info_.CpuOverallLiveInfo.BusSpeed));
  }

  //private static void TestSummary() {
  //  var result1_ = CpuInfoServices.Queries.CpuInfoQueries.QuerySummaryInfo();
  //}

  static void Main(string[] args) {
    TestCpuInfoQueries();

    Console.ReadLine();
  }
}
