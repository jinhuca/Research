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
    
  }

  private float _totalMemory;
  public float TotalMemory {
    get => _totalMemory;
    set => SetProperty(ref _totalMemory, value);
  }

  private float _speed;
  public float Speed {
    get => _speed;
    set => SetProperty(ref _speed, value);
  }

  private int _slotsUsed;
  public int SlotsUsed {
    get => _slotsUsed;
    set => SetProperty(ref _slotsUsed, value);
  }

  private int _totalSlots;
  public int TotalSlots {
    get => _totalSlots;
    set => SetProperty(ref _totalSlots, value);
  }

  private string _formFactor;
  public string FormFactor {
    get => _formFactor;
    set => SetProperty(ref _formFactor, value);
  }

  private IHardwareReservedMemoryInfo _hardwareReservedMemory;
  public IHardwareReservedMemoryInfo HardwareReservedMemory {
    get => _hardwareReservedMemory;
    set => SetProperty(ref _hardwareReservedMemory, value);
  }

  private IStickInfo _stickInfo;
  public IStickInfo StickInfo { 
    get => _stickInfo; 
    set => SetProperty(ref _stickInfo, value); }
}