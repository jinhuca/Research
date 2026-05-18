using StorageModule.Models;

namespace StorageModule.ViewModels;

public class StorageViewModel : BindableBase, IStorageViewModel {
  private IStorageModel _model;
  public StorageViewModel(IStorageModel model) {
    _model = model;
  }
}
