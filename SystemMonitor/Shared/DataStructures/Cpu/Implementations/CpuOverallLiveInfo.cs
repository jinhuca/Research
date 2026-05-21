using DataStructures.Cpu.Interfaces;
using DataStructures.TypeDefinitions;

namespace DataStructures.Cpu.Implementations;


public class CpuOverallLiveInfo : ICpuOverallLiveInfo {
  public SensorDataType BusSpeed { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType CpuSpeed { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType Voltage { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType PlatformPower { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType PackagePower { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType MemoryPower { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType CoresPower { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType PackageTemperature { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType CoreMaxTemperature { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType CoreAvgTemperature { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType TotalLoad { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
  public SensorDataType CoreMaxLoad { get; set; } = new SensorDataType(0.0f, 0.0f, 0.0f);
}