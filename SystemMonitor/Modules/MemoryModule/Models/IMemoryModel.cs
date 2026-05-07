using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MemoryModule.Models; 
public interface IMemoryModel {
  ulong TotalInstalledMemory { get; set; }
  ulong AvailableMemory { get; set; }
  int SlotsUsed { get; set; }
  uint TotalSlots { get; set; }
  //string FormFactor { get; set; }
  ulong HardwareReservedMemory { get; set; }
  List<IStickInfo> RAMStickInfo { get; set; }
}

// not used for now
public interface IHardwareReservedMemoryInfo {
  int TotalPhysicalMemory { get; set; }
  int TotalVisibleMemory { get; set; }
  int HardwareReservedMemory { get; set; }
}

public interface IStickInfo {
  ulong Capacity { get; set; }
  uint Speed { get; set; }
  string FormFactor { get; set; }
}

public class StickInfo : BindableBase, IStickInfo {
  public StickInfo(ulong capacity, uint speed, string factor) {
    Capacity = capacity;
    Speed = speed;
    FormFactor = factor;
  }

  private ulong _capacity = 0u;
  public ulong Capacity { 
    get => _capacity; 
    set => SetProperty(ref _capacity, value); 
  }

  private uint _speed = 0;
  public uint Speed { 
    get => _speed; 
    set => SetProperty(ref _speed, value);
  }

  private string _formFactor = string.Empty;
  public string FormFactor { 
    get => _formFactor; 
    set => SetProperty(ref _formFactor, value); 
  }
}
