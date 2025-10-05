using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(SessionStatus), typeof(SolidColorBrush))]
	public class SessionStatusToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();

			var defaultBrush = brushConverter.ConvertFromString("#FFE0E0E0");
			/*var readyBrush = brushConverter.ConvertFromString("#FF00AFAF");
			var startedBrush = brushConverter.ConvertFromString("#FFFF0000");
			var pauseBrush = brushConverter.ConvertFromString("#FFFFAA00");
			var finishBrush = brushConverter.ConvertFromString("#FF99FF33");
			var stoppedBrush = brushConverter.ConvertFromString("#FFF87116");
			var stoppingBrush = brushConverter.ConvertFromString("#FFEA2222");

			switch ((SessionStatus)value)
			{
				case SessionStatus.Ready:
					return readyBrush;
				case SessionStatus.Started:
					return startedBrush;
				case SessionStatus.Paused:
					return pauseBrush;
				case SessionStatus.Stopped:
					return stoppedBrush;
				case SessionStatus.Stopping:
					return stoppingBrush;
				case SessionStatus.Finished:
					return finishBrush;
				case SessionStatus.Unknown:
				case SessionStatus.Resumed:
				default:
					return defaultBrush;
			}*/
			return defaultBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
