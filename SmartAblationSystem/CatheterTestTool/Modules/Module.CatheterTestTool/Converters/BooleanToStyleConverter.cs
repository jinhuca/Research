
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Module.CatheterTestTool.Converters
{
  public class BooleanToStyleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      bool trigger = (bool)values[0];
      var firstStyle = (Style)values[1];
      var secondStyle = (Style)values[2];

      return trigger ? firstStyle : secondStyle;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
