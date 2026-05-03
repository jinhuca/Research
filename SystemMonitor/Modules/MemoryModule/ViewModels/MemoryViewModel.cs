using MemoryModule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.ViewModels; 
public class MemoryViewModel : BindableBase, IMemoryViewModel {
  private readonly MemoryModel _model;

  public MemoryViewModel(MemoryModel model) {
    _model = model;
  }

  public string Name { get; set; }
}
