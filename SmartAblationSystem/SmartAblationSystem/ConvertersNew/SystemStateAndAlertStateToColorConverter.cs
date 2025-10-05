using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;

namespace SmartAblationSystem.ConvertersNew
{
  using System.Drawing;

  using ColorConverter = System.Windows.Media.ColorConverter;

  internal class SystemStateAndAlertStateToColorConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(!(values[0] is MessageStateId id_))
        throw new ArgumentNullException(nameof(values));

      var isDMSAmplitudeThresholdReached = (bool)values[1];
      var isEsophagusTemperatureConditionAlertsMeet = (bool)values[2];

      if (isEsophagusTemperatureConditionAlertsMeet || isDMSAmplitudeThresholdReached)
      {
        return new ColorConverter().ConvertFrom(DefinedStateColors.ExceptionStateColor);
      }
      
      var stateToColorConverter = new SystemStateToColorConverter();
      return stateToColorConverter.Convert(values[0], typeof(Color), parameter, culture); 
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new [] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
    }
  }
}
