using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class ExportStateMessageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool state_ ? state_ ? "Cancel" : "OK" : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
