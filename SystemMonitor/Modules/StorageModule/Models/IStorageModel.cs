using StorageModule.Interfaces;
using System.Collections.ObjectModel;

namespace StorageModule.Models;

public interface IStorageModel {
  ObservableCollection<DiskInfo> Disks { get; set; }

}
