using Communication;
using SmartAblationSystem.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;


namespace SmartAblationSystem.ConvertersNew
{
  internal class AblationStateToEnabledConverter: IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      
      if (!(values[0] is bool isThawingTemperatureSetPointReached_))
      {
        throw new ArgumentException("Invalid argument");
      }

      if (!(values[1] is CanBusMessageDefinition.MessageStateId id_))
      {
        throw new ArgumentException("Invalid argument");
      }

      if (!(values[2] is bool isInDASBalloonTransition))
      {
        throw new ArgumentException("Invalid argument");
      }

      if (!(parameter is string parameter_))
      {
        throw new ArgumentException("Invalid argument");
      }

      switch (parameter_)
        {
          case ActionConstants.Ablate:
            if (id_ == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && !CommonViewModel.Current.DeflateAfterThaw)
            {
              if (isThawingTemperatureSetPointReached_)
              {
                return true;
              }

            }
            return (id_ == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION) && !isInDASBalloonTransition;
          default:
            return false;
        }
      
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
