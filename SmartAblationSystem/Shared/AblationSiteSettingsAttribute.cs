using System;

namespace Shared
{
  public class AblationSiteSettingsAttribute : Attribute
  {
    public AblationSiteSettingsAttribute(string description, int priority) : 
      this(description, priority, description)
    {
    }

    public AblationSiteSettingsAttribute(string description, int priority, string groupName)
    {
      Description = description;
      Priority = priority;
      GroupName = groupName;
    }

    public string Description { get; set; }
    public int Priority { get; set; }
    public string GroupName { get; set; }
  }
}
