using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace CustomControls.Converters
{
  public class IsValueGreaterThanConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        var baseValue = System.Convert.ToDouble(parameter);
        var doubleValue = System.Convert.ToDouble(value);

        return doubleValue > baseValue;
      }
      catch (Exception ex)
      {
        return false; 
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}
