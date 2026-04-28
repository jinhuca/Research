using Converters;
using CpuModule.Models;
using CpuModule.Views;

namespace CpuModule.ViewModels;

public class CpuViewModel : BindableBase, ICpuViewModel {
  private readonly CpuModel _model;
  public CpuViewModel(CpuModel model) {
    _model = model;
    _model.PropertyChanged += (s, e) => {
      if(e.PropertyName == nameof(_model.Utilization))
        RaisePropertyChanged(nameof(Utilization));
    };
    initProperties();
  }

  private void initProperties() {
    //BrandName = _model.BrandName ?? string.Empty;
    BaseSpeed = HzUnitConverter.ConvertMHzToReadableUnit(_model.BasicInfo.BaseSpeed);
    SocketNum = _model.BasicInfo.SocketNum;
    PhysicalCoresNum = _model.BasicInfo.NumOfPhysicalCores;
    LogicalCoresNum = _model.BasicInfo.NumOfLogicalCores;
    Virtualization = _model.BasicInfo.VirtualizationEnabled ? "Enabled" : "Disabled";
    L1CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L1_cache_size);
    var temp = _model.CacheSize.L2_cache_line_size;
    L2CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L2_cache_size);
    L3CacheSize = ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L3_cache_size);

    Utilization = _model.RealTimeInfo?.Utilization ?? 0;
    
  }

  public string BrandName {
    get {
      return _model.BrandName;
    }
    set {
      if(_model.BrandName != value) {
        _model.BrandName = value;
        RaisePropertyChanged(nameof(BrandName));
      }
    }
  }

  public string VendorName {
    get {
      return ViewModelConversions.VendorNameConvert(_model.VendorName);
    }
    set {
      if(_model.VendorName != value) {
        _model.VendorName = value;
        RaisePropertyChanged(nameof(VendorName));
      }
    }
  }

  public string BaseSpeed {
    get {
      return HzUnitConverter.ConvertMHzToReadableUnit(_model.BasicInfo.BaseSpeed);
    }
    set {
      if(HzUnitConverter.ConvertMHzToReadableUnit(_model.BasicInfo.BaseSpeed) != value) {
        _model.BasicInfo.BaseSpeed = Convert.ToInt32(value);
        RaisePropertyChanged(nameof(BaseSpeed));
      }
    }
  }

  public int SocketNum {
    get {
      return _model.BasicInfo.SocketNum;
    }
    set {
      if(_model.BasicInfo.SocketNum != value) {
        _model.BasicInfo.SocketNum = value;
        RaisePropertyChanged(nameof(SocketNum));
      }
    }
  }

  public int PhysicalCoresNum {
    get {
      return _model.BasicInfo.NumOfPhysicalCores;
    }
    set {
      if(_model.BasicInfo.NumOfPhysicalCores != value) {
        _model.BasicInfo.NumOfPhysicalCores = value;
        RaisePropertyChanged(nameof(PhysicalCoresNum));
      }
    }
  }

  public int LogicalCoresNum {
    get {
      return _model.BasicInfo.NumOfLogicalCores;
    }
    set {
      if(_model.BasicInfo.NumOfLogicalCores != value) {
        _model.BasicInfo.NumOfLogicalCores = value;
        RaisePropertyChanged(nameof(LogicalCoresNum));
      }
    }
  }

  public string Virtualization {
    get {
      return _model.BasicInfo.VirtualizationEnabled
        ? ViewDefinitions.EnabledText
        : ViewDefinitions.DisabledText;
    }
    set {
      if(value == ViewDefinitions.EnabledText) {
        _model.BasicInfo.VirtualizationEnabled = true;
      }
      else {
        _model.BasicInfo.VirtualizationEnabled = false;
      }
      RaisePropertyChanged(nameof(Virtualization));
    }
  }

  public string L1CacheSize {
    get {
      return ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L1_cache_size);
    }
    set {
      if(_model.CacheSize.L1_cache_size != ByteUnitConverters.ConvertReadableUnitToBytes(value)) {
        long temp = ByteUnitConverters.ConvertReadableUnitToBytes(value);
        CacheSize cs = _model.CacheSize;
        cs.L1_cache_size = (int)temp;
        _model.CacheSize = cs;
        RaisePropertyChanged(nameof(L1CacheSize));
      }
    }
  }

  public string L1CacheLineSize {
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }

  public string L2CacheSize {
    get {
      return ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L2_cache_size);
    }
    set {
      if(_model.CacheSize.L2_cache_size != ByteUnitConverters.ConvertReadableUnitToBytes(value)) {
        long temp = ByteUnitConverters.ConvertReadableUnitToBytes(value);
        CacheSize cs = _model.CacheSize;
        cs.L2_cache_size = (int)temp;
        _model.CacheSize = cs;
        RaisePropertyChanged(nameof(L2CacheSize));
      }
    }
  }

  public string L3CacheSize {
    get {
      return ByteUnitConverters.ConvertBytesToReadableUnit(_model.CacheSize.L3_cache_size);
    }
    set {
      if(_model.CacheSize.L3_cache_size != ByteUnitConverters.ConvertReadableUnitToBytes(value)) {
        long temp = ByteUnitConverters.ConvertReadableUnitToBytes(value);
        CacheSize cs = _model.CacheSize;
        cs.L3_cache_size = (int)temp;
        _model.CacheSize = cs;
        RaisePropertyChanged(nameof(L3CacheSize));
      }
    }
  }

  public double Utilization {
    get => _model.Utilization;
    set {
      _model.Utilization = value;
      RaisePropertyChanged();
    }
  }

  private double _currentSpeed;
  public double CurrentSpeed {
    get => _currentSpeed;
    set => SetProperty(ref _currentSpeed, value);
  }

  private int _processes;
  public int Processes {
    get => _processes;
    set => SetProperty(ref _processes, value);
  }

  public int _threads;
  public int Threads {
    get => _threads;
    set => SetProperty(ref _threads, value);
  }

  private int _handles;
  public int Handles {
    get => _handles;
    set => SetProperty(ref _handles, value);
  }

  private TimeSpan _uptime;
  public TimeSpan UpTime {
    get => _uptime;
    set => SetProperty(ref _uptime, value);
  }
}
