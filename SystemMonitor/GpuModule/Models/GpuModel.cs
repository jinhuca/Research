using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace GpuModule.Models;

struct GpuModelDefinitions {
  public const int TimerStartDelay = 0;
  public const int TimerInterval = 1000;
}

public class GpuModel : BindableBase, IGpuModel {
  private Timer _timer;
  private readonly ISMProvider? _smProvider;
  public Dictionary<string, Dictionary<string, (string, string)>> GpuInfoList { get; private set; }
  = new Dictionary<string, Dictionary<string, (string, string)>>();

  public GpuModel(ISMProvider? smProvider = null) {
    _smProvider = smProvider;
    Init();
    initTimer();
  }

  private void initTimer() {
    _timer = new Timer(
      callback: fetchSystemInfo,
      state: DateTime.UtcNow,
      dueTime: GpuModelDefinitions.TimerStartDelay,
      period: GpuModelDefinitions.TimerInterval);
  }

  private void fetchSystemInfo(object? state) {
    Utilization = GetCurrentGpuUsage();
    RaisePropertyChanged(nameof(Utilization));
    Debug.WriteLine($"Current GPU Usage: {GetCurrentGpuUsage():F2}%");
  }

  private float GetCurrentGpuUsage() {
    Computer computer = new Computer { IsGpuEnabled = true };
    computer.Open();
    foreach(var hardware in computer.Hardware) {
      if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel) {
        hardware.Update();
        foreach (var sensor in hardware.Sensors) {
          if (sensor.SensorType == SensorType.Load && sensor.Name.Equals("GPU Core", StringComparison.OrdinalIgnoreCase)) {
            return sensor.Value ?? 0.0f;
          }
        }
      }
    }
    return 0.0f; // Placeholder value
  }

  private void Init() {
    try {
      ISMQuery? gpuQuery_ = _smProvider?.GetQueryProvider(SMCategories.Gpu);
      GpuInfoList = gpuQuery_?.QueryMultiple("SELECT * FROM Win32_VideoController") ?? [];

      //var temp =gpuQuery_.QueryMultiple("SELECT * FROM Win32_VideoController").Values;
    }
    catch (Exception ex) {
      // Handle exceptions gracefully, perhaps log them
      Name = "Unknown GPU";
    }
  }

  private string _name;
  public string Name {
    get => _name; 
    set => SetProperty(ref _name, value);
  }

  private BasicInfo _basicInfo = new BasicInfo();
  public BasicInfo BasicInfo {
    get => _basicInfo;
    set => SetProperty(ref _basicInfo, value);
  }
  private float _utilization;
  public float Utilization {
    get => _utilization;
    set => SetProperty(ref _utilization, value);
  }
  private float _speed;
  public float Speed {
    get => _speed;
    set => SetProperty(ref _speed, value);
  }
  private float _temperature;
  public float Temperature {
    get => _temperature;
    set => SetProperty(ref _temperature, value);
  }
}
