using SmartAblationSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an element to a triangle point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class ElementToTrianglePointsConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a target type depending on the object received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FrameworkElement element = value as FrameworkElement;
            PointCollection points = new PointCollection();
            const double OFFSET = 0.6;

            if (parameter is string && (string)parameter == "LEFTPOLYGON")
            {
                Action fillPoints = () =>
                {
                    points.Clear();
                    points.Add(new Point(0 + OFFSET, 0 + OFFSET));
                    points.Add(new Point(element.Width / 2, element.Height / 2));
                    points.Add(new Point(0 + OFFSET, element.Height - OFFSET));
                };
                fillPoints();
                element.SizeChanged += (s, ee) => fillPoints();
            }
            else if (parameter is string && (string)parameter == "RIGHTPOLYGON")
            {
                //84.5 0.5,42.5 23.5, 84.5 47
                //width 81
                //height 51
                Action fillPoints = () =>
                {
                    points.Clear();
                    points.Add(new Point(element.Width - OFFSET, 0 + OFFSET));
                    points.Add(new Point(element.Width / 2, element.Height / 2));
                    points.Add(new Point(element.Width - OFFSET, element.Height - OFFSET));
                };
                fillPoints();
                element.SizeChanged += (s, ee) => fillPoints();
            }

            return points;
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
            //do nothing, return default value
            throw new NotImplementedException();
        }
    }
}