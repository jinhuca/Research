using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using UniversalLoginManager;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a User to a String
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class UserToStringConverter : IValueConverter
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
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                if (value is Patient)
                {
                    Patient patient = (Patient)value;

                    if (parameter.ToString() == "FULLNAME")
                    {
                        return patient.FullName;
                    }
                    else if (parameter.ToString() == "FIRSTNAME")
                    {
                        return patient.FirstName;
                    }
                    else if (parameter.ToString() == "LASTNAME")
                    {
                        return patient.LastName;
                    }
                    else if (parameter.ToString() == "BIRTHDATE")
                    {
                        return patient.DateOfBirth.ToString("MMMM dd, yyyy");
                    }
                    else if (parameter.ToString() == "GENDER")
                    {
                        return patient.Gender == 1 ? "Male" : "Female";
                    }
                    else if (parameter.ToString() == "HOSPITAL_ID")
                    {
                        return patient.HospitalPatientId;
                    }
                    else if (parameter.ToString() == "PHYSICIAN_NAME")
                    {
                        return patient?.Physician?.Name;
                    }
                }
                else if (value is Procedure)
                {
                    Procedure procedure = (Procedure)value;

                    if (parameter.ToString() == "PROCEDURE_DATE")
                    {
                        return procedure.ProcedureStartDateTime.ToString("MMMM dd, yyyy");
                    }
                    else if (parameter.ToString() == "DIAGNOSIS")
                    {
                        return procedure.Diagnosis;
                    }
                    else if (parameter.ToString() == "OUTCOME")
                    {
                        return procedure.OutCome;
                    }
                }
                else if (value is Ablation)
                {
                    Ablation ablation = (Ablation)value;

                    if (parameter.ToString() == "TREATMENT_NOTES")
                    {
                        return ablation.TreatmentNote;
                    }
                }
                else if (parameter.ToString() == "TYPE")
                {
                    int userTypeId = 0;

                    IEnumerable<object> listOfTypes = value as IEnumerable<object>;

                    //These will loop when time
                    foreach (object ob in listOfTypes)
                    {
                        DataAccessLayer.Type tempo = null;
                        tempo = ob as DataAccessLayer.Type;
                        if (tempo != null)
                        {
                            userTypeId = tempo.Id;
                        }
                    }

                    if (userTypeId == (int)LoginManager.AccessControlType.USER)
                    {
                        return "User";
                    }
                    else if (userTypeId == (int)LoginManager.AccessControlType.ADMIN)
                    {
                        return "Admin";
                    }
                    else if (userTypeId == (int)LoginManager.AccessControlType.CRYTERION)
                    {
                        return "Cryterion";
                    }
                    else if (userTypeId == (int)LoginManager.AccessControlType.DOCTOR)
                    {
                        return "Doctor";
                    }
                    else
                    {
                        return "Unknown";
                    }
                }
                else if (parameter.ToString() == "EDIT_USER_DOCTOR")
                {
                    if (value is Boolean)
                    {
                        if (!(bool)value)
                        {
                            //USER
                            return "Edit User";
                        }
                        else
                        {
                            //DOCTOR
                            return "Edit Doctor";
                        }
                    }
                }
                else if (parameter.ToString() == "DELETE_USER_DOCTOR")
                {
                    if (value is Boolean)
                    {
                        if (!(bool)value)
                        {
                            //USER
                            return "Delete User";
                        }
                        else
                        {
                            //DOCTOR
                            return "Delete Doctor";
                        }
                    }
                }
                else if (parameter.ToString() == "STATUS")
                {
                    if (value is bool)
                    {
                        if ((bool)value)
                        {
                            return "Active";
                        }
                        else
                        {
                            return "Inactive";
                        }
                    }
                }
            }
            return string.Empty;
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
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                //TO DO
                ex.ToString();
                return 0;
            }
        }

        #endregion IValueConverter Members
    }
}