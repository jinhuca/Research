using Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace MemoryModule.Models;

public class MemoryModel : BindableBase, IMemoryModel {
  public MemoryModel() { 
    init();
  }

  private void init() {
    ManagementObjectSearcher searcher_ = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    foreach (ManagementObject wmi_ in searcher_.Get()) {
      var cp1_ = long.Parse(wmi_["Capacity"].ToString());
      var cp_ = ByteUnitConverters.ConvertBytesToReadableUnit(cp1_);
      Debug.WriteLine("Capacity: " + cp_);

      var speedInMHz = double.Parse(wmi_["Speed"].ToString());
      Debug.WriteLine("Raw Speed: " + wmi_["Speed"] + " MT/s");
      //var speedInGHz = HzUnitConverter.ConvertMHzToReadableUnit(speedInMHz);
      //Debug.WriteLine("Speed: " + speedInGHz + " MHz");
    }
  }

  private string _name;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private double _capacity;
  public double Capacity {
    get => _capacity;
    set => SetProperty(ref _capacity, value);
  }

  private double _speed;
  public double Speed {
    get => _speed;
    set => SetProperty(ref _speed, value);
  }
}
