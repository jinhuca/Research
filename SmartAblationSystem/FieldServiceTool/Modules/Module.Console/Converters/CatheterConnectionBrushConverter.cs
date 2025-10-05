using Module.Infrastructure;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Module.Console.Converters
{
	[ValueConversion(typeof(CatheterStatus), typeof(SolidColorBrush))]
	public class CatheterConnectionBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();
			var disconnectedBrush = brushConverter.ConvertFromString("#FFE0E0E0");
			/*var connectedBrush = brushConverter.ConvertFromString("#FF11FF11");
			var readyBrush = brushConverter.ConvertFromString("#FF00AFAF");

			switch ((CatheterStatus)value)
			{
				case CatheterStatus.Disconnected:
					return disconnectedBrush;
				case CatheterStatus.Connected:
					return connectedBrush;
				case CatheterStatus.Ready:
					return readyBrush;
				default:
					return disconnectedBrush;
			}*/
			return disconnectedBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
