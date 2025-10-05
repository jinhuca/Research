using Console;
using SmartAblationSystem.ViewModels;
using SmartAblationSystem.Views;
using System;
using System.Windows.Data;
using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Weight to a Level
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class WeightToLevelConverter : IValueConverter
    {
        private const double maximum = 100;
        private const double minimum = 9;
        private const double FivePercentRatio = 0.95;

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
            double loadCellHighRangeLimit = 16;

            if (CommonViewModel.Current.CurrentTank.Type == (int)TankType.Tank_10pounds)
            {
                loadCellHighRangeLimit = 10;
            }

            else
            {
                loadCellHighRangeLimit = 16;
            }


            double tankLevel = ((double)value * maximum) / (loadCellHighRangeLimit * FivePercentRatio);

            if (tankLevel < minimum)
            {
                tankLevel = minimum;
            }


            else if (tankLevel > maximum)
            {
                tankLevel = maximum;
            }


            return tankLevel;
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

            return strvalue;
        }

        #endregion IValueConverter Members
    }
}