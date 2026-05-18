using SystemManagementProvider.Interfaces;

namespace SystemManagementProvider;

public class SystemManagementProviderModule : IModule {
  public void OnInitialized(IContainerProvider containerProvider) {

  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ISMProvider, SMProvider>();
  }
}
