using System.Management;
using SystemManagementProvider.Queries;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider; 

public class SMProvider {
  //private readonly QueryProcessors _queryProcessorObj;
  private readonly ManagementObjectSearcher _searchObjectSearcher;
  public Dictionary<string, string> ProcessorInfo { get; private set; }

  public SMProvider(ManagementObjectSearcher searcher_) {
    _searchObjectSearcher = searcher_;
  }

  public void Invoke_Query_Processors(string queryText) {
    QueryProcessors query = new(_searchObjectSearcher);
    query.Query(queryText);
  }


  public void Invoke_Query_OperatingSystem(ManagementObjectSearcher searcher) { 
  }
}
