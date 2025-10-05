using System;
using System.Globalization;
using System.Windows.Data;

using static Shared.SharedConstants;

namespace SmartAblationSystem.Converters
{
  class BalloonSizeTextConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (bool)value ? BalloonSize_31mm : BalloonSize_28mm;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
