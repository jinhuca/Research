using System;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a State ID to text
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class StateIdToTextConverter : IValueConverter
    {
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
            switch ((int)value)
            {
                case (int)MessageStateId.CAN_ID_STATE_IDLE:

                    return "IDLE";

                case (int)MessageStateId.CAN_ID_STATE_READY:

                    return "READY";

                case (int)MessageStateId.CAN_ID_STATE_INFLATION:

                    return "INFLATION";

                case (int)MessageStateId.CAN_ID_STATE_TRANSITION:

                    return "TRANSITION";

                case (int)MessageStateId.CAN_ID_STATE_ABLATION:

                    return "ABLATION";

                case (int)MessageStateId.CAN_ID_STATE_THAWING:

                    return "THAWING";

                default:
                    return "UNKNOWN";
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
            throw new NotImplementedException();
        }
    }
}