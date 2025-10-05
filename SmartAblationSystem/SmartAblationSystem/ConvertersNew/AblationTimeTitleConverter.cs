using Communication;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class AblationTimeTitleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var isOnPlaybackMode_ = System.Convert.ToBoolean(values[0]);
      var state_ = SystemStateToStringConverter.Convert((CanBusMessageDefinition.MessageStateId)values[1]);
      var parameter_ = parameter.ToString();

      if(isOnPlaybackMode_)
        return Visibility.Hidden;

      switch(parameter_)
      {
        case UIConstants.AblationState when state_ == UIConstants.AblationState || state_ == UIConstants.TransitionState:
        case UIConstants.ThawingState when state_ == UIConstants.ThawingState:
          return Visibility.Visible;
        default:
          return Visibility.Hidden;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
