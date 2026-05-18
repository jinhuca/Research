using MemoryModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoryModule.ViewModels;  
public interface IMemoryViewModel {
  string TotalMemorySize { get; }
  string AvailableMemorySize { get; }
  int UsedSlotNum { get; }
  //List<IStickInfo> StickInfo { get; }
  List<StickInfoViewModel> StickViewModel { get; }
}
