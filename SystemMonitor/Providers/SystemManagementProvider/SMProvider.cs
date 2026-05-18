using System.Management;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;
using SystemManagementProvider.Queries;

namespace SystemManagementProvider; 
public class SMProvider : ISMProvider {

  private readonly ManagementObjectSearcher? _objectSearcher;

  public SMProvider() {

  }

  //public SMProvider(ManagementObjectSearcher searcher_) {
  //  _objectSearcher = searcher_;
  //  initializeData();
  //}

  private void initializeData() {
    if (_objectSearcher == null) {
      return;
    }

  }

  public Dictionary<string, string>? ProcessorInfo { get; private set; }

  public void Invoke_Query_Processors(string queryText) {
    QueryProcessors query = new();
    query.Query(queryText);
  }

  public void Invoke_Query_OperatingSystem(string queryText) {
    QueryOperatingSystem query = new();
    query.GetInfo();
  }

  public string Query(SMCategories category, string query) {
    var result_ = string.Empty;

    switch (category) {
      case SMCategories.Processor:
        break;
      case SMCategories.Bios:
        break;
      case SMCategories.OperatingSystem:
        break;
      case SMCategories.Gpu:
        break;
    }

    return result_;
  }

  public ISMQuery GetQueryProvider(SMCategories category) {
    switch (category) {
      case SMCategories.Processor:
        return new QueryProcessors();
      case SMCategories.Bios:
        break;
      case SMCategories.OperatingSystem:
        return new QueryOperatingSystem();
      case SMCategories.Gpu:
        return new QueryGpu();
    }
    throw new NotImplementedException();
  }
}