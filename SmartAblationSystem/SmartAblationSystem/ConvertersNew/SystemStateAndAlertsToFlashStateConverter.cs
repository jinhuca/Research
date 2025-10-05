
namespace SmartAblationSystem.ConvertersNew
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  
  using Communication;
  using static Communication.CanBusMessageDefinition;

  internal class SystemStateAndAlertsToFlashStateConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(!(values[0] is CanBusMessageDefinition.MessageStateId id_))
        throw new ArgumentNullException(nameof(values));

      var systemState = (CanBusMessageDefinition.MessageStateId)values[0];
      var isDMSAmplitudeThresholdReached = (bool)values[1];
      var isEsophagusTemperatureConditionAlertsMeet = (bool)values[2];
      var isEnableEnhancedAudio = (bool)values[3];

      // flash when dmsAmplitudeThresholdReached and in Ablation State 
      var result = isDMSAmplitudeThresholdReached
                        && (systemState == MessageStateId.CAN_ID_STATE_ABLATION
                        || systemState == MessageStateId.CAN_ID_STATE_TRANSITION);

      if (result)
      {
        return result;
      }

      result = isEsophagusTemperatureConditionAlertsMeet 
               && (systemState == MessageStateId.CAN_ID_STATE_INFLATION
                   || systemState == MessageStateId.CAN_ID_STATE_ABLATION
                   || systemState == MessageStateId.CAN_ID_STATE_TRANSITION
                   || systemState == MessageStateId.CAN_ID_STATE_THAWING);

      if (result)
      {
        return result;
      }

      var stateToFlashConverter = new FlashStateConverter();
      return isEnableEnhancedAudio && (bool)stateToFlashConverter.Convert(values[0], typeof(bool), parameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new [] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
    }
  }
}
