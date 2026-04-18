using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider.Queries; 
internal static class QueryProcessors {
  private static Dictionary<string, (string, string)> info = [];
  public static void Invoke() {
    try {
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query_String);
      if (searcher == null) {
        return;
      }
      foreach (var obj in searcher.Get()) {
        info.Add(AddressWidthKey, (Convert.ToString(obj[AddressWidthKey]), AddressWidthDesc));
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
  }
}
