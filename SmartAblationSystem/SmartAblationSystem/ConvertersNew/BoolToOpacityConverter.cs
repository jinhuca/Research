using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class BoolToOpacityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(!(value is bool v_)) return 1;
      return v_ ? 0.3 : 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
