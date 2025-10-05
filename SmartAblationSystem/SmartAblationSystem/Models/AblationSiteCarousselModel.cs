using System.Collections.Generic;
using Shared;

namespace SmartAblationSystem.Models
{
  public static class AblationSiteCarousselModel
  {
    private static AblationSiteEnum previousAblationSite;
    private static AblationSiteEnum currentAblationSite;

    private static IList<AblationSiteEnum> _orderedAblationSiteEnums = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
    private static int _maxAblationSiteIndex = _orderedAblationSiteEnums.Count - 1; 
    public static AblationSiteEnum PreviousAblationSite
    {
      get
      {
        return previousAblationSite;
      }
      set
      {
        previousAblationSite = value;
      }
    }


    public static AblationSiteEnum CurrentAblationSite
    {
      get
      {
        return currentAblationSite;
      }
      set
      {
        currentAblationSite = value;
      }
    }

    public static void MoveAblationSiteToTheLeft()
    {
      // Reverse traversal in _orderedAblationSiteEnums 
      var index = _orderedAblationSiteEnums.IndexOf(CurrentAblationSite);
      if (index >= 0)
      {
        index = index == 0 ? _maxAblationSiteIndex : --index; 
          
        CurrentAblationSite = _orderedAblationSiteEnums[index];
      }
      else
      {
        CurrentAblationSite = AblationSiteEnum.OTHER; 
      }
    }

    public static void MoveAblationSiteToTheRight()
    {
      // Forward traversal in _orderedAblationSiteEnums
      var index = _orderedAblationSiteEnums.IndexOf(CurrentAblationSite);
      if (index >= 0)
      {
        index = index == _maxAblationSiteIndex ? 0 : ++index;

        CurrentAblationSite = _orderedAblationSiteEnums[index];
      }
      else
      {
        CurrentAblationSite = AblationSiteEnum.OTHER;
      }
    }

  }
}
