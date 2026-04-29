using System.Globalization;
using System.Windows.Data;

namespace CustomControls;

[ValueConversion(typeof(double), typeof(double))]
public class ValueToAngleConverter : IValueConverter {
  private const double MinAngle = -120.0;
  private const double MaxAngle = 120.0;
  private const double MinValue = 0.0;
  private const double MaxValue = 100.0;

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    double v = 0.0;

    switch (value) {
      case double d:
        v = d;
        break;
      case float f:
        v = f;
        break;
      case int i:
        v = i;
        break;
      case null:
        v = MinValue;
        break;
      default:
        double.TryParse(System.Convert.ToString(value, culture), NumberStyles.Any, culture, out v);
        break;
    }

    // Normalize and clamp to [0,1]
    double t = (v - MinValue) / (MaxValue - MinValue);
    t = Math.Max(0.0, Math.Min(1.0, t));

    // Map to angle range
    return MinAngle + t * (MaxAngle - MinAngle);
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotSupportedException();
  }
}
