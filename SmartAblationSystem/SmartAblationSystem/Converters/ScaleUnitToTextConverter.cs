using System;
using SmartAblationSystem.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts scale unit to text
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class ScaleUnitToTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                string tankType = string.Empty;

                if (parameter.ToString() == "TenPounds")
                {
                    if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                        return " 10 (lbs)";
                    else
                        return " 4.5  (kg)";
                }
                else if (parameter.ToString() == "FifteenPounds")
                {
                    if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                        return "BSC "; //15 (lbs)";
                    else
                        return "BSC ";  //" 6.8 (kg)";
                }

                if (parameter.ToString() == "NoParentheses")
                {
                  return Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs 
                    ? " lbs" 
                    : " Kg";
                }
            }

            if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                return " (lbs)";
            else
                return " (Kg)";

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

    }
}
