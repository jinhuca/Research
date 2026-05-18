using BiosModule.ViewModels.SubViewModels;

namespace BiosModule.ViewModels;

public interface IBiosViewModel {
  ISM_SummaryViewModel Summary { get; set; }
}
