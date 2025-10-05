using DataAccessLayer;
using SmartAblationSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an Field value to text
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class FieldToTextConverter : IValueConverter
    {

        /// <summary>
        /// Initializes GuiField Translation if it has not been setup yet
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FieldToTextConverter()
        {
            if (!Languages.GuiFieldTranslationInitialized)
            {
                Languages.InitializeGuiFieldTranslation();
                Languages.GuiFieldTranslationInitialized = true; 
            }
        }

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
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueToBeConverted = value.ToString();

            if (Languages.GuiFieldTranslation.ContainsKey(valueToBeConverted))
            {
                if ((string)parameter == "CAPS")
                {
                    return Languages.GuiFieldTranslation[valueToBeConverted].ToUpper();
                }
                else if ((string)parameter == "TITLECASE")
                {
                    //Convert string like "summary report" to "Summary Report"
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Languages.GuiFieldTranslation[valueToBeConverted].ToLower());
                }
                else
                {
                    return Languages.GuiFieldTranslation[valueToBeConverted];
                }
            }

            return value;
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
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return "";
        }

        #endregion IValueConverter Members
    }
}
