using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Module.Infrastructure.StepStatus;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(StepStatus), typeof(SolidColorBrush))]
	public class StepStatusToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color defaultColor = (Color)ColorConverter.ConvertFromString("#FFA9A9A9");
			switch((StepStatus)value)
			{
				case InProgress:
				case FailedInProgress:
					return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF8C00"));
				case Processed:
				case Finished:
				case Failed:
				case Passed:
				case FailedFinished:
					return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00AFAF"));
				case Unknown:
				case NotStarted:
				default:
					return new SolidColorBrush(defaultColor);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
