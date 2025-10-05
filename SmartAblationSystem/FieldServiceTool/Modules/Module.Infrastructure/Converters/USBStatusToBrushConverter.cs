using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
	public class USBStatusToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();
			//var usbOnBrush = brushConverter.ConvertFromString("#FF99EE33");
			var usbOffBrush = brushConverter.ConvertFromString("#FFE0E0E0");
			return usbOffBrush;
			//return bool.TryParse(value.ToString(), out var boolValue) ? boolValue ? usbOnBrush : usbOffBrush : usbOffBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
