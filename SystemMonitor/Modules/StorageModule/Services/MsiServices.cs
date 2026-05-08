using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Management.Infrastructure;

namespace StorageModule.Services; 
internal static class MsiServices {
  public static void QueryMsi() {
    using CimSession session = CimSession.Create(null);
    var instances = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_OperatingSystem");
    foreach(var instance in instances) {
      Debug.WriteLine($"OS: {instance.CimInstanceProperties["Caption"].Value}");
    }
  }
}
