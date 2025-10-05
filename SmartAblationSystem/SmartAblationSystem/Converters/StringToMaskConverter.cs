using DataAccessLayer;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  [ValueConversion(typeof(string), typeof(string))]
  public class StringToMaskConverter : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      char mask_ = '\u002A';

      if(value is Patient p)
      {
        var defaultValue_ = new string(mask_, (p.FirstName + " " + p.LastName).Length);
        var emptyValue_ = string.Empty;

        if (parameter == null)
        {
          return defaultValue_;
        }

        switch(parameter.ToString().ToUpperInvariant())
        {
          case "FULLNAME":
          case "HOSPITAL_ID":
            return defaultValue_;
          case "BIRTHDATE":
            return new string(mask_, p.DateOfBirth.ToShortDateString().Length);
          case "GENDER":
            return new string(mask_, 3);
          case "BMI":
            return new string(mask_, 2);
          case "WEIGHT":
          case "HEIGHT":
            return new string(mask_, 3);
          default:
            return emptyValue_;
        }
      }

      return string.Empty;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
