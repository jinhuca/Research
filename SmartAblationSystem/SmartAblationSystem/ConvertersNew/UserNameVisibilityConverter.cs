using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class UserNameVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var name_ = (string)value;
      return string.IsNullOrEmpty(name_) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
