using System;
using System.Globalization;
using System.Windows.Data;
using static Module.Infrastructure.SessionStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(SessionStatus), typeof(string))]
	public class SessionStatusToPauseTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sessionStatus = (SessionStatus)value;
			var param = (string)parameter;
			string result = string.Empty;

			switch(sessionStatus)
			{
				case Pausing:
				case Paused:
					if(param == AppCommandParameterName.PauseCommandName)
					{
						result = AppCommandNames.ResumeCommandName;
					}
					break;
				case Started:
				case Stopped:
				case Unknown:
				case Ready:
				case Finished:
				case Resumed:
				case Stopping:
				case Starting:
				case Resuming:
				case Finishing:
				case SessionStatus.Exception:
					if(param == AppCommandParameterName.PauseCommandName)
					{
						result = AppCommandNames.PauseCommandName;
					}
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
