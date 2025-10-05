using DevExpress.Xpf.Gauges;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a boolean to a symbol representation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class BoolToSymbolPresentationConverter : IValueConverter
    {
        private static SolidColorBrush redBrush = new SolidColorBrush(Color.FromArgb(230, 255, 56, 56));
        private static SolidColorBrush transparentRedBrush = new SolidColorBrush(Color.FromArgb(25, 255, 56, 56));
        private static SolidColorBrush greenBrush = new SolidColorBrush(Color.FromArgb(230, 27, 255, 20));
        private static SolidColorBrush transparentGreenBrush = new SolidColorBrush(Color.FromArgb(25, 27, 255, 20));
        private static SolidColorBrush yellowBrush = new SolidColorBrush(Color.FromArgb(230, 238, 255, 20));
        private static SolidColorBrush transparentYellowBrush = new SolidColorBrush(Color.FromArgb(25, 238, 255, 20));
        private static SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);

        private static DefaultMatrix8x14Presentation redSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = redBrush, FillInactive = transparentBrush };
        private static DefaultMatrix8x14Presentation gangerRedSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = redBrush, FillInactive = transparentBrush };
        private static DefaultMatrix8x14Presentation yellowSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = yellowBrush, FillInactive = transparentBrush };
        private static DefaultMatrix8x14Presentation greenLeftSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = greenBrush, FillInactive = transparentBrush };
        private static DefaultMatrix8x14Presentation greenRightSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = greenBrush, FillInactive = transparentBrush };
        private static DefaultMatrix8x14Presentation gangerGreenSegmentPresentation = new DefaultMatrix8x14Presentation() { FillActive = transparentGreenBrush, FillInactive = transparentBrush };
        private static DefaultFourteenSegmentsPresentation timerPresentation = new DefaultFourteenSegmentsPresentation() { FillActive = greenBrush, FillInactive = transparentBrush };

        #region IValueConvector implementation

        /// <summary>
        /// Converts a value to a target type depending on the object received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object to convert.</param>
        /// <param name="targetType">A Type representing the conversion target type.</param>
        /// <param name="parameter">An object representing the conversion's parameter.</param>
        /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
        /// <returns>An object converted to the target type.</returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.BaseType == typeof(SymbolPresentation))
            {
                string currentSegment = (string)parameter;
                bool isSegmentEnabled = (bool)value;

                switch (currentSegment)
                {
                    case "Red":
                        {
                            if (isSegmentEnabled)
                                redSegmentPresentation.FillActive = redBrush;
                            else
                                redSegmentPresentation.FillActive = transparentRedBrush;
                            return redSegmentPresentation;
                        }
                    case "Yellow":
                        {
                            if (isSegmentEnabled)
                                yellowSegmentPresentation.FillActive = yellowBrush;
                            else
                                yellowSegmentPresentation.FillActive = transparentYellowBrush;
                            return yellowSegmentPresentation;
                        }
                    case "GreenLeft":
                        {
                            if (isSegmentEnabled)
                                greenLeftSegmentPresentation.FillActive = greenBrush;
                            else
                                greenLeftSegmentPresentation.FillActive = transparentGreenBrush;
                            return greenLeftSegmentPresentation;
                        }
                    case "GreenRight":
                        {
                            if (isSegmentEnabled)
                                greenRightSegmentPresentation.FillActive = greenBrush;
                            else
                                greenRightSegmentPresentation.FillActive = transparentGreenBrush;
                            return greenRightSegmentPresentation;
                        }
                    case "GangerGreen":
                        {
                            if (isSegmentEnabled)
                                gangerGreenSegmentPresentation.FillActive = greenBrush;
                            else
                                gangerGreenSegmentPresentation.FillActive = transparentGreenBrush;
                            return gangerGreenSegmentPresentation;
                        }
                    case "GangerRed":
                        {
                            if (isSegmentEnabled)
                                gangerRedSegmentPresentation.FillActive = redBrush;
                            else
                                gangerRedSegmentPresentation.FillActive = transparentRedBrush;
                            return gangerRedSegmentPresentation;
                        }
                    default:
                        {
                            if (isSegmentEnabled)
                                timerPresentation.FillActive = greenBrush;
                            else
                                timerPresentation.FillActive = redBrush;
                            return timerPresentation;
                        }
                }
            }
            return null;
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
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion IValueConvector implementation
    }
}