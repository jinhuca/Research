using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

using static QueryConstants.Management.Win32VideoController;

namespace SystemInfoCollection;

public static class WIn32VideoController {
  public static List<(string key, string infoItem, string description)> Details = new();

  public static void Init() {
    Details.Clear();
    using ManagementObjectSearcher searcher = new(QueryString);
    foreach (var obj in searcher.Get()) {
      Details.Add((VideoProcessorKey, Convert.ToString(obj[VideoProcessorKey]), VideoProcessorDesc));
      Details.Add((AdapterCompatibilityKey, Convert.ToString(obj[AdapterCompatibilityKey]), AdapterCompatibilityDesc));
      Details.Add((AdapterDACTypeKey, Convert.ToString(obj[AdapterDACTypeKey]), AdapterDACTypeDesc));
      Details.Add((AdapterRAMKey, Convert.ToString(Convert.ToUInt32(obj[AdapterRAMKey])), AdapterRAMDesc));
    }
    PrintResult();
  }

  public static void tryit() {
    try {
      // Query all video controllers on the system
      using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
          "SELECT Name, AcceleratorCapabilities FROM Win32_VideoController")) {
        foreach (ManagementObject obj in searcher.Get()) {
          Console.WriteLine("Video Controller: {0}", obj["Name"]);

          // AcceleratorCapabilities is an array of ushort values
          if (obj["AcceleratorCapabilities"] is ushort[] capabilities && capabilities.Length > 0) {
            Console.WriteLine("  Accelerator Capabilities:");
            foreach (ushort cap in capabilities) {
              Console.WriteLine("    {0} - {1}", cap, GetCapabilityDescription(cap));
            }
          }
          else {
            Console.WriteLine("  No accelerator capabilities reported.");
          }

          Console.WriteLine();
        }
      }
    }
    catch (ManagementException mex) {
      Console.WriteLine("WMI query error: " + mex.Message);
    }
    catch (Exception ex) {
      Console.WriteLine("Unexpected error: " + ex.Message);
    }
  }

  static string GetCapabilityDescription(ushort code) {
    return code switch
    {
      1 => "Other",
      2 => "Unknown",
      3 => "Graphics Accelerator",
      4 => "3D Accelerator",
      _ => "Reserved/Undefined"
    };
  }

  static void PrintResult() {
    foreach (var obj in Details) {
      Console.WriteLine(obj.ToString());
    }
  }
}
