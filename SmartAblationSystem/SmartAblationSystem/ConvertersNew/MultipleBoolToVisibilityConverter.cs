using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class MultipleBoolToVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(values.Any(v => !(v is bool)))
      {
        throw new ArgumentException(nameof(values));
      }

      return values.All(v=>v is bool value_ && value_) ? Visibility.Visible : Visibility.Hidden;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
