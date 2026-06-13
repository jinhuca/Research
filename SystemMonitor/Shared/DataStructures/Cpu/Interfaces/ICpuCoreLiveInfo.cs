using DataStructures.TypeDefinitions;
using DataStructures.Types;

namespace DataStructures.Cpu.Interfaces;

public interface ICpuCoreLiveInfo {
  string Name { get; set; }
  SensorReading Voltage { get; set; }
  SensorReading Speed { get; set; }
  SensorReading Temperature { get; set; }
  SensorReading Load { get; set; }
}
