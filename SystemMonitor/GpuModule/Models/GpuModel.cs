using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace GpuModule.Models;

public class GpuModel : BindableBase, IGpuModel {
  public GpuModel() {
    
  }
  public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
