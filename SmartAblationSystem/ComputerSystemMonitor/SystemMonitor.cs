using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using static System.Console;

namespace ComputerSystemMonitor
{
  internal class SystemMonitor
  {
    internal static SystemInfo ComputerSystem { get; set; } = new SystemInfo();
    internal static ProcessSnapshot Snapshot { get; set; } = new ProcessSnapshot();
    internal static ProcessLifeTime ProcessLifeTime { get; set; } = new ProcessLifeTime();
    internal static ProcessMemoryStatistics MemoryStatistics = new ProcessMemoryStatistics();
    private static string fileFullPathName = string.Empty;

    private static PerformanceCounter ProcessorCounter;
    private static PerformanceCounter Gen0Collections;
    private static PerformanceCounter Gen1Collections;
    private static PerformanceCounter Gen2Collections;

    private static PerformanceCounter ExceptionsCounter;
    private static PerformanceCounter AssemblyCounter;
    private static PerformanceCounter ManagedThreadCounter;
    private static PerformanceCounter NativeThreadCounter;

    private static int processorCores = 0;

    internal SystemMonitor()
    {
    }

    internal static void Start()
    {
      SetupMonitorFileSystem();
      SetConsoleTitle(Constants.AppTitle);
      PrintSectionTitle(Constants.ComputerSystemTitle);
      GetSystemInformation();
      SerializeComputerSystemInfo();
      MonitorProcess();
      ReadLine();
    }

    private static void SetupMonitorFileSystem()
    {
      string monitorFileName = Constants.MonitorTitle + Constants.Underscore + DateTime.Now.ToString(Constants.DateTimeInFileName) + Constants.MonitorExtension;
      string filePath = Assembly.GetExecutingAssembly().Location;
      string directoryName = Path.GetDirectoryName(filePath);
      if (directoryName != null)
      {
        var pathMonitorFile = Path.Combine(directoryName, Constants.MonitorFolderName);
        if (!Directory.Exists(pathMonitorFile))
        {
          Directory.CreateDirectory(pathMonitorFile);
        }
      }
      string[] path = { directoryName, Constants.MonitorFolderName, monitorFileName };
      var pathString = Path.Combine(path);
      fileFullPathName = pathString;
    }

    private static void GetSystemInformation()
    {
      QueryProcessorInformation();
      QueryWin32_BIOS();
      QueryWin32_PhysicalMemory();
      QueryWin32_DiskDrive();
      QueryWin32_OperatingSystem();
      QueryWin32_ComputerSystem();
    }

    private static void MonitorProcess()
    {
      using (var processMonitored = HookProcessByName(Constants.ApplicationName))
      {
        try
        {
          var processStartTime = processMonitored.StartTime;
          PrintPerformancePair("Start Time", $"{DateTimeOutput(processStartTime)}");
          ProcessLifeTime.StartTime = processStartTime;
          ProcessLifeTime.Status = processMonitored.Responding ? ProcessStatus.Running : ProcessStatus.NotResponding;
        }
        catch (Exception ex)
        {
          TerminateMonitoringProcess(ex);
        }

        do
        {
          if (processMonitored?.HasExited == true)
          {
            continue;
          }
          processMonitored?.Refresh();

          PrintPerformancePair("Time Stamp", $"{DateTimeOutput(DateTime.Now)}");
          Snapshot.TimeStamp = DateTime.Now;

          PrintSubSectionTitle("CPU:");
          GetCpuUsage(processMonitored);

          PrintSubSectionTitle("Memory:");
          GetMemoryCounters(processMonitored);

          PrintSubSectionTitle("Storage:");
          GetDiskCounters(processMonitored);

          PrintSubSectionTitle("Exceptions thrown:");
          GetExceptionsThrown(processMonitored);

          PrintSubSectionTitle("Assemblies Loaded:");
          GetLoadingPerformanceCounters(processMonitored);

          PrintSubSectionTitle("Threads:");
          GetLockThreads(processMonitored);
          GetNativeThreads(processMonitored);

          PrintSubSectionTitle("State:");
          var runningTime = DateTime.Now - processMonitored.StartTime;
          Snapshot.RunningTime = runningTime;
          PrintPerformancePair("Running Time", $"{runningTime}");

          var status = processMonitored.Responding ? ProcessStatus.Running : ProcessStatus.NotResponding;
          Snapshot.Status = status;
          PrintPerformancePair("Running Status", $"{status.ToString()}");

          CursorTop = Constants.CursorTop_UpdatePerformance;
          CursorLeft = Constants.CursorLeft_Reset;
          CursorVisible = false;
          SerializeSnapshot();

          MemoryStatistics.PeakPagedMemoryUsed = processMonitored.PeakPagedMemorySize64;
          MemoryStatistics.PeakVirtualMemoryUsed = processMonitored.PeakVirtualMemorySize64;
          MemoryStatistics.PeakMemoryUsed = processMonitored.PeakWorkingSet64;
        } while (!processMonitored.WaitForExit(300));

        TerminateMonitoringProcess();
      }
    }

