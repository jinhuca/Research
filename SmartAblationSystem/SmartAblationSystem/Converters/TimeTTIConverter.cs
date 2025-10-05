using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    internal class TimeTTIConverter : IMultiValueConverter
    {
        bool isCatheterConnected = false;
        //int timeTTI = 0;
        int cryoTherapyTime = 0;
        int veinIsolationDuration = 0;
        string state = string.Empty;
        bool playBackBoolValue = false;
        int timeInAblationMax = 0;
        // bool isVeinIsolated = false;

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
            isCatheterConnected = System.Convert.ToBoolean(values[0]);
            //timeTTI = System.Convert.ToInt32(values[1]);
            cryoTherapyTime = System.Convert.ToInt32(values[1]);
            veinIsolationDuration = System.Convert.ToInt32(values[2]);
            state = values[3].ToString();
            playBackBoolValue = System.Convert.ToBoolean(values[4]);
            timeInAblationMax = System.Convert.ToInt32(values[5]);
            //  isVeinIsolated = System.Convert.ToBoolean(values[5]);
            if (isCatheterConnected && veinIsolationDuration > 0 && (state == "CAN_ID_STATE_TRANSITION" ||state == "CAN_ID_STATE_ABLATION" || state == "CAN_ID_STATE_THAWING"))  //CAN_ID_STATE_INFLATION
            {
                return (cryoTherapyTime - veinIsolationDuration).ToString();
               // return timeTTI.ToString();     
            }
            else if (playBackBoolValue) 
            {
                if (veinIsolationDuration > 0)
                {
                    if (timeInAblationMax > 0 )
                    {
                        if ((timeInAblationMax - veinIsolationDuration) >= 0)
                            return (timeInAblationMax - veinIsolationDuration).ToString();
                        else
                            return "-";
                    }
                    else
                        return "-";
                }
                else
                    return "-";
            }
            else
            {
                return "-";
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }

    }
}
