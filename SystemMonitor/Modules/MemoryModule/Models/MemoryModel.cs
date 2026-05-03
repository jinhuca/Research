using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MemoryModule.Models; 
public class MemoryModel : BindableBase, IMemoryModel {
  public MemoryModel() {
    
  }

  public string Name { get; set; }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
