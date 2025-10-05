using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class MultipleBoolValueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
      return values.All(value => value is bool) ? (object)values.All(obj => (bool)obj) : null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}