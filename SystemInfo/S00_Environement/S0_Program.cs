namespace S00_Environement;

using System;
using System.Management;
using System.Collections;

using QueryConstants.Management;
using SystemInfoCollection;

internal class S0_Program {
  public static void test1() {
    
  }

  public static void Query_CPU() {
    try {
      ManagementObjectSearcher searcher = new(queryString: QueryConstants.Management.Win32Processor.Query_String);
      foreach (var obj in searcher.Get()) {
        Console.WriteLine("Processor Name:     " + obj["Name"]);
        Console.WriteLine("Description:        " + obj["Description"]);
        Console.WriteLine("Architecture:       " + GetArchitecture(Convert.ToUInt16(obj["Architecture"])));
        Console.WriteLine("Socket:             " + obj["SocketDesignation"]);
        Console.WriteLine("Physical Cores:     " + obj["NumberOfCores"]);
        Console.WriteLine("Logical Processors: " + obj["NumberOfLogicalProcessors"]);
        Console.WriteLine("Max Speed:          " + obj["MaxClockSpeed"] + " MHz");
        Console.WriteLine("Processor ID:       " + obj["ProcessorId"]);
      }
    }
    catch(ManagementException e) { 
      Console.WriteLine("Error: "+e.Message);
    }
  }

  private static string GetArchitecture(ushort arch) {
    return arch switch {
      0 => "x86",
      5 => "ARM",
      6 => "Itanium-based systems",
      9 => "x64",
      12 => "ARM64",
      _ => "Unknown"
    };
  }

  private static void f() {
    SystemInfoCollection.Win32Processor.Init();
    //SystemInfoCollection.WIn32VideoController.Init();
    SystemInfoCollection.Win32OperatingSystem.Init();

    //SystemInfoCollection.WIn32VideoController.tryit();
  }

  static void Main(string[] args) {
    //Query_CPU();
    f();
  }
}
