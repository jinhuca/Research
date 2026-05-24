using System.Collections.ObjectModel;

namespace CpuModule.ViewModels.Interfaces;

public interface ICpuLiveViewModel {
  ICpuOverallLiveViewModel CpuOverallLiveViewModel { get; set; }
  ObservableCollection<ICoreLiveViewModel> CoreLiveViewModel { get; set; }
}
