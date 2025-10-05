using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static System.Windows.Media.ColorConverter;

namespace CustomControls.Converters
{
  internal class StateForegroundConverter : IValueConverter
  {
    private static readonly Color DisabledTextColor = (Color)ConvertFromString("#ffcdc2c5");

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value == null)
        throw new ArgumentNullException();

      SolidColorBrush defColorBrush_ = new SolidColorBrush(DisabledTextColor);
      defColorBrush_.Freeze();

      return value is bool b_ && b_ ? Brushes.Black : defColorBrush_;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
