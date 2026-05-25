namespace MemoryModule.Models;

public class MemoryModel : BindableBase, IMemoryModel {
  public MemoryModel() {
    init();
  }

  private void init() {
    RAMStickInfo = QueryMemory.GetStickInfo();
    TotalInstalledMemory = QueryMemory.GetInstalledMemorySize();
    AvailableMemory = QueryMemory.GetOSVisibleRAMSize();

    SlotsUsed = QueryMemory.GetSlotsUsed();
  }

  private ulong _totalInstalledMemory;
  public ulong TotalInstalledMemory {
    get => _totalInstalledMemory;
    set => SetProperty(ref _totalInstalledMemory, value);
  }

  private ulong _availableMemory;
  public ulong AvailableMemory {
    get => _availableMemory;
    set => SetProperty(ref _availableMemory, value);
  }

  private int _slotsUsed;
  public int SlotsUsed {
    get => _slotsUsed;
    set => SetProperty(ref _slotsUsed, value);
  }

  private uint _totalSlots;
  public uint TotalSlots {
    get => _totalSlots;
    set => SetProperty(ref _totalSlots, value);
  }

  private ulong _hardwareReservedMemory;
  public ulong HardwareReservedMemory {
    get => _hardwareReservedMemory;
    set => SetProperty(ref _hardwareReservedMemory, value);
  }

  private List<IStickInfo> _ramStickInfo = new List<IStickInfo>();
  public List<IStickInfo> RAMStickInfo {
    get => _ramStickInfo;
    set => SetProperty(ref _ramStickInfo, value);
  }
}