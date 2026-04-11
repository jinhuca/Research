namespace QueryConstants.Management; 

public static class Win32Processor {
  public const string Query_String = "SELECT * FROM Win32_Processor";

  public const string AddressWidthKey = "AddressWidth";
  public const string AddressWidthDesc = "Processor Address Width";

  public const string ArchitectureKey = "Architecture";
  public const string ArchitectureDesc = "Processor Architecture";

  public const string AssetTagKey = "AssetTag";
  public const string AssetTagDesc = "Asset Tag Identifier";

  public const string AvailabilityKey = "Availability";
  public const string AvailabilityDesc = "";

  public const string CaptionKey = "Caption";
  public const string CaptionDesc = "Prcessor Caption";

  public const string CharacteristicsKey = "Characteristics";
  public const string CharacteristicsDesc = "Processor Functionality";

  public const string ConfigManagerErrorCodeKey = "ConfigManagerErrorCode";
  public const string ConfigManagerErrorCodeDesc = "Configuration Manager Error Code";

  public const string ConfigManagerUserConfigKey = "ConfigManagerUserConfig";
  public const string ConfigManagerUserConfigDesc = "Processor Configured (True/False)";

  public const string CpuStatusKey = "CpuStatus";
  public const string CpuStatusDesc = "Current status of the processor";

  public const string CreationClassNameKey = "CreationClassName";
  public const string CreationClassNameDesc = "Creation Class Name";

  public const string CurrentClockSpeedKey = "CurrentClockSpeed";
  public const string CurrentClockSpeedDesc = "Processor Current Clock Speed";

  public const string CurrentVoltageKey = "CurrentVoltage";
  public const string CurrentVoltageDesc = "Voltage of the processor";

  public const string DataWidthKey = "DataWidth";
  public const string DataWidthDesc = "Processor Data Width";

  public const string DescriptionKey = "Description";
  public const string DescriptionDesc = "Processor Description";

  public const string DeviceIDKey = "DeviceID";
  public const string DeviceIDDesc = "Unique identifier of a processor on the system";

  public const string ErrorClearedKey = "ErrorCleared";
  public const string ErrorClearedDesc = "If True, the error reported in LastErrorCode is clear";

  public const string ErrorDescriptionKey = "ErrorDescription";
  public const string ErrorDescriptionDesc = "Information about the error recorded in LastErrorCode";

  public const string ExtClockKey = "ExtClock";
  public const string ExtClockDesc = "External clock frequency in MHz";

  public const string FamilyKey = "Family";
  public const string FamilyDesc = "Processor Family";

  public const string InstallDateKey = "InstallDate";
  public const string InstallDateDesc = "Date and time the object is installed";

  public const string L2CacheSizeKey = "L2CacheSize";
  public const string L2CacheSizeDesc = "Size of the Level 2 processor cache (KB)";

  public const string L2CacheSpeedKey = "L2CacheSpeed";
  public const string L2CacheSpeedDesc = "Clock speed of the Level 2 processor cache (MHz)";

  public const string L3CacheSizeKey = "L3CacheSize";
  public const string L3CacheSizeDesc = "Size of the Level 3 processor cache (KB)";

  public const string L3CacheSpeedKey = "L3CacheSpeed";
  public const string L3CacheSpeedDesc = "Clock speed of the Level 3 processor cache (MHz)";

  public const string LastErrorCodeKey = "LastErrorCode";
  public const string LastErrorCodeDesc = "Last error code reported by processor.";


  public const string NameKey = "Name";
  public const string NameDesc = "Processor Name";
  


  public const string IdKey = "ProcessorId";
  public const string IdDesc = "Processor ID";



  public const string MaxClockSpeedKey = "MaxClockSpeed";
  public const string MaxClockSpeedDesc = "Max Clock Speed";
  public const string SocketKey = "SocketDesignation";
  
  public const string SpeedUnit = "MHz";
  public const string CacheSizeUnit = "KB";

  public const string PhysicalCoreNumberKey = "NumberOfCores";
  public const string LogicalProcessorNumberKey = "NumberOfLogicalProcessors";
  public const string UniqueIdKey = "UniqueId";
  public const string SteppingKey = "Stepping";
  public const string SystemNameKey = "SystemName";
  

  

  /*
   [Dynamic, Provider("CIMWin32"), UUID("{8502C4BB-5FBB-11D2-AAC1-006008C78BC7}"), AMENDMENT]
class Win32_Processor : CIM_Processor
{
  boolean  ErrorCleared;
  string   ErrorDescription;
  uint32   ExtClock;
  uint16   Family;
  datetime InstallDate;
  uint32   L2CacheSize;
  uint32   L2CacheSpeed;
  uint32   L3CacheSize;
  uint32   L3CacheSpeed;
  uint32   LastErrorCode;
  uint16   Level;
  uint16   LoadPercentage;
  string   Manufacturer;
  uint32   MaxClockSpeed;
  string   Name;
  uint32   NumberOfCores;
  uint32   NumberOfEnabledCore;
  uint32   NumberOfLogicalProcessors;
  string   OtherFamilyDescription;
  string   PartNumber;
  string   PNPDeviceID;
  uint16   PowerManagementCapabilities[];
  boolean  PowerManagementSupported;
  string   ProcessorId;
  uint16   ProcessorType;
  uint16   Revision;
  string   Role;
  boolean  SecondLevelAddressTranslationExtensions;
  string   SerialNumber;
  string   SocketDesignation;
  string   Status;
  uint16   StatusInfo;
  string   Stepping;
  string   SystemCreationClassName;
  string   SystemName;
  uint32   ThreadCount;
  string   UniqueId;
  uint16   UpgradeMethod;
  string   Version;
  boolean  VirtualizationFirmwareEnabled;
  boolean  VMMonitorModeExtensions;
  uint32   VoltageCaps;
};
   */
}

