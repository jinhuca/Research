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

  public CpuModel(ISMProvider? smProvider_) {
    _smProvider = smProvider_;
    init();
  }

  private void init() {
    try {
      VendorName = NativeMethodGroup.Vendor();
      BrandName = NativeMethodGroup.Brand();

      BasicInfo = new BasicInfo {
        Vendor = NativeMethodGroup.Vendor(),
        Brand = NativeMethodGroup.Brand(),
        BaseSpeed = NativeMethodGroup.GetBaseSpeed(),
        SocketNum = NativeMethodGroup.GetSocketNum(),
        NumOfPhysicalCores = NativeMethodGroup.GetPhysicalCoreCount(),
        NumOfLogicalCores = NativeMethodGroup.GetLogicalCoreCount(),
        VirtualizationEnabled = NativeMethodGroup.VirtualizationEnabled(),
      };
      InstructionInfo = NativeMethodGroup.GetInstructionSetStruct();
    }
    catch (Exception ex) {
      BasicInfo = null;
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
  }

  private string? _vendor;
  public string? VendorName {
    get => _vendor;
    set => SetProperty(ref _vendor, value);
  }

  private string? _name = string.Empty;
  public string? BrandName {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private BasicInfo? _processorInfo;
  public BasicInfo? BasicInfo {
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
}
