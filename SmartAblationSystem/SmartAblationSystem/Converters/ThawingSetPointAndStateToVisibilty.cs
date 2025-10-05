using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    public class ThawingSetPointAndStateToVisibilty : IMultiValueConverter
    {

        bool IsThawingTemperatureSetPointReached = false;
        int SystemState = (int)MessageStateId.CAN_ID_STATE_UNKNOWN;

        /// <summary>
        /// Converts a value to a target type depending of the object recieved in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility isVisible = Visibility.Hidden;


            IsThawingTemperatureSetPointReached = System.Convert.ToBoolean(values[0]);
            SystemState = System.Convert.ToInt32(values[1]);

            if (parameter != null)
            {
                if (parameter.ToString() == "START")
                {
                    switch ((int)SystemState)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            isVisible = Visibility.Hidden;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            if (IsThawingTemperatureSetPointReached)
                                isVisible = Visibility.Visible;
                            else
                                isVisible = Visibility.Hidden;
                            break;
                    }
                }


            }

            return isVisible;
        }

        /// <summary>
        /// Converts back an object to a target type depending of the object recieved in parameter
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
