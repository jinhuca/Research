using System.Collections.Specialized;
using System.Diagnostics;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace CpuModule.Models;

struct CpuModelDefinitions {
  public const int TimerStartDelay = 2000;
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

  private void fetchSystemInfo(object data) {
    RealTimeInfo = new RealTimeInfo {
      Utilization = NativeMethodGroup.GetTotalCpuUtilization(),
      Speed = NativeMethodGroup.GetCurrentCpuSpeed(),
      Processes = Process.GetProcesses().Length,
      Threads = Process.GetProcesses().Sum(proc => proc.Threads.Count),
      Handles = Process.GetProcesses().Sum(proc => proc.HandleCount),
      UpTime = TimeSpan.FromMilliseconds(Environment.TickCount64)
    };

    Utilization = NativeMethodGroup.GetTotalCpuUtilization();
    Debug.WriteLine("++++");
    Debug.WriteLine(Utilization);
    //RealTimeInfo = realTime_;

    Debug.WriteLine("======================");
    Debug.WriteLine("Utilization =   " + RealTimeInfo.Utilization);
    Debug.WriteLine("Current Speed = " + RealTimeInfo.Speed);
    Debug.WriteLine("Processes =     " + RealTimeInfo.Processes);
    Debug.WriteLine("Threads =       " + RealTimeInfo.Threads);
    Debug.WriteLine("Handles =       " + RealTimeInfo.Handles);
    Debug.WriteLine("Up time =       " + RealTimeInfo.UpTime);
    Debug.WriteLine("");
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
    catch(Exception ex) {
      //BasicInfo = null;
      Console.WriteLine(ex.Message);
    }

    try {
      if(_smProvider != null) {
        ISMQuery cpuQuery_ = _smProvider.GetQueryProvider(SMCategories.Processor);
        ExtendedInfo = new ExtendedInfo { InfoDictionary = cpuQuery_.Query(Win32_Processor.QueryString) };
      }
    }
    catch(System.Management.ManagementException smx) {
      ExtendedInfo = null;
      Console.WriteLine(smx.Message);
    }

    RealTimeInfo = new RealTimeInfo {
      //Utilization = NativeMethodGroup.GetTotalCpuUtilization(),
    };
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
