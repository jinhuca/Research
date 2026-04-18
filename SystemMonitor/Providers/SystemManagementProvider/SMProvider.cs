using System.Management;
using SystemManagementProvider.Queries;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider; 

public static class SMProvider {
  public static void Query_SM() {
    try {
      ManagementObjectSearcher searcher = new(Query_String);
    }
    catch(ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
  }

  public static void Invoke_Query_Processors() {
    QueryProcessors.Invoke();
  }

  public static void Invoke_Query_OperatingSystem(ManagementObjectSearcher searcher) { 
  }
}
