
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts BSC and Doctor message
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class CrtAndDoctorMessageConverter : IMultiValueConverter
    {

        string CrtMessage = string.Empty;
        string Message = string.Empty;

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

            int index = 0;

            CrtMessage = values[0].ToString();

            if (CrtMessage?.Length > 0)
            {
                if (CrtMessage.Contains("-"))
                {
                    index = CrtMessage.IndexOf('-');

                    if (index + 11 <= CrtMessage.Length)
                        CrtMessage = CrtMessage.Remove(index + 11);
                    
                }
            }

            Message = values[1].ToString();
            

            return CrtMessage + Message;
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

