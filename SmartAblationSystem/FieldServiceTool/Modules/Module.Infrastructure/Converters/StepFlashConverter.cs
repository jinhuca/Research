using System;
using System.Globalization;
using System.Windows.Data;
using static Module.Infrastructure.StepStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(StepStatus), typeof(bool))]
	public class StepFlashConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stepStatus = (StepStatus)value;
			switch(stepStatus)
			{
				case InProgress:
					return true;
				case FailedInProgress:
				case Unknown:
				case NotStarted:
				case Processed:
				case Finished:
				case Failed:
				case Passed:
				case FailedFinished:
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
