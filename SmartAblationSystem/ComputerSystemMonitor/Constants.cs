namespace ComputerSystemMonitor
{
  internal static class Constants
  {
    public const string Win32_Processor = "Win32_Processor";
    public const string Win32_PhysicalMemory = "Win32_PhysicalMemory";
    public const string Win32_DiskDrive = "Win32_DiskDrive";
    public const string Win32_OperatingSystem = "Win32_OperatingSystem";
    public const string Win32_ComputerSystem = "Win32_ComputerSystem";

    public const string CurrentSpeed = "CurrentClockSpeed";
    public const string Cores = "NumberOfCores";

    public const string PerformanceProcessCategory = "Process";
    public const string PerformanceCpuCounterName = "% Processor Time";

    public const string PerformanceDotNetMemoryCounterName = ".NET CLR Memory";
    public const string CounterGen0Name = "# Gen 0 Collections";
    public const string CounterGen1Name = "# Gen 1 Collections";
    public const string CounterGen2Name = "# Gen 2 Collections";

    public const string PhysicalMemory = "Physical Memory";
    public const string PhysicalMemoryName = "Physical memory used";
    public const string PagedSystemMemoryName = "Paged system memory";
    public const string PagdMemoryAllocatedName = "Pagd memory allocated";
    public const string VirtualMemorySizeName = "Virtual Memory size";

    public const string AvailableDiskSpaceName = "Available Disk Space";
    public const string TotalDiskSpaceName = "Total Disk Space";

    public const string PerformanceDotNetExceptionsName = ".NET CLR Exceptions";
    public const string CounterExceptions = "# of Exceps Thrown";

    public const string PerformanceDotNetLoadingName = ".NET CLR Loading";
    public const string CounterCurrentAssemblies = "Current Assemblies";

    public const string PerformanceDotNetThreadsName = ".NET CLR LocksAndThreads";
    public const string CounterLogicalThreads = "# of current logical Threads";
    public const string CounterPhysicalThreads = "# of current physical Threads";
    public const string ManagedThreads = ".NET Managed threads";
    public const string NativeThreads = "OS Native threads";

    public const string AppTitle = "Console for monitoring Smart Ablation System (TM)";
    public const string ComputerSystemTitle = "Computer System";
    public const string ApplicationName = "SmartAblationSystem";
    public const string DateTimeFormatString = "MMM/dd/yyyy HH:mm:ss.fff";
    public const string DateTimeInFileName = "yyyy_MMddHHmmssfff";
    public const string ByteFormatString = "0,0";
    public static readonly string LineSeparator = new string('-', 104);

    public const string Underscore = "_";
    public const string Tab = "  ";
    public const string Colon = ": ";
    public const string Comma = ",";
    public const string Semicolon = "; ";
    public const string Dash = "-";
    public const int PadSizeLeft = 30;
    public const int PadSizeRight = 70;
    public const int OneThousand = 1_000;

    public const int CursorLeft_Reset = 0;
    public const int CursorTop_Decreasement = 1;
    public const int CursorTop_UpdateState = 15;
    public const int CursorTop_UpdatePerformance = 17;
    public const int CursorTop_UpdateRunningStatus = 41;
    public const int CursorTop_Summary = 42;

    public const string MonitorTitle = "SmartFreezeSystem";
    public const string MonitorExtension = ".monitor";
    public const string MonitorFolderName = "Monitors";

    public const string Processor = "Processor";
    public const string Process = "Process";

    public const string SystemName = "System Name";
    public const string SystemNameProperty = "SystemName";
    
    public const string OperatingSystemName = "Operating System";
    public const string LastBootName = "Last Boot Up Time";
    public const string LastBootUpTime = "LastBootUpTime";
    public const string Organization = "Organization";

    public const string UserNameProperty = "UserName";
    public const string UserName = "User Name";
    public const string BytesName = "Bytes";
  }
}
