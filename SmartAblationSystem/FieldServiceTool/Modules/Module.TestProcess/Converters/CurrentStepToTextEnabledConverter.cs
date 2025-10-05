using Module.TestProcess.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.TestProcess.Converters
{
	[ValueConversion(typeof(ITestViewModel), typeof(bool))]
	public class CurrentStepToTextEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null) return false;
			var activeTestViewModel = (ITestViewModel)value;
			var result = true;

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
