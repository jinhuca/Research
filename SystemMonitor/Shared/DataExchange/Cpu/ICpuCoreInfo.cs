namespace DataExchange.Cpu;

public interface ICpuCoreInfo {
  string Name { get; set; }
  float? Voltage { get; set; }        // Volt
  float? Speed { get; set; }          // MHz
  float? Temperature { get; set; }    // Celsius
  float? Load { get; set; }           // Percentage
}
