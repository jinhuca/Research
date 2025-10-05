using System;
using System.Globalization;
using System.Windows.Controls;

namespace SmartAblationSystem.Validation
{
    /// <summary>
    /// This class is for the Register Validator's validation rule
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class RegisterValidator : ValidationRule
    {
        private double _min;
        private double _max;

        /// <summary>
        /// Gets or sets the Min value 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Gets or sets the Max value 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the Register Validation succeedeed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="value">An object value to validate.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>A boolean value if the validation succeeded or not.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double measurementParameter = 0;

            try
            {
                if (((string)value).Length > 0)
                    measurementParameter = double.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal type or " + e.Message);
            }

            if ((measurementParameter < Min) || (measurementParameter > Max))
            {
                return new ValidationResult(false,
                  "Out of range (" + Min + " - " + Max + ").");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}