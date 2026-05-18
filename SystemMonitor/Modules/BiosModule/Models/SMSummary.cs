using System;
using System.Collections.Generic;
using System.Text;

namespace BiosModule.Models;  
public class SMSummary {
  public string[] BiosVersion { get; set; } = Array.Empty<string>();
  public string BuildNumber { get; set; } = string.Empty;

  public string Caption { get; set; } = string.Empty;
  public string CodeSet { get; set; } = string.Empty;
  public string CurrentLanguage { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public ushort EmbeddedControllerMajorVersion { get; set; } = 0;
  public ushort EmbeddedControllerMinorVersion { get; set; } = 0;

  public string IdentificationCode { get; set; } = string.Empty;
  public DateTime InstallDate { get; set; } = DateTime.MinValue;

  public string LanguageEdition { get; set; } = string.Empty;

  public string Manufacturer { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public bool PrimaryBIOS { get; set; } = false;

  public DateTime ReleaseDate { get; set; } = DateTime.MinValue;

  public string SerialNumber { get; set; } = string.Empty;
  public string SMBIOSBIOSVersion { get; set; } = string.Empty;
  public ushort SMBIOSMajorVersion { get; set; } = 0;
  public ushort SMBIOSMinorVersion { get; set; } = 0;
  public bool SMBIOSPresent { get; set; } = false;
  public string SoftwareElementID { get; set; } = string.Empty;
  public ushort SoftwareElementState { get; set; } = 0;
  public string Status { get; set; } = string.Empty;
  public ushort SystemBiosMajorVersion { get; set; } = 0;
  public ushort SystemBiosMinorVersion { get; set; } = 0;  
  public ushort TargetOperatingSystem { get; set; } = 0;

  public string Version { get; set; } = string.Empty;

}
