
using Shared;

namespace SmartAblationSystem.ViewModels
{
  public interface IAblationSiteAware
  {
    AblationSiteEnum AblationSite { get; set; }
    bool DisplayAblationSiteWarning { get; }

    void UpdateAblationSiteChanged(AblationSiteEnum newAblationSite); 
  }
}
