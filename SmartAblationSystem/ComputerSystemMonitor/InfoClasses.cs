using System;

namespace ComputerSystemMonitor
{
  public enum ProcessStatus
  {
    NotStart,
    Running,
    NotResponding,
    Exited,
  }

  public class ProcessLifeTime
  {
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? RunningTime { get; set; }
    public ProcessStatus? Status { get; set; }
  }

  public class MemoryInfo
  {
    public float Gen0Collection { get; set; }
    public float Gen1Collection { get; set; }
    public float Gen2Collection { get; set; }
    public long PhysicalMemoryUsed { get; set; }
    public long PagedSystemMemory { get; set; }
    public long PagedMemory { get; set; }
    public long VirtualMemory { get; set; }
  }

  public class DiskInfo
  {
    public long AvailableSize { get; set; }
  }

  public class ProcessMemoryStatistics
  {
    public long PeakMemoryUsed { get; set; }
    public long PeakPagedMemoryUsed { get; set; }
    public long PeakVirtualMemoryUsed { get; set; }
  }

  public class ProcessorInfo
  {
    public float PercentageUsed { get; set; }
    public TimeSpan UserTime { get; set; }
    public TimeSpan TotalTime { get; set; }
  }

  public class ExceptionInfo
  {
    public int Throwns { get; set; }
  }

  public class ThreadsInfo
  {
    public int ManagedThreads { get; set; }
    public int NativeThreads { get; set; }
  }

  public class SystemInfo
  {
    public string Name { get; set; }
    public string Processor { get; set; }
    public string BIOS { get; set; }
    public long MemorySize { get; set; }
    public long DiskSize { get; set; }
    public string OperatingSystem { get; set; }
    public DateTime LastBootUpTime { get; set; }
    public string Organization { get; set; }
    public string UserName { get; set; }
  }

  public class ProcessSnapshot
  {
    public DateTime TimeStamp { get; set; }
    public ProcessorInfo Processor { get; set; } = new ProcessorInfo();
    public MemoryInfo Memory { get; set; } = new MemoryInfo();
    public DiskInfo Disk { get; set; }= new DiskInfo();
    public ExceptionInfo Exceptions { get; set; } = new ExceptionInfo();
    public int AssembliesLoaded { get; set; }
    public ThreadsInfo Threads { get; set; } = new ThreadsInfo();
    public TimeSpan RunningTime { get; set; }
    public ProcessStatus Status { get; set; }
  }
}
