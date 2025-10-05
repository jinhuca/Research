using System;
using System.Globalization;
using System.Windows.Data;
using static Module.Infrastructure.SessionStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(SessionStatus), typeof(string))]
	public class SessionStatusToStopTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sessionStatus = (SessionStatus)value;
			var param = (string)parameter;
			string result = string.Empty;

			switch (sessionStatus)
			{
				case Unknown:
				case Ready:
				case Started:
				case Paused:
				case Stopped:
					result = AppCommandNames.StopCommandName;
					break;
				case Finished:
					result = AppCommandNames.FinishCommandName;
					break;
				case Resumed:
				case Stopping:
					break;
				case Starting:
					break;
				case Pausing:
					break;
				case Resuming:
					break;
				case Finishing:
					break;
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
