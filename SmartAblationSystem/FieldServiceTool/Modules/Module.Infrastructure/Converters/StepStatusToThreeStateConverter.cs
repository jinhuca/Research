using System;
using System.Globalization;
using System.Windows.Data;
using static Module.Infrastructure.StepStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(StepStatus), typeof(bool?))]
	public class StepStatusToThreeStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is StepStatus status))
			{
				return false;
			}

			switch(status)
			{
				case Unknown:
				case NotStarted:
				case InProgress:
				case FailedInProgress:
					return false;
				case Processed:
				case Finished:
				case Failed:
				case Passed:
					return true;
				case FailedFinished:
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch(value)
			{
				case null:
					return Failed;
				default:
					return NotStarted;
			}
		}
	}
}
