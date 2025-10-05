using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Module.SystemParameters
{
  /// <summary>
  /// Class converts float value to string with sccm unit.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(double), typeof(string))]
  public class DoubleToSccmConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string result = string.Empty;

      if(value is double doubleValue)
      {
        result = $"{doubleValue:0.#} sccm";
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
