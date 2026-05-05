using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using static ResourceModule.Controls.Meter.Definitions;

namespace ResourceModule.Controls.Meter;

public class ValuesToDisplayConverter : IMultiValueConverter {
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
    if (values == null || values.Any(static v => v == DependencyProperty.UnsetValue))
      return Binding.DoNothing;

    // (1) cast the passed values to doubles
    double valuePassed_, minValuePassed_, maxValuePassed_;

    try {
      valuePassed_ = System.Convert.ToDouble(values[0]);
      minValuePassed_ = System.Convert.ToDouble(values[1]);
      maxValuePassed_ = System.Convert.ToDouble(values[2]);
    }
    catch (InvalidCastException ice) {
      Debug.WriteLine(ice.Message);
      return double.NaN;
    }

    // (2) check the passed values validation
    if (!IsValidInput(valuePassed_, minValuePassed_, maxValuePassed_)) {
      return double.NaN;
    }

    // (3) cast the passed value to Unit defined
    if (Enum.TryParse<Unit>(values[3].ToString(), out Unit unit_)) {
      //Debug.WriteLine($"Parsed unit: {unit_}");
    }
    else {
      //Debug.WriteLine($"Failed to parse unit from value: {values[3]}");
      unit_ = Unit.None; // Default to None if parsing fails
    }

    // (4) convert the validated value to display
    double calculatedValue_ = valuePassed_;
    switch (unit_) {
      case Unit.Percent:
        calculatedValue_ = (valuePassed_ - minValuePassed_) / (maxValuePassed_ - minValuePassed_) * 100;
        return Math.Round(calculatedValue_, 2).ToString(CultureInfo.InvariantCulture);
      case Unit.Absolute:
        calculatedValue_ = valuePassed_ - minValuePassed_;
        return Math.Round(calculatedValue_, 2).ToString(CultureInfo.InvariantCulture);
      default:
        calculatedValue_ = valuePassed_;
        return Math.Round(calculatedValue_, 2).ToString(CultureInfo.InvariantCulture);
    }
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }

  private bool IsValidInput(double value, double minValue, double maxValue) {
    return !(double.IsNaN(value) || double.IsNaN(minValue) || double.IsNaN(maxValue)
      || maxValue < minValue || value < minValue || value > maxValue);
  }
}
