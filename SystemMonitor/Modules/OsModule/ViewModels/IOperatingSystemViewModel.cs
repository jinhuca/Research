using System;
using System.Collections.Generic;
using System.Text;

namespace OsModule.ViewModels;  
public interface IOperatingSystemViewModel {
  string Caption { get; set; }
  string BuildNumber { get; set; }
  string Version { get; set; }
  string Language { get; set; }
  string OSArchitecture { get; set; }

  string CodeSet { get; set; }
  string CSName { get; set; }
  string TimeZone { get; set; }
  string SerialNumber { get; set; }
  string Locale { get; set; }
}
