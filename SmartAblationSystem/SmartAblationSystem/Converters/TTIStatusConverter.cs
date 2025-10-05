using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  public class TTIStatusConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var result_ = "";

      var isTTISet_ = System.Convert.ToInt32(values[0]) != 0;
      var systemState_ = System.Convert.ToInt32(values[1]);
      var playbackStatus = System.Convert.ToBoolean(values[2]);

      switch(systemState_)
      {
        case (int)MessageStateId.CAN_ID_STATE_IDLE:
        case (int)MessageStateId.CAN_ID_STATE_READY:
        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
        case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
        case (int)MessageStateId.CAN_ID_STATE_THAWING:
          result_ = "";
          break;
        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
        case (int)MessageStateId.CAN_ID_STATE_ABLATION:
          if(isTTISet_ && !playbackStatus)
          {
            result_ = "HOLD TO\n   RESET";
          }
          break;
        default:
          result_ = "";
          break;
      }

      return result_;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}