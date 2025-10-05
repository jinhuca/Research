using System;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// This class is for the Year Validator's validation rule
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class YearValidator : ValidationRule
    {
        /// <summary>
        /// Gets a value indicating whether the Year Validation succeedeed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object value to validate.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>A boolean representing if the validation succeeded or not.</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int Year;
            bool isYearValid = false;

            if (value is String)
            {
                if (!String.IsNullOrWhiteSpace((String)value))
                {
                    if (int.TryParse((string)value, out Year) && Year > 1900 && Year <= DateTime.Now.Year)
                    {
                        isYearValid = true;
                    }
                }
            }
            return isYearValid ? ValidationResult.ValidResult : new ValidationResult(false, "Invalid Year");
        }
    }
}