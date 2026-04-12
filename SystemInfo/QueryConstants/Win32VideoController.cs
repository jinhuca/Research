using System;
using System.Collections.Generic;
using System.Text;

namespace QueryConstants.Management;

public static class Win32VideoController {
  public const string QueryString = "SELECT * FROM Win32_VideoController";

  public const string VideoProcessorKey = "VideoProcessor";
  public const string VideoProcessorDesc = "Free-form string describing the video processor";

  public const string AcceleratorCapabilitiesKey = "AcceleratorCapabilities";
  public const string AcceleratorCapabilitiesDesc = "Array of graphics and 3-D capabilities of the video controller";

  public const string AdapterCompatibilityKey = "AdapterCompatibility";
  public const string AdapterCompatibilityDesc = "General chipset used for this controller to compare compatibilities with the system";

  public const string AdapterDACTypeKey = "AdapterDACType";
  public const string AdapterDACTypeDesc = "Name or identifier of the digital-to-analog converter (DAC) chip";

  public const string AdapterRAMKey = "AdapterRAM";
  public const string AdapterRAMDesc = "Memory size of the video adapter";
}
