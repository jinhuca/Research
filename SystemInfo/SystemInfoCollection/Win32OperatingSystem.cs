using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using static QueryConstants.Management.Win32OperatingSystem;

namespace SystemInfoCollection;

public static class Win32OperatingSystem {
  public static List<(string key, string infoItem, string description)> Details = new();

  [System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
  public static void Init() {
    try {
      ManagementObjectSearcher searcher = new(QueryString);
      foreach (var mgtObj in searcher.Get()) {
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

        Details.Add((LocaleKey, Convert.ToString(mgtObj[LocaleKey]), LocaleDesc));

        Details.Add((VersionKey, Convert.ToString(mgtObj[VersionKey]), VersionDesc));
      }
    }
    catch (ManagementException e) {
      Console.WriteLine(e.Message);
    }

    PrintResult();
  }
  static void PrintResult() {
    foreach (var obj in Details) {
      Console.WriteLine(obj.ToString());
    }
  }
}
