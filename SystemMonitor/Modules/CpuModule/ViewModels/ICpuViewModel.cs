using System;
using System.Collections.Generic;
using System.Text;

namespace CpuModule.ViewModels;

public interface ICpuViewModel {
  string BrandName { get; set; }
  string VendorName { get; set; }
  string BaseSpeed { get; set; }
  int SocketNum { get; set; }
  int PhysicalCoresNum { get; set; }
  int LogicalCoresNum { get; set; }
  string Virtualization { get; set; }
  string L1CacheSize { get; set; }
  string L1CacheLineSize { get; set; }
  string L2CacheSize { get; set; }
  string L3CacheSize { get; set; }
  double Utilization { get; set; }
  double CurrentSpeed { get; set; }
  int Processes { get; set; }
  int Threads { get; set; }
  int Handles { get; set; }
  TimeSpan UpTime { get; set; }
}
