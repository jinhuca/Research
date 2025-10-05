using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Communication;

namespace SmartAblationSystem.ConvertersNew
{
  internal class SystemStateToVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null || values.Length < 2)
      {
        throw new ArgumentException(nameof(values));
      }
      var isInPlaybackMode = ((bool?)values[1]) ?? false;
      if (isInPlaybackMode)
      {
        return Visibility.Collapsed;
      }

      var currentState_ = SystemStateToStringConverter.Convert((CanBusMessageDefinition.MessageStateId)values[0]);

      var parameter_ = (string)parameter;

      var visibility = Visibility.Collapsed;

      switch (currentState_)
      {
        case UIConstants.TransitionState:
        case UIConstants.AblationState:
          visibility = parameter_ == UIConstants.AblationState ? Visibility.Visible : Visibility.Collapsed;
          break;
        case UIConstants.ThawingState:
          visibility = parameter_ == currentState_ ? Visibility.Visible : Visibility.Collapsed;
          break;
      }

      return visibility;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
