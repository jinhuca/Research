using SmartAblationSystem.Helpers;
using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a String to a Double
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class StringTodoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        int esophagusTemperature;

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
            if (parameter != null)
            {
                if (parameter.ToString() == "WithoutDecimalsSccm")
                {
                    return System.Convert.ToInt32(value).ToString();
                }
                else if (parameter.ToString() == "WithoutDecimal")
                {
                    return System.Convert.ToInt32(value).ToString();
                }
                else if (parameter.ToString() == "WithoutDecimalsPsig")
                {
                    return System.Convert.ToInt32(value).ToString();
                }
                else if (parameter.ToString() == "DiaphragmMovement")
                {
                    int diaphragmMovement = 0;
                    diaphragmMovement = System.Convert.ToInt32(value);

                    if (diaphragmMovement > 100 || diaphragmMovement < 0)
                    {
                        return "--";
                    }
                    else
                    {
                        return (System.Convert.ToInt32(value));
                    }
                }

                else if (parameter.ToString() == "TimeToVeinIsolation")
                {
                    int timeToVeinIsolation = 0;
                    timeToVeinIsolation = System.Convert.ToInt32(value);

                    if (timeToVeinIsolation == 0)
                    {
                        return "--";
                    }
                    else
                    {
                        return (System.Convert.ToInt32(value));
                    }
                }

                else if (parameter.ToString() == "TimeToTarget")
                {
                    int timeToTarget = 0;
                    timeToTarget = System.Convert.ToInt32(value);

                    if (timeToTarget == 0)
                    {
                        return "--";
                    }
                    else
                    {
                        return (System.Convert.ToInt32(value));
                    }
                }

                else if (parameter.ToString() == "TimeToThaw")
                {
                    int TimeToThaw = 0;
                    TimeToThaw = System.Convert.ToInt32(value);

                    if (TimeToThaw == 0)
                    {
                        return "--";
                    }
                    else
                    {
                        return (System.Convert.ToInt32(value));
                    }
                }

                else if (parameter.ToString() == "EsophagusTemperature")
                {

                    esophagusTemperature = (int)Math.Round(System.Convert.ToDouble(value));

                    if (esophagusTemperature < 0 || esophagusTemperature > 50)
                        return "--";

                    return esophagusTemperature;
                }
                else if (parameter.ToString() == "OneDecimalValue")
                {
                    return (System.Convert.ToDecimal(value).ToString("00.0"));
                }
                else if (parameter.ToString() == "LC1Reading")
                {
                    if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                        return System.Convert.ToDouble(value).ToString("0.0");
                    else
                        return Scale.ConvertLbToKg(System.Convert.ToDouble(value)).ToString("0.0");

                  }
                else if (parameter.ToString() == "Weight" || parameter.ToString() == "Height")
                {
	                var successful_ = double.TryParse(value?.ToString(), out double value_);
	                if (value != null) return successful_ ? (value_).ToString("00") : "--";
                }
            }
            return value.ToString();
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
                string strvalue = value as string;

                return System.Convert.ToDouble(strvalue);
            }
            catch (Exception ex)
            {
                // TO DO
                ex.ToString();
                return parameter.ToString() == "CanBeEmpty" 
                  ? Binding.DoNothing
                  : 0;
            }
        }

        #endregion IValueConverter Members
    }
}