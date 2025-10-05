using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Boolean To a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class BooleanToStringConverter : IValueConverter
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
            if (parameter != null)
            {
                if (parameter.ToString() == "TipOrBalloonPressure")
                {
                    if (value is bool && (bool)value)
                    {
                        return "TIP PRESSURE";
                    }
                    return "BALLOON PRESSURE";
                }
                else if (parameter.ToString() == "TipOrBalloonPressureUnit")
                {
                    if (value is bool && (bool)value)
                    {
                        return "mmHg";
                    }
                    return "PSI";
                }
                else if (parameter.ToString() == "DiaphragmMovementUnit")
                {
                    if (value is bool && (bool)value)
                    {
                        return "%";
                    }
                    return "G";
                }
                else if (parameter.ToString() == "AddEditUser")
                {
                    if (value is bool && (bool)value)
                    {
                        return "Add User";
                    }
                    return "Edit User";
                }
                else if (parameter.ToString() == "BoxCheckedUnchecked")
                {
                    if (value is bool && (bool)value)
                    {
                        return "/Images/BoxImageChecked.png";
                    }
                    return "/Images/BoxImageUnchecked.png";
                }
                else if (parameter.ToString() == "TANK_GO")
                {
                    if (value is bool && (bool)value)
                    {
                        return "/Images/Tank_go.png";
                    }
                    return "/Images/Tank_stop.png";
                }
            }
            else
            {
                if (value is bool && (bool)value)
                {
                    return "Disconnect";
                }
                return "Connect";
            }

            return "";
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