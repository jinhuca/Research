using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  internal class CryoTherapyViewConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var state_ = values[0]?.ToString();
      var screen_ = values[1]?.ToString();

      if(screen_ != "Cryo Therapy")
      {
        return Visibility.Collapsed;
      }

      switch(state_)
      {
        case "CAN_ID_STATE_UNKNOWN":
        case "CAN_ID_STATE_IDLE":
        case "CAN_ID_STATE_READY":
          return Visibility.Visible;
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