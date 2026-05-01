using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CustomControls;

public class MultipleValuesConverter : IMultiValueConverter {
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
    // check the values validation
    var givenValue = (double)values[0];
    var givenMinValue = (double)values[1];
    var givenMaxValue = (double)values[2];
    // ...

    // Normalize and clamp to [0,1]
    //double normalizedValue = (givenValue - givenMinValue) / (givenMaxValue - givenMinValue);
    //double mappedValue = Math.Max(Math.Min(1.0, normalizedValue), 0.0);
    //double result = MultipleValueConverterDefinitions.MinAngle + mappedValue * (MultipleValueConverterDefinitions.MaxAngle
    //  - MultipleValueConverterDefinitions.MinAngle);

    var vm = (givenValue - givenMinValue) * (MultipleValueConverterDefinitions.MaxAngle - MultipleValueConverterDefinitions.MinAngle) / (givenMaxValue - givenMinValue)
      + MultipleValueConverterDefinitions.MinAngle;

    return vm;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

internal static class MultipleValueConverterDefinitions {
  public const double MinAngle = -120.0;
  public const double MaxAngle = 120.0;
  public const double MinValue = 0.0;
  public const double MaxValue = 100.0;
}
