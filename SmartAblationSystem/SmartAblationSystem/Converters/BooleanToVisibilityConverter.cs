using System;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a Boolean to Visibility
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class BooleanToVisibilityConverter : IValueConverter
    {
        private string param = "";

        ViewModels.CommonViewModel localCommonViewModel = ViewModels.CommonViewModel.Current;

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
                // Reset the param
                param = string.Empty;

                param = (string)parameter;
                if (param == "CatheterAndBallonState" || param == "DiaphragmMovement" || param == "EsophagusTemperature" || param == "IsRequiredAblationTimeVisible")
                {
                    if (value is Boolean && (bool)value)
                    {

                        return Visibility.Visible;
                    }

                    else
                        return Visibility.Hidden;

                }

                if (param == "ResstCurrentPassword")
                {
                    if (localCommonViewModel.IsCryterionUser|| localCommonViewModel.IsBSCADMINUser)
                    {
                        
                            return Visibility.Collapsed;
                    }

                    else
                    {
                        if (value is Boolean && (bool)value)
                        {
                            return Visibility.Visible;
                        }

                        else
                        {
                            return Visibility.Collapsed;
                        }
                    }

                }

                if (param == "IsLoadingFirmware")
                {
                    if (value is Boolean && (bool)value)
                    {

                        return Visibility.Visible;
                    }

                    else
                        return Visibility.Hidden;

                }
                if (param == "AudioAlertMuteButton")
                {
                    if (value is Boolean && (bool)value)
                    {
                        if (localCommonViewModel.Console.IsUsingAudioAlertMute)
                            return Visibility.Visible;
                        else
                            return Visibility.Hidden;
                    }
                    else
                        return Visibility.Hidden;
                }
                if (param == "AudioAlertButton")
                {
                    if (value is Boolean && (bool)value)
                    {
                        if (!localCommonViewModel.Console.IsUsingAudioAlertMute)
                            return Visibility.Visible;
                        else
                            return Visibility.Hidden;
                    }
                    else
                        return Visibility.Hidden;

                    
                }

                if(param == "DeleteJson")
                {
	                if(value is bool b_ && b_ && (localCommonViewModel.IsDoctor || localCommonViewModel.IsAdminUser))
		                return Visibility.Visible;

	                return Visibility.Hidden;
                }

                if (param == "ProcedureExporting")
                {
	                switch (value)
	                {
		                case bool b1_ when b1_:
			                return Visibility.Hidden;
		                case bool b2_ when !b2_:
			                return Visibility.Visible;
	                }
                }
						}

            if (value is Boolean && (bool)value)
            {
                return Visibility.Visible;
            }

            if (parameter is string && (string)parameter == "HIDDEN")
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Collapsed;
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
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}