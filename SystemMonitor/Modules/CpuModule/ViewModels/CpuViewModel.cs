using CpuModule.Models;

namespace CpuModule.ViewModels;

public class CpuViewModel : BindableBase {
  private readonly CpuModel _model;
  public CpuViewModel(CpuModel model) {
    _model = model;
    initProperties();
  }

  private void initProperties() {
    //Name = _model.GetData(Win32_Processor.NameKey);
    Name = _model.BasicInfo?.Brand ?? string.Empty;
  }

  private string _name;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }
}
