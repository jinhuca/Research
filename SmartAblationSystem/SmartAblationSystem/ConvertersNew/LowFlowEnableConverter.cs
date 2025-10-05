using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class LowFlowEnableConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Any(v => !(v is bool)))
      {
        throw new ArgumentException(nameof(values));
      }

      return (bool)values[0] && (bool)values[1] && !(bool)values[2];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
