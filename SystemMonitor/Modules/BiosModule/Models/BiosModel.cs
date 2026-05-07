using BiosModule.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiosModule.Models;

public class BiosModel : BindableBase, IBiosModel {
  public BiosModel() {
    Init();
  }

  private SMSummary _smSummary = new();
  public SMSummary SM_Summary { 
    get => _smSummary; 
    set => SetProperty(ref _smSummary, value); 
  }

  private void Init() {
    SM_Summary = SMServices.GetBiosSerialNumber();
  }
}
