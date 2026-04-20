using CpuModule.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SystemManagementProvider.Constants;

namespace CpuModule.ViewModels;

public class CpuViewModel : BindableBase {
  private readonly CpuModel _model;
  public CpuViewModel() {
    
  }
  public CpuViewModel(CpuModel model) {
    _model = model;
    initProperties();
  }

  private void initProperties() {
    //Name = _model.GetData(Win32_Processor.NameKey);
    Name = "Test Name";
  }

  private string _name;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }
}
