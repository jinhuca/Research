using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{

    /// <summary>
    /// This class converts boolean to the opacity
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(bool), typeof(double))]
    public class BooleanToOpacityConverter : IValueConverter
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
            if (parameter != null)
            {
                if (parameter.ToString() == "TTILOGIC" || parameter.ToString() == "AblationSite")
                {
                    if (value is bool && (bool)value)
                    {
                        return 1;
                    }
                    return 0.5;
                }

                if (parameter.ToString() == "DAS")
                {
                    if (value is bool && (bool)value)
                    {
                        return 1;
                    }
                    return 0.6;
                }

                if (parameter.ToString() == "BootLoader")
                {
                    if (value is bool && (bool)value)
                    {
                        return 0.5;
                    }
                    return 1;
                }

                if (parameter.ToString() == "BootLoaderCancel")
                {
                    if (value is bool && (bool)value)
                    {
                        return 1;
                    }
                    return 0.5;
                }

            }

            if (value is Boolean && (bool)value)
            {
                if (parameter is double && (double)parameter >= 0)
                {
                    return (double)parameter;
                }
                else
                {
                    return 1;
                }                
            }
            return 0;
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
