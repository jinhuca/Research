using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(TestStatus), typeof(bool))]
	public class TestStatusToEnabledBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is TestStatus status)) return false;
			switch(status)
			{
				case TestStatus.Inprogress:
				case TestStatus.Paused:
				case TestStatus.Passed:
				case TestStatus.Failed:
				case TestStatus.Finished:
				case TestStatus.Stopped:
				case TestStatus.Aborted:
				case TestStatus.Retry:
					return true;
				case TestStatus.NotStarted:
				case TestStatus.Unknown:
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
