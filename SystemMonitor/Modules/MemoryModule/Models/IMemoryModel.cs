using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MemoryModule.Models; 
public interface IMemoryModel {
  float TotalMemory { get; set; }
  float Speed { get; set; }
  int SlotsUsed { get; set; }
  int TotalSlots { get; set; }
  string FormFactor { get; set; }
  IHardwareReservedMemoryInfo HardwareReservedMemory { get; set; }
  IStickInfo StickInfo { get; set; }
}

public interface IHardwareReservedMemoryInfo {
  int TotalPhysicalMemory { get; set; }
  int TotalVisibleMemory { get; set; }
  int HardwareReservedMemory { get; set; }
}

public interface IStickInfo {
  string Capacity { get; set; }
  string Speed { get; set; }
  string FormFactor { get; set; }
}