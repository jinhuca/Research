using System;
using System.Windows.Data;

namespace Module.Infrastructure
{
  /// <summary>
  /// This class converts a Boolean to USB Connection path
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(bool), typeof(string))]
  public class BooleanToUSBConnectionConverter : IValueConverter
  {
    /// <summary>
    /// Converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool && (bool)value)
      {
        return "/Images/USBDriveConnected.png";
      }
      return "/Images/USBDriveNotConnected.png";
    }

    /// <summary>
    /// Converts back an object to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert back.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}
