using GpuModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GpuModule.ViewModels;

public class GpuViewModel : BindableBase, IGpuViewModel {
  private readonly GpuModel _model;

  public GpuViewModel(GpuModel model) {
    _model = model;
  }

  public string BrandName { 
    get => throw new NotImplementedException(); 
    set => throw new NotImplementedException(); 
  }
}
