namespace MemoryModule.Definitions;

internal class Constants {
  public const string Win32PhysicalQueryString = "SELECT * FROM Win32_PhysicalMemory";

  public const string StickCapacityKey = "Capacity";
  public const string StickSpeedKey = "Speed";
  public const string FormFactorQueryKey = "FormFactor";

  public const string StickCountKey = "MemoryDevices";

  public const string Win32OSQueryString = "SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem";
  public const string TotalVisibleMemorySizeQueryKey = "TotalVisibleMemorySize";
}
