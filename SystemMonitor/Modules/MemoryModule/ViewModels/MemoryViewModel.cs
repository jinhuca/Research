using Converters;
using MemoryModule.Models;
using System.ComponentModel;

namespace MemoryModule.ViewModels; 
public class MemoryViewModel : BindableBase, IMemoryViewModel {
  private readonly MemoryModel _model;

  public MemoryViewModel(MemoryModel model) {
    _model = model;
  }

  /*
  private void _model_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
    switch(e.PropertyName) {
      case nameof(_model.TotalInstalledMemory):
        RaisePropertyChanged(nameof(TotalMemorySize));
        break;
      case nameof(_model.AvailableMemory):
        RaisePropertyChanged(nameof(AvailableMemorySize));
        break;
      case nameof(_model.SlotsUsed):
        RaisePropertyChanged(nameof(UsedSlotNum));
        break;
      case nameof(_model.RAMStickInfo):
        StickViewModel.Clear();
        int index_ = 1;
        foreach(var stick_ in _model.RAMStickInfo) {
          StickInfoViewModel svm_ = new();
          svm_.Id = index_++;
          svm_.CapacityInGB = ByteUnitConverters.ConvertBytesToReadableUnit(stick_.Capacity);
          svm_.FormFactor = stick_.FormFactor;
          svm_.SpeedInGHz = HzUnitConverter.ConvertMHzToReadableUnit(stick_.Speed / (1000_000));
          StickViewModel.Add(svm_);
        }
        RaisePropertyChanged(nameof(StickViewModel));
        break;
    }
  }
  */
  public string TotalMemorySize {
    get => ByteUnitConverters.ConvertBytesToReadableUnit(_model.TotalInstalledMemory);
  }

  public string AvailableMemorySize {
    get => ByteUnitConverters.ConvertBytesToReadableUnit(_model.AvailableMemory);
  }

  public int UsedSlotNum {
    get => _model.SlotsUsed;
  }

  private List<StickInfoViewModel> _stickViewModel = new();
  public List<StickInfoViewModel> StickViewModel {
    get {
      _stickViewModel.Clear();
      int index_ = 1;
      foreach(var stick_ in _model.RAMStickInfo) {
        StickInfoViewModel svm_ = new();
        svm_.Id = index_++;
        svm_.CapacityInGB = ByteUnitConverters.ConvertBytesToReadableUnit(stick_.Capacity);
        svm_.FormFactor = stick_.FormFactor;
        svm_.SpeedInGHz = HzUnitConverter.ConvertMHzToReadableUnit(stick_.Speed);
        _stickViewModel.Add(svm_);
      }
      return _stickViewModel;
    }
  }

}
