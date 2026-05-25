using CpuModule.ViewModels.Interfaces;
using DataStructures.TypeDefinitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CpuModule.ViewModels.Implementations;

public class CoreLiveViewModel : BindableBase, ICoreLiveViewModel {
  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private SensorDataType _voltage = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Voltage {
    get => _voltage;
    set => SetProperty(ref _voltage, value);
  }

  private SensorDataType _temperature = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Temperature {
    get => _temperature;
    set => SetProperty(ref _temperature, value);
  }

  private SensorDataType _load = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Load {
    get => _load;
    set => SetProperty(ref _load, value);
  }

  private SensorDataType _speed = new SensorDataType { Value = 0.0f, Min = 0.0f, Max = 0.0f };
  public SensorDataType Speed {
    get => _speed;
    set => SetProperty(ref _speed, value);
  }
}