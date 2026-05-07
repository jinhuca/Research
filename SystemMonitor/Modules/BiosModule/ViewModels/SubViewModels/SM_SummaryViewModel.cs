using System;
using System.Collections.Generic;
using System.Text;

namespace BiosModule.ViewModels.SubViewModels;

public class SM_SummaryViewModel : BindableBase, ISM_SummaryViewModel {
  private string _name = string.Empty;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private string _manufacturer = string.Empty;
  public string Manufacturer {
    get => _manufacturer;
    set => SetProperty(ref _manufacturer, value);
  }

  private string _serialNum = string.Empty;
  public string SerialNum {
    get => _serialNum;
    set => SetProperty(ref _serialNum, value);
  }

  private string _majorVersion = string.Empty;
  public string MajorVersion {
    get => _majorVersion;
    set => SetProperty(ref _majorVersion, value);
  }

  private string _minorVersion = string.Empty;
  public string MinorVersion {
    get => _minorVersion;
    set => SetProperty(ref _minorVersion, value);
  }
}
