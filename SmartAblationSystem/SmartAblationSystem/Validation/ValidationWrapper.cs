using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartAblationSystem.Validation
{
    /// <summary>
    /// This class creates a dependency property
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ValidationWrapper : DependencyObject
    {
        //This wrapper is needed to create a dependency property.
        //https://social.technet.microsoft.com/wiki/contents/articles/31422.wpf-passing-a-data-bound-value-to-a-validation-rule.aspx

        //Hospital Name Min Length
        public static readonly DependencyProperty HospitalNameMinLengthProperty =
                                DependencyProperty.Register("HospitalNameMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of hospital name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalNameMinLength
        {
            get { return (int)GetValue(HospitalNameMinLengthProperty); }
            set { SetValue(HospitalNameMinLengthProperty, value); }
        }

        //Hospital Address Min Length
        public static readonly DependencyProperty HospitalAddressMinLengthProperty =
                                DependencyProperty.Register("HospitalAddressMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));
        /// <summary>
        /// Gets or sets min-length of address(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalAddressMinLength
        {
            get { return (int)GetValue(HospitalAddressMinLengthProperty); }
            set { SetValue(HospitalAddressMinLengthProperty, value); }
        }

        //Hospital City Min Length
        public static readonly DependencyProperty HospitalCityMinLengthProperty =
                                DependencyProperty.Register("HospitalCityMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of city name(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalCityMinLength
        {
            get { return (int)GetValue(HospitalCityMinLengthProperty); }
            set { SetValue(HospitalCityMinLengthProperty, value); }
        }

        //Hospital State Min Length
        public static readonly DependencyProperty HospitalStateMinLengthProperty =
                                DependencyProperty.Register("HospitalStateMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of state name(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalStateMinLength
        {
            get { return (int)GetValue(HospitalStateMinLengthProperty); }
            set { SetValue(HospitalStateMinLengthProperty, value); }
        }

        //Hospital State Min Length
        public static readonly DependencyProperty HospitalZIPCodeMinLengthProperty =
                                DependencyProperty.Register("HospitalZIPCodeMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of ZIP code(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalZIPCodeMinLength
        {
            get { return (int)GetValue(HospitalZIPCodeMinLengthProperty); }
            set { SetValue(HospitalZIPCodeMinLengthProperty, value); }
        }

        //Hospital Country Min Length
        public static readonly DependencyProperty HospitalCountryMinLengthProperty =
                                DependencyProperty.Register("HospitalCountryMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of country name(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalCountryMinLength
        {
            get { return (int)GetValue(HospitalCountryMinLengthProperty); }
            set { SetValue(HospitalCountryMinLengthProperty, value); }
        }

        //Hospital Phone Number Min Length
        public static readonly DependencyProperty HospitalPhoneNumberMinLengthProperty =
                                DependencyProperty.Register("HospitalPhoneNumberMinLength", typeof(int),
                                typeof(ValidationWrapper), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets or sets min-length of phone number(hospital)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalPhoneNumberMinLength
        {
            get { return (int)GetValue(HospitalPhoneNumberMinLengthProperty); }
            set { SetValue(HospitalPhoneNumberMinLengthProperty, value); }
        }
    }
}
