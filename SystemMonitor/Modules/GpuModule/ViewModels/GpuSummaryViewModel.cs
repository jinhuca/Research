using GpuModule.Models;

namespace GpuModule.ViewModels;

public class GpuSummaryViewModel : BindableBase, IGpuSummaryViewModel {
  private GpuModel _gpuModel;
  public GpuSummaryViewModel(GpuModel gpuModel) {
    _gpuModel = gpuModel;
    //_gpuModel.PropertyChanged += (s, e) => {
    //  if (e.PropertyName == nameof(_gpuModel.Utilization)) {
    //    RaisePropertyChanged(nameof(Utilization));
    //  }
    //};
  }

  private string _ID = string.Empty;
  public string ID {
    get => _ID;
    set => SetProperty(ref _ID, value);
  }

  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private string _vendor = string.Empty;
  public string Vendor {
    get => _vendor;
    set => SetProperty(ref _vendor, value);
  }

  private string _type = string.Empty;
  public string Type {
    get => _type;
    set => SetProperty(ref _type, value);
  }

  private string _version = string.Empty;
  public string Version {
    get => _version;
    set => SetProperty(ref _version, value);
  }

  private string _ram = string.Empty;
  public string Ram {
    get => _ram;
    set => SetProperty(ref _ram, value);
  }
}