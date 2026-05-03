using System;
using System.Collections.Generic;
using System.Text;

namespace CpuModule.Models; 
public class RealTimeInfo : BindableBase {
  public double Utilization { get; set; }
  public double Speed { get; set; }
  public int Processes { get; set; }
  public int Threads { get; set; }
  public int Handles { get; set; }
  public float Temperature { get; set; }
  public TimeSpan UpTime { get; set; }
}