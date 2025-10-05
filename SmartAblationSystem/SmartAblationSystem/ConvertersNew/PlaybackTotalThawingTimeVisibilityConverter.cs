using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class PlaybackTotalThawingTimeVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedTime_ = System.Convert.ToInt32(values[0]);
      var actualAblationTime_ = System.Convert.ToInt32(values[1]);
      var isInPlaybackMode_ = System.Convert.ToBoolean(values[2]);

      if(isInPlaybackMode_ && selectedTime_ > actualAblationTime_)
      {
        return Visibility.Visible;
      }

      return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}