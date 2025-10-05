using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class ExportStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool b_ ? (object)(b_ ? Visibility.Visible : Visibility.Hidden) : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
