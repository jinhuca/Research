namespace DataStructures.Cpu.Interfaces;

public interface ICpuOverallLiveInfo {
  (float? val, float? max) BusSpeed { get; set; }
  (float? val, float? max) CpuSpeed { get; set; }
  (float? val, float? max) Voltage { get; set; }
  (float? val, float? max) PlatformPower { get; set; }
  (float? val, float? max) PackagePower { get; set; }
  (float? val, float? max) MemoryPower { get; set; }
  (float? val, float? max) CoresPower { get; set; }
  (float? val, float? max) PackageTemperature { get; set; }
  (float? val, float? max) CoreMaxTemperature { get; set; }
  (float? val, float? max) CoreAvgTemperature { get; set; }
  (float? val, float? max) TotalLoad { get; set; }
  (float? val, float? max) CoreMaxLoad { get; set; }
}
