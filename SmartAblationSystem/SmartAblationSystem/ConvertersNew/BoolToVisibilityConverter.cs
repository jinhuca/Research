using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class BoolToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool value_)) throw new ArgumentException();
      var parameter_ = (string)parameter;

      if (parameter_ == "REVERSE")
      {
        return value_ ? Visibility.Collapsed : Visibility.Visible;
      }

      return value_ ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
