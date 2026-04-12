using System;
using System.Collections.Generic;
using System.Text;

namespace QueryConstants; 
public static class Win32VideoController {
  public const string QueryString = "SELECT * FROM Win32_VideoController";

  public const string VideoProcessorKey = "VideoProcessor";
  public const string VideoProcessorDesc = "Free-form string describing the video processor";
}
