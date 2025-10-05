using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
  /// <summary>
  /// Class converts float into string with value and Percentage.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(double), typeof(string))]
  public class DoubleToPercentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string result = string.Empty;
      if (value is double doubleValue)
      {
        result = $"{doubleValue:0.#} %";
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
