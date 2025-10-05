using SmartAblationSystem.ViewModels;
using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts State to Snow Flake image file path
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class StateToSnowFlakeConverter : IValueConverter
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
            ImageSource imageSource;

            if (parameter != null)
            {
                if (parameter.ToString() == "BallonInThawingState")
                {
                    switch ((int)value)
                    {


                        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                            imageSource = new BitmapImage(new Uri("/Images/Snow Flake.png", UriKind.Relative));
                            break;

                        default:
                            imageSource = null;
                            //imageSource = new BitmapImage(new Uri("/Images/Background-withoutlogo.jpg", UriKind.Relative));
                            
                            break;
                    }
                    return imageSource;
                }
                else if (parameter.ToString() == "InFlationORThawingState")
                {
                    switch ((int)value)
                    {
                        case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        case (int)MessageStateId.CAN_ID_STATE_READY:
                        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                        case (int)MessageStateId.CAN_ID_STATE_THAWING :
                            return true;
                        default:
                            return false;
                    }
                   
                }

            }


            switch ((int)value)
            {
                case (int)MessageStateId.CAN_ID_STATE_IDLE:
                    imageSource = new BitmapImage(new Uri("/Images/Status Idle.png", UriKind.Relative));
                    break;

                case (int)MessageStateId.CAN_ID_STATE_READY:
                    imageSource = new BitmapImage(new Uri("/Images/Status Ready.png", UriKind.Relative));
                    break;
                    
                case (int)MessageStateId.CAN_ID_STATE_INFLATION:
                    imageSource = new BitmapImage(new Uri("/Images/Status Inflated-Thaw.png", UriKind.Relative));
                    break;

                case (int)MessageStateId.CAN_ID_STATE_THAWING:
                    imageSource = new BitmapImage(new Uri("/Images/Status Inflated-Thaw.png", UriKind.Relative));
                    break;

                case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
                case (int)MessageStateId.CAN_ID_STATE_ABLATION:
                    imageSource = new BitmapImage(new Uri("/Images/Snow Flake.png", UriKind.Relative));
                    break;

                case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
                    imageSource = new BitmapImage(new Uri("/Images/Background-withoutlogo.jpg", UriKind.Relative));
                    break;

                default:
                    imageSource = new BitmapImage(new Uri("/Images/Background-withoutlogo.jpg", UriKind.Relative));
                    break;
            }
            return imageSource;
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