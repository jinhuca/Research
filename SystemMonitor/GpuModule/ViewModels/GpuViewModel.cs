using Converters;
using GpuModule.Models;
using GpuModule.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static GpuModule.Views.ViewDefinitions;

namespace GpuModule.ViewModels;

public class GpuViewModel : BindableBase, IGpuViewModel {
  private readonly GpuModel _model;

  public GpuViewModel(GpuModel model) {
    _model = model;
    _model.PropertyChanged += (s, e) => {
      //if (e.PropertyName == nameof(_model.BrandName) || e.PropertyName == nameof(_model.Summary)) {
      updateProperties();
      //}
    };

    initProperties();

  }

  private void initProperties() {
    foreach(var gpuInfo in _model.GpuInfoList) {
      var gpuName = gpuInfo.Key;
      var gpuDetails = gpuInfo.Value;

      if(gpuName != null) {
        GpuSummaryList[gpuName] = new Dictionary<string, string>();
        if(gpuDetails.TryGetValue(ViewModelDefinitions.CaptionString, out var caption_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.CaptionString] = caption_.Item1;
        }
        if(gpuDetails.TryGetValue(ViewModelDefinitions.AdapterRamString, out var ram_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.AdapterRamString] = ram_.Item1;
        }
        if(gpuDetails.TryGetValue(ViewModelDefinitions.AdapterCompatibility, out var compatibility_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.AdapterCompatibility] = compatibility_.Item1;
        }
        if(gpuDetails.TryGetValue(ViewModelDefinitions.DeviceId, out var deviceId_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.DeviceId] = deviceId_.Item1;
        }
        if(gpuDetails.TryGetValue(ViewModelDefinitions.DriverVersion, out var driverVersion_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.DriverVersion] = driverVersion_.Item1;
        }
        if(gpuDetails.TryGetValue(ViewModelDefinitions.AdapterDACType, out var dacType_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.AdapterDACType] = dacType_.Item1;
        }

        // Add the GPU name to the list
        GpuNameList.Add(gpuName);
      }
    }
    InitializeSummary();
  }

  private void InitializeSummary() {
    foreach(var gpuName in GpuNameList) {
      if(string.IsNullOrEmpty(gpuName)) continue;

      var g1_ = GpuSummaryList[gpuName];
      if(g1_.TryGetValue(ViewModelDefinitions.AdapterDACType, out var type_)) {
        if(type_.Contains(ViewModelDefinitions.InternalGpuType)) {
          InternalGpuSummaryViewModel.ID = g1_[ViewModelDefinitions.DeviceId];
          InternalGpuSummaryViewModel.Name = g1_[ViewModelDefinitions.CaptionString].Replace("(R)", "");
          InternalGpuSummaryViewModel.Vendor = g1_[ViewModelDefinitions.AdapterCompatibility];
          InternalGpuSummaryViewModel.Type = ViewModelDefinitions.InternalGpuType;
          InternalGpuSummaryViewModel.Version = g1_[ViewModelDefinitions.DriverVersion];
          InternalGpuSummaryViewModel.Ram = ByteUnitConverters.ConvertBytesToReadableUnit(long.Parse(g1_[ViewModelDefinitions.AdapterRamString]));
        }
        if(type_.Contains(ViewModelDefinitions.DedicatedGpuType)) {
          DedicatedGpuSummaryViewModel.ID = g1_[ViewModelDefinitions.DeviceId];
          DedicatedGpuSummaryViewModel.Name = g1_[ViewModelDefinitions.CaptionString];
          DedicatedGpuSummaryViewModel.Vendor = g1_[ViewModelDefinitions.AdapterCompatibility];
          DedicatedGpuSummaryViewModel.Type = ViewModelDefinitions.DedicatedGpuType;
          DedicatedGpuSummaryViewModel.Version = g1_[ViewModelDefinitions.DriverVersion];
          DedicatedGpuSummaryViewModel.Ram = ByteUnitConverters.ConvertBytesToReadableUnit(long.Parse(g1_[ViewModelDefinitions.AdapterRamString]));
        }
      }
    }
  }

  private void updateProperties() {
    //if (_model.BrandName != BrandName) {
    //  BrandName = _model.BrandName ?? string.Empty;
    //}
    //if (_model.Summary != Summary) {
    //  Summary = _model.Summary ?? new Dictionary<string, BasicInfo>();
    //}
    RaisePropertyChanged(nameof(InternalGpuSummaryViewModel));
    RaisePropertyChanged(nameof(DedicatedGpuSummaryViewModel));
  }

  private Dictionary<string, Dictionary<string, string>> _gpuSummaryList = new();
  public Dictionary<string, Dictionary<string, string>> GpuSummaryList {
    get => _gpuSummaryList;
    set => SetProperty(ref _gpuSummaryList, value);
  }

  private List<string> _gpuNameList = new();
  public List<string> GpuNameList {
    get => _gpuNameList;
    set => SetProperty(ref _gpuNameList, value);
  }

  private IGpuSummaryViewModel _integratedGpuSummaryViewModel = new GpuSummaryViewModel();
  public IGpuSummaryViewModel InternalGpuSummaryViewModel {
    get => _integratedGpuSummaryViewModel;
    set => SetProperty(ref _integratedGpuSummaryViewModel, value);
  }

  private IGpuSummaryViewModel _dedicatedGpuSummaryViewModel = new GpuSummaryViewModel();
  public IGpuSummaryViewModel DedicatedGpuSummaryViewModel {
    get => _dedicatedGpuSummaryViewModel;
    set => SetProperty(ref _dedicatedGpuSummaryViewModel, value);
  }
}
