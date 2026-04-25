using System.Collections.Specialized;
using System.Windows.Navigation;
using SystemManagementProvider;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;
using Unity.Registration;

namespace CpuModule.Models;

public class CpuModel : BindableBase, ICpuModel {
  public Dictionary<string, (string, string)> Data = new();
  private readonly ISMProvider? _smProvider;

  public event NotifyCollectionChangedEventHandler? CollectionChanged;

  /*
  public CpuModel() {
    try {
      ProcessorInfo = new ProcessorInfo
      {
        Vendor = NativeMethodGroup.Vendor(),
        Brand = NativeMethodGroup.Brand(),
        BaseSpeed = NativeMethodGroup.GetBaseSpeed(),
        SocketNum = NativeMethodGroup.GetSocketNum(),
        NumOfPhysicalCores = NativeMethodGroup.GetPhysicalCoreCount(),
        NumOfLogicalCores = NativeMethodGroup.GetLogicalCoreCount(),
        VirtualizationEnabled = NativeMethodGroup.VirtualizationEnabled(),
        Features = NativeMethodGroup.GetInstructionSetStruct()
      };
    }
    catch (Exception ex) {
      ProcessorInfo = null;
      Console.WriteLine(ex.Message);
    }

    //IContainerExtension container = new UnityContainerExtension();
    //container.RegisterInstance<ISMProvider>(new SMProvider());
    //_smProvider = container.Resolve<ISMProvider>();
    //ISMQuery provider_ = _smProvider.GetQueryProvider(SMCategories.Processor);
    //var container = ContainerLocator.Container;
    //var provider = container.Resolve<ISMProvider>();
  }
  */
  public CpuModel(ISMProvider? smProvider_) {
    _smProvider = smProvider_;
    if (_smProvider != null) {
      // Call provider only when the provider is available.
      // Discard return value to avoid an unused-assignment warning.
      var q1 = _smProvider.GetQueryProvider(SMCategories.Processor);
      var ds = q1.Query("SELECT * FROM Win32_Processor");
    }
    init();
  }

  private void init() {
    try {
      ProcessorInfo = new ProcessorInfo
      {
        Vendor = NativeMethodGroup.Vendor(),
        Brand = NativeMethodGroup.Brand(),
        BaseSpeed = NativeMethodGroup.GetBaseSpeed(),
        SocketNum = NativeMethodGroup.GetSocketNum(),
        NumOfPhysicalCores = NativeMethodGroup.GetPhysicalCoreCount(),
        NumOfLogicalCores = NativeMethodGroup.GetLogicalCoreCount(),
        VirtualizationEnabled = NativeMethodGroup.VirtualizationEnabled(),
        Features = NativeMethodGroup.GetInstructionSetStruct()
      };
    }
    catch (Exception ex) {
      ProcessorInfo = null;
      Console.WriteLine(ex.Message);
    }

  }

  public string GetData(string key) {

    if (Data.ContainsKey(key))
      return Data[key].Item1;
    return string.Empty;
  }

  private string? _name = "Test Name";
  public string? Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private string? _description;
  public string? Description {
    get => _description;
    set => SetProperty(ref _description, value);
  }

  private ProcessorInfo? _processorInfo;
  public ProcessorInfo? ProcessorInfo {
    get => _processorInfo;
    set => SetProperty(ref _processorInfo, value);
  }
}
