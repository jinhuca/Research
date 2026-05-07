using Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Text;
using static MemoryModule.Definitions.Constants;

namespace MemoryModule.Models;

internal class QueryMemory {
  public QueryMemory() { }

  public static string GetRamFormFactor() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher(Win32PhysicalQueryString);
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      return GetMemoryFormFactorName(wmi_[FormFactorQueryKey]);
    }
    return string.Empty;
  }

  public static string GetMemoryFormFactorName(object formFactorValue) {
    // 1. Get the converter for the value's actual type
    Type valueType = formFactorValue.GetType();
    TypeConverter converter = TypeDescriptor.GetConverter(valueType);
    // 2. Convert the value to its string representation
    string formFactorString = converter.ConvertToString(formFactorValue) ?? string.Empty;
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

  public static ulong GetOSVisibleRAMSize() {
    ulong totalVisibleMemory_ = 0;
    using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(Win32OSQueryString)) {
      foreach(ManagementObject obj in searcher.Get()) {
        totalVisibleMemory_ = (ulong)obj[TotalVisibleMemorySizeQueryKey] * 1024; // Convert KB to Bytes
      }
    }
    return totalVisibleMemory_;
  }

  public static void GetHardwareReservedRam() {
    ulong totalPhysicalMemory = 0;
    using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem")) {
      foreach(ManagementObject obj in searcher.Get()) {
        totalPhysicalMemory = (ulong)obj["TotalPhysicalMemory"];
      }
    }

    ulong totalVisibleMemory = 0;
    using(ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem")) {
      foreach(ManagementObject obj in searcher.Get()) {
        totalVisibleMemory = (ulong)obj["TotalVisibleMemorySize"] * 1024; // Convert KB to Bytes
      }
    }

    ulong reservedMemory = totalPhysicalMemory - totalVisibleMemory;

    Debug.WriteLine($"Total Physical Memory: {totalPhysicalMemory} Bytes");
    Debug.WriteLine($"++ Total Physical Memory: {totalVisibleMemory} Bytes");
    Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)totalPhysicalMemory));  // OS available

    Debug.WriteLine($"Total Visible Memory: {totalVisibleMemory / (1024 * 1024)} MB");
    Debug.WriteLine($"Reserved Memory: {reservedMemory / (1024 * 1024)} MB");

    ManagementObjectSearcher stickSearcher = new ManagementObjectSearcher("SELECT Capacity, Speed FROM Win32_PhysicalMemory");

    foreach(ManagementObject stick in stickSearcher.Get()) {
      ulong capacity = (ulong)stick["Capacity"];
      uint speed = (uint)stick["Speed"];
      Debug.WriteLine(Converters.ByteUnitConverters.ConvertBytesToReadableUnit((ulong)capacity));
      Debug.WriteLine($"Stick: {capacity / (1024 * 1024)} MB | Speed: {speed} MHz");
    }

  }

  public static string GetInstalledMemorySizeInString(ulong capacity) {
    return ByteUnitConverters.ConvertBytesToReadableUnit((ulong)capacity);
  }

  public static ulong GetInstalledMemorySize() {
    ulong totalPhysicalMemory = 0;
    ManagementObjectSearcher stickSearcher = new(Win32PhysicalQueryString);
    foreach(var stick in stickSearcher.Get()) {
      ulong capacity_ = (ulong)stick[StickCapacityKey];
      totalPhysicalMemory += capacity_;
    }
    return totalPhysicalMemory;
  }

  public static List<IStickInfo> GetStickInfo() {
    List<IStickInfo> result_ = new();
    ManagementObjectSearcher stickSearcher_ = new(Win32PhysicalQueryString);
    foreach(var stick_ in stickSearcher_.Get()) {
      ulong capacity_ = (ulong)stick_[StickCapacityKey];
      uint speed_ = (uint)stick_[StickSpeedKey];
      string factor_ = GetMemoryFormFactorName(stick_[FormFactorQueryKey]);
      result_.Add(new StickInfo(capacity_, speed_, factor_));
    }
    return result_;
  }

  private void GetAvailableMemory() {
    PerformanceCounter availableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
    float availableMemoryMB = availableMemoryCounter.NextValue();
    string availableMemoryReadable = ByteUnitConverters.ConvertBytesToReadableUnit((ulong)(availableMemoryMB * 1024 * 1024));
    //float availableMemoryGB = ByteUnitConverters.ConvertMBToReadableUnit((long)availableMemoryMB);
    Debug.WriteLine("Available Memory: " + availableMemoryReadable);
  }

  private void GetTotalMemory() {
    PerformanceCounter totalMemoryCounter = new PerformanceCounter("Memory", "Committed Bytes");
    float totalMemoryBytes = totalMemoryCounter.NextValue();
    string totalMemoryReadable = ByteUnitConverters.ConvertBytesToReadableUnit((ulong)totalMemoryBytes);
    Debug.WriteLine("Total Memory: " + totalMemoryReadable);
  }

  public static int GetSlotsUsed() {
    ManagementObjectSearcher searcher_ = new(Win32PhysicalQueryString);
    int slotCount = 0;
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      slotCount++;
    }
    Debug.WriteLine("Slots Used: " + slotCount);
    return slotCount;
  }

  public static uint GetSlotsTotal() {
    uint slotCount = 0;
    try {
      ManagementObjectSearcher searcher = new("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");

      foreach(ManagementObject obj in searcher.Get()) {
        slotCount = (uint)obj["MemoryDevices"];
        Console.WriteLine("Total RAM Slots: " + slotCount);
      }
    }
    catch(Exception ex) {
      Debug.WriteLine(ex.Message);
      slotCount = 0;
    }
    return slotCount;
  }

  public static string GetMemoryTypeString(object memoryValue) {
    // 1. Get the converter for the value's actual type
    TypeConverter converter = TypeDescriptor.GetConverter(memoryValue.GetType());

    // 2. Check if it can convert to a string
    if(converter != null && converter.CanConvertTo(typeof(string))) {
      // 3. Return the converted string
      return converter.ConvertToString(memoryValue) ?? string.Empty;
    }

    // Fallback: Default to standard ToString()
    return memoryValue?.ToString() ?? string.Empty;
  }

  public static void GetMemoryType() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher(Win32PhysicalQueryString);
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      var memoryTypeCode = int.Parse(wmi_["MemoryType"].ToString());
      var memoryType = GetMemoryTypeString(memoryTypeCode);
      Debug.WriteLine("Memory Type: " + memoryType);
    }
  }

  private void Test() {
    Debug.WriteLine("Ram Form Factor = " + QueryMemory.GetRamFormFactor());
    QueryMemory.GetHardwareReservedRam();

    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach(ManagementObject wmi_ in searcher_.Get()) {
      var cp1_ = ulong.Parse(wmi_["Capacity"].ToString());
      var cp_ = ByteUnitConverters.ConvertBytesToReadableUnit(cp1_);
      Debug.WriteLine("Capacity: " + cp_);

      var speedInMHz = double.Parse(wmi_["Speed"].ToString());
      Debug.WriteLine("Raw Speed: " + wmi_["Speed"] + " MT/s");
      //var speedInGHz = HzUnitConverter.ConvertMHzToReadableUnit(speedInMHz);
      //Debug.WriteLine("Speed: " + speedInGHz + " MHz");
    }
  }
}
