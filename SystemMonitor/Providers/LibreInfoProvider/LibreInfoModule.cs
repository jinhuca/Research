using LibreInfoProvider.Implementations;

namespace LibreInfoProvider;

public class LibreInfoModule : IModule {
  public void OnInitialized(IContainerProvider containerProvider) {

  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.RegisterSingleton<CpuInfoGenerator>();
  }
}
