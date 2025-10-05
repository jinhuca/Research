using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  [ValueConversion(typeof(string), typeof(SolidColorBrush))]
  internal class ConsoleStateToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var parameter_ = (string)parameter;
      if(parameter_ == null)
      {
        throw new ArgumentNullException(nameof(value));
      }

      var inactiveSolidBrush_ = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF57585A"));
      var activeSolidBrush_ = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9333"));
      var activeIdleBrush_ = new SolidColorBrush(Colors.Yellow);
      var activeReadyBrush_ = new SolidColorBrush(Colors.LimeGreen);

      switch (parameter_)
      {
        case "IDLE":
          return (int)value == (int)MessageStateId.CAN_ID_STATE_IDLE ? activeIdleBrush_ : inactiveSolidBrush_;
        case "READY":
          return (int)value == (int)MessageStateId.CAN_ID_STATE_READY ? activeReadyBrush_ : inactiveSolidBrush_;
        case "INFLATION":
          return (int)value == (int)MessageStateId.CAN_ID_STATE_INFLATION ? activeSolidBrush_ : inactiveSolidBrush_;
        case "ABLATION":
          return (int)value == (int)MessageStateId.CAN_ID_STATE_ABLATION ||
                 (int)value == (int)MessageStateId.CAN_ID_STATE_TRANSITION
            ? activeSolidBrush_
            : inactiveSolidBrush_;
        case "THAWING":
          return (int)value == (int)MessageStateId.CAN_ID_STATE_THAWING ? activeSolidBrush_ : inactiveSolidBrush_;
        default:
          return inactiveSolidBrush_;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
