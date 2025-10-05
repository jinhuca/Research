using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts system state to opacity
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class  SystemStateToOpacityConverter : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double opacity = 1;

            if (parameter != null)
            {
                if (parameter.ToString() == "RequiredTime")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_THAWING:

                            opacity = 0.2;
                            break;


                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                        case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                            opacity = 1;
                            break;
                    }
                }

            }

            return opacity; ;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}
