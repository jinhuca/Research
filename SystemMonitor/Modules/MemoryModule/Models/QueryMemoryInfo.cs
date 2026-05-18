using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace MemoryModule.Models;

internal class QueryMemoryInfo {
  public QueryMemoryInfo() { }

  public static string GetRamFormFactor() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach (ManagementObject wmi_ in searcher_.Get()) {
      return GetMemoryFormFactorName(wmi_["FormFactor"]);
    }
    return string.Empty;
  }

  public static string GetMemoryFormFactorName(object formFactorValue) {
    // 1. Get the converter for the value's actual type
    Type valueType = formFactorValue.GetType();
    TypeConverter converter = TypeDescriptor.GetConverter(valueType);
    // 2. Convert the value to its string representation
    string formFactorString = converter.ConvertToString(formFactorValue);
    // 3. Map the string representation to a human-readable name
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
    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem")) {
      foreach (ManagementObject obj in searcher.Get()) {
        totalPhysicalMemory = (ulong)obj["TotalPhysicalMemory"];
      }
    }

    ulong totalVisibleMemory = 0;
    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem")) {
      foreach (ManagementObject obj in searcher.Get()) {
        totalVisibleMemory = (ulong)obj["TotalVisibleMemorySize"] * 1024; // Convert KB to Bytes
      }
    }

    ulong reservedMemory = totalPhysicalMemory - totalVisibleMemory;

    Debug.WriteLine($"Total Physical Memory: {totalPhysicalMemory} Bytes");
    Debug.WriteLine($"Total Physical Memory: {totalPhysicalMemory / (1024 * 1024)} MB");
    Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit(totalPhysicalMemory));

    Debug.WriteLine($"Total Visible Memory: {totalVisibleMemory / (1024 * 1024)} MB");
    Debug.WriteLine($"Reserved Memory: {reservedMemory / (1024 * 1024)} MB");

    ManagementObjectSearcher stickSearcher = new ManagementObjectSearcher("SELECT Capacity, Speed FROM Win32_PhysicalMemory");

    foreach (ManagementObject stick in stickSearcher.Get()) {
      ulong capacity = (ulong)stick["Capacity"];
      uint speed = (uint)stick["Speed"];
      Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit(capacity));
      Debug.WriteLine($"Stick: {capacity / (1024 * 1024)} MB | Speed: {speed} MHz");
    }

  }


  //private void GetAvailableMemory() {
  //  PerformanceCounter availableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
  //  float availableMemoryMB = availableMemoryCounter.NextValue();
  //  string availableMemoryReadable = ByteUnitConverters.ConvertBytesToReadableUnit((long)(availableMemoryMB * 1024 * 1024));
  //  //float availableMemoryGB = ByteUnitConverters.ConvertMBToReadableUnit((long)availableMemoryMB);
  //  Debug.WriteLine("Available Memory: " + availableMemoryReadable);
  //}

  //private void GetTotalMemory() {
  //  PerformanceCounter totalMemoryCounter = new PerformanceCounter("Memory", "Committed Bytes");
  //  float totalMemoryBytes = totalMemoryCounter.NextValue();
  //  string totalMemoryReadable = ByteUnitConverters.ConvertBytesToReadableUnit((long)totalMemoryBytes);
  //  Debug.WriteLine("Total Memory: " + totalMemoryReadable);
  //}

  private void GetSlotsUsed() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    int slotCount = 0;
    foreach (ManagementObject wmi_ in searcher_.Get()) {
      slotCount++;
    }
    Debug.WriteLine("Slots Used: " + slotCount);
  }

  public string GetMemoryTypeString(object memoryValue) {
    // 1. Get the converter for the value's actual type
    TypeConverter converter = TypeDescriptor.GetConverter(memoryValue.GetType());

    // 2. Check if it can convert to a string
    if (converter != null && converter.CanConvertTo(typeof(string))) {
      // 3. Return the converted string
      return converter.ConvertToString(memoryValue);
    }

    // Fallback: Default to standard ToString()
    return memoryValue?.ToString() ?? string.Empty;
  }

  private void GetMemoryType() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach (ManagementObject wmi_ in searcher_.Get()) {
      var memoryTypeCode = int.Parse(wmi_["MemoryType"].ToString());
      var memoryType = GetMemoryTypeString(memoryTypeCode);
      Debug.WriteLine("Memory Type: " + memoryType);
    }
  }

  //private void Test() {
  //  Debug.WriteLine("Ram Form Factor = " + QueryMemoryInfo.GetRamFormFactor());
  //  QueryMemoryInfo.GetHardwareReservedRam();

  //  ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
  //  foreach (ManagementObject wmi_ in searcher_.Get()) {
  //    var cp1_ = long.Parse(wmi_["Capacity"].ToString());
  //    var cp_ = ByteUnitConverters.ConvertBytesToReadableUnit(cp1_);
  //    Debug.WriteLine("Capacity: " + cp_);

  //    var speedInMHz = double.Parse(wmi_["Speed"].ToString());
  //    Debug.WriteLine("Raw Speed: " + wmi_["Speed"] + " MT/s");
  //    //var speedInGHz = HzUnitConverter.ConvertMHzToReadableUnit(speedInMHz);
  //    //Debug.WriteLine("Speed: " + speedInGHz + " MHz");
  //  }
  //}
}
