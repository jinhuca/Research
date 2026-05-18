using DataStructures.Cpu.Interfaces;
using System.ComponentModel;

namespace CpuModule.Models;

public interface ICpuModel : INotifyPropertyChanged {
  ICpuSummaryInfo SummaryInfo { get; set; }
  ICpuLiveInfo LiveInfo { get; set; }
}
