using System;
using System.Collections.Generic;
using System.Text;

namespace StorageModule.Definitions; 
internal static class MsiQueryDefinitions {

  public const string Cim2NamespaceName = @"root\cimv2";
  public const string MSFTNamespaceName = @"root\Microsoft\Windows\Storage";

  public const string QueryDialect = "WQL";
  public const string Logical_Drive_Query_Expression = "SELECT * FROM Win32_LogicalDisk";
  public const string Physical_Drive_Query_Expression = "SELECT * FROM Win32_DiskDrive";

  public const string DeviceIdKey = "DeviceID";
  public const string VolumeNameKey = "VolumeName";
  public const string FileSystemKey = "FileSystem";
  public const string SizeKey = "Size";
  public const string FreeSpaceKey = "FreeSpace";

  public const string NA = "N/A";
}
