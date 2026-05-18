using StorageModule.Interfaces;
using StorageModule.Services;
using System.Collections.ObjectModel;

namespace StorageModule.Models;

public class StorageModel : BindableBase, IStorageModel {
  public StorageModel() {
    var temp = WmiServices.GetDiskinfo();
    var tempx = MsiServices.QueryMsiLogicalDisks();
  }

  private ObservableCollection<DiskInfo> _disks;
  public ObservableCollection<DiskInfo> Disks {
    get => _disks;
    set => SetProperty(ref _disks, value);
  }
}
