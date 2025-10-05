using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class DateTimeAndBooleanToStringConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        DateTime birthdate = new DateTime(1800, 1, 1);
        bool isDateSelected = false;

        /// <summary>
        /// Converts a value to a target type depending on the object received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (values[0] is DateTime)
         {
                birthdate = (DateTime)values[0];
                isDateSelected = System.Convert.ToBoolean(values[1]);

                if (parameter is string && (string)parameter == "BIRTHDATE")
                {
                    if (birthdate.Day == 15 && birthdate.Month == 6 && birthdate.Year == 1960 && isDateSelected == false)
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
                        if (birthdate.Day == 1 && birthdate.Month == 1 && birthdate.Year == 1800)
                        {
                            return Models.Languages.GuiFieldTranslation["SelectADateLabel"];
                        }
                        else
                        {
                            return birthdate.ToString("MMM dd yyyy");
                        }
                    }
                }
                else
                {
                    return birthdate.Hour.ToString("00") + ":" + birthdate.Minute.ToString("00");
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
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotSupportedException("Cannot convert back");
        }

        #endregion IValueConverter Members
    }
}
