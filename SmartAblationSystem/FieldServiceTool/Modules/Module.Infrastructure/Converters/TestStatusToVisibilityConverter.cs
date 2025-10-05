using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(TestStatus), typeof(Visibility))]
	public class TestStatusToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch((TestStatus)value)
			{
				case TestStatus.Inprogress:
				case TestStatus.Retry:
					return Visibility.Visible;
				case TestStatus.Paused:
				case TestStatus.Failed:
				case TestStatus.Stopped:
				case TestStatus.Aborted:
				case TestStatus.Passed:
				case TestStatus.NotStarted:
				case TestStatus.Unknown:
				case TestStatus.Finished:
					return Visibility.Collapsed;
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
