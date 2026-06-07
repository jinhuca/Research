namespace QueryGpu; 
public class QueryGpuProgram {
  static void Main(string[] args) {
    using var monitor = new GpuMonitor();
    monitor.PrintAllGpuInfo();
    monitor.PrintSpecificMetrics();
    monitor.Dispose();
  }
}