    private static void TerminateMonitoringProcess(Exception ex = null)
    {
      ProcessLifeTime.EndTime = DateTime.Now;
      ProcessLifeTime.RunningTime = ProcessLifeTime.EndTime - ProcessLifeTime.StartTime;
      ProcessLifeTime.Status = ProcessStatus.Exited;
      SerializeSummary();

      CursorTop = Constants.CursorTop_UpdateState;
      CursorLeft = Constants.CursorLeft_Reset;
      UpdateMonitoringState("Monitoring State", "Terminiated");

      CursorTop = Constants.CursorTop_UpdateRunningStatus;
      CursorLeft = Constants.CursorLeft_Reset;
      UpdateMonitoringState("Running Status", "Exited");
      CursorTop = Constants.CursorTop_Summary;
      CursorLeft = Constants.CursorLeft_Reset;

      PrintSectionTitle($"Execution Summary");
      PrintSummaryInformationPair("Process exit time", $"{DateTimeOutput(DateTime.Now)}");
      PrintSummaryInformationPair("Peak physical memory usage", $"{ByteOutput(MemoryStatistics.PeakMemoryUsed)}");
      PrintSummaryInformationPair("Peak paged memory usage", $"{ByteOutput(MemoryStatistics.PeakPagedMemoryUsed)}");
      PrintSummaryInformationPair("Peak virtual memory usage", $"{ByteOutput(MemoryStatistics.PeakVirtualMemoryUsed)}");

      WriteLine();
      ResetColor();
      var exceptionMsg = ex != null ? $"\n@ {ex.Message}." : string.Empty;
      WriteLine($"The monitored application - {Constants.ApplicationName} has terminated. {exceptionMsg}");
      Environment.Exit(0);
    }

    private static Process HookProcessByName(string processName)
    {
      PrintSectionTitle($"{processName}");
      var processCollection = Process.GetProcesses();

      ResetColor();
      PrintPerformancePair("Monitoring State", $"Waiting ...");
      ResetColor();

      var timer = new System.Timers.Timer() { AutoReset = true, Interval = 100 };

      if (processCollection.All(process => !string.Equals(process.ProcessName, processName, StringComparison.OrdinalIgnoreCase)))
      {
        timer.Start();
        timer.Elapsed += (s, e) => { processCollection = Process.GetProcesses(); };
      }

      while (processCollection.All(x => !string.Equals(x.ProcessName, processName, StringComparison.OrdinalIgnoreCase)))
      {
      }

      timer.Stop();
      CursorLeft = Constants.CursorLeft_Reset;
      CursorTop -= Constants.CursorTop_Decreasement;

      PrintPerformancePair("Monitoring State", $"Active");

      ProcessorCounter = new PerformanceCounter(Constants.PerformanceProcessCategory, Constants.PerformanceCpuCounterName, Constants.ApplicationName, true);
      Gen0Collections = new PerformanceCounter(Constants.PerformanceDotNetMemoryCounterName, Constants.CounterGen0Name, Constants.ApplicationName, true);
      Gen1Collections = new PerformanceCounter(Constants.PerformanceDotNetMemoryCounterName, Constants.CounterGen1Name, Constants.ApplicationName, true);
      Gen2Collections = new PerformanceCounter(Constants.PerformanceDotNetMemoryCounterName, Constants.CounterGen2Name, Constants.ApplicationName, true);
      ExceptionsCounter = new PerformanceCounter(Constants.PerformanceDotNetExceptionsName, Constants.CounterExceptions, Constants.ApplicationName, true);
      AssemblyCounter = new PerformanceCounter(Constants.PerformanceDotNetLoadingName, Constants.CounterCurrentAssemblies, Constants.ApplicationName, true);
      ManagedThreadCounter = new PerformanceCounter(Constants.PerformanceDotNetThreadsName, Constants.CounterLogicalThreads, Constants.ApplicationName, true);
      NativeThreadCounter = new PerformanceCounter(Constants.PerformanceDotNetThreadsName, Constants.CounterPhysicalThreads, Constants.ApplicationName, true);

      var temp = processCollection.First(x => string.Equals(x.ProcessName, processName, StringComparison.OrdinalIgnoreCase));

      return temp;
    }

