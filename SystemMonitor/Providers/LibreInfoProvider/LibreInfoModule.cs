using LibreInfoProvider.Implementations;
using LibreInfoProvider.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibreInfoProvider;

public class LibreInfoModule : IModule {
  public void OnInitialized(IContainerProvider containerProvider) {

  }

  public void RegisterTypes(IContainerRegistry containerRegistry) {
    containerRegistry.Register<ICpuInfoGenerator, CpuInfoGenerator>();
  }
}
