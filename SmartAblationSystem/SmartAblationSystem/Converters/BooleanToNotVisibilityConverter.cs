using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts Boolean to the opposite of Visibility.
    /// Ex: A true boolean value will converts to a non-visible visibility value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class BooleanToNotVisibilityConverter : IValueConverter
    {
        private string param = "";

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
                param = (string)parameter;
                if (param == "CatheterConnection")
                {
                    if (value is Boolean && (bool)value)
                    {

                        return Visibility.Collapsed;
                    }
                    if (CommonViewModel.Current.IsCatheterCableConnected)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;

                }

                if (parameter is string && (string)parameter == "HIDDEN")
                {
                    if (value is Boolean && (bool)value)
                    {
                        return Visibility.Hidden;
                    }

                    else
                    {
                        return Visibility.Visible;
                    }
                }

            }


            if (value is Boolean && (bool)value)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
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
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}