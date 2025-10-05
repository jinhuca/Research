using DataAccessLayer;
using SmartAblationSystem.Helpers;
using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Tank to a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class TankToStringConverter : IValueConverter
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
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Tank tank = null;
            string param = "";

            if (value is Tank)
            {
                tank = (Tank)value;
                param = (string)parameter;

                if (tank != null && !string.IsNullOrWhiteSpace(param))
                {
                    if (param == "CHANGE_DATE")
                    {
                        return tank.ReplacementDate.ToString("MMMM dd, yyyy");
                    }
                    else if (param == "WEIGHT_AT_CHANGE")
                    {

                        if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                            return tank.WeightAtReplacementDate.ToString("0.0");
                        else
                            return Scale.ConvertLbToKg(System.Convert.ToDouble(tank.WeightAtReplacementDate)).ToString("0.0");           
                    }
                }
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
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return value;
            }
        }

        #endregion IValueConverter Members
    }
}