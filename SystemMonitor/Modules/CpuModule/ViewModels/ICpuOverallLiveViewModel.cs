namespace CpuModule.ViewModels;

public interface ICpuOverallLiveViewModel {
  float? LoadViewModel { get; set; }
  float? TemperatureViewModel { get; set; }
  float? SpeedViewModel { get; set; }
  float? VoltageViewModel { get; set; }

  float PlatformPowerValueViewModel { get; set; }
  float PlatformPowerMaxViewModel { get; set; }

  float PackagePowerValueViewModel { get; set; }
  float PackagePowerMaxViewModel { get; set; }

  float CoresPowerValueViewModel { get; set; }
  float CoresPowerMaxViewModel { get; set; }

  float MemoryPowerValueViewModel { get; set; }
  float MemoryPowerMaxViewModel { get; set; }
}
