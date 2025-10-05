using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a double to string
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class DoubleToStringConverter : MarkupExtension, IValueConverter
    {
        private string param = "";
        private double value;


        /// <summary>
        /// Provides a value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="serviceProvider">A IServiceProvider representing a service provider.</param>
        /// <returns>This DoubleToIntConverter object.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

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
            if (value is double)
            {
                // We have to handle to logic if the sensor are connected and the syetem is not in play back


                this.value = (double)value;
                param = (string)parameter;


                if (SensorReadingMananger.AreSensorsConnected)
                {
                    if (!string.IsNullOrWhiteSpace(param))
                    {
                        if (param != "DiaphragmAmplitude" && !SensorReadingMananger.IsCatheterCableConnected)
                        {
                            return "--";
                        }

                        if (param == "TipOrBalloonPressure")
                        {
                            return TipBalloonPressureSelection.TipPressureSelected ? ((int)this.value).ToString() : (this.value).ToString("0.0");
                        }

                        if (param == "OBP")
                        {
                            return (this.value).ToString("0.0");
                        }

                        if (param == "DiaphragmAmplitude")
                        {
                            return ((this.value).ToString() + "  %");
                        }

                        if (param == "BDI")
                        {
                            return this.value;
                        }

                        if (param == "BloodDetecorImValue")
                        {
                            return this.value;
                        }
                    }
                }

                else
                {
                    if (!string.IsNullOrWhiteSpace(param))
                    {

                        if (param == "TipOrBalloonPressure")
                        {
                            return TipBalloonPressureSelection.TipPressureSelected ? ((int)this.value).ToString() : (this.value).ToString("0.0");
                        }

                        if (param == "OBP")
                        {
                            return (this.value).ToString("0.0");
                        }

                        if (param == "DiaphragmAmplitude")
                        {
                            return ((this.value).ToString() + "  %");
                        }

                        if (param == "BDI")
                        {
                            return this.value;
                        }

                        if (param == "BloodDetecorImValue")
                        {
                            return this.value;
                        }
                    }
                }
            }

            if (value is int)
            {
                this.value = (int)value;
                param = (string)parameter;

                if (SensorReadingMananger.AreSensorsConnected)
                {

                    if (!string.IsNullOrWhiteSpace(param))
                    {
                        if (!SensorReadingMananger.IsCatheterCableConnected)
                        {
                            return "--";
                        }


                        if (param == "BloodDetecorImValue")
                        {
                            return this.value;
                        }
                    }
                }
                else
                {
                    if (param == "BloodDetecorImValue")
                    {
                        return this.value;
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
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}