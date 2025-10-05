using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    [ValueConversion(typeof(object), typeof(string))]

    /// <summary>
    /// This class converts value to refrigerant unit (min/lbs)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class ShortToRefrigerantUnitConverter : IValueConverter
    {
        #region IValueConverter Members

        private const string UNIT_WEIGHT_LBS = @" lbs";
        private const string UNIT_WEIGHT_KG = @" Kg";
        private const string UNIT_TIME_MIN = @" min";
        private const string UNIT_ABLATION_REMAINING = @" Abl.";

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
          var unitString = UNIT_WEIGHT_LBS;

          if (value != null)
          {
            // To keep compatibility with old converter
            bool isNewUI = parameter?.ToString() == "NewUI";

            if (!isNewUI)
            {
              short _value = System.Convert.ToInt16(value);

              switch (_value)
              {
                case 1:
                  unitString = UNIT_TIME_MIN;
                  break;
                case 2:
                  unitString = UNIT_ABLATION_REMAINING;
                  break;
              }
            }

            if (isNewUI || unitString.Equals(UNIT_WEIGHT_LBS))
            {
              unitString = (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                              ? UNIT_WEIGHT_LBS
                              : UNIT_WEIGHT_KG;
            }
          }

          return unitString;
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
                var unitString = (string)value; 
                short result = 0;

                switch (unitString)
                {
                  case UNIT_TIME_MIN:
                    result = 1;
                    break;
                  case UNIT_ABLATION_REMAINING:
                    result = 2;
                    break;
                  case UNIT_WEIGHT_KG:
                  case UNIT_WEIGHT_LBS:
                    result = 0;
                    break;
                  default:
                    return Binding.DoNothing;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return Binding.DoNothing;
            }
        }

        #endregion IValueConverter Members
    }
}
