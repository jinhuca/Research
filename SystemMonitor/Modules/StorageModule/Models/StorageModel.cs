using StorageModule.Interfaces;
using StorageModule.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StorageModule.Models;

public class StorageModel : BindableBase, IStorageModel {
  public StorageModel() {
    var temp = WmiServices.GetDiskinfo();
    MsiServices.QueryMsi();
  }

  private ObservableCollection<DiskInfo> _disks;
  public ObservableCollection<DiskInfo> Disks {
    get => _disks;
    set => SetProperty(ref _disks, value);
  }
}
