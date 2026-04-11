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
public static class Bios {
  public static void Query() {
    try {
      ObjectQuery query = new(Win32BiosQuery);
      ManagementObjectSearcher searcher = new(query);

      foreach (ManagementObject bios in searcher.Get()) {
        Console.WriteLine("Bios Name = " + Convert.ToString(bios[BiosName]));
        Console.WriteLine("Bios Version = " + Convert.ToString(bios[BiosVersion]));
        Console.WriteLine("Bios Language = " + Convert.ToString(bios[BiosLanguage]));
        Console.WriteLine("Bios Release Date = " + Convert.ToString(bios[BiosReleaseDate]));
        Console.WriteLine("Bios Serial Number = " + Convert.ToString(bios[BiosSerialNumber]));
        Console.WriteLine("Bios Caption = " + Convert.ToString(bios[BiosCaption]));
        Console.WriteLine("BiosCurrentLanguage = " + Convert.ToString(bios[BiosCurrentLanguage]));
      }
    }
    catch (ManagementException e) {
    }
  }
}
