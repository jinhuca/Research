using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using StorageModule.Definitions;
using StorageModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;
using static StorageModule.Definitions.MsiQueryDefinitions;

namespace StorageModule.Services;  
internal static class MsiServices {
  public static void QueryMsi() {
    using CimSession session = CimSession.Create(null);
    var instances = session.QueryInstances(namespaceName: @"root\cimv2", "WQL", "SELECT * FROM Win32_OperatingSystem");
    foreach(var instance in instances) {
      Debug.WriteLine($"OS: {instance.CimInstanceProperties["Caption"].Value}");
    }
  }

  public static List<DiskInfo> QueryMsiLogicalDisks() {
    List<DiskInfo> result_ = new();
    try {
      // (1) Create a CIM session to the local machine
      using CimSession session = CimSession.Create(Environment.MachineName);

      // (2) Query for logical disk information
      var instances = session.QueryInstances(
          namespaceName: Cim2NamespaceName,
          queryDialect: QueryDialect,
          queryExpression: Logical_Drive_Query_Expression
      );

      foreach (var disk_ in instances) {
        var temp1 = disk_.CimInstanceProperties["Access"]?.Value?.ToString() ?? NA;
        var temp2 = disk_.CimInstanceProperties["Availability"]?.Value?.ToString() ?? NA;
        var temp3 = disk_.CimInstanceProperties["Compressed"]?.Value?.ToString() ?? NA;
        var temp4 = disk_.CimInstanceProperties["DriveType"]?.Value?.ToString() ?? NA;
        var temp5 = disk_.CimInstanceProperties["Name"]?.Value?.ToString() ?? NA;
        var temp6 = disk_.CimInstanceProperties["VolumeName"]?.Value?.ToString() ?? NA;
        var temp7 = disk_.CimInstanceProperties["VolumeSerialNumber"]?.Value?.ToString() ?? NA;

        DiskInfo info_ = new DiskInfo {
          DeviceId = disk_.CimInstanceProperties[DeviceIdKey]?.Value?.ToString() ?? NA,
          VolumeName = disk_.CimInstanceProperties[VolumeNameKey]?.Value?.ToString() ?? NA,
          FileSystem = disk_.CimInstanceProperties[FileSystemKey]?.Value?.ToString() ?? NA,
          Capacity = disk_.CimInstanceProperties[SizeKey] != null
            ? Convert.ToUInt64(disk_.CimInstanceProperties[SizeKey].Value) : 0,
          FormattedCapacity = disk_.CimInstanceProperties[FreeSpaceKey] != null
            ? Convert.ToUInt64(disk_.CimInstanceProperties[FreeSpaceKey].Value) : 0
        };

        result_.Add(info_);
      }
    }
    catch (ManagementException mex) {
      Debug.WriteLine(Messages.WmiQueryFailed + mex.Message);
      result_.Clear(); // Clear any partial results
    }
    catch (UnauthorizedAccessException uae) {
      Debug.WriteLine(Messages.AccessDenied + uae.Message);
      result_.Clear();
    }
    catch (Exception ex) {
      Debug.WriteLine(Messages.UnexpectedError + ex.Message);
      result_ = new List<DiskInfo>();
    }
    return result_;
  }
}
