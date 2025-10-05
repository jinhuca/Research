using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DevExpress.Charts.Native;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// Converts a value to a target type depending on the object received in parameter
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  class CoolingTimeTextConverter : IMultiValueConverter
  {
    // This converter displays - if the visibility would've been hidden, and the actual value if not.
    private readonly IValueConverter _visibilityConverter = new BooleanToVisibilityConverter();

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Visibility isVisible = (Visibility)_visibilityConverter.Convert(values[0], targetType, parameter, culture);
      if (isVisible == Visibility.Visible)
      {
        // returns temperature
        return values[1].ToString();
      } 
      else
      {
        return "--";
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }
}
