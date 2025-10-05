using Module.Infrastructure;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Module.Infrastructure.SessionStatus;

namespace Module.TestProcess
{
	[ValueConversion(typeof(SessionStatus), typeof(Visibility))]
	public class SessionVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result_ = Visibility.Collapsed;
			switch((SessionStatus)value)
			{
				case Unknown:
					break;
				case Ready:
					result_ = Visibility.Visible;
					break;
				case Starting:
					result_ = Visibility.Visible;
					break;
				case Started:
					result_ = Visibility.Collapsed;
					break;
				case Pausing:
					result_ = Visibility.Visible;
					break;
				case Paused:
					break;
				case Resuming:
					result_ = Visibility.Visible;
					break;
				case Resumed:
					break;
				case Stopping:
					result_ = Visibility.Visible;
					break;
				case Stopped:
					break;
				case Finishing:
					break;
				case Finished:
					break;
				case SessionStatus.Exception:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(value), value, null);
			}

			return result_;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