    private static void SerializeComputerSystemInfo()
    {
      string serializedData = JsonConvert.SerializeObject(ComputerSystem, Formatting.Indented);
      File.WriteAllText(fileFullPathName, serializedData);
    }

    private static void SerializeSnapshot()
    {
      string serializedSystemInfo = JsonConvert.SerializeObject(ComputerSystem, Formatting.Indented) + Environment.NewLine;
      File.WriteAllText(fileFullPathName, serializedSystemInfo);
      string serializedData = JsonConvert.SerializeObject(Snapshot, Formatting.Indented);
      File.AppendAllText(fileFullPathName, serializedData);
    }

    private static void SerializeSummary()
    {
      string serializedSystemInfo = JsonConvert.SerializeObject(ComputerSystem, Formatting.Indented) + Environment.NewLine;
      File.WriteAllText(fileFullPathName, serializedSystemInfo);
      string serializedData = JsonConvert.SerializeObject(Snapshot, Formatting.Indented) + Environment.NewLine;
      File.AppendAllText(fileFullPathName, serializedData);
      string serializedSummary = JsonConvert.SerializeObject(ProcessLifeTime, Formatting.Indented);
      File.AppendAllText(fileFullPathName, serializedSummary);
    }

    #region Query System Information

    private static void QueryProcessorInformation()
    {
      var processorInfo = QueryManagementObject(Constants.Win32_Processor);
      foreach (var item in processorInfo)
      {
        var sysName = item.Properties[Constants.SystemNameProperty].Value.ToString();
        PrintNameValuePair(Constants.SystemName, sysName);
        ComputerSystem.Name = sysName;

        var processorName = item.Properties["Name"].Value.ToString();
        if (double.TryParse(item.Properties[Constants.CurrentSpeed].Value.ToString(), out double speed)
          && int.TryParse(item.Properties[Constants.Cores].Value.ToString().Trim(), out processorCores))
        {
          speed /= Constants.OneThousand;
          processorName += $"{Constants.Semicolon}{processorCores} Cores{Constants.Semicolon}Running @ {speed} GHz";
        }
        PrintNameValuePair("Processor", processorName);
        ComputerSystem.Processor = processorName;
      }
    }

    private static void QueryWin32_BIOS()
    {
      var BiosInfo = QueryManagementObject("Win32_BIOS");
      foreach (var item in BiosInfo)
      {
        var bios = $"{item.Properties["Manufacturer"].Value}" + $" {Constants.Dash} " + $"{item.Properties["Name"].Value}";
        PrintNameValuePair("BIOS", bios);
        ComputerSystem.BIOS = bios;
      }
    }

    private static void QueryWin32_PhysicalMemory()
    {
      var Win32_PhysicalMemoryInfo = QueryManagementObject(Constants.Win32_PhysicalMemory);
      long total = 0;
      foreach (var item in Win32_PhysicalMemoryInfo)
      {
        if (long.TryParse(item.Properties["Capacity"].Value.ToString(), out var memorySize))
        {
          total += memorySize;
        }
      }
      PrintNameValuePair(Constants.PhysicalMemory, $"{ByteOutput(total)}");
      ComputerSystem.MemorySize = total;
    }

    private static void QueryWin32_DiskDrive()
    {
      var Win32_DiskDriveInfo = QueryManagementObject(Constants.Win32_DiskDrive);
      long total = 0;
      foreach (var item in Win32_DiskDriveInfo)
      {
        var size = item.Properties["Size"].Value.ToString();
        if (long.TryParse(size, out var diskSize))
        {
          total += diskSize;
        }
      }
      PrintNameValuePair("Disk Drives", $"{ByteOutput(total)}");
      ComputerSystem.DiskSize = total;
    }

