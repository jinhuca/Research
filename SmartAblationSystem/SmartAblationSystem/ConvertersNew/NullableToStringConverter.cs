using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DataAccessLayer;
using SmartAblationSystem.Models;
using Type = System.Type;

namespace SmartAblationSystem.ConvertersNew
{
  /// <summary>
  /// MultiValue Converter class for converting a nullable value to string representation.
  /// </summary>
  internal class NullableToStringConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var navigatedRecord_ = values[0];
      var sensorValue_ = values[1];

      if(navigatedRecord_ is null && parameter?.ToString() != "IsDataEdited")
      {
        return UIConstants.DoubleDash;
      }

      if(parameter?.ToString() == "DiaphragmMovement")
      {
        var diaphragmMovement_ = System.Convert.ToInt32(values[1]);

        return diaphragmMovement_ > 100 || diaphragmMovement_ < 0
          ? UIConstants.DoubleDash
          : diaphragmMovement_.ToString(CultureInfo.InvariantCulture);
      }

      if(parameter?.ToString() == "EsophagusTemperature")
      {
        var esophagusTemperature_ = (int)Math.Round(System.Convert.ToDouble(values[1]));

        return esophagusTemperature_ < 0 || esophagusTemperature_ > 50
          ? UIConstants.DoubleDash
          : esophagusTemperature_.ToString(CultureInfo.InvariantCulture);
      }

      if(parameter?.ToString() == "VeinIsolationDuration" || parameter?.ToString() == "TemperatureAtTTI" || parameter?.ToString() == "TimeSinceTTI")
      {
        var ttiDuration_ = System.Convert.ToInt32(sensorValue_);
        return ttiDuration_ == 0
          ? UIConstants.DoubleDash
          : ttiDuration_.ToString(CultureInfo.InvariantCulture);
      }

      if(parameter?.ToString() == "IsDataEdited")
      {
        if (values[1] is Procedure procedure_)
        {
          return procedure_.IsDataEdited
            ? Visibility.Visible
            : Visibility.Hidden;
        }

        return Visibility.Hidden;
      }

      if (parameter?.ToString() == "AblationTime")
      {
        if (values[1] is Visibility visibility_)
        {
          return visibility_;
        }
        return Visibility.Hidden;
      }

      if (parameter?.ToString() == "IBP" || parameter?.ToString() == "OBP")
      {
        if (TipBalloonPressureSelection.TipPressureSelected)
        {
          return ((int)sensorValue_).ToString("0.0");
        }
        return UIConstants.DoubleDash;
      }

      if (parameter?.ToString() == "BalloonSize")
      {
        return sensorValue_;
      }

      if (parameter?.ToString() == "AblationSite")
      {
        return sensorValue_;
      }

      switch(values[1])
      {
        case Int32 intValue_:
          return System.Convert.ToInt32(intValue_).ToString(CultureInfo.InvariantCulture);
        case double doubleValue_:
          return ((int)System.Convert.ToDouble(doubleValue_)).ToString(CultureInfo.InvariantCulture);
        case string stringValue_:
          return System.Convert.ToInt32(values[1]).ToString(CultureInfo.InvariantCulture);
        default:
          throw new InvalidCastException();
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
