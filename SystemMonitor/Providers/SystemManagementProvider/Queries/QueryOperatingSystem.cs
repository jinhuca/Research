using System.Management;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;
using static SystemManagementProvider.Constants.Win32_OperatingSystem;

namespace SystemManagementProvider.Queries; 
public class QueryOperatingSystem : ISMQuery {
  private ManagementObjectSearcher _searcher;
  private static Dictionary<string, (string, string)> info = [];
  public static string[]? MUILanguages;
  public static int? NumberOfProcesses;

  public QueryOperatingSystem() {
    _searcher = new ManagementObjectSearcher(QueryString);
    if(_searcher is null) 
      throw new ArgumentNullException(nameof(_searcher));
  }

  public Dictionary<string,(string,string)> GetInfo() {
    info.Clear();
    try {
      using ManagementObjectCollection objCollection_ = _searcher.Get();
      foreach (var obj in objCollection_.Cast<ManagementObject>()) {
        info.Add(BootDeviceKey, (Convert.ToString(obj[BootDeviceKey]), BootDeviceDesc));
        info.Add(BuildNumberKey, (Convert.ToString(obj[BuildNumberKey]), BuildNumberDesc));
        info.Add(BuildTypeKey, (Convert.ToString(obj[BuildTypeKey]), BuildTypeDesc));
        info.Add(CaptionKey, (Convert.ToString(obj[CaptionKey]), CaptionDesc));
        info.Add(CodeSetKey, (Convert.ToString(obj[CodeSetKey]), CodeSetDesc));
        info.Add(CountryCodeKey, (Convert.ToString(obj[CountryCodeKey]), CountryCodeDesc));
        info.Add(CreationClassNameKey, (Convert.ToString(obj[CreationClassNameKey]), CreationClassNameDesc));
        info.Add(CSCreationClassNameKey, (Convert.ToString(obj[CSCreationClassNameKey]), CSCreationClassNameDesc));
        info.Add(CSDVersionKey, (Convert.ToString(obj[CSDVersionKey]), CSDVersionDesc));
        info.Add(CSNameKey, (Convert.ToString(obj[CSNameKey]), CSNameDesc));
        info.Add(CurrentTimeZoneKey, (Convert.ToString(obj[CurrentTimeZoneKey]), CurrentTimeZoneDesc));
        info.Add(DataExecutionPrevention_32BitApplicationsKey, (Convert.ToString(obj[DataExecutionPrevention_32BitApplicationsKey]), DataExecutionPrevention_32BitApplicationsDesc));
        info.Add(DataExecutionPrevention_AvailableKey, (Convert.ToString(obj[DataExecutionPrevention_AvailableKey]), DataExecutionPrevention_AvailableDesc));
        info.Add(DataExecutionPrevention_DriversKey, (Convert.ToString(obj[DataExecutionPrevention_DriversKey]), DataExecutionPrevention_DriversDesc));
        info.Add(LastBootUpTimeKey, (Convert.ToString(ConvertObjToDateTime(obj[LastBootUpTimeKey])), LastBootUpTimeDesc));
        info.Add(LocalDateTimeKey, (Convert.ToString(ConvertObjToDateTime(obj[LocalDateTimeKey])), LocalDateTimeDesc));
        info.Add(LocaleKey, (Convert.ToString(obj[LocaleKey]), LocaleDesc));
        info.Add(ManufacturerKey, (Convert.ToString(obj[ManufacturerKey]), ManufacturerDesc));
        info.Add(MaxNumberOfProcessesKey, (Convert.ToString(obj[MaxNumberOfProcessesKey]), MaxNumberOfProcessesDesc));
        info.Add(MaxProcessMemorySizeKey, (Convert.ToString(obj[MaxProcessMemorySizeKey]), MaxProcessMemorySizeDesc));
        info.Add(NameKey, (Convert.ToString(obj[NameKey]), NameDesc));
        info.Add(NumberOfLicensedUsersKey, (GetNumbersOfLicensedUsers(Convert.ToInt32(obj[NumberOfLicensedUsersKey])), NumberOfLicensedUsersDesc));
        info.Add(VersionKey, (Convert.ToString(obj[VersionKey]), VersionDesc));
        info.Add(NumberOfUsersKey, (Convert.ToString(obj[NumberOfUsersKey]), NumberOfUsersDesc));
        info.Add(OperatingSystemSKUKey, (GetOperatingSystemSKU(Convert.ToUInt32(obj[OperatingSystemSKUKey])), OperatingSystemSKUDesc));
        info.Add(OrganizationKey, (Convert.ToString(obj[OrganizationKey]), OrganizationDesc));
        info.Add(OSArchitectureKey, (Convert.ToString(obj[OSArchitectureKey]), OSArchitectureDesc));
        info.Add(OSLanguageKey, (GetOSLanguage(Convert.ToUInt32(obj[OSLanguageKey])), OSLanguageDesc));
        info.Add(OSProductSuiteKey, (GetOSProductSuite(Convert.ToUInt32(obj[OSProductSuiteKey])), OSProductSuiteDesc));
        info.Add(OSTypeKey, (GetOSType(Convert.ToUInt16(obj[OSTypeKey])), OSTypeDesc));
        info.Add(OtherTypeDescriptionKey, (Convert.ToString(obj[OtherTypeDescriptionKey]), OtherTypeDescriptionDesc));
        info.Add(PAEEnabledKey, (Convert.ToString(Convert.ToBoolean(obj[PAEEnabledKey])), PAEEnabledDesc));
        info.Add(PortableOperatingSystemKey, (Convert.ToString(Convert.ToBoolean(obj[PortableOperatingSystemKey])), PortableOperatingSystemDesc));
        info.Add(PrimaryKey, (Convert.ToString(Convert.ToBoolean(obj[PrimaryKey])), PrimaryDesc));
        info.Add(ProductTypeKey, (Convert.ToString(GetProductType(Convert.ToInt32(obj[ProductTypeKey]))), ProductTypeDesc));
        info.Add(RegisteredUserKey, (Convert.ToString(obj[RegisteredUserKey]), RegisteredUserDesc));
        info.Add(SerialNumberKey, (Convert.ToString(obj[SerialNumberKey]), SerialNumberDesc));

        MUILanguages = (string[])obj[MUILanguagesKey];
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
    return info;
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
    //foreach (var obj in info) {
    //  Console.WriteLine(obj.key + ":\t" + obj.infoItem);
    //}
  }

  public string Query(string query) {
    throw new NotImplementedException();
  }

  Dictionary<string, (string, string)> ISMQuery.Query(string query) {
    info.Clear();
    info = GetInfo();
    return info;
  }
}