    private static void QueryWin32_OperatingSystem()
    {
      var Win32_OperatingSystemInfo = QueryManagementObject(Constants.Win32_OperatingSystem);
      foreach (var item in Win32_OperatingSystemInfo)
      {
        StringBuilder sb = new StringBuilder();
        sb.Append(item.Properties["Caption"].Value);
        sb.Append($", {item.Properties["OSArchitecture"].Value}");
        sb.Append($", Build: {item.Properties["BuildNumber"].Value}");
        var os = sb.ToString();

        PrintNameValuePair(Constants.OperatingSystemName, os);
        ComputerSystem.OperatingSystem = os;

        var lastBoot = ManagementDateTimeConverter.ToDateTime(item.Properties[Constants.LastBootUpTime].Value.ToString());
        PrintNameValuePair(Constants.LastBootName, lastBoot.ToString(Constants.DateTimeFormatString));
        ComputerSystem.LastBootUpTime = lastBoot;

        var organization = item.Properties[Constants.Organization].Value.ToString();
        PrintNameValuePair(Constants.Organization, organization);
        ComputerSystem.Organization = organization;
      }
    }

    private static void QueryWin32_ComputerSystem()
    {
      var Win32_CacheMemoryInfo = QueryManagementObject(Constants.Win32_ComputerSystem);
      foreach (var item in Win32_CacheMemoryInfo)
      {
        string userName = item.Properties[Constants.UserNameProperty].Value.ToString();
        PrintNameValuePair(Constants.UserName, $"{userName}");
        ComputerSystem.UserName = userName;
      }
    }

