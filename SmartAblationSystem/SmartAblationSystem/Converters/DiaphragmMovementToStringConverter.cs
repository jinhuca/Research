using SmartAblationSystem.Models;
using System;
using System.Diagnostics;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an Ablation Summary to a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class DiaphragmMovementToStringConverter : IValueConverter
    {
        ViewModels.CommonViewModel localCommonViewModel = ViewModels.CommonViewModel.Current;
        MessageStateId currentState;
        int diaphragmMaxValue = 100;
        int diaphragmMinValue = 0;

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
            currentState = localCommonViewModel.SystemState;

            int diaphragmCurentvalue = System.Convert.ToInt32(value);

            if (!localCommonViewModel.AreSensorsInPlayBackMode)
            {
                //During ablation
                if (currentState == MessageStateId.CAN_ID_STATE_IDLE ||
                    currentState == MessageStateId.CAN_ID_STATE_READY ||
                    currentState == MessageStateId.CAN_ID_STATE_INFLATION ||
                    !localCommonViewModel.IsDiaphragmMovementDetected ||
                    diaphragmCurentvalue > diaphragmMaxValue || diaphragmCurentvalue < diaphragmMinValue)
                {
 
                    return "-";
                }
                else
                {
            
                    return System.Convert.ToInt32(value);
                }
            }
            else if (diaphragmCurentvalue > diaphragmMaxValue || diaphragmCurentvalue < diaphragmMinValue)
            {

     
                //In playback mode with out of range values
                return "-";
            }
            else
            {

                //In playback mode
                return diaphragmCurentvalue;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                // TO DO
                ex.ToString();
                return value;
            }
        }

        #endregion IValueConverter Members
    }
}