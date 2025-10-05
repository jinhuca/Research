using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// Converts a value to a target type depending on the object received in parameter
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  class BooleanAndStateToTextValueConverter : IMultiValueConverter
  {
    // This converter displays - if the visibility would've been hidden, and the actual value if not.
    private readonly IMultiValueConverter _visibilityConverter = new BooleanAndStateToVisibilityConverter();

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {


      if (!(values[0] is bool isCatheterCableConnected))
      {
        throw new ArgumentException("Invalid argument");
      }
      if (values[1] is null)
      {
        throw new ArgumentException("Invalid argument System State");
      }
      if (values[2] is null)
      {
        // is a boolean but needs to become an object
        throw new ArgumentException("Invalid argument ");
      }
      if (!(values[3] is double maxTemperatureRate))
      {
        throw new ArgumentException("Invalid argument");
      }

      Visibility isVisible = (Visibility)_visibilityConverter.Convert(new[] { isCatheterCableConnected, values[1], values[2] }, targetType, parameter, culture);
      if (isVisible == Visibility.Visible)
      {
        // returns temperature
        return maxTemperatureRate.ToString();
      } 
      else
      {
        return "-";
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }
}
