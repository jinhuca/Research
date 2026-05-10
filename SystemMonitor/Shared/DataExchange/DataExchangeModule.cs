using DataExchange.Cpu;

namespace DataExchange;

public class DataExchangeModule : IModule {
  public void OnInitialized(IContainerProvider containerProvider) {

  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuCoreInfo, CpuCoreInfo>();
  }
}
