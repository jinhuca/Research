using SmartAblationSystem.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using Type = System.Type;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts a Catheter ID to name
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  [ValueConversion(typeof(object[]), typeof(string))]
  internal class CatheterIDToNameMultiBinding : IMultiValueConverter
  {
    #region IValueConverter Members
    const int ID28mm = (int)Helpers.Enumeration.CatheterType.ID28mm;
    const int plus = (int)Helpers.Enumeration.CatheterType.Plus;

    private readonly string[] catheterDescriptionID28mm = Languages.CatheterDescription[ID28mm].Split('-');
    private readonly string[] catheterDescriptionplus = Languages.CatheterDescription[plus].Split('-');


    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values[2] == null)
      {
        return UIConstants.DoubleDash;
      }

      var _catheterType = (Helpers.Enumeration.CatheterType)values[0];
      var _isusedEngineering = (bool)values[1];

      if (_catheterType == Helpers.Enumeration.CatheterType.ID28mm) 
      {
        var catheterused = catheterDescriptionID28mm[0] + catheterDescriptionID28mm[2];
        return _isusedEngineering ? catheterused + " Test" : catheterused;
      }
      else if (_catheterType == Helpers.Enumeration.CatheterType.Plus)
      {
        var catheterused = catheterDescriptionplus[0] + catheterDescriptionplus[2];
        return _isusedEngineering ? catheterused + " Test" : catheterused;
      }
      return "";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion IValueConverter Members
  }
}
