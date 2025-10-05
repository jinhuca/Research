using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class VacuumFlashConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isOffOn = (bool)value;
			switch (isOffOn)
			{
				case false:
				case true:
					return true;
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
