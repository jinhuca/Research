using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts boot loader update and state to enable state
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class StateAndBooleanToEnableConverter : IMultiValueConverter
    {
        MessageStateId state;
        bool isBootLoaderUpdatingFirmware = false;

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
            bool isEnable = false;

            state = (MessageStateId)values[0];
            isBootLoaderUpdatingFirmware = System.Convert.ToBoolean(values[1]);

            if(isBootLoaderUpdatingFirmware)
                return false;

            else if (parameter.ToString() == "HOME")
            {
                switch ((int)state)
                {
                    case (int)MessageStateId.CAN_ID_STATE_IDLE:
                    case (int)MessageStateId.CAN_ID_STATE_READY:
                    case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                    case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:
                        if(!isBootLoaderUpdatingFirmware)
                        isEnable = true;
                        break;

                    case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                    case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                    case (int)MessageStateId.CAN_ID_STATE_THAWING:
                        isEnable = false;
                        break;
                }
            }
            else if (parameter.ToString() == "RequiredTime")
            {
                switch ((int)state)
                {
                    case (int)MessageStateId.CAN_ID_STATE_THAWING:
                        isEnable = false;
                        break;


                    case (int)MessageStateId.CAN_ID_STATE_IDLE:
                    case (int)MessageStateId.CAN_ID_STATE_READY:
                    case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                    case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                    case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                    case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                        if (isBootLoaderUpdatingFirmware)
                        {
                            isEnable = false;
                        }
                        else
                        {
                            isEnable = true;
                        }
                        break;
                }
            }

            return isEnable;
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
