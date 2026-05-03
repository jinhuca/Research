using OsModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsModule.ViewModels; 
public class OperatingSystemViewModel : BindableBase, IOperatingSystemViewModel {
  private readonly OperatingSystemModel _operatingSystem;

  public OperatingSystemViewModel(OperatingSystemModel operatingSystem) {
    _operatingSystem = operatingSystem;
  }

  private string _name;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }
}
