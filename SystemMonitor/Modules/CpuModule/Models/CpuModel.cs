using System.Collections.Specialized;
using System.Windows.Navigation;
using SystemManagementProvider;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace CpuModule.Models;

public class CpuModel : BindableBase, ICpuModel {
  public Dictionary<string, (string, string)> Data = new();
  private readonly ISMProvider? _smProvider;

  public event NotifyCollectionChangedEventHandler? CollectionChanged;
  public CpuModel() {
    
  }

  public CpuModel(ISMProvider? smProvider_) {
    _smProvider = smProvider_;
    ISMQuery provider_ = _smProvider.GetQueryProvider(SMCategories.Processor);
  }

  public string GetData(string key) {
    if (Data.ContainsKey(key))
      return Data[key].Item1;
    return string.Empty;
  }

  private string? _name ="Test Name";
  public string? Name {
    get => _name;
    set => SetProperty(ref _name, value);
  }

  private string? _description;
  public string? Description {
    get => _description; 
    set => SetProperty(ref _description, value);
  }

}
