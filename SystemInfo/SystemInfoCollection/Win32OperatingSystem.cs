using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Management;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using static QueryConstants.Management.Win32OperatingSystem;
using static System.Net.WebRequestMethods;

namespace SystemInfoCollection;

public static class Win32OperatingSystem {
  public static List<(string key, string infoItem, string description)> Details = new();
  public static string[]? MUILanguages;
  public static int? NumberOfProcesses;

  [System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
  public static void Init() {
    try {
      ManagementObjectSearcher searcher = new(QueryString);
      using ManagementObjectCollection objCollection = searcher.Get();
      foreach (var mgtObj in objCollection) {
        Details.Add((BootDeviceKey, Convert.ToString(mgtObj[BootDeviceKey]), BootDeviceDesc));
        Details.Add((BuildNumberKey, Convert.ToString(mgtObj[BuildNumberKey]), BuildNumberDesc));
        Details.Add((BuildTypeKey, Convert.ToString(mgtObj[BuildTypeKey]), BuildTypeDesc));
        Details.Add((CaptionKey, Convert.ToString(mgtObj[CaptionKey]), CaptionDesc));
        Details.Add((CodeSetKey, Convert.ToString(mgtObj[CodeSetKey]), CodeSetDesc));
        Details.Add((CountryCodeKey, Convert.ToString(mgtObj[CountryCodeKey]), CountryCodeDesc));
        Details.Add((CreationClassNameKey, Convert.ToString(mgtObj[CreationClassNameKey]), CreationClassNameDesc));
        Details.Add((CSCreationClassNameKey, Convert.ToString(mgtObj[CSCreationClassNameKey]), CSCreationClassNameDesc));
        Details.Add((CSDVersionKey, Convert.ToString(mgtObj[CSDVersionKey]), CSDVersionDesc));
        Details.Add((CSNameKey, Convert.ToString(mgtObj[CSNameKey]), CSNameDesc));
        Details.Add((CurrentTimeZoneKey, Convert.ToString(mgtObj[CurrentTimeZoneKey]), CurrentTimeZoneDesc));
        Details.Add((DataExecutionPrevention_32BitApplicationsKey, Convert.ToString(mgtObj[DataExecutionPrevention_32BitApplicationsKey]), DataExecutionPrevention_32BitApplicationsDesc));
        Details.Add((DataExecutionPrevention_AvailableKey, Convert.ToString(mgtObj[DataExecutionPrevention_AvailableKey]), DataExecutionPrevention_AvailableDesc));
        Details.Add((DataExecutionPrevention_DriversKey, Convert.ToString(mgtObj[DataExecutionPrevention_DriversKey]), DataExecutionPrevention_DriversDesc));
        Details.Add((LastBootUpTimeKey, Convert.ToString(ConvertObjToDateTime(mgtObj[LastBootUpTimeKey])), LastBootUpTimeDesc));
        Details.Add((LocalDateTimeKey, Convert.ToString(ConvertObjToDateTime(mgtObj[LocalDateTimeKey])), LocalDateTimeDesc));
        Details.Add((LocaleKey, Convert.ToString(mgtObj[LocaleKey]), LocaleDesc));
        Details.Add((ManufacturerKey, Convert.ToString(mgtObj[ManufacturerKey]), ManufacturerDesc));
        Details.Add((MaxNumberOfProcessesKey, Convert.ToString(mgtObj[MaxNumberOfProcessesKey]), MaxNumberOfProcessesDesc));
        Details.Add((MaxProcessMemorySizeKey, Convert.ToString(mgtObj[MaxProcessMemorySizeKey]), MaxProcessMemorySizeDesc));
        Details.Add((NameKey, Convert.ToString(mgtObj[NameKey]), NameDesc));
        Details.Add((NumberOfLicensedUsersKey, GetNumbersOfLicensedUsers(Convert.ToInt32(mgtObj[NumberOfLicensedUsersKey])), NumberOfLicensedUsersDesc));
        Details.Add((VersionKey, Convert.ToString(mgtObj[VersionKey]), VersionDesc));
        Details.Add((NumberOfUsersKey, Convert.ToString(mgtObj[NumberOfUsersKey]), NumberOfUsersDesc));
        Details.Add((OperatingSystemSKUKey, GetOperatingSystemSKU(Convert.ToUInt32(mgtObj[OperatingSystemSKUKey])), OperatingSystemSKUDesc));
        Details.Add((OrganizationKey, Convert.ToString(mgtObj[OrganizationKey]), OrganizationDesc));
        Details.Add((OSArchitectureKey, Convert.ToString(mgtObj[OSArchitectureKey]), OSArchitectureDesc));
        Details.Add((OSLanguageKey, GetOSLanguage(Convert.ToUInt32(mgtObj[OSLanguageKey])), OSLanguageDesc));
        Details.Add((OSProductSuiteKey, GetOSProductSuite(Convert.ToUInt32(mgtObj[OSProductSuiteKey])), OSProductSuiteDesc));
        Details.Add((OSTypeKey, GetOSType(Convert.ToUInt16(mgtObj[OSTypeKey])), OSTypeDesc));
        Details.Add((OtherTypeDescriptionKey, Convert.ToString(mgtObj[OtherTypeDescriptionKey]), OtherTypeDescriptionDesc));
        Details.Add((PAEEnabledKey, Convert.ToString(Convert.ToBoolean(mgtObj[PAEEnabledKey])), PAEEnabledDesc));
        Details.Add((PortableOperatingSystemKey, Convert.ToString(Convert.ToBoolean(mgtObj[PortableOperatingSystemKey])), PortableOperatingSystemDesc));
        Details.Add((PrimaryKey, Convert.ToString(Convert.ToBoolean(mgtObj[PrimaryKey])), PrimaryDesc));
        Details.Add((ProductTypeKey, Convert.ToString(GetProductType(Convert.ToInt32(mgtObj[ProductTypeKey]))), ProductTypeDesc));

        Details.Add((RegisteredUserKey, Convert.ToString(mgtObj[RegisteredUserKey]), RegisteredUserDesc));
        Details.Add((SerialNumberKey, Convert.ToString(mgtObj[SerialNumberKey]), SerialNumberDesc));

        MUILanguages = (string[])mgtObj[MUILanguagesKey];
      }
    }
    catch (ManagementException e) {
      Console.WriteLine(e.Message);
    }

    PrintResult();
  }

  private static DateTime ConvertObjToDateTime(object obj) {
    if (obj == null) throw new ArgumentNullException();
    var str = Convert.ToString(obj);
    return ManagementDateTimeConverter.ToDateTime(str);
  }

  private static string GetNumbersOfLicensedUsers(int val) {
    return val switch
    {
      0 => "unlimited",
      -1 => "unknown",
      _ => val.ToString()
    };
  }

  private static string GetOperatingSystemSKU(uint val) {
    return val switch
    {
      0 => "Undefined(0)",
      1 => "Ultimate Edition",
      2 => "Home Basic Edition",
      3 => "Home Premium Edition",
      4 => "Enterprise Edition",
      6 => "Business Edition",
      7 => "Windows Server Standard Edition(Desktop Experience installation)",
      8 => "Windows Server Datacenter Edition(Desktop Experience installation)",
      9 => "Small Business Server Edition",
      10 => "Enterprise Server Edition",
      11 => "Starter Edition",
      12 => "Datacenter Server Core Edition",
      13 => "Standard Server Core Edition",
      14 => "Enterprise Server Core Edition",
      17 => "Web Server Edition",
      19 => "Home Server Edition",
      20 => "Storage Express Server Edition",
      21 => "Windows Storage Server Standard Edition(Desktop Experience installation)",
      22 => "Windows Storage Server Workgroup Edition(Desktop Experience installation)",
      23 => "Storage Enterprise Server Edition",
      24 => "Server For Small Business Edition",
      25 => "Small Business Server Premium Edition",
      27 => "Windows Enterprise Edition",
      28 => "Windows Ultimate Edition",
      29 => "Windows Server Web Server Edition(Server Core installation)",
      36 => "Windows Server Standard Edition without Hyper - V",
      37 => "Windows Server Datacenter Edition without Hyper - V(full installation)",
      38 => "Windows Server Enterprise Edition without Hyper - V(full installation)",
      39 => "Windows Server Datacenter Edition without Hyper - V(Server Core installation)",
      40 => "Windows Server Standard Edition without Hyper - V(Server Core installation)",
      41 => "Windows Server Enterprise Edition without Hyper - V(Server Core installation)",
      42 => "Microsoft Hyper - V Server",
      43 => "Storage Server Express Edition(Server Core installation)",
      44 => "Storage Server Standard Edition(Server Core installation)",
      45 => "Storage Server Workgroup Edition(Server Core installation)",
      46 => "Storage Server Enterprise Edition(Server Core installation)",
      48 => "Windows Professional",
      50 => "Windows Server Essentials(Desktop Experience installation)",
      63 => "Small Business Server Premium(Server Core installation)",
      64 => "Windows Compute Cluster Server without Hyper-V",
      97 => "Windows RT",
      101 => "Windows Home",
      103 => "Windows Professional with Media Center",
      104 => "Windows Mobile",
      123 => "Windows IoT(Internet of Things) Core",
      143 => "Windows Server Datacenter Edition(Nano Server installation)",
      144 => "Windows Server Standard Edition(Nano Server installation)",
      147 => "Windows Server Datacenter Edition(Server Core installation)",
      148 => "Windows Server Standard Edition(Server Core installation)",
      175 => "Windows Enterprise for Virtual Desktops (Azure Virtual Desktop)",
      407 => "Windows Server Datacenter: Azure Edition",
      _ => "Unknown"
    };
  }

  private static string GetOSLanguage(uint val) {
    return val switch
    {
      1 => "Arabic",
      4 => "Chinese(Simplified)– China",
      9 => "English",
      1025 => "Arabic – Saudi Arabia",
      1026 => "Bulgarian",
      1027 => "Catalan",
      1028 => "Chinese(Traditional) – Taiwan",
      1029 => "Czech",
      1030 => "Danish",
      1031 => "German – Germany",
      1032 => "Greek",
      1033 => "English – United States",
      1034 => "Spanish – Traditional Sort",
      1035 => "Finnish",
      1036 => "French – France",
      1037 => "Hebrew",
      1038 => "Hungarian",
      1039 => "Icelandic",
      1040 => "Italian – Italy",
      1041 => "Japanese",
      1042 => "Korean",
      1043 => "Dutch – Netherlands",
      1044 => "Norwegian – Bokmal",
      1045 => "Polish",
      1046 => "Portuguese – Brazil",
      1047 => "Rhaeto - Romanic",
      1048 => "Romanian",
      1049 => "Russian",
      1050 => "Croatian",
      1051 => "Slovak",
      1052 => "Albanian",
      1053 => "Swedish",
      1054 => "Thai",
      1055 => "Turkish",
      1056 => "Urdu",
      1057 => "Indonesian",
      1058 => "Ukrainian",
      1059 => "Belarusian",
      1060 => "Slovenian",
      1061 => "Estonian",
      1062 => "Latvian",
      1063 => "Lithuanian",
      1065 => "Persian",
      1066 => "Vietnamese",
      1069 => "Basque(Basque)",
      1070 => "Serbian",
      1071 => "Macedonian(North Macedonia)",
      1072 => "Sutu",
      1073 => "Tsonga",
      1074 => "Tswana",
      1076 => "Xhosa",
      1077 => "Zulu",
      1078 => "Afrikaans",
      1080 => "Faeroese",
      1081 => "Hindi",
      1082 => "Maltese",
      1084 => "Scottish Gaelic(United Kingdom)",
      1085 => "Yiddish",
      1086 => "Malay – Malaysia",
      2049 => "Arabic – Iraq",
      2052 => "Chinese(Simplified) – PRC",
      2055 => "German – Switzerland",
      2057 => "English – United Kingdom",
      2058 => "Spanish – Mexico",
      2060 => "French – Belgium",
      2064 => "Italian – Switzerland",
      2067 => "Dutch – Belgium",
      2068 => "Norwegian – Nynorsk",
      2070 => "Portuguese – Portugal",
      2072 => "Romanian – Moldova",
      2073 => "Russian – Moldova",
      2074 => "Serbian – Latin",
      2077 => "Swedish – Finland",
      3073 => "Arabic – Egypt",
      3076 => "Chinese(Traditional) – Hong Kong SAR",
      3079 => "German – Austria",
      3081 => "English – Australia",
      3082 => "Spanish – International Sort",
      3084 => "French – Canada",
      3098 => "Serbian – Cyrillic",
      4097 => "Arabic – Libya",
      4100 => "Chinese(Simplified) – Singapore",
      4103 => "German – Luxembourg",
      4105 => "English – Canada",
      4106 => "Spanish – Guatemala",
      4108 => "French – Switzerland",
      5121 => "Arabic – Algeria",
      5127 => "German – Liechtenstein",
      5129 => "English – New Zealand",
      5130 => "Spanish – Costa Rica",
      5132 => "French – Luxembourg",
      6145 => "Arabic – Morocco",
      6153 => "English – Ireland",
      6154 => "Spanish – Panama",
      7169 => "Arabic – Tunisia",
      7177 => "English – South Africa",
      7178 => "Spanish – Dominican Republic",
      8193 => "Arabic – Oman",
      8201 => "English – Jamaica",
      8202 => "Spanish – Venezuela",
      9217 => "Arabic – Yemen",
      9226 => "Spanish – Colombia",
      10241 => "Arabic – Syria",
      10249 => "English – Belize",
      10250 => "Spanish – Peru",
      11265 => "Arabic – Jordan",
      11273 => "English – Trinidad",
      11274 => "Spanish – Argentina",
      12289 => "Arabic – Lebanon",
      12298 => "Spanish – Ecuador",
      13313 => "Arabic – Kuwait",
      13322 => "Spanish – Chile",
      14337 => "Arabic – U.A.E.",
      14346 => "Spanish – Uruguay",
      15361 => "Arabic – Bahrain",
      15370 => "Spanish – Paraguay",
      16385 => "Arabic – Qatar",
      16394 => "Spanish – Bolivia",
      17418 => "Spanish – El Salvador",
      18442 => "Spanish – Honduras",
      19466 => "Spanish – Nicaragua",
      20490 => "Spanish – Puerto Rico",
      _ => "Unknown"
    };
  }

  private static string GetOSProductSuite(uint val) {
    return val switch
    {
      1 => "Microsoft Small Business Server installed",
      2 => "Windows Server 2008 Enterprise is installed",
      4 => "Windows BackOffice components are installed",
      8 => "Communication Server is installed",
      16 => "Terminal Services is installed",
      32 => "Microsoft Small Business Server is installed with the restrictive client license",
      64 => "Windows Embedded is installed",
      128 => "A Datacenter edition is installed",
      256 => "Terminal Services is installed, but only one interactive session is supported",
      512 => "Windows Home Edition is installed",
      1024 => "Web Server Edition is installed",
      8192 => "Storage Server Edition is installed",
      16384 => "Compute Cluster Edition is installed",
      _ => "Unknown"
    };
  }

  private static string GetOSType(ushort val) {
    return val switch
    {
      0 => "Unknown",
      1 => "Other",
      2 => "MACOS",
      3 => "ATTUNIX",
      4 => "DGUX",
      5 => "DECNT",
      6 => "Digital Unix",
      7 => "OpenVMS",
      8 => "HPUX",
      9 => "AIX",
      10 => "MVS",
      11 => "OS400",
      12 => "OS/2",
      13 => "JavaVM",
      14 => "MSDOS",
      15 => "WIN3x",
      16 => "WIN95",
      17 => "WIN98",
      18 => "WINNT",
      19 => "WINCE",
      20 => "NCR3000",
      21 => "NetWare",
      22 => "OSF",
      23 => "DC/OS",
      24 => "Reliant UNIX",
      25 => "SCO UnixWare",
      26 => "SCO OpenServer",
      27 => "Sequent",
      28 => "IRIX",
      29 => "Solaris",
      30 => "SunOS",
      31 => "U6000",
      32 => "ASERIES",
      33 => "TandemNSK",
      34 => "TandemNT",
      35 => "BS2000",
      36 => "LINUX",
      37 => "Lynx",
      38 => "XENIX",
      39 => "VM/ESA",
      40 => "Interactive UNIX",
      41 => "BSDUNIX",
      42 => "FreeBSD",
      43 => "NetBSD",
      44 => "GNU Hurd",
      45 => "OS9",
      46 => "MACH Kernel",
      47 => "Inferno",
      48 => "QNX",
      49 => "EPOC",
      50 => "IxWorks",
      51 => "VxWorks",
      52 => "MiNT",
      53 => "BeOS",
      54 => "HP MPE",
      55 => "NextStep",
      56 => "PalmPilot",
      57 => "Rhapsody",
      58 => "Windows 2000",
      59 => "Dedicated",
      60 => "OS/390",
      61 => "VSE",
      62 => "TPF",
      _ => "Unknown"
    };
  }

  private static string GetProductType(int val) {
    return val switch
    {
      1 => "Work Station",
      2 => "Domain Controller",
      3 => "Server",
      _ => "Unknown"
    };
  }

  static void PrintResult() {
    foreach (var obj in Details) {
      Console.WriteLine(obj.key + ":\t" + obj.infoItem);
    }
  }
}
