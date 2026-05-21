using DataStructures.TypeDefinitions;

namespace DataStructures.Cpu.Interfaces;

public interface ICpuOverallLiveInfo {
  SensorDataType BusSpeed { get; set; }
  SensorDataType CpuSpeed { get; set; }
  SensorDataType Voltage { get; set; }
  SensorDataType PlatformPower { get; set; }
  SensorDataType PackagePower { get; set; }
  SensorDataType MemoryPower { get; set; }
  SensorDataType CoresPower { get; set; }
  SensorDataType PackageTemperature { get; set; }
  SensorDataType CoreMaxTemperature { get; set; }
  SensorDataType CoreAvgTemperature { get; set; }
  SensorDataType TotalLoad { get; set; }
  SensorDataType CoreMaxLoad { get; set; }
}
