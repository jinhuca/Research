using Communication;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  public class SystemStateToBalloonVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {

      Visibility result_ = Visibility.Hidden;
      var stateId_ = (CanBusMessageDefinition.MessageStateId)value[0];
      var isPlaybackMode = (bool)value[1];
      
      if (isPlaybackMode) return Visibility.Visible;

      switch (stateId_)
      {
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
          result_ = Visibility.Visible;
          break;
        default:
          result_ = Visibility.Collapsed;
          break;
      }

      return result_;
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
