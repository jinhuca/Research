using LibreHardwareMonitor.Hardware;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Management;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace CpuModule.Models;

struct CpuModelDefinitions {
  public const int TimerStartDelay = 0;
  public const int TimerInterval = 1000;
}

public class CpuModel : BindableBase, ICpuModel {
  private Timer _timer;
  private readonly ISMProvider? _smProvider;

  public event NotifyCollectionChangedEventHandler? CollectionChanged;

  public CpuModel(ISMProvider? smProvider) {
    _smProvider = smProvider;
    init();
    initTimer();
  }

  private void initTimer() {
    _timer = new Timer(
      callback: fetchSystemInfo,
      state: DateTime.UtcNow,
      dueTime: CpuModelDefinitions.TimerStartDelay,
      period: CpuModelDefinitions.TimerInterval);
  }

  private double GetCurrentCpuSpeed() {
    // 1. Get the Maximum clock speed of the CPU via WMI (returned in MHz)
    uint maxClockSpeed = 0;
    using (var searcher = new ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor")) {
      foreach (var obj in searcher.Get()) {
        maxClockSpeed = (uint)obj["MaxClockSpeed"];
        break; // Assuming there's only one processor
      }
    }
    // 2. Setup the Performance Counter for current performance percentage
    // This represents the current speed as a % of the max speed
    using (var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total")) {
      // Initial call often returns 0; needs a small delay for an accurate reading
      cpuPerfCounter.NextValue();
      Thread.Sleep(1000);

      //while (true) {
      float perfPercentage = cpuPerfCounter.NextValue();

      // 3. Calculate current speed in MHz
      double currentSpeedMhz = (maxClockSpeed * perfPercentage) / 100.0;
      double currentSpeedGhz = currentSpeedMhz / 1000.0;

      //Console.WriteLine($"Max Speed:     {maxClockSpeed} MHz");
      //Console.WriteLine($"Current Perf:  {perfPercentage:F2}%");
      //Console.WriteLine($"Current Speed: {currentSpeedGhz:F2} GHz");

      //Thread.Sleep(1000); // Update every second
      //RealTimeInfo.Speed = currentSpeedGhz;
      return currentSpeedGhz;
    }
  }

  private void fetchSystemInfo(object data) {
    RealTimeInfo = new RealTimeInfo {
      Utilization = NativeMethodGroup.GetTotalCpuUtilization(),
      Speed = GetCurrentCpuSpeed(),
      Processes = Process.GetProcesses().Length,
      Threads = Process.GetProcesses().Sum(proc => proc.Threads.Count),
      Handles = Process.GetProcesses().Sum(proc => proc.HandleCount),
      UpTime = TimeSpan.FromMilliseconds(Environment.TickCount64),
      Temperature = GetTemperature()
    };

    //Utilization = NativeMethodGroup.GetTotalCpuUtilization();
    //Debug.WriteLine("++++");
    //Debug.WriteLine(Utilization);
    //RealTimeInfo = realTime_;
    //GetCurrentCpuSpeed();

    //Debug.WriteLine("======================");
    //Debug.WriteLine("Utilization =   " + RealTimeInfo.Utilization);
    //Debug.WriteLine("Current Speed = " + RealTimeInfo.Speed);
    //Debug.WriteLine("Processes =     " + RealTimeInfo.Processes);
    //Debug.WriteLine("Threads =       " + RealTimeInfo.Threads);
    //Debug.WriteLine("Handles =       " + RealTimeInfo.Handles);
    //Debug.WriteLine("Up time =       " + RealTimeInfo.UpTime);
    //Debug.WriteLine("Temperature =   " + RealTimeInfo.Temperature);
    //Debug.WriteLine("");
  }

  private void init() {
    try {
      VendorName = NativeMethodGroup.Vendor();
      BrandName = NativeMethodGroup.Brand();

      BasicInfo = new BasicInfo {
        BaseSpeed = NativeMethodGroup.GetBaseSpeed(),
        SocketNum = NativeMethodGroup.GetSocketNum(),
        NumOfPhysicalCores = NativeMethodGroup.GetPhysicalCoreCount(),
        NumOfLogicalCores = NativeMethodGroup.GetLogicalCoreCount(),
        VirtualizationEnabled = NativeMethodGroup.VirtualizationEnabled(),
      };
      InstructionInfo = NativeMethodGroup.GetInstructionSetStruct();
      CacheSize = NativeMethodGroup.GetCacheSize();
      //ReadableCacheSize = CacheSize.Value.ToReadableCacheSize();
      var temp = Converters.HzUnitConverter.ConvertMHzToReadableUnit(BasicInfo.BaseSpeed);
    }
    catch (Exception ex) {
      //BasicInfo = null;
      Console.WriteLine(ex.Message);
    }

    try {
      if (_smProvider != null) {
        ISMQuery cpuQuery_ = _smProvider.GetQueryProvider(SMCategories.Processor);
        ExtendedInfo = new ExtendedInfo { InfoDictionary = cpuQuery_.Query(Win32_Processor.QueryString) };
      }
    }
    catch (System.Management.ManagementException smx) {
      ExtendedInfo = null;
      Console.WriteLine(smx.Message);
    }

    RealTimeInfo = new RealTimeInfo {
      //Utilization = NativeMethodGroup.GetTotalCpuUtilization(),
    };
  }

  private float GetTemperature() {
    try {
      Computer computer = new Computer { IsCpuEnabled = true };
      computer.Open();
      foreach (var hardware in computer.Hardware) {
        if (hardware.HardwareType == HardwareType.Cpu) {
          hardware.Update();
          foreach (var sensor in hardware.Sensors) {
            if (sensor.SensorType == SensorType.Temperature) {
              return sensor.Value ?? 0;
            }
          }
        }
      }
    }
    catch (Exception ex) {
      Console.WriteLine(ex.Message);
    }
    return 0;
  }

  private string _vendorName = string.Empty;
  public string VendorName {
    get => _vendorName;
    set => SetProperty(ref _vendorName, value);
  }

  private string _brandName = string.Empty;
  public string BrandName {
    get => _brandName;
    set => SetProperty(ref _brandName, value);
  }

  private BasicInfo _processorInfo;
  public BasicInfo BasicInfo {
    get => _processorInfo;
    set => SetProperty(ref _processorInfo, value);
  }

  private InstructionInfo? _instructionInfo;
  public InstructionInfo? InstructionInfo {
    get => _instructionInfo;
    set => SetProperty(ref _instructionInfo, value);
  }

  private ExtendedInfo? _extendedInfo;
  public ExtendedInfo? ExtendedInfo {
    get => _extendedInfo;
    set => SetProperty(ref _extendedInfo, value);
  }

  private CacheSize _cacheSize;
  public CacheSize CacheSize {
    get => _cacheSize;
    set => SetProperty(ref _cacheSize, value);
  }

  //private ReadableCacheSize _readableCacheSize;
  //public ReadableCacheSize ReadableCacheSize {
  //  get => _readableCacheSize;
  //  set => SetProperty(ref _readableCacheSize, value);
  //}

  private RealTimeInfo _realTimeInfo;
  public RealTimeInfo RealTimeInfo {
    get => _realTimeInfo;
    set => SetProperty(ref _realTimeInfo, value);
  }

  private double _utilization;
  public double Utilization {
    get => _utilization;
    set => SetProperty(ref _utilization, value);
  }
}
