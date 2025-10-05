using DataAccessLayer;
using System.Windows;
using System.Windows.Controls;
using static SmartAblationSystem.UIConstants;

namespace SmartAblationSystem.Selectors
{
  internal class SystemInfoDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if(container is FrameworkElement element_ && item is User user_)
      {
        DataTemplate dataTemplate_;

        if (user_.UserName == BSCUser || user_.UserName == BSCADMINUser)
        {
          dataTemplate_ = element_.FindResource(BSCDataTemplate) as DataTemplate; 
        }
        else
        {
          dataTemplate_ = element_.FindResource(DoctorDataTemplate) as DataTemplate;
        }
        return dataTemplate_;
      }

      return base.SelectTemplate(item, container);
    }
  }
}
