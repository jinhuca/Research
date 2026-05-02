using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ResourceModule.Controls;

[ValueConversion(typeof(Unit), typeof(string))]
public class UnitToStringConverter : IValueConverter {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    var abs = Definitions.AbsoluteString ?? string.Empty;
    var pct = Definitions.PercentageString ?? string.Empty;
    var none = Definitions.NoneString ?? string.Empty;

    var input = value?.ToString() ?? string.Empty;
    if (Enum.TryParse<Unit>(input, out Unit unit_)) {
      return unit_ switch {
        Unit.Percent => pct,
        Unit.Absolute => parameter?.ToString() ?? abs,
        Unit.None => none,
        _ => string.Empty,
      };
    }
    return abs;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotSupportedException();
  }
}