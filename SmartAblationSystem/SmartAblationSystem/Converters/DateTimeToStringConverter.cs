using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts DateTime to string converter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class DateTimeToStringConverter : IValueConverter
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
            DateTime dateTime;

            if (value is DateTime)
            {
                dateTime = (DateTime)value;

                if (parameter is string && (string)parameter == "BIRTHDATE")
                {
                    if (dateTime.Day == 15 && dateTime.Month == 6 && dateTime.Year == 1960)
                    {
                        if (Models.Languages.GuiFieldTranslation.ContainsKey("SelectADateLabel"))
                        {
                            return Models.Languages.GuiFieldTranslation["SelectADateLabel"];
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        return dateTime.ToString("MMM dd yyyy");
                    }
                }
                else
                {
                    return dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00");
                }
            }
            else if (value is string)
            {
                if ((string)parameter == "ErrorLogSearchDATE")
                {
                    if (value.ToString() == "")
                        return null ;
                    else
                        return value;

                }
            }

            return string.Empty;
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

            if ((string)parameter == "ErrorLogSearchDATE")
                return value;
            else
                return "";
        }

        #endregion IValueConverter Members
    }
}