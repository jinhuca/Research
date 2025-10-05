using System;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// This class is the Null String Validator's validation rule
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class NullStringValidator : ValidationRule
    {
        /// <summary>
        /// Gets a value indicating whether the Null String Validation succeedeed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object value to validate.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>A boolean value if the validation succeeded or not.</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is String)
            {
                if (String.IsNullOrWhiteSpace((String)value))
                {
                    return new ValidationResult(false, "This field is required");
                }
                else
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "The value is not a string");
        }
    }
}