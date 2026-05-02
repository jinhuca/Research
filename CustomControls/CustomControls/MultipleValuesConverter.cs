using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CustomControls;

public class MultipleValuesConverter : IMultiValueConverter {
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
    // (1) cast the input values to double
    double valuePassed = System.Convert.ToDouble(values[0]);
    double minValuePassed = System.Convert.ToDouble(values[1]);
    double maxValuePassed = System.Convert.ToDouble(values[2]);
    
    // (2) check the values validation
    if (!IsValidInput(valuePassed, minValuePassed, maxValuePassed)) {
      return DependencyProperty.UnsetValue;
    }

    // (3)Normalize and clamp to [0,1]
    //double normalizedValue = (givenValue - givenMinValue) / (givenMaxValue - givenMinValue);
    //double mappedValue = Math.Max(Math.Min(1.0, normalizedValue), 0.0);
    //double result = MultipleValueConverterDefinitions.MinAngle + mappedValue * (MultipleValueConverterDefinitions.MaxAngle
    //  - MultipleValueConverterDefinitions.MinAngle);

    var vm = (valuePassed - minValuePassed) * (MultipleValueConverterDefinitions.MaxAngle - MultipleValueConverterDefinitions.MinAngle) / (maxValuePassed - minValuePassed)
      + MultipleValueConverterDefinitions.MinAngle;

    return vm;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  private bool IsValidInput(double value, double minValue, double maxValue) {
    return !(double.IsNaN(value) || double.IsNaN(minValue) || double.IsNaN(maxValue) || maxValue < minValue);
  }
}

internal static class MultipleValueConverterDefinitions {
  public const double MinAngle = -120.0;
  public const double MaxAngle = 120.0;
  public const double MinValue = 0.0;
  public const double MaxValue = 100.0;
}
