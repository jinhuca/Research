using StorageModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageModule.ViewModels; 
public class StorageViewModel : BindableBase, IStorageViewModel {
  private IStorageModel _model;
  public StorageViewModel(IStorageModel model) {
    _model = model;
  }
}
