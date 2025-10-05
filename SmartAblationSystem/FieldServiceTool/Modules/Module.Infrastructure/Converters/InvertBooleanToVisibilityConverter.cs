using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Module.Infrastructure
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class InvertBooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool boolValue)) return Visibility.Visible;

      return boolValue ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Visibility visibility)) return false;

      return visibility != Visibility.Visible;
    }
  }
}
