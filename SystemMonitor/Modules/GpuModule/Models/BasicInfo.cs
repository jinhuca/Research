namespace GpuModule.Models;

public class BasicInfo : BindableBase {
  public BasicInfo() {

  }

  private string _vendor = string.Empty;
  public string Vendor {
    get => _vendor;
    set => SetProperty(ref _vendor, value);
  }

  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }
}