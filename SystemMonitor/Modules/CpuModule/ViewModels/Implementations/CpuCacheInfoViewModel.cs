namespace CpuModule.ViewModels.Implementations;

public class CpuCacheInfoViewModel : BindableBase {
  private string _L1_Cache_size = string.Empty;
  public string L1_Cache_size {
    get => _L1_Cache_size;
    set => SetProperty(ref _L1_Cache_size, value);
  }

  private string _L2_Cache_size = string.Empty;
  public string L2_Cache_size {
    get => _L2_Cache_size;
    set => SetProperty(ref _L2_Cache_size, value);
  }

  private string _L3_Cache_size = string.Empty;
  public string L3_Cache_size {
    get => _L3_Cache_size;
    set => SetProperty(ref _L3_Cache_size, value);
  }

  private string _L1_Cache_Line_size = string.Empty;
  public string L1_Cache_Line_size {
    get => _L1_Cache_Line_size;
    set => SetProperty(ref _L1_Cache_Line_size, value);
  }

  private string _L2_Cache_Line_size = string.Empty;
  public string L2_Cache_Line_size {
    get => _L2_Cache_Line_size;
    set => SetProperty(ref _L2_Cache_Line_size, value);
  }

  private string _L3_Cache_Line_size = string.Empty;
  public string L3_Cache_Line_size {
    get => _L3_Cache_Line_size;
    set => SetProperty(ref _L3_Cache_Line_size, value);
  }
}
