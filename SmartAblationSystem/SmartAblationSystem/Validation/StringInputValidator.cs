using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace SmartAblationSystem.Validation
{
  using System.Globalization;
  using System.Text.RegularExpressions;

  using SmartAblationSystem.Views;

  internal class StringInputValidator : NullStringValidator
  {
    private static readonly Regex _objAlphaNumericPattern = new Regex("^[a-zA-Z0-9 _,-]*$", RegexOptions.Compiled);

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      // Get and convert the value
      string stringValue = GetBoundValue(value) as string;

      var result = base.Validate(stringValue, cultureInfo);
      if (result != null && result.IsValid)
      {
        result = InputRegularExpression(stringValue) 
                   ? result 
                   : new ValidationResult(false, "Alphanumeric, space, underscore, hyphen, comma only.");
      }

      return result;
    }

    private object GetBoundValue(object value)
    {
      if (value is BindingExpression)
      {
        // ValidationStep was UpdatedValue or CommittedValue (validate after setting)
        // Need to pull the value out of the BindingExpression.
        BindingExpression binding = (BindingExpression)value;

        // Get the bound object and name of the property
        string resolvedPropertyName = binding.GetType().GetProperty("ResolvedSourcePropertyName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).GetValue(binding, null).ToString();
        object resolvedSource = binding.GetType().GetProperty("ResolvedSource", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).GetValue(binding, null);

        // Extract the value of the property
        object propertyValue = resolvedSource.GetType().GetProperty(resolvedPropertyName).GetValue(resolvedSource, null);

        return propertyValue;
      }
      else
      {
        return value;
      }
    }

    private static bool InputRegularExpression(string inputstring)
    {
      bool isValid = false;
      try
      {
        isValid = _objAlphaNumericPattern.IsMatch(inputstring);
      }
      catch(Exception ex)
      {
      }

      return isValid;
    }
  }
}
