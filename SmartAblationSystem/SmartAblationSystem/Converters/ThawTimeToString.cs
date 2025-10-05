using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts thaw time to string time to target temperature and vein isolation duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class ThawTimeToStringTimeToTargetTemperatureAndVeinIsolationDuration : IMultiValueConverter
    {

        string TimeToThawTemperature = "-";
        bool IsThawTemperatureReached = false;
        string state = string.Empty;
        bool playBackBoolValue = false;


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
            try
            {
                TimeToThawTemperature = values.Length > 0 ? System.Convert.ToString(values[0]) : "-";

                if (TimeToThawTemperature != null && TimeToThawTemperature != string.Empty)
                {
                    if (System.Convert.ToInt16(TimeToThawTemperature) <= 0)
                    {
                        TimeToThawTemperature = "--";
                    }
                }


                IsThawTemperatureReached = values.Length > 1 ? System.Convert.ToBoolean(values[1]) : false;
                state = values.Length > 2 ? values[2].ToString() : string.Empty;
                playBackBoolValue = values.Length > 3 ? System.Convert.ToBoolean(values[3]) : false;

                if ((state == "CAN_ID_STATE_IDLE" || state == "CAN_ID_STATE_READY" || state == "CAN_ID_STATE_INFLATION") && !playBackBoolValue)
                {
                    return "-";
                }

                if (IsThawTemperatureReached)
                    return TimeToThawTemperature;

                return "-";
            }

            catch (Exception ex)
            {
                return "-";
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
