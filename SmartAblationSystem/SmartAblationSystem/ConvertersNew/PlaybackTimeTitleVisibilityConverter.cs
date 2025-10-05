using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class PlaybackTimeTitleVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedTime_ = System.Convert.ToInt32(values[0]);
      var temporaryManualAblationTime_ = System.Convert.ToInt32(values[1]);
      var actualAblationTime_ = System.Convert.ToInt32(values[2]);
      var parameter_ = parameter.ToString();

      switch(parameter_)
      {
        case UIConstants.AblationState:
        {
          return actualAblationTime_ == 0
            ? selectedTime_ <= temporaryManualAblationTime_ ? Visibility.Visible : Visibility.Collapsed
            : selectedTime_ <= actualAblationTime_
              ? Visibility.Visible
              : Visibility.Collapsed;
        }
        case UIConstants.ThawingState:
        {
          return actualAblationTime_ == 0
            ? selectedTime_ > temporaryManualAblationTime_ ? Visibility.Visible : Visibility.Collapsed
            : selectedTime_ > actualAblationTime_
              ? Visibility.Visible
              : Visibility.Collapsed;
        }
        default:
          return Visibility.Collapsed;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
