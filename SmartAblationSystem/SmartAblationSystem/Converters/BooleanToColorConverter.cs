using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Boolean to a Color.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class BooleanToColorConverter : IValueConverter
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
            Color greenMetroColor = (Color)ColorConverter.ConvertFromString("#1e7145");

            Color color = (Color)ColorConverter.ConvertFromString("#FF00afef");

            if (parameter != null)
            {
                if (parameter.ToString() == "Mode")
                {
                    if (value is bool && (bool)value)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }
                    return new SolidColorBrush(Colors.Red);
                }

                else if (parameter.ToString() == "ALERT")
                {
                    if (value is bool && (bool)value)
                    {
                        return System.Windows.Media.Colors.Transparent;
                    }
                    return System.Windows.Media.Colors.Red;
                }

                else if (parameter.ToString() == "EsophagusTemperature" || parameter.ToString() == "DiaphragmAmplitudeThreshold")
                {
                    if (value is bool && (bool)value)
                    {
                        return new SolidColorBrush(Colors.Orange);
                    }

                    return new SolidColorBrush(color);
                }

                else if (parameter.ToString() == "AblationSite" )
                {
                    if (value is bool && (bool)value)
                    {
                        return new SolidColorBrush(Colors.Yellow);
                    }

                    return new SolidColorBrush(Colors.Transparent);
                }

                else if (parameter.ToString() == "SimulationICB")
                {
                    if (value is bool && (bool)value)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }

                    return new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                if (value is bool && (bool)value)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                return new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Transparent);
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
            return (value == Brushes.Green) ? true : false;
        }
    }
}