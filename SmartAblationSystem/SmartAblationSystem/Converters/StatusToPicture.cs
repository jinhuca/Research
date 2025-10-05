using System;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Status to a Picture file path
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class StatusToPicture : IValueConverter
    {
        private const string IMG_OK = "/Images/oksign.png";                    //Image location the successful image
        private const string IMG_FAIL = "/Images/notconnectedsign.png";        //Image location the failure  image

        string state = string.Empty;

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
            string newImage = "/Images/" + value.ToString() + ".png";

            if (parameter?.ToString() == "UseSystemState")
            {
                switch ((int)value)
                {

                    case (int)MessageStateId.CAN_ID_STATE_IDLE:
                        newImage = IMG_FAIL;
                        break;

                    default:
                        newImage = IMG_OK;
                        break;
                }

            }

            else
            {              
                newImage = IMG_FAIL;
                bool statusValue = false;


                try
                {
                    statusValue = System.Convert.ToBoolean(value);
                    if (statusValue == true)
                    {
                        newImage = IMG_OK;
                    }
                }
                catch
                {    //do nothing it should always convert
                }
            }
            return newImage;
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