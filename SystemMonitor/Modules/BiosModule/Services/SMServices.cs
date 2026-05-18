using BiosModule.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;
using static BiosModule.Constants.QueryKeys;

namespace BiosModule.Services;  
internal class SMServices {
  static ManagementObjectSearcher searcher_ = new(SMQueryString);

  public SMServices() { }

  public static string GetBiosVersion() {
    return "BIOS Version: 1.0.0";
  }

  public static SMSummary GetBiosSerialNumber() {
    SMSummary summaryInfo_ = new();
    try {
      var collection_ = searcher_.Get();
      foreach (ManagementObject wmi_ in collection_) {
        summaryInfo_.BiosVersion = wmi_[BiosVersion]?.ToString()?.Split(',') ?? Array.Empty<string>();
        summaryInfo_.BuildNumber = wmi_[BuildNumber]?.ToString() ?? string.Empty;

        summaryInfo_.Caption = wmi_[Caption]?.ToString() ?? string.Empty;
        summaryInfo_.CodeSet = wmi_[CodeSet]?.ToString() ?? string.Empty;
        summaryInfo_.CurrentLanguage = wmi_[CurrentLanguage]?.ToString() ?? string.Empty;

        summaryInfo_.Description = wmi_[Description]?.ToString() ?? string.Empty;

        summaryInfo_.EmbeddedControllerMajorVersion = ushort.TryParse(wmi_[EmbeddedControllerMajorVersion]?.ToString(), out var temp7) ? temp7 : (ushort)0;
        summaryInfo_.EmbeddedControllerMinorVersion = ushort.TryParse(wmi_[EmbeddedControllerMinorVersion]?.ToString(), out var temp8) ? temp8 : (ushort)0;
      
        summaryInfo_.IdentificationCode = wmi_[IdentificationCode]?.ToString() ?? string.Empty;
        summaryInfo_.InstallDate = DateTime.TryParse(wmi_[InstallDate]?.ToString(), out var temp10) ? temp10 : DateTime.MinValue;

        summaryInfo_.LanguageEdition = wmi_[LanguageEdition]?.ToString() ?? string.Empty;
      
        summaryInfo_.Manufacturer = wmi_[Manufacturer]?.ToString() ?? string.Empty;
        summaryInfo_.Name = wmi_[Name]?.ToString() ?? string.Empty;

        summaryInfo_.PrimaryBIOS = bool.TryParse(wmi_[PrimaryBIOS]?.ToString(), out var temp14) ? temp14 : false;
        summaryInfo_.ReleaseDate = DateTime.TryParse(wmi_[ReleaseDate]?.ToString(), out var temp15) ? temp15 : DateTime.MinValue;

        summaryInfo_.SerialNumber = wmi_[SerialNumber]?.ToString() ?? string.Empty;  
        summaryInfo_.SMBIOSBIOSVersion = wmi_[SMBIOSBIOSVersion]?.ToString() ?? string.Empty;
        summaryInfo_.SMBIOSMajorVersion = ushort.TryParse(wmi_[SMBIOSMajorVersion]?.ToString(), out var temp18) ? temp18 : (ushort)0;
        summaryInfo_.SMBIOSMinorVersion = ushort.TryParse(wmi_[SMBIOSMinorVersion]?.ToString(), out var temp19) ? temp19 : (ushort)0;

        summaryInfo_.SMBIOSPresent = bool.TryParse(wmi_[SMBIOSPresent]?.ToString(), out var temp20) ? temp20 : false;
        summaryInfo_.SoftwareElementID = wmi_[SoftwareElementID]?.ToString() ?? string.Empty;
        summaryInfo_.SoftwareElementState = ushort.TryParse(wmi_[SoftwareElementState]?.ToString(), out var temp22) ? temp22 : (ushort)0;
        summaryInfo_.Status = wmi_[Status]?.ToString() ?? string.Empty;
        summaryInfo_.SystemBiosMajorVersion = ushort.TryParse(wmi_[SystemBiosMajorVersion]?.ToString(), out var temp24) ? temp24 : (ushort)0;
        summaryInfo_.SystemBiosMinorVersion = ushort.TryParse(wmi_[SystemBiosMinorVersion]?.ToString(), out var temp25) ? temp25 : (ushort)0;

        summaryInfo_.TargetOperatingSystem = ushort.TryParse(wmi_[TargetOperatingSystem]?.ToString(), out var temp26) ? temp26 : (ushort)0;
        summaryInfo_.Version = wmi_[SMVersion]?.ToString() ?? string.Empty;
      }
    } catch (Exception ex) {
      Debug.WriteLine(ex.Message);
    }
    return summaryInfo_;
  }
}
