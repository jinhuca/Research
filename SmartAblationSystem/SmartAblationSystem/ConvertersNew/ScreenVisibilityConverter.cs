using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.ConvertersNew
{
	internal class ScreenVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (string)value == "Home" 
				? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF222228")) 
				: null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
