using SmartAblationSystem.Models;
using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Catheter ID to name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    internal class CatheterIDToName : IValueConverter
    {
        #region IValueConverter Members
        const int ID28mm = (int)Helpers.Enumeration.CatheterType.ID28mm;
        const int plus = (int)Helpers.Enumeration.CatheterType.Plus;

        string[] catheterDescriptionID28mm = Languages.CatheterDescription[ID28mm].Split('-');
        string[] catheterDescriptionplus = Languages.CatheterDescription[plus].Split('-');

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

                if (parameter.ToString() == "CatheterDescriptionSplit")
                {
                    switch ((int)value)
                    {
                        case ID28mm:
                            return catheterDescriptionID28mm[0];

                        case plus:
                            return catheterDescriptionplus[0];

                        default:

                            return "";
                    }

                }

                if (parameter.ToString() == "CatheterDescriptionTM")
                {
                    switch ((int)value)
                    {
                        case ID28mm:
                            return catheterDescriptionID28mm[1];

                        case plus:
                            return catheterDescriptionplus[1];

                        default:

                            return "";
                    }

                }

                if (parameter.ToString() == "CatheterDescription")
                {
                    switch ((int)value)
                    {
                        case ID28mm:
                            return catheterDescriptionID28mm[2];

                        case plus:
                            return catheterDescriptionplus[2];

                        default:

                            return "";
                    }

                }

                if (parameter.ToString() == "PolarX")
                {
                    switch ((int)value)
                    {
                        case ID28mm:
                        case plus:
                            return "POLARx";

                        default:

                            return "";
                    }

                }

            }

            switch ((int)value)
            {
                case (int)Helpers.Enumeration.CatheterType.ID28mm:

                    return "22 mm";

                case (int)Helpers.Enumeration.CatheterType.Plus:

                    return "28 mm";

                default:

                    return "--";
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