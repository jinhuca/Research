using System.Globalization;
using System.Windows.Controls;
using System.Linq;

namespace SmartAblationSystem.Validation
{
  public class HospitalInformationTextFieldValidator : ValidationRule
  {
    const int phoneNumberMaxLength = 15;
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      //double measurementParameter = 0;
      string textToValidate = string.Empty;
      int minLength = -1;

      if (value is string)
      {
        textToValidate = (string)value;
      }

      if (this.Wrapper.HospitalNameMinLength > -1)
      {
        minLength = this.Wrapper.HospitalNameMinLength;
      }
      else if (this.Wrapper.HospitalAddressMinLength > -1)
      {
        minLength = this.Wrapper.HospitalAddressMinLength;
      }
      else if (this.Wrapper.HospitalCityMinLength > -1)
      {
        minLength = this.Wrapper.HospitalCityMinLength;
      }
      else if (this.Wrapper.HospitalStateMinLength > -1)
      {
        minLength = this.Wrapper.HospitalStateMinLength;
      }
      else if (this.Wrapper.HospitalZIPCodeMinLength > -1)
      {
        minLength = this.Wrapper.HospitalZIPCodeMinLength;
      }
      else if (this.Wrapper.HospitalCountryMinLength > -1)
      {
        minLength = this.Wrapper.HospitalCountryMinLength;
      }
      else if (this.Wrapper.HospitalPhoneNumberMinLength > -1)
      {
        minLength = this.Wrapper.HospitalPhoneNumberMinLength;
        return ValidatePhoneNumber(textToValidate, minLength);
      }

      if (textToValidate.Length < minLength)
      {
        return new ValidationResult(false,
          "Out of range. Enter a parameter in the range: " + minLength + ".");
      }
      else
      {
        return new ValidationResult(true, null);
      }
    }

    /// <summary>
    /// Gets or sets vaule
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>   
    public ValidationWrapper Wrapper { get; set; }

    private ValidationResult ValidatePhoneNumber(string phoneNumber, int minLength)
    {
      /*var pattern = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,5})|(\(?\d{2,6}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
      var options = System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase;
      System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, pattern, options);*/
      if (phoneNumber.Length < minLength)
      {
        return new ValidationResult(false,
          "Out of range. Enter a parameter in the range: " + minLength + ".");
      }
      else if (phoneNumber.All(char.IsDigit) && phoneNumber.Length <= phoneNumberMaxLength)
      {
        return new ValidationResult(true, null);
      }
      else
      {
        return new ValidationResult(false, "Invalid Phone Number.");
      }
    }
  }
}
