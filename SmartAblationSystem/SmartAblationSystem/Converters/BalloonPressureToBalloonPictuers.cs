using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts State to Snow Flake image file path
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class BalloonPressureToBalloonPictuers : IValueConverter
    {
        #region IValueConverter Members

        string picture = string.Empty;
        /// <summary>
        /// Gets/sets picture string
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Picture
        {
            get
            {
                return picture;
            }

            set
            {
                picture = value;
            }
        }

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

            double targetBallonPressure = 2.5; // CommonViewModel.Current.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetBalloonPressure;
            //string picture = string.Empty;

            double innerBallonPressure = (double)value;

            if (innerBallonPressure < 0)
            {
                Picture = "/Images/Balloon0.png";
            }

            if (innerBallonPressure == 0)
            {
                Picture = "/Images/Balloon1.png";
            }

            if ((innerBallonPressure < targetBallonPressure * 0.9) && (innerBallonPressure>= targetBallonPressure/2)) 
            {
                Picture = "/Images/Balloon2.png";
            }

            if (innerBallonPressure >= targetBallonPressure *0.9) //90% of the ballon pressure
            {
                Picture = "/Images/Balloon3.png";
            }

           return Picture;
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
