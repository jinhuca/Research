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


}
