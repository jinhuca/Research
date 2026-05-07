using BiosModule.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiosModule.ViewModels; 
public interface IBiosViewModel {
  ISM_SummaryViewModel Summary { get; set; }
}
