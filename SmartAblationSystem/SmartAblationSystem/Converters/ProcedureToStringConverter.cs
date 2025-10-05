using DataAccessLayer;
using SmartAblationSystem.Helpers;
using System;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class converts a procedure object property to a string
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class ProcedureToStringConverter : IValueConverter
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
                        return patient.FirstName + " " + patient.LastName;
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

                        if (patient.DateOfBirth.Day == 1 && patient.DateOfBirth.Month == 1 && patient.DateOfBirth.Year == 1800)
                            return string.Empty;
                            return patient.DateOfBirth.ToString("MMMM dd, yyyy");
                    }
                    else if (parameter.ToString() == "GENDER")
                    {
                        if (patient.Gender == -1)
                            return "--";
                            return patient.Gender == 1 ? "Male" : "Female";
                    }

                    else if (parameter.ToString() == "BMI")
                    {
                        if (patient.Height == 0)
                            return "--";
                        return ((double)(patient?.Weight / Math.Pow((double)(patient?.Height / 100.0), 2))).ToString("00");
                    }

                    else if (parameter.ToString() == "HOSPITAL_ID")
                    {
                        return patient.HospitalPatientId;
                    }
                    else if (parameter.ToString() == "PHYSICIAN_NAME")
                    {
                        return patient?.Physician?.Name;
                    }
                    else if (parameter.ToString() == "PHYSICIAN_FULLNAME")
                    {
                        string DrFullName = patient?.Physician?.FirstName + " " + patient?.Physician?.LastName;
                        if (DrFullName.Length > 1) return "Dr. " + DrFullName;
                        else return "";
                    }
                    else if (parameter.ToString() == "WEIGHT")
                    {

                        if (Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs)
                        {
                            return Scale.ConvertKgToLb((double)patient?.Weight).ToString("00");
                        }
                        else
                        {
                            return (bool)patient?.Weight.HasValue ? patient?.Weight.Value.ToString("00") : "--";
                        }

                        
                    }
                    else if (parameter.ToString() == "HEIGHT")
                    {
                        if (Toise.CurrentToiseUnit == Enumeration.LengthUnit.Inches)
                        {
                            return Toise.ConvertCmToInch((double)patient?.Height).ToString("00");
                        }
                        else
                        {
                            return (bool)patient?.Height.HasValue ? patient?.Height.Value.ToString("00") : "--";
                        }
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

                    else if (parameter.ToString() == "InBodyTime")
                    {
                        return (procedure.SkinToSkinDuration / 60) + " min";
                    }

                    else if (parameter.ToString() == "IsDataEdited")
                    {
                        if (procedure.IsDataEdited == true)
                            return Visibility.Visible;
                        else
                            return
                                Visibility.Hidden;
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

                else if (parameter.ToString() == "IsDataEdited" && value == null)
                {
                        return Visibility.Hidden;
                }
            }
            return string.Empty;
        }

        /// <summary> Converts back an object to a target type depending on the object received in
        /// parameter. Safety classification: No injury or damage to health is possible(IEC 62304
        /// Class A). </summary> <param name="value">An object to convert back.</param> <param
        /// name="targetType">A Type representing the conversion target type.</param> <param
        /// name="parameter">An object representing the conversion's parameter.</param> <param
        /// name="culture">Provides information about a specific culture (called a locale for unmanaged</param>
        /// <returns>An object converted to the target type.</returns>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                // TO DO
                ex.ToString();
                return 0;
            }
        }

        #endregion IValueConverter Members
    }
}