using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace OsModule.Models; 
public class OperatingSystemModel : BindableBase, IOperatingSystemModel {
  private string _name;
  public string Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
