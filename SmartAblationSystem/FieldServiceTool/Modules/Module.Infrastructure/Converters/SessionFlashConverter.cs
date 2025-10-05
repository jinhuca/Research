using System;
using System.Globalization;
using System.Windows.Data;
using static Module.Infrastructure.SessionStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(SessionStatus), typeof(bool))]
	public class SessionFlashConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sessionStatus = (SessionStatus)value;
			switch (sessionStatus)
			{
				case Finishing:
				case Finished:
				case Ready:
				case Starting:
				case Started:
				case Resuming:
				case Paused:
				case Pausing:
				case Resumed:
				case Stopped:
				case Stopping:
					return true;
				case SessionStatus.Exception:
				case Unknown:
				default:
					return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
