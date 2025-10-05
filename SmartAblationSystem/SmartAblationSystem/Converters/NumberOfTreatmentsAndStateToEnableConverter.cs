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
    /// This class converts nubmer of treatments and state to boolean.
    ///  Ex: A true boolean value will converts to a non-visible visibility value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class NumberOfTreatmentsAndStateToEnableConverter : IMultiValueConverter
    {
        Int16 numberOfTreatments = 0;
        bool isSystemInplayBack = false;

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

            numberOfTreatments = System.Convert.ToInt16(values[1]);
            isSystemInplayBack = System.Convert.ToBoolean(values[2]);

            switch ((int)values[0])
            {
                case (int)MessageStateId.CAN_ID_STATE_IDLE:
                case (int)MessageStateId.CAN_ID_STATE_READY:

                    if (numberOfTreatments == 0 || !isSystemInplayBack)
                        isEnable = false;
                     else
                        isEnable = true;
                    break;

                case (int)MessageStateId.CAN_ID_STATE_INFLATION:

                    isEnable = false;
                    break;
                case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                case (int)MessageStateId.CAN_ID_STATE_THAWING:
                    isEnable = true;
                    break;
            }

            return isEnable;
        }

        /// <summary>
        /// Converts back an object to a target type depending on the object recieved in parameter
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
