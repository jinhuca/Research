using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace MemoryModule.Models;

internal class QueryMemoryInfo {
  public QueryMemoryInfo() { }

  public static string GetRamFormFactor() {
    using var searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      var formVal = wmi_["FormFactor"];
      return GetMemoryFormFactorName(formVal);
    }
    return string.Empty;
  }

  public static string GetMemoryFormFactorName(object? formFactorValue) {
    if(formFactorValue == null) return "Unknown";

    Type valueType = formFactorValue.GetType();
    TypeConverter converter = TypeDescriptor.GetConverter(valueType);
    string formFactorString = converter.ConvertToString(formFactorValue) ?? string.Empty;
    return formFactorString switch {
      "0" => "Unknown",
      "1" => "Other",
      "2" => "SIP",
      "3" => "DIP",
      "4" => "ZIP",
      "5" => "SOJ",
      "6" => "Proprietary",
      "7" => "SIMM",
      "8" => "DIMM",
      "9" => "TSOP",
      "10" => "Row of chips",
      "11" => "RIMM",
      "12" => "SODIMM",
      "13" => "SRIMM",
      _ => $"Unknown Form Factor ({formFactorString})"
    };
  }

  public static void GetHardwareReservedRam() {
    ulong totalPhysicalMemory = 0;
    using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem")) {
      foreach(ManagementObject obj in searcher.Get()) {
        if(obj["TotalPhysicalMemory"] is ulong val) totalPhysicalMemory = val;
      }
    }

    ulong totalVisibleMemory = 0;
    using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem")) {
      foreach(ManagementObject obj in searcher.Get()) {
        if(obj["TotalVisibleMemorySize"] is ulong vis) totalVisibleMemory = vis * 1024;
      }
    }

    ulong reservedMemory = totalPhysicalMemory - totalVisibleMemory;

    Debug.WriteLine($"Total Physical Memory: {totalPhysicalMemory} Bytes");
    Debug.WriteLine($"Total Physical Memory: {totalPhysicalMemory / (1024 * 1024)} MB");
    Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit(totalPhysicalMemory));

    Debug.WriteLine($"Total Visible Memory: {totalVisibleMemory / (1024 * 1024)} MB");
    Debug.WriteLine($"Reserved Memory: {reservedMemory / (1024 * 1024)} MB");

    using ManagementObjectSearcher stickSearcher = new ManagementObjectSearcher("SELECT Capacity, Speed FROM Win32_PhysicalMemory");
    foreach(ManagementObject stick in stickSearcher.Get()) {
      if(stick["Capacity"] is ulong capacity) {
        uint speed = 0;
        if(stick["Speed"] is uint s) speed = s;
        Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit(capacity));
        Debug.WriteLine($"Stick: {capacity / (1024 * 1024)} MB | Speed: {speed} MHz");
      }
    }
  }

  private void GetSlotsUsed() {
    using var searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    int slotCount = 0;
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      slotCount++;
    }
    Debug.WriteLine("Slots Used: " + slotCount);
  }

  public string GetMemoryTypeString(object? memoryValue) {
    if(memoryValue == null) return string.Empty;
    TypeConverter converter = TypeDescriptor.GetConverter(memoryValue.GetType());
    if(converter != null && converter.CanConvertTo(typeof(string))) {
      return converter.ConvertToString(memoryValue) ?? string.Empty;
    }
    return memoryValue?.ToString() ?? string.Empty;
  }

  private void GetMemoryType() {
    using var searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      var memVal = wmi_["MemoryType"]?.ToString();
      if(int.TryParse(memVal, out int memoryTypeCode)) {
        var memoryType = GetMemoryTypeString(memoryTypeCode);
        Debug.WriteLine("Memory Type: " + memoryType);
      }
    }
  }
}