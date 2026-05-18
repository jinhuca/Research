using Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace MemoryModule.Models; 
public class MemoryModel : BindableBase, IMemoryModel {
  public MemoryModel() {
    init();
  }

  private void init() {
    //FormFactor = QueryMemory.GetRamFormFactor();
    //QueryMemory.GetHardwareReservedRam();

    //var capacity_ = QueryMemory.GetInstalledMemorySize();
    //Debug.WriteLine(capacity_);
    //var capacityString_ = QueryMemory.GetInstalledMemorySizeInString(capacity_);
    //Debug.WriteLine(capacityString_ + Environment.NewLine);


    //QueryMemory.GetHardwareReservedRam();

    RAMStickInfo = QueryMemory.GetStickInfo();
    TotalInstalledMemory = QueryMemory.GetInstalledMemorySize();
    AvailableMemory = QueryMemory.GetOSVisibleRAMSize();

    SlotsUsed = QueryMemory.GetSlotsUsed();
    //TotalSlots = QueryMemory.GetSlotsTotal();
  }

  private ulong _totalInstalledMemory;
  public ulong TotalInstalledMemory {
    get => _totalInstalledMemory;
    set => SetProperty(ref _totalInstalledMemory, value);
  }

  private ulong _availableMemory;
  public ulong AvailableMemory {
    get=> _availableMemory;
    set=>SetProperty(ref _availableMemory, value);
  }

  private int _slotsUsed;
  public int SlotsUsed {
    get => _slotsUsed;
    set => SetProperty(ref _slotsUsed, value);
  }

  private uint _totalSlots;
  public uint TotalSlots {
    get => _totalSlots;
    set => SetProperty(ref _totalSlots, value);
  }

  private ulong _hardwareReservedMemory;
  public ulong HardwareReservedMemory {
    get => _hardwareReservedMemory;
    set => SetProperty(ref _hardwareReservedMemory, value);
  }

  private List<IStickInfo> _ramStickInfo;
  public List<IStickInfo> RAMStickInfo { 
    get => _ramStickInfo; 
    set => SetProperty(ref _ramStickInfo, value); }
}