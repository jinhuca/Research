using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Management;
using System.Text;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider.Queries;

public class QueryProcessors {
  private ManagementObjectSearcher _searcher;
  private static Dictionary<string, (string, string)> info = [];

  public QueryProcessors(ManagementObjectSearcher searcher) {
    _searcher = searcher;
  }

  public (string, string) Query(string queryString) {
    (string, string) result_ = new();
    try {
      if (_searcher == null) {
        return (string.Empty, string.Empty);
      }
      foreach (var obj in _searcher.Get()) {
        result_ = (queryString, Convert.ToString(obj[queryString])!);
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
    return result_;
  }

  public void Invoke() {
    try {
      if (_searcher == null) {
        return;
      }
      foreach (var obj in _searcher.Get()) {
        info.Add(AddressWidthKey, (Convert.ToString(obj[AddressWidthKey]), AddressWidthDesc));
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
  }


}
