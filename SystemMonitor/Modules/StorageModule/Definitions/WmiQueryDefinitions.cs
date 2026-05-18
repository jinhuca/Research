using System;
using System.Collections.Generic;
using System.Text;

namespace StorageModule.Definitions;  
internal static class WmiQueryDefinitions {
  public const string QueryScopeString = @"\\.\ROOT\Microsoft\Windows\Storage";
  public const string PhysicalDiskQueryString = "SELECT * FROM MSFT_PhysicalDisk";
  public const string DiskDriveQueryString = "SELECT * FROM Win32_DiskDrive";

  public const string PhysicalDiskId = "Physical Disk";

  public const string DiskNameKey = "FriendlyName";
  public const string DeviceIdKey = "DeviceId";
  public const string SerialNumberKey = "SerialNumber";
  public const string MediaTypeKey = "MediaType";
  public const string DriveTypeKey = "MediaType";
  public const string SizeKey = "size";
  public const string HealthStatusKey = "HealthStatus";
  public const string OperationalStatusKey = "OperationalStatus";

  public const string WmiExceptionHeader = "WMI query failed: ";
  public const string UnauthorizedAccessExceptionHeader = "Access denied. Try running as Administrator: ";
  public const string UnexpectedExceptionHeader = "Unexpected error: ";
}
