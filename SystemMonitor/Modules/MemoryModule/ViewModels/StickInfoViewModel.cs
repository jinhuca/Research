using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.ViewModels;

public class StickInfoViewModel : BindableBase {
  public StickInfoViewModel() {

  }

  private int _id;
  public int Id {
    get => _id;
    set => SetProperty(ref _id, value);
  }

  private string _capacityInGB = string.Empty;
  public string CapacityInGB {
    get => _capacityInGB;
    set => SetProperty(ref _capacityInGB, value);
  }

  private string _speedInGHz = string.Empty;
  public string SpeedInGHz {
    get => _speedInGHz;
    set => SetProperty(ref _speedInGHz, value);
  }

  private string _formFactor = string.Empty;
  public string FormFactor {
    get => _formFactor;
    set => SetProperty(ref _formFactor, value);
  }
}
