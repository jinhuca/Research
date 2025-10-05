using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ConvertersNew
{
  internal class PlaybackAblationTimeVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedTime_ = System.Convert.ToInt32(values[0]); // CryoTherapyTime
      var requiredAblationTime_ = System.Convert.ToInt32(values[1]);  // RequiredAblationTime
      var systemState_ = (MessageStateId)values[2];  // SystemState
      var isInPlaybackMode_ = System.Convert.ToBoolean(values[3]);  // IsTreatmentNumberAndPlayBackVisible

      if(isInPlaybackMode_ && selectedTime_ <= requiredAblationTime_)
      {
        return Visibility.Visible;
      }

      if (systemState_ == MessageStateId.CAN_ID_STATE_TRANSITION ||
          systemState_ == MessageStateId.CAN_ID_STATE_ABLATION)
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
