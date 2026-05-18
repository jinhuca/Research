namespace MemoryModule.ViewModels;

public interface IMemoryViewModel {
  string TotalMemorySize { get; }
  string AvailableMemorySize { get; }
  int UsedSlotNum { get; }
  //List<IStickInfo> StickInfo { get; }
  List<StickInfoViewModel> StickViewModel { get; }
}
