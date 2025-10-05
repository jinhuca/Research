using SmartAblationSystem.ViewModels;
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
    /// This class converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class StateToSnowFlakAndBallonConverter : IValueConverter
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
            string picture = string.Empty;

            switch ((int)value)
            {
                case (int)MessageStateId.CAN_ID_STATE_IDLE:
                case (int)MessageStateId.CAN_ID_STATE_READY:
                    //case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:
                    picture = "/Images/balloon deflated.png";
                    break;

                case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                    picture = "/Images/Inflation On.png";
                    break;

                case (int)MessageStateId.CAN_ID_STATE_THAWING:
                    picture = "/Images/Inflation On.png";
                    break;

                case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                case (int)MessageStateId.CAN_ID_STATE_ABLATION:

                    picture = "/Images/balloon small cryo.png";

                    break;

                case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                    picture = "/Images/Background-withoutlogo.jpg";
                    break;
            }
            return picture;
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

        #endregion IValueConverter Members
    }
}

