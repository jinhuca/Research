using System;
using System.Collections.Generic;
using System.Text;

namespace QueryConstants.Management;

public static class Win32BIOS {
  public const string Win32BiosQuery = "SELECT * FROM Win32_BIOS";

  public const string BiosName = "Name";
  public const string BiosCaption = "Caption";
  public const string BiosVersion = "Version";
  public const string BiosLanguage = "LanguageEdition";
  public const string BiosReleaseDate = "ReleaseDate";
  public const string BiosSerialNumber = "SerialNumber";
  public const string BiosCurrentLanguage = "CurrentLanguage";
  /*
    uint16   BiosCharacteristics[];
  string   BIOSVersion[];
  string   BuildNumber;
  string   CodeSet;
  string   CurrentLanguage;
  string   Description;
  uint8    EmbeddedControllerMajorVersion;
  uint8    EmbeddedControllerMinorVersion;
  string   IdentificationCode;
  uint16   InstallableLanguages;
  datetime InstallDate;
  string   LanguageEdition;
  String   ListOfLanguages[];
  string   Manufacturer;
  string   Name;
  string   OtherTargetOS;
  boolean  PrimaryBIOS;
  datetime ReleaseDate;
  string   SerialNumber;
  string   SMBIOSBIOSVersion;
  uint16   SMBIOSMajorVersion;
  uint16   SMBIOSMinorVersion;
  boolean  SMBIOSPresent;
  string   SoftwareElementID;
  uint16   SoftwareElementState;
  string   Status;
  uint8    SystemBiosMajorVersion;
  uint8    SystemBiosMinorVersion;
  uint16   TargetOperatingSystem;
  string   Version;
   */
}
