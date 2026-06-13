using CpuModule.ViewModels.Interfaces;
using CrystalMonitor.Hardware;
using DataStructures.Types;

namespace CpuModule.ViewModels.Implementations;

public class CoreLiveViewModel : BindableBase, ICoreLiveViewModel {
  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private SensorReading _voltage = new("", HardwareType.Cpu, "", SensorType.Voltage, null, null, null, null);
  public SensorReading Voltage {
    get => _voltage;
    set => SetProperty(ref _voltage, value);
  }

  private SensorReading _temperature = new("", HardwareType.Cpu, "", SensorType.Temperature, null, null, null, null);
  public SensorReading Temperature {
    get => _temperature;
    set => SetProperty(ref _temperature, value);
  }

  private SensorReading _load = new("", HardwareType.Cpu, "", SensorType.Load, null, null, null, null);
  public SensorReading Load {
    get => _load;
    set => SetProperty(ref _load, value);
  }

  private SensorReading _speed = new("", HardwareType.Cpu, "", SensorType.Clock, null, null, null, null);
  public SensorReading Speed {
    get => _speed;
    set => SetProperty(ref _speed, value);
  }
}