using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  public class CryoTherapyViewPopIsOpenConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var isSimpleView = "SimpleView" == (string)parameter;

      return isSimpleView 
        ? (bool)values[0] && (bool)values[1]
        : (bool)values[0] && !(bool)values[1];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
