using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
  /// <summary>
  /// Class converts Nullable<bool> value to integer with Celsius Unit.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(double), typeof(string))]
  public class NullableBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool? result = null;
      if(!string.IsNullOrEmpty(value as string))
      {
	      if(bool.TryParse((string)value, out var parsedResult))
        {
          result = parsedResult;
        }
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
