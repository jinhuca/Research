using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Module.CatheterTestTool.Converters
{
  [ValueConversion(typeof(string), typeof(Visibility))]
  public class SystemStateToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is string currentState)) return Visibility.Collapsed;
      if (!(parameter is string targetState)) return Visibility.Collapsed;

      return currentState.Equals(targetState, StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
