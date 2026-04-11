using QueryConstants.Management;
using System;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using SystemInfoCollection;
using static QueryConstants.Management.Win32Processor;
using static QueryConstants.Management.Win32Fan;
using static QueryConstants.Management.Win32BIOS;

namespace SystemInfoCollection; 
public static class Fan {
  public static void Query() {
    try {
      // Define the WMI query
      ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Fan");
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

      // Execute the query and iterate through results
      foreach (ManagementObject fan in searcher.Get()) {
        Console.WriteLine("Fan Name: {0}", fan["Name"]);
        Console.WriteLine("Status: {0}", fan["Status"]);
        Console.WriteLine("Active Cooling: {0}", fan["ActiveCooling"]);
        // Note: DesiredSpeed often returns null on many consumer systems
        Console.WriteLine("Desired Speed: {0}", fan["DesiredSpeed"]);
        Console.WriteLine("---------------------------------------");
      }
    }
    catch (ManagementException e) {
    }

  }
}
