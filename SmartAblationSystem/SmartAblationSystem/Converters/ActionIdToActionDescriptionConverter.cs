using System;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts an Action ID to an Action Description
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class ActionIdToActionDescriptionConverter : IValueConverter
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
            switch ((int)value)
            {
                case (int)Helpers.Enumeration.Actions.Login:

                    return "Login";

                case (int)Helpers.Enumeration.Actions.Logout:

                    return "Logout";

                case (int)Helpers.Enumeration.Actions.StartCommand:

                    return "StartCommand";

                case (int)Helpers.Enumeration.Actions.StopCommand:

                    return "StopCommand";

                case (int)Helpers.Enumeration.Actions.CreateProcedure:

                    return "CreateProcedure";

                case (int)Helpers.Enumeration.Actions.CreateUser:

                    return "CreateUser";

                case (int)Helpers.Enumeration.Actions.EditUser:

                    return "EditUser";

                case (int)Helpers.Enumeration.Actions.DeleteUser:

                    return "DeleteUser";

                case (int)Helpers.Enumeration.Actions.ResetPassword:

                    return "ResetPassword";

                case (int)Helpers.Enumeration.Actions.AccessRecord:

                    return "AccessRecord";

                case (int)Helpers.Enumeration.Actions.AccessChangeTank:

                    return "AccessChangeTank";

                case (int)Helpers.Enumeration.Actions.AccessSettings:

                    return "AccessSettings";

                case (int)Helpers.Enumeration.Actions.AccessManageUsers:

                    return "AccessManageUsers";

                case (int)Helpers.Enumeration.Actions.AccessDateAndTime:

                    return "AccessDateAndTime";

                case (int)Helpers.Enumeration.Actions.AccessMaintenance:

                    return "AccessMaintenance";

                case (int)Helpers.Enumeration.Actions.AccessSiteSetup:

                    return "AccessSiteSetup";

                case (int)Helpers.Enumeration.Actions.AccessPIDControl:

                    return "AccessPIDControl";

                case (int)Helpers.Enumeration.Actions.AccessElectricalMonitoring:

                    return "AccessElectricalMonitoring";

                case (int)Helpers.Enumeration.Actions.AccessLoadCellCalibration:

                    return "AccessLoadCellCalibration";

                case (int)Helpers.Enumeration.Actions.AccessSystemFiles:

                    return "AccessSystemFiles";

                case (int)Helpers.Enumeration.Actions.AccessCatheterDatabase:

                    return "AccessCatheterDatabase";

                case (int)Helpers.Enumeration.Actions.AccessMechanicalMonitoring:

                    return "AccessMechanicalMonitoring";

                case (int)Helpers.Enumeration.Actions.AccessFlowCurveProgramming:

                    return "AccessFlowCurveProgramming";

                case (int)Helpers.Enumeration.Actions.LoadFirmwareVersionCommand:

                    return "LoadFirmwareVersionCommand";

                case (int)Helpers.Enumeration.Actions.AppModeCommand:

                    return "AppModeCommand";

								case (int)Helpers.Enumeration.Actions.DeleteProcedure:
										return "DeleteProcedure";

                default:

                    return "---";
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
            string strvalue = value as string;

            return System.Convert.ToInt32(strvalue);
        }

        #endregion IValueConverter Members
    }
}