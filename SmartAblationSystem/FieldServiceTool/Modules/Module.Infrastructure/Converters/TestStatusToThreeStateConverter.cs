using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(TestStatus), typeof(bool?))]
	public class TestStatusToThreeStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is TestStatus status)
			{
				switch(status)
				{
					case TestStatus.NotStarted:
					case TestStatus.Unknown:
					case TestStatus.Inprogress:
					case TestStatus.Stopped:
					case TestStatus.Paused:
					case TestStatus.Retry:
						return false;
					case TestStatus.Passed:
					case TestStatus.Finished:
						return true;
					case TestStatus.Failed:
					case TestStatus.Aborted:
					default:
						return null;
				}
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch(value)
			{
				case null:
					return TestStatus.Failed;
				case bool state:
					return state ? TestStatus.Passed : TestStatus.NotStarted;
				default:
					return TestStatus.NotStarted;
			}
		}
	}
}
