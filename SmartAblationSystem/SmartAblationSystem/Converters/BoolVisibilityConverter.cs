using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
  public class BoolVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
	    if (value is string)
	    {
		    bool.TryParse(value.ToString(), out bool v_);
		    return v_ ? Visibility.Visible : Visibility.Hidden;
	    }
			else if (value is bool v_)
	    {
		    return v_ ? Visibility.Visible : Visibility.Hidden;
	    }
	    else
	    {
		    return Visibility.Collapsed;
	    }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
