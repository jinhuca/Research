namespace QueryConstants.Management; 

public static class CPU {
  public const string Query_String = "SELECT * FROM Win32_Processor";
  public const string Name = "Name";
  public const string Family = "Family";
  public const string Id = "ProcessorId";
  public const string Description = "Description";
  public const string Architecture = "Architecture";
  public const string Socket = "SocketDesignation";
  public const string MaxClockSpeed = "MaxClockSpeed";
  public const string SpeedUnit = "MHz";
  public const string PhysicalCoreNumber = "NumberOfCores";
  public const string LogicalProcessorNumber = "NumberOfLogicalProcessors";
  public const string UniqueId = "UniqueId";
  public const string Stepping = "Stepping";
  public const string SystemName = "SystemName";
  public const string DeviceID = "DeviceID";
  public const string Caption = "Caption";
  public const string CurrentClockSpeed = "CurrentClockSpeed";

  public static (string Key, string Value) name_result = (Name, string.Empty);
  public static (string Key, string Value) id_result = (Id, string.Empty);

}

public static class GPU { }

