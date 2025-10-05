using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts required ablation time to int
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class RequiredAblationTimeConverter : IMultiValueConverter
    {
        int _RequiredAblationTimelValue = 30;
        int _RequiredAblationTimelLastValue = 30;
        bool _playBackBoolValue = false;
        string state = string.Empty;

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



            _RequiredAblationTimelValue = System.Convert.ToInt32(values[0]);
            _playBackBoolValue = System.Convert.ToBoolean(values[1]);
            state = values[2].ToString();

            if (!_playBackBoolValue && state != "CAN_ID_STATE_IDLE" && state != "CAN_ID_STATE_READY")
            {
                _RequiredAblationTimelLastValue = _RequiredAblationTimelValue;
                return _RequiredAblationTimelValue;
            }
            return
                _RequiredAblationTimelLastValue;


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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
