using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
  internal class PlaybackTimeConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedTime_ = System.Convert.ToInt32(values[0]);
      var requiredAblationTime_ = System.Convert.ToInt32(values[1]);
      var actualAblationTime_ = System.Convert.ToInt32(values[2]);
      int timeReturn_;

      if (actualAblationTime_ == 0)
      {
        timeReturn_ = selectedTime_ <= requiredAblationTime_ 
          ? selectedTime_ 
          : selectedTime_ - requiredAblationTime_;
      }
      else
      {
        timeReturn_ = selectedTime_ <= actualAblationTime_ 
          ? selectedTime_ 
          : selectedTime_ - actualAblationTime_;
      }

      return timeReturn_.ToString(CultureInfo.InvariantCulture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
