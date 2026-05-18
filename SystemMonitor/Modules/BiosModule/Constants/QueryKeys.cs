using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BiosModule.Constants; 
internal static class QueryKeys {
  public const string SMQueryString = "SELECT * FROM Win32_BIOS";

  public const string BiosVersion= "Version";
  public const string BuildNumber = "BuildNumber";

  public const string Caption = "Caption";
  public const string CodeSet = "CodeSet";
  public const string CurrentLanguage = "CurrentLanguage";

  public const string Description = "Description";

  public const string EmbeddedControllerMajorVersion = "EmbeddedControllerMajorVersion";
  public const string EmbeddedControllerMinorVersion = "EmbeddedControllerMinorVersion";

  public const string IdentificationCode = "IdentificationCode";
  public const string InstallDate = "InstallDate";

  public const string LanguageEdition = "LanguageEdition";

  public const string Manufacturer = "Manufacturer";

  public const string Name = "Name";

  public const string PrimaryBIOS = "PrimaryBIOS";

  public const string ReleaseDate = "ReleaseDate";

  public const string SerialNumber = "SerialNumber";
  public const string SMBIOSBIOSVersion = "SMBIOSBIOSVersion";
  public const string SMBIOSMajorVersion = "SMBIOSMajorVersion";
  public const string SMBIOSMinorVersion = "SMBIOSMinorVersion";
  public const string SMBIOSPresent = "SMBIOSPresent";
  public const string SoftwareElementID = "SoftwareElementID";
  public const string SoftwareElementState = "SoftwareElementState";
  public const string Status = "Status";
  public const string SystemBiosMajorVersion = "SystemBiosMajorVersion";
  public const string SystemBiosMinorVersion = "SystemBiosMinorVersion";

  public const string TargetOperatingSystem = "TargetOperatingSystem";

  public const string SMVersion = "Version";
}
