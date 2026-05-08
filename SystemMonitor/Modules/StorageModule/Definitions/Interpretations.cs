using System;
using System.Collections.Generic;
using System.Text;

namespace StorageModule.Definitions; 
internal class Interpretations {
  public static string ConvertMediaType(int mediaType) {
    return mediaType switch {
      3 => "HDD",
      4 => "SDD",
      _ => string.Empty,
    };
  }
}
