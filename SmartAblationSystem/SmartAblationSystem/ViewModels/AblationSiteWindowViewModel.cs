using System.Collections.Generic;
using Prism.Mvvm;
using Shared;

namespace SmartAblationSystem.ViewModels
{
  internal class AblationSiteWindowViewModel : BindableBase
  {
    public IList<AblationSiteEnum> AblationSiteList => AblationSiteEnumHelper.GetSortedAblationSiteEnums();

    private AblationSiteEnum _selectedAblationSite;
    public AblationSiteEnum SelectedAblationSite
    {
      get => _selectedAblationSite;
      set => SetProperty(ref _selectedAblationSite, value);
    }

    private bool _displayAblationSiteWarning;

    public bool DisplayAblationSiteWarning
    {
      get => _displayAblationSiteWarning;
      set => SetProperty(ref _displayAblationSiteWarning, value);
    }
  }
}