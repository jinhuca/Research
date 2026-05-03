using OsModule.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OsModule.ViewModels; 
public class OperatingSystemViewModel : BindableBase, IOperatingSystemViewModel {
  private readonly OperatingSystemModel _operatingSystemModel;

  public OperatingSystemViewModel(OperatingSystemModel operatingSystem) {
    _operatingSystemModel = operatingSystem;
    _operatingSystemModel.PropertyChanged += (s, e) => {
      if (e.PropertyName == nameof(_operatingSystemModel.Caption)) {
        RaisePropertyChanged(nameof(Caption));
      }
    };
    initProperties();
  }

  private void initProperties() {
    Caption = _operatingSystemModel.Caption ?? string.Empty;
  }

  [Required] private string _caption;
  public string Caption {
    get => _caption;
    set => SetProperty(ref _caption, value);
  }
}
