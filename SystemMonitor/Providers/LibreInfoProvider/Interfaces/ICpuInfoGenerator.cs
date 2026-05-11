using DataExchange.Cpu;

namespace LibreInfoProvider.Interfaces; 
public interface ICpuInfoGenerator {
  ICpuSummaryInfo GetCpuSummaryInfo();
  List<ICpuCoreInfo> GetCpuCoreInfo();
}
