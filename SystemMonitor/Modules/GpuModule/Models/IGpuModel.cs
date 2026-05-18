namespace GpuModule.Models;

public interface IGpuModel {
  string Name { get; set; }
  BasicInfo BasicInfo { get; set; }
  float Utilization { get; set; }
  float Speed { get; set; }
  float Temperature { get; set; }
}
