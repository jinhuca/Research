using System;
using System.Collections.Generic;
using System.Text;

namespace QueryConstants.Management;

public static class Win32OperatingSystem {
  public const string QueryString = "SELECT * FROM Win32_OperatingSystem";

  public const string BootDeviceKey = "BootDevice";
  public const string BootDeviceDesc = "Name of the disk drive from which the Windows operating system starts";

  public const string BuildNumberKey = "BuildNumber";
  public const string BuildNumberDesc = "Build number of an operating system";

  public const string BuildTypeKey = "BuildType";
  public const string BuildTypeDesc = "Type of build used for an operating system";

  public const string CaptionKey = "Caption";
  public const string CaptionDesc = "Short description of OS";

  public const string CodeSetKey = "CodeSet";
  public const string CodeSetDesc = "Code page value an operating system uses";

  public const string CountryCodeKey = "CountryCode";
  public const string CountryCodeDesc = "Code for the country/region that an operating system uses";

  public const string CreationClassNameKey = "CreationClassName";
  public const string CreationClassNameDesc = "Name of the first concrete class that appears in the inheritance chain used in the creation of an instance";

  public const string CSCreationClassNameKey = "CSCreationClassName";
  public const string CSCreationClassNameDesc = "Creation class name of the scoping computer system";

  public const string CSDVersionKey = "CSDVersion";
  public const string CSDVersionDesc = "Latest service pack installed";

  public const string CSNameKey = "CSName";
  public const string CSNameDesc = "Name of the scoping computer system";

  public const string VersionKey = "Version";
  public const string VersionDesc = "Version number of the operating system";

  public const string CurrentTimeZoneKey = "CurrentTimeZone";
  public const string CurrentTimeZoneDesc = "Number, in minutes, an operating system is offset from Greenwich mean time (GMT)";

  public const string DataExecutionPrevention_32BitApplicationsKey = "DataExecutionPrevention_32BitApplications";
  public const string DataExecutionPrevention_32BitApplicationsDesc = "Availability of data execution prevention hardware feature";

  public const string DataExecutionPrevention_AvailableKey = "DataExecutionPrevention_Available";
  public const string DataExecutionPrevention_AvailableDesc = "Availability of of data execution prevention";

  public const string DataExecutionPrevention_DriversKey = "DataExecutionPrevention_Drivers";
  public const string DataExecutionPrevention_DriversDesc = "When the data execution prevention hardware feature is available, this property indicates that the feature is set to work for drivers if True";

  public const string LocaleKey = "Locale";
  public const string LocaleDesc = "Language identifier used by the operating system";
}
