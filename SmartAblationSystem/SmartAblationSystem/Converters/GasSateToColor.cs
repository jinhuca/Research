using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts a Gas State to a color
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(object), typeof(string))]
  internal class GasSateToColor : IValueConverter
  {
    #region IValueConverter Members

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
      Color normalColor_ = (Color)ColorConverter.ConvertFromString("#887cc2f2");

      switch ((int)value)
      {
        case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_LOW:

          return new SolidColorBrush(Colors.Yellow);

        case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_TOO_LOW:

          return new SolidColorBrush(Colors.Red);

        case (int)Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_OF_BOUNDS:

          return new SolidColorBrush(normalColor_);

        default:

          return new SolidColorBrush(normalColor_);
      }
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
      string strvalue = value as string;

      return System.Convert.ToInt32(strvalue);
    }

    #endregion IValueConverter Members
  }
}