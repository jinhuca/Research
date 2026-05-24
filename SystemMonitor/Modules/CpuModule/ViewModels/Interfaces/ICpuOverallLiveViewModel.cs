namespace CpuModule.ViewModels.Interfaces;

public interface ICpuOverallLiveViewModel {
  float? LoadViewModel { get; set; }
  
  float? SpeedViewModel { get; set; }

  //float? VoltageViewModel { get; set; }

  int ProcessNum { get; set; }
  int ThreadNum { get; set; }
  int HandleNum { get; set; }
  TimeSpan UpTime { get; set; }

  float PlatformVoltageValueViewModel { get; set; }
  float PlatformVoltageMinViewModel { get; set; }
  float PlatformVoltageMaxViewModel { get; set; }

  float PlatformPowerValueViewModel { get; set; }
  float PlatformPowerMinViewModel { get; set; }
  float PlatformPowerMaxViewModel { get; set; }

  float PackagePowerValueViewModel { get; set; }
  float PackagePowerMinViewModel { get; set; }
  float PackagePowerMaxViewModel { get; set; }

  float CoresPowerValueViewModel { get; set; }
  float CoresPowerMinViewModel { get; set; }
  float CoresPowerMaxViewModel { get; set; }

  float MemoryPowerValueViewModel { get; set; }
  float MemoryPowerMinViewModel { get; set; }
  float MemoryPowerMaxViewModel { get; set; }

  float? TemperatureViewModel { get; set; }

  float PackageTemperatureValueViewModel { get; set; }
  float PackageTemperatureMinViewModel { get; set; }
  float PackageTemperatureMaxViewModel { get; set; }

  float CoreAvgTemperatureValueViewModel { get; set; }
  float CoreAvgTemperatureMinViewModel { get; set; }
  float CoreAvgTemperatureMaxViewModel { get; set; }

  float CoreMaxTemperatureValueViewModel { get; set; }
  float CoreMaxTemperatureMinViewModel { get; set; }
  float CoreMaxTemperatureMaxViewModel { get; set; }
}
