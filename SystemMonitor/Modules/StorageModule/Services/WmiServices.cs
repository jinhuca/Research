using StorageModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;
using LogModule;
using StorageModule.Definitions;
using static StorageModule.Definitions.WmiQueryDefinitions;

namespace StorageModule.Services;

internal static class WmiServices {
  public static List<DiskInfo> GetDiskinfo() {
    List<DiskInfo> result_ = new();
    try {
      LogModule.TestDebug.WriteOut("Call LogModule method.");
      // (1) Connect to the storage namespace
      var scope = new ManagementScope(QueryScopeString);
      scope.Connect();

      // (2) Query MSFT_PhysicalDisk
      var query = new ObjectQuery(PhysicalDiskQueryString);
      using(var searcher = new ManagementObjectSearcher(scope, query)) {
        foreach(ManagementObject disk in searcher.Get()) {
          DiskInfo diskInfo = new();
          diskInfo.Name = disk[DiskNameKey] != null ? disk[DiskNameKey].ToString() : string.Empty;
          //diskInfo.DeviceId = disk[DeviceIdKey] != null ? int.Parse(disk[DeviceIdKey].ToString()) : int.MinValue;
          diskInfo.MediaType = disk[MediaTypeKey] != null ? disk[MediaTypeKey].ToString() : string.Empty;
          diskInfo.Capacity = disk[SizeKey] != null ? ulong.Parse(disk[SizeKey].ToString()) : 0;
          diskInfo.SerialNum = disk[SerialNumberKey] != null ? disk[SerialNumberKey].ToString() : string.Empty;

          Debug.WriteLine("=== Physical Disk ===");
          Debug.WriteLine($"FriendlyName : {disk["FriendlyName"]}");
          Debug.WriteLine($"DeviceId     : {disk["DeviceId"]}");
          Debug.WriteLine($"SerialNumber : {disk["SerialNumber"]}");
          Debug.WriteLine($"MediaType    : {disk["MediaType"]}"); // 3 = HDD, 4 = SSD
          Debug.WriteLine($"Size (bytes) : {disk["Size"]}");
          Debug.WriteLine($"HealthStatus : {disk["HealthStatus"]}"); // 0 = Healthy
          Debug.WriteLine($"OperationalStatus : {disk["OperationalStatus"]}");

          result_.Add(diskInfo);
        }
      }
    }
    catch(ManagementException mex) {
      Debug.WriteLine("WMI query failed: " + mex.Message);
    }
    catch(UnauthorizedAccessException uae) {
      Debug.WriteLine("Access denied. Try running as Administrator: " + uae.Message);
    }
    catch(Exception ex) {

    }

    return result_;
  }
}
