using CpuInfoServices.Observables;
using DataStructures.Cpu.Implementations;
using DataStructures.Cpu.Interfaces;

namespace CpuInfoServices; 
public class CpuInfoServicesModule : IModule {
  public void OnInitialized(IContainerProvider containerProvider) {
  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuSummaryInfo, CpuSummaryInfo>();
    containerRegistry.Register<ICpuLiveInfo, CpuLiveInfo>();
  }
}
