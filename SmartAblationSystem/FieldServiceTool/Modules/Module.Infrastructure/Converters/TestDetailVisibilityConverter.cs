using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(TestStatus), typeof(Visibility))]
	public class TestDetailVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch((TestStatus)value)
			{
				case TestStatus.Aborted:
				case TestStatus.NotStarted:
				case TestStatus.Inprogress:
				case TestStatus.Stopped:
				case TestStatus.Paused:
				case TestStatus.Passed:
				case TestStatus.Failed:
				case TestStatus.Finished:
				case TestStatus.Retry:
					return Visibility.Visible;
				case TestStatus.Unknown:
				default:
					return Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
