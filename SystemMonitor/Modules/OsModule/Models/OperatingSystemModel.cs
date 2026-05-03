using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace OsModule.Models; 
public class OperatingSystemModel : BindableBase, IOperatingSystemModel {
  private readonly ISMProvider? _smProvider;

  public OperatingSystemModel(ISMProvider? smProvider) {
    _smProvider = smProvider;
    //ISMQuery osQuery_ = _smProvider.GetQueryProvider(SMCategories.OperatingSystem);
    //var osInfoDict = osQuery_.Query(Win32_OperatingSystem.QueryString);
    fetchSystemInfo(null);
  }

  private void fetchSystemInfo(object? state) {
    ISMQuery? osQuery_ = _smProvider?.GetQueryProvider(SMCategories.OperatingSystem);
    var osInfoDict = osQuery_?.Query(Win32_OperatingSystem.QueryString);
    if (osInfoDict != null) {
      Caption = osInfoDict[Win32_OperatingSystem.CaptionKey].Item1;
    }
  }

  private string _caption = string.Empty;
  public string Caption {
    get => _caption;
    set => SetProperty(ref _caption, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
