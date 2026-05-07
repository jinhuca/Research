 using BiosModule.Models;
using BiosModule.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiosModule.ViewModels; 
public class BiosViewModel : BindableBase, IBiosViewModel {
  private readonly BiosModel _model;
  public BiosViewModel(BiosModel model) {
    _model = model;
    _model.PropertyChanged += (s, e) => {
    };
    init();
  }

  private void init() {
    var smSummary = _model.SM_Summary;
    Summary = new SM_SummaryViewModel {
      Name = smSummary.Name,
      Manufacturer = smSummary.Manufacturer,
      SerialNum = smSummary.SerialNumber,
      MajorVersion = smSummary.SMBIOSMajorVersion.ToString(),
      MinorVersion = smSummary.SMBIOSMinorVersion.ToString()
    };
  }

  private ISM_SummaryViewModel _summary;
  public ISM_SummaryViewModel Summary { 
    get => _summary; 
    set => SetProperty(ref _summary, value); 
  }
}
