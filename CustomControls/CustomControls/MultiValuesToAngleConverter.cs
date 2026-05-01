using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CustomControls;

public class MultiValuesToAngleConverter : IMultiValueConverter {
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
    // check the values validation
    var givenValue = (double)values[0];
    var givenMinValue = (double)values[1];
    var givenMaxValue = (double)values[2];
    // ...
    // Normalize and clamp to [0,1]
    var mappedValue = MapRange(givenValue, givenMinValue, givenMaxValue, 0, 1);

    var calculatedAngle = mappedValue * (ConverterDefinitions.MaxAngle - ConverterDefinitions.MinAngle);

    var valueAngle = (givenValue - givenMinValue) * (ConverterDefinitions.MaxAngle - ConverterDefinitions.MinAngle) / (givenMaxValue - givenMinValue)
      + ConverterDefinitions.MinAngle;
    return valueAngle;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  private static double MapRange(double value, double fromMin, double fromMax, double toMin, double toMax) {
    return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
  }
}

internal static class ConverterDefinitions {
  public const double MinAngle = -120.0;
  public const double MaxAngle = 120.0;
  //public const double MinValue = 0.0;
  //public const double MaxValue = 100.0;
}
