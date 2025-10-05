using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(TestStatus), typeof(SolidColorBrush))]
	public class TestStatusToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var defaultColor = (Color)ColorConverter.ConvertFromString("#FFA9A9A9");
			var activeColor = (Color)ColorConverter.ConvertFromString("#FFFF8C00");
			var pausedColor = (Color)ColorConverter.ConvertFromString("#FFFF8C00");
			var finishedColor = (Color)ColorConverter.ConvertFromString("#FF00AFAF");
			var stoppedColor = (Color)ColorConverter.ConvertFromString("#FFFA2222");
			var failedColor = (Color)ColorConverter.ConvertFromString("#FFFF0000");

			switch ((TestStatus)value)
			{
				case TestStatus.Inprogress:
					return activeColor;
				case TestStatus.paused:
					return pausedColor;
				case TestStatus.Stopped:
					return stoppedColor;
				case TestStatus.Failed:
					return failedColor;
				case TestStatus.Passed:
				case TestStatus.Finished:
					return finishedColor;
				case TestStatus.NotStarted:
				case TestStatus.Unknown:
				default:
					return defaultColor;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
