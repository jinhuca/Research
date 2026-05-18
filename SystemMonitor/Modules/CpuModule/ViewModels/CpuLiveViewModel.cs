using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace CpuModule.ViewModels;

public class CpuLiveViewModel : BindableBase, ICpuLiveViewModel {
  private ICpuOverallLiveViewModel? _cpuOverallLiveViewModel = new CpuOverallLiveViewModel();
  public ICpuOverallLiveViewModel? CpuOverallLiveViewModel {
    get => _cpuOverallLiveViewModel;
    set => SetProperty(ref _cpuOverallLiveViewModel, value);
  }

  public ICoreLiveViewModel CoreLiveViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

}
