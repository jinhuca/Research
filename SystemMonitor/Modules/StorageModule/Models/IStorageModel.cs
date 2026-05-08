using StorageModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StorageModule.Models; 
public interface IStorageModel {
  ObservableCollection<DiskInfo> Disks { get; set; }

}
