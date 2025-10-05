using System;
using System.Windows.Data;
using System.Windows.Media;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a State to a Color
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class StateToColorConverter : IValueConverter
    {
        ViewModels.CommonViewModel localCommonViewModel = ViewModels.CommonViewModel.Current;
        Color blueColor = (Color)ColorConverter.ConvertFromString("#00afef");
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
            Color orangeColor = (Color)ColorConverter.ConvertFromString("#FF9333");
            Color color = (Color)ColorConverter.ConvertFromString("#FF00afef");
            if (parameter != null)
            {
                if (parameter.ToString() == "IDLE")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE)
                    {
                        return new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "READY")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_READY)
                    {
                        return new SolidColorBrush(Colors.LimeGreen);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "INFLATION")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_INFLATION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "ABLATION")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_ABLATION || (int)value == (int)MessageStateId.CAN_ID_STATE_TRANSITION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "THAWING")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_THAWING)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }

                // The color is diffrent when we are in cryo screen 
                else if (parameter.ToString() == "INFLATION_CRYO")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_INFLATION)
                    {
                        return new SolidColorBrush(orangeColor);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "ABLATION_CRYO")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_ABLATION || (int)value == (int)MessageStateId.CAN_ID_STATE_TRANSITION)
                    {
                        return new SolidColorBrush(orangeColor);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "THAWING_CRYO")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_THAWING)
                    {
                        return new SolidColorBrush(orangeColor);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }

                else if (parameter.ToString() == "TRANSITIONPID")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_TRANSITION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }
                else if (parameter.ToString() == "ABLATIONPID")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_ABLATION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.White);
                    }
                }

                else if (parameter.ToString() == "DiaphragmMovement")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_INFLATION ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_THAWING ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        // Here that we are using  to alert before chadi change 20 FEV 2018
                        //if(localCommonViewModel.EcgChannel7And8Reading != 100)
                        //return new SolidColorBrush(Colors.Red);

                        //else
                        //    return new SolidColorBrush(color);

                        // Same code we can remove else...
                        return new SolidColorBrush(color);

                    }
                }

                else if (parameter.ToString() == "DiaphragmAmplitudeThreshold")
                {
                    if (!localCommonViewModel.AreSensorsInPlayBackMode &&
                        (int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_INFLATION ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_THAWING ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }

                else if (parameter.ToString() == "EsophagusTemperature")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY)

                    {
                        return new SolidColorBrush(color);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }

                else if (parameter.ToString() == "CryoBorder")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_THAWING ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }

                else if (parameter.ToString() == "EsophagusTemperatureThresholdReachedCryoBorder")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }
                else if (parameter.ToString() == "CryoBorderNoAlert")
                {
                    if ((int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_READY ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_THAWING ||
                        (int)value == (int)MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        return blueColor;
                    }
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strvalue = value as string;

            return System.Convert.ToInt32(strvalue);
        }

        #endregion IValueConverter Members
    }
}