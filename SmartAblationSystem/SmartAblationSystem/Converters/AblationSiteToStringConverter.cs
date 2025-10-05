using System;
using System.Windows.Data;
using Shared;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts an Ablation Site to a String
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class AblationSiteToStringConverter : IValueConverter
  {
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
      string returnValue = "-";
      FieldToTextConverter converter = new FieldToTextConverter();

      try
      {
        if (value is AblationSiteEnum)
        {
          var ablationSite = (AblationSiteEnum)value; 
          if (ablationSite != AblationSiteEnum.UNKNOWN)
            returnValue = ablationSite.GetDescription(); 
        }
        else if (value is int)
        {
          returnValue = IntToAblationSiteEnumString((int)value, returnValue);
        }
        else if (value is string)
        {
          if (Int32.TryParse((string)value, out int siteValue))
          {
            returnValue = IntToAblationSiteEnumString(siteValue, returnValue); 
          }
          else if (Enum.TryParse((string)value, true, out AblationSiteEnum ablationSite))
          {
            returnValue = ablationSite != AblationSiteEnum.UNKNOWN ? ablationSite.GetDescription() : returnValue; 
          }
        }

        return returnValue;
      }
      catch (Exception exception)
      {
        exception.ToString();
        return "-";
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
      //do nothing, return default value
      return Binding.DoNothing;
    }

    private string IntToAblationSiteEnumString(int value, string returnValue)
    {
      var ablationSiteValue = (int)value;
      var ablationSite = Enum.IsDefined(typeof(AblationSiteEnum), ablationSiteValue)
        ? (AblationSiteEnum)ablationSiteValue
        : AblationSiteEnum.UNKNOWN;

      if (ablationSite != AblationSiteEnum.UNKNOWN)
      {
        returnValue = ablationSite.GetDescription();
      }

      return returnValue;
    }
  }
}