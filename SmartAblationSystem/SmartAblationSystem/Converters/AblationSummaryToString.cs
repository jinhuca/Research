using SmartAblationSystem.Models;
using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an Ablation Summary to a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class AblationSummaryToString : IValueConverter
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
            AblationSummary ablationSummary = null;
            string param = "";

            if (value is AblationSummary)
            {
                ablationSummary = (AblationSummary)value;
                param = (string)parameter;

                if (ablationSummary != null && !string.IsNullOrWhiteSpace(param))
                {
                    if (param == "TotalRSPV")
                    {
                        return ablationSummary.TotalRSPV;
                    }
                    else if (param == "TotalRIPV")
                    {
                        return ablationSummary.TotalRIPV;
                    }
                    else if (param == "TotalLSPV")
                    {
                        return ablationSummary.TotalLSPV;
                    }
                    else if (param == "TotalLIPV")
                    {
                        return ablationSummary.TotalLIPV;
                    }
                    else if (param == "TotalOther")
                    {
                        return ablationSummary.TotalOther;
                    }
                    else if(param == "TotalLCPV")
                    {
                      return ablationSummary.TotalLCPV;
                    }
                    else if(param == "TotalRMPV")
                    {
                      return ablationSummary.TotalRMPV;
                    }
                    else if (param == "TotalRSPVDuration")
                    {
                        return ablationSummary.TotalRSPVDuration;
                    }
                    else if (param == "TotalRIPVDuration")
                    {
                        return ablationSummary.TotalRIPVDuration;
                    }
                    else if (param == "TotalLSPVDuration")
                    {
                        return ablationSummary.TotalLSPVDuration;
                    }
                    else if (param == "TotalLIPVDuration")
                    {
                        return ablationSummary.TotalLIPVDuration;
                    }
                    else if(param == "TotalLCPVDuration")
                    {
                      return ablationSummary.TotalLCPVDuration;
                    }
                    else if(param == "TotalRMPVDuration")
                    {
                      return ablationSummary.TotalRMPVDuration;
                    }
                    else if (param == "TotalOtherDuration")
                    {
                        return ablationSummary.TotalOtherDuration;
                    }
                    else if (param == "TotalAblation")
                    {
                        return ablationSummary.TotalAblation;
                    }
                    else if (param == "TotalAblationDuration")
                    {
                        return ablationSummary.TotalAblationDuration;
                    }
                }
            }

            return "";
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
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                //TO DO
                ex.ToString();
                return value;
            }
        }

        #endregion IValueConverter Members
    }
}