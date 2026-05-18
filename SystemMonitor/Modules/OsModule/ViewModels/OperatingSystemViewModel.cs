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
      if (e.PropertyName == nameof(_operatingSystemModel.BuildNumber)) {
        RaisePropertyChanged(nameof(BuildNumber));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.Version)) {
        RaisePropertyChanged(nameof(Version));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.Language)) {
        RaisePropertyChanged(nameof(Language));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.OSArchitecture)) {
        RaisePropertyChanged(nameof(OSArchitecture));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.CodeSet)) {
        RaisePropertyChanged(nameof(CodeSet));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.CSName)) {
        RaisePropertyChanged(nameof(CSName));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.TimeZone)) {
        RaisePropertyChanged(nameof(TimeZone));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.SerialNumber)) {
        RaisePropertyChanged(nameof(SerialNumber));
      }
      if (e.PropertyName == nameof(_operatingSystemModel.Locale)) {
        RaisePropertyChanged(nameof(Locale));
      }
    };
    initProperties();
  }

  private void initProperties() {
    Caption = _operatingSystemModel.Caption ?? string.Empty;
    BuildNumber = _operatingSystemModel.BuildNumber ?? string.Empty;
    Version = _operatingSystemModel.Version ?? string.Empty;
    Language = _operatingSystemModel.Language ?? string.Empty;
    OSArchitecture = _operatingSystemModel.OSArchitecture ?? string.Empty;
  }

  public string Caption {
    get => _operatingSystemModel.Caption ?? string.Empty;
    set => _operatingSystemModel.Caption = value;
  }
  public string BuildNumber {
    get => _operatingSystemModel.BuildNumber ?? string.Empty;
    set => _operatingSystemModel.BuildNumber = value;
  }
  public string Version {
    get => _operatingSystemModel.Version ?? string.Empty;
    set => _operatingSystemModel.Version = value;
  }
  public string Language {
    get => _operatingSystemModel.Language ?? string.Empty;
    set => _operatingSystemModel.Language = value;
  }
  public string OSArchitecture {
    get => _operatingSystemModel.OSArchitecture ?? string.Empty;
    set => _operatingSystemModel.OSArchitecture = value;
  }
  public string CodeSet {
    get => _operatingSystemModel.CodeSet ?? string.Empty;
    set => _operatingSystemModel.CodeSet = value;
  }
  public string CSName {
    get => _operatingSystemModel.CSName ?? string.Empty;
    set => _operatingSystemModel.CSName = value;
  }
  public string TimeZone {
    get => _operatingSystemModel.TimeZone ?? string.Empty;
    set => _operatingSystemModel.TimeZone = value;
  }
  public string SerialNumber {
    get => _operatingSystemModel.SerialNumber ?? string.Empty;
    set => _operatingSystemModel.SerialNumber = value;
  }
  public string Locale {
    get => _operatingSystemModel.Locale ?? string.Empty;
    set => _operatingSystemModel.Locale = value;
  }
}
