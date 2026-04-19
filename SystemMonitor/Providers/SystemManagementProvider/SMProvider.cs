using System.Management;
using SystemManagementProvider.Queries;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider;

public class SMProvider(ManagementObjectSearcher searcher_) {
  //private readonly QueryProcessors _queryProcessorObj;
  private readonly ManagementObjectSearcher _searchObjectSearcher = searcher_;
  public Dictionary<string, string>? ProcessorInfo { get; private set; }

  public void Invoke_Query_Processors(string queryText) {
    QueryProcessors query = new(_searchObjectSearcher);
    query.Query(queryText);
  }


  public void Invoke_Query_OperatingSystem(string queryText) {
    QueryOperatingSystem query = new(_searchObjectSearcher);
    query.GetInfo();
  }
}
