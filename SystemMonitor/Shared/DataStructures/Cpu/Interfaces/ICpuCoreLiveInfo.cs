namespace DataStructures.Cpu.Interfaces;

public interface ICpuCoreLiveInfo {
  string Name { get; set; }
  (float? val, float? max) Voltage { get; set; }
  (float? val, float? max) Speed { get; set; }
  (float? val, float? max) Temperature { get; set; }
  (float? val, float? max) Load { get; set; }
}
