using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an integer to duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class IntToDurationConverter : IValueConverter
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
        public object Convert(object value, Type targetType, object parameterMinValue, System.Globalization.CultureInfo culture)
        {
            string valueToConvert = string.Empty;
            int aValue = -1;
            int parameterValue = -1;

            if (value != null && value is int)
            {
                aValue = (int)value;

                if (parameterMinValue != null && parameterMinValue is string)
                {
                    parameterValue = Int32.Parse((string)parameterMinValue);

                    if (aValue == parameterValue)
                    {
                        valueToConvert = "--";
                    }
                    else
                    {
                        valueToConvert = aValue.ToString() ;
                    }
                }
            }

            return valueToConvert;
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
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                //TO DO
                ex.ToString();
                return value;
            }
        }

        #endregion IValueConverter Members
    }
}