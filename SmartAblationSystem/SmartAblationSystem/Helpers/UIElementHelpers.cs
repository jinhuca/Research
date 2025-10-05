using System.Windows;
using System.Windows.Media;

namespace SmartAblationSystem.Helpers
{
  internal static class UIElementHelpers
  {
    public static T FindElementByName<T>(FrameworkElement element, string childName) where T : FrameworkElement
    {
      T childElement_ = null;
      var childCount_ = VisualTreeHelper.GetChildrenCount(element);
      for(int i = 0; i < childCount_; i++)
      {
        var child_ = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

        if(child_ == null)
          continue;

        if(child_ is T element_ && element_.Name.Equals(childName))
        {
          childElement_ = element_;
          break;
        }

        childElement_ = FindElementByName<T>(child_, childName);

        if(childElement_ != null)
          break;
      }
      return childElement_;
    }
  }
}