    private static ManagementObjectCollection QueryManagementObject(string searchKey)
    {
      using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + searchKey))
      {
        return searcher.Get();
      }
    }

    #endregion Query System Information

    #region Query PerformanceCounters

    private static void GetCpuUsage(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }

      try
      {
        PrintPerformancePair("Percentage of CPU Usage", $"{ProcessorCounter.NextValue() / processorCores}");
        PrintPerformancePair("User processor time", $"{theProcess?.UserProcessorTime}");
        PrintPerformancePair("Total processor time", $"{theProcess?.TotalProcessorTime}");

        Snapshot.Processor.PercentageUsed = ProcessorCounter.NextValue() / processorCores;
        Snapshot.Processor.UserTime = theProcess.UserProcessorTime;
        Snapshot.Processor.TotalTime = theProcess.TotalProcessorTime;
      }
      catch (Exception ex)
      {
        TerminateMonitoringProcess(ex);
      }
    }

    private static void GetMemoryCounters(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }

      try
      {
        var gen0 = Gen0Collections?.NextValue();
        var gen1 = Gen1Collections?.NextValue();
        var gen2 = Gen2Collections?.NextValue();
        var workingSet = theProcess.WorkingSet64;
        var pagedSystemMemory = theProcess.PagedSystemMemorySize64;
        var pagedMemory = theProcess.PagedMemorySize64;
        var virtualMemory = theProcess.VirtualMemorySize64;

        if (gen0.HasValue) Snapshot.Memory.Gen0Collection = gen0.Value;
        if (gen1.HasValue) Snapshot.Memory.Gen1Collection = gen1.Value;
        if (gen2.HasValue) Snapshot.Memory.Gen2Collection = gen2.Value;
        Snapshot.Memory.PhysicalMemoryUsed = workingSet;
        Snapshot.Memory.PagedSystemMemory = pagedSystemMemory;
        Snapshot.Memory.PagedMemory = pagedMemory;
        Snapshot.Memory.VirtualMemory = virtualMemory;

        PrintPerformancePair(Constants.CounterGen0Name, $"{gen0}");
        PrintPerformancePair(Constants.CounterGen1Name, Gen1Collections?.NextValue().ToString());
        PrintPerformancePair(Constants.CounterGen2Name, Gen2Collections?.NextValue().ToString());
        PrintPerformancePair(Constants.PhysicalMemoryName, $"{ ByteOutput(workingSet)}");
        PrintPerformancePair(Constants.PagedSystemMemoryName, $"{ ByteOutput(pagedSystemMemory)}");
        PrintPerformancePair(Constants.PagdMemoryAllocatedName, $"{ByteOutput(pagedMemory)}");
        PrintPerformancePair(Constants.VirtualMemorySizeName, $"{ByteOutput(virtualMemory)}");
      }
      catch (Exception ex)
      {
        TerminateMonitoringProcess(ex);
      }
    }

    private static void GetDiskCounters(Process theProcess)
    {
      long freeSpace = 0;
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      foreach (DriveInfo drive in allDrives)
      {
        if (drive.IsReady == true)
        {
          freeSpace += drive.AvailableFreeSpace;
        }
      }
      Snapshot.Disk.AvailableSize = freeSpace;
      PrintPerformancePair(Constants.AvailableDiskSpaceName, $"{ByteOutput(freeSpace)}");
    }

    private static void GetExceptionsThrown(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }
      int exceptionThrown = (int)ExceptionsCounter?.NextValue();
      PrintPerformancePair("Exceptions", exceptionThrown.ToString());
      Snapshot.Exceptions.Throwns = exceptionThrown;
    }

    private static void GetLoadingPerformanceCounters(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }
      int assemblies = (int)AssemblyCounter?.NextValue();
      Snapshot.AssembliesLoaded = assemblies;
      PrintPerformancePair("Loaded Assemblies", assemblies.ToString());
    }

    private static void GetLockThreads(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }
      int managedThreads = (int)ManagedThreadCounter?.NextValue();
      Snapshot.Threads.ManagedThreads = managedThreads;
      PrintPerformancePair(Constants.ManagedThreads, $"{managedThreads}");
    }

    private static void GetNativeThreads(Process theProcess)
    {
      if (theProcess?.HasExited == true)
      {
        return;
      }
      int nativeThreads = (int)NativeThreadCounter?.NextValue();
      Snapshot.Threads.NativeThreads = nativeThreads;
      PrintPerformancePair(Constants.NativeThreads, $"{nativeThreads}");
    }

    #endregion Query PerformanceCounters

    #region Helper Methods

    private static void UpdateMonitoringState(string name, string value)
    {
      ForegroundColor = ConsoleColor.Yellow;
      Write(RightPadString($"{Constants.Tab}{name}"));
      PrintNameValueSeparator();
      ForegroundColor = ConsoleColor.White;
      WriteLine(LeftPadString(value));
      ResetColor();
    }

    private static void PrintNameValuePair(string name, string value)
    {
      SetSystemInfoForegroundBegin();
      Write(RightPadString($"{Constants.Tab}{name}"));
      PrintNameValueSeparator();
      ForegroundColor = ConsoleColor.Blue;
      WriteLine(value);
      ResetColor();
    }

    private static void PrintPerformancePair(string name, string value)
    {
      SetSystemInfoForegroundBegin();
      Write(RightPadString($"{Constants.Tab}{name}"));
      PrintNameValueSeparator();
      ForegroundColor = ConsoleColor.Green;
      WriteLine(LeftPadString(value));
      ResetColor();
    }

    private static void PrintSummaryInformationPair(string name, string value)
    {
      ForegroundColor = ConsoleColor.Yellow;
      Write(RightPadString($"{Constants.Tab}{name}"));
      PrintNameValueSeparator();
      ForegroundColor = ConsoleColor.White;
      WriteLine(LeftPadString(value));
      ResetColor();
    }

    private static void SetConsoleTitle(string title) => Title = title;

    internal static void PrintSectionTitle(string sectionTitle)
    {
      ResetColor();
      WriteLine(Constants.LineSeparator);
      WriteLine($" {sectionTitle}");
      WriteLine(Constants.LineSeparator);
    }

    internal static void PrintSubSectionTitle(string subSectionTitle)
    {
      ResetColor();
      WriteLine($" {subSectionTitle}");
    }

    private static void PrintNameValueSeparator() => Write($"{Constants.Colon} ");
    private static string RightPadString(string message) => message.PadRight(Constants.PadSizeLeft);
    private static string LeftPadString(string message) => message.PadLeft(Constants.PadSizeRight);
    private static void SetSystemInfoForegroundBegin() => ForegroundColor = ConsoleColor.Yellow;
    private static string ByteOutput(long value) => value.ToString(Constants.ByteFormatString, CultureInfo.InvariantCulture) + $" {Constants.BytesName}";
    private static string DateTimeOutput(DateTime value) => value.ToString(Constants.DateTimeFormatString, CultureInfo.InvariantCulture);

    #endregion Helper Methods
  }
}
