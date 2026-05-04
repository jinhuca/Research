using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace GpuModule.Models;

public class GpuModel : BindableBase, IGpuModel {
  private readonly ISMProvider? _smProvider;
  public Dictionary<string, Dictionary<string, (string, string)>> GpuInfoList { get; private set; }
  = new Dictionary<string, Dictionary<string, (string, string)>>();

  public GpuModel(ISMProvider? smProvider = null) {
    _smProvider = smProvider;
    Init();
  }

  private void Init() {
    try {
      ISMQuery? gpuQuery_ = _smProvider?.GetQueryProvider(SMCategories.Gpu);
      GpuInfoList = gpuQuery_?.QueryMultiple("SELECT * FROM Win32_VideoController") ?? [];

      //var temp =gpuQuery_.QueryMultiple("SELECT * FROM Win32_VideoController").Values;
    }
    catch (Exception ex) {
      // Handle exceptions gracefully, perhaps log them
      Name = "Unknown GPU";
    }
  }

  private string _name;
  public string Name {
    get => _name; 
    set => SetProperty(ref _name, value);
  }

  private BasicInfo _basicInfo = new BasicInfo();
  public BasicInfo BasicInfo {
    get => _basicInfo;
    set => SetProperty(ref _basicInfo, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
