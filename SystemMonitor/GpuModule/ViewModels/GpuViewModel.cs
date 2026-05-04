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
    foreach (var gpuInfo in _model.GpuInfoList) {
      var gpuName = gpuInfo.Key;
      var gpuDetails = gpuInfo.Value;

      if(gpuName!= null) {
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
        if (gpuDetails.TryGetValue(ViewModelDefinitions.AdapterDACType, out var dacType_)) {
          GpuSummaryList[gpuName][ViewModelDefinitions.AdapterDACType] = dacType_.Item1;
        }
      }

      // Process the GPU details as needed
      // For example, you can extract specific information like vendor, memory, etc.
      if (gpuDetails.TryGetValue(ViewModelDefinitions.CaptionString, out var captionInfo)) {
        var caption = captionInfo.Item1; // Assuming the first item is the caption
        // Do something with the caption information
      }
      if (gpuDetails.TryGetValue(ViewModelDefinitions.AdapterRamString, out var memoryInfo)) {
        var memoryBytes = memoryInfo.Item1; // Assuming the first item is the memory size in bytes
        // Convert memoryBytes to a more readable format if needed
      }
      // Add more processing as required for other GPU details

    }
  }

  private void updateProperties() {
    //if (_model.BrandName != BrandName) {
    //  BrandName = _model.BrandName ?? string.Empty;
    //}
    //if (_model.Summary != Summary) {
    //  Summary = _model.Summary ?? new Dictionary<string, BasicInfo>();
    //}
  }

  //public string BrandName { 
  //  get => throw new NotImplementedException(); 
  //  set => throw new NotImplementedException(); 
  //}

  //public Dictionary<string, BasicInfo> Summary {
  //  get => throw new NotImplementedException();
  //  set => throw new NotImplementedException();
  //}

  private Dictionary<string, Dictionary<string, string>> _gpuSummaryList = new();
  public Dictionary<string, Dictionary<string, string>> GpuSummaryList {
    get => _gpuSummaryList;
    set => SetProperty(ref _gpuSummaryList, value);
  }
}
