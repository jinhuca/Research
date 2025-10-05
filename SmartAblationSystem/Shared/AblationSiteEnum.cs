
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared
{
  /// <summary>
  /// Ablation site  enumeration
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  /// <remarks> For backward compatibility and historical reason, we need to keep the enum value the same as before,
  /// but add priority for sorting them in different order 
  /// </remarks>>
  public enum AblationSiteEnum
  {
    [AblationSiteSettings(nameof(RSPV), 3)]
    RSPV = 0,

    [AblationSiteSettings(nameof(RIPV), 2)]
    RIPV = 1,

    [AblationSiteSettings(nameof(LSPV), 0)]
    LSPV = 2,

    [AblationSiteSettings(nameof(LIPV), 1)]
    LIPV = 3,

    [AblationSiteSettings(nameof(OTHER), 6, nameof(OTHER))]
    OTHER = 4,

    [AblationSiteSettings(nameof(UNKNOWN), 7, nameof(OTHER))]
    UNKNOWN = 5,

    [AblationSiteSettings(nameof(LCPV), 4)]
    LCPV = 6,

    [AblationSiteSettings(nameof(RMPV), 5)]
    RMPV = 7
  }

  public static class AblationSiteEnumHelper
  {
    public static IList<AblationSiteEnum> GetSortedAblationSiteEnums()
    {
      return Enum.GetValues(typeof(AblationSiteEnum))
        .OfType<AblationSiteEnum>()
        .Where(s => s != AblationSiteEnum.UNKNOWN)
        .OrderBy(GetEnumPriority) 
        .ToList();
    }

    public static IList<string> GetAblationSiteGroupNames()
    {
      return GetSortedAblationSiteEnums()
        .Select(s => s.GetGroupName())
        .Distinct()
        .ToList(); 
    } 

    public static string GetDescription(this AblationSiteEnum ablationSite)
    {
      var attribute = ablationSite.GetAttributeOfType<AblationSiteSettingsAttribute>(); 
      return attribute?.Description??string.Empty;
    }

    public static string GetGroupName(this AblationSiteEnum ablationSite)
    {
      var attribute = ablationSite.GetAttributeOfType<AblationSiteSettingsAttribute>();
      return attribute?.GroupName ?? ablationSite.ToString();
    }

    public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
    {
      var type = enumVal.GetType();
      var memInfo = type.GetMember(enumVal.ToString());
      var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
      return (attributes.Length > 0) ? (T)attributes[0] : null;
    }

    public static bool IsValidAblationSite(this AblationSiteEnum ablationSite)
    {
      return Enum.IsDefined(typeof(AblationSiteEnum), (int)ablationSite) && ablationSite != AblationSiteEnum.UNKNOWN;
    }

    private static int GetEnumPriority(AblationSiteEnum ablationSite)
    {
      var attribute = ablationSite.GetAttributeOfType<AblationSiteSettingsAttribute>();
      return attribute?.Priority ?? 0; 
    }
  }

}
