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
    /// <summary>
    /// This class return the visibility value of state and time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class StateAndTimeToVisibilityConverter : IMultiValueConverter
    {
        bool displayThawingBallon = true;
        int value =  0;

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
            value = System.Convert.ToInt16(values[0]);
            displayThawingBallon = System.Convert.ToBoolean(values[1]);

            Visibility isVisible = Visibility.Hidden;

            if (parameter != null)
            {
                if (parameter.ToString() == "START")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            isVisible = Visibility.Hidden;
                            break;
                    }
                }
                else if (parameter.ToString() == "STOP")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                            isVisible = Visibility.Hidden;
                            break;
                    }
                }
                else if (parameter.ToString() == "VACUUM")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                            isVisible = Visibility.Hidden;
                            break;
                    }
                }
                else if (parameter.ToString() == "TABS")
                {
                    isVisible = Visibility.Visible;
                }
                else if (parameter.ToString() == "TREATMENT")
                {
                    isVisible = Visibility.Visible;
                }
                else if (parameter.ToString() == "HOME" || parameter.ToString() == "UserManual")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                        case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:

                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                        case (int)MessageStateId.CAN_ID_STATE_THAWING:

                            isVisible = Visibility.Hidden;
                            break;
                    }
                }
                else if (parameter.ToString() == "ABLATION_TIMER" ||
                         parameter.ToString() == "TARGET_TEMPERATURE" ||
                         parameter.ToString() == "ABLATION_SITE" ||
                         parameter.ToString() == "TREATMENT_NOTES" ||
                         parameter.ToString() == "CATHETER_TYPE" ||
                         parameter.ToString() == "VOLUME")
                {
                    isVisible = Visibility.Visible;
                }
                else if (parameter.ToString() == "TIME_TO_THAW")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            isVisible = Visibility.Hidden;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            isVisible = Visibility.Visible;
                            break;
                    }
                }
                else if (parameter.ToString() == "TIME_TO_TEMPERATURE")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                            isVisible = Visibility.Hidden;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            isVisible = Visibility.Visible;
                            break;
                    }
                }

                else if (parameter.ToString() == "CatheterAndBallonState")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                            isVisible = Visibility.Visible;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            isVisible = Visibility.Hidden;
                            break;

                        case (int)MessageStateId.CAN_ID_STATE_THAWING:
                            if(displayThawingBallon)
                            isVisible = Visibility.Visible;
                            else
                            isVisible = Visibility.Hidden;
                            break;
                    }
                }

                else if (parameter.ToString() == "CatheterTube")
                {
                    switch ((int)value)
                    {

                        case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                        case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:

                            isVisible = Visibility.Hidden;
                            break;

                        default:
                            isVisible = Visibility.Visible;
                            break;
                    }
                }
            }

            return isVisible;

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
