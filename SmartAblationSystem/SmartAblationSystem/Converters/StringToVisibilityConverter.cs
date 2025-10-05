using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class StringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Hidden : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}