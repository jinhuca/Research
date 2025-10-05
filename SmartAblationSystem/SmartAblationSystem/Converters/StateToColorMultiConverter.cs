using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    class StateToColorMultiConverter : IMultiValueConverter
    {
        bool _isSystemInAlert = false;
        int _state = 0;
        bool _isUsingEnhancedNotification = false;

        Color color = (Color)ColorConverter.ConvertFromString("#FF00afef");

        Color blueColor = (Color)ColorConverter.ConvertFromString("#00afef");
        


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
            _isSystemInAlert = System.Convert.ToBoolean(values[0]);
            _state = System.Convert.ToInt16(values[1]);
            _isUsingEnhancedNotification = System.Convert.ToBoolean(values[2]);


            if (parameter.ToString() == "CryoBorder")
            {
                if (_state == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                    _state == (int)MessageStateId.CAN_ID_STATE_READY ||
                    _state == (int)MessageStateId.CAN_ID_STATE_THAWING ||
                    _state == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    if(_isSystemInAlert)
                    return new SolidColorBrush(Colors.Orange);

                    else if(_isUsingEnhancedNotification)
                    return blueColor;
                    else
                    return new SolidColorBrush(Colors.Transparent);

                }
            }

            else if (parameter.ToString() == "EsophagusTemperatureThresholdReachedCryoBorder")
            {
                if (_state == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                   _state == (int)MessageStateId.CAN_ID_STATE_READY ||
                    _state == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    if (_isSystemInAlert)
                    return new SolidColorBrush(Colors.Orange);

                    else if (_isUsingEnhancedNotification)
                    return new SolidColorBrush(blueColor);
                    else
                        return new SolidColorBrush(Colors.Transparent);
                }
            }

            if (parameter.ToString() == "CryoBorderNoAlert")
            {
                if (_state == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                    _state == (int)MessageStateId.CAN_ID_STATE_READY ||
                    _state == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    if (_isSystemInAlert)
                        return new SolidColorBrush(Colors.Red);

                    else if (_isUsingEnhancedNotification)
                    return new SolidColorBrush(blueColor);
                    else
                        return new SolidColorBrush(Colors.Transparent);

                }
            }


            return new SolidColorBrush(Colors.White);



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
